terraform {
  required_providers {
    aws = {
      source = "hashicorp/aws"
      version = "4.45.0" # Specify AWS provider version to ensure compatibility
    }
  }
}

provider "aws" {
  region  = "eu-north-1" # AWS region where resources will be created
}

# Create an ECR repository for the Docker image
resource "aws_ecr_repository" "checkout_ecr_repo" {
  name = "checkoutapiv2" # Name of the ECR repository
}

# Create an ECS Cluster
resource "aws_ecs_cluster" "my_cluster" {
  name = "checkoutapiv2" # Name of the ECS cluster
}

# Define ECS Task Definition for the service
resource "aws_ecs_task_definition" "checkout_task" {
  family                   = "checkout-task" # Logical name of the task definition
  container_definitions    = jsonencode([{
      name      = "checkout-task" # Name of the container
      image     = aws_ecr_repository.checkout_ecr_repo.repository_url # Docker image from ECR
      essential = true # Ensure the container is essential for the task
      portMappings = [{
        containerPort = 8080
        hostPort      = 8080 # Port mapping for container and host
      }],
      memory = 512
      cpu    = 256
      environment = [ # Define environment variables for the container
        {
          name  = "DB_HOST",
          value = aws_db_instance.postgres_db.address
        },
        {
          name  = "DB_PORT",
          value = tostring(aws_db_instance.postgres_db.port)
        },
        {
          name  = "DB_USER",
          value = "postgres"
        },
        {
          name  = "DB_PASSWORD",
          value = "admin12345678"
        },
        {
          name  = "DB_NAME",
          value = "checkout"
        }
      ],
      logConfiguration = { # Log configuration for CloudWatch
        logDriver = "awslogs",
        options = {
          awslogs-group         = "/ecs/checkoutapiv2"
          awslogs-region        = "eu-north-1"
          awslogs-stream-prefix = "ecs"
        }
      }
    }]
  )
  requires_compatibilities = ["FARGATE"] # Use Fargate for serverless deployment
  network_mode             = "awsvpc" # Use AWS VPC network mode
  memory                   = 512
  cpu                      = 256
  execution_role_arn       = aws_iam_role.ecsTaskExecutionRole.arn
}

# Create an IAM role for ECS Task Execution
resource "aws_iam_role" "ecsTaskExecutionRole" {
  name               = "ecsTaskExecutionRole"
  assume_role_policy = "${data.aws_iam_policy_document.assume_role_policy.json}" # Policy to allow ECS tasks to assume the role
}

# Define policy for ECS Task Execution Role
data "aws_iam_policy_document" "assume_role_policy" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"] # ECS tasks service
    }
  }
}

# Attach AmazonECSTaskExecutionRolePolicy to the ECS task role
resource "aws_iam_role_policy_attachment" "ecsTaskExecutionRole_policy" {
  role       = "${aws_iam_role.ecsTaskExecutionRole.name}"
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

# Reference to default VPC for AWS resources
resource "aws_default_vpc" "default_vpc" {
}

# Reference to default subnets in specific availability zones
resource "aws_default_subnet" "default_subnet_a" {
  availability_zone = "eu-north-1a"
}

resource "aws_default_subnet" "default_subnet_b" {
  availability_zone = "eu-north-1b"
}

# Create an Application Load Balancer
resource "aws_alb" "application_load_balancer" {
  name               = "load-balancer-checkout"
  load_balancer_type = "application"
  subnets = [
    "${aws_default_subnet.default_subnet_a.id}",
    "${aws_default_subnet.default_subnet_b.id}"
  ]
  security_groups = ["${aws_security_group.load_balancer_security_group.id}"]
}

# Create a security group for the Load Balancer
resource "aws_security_group" "load_balancer_security_group" {
  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"] # Allow HTTP traffic from anywhere
  }

  ingress {
    from_port   = 8080
    to_port     = 8080
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"] # Allow traffic on port 8080 from anywhere
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"] # Allow outbound traffic to any destination
  }
}

# Create Target Group for Load Balancer
resource "aws_lb_target_group" "target_group" {
  name        = "target-group"
  port        = 8080
  protocol    = "HTTP"
  target_type = "ip"
  vpc_id      = "${aws_default_vpc.default_vpc.id}"

  health_check { # Health check configuration
    path                = "/health"
    interval            = 30
    timeout             = 5
    healthy_threshold   = 2
    unhealthy_threshold = 2
  }
}

# Create Listener for Load Balancer to forward requests to Target Group
resource "aws_lb_listener" "listener" {
  load_balancer_arn = "${aws_alb.application_load_balancer.arn}"
  port              = "8080"
  protocol          = "HTTP"
  default_action {
    type             = "forward"
    target_group_arn = "${aws_lb_target_group.target_group.arn}"
  }
}

# Create ECS Service to run the task
resource "aws_ecs_service" "checkout_service" {
  name            = "checkout-service"
  cluster         = "${aws_ecs_cluster.my_cluster.id}"
  task_definition = "${aws_ecs_task_definition.checkout_task.arn}"
  launch_type     = "FARGATE"
  desired_count   = 3 # Number of desired running tasks

  load_balancer {
    target_group_arn = "${aws_lb_target_group.target_group.arn}"
    container_name   = "${aws_ecs_task_definition.checkout_task.family}"
    container_port   = 8080
  }

  network_configuration {
    subnets          = [aws_default_subnet.default_subnet_a.id, aws_default_subnet.default_subnet_b.id]
    assign_public_ip = true
    security_groups  = [aws_security_group.service_security_group.id]
  }
}

# Create CloudWatch Log Group for ECS logs
resource "aws_cloudwatch_log_group" "ecs_log_group" {
  name              = "/ecs/checkoutapiv2"
  retention_in_days = 7 # Retain logs for 7 days
}

# Create Security Group for ECS Service
resource "aws_security_group" "service_security_group" {
  ingress {
    from_port = 0
    to_port   = 0
    protocol  = "-1"
    security_groups = ["${aws_security_group.load_balancer_security_group.id}"] # Allow traffic from Load Balancer
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"] # Allow outbound traffic to any destination
  }
}

# Create PostgreSQL Database
resource "aws_db_instance" "postgres_db" {
  allocated_storage    = 20
  engine               = "postgres"
  instance_class       = "db.t3.micro"
  username             = "postgres"
  password             = "admin12345678"
  publicly_accessible  = true
  skip_final_snapshot  = true
  vpc_security_group_ids = [aws_security_group.db_security_group.id]
}

# Create Security Group for PostgreSQL
resource "aws_security_group" "db_security_group" {
  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    security_groups = ["${aws_security_group.service_security_group.id}"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Output Load Balancer URL
output "app_url" {
  value = aws_alb.application_load_balancer.dns_name
}
