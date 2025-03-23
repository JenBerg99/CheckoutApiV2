# Create ECR Repository
resource "aws_ecr_repository" "checkoutapiv2" {
  name         = "checkoutapiv2"
  force_delete = true
}

# Create ECS Cluster
resource "aws_ecs_cluster" "checkout_cluster" {
  name = "checkout-cluster"
}

# Create Task Definition
resource "aws_ecs_task_definition" "checkout_task" {
  family                   = "checkout-api-task"
  container_definitions    = <<DEFINITION
  [
    {
      "name": "checkoutapiv2",
      "image": "${aws_ecr_repository.checkoutapiv2.repository_url}:latest",
      "essential": true,
      "portMappings": [
        {
          "containerPort": 8080,
          "hostPort": 8080,
          "protocol": "tcp"
        },
        {
          "containerPort": 8081,
          "hostPort": 8081,
          "protocol": "tcp"
        }
      ],
      "memory": 512,
      "cpu": 256,
      "environment": [
        {
          "name": "DB_HOST",
          "value": "${aws_db_instance.checkout_db.address}"
        },
        {
          "name": "DB_PORT",
          "value": "5432"
        },
        {
          "name": "DB_NAME",
          "value": "checkout"
        },
        {
          "name": "DB_USER",
          "value": "postgres"
        },
        {
          "name": "DB_PASSWORD",
          "value": "admin12345678"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/checkout-api-service",
          "awslogs-region": "eu-north-1",
          "awslogs-stream-prefix": "ecs"
        }
      }
    }
  ]
  DEFINITION

  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  memory                   = 512
  cpu                      = 256
  execution_role_arn       = data.aws_iam_role.ecsTaskExecutionRole.arn
}

# Create CloudWatch Log Group
resource "aws_cloudwatch_log_group" "checkout_api_logs" {
  name              = "/ecs/checkout-api-service"
  retention_in_days = 7
}

# Create IAM Role for ECS Task Execution
data "aws_iam_role" "ecsTaskExecutionRole" {
  name = "ecsTaskExecutionRole"
}

data "aws_iam_policy_document" "assume_role_policy" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }
}

# Attach ECS Task Execution Policy to Role
resource "aws_iam_role_policy_attachment" "ecsTaskExecutionRole_policy" {
  role       = data.aws_iam_role.ecsTaskExecutionRole.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

# Create VPC
resource "aws_vpc" "main_vpc" {
  cidr_block = "10.1.0.0/16"

  enable_dns_support   = true
  enable_dns_hostnames = true
}

# Create Subnets
resource "aws_subnet" "subnet_a" {
  vpc_id                  = aws_vpc.main_vpc.id
  cidr_block              = "10.1.1.0/24"
  availability_zone       = "eu-north-1b"
  map_public_ip_on_launch = true
}

resource "aws_subnet" "subnet_b" {
  vpc_id                  = aws_vpc.main_vpc.id
  cidr_block              = "10.1.2.0/24"
  availability_zone       = "eu-north-1c"
  map_public_ip_on_launch = true
}

# Create Subnet in eu-north-1a
resource "aws_subnet" "subnet_c" {
  vpc_id                  = aws_vpc.main_vpc.id
  cidr_block              = "10.1.3.0/24"
  availability_zone       = "eu-north-1a"
  map_public_ip_on_launch = true
}

# Associate the new subnet with the route table
resource "aws_route_table_association" "subnet_c_association" {
  subnet_id      = aws_subnet.subnet_c.id
  route_table_id = aws_route_table.route_table.id
}


# Create Internet Gateway
resource "aws_internet_gateway" "igw" {
  vpc_id = aws_vpc.main_vpc.id
}

# Create Route Table
resource "aws_route_table" "route_table" {
  vpc_id = aws_vpc.main_vpc.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.igw.id
  }
}

# Associate Route Table with Subnets
resource "aws_route_table_association" "subnet_a_association" {
  subnet_id      = aws_subnet.subnet_a.id
  route_table_id = aws_route_table.route_table.id
}

resource "aws_route_table_association" "subnet_b_association" {
  subnet_id      = aws_subnet.subnet_b.id
  route_table_id = aws_route_table.route_table.id
}

# Create Security Group for Load Balancer
resource "aws_security_group" "load_balancer_security_group" {
  name_prefix = "load-balancer-sg"
  description = "Security group for load balancer"
  vpc_id      = aws_vpc.main_vpc.id

  ingress {
    from_port   = 8080
    to_port     = 8080
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port   = 8081
    to_port     = 8081
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Create Application Load Balancer
resource "aws_alb" "application_load_balancer" {
  name               = "load-balancer-dev"
  load_balancer_type = "application"
  subnets            = [aws_subnet.subnet_a.id, aws_subnet.subnet_b.id]
  security_groups    = [aws_security_group.load_balancer_security_group.id]
}

# Create Load Balancer Target Group
resource "random_id" "tg_id" {
  byte_length = 4
}

resource "aws_lb_target_group" "target_group" {
  name        = "target-group-${random_id.tg_id.hex}"
  port        = 8080
  protocol    = "HTTP"
  target_type = "ip"
  vpc_id      = aws_vpc.main_vpc.id

  health_check {
    path                = "/health"
    interval            = 10
    timeout             = 5
    healthy_threshold   = 2
    unhealthy_threshold = 2
  }
}

# Create Load Balancer Listener
resource "aws_lb_listener" "listener_8080" {
  load_balancer_arn = aws_alb.application_load_balancer.arn
  port              = "8080"
  protocol          = "HTTP"
  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.target_group.arn
  }
}

# Create ECS Service
resource "aws_ecs_service" "checkout_service" {
  name            = "checkout-api-service"
  cluster         = aws_ecs_cluster.checkout_cluster.id
  task_definition = aws_ecs_task_definition.checkout_task.arn
  launch_type     = "FARGATE"
  desired_count   = 3

  network_configuration {
    subnets = [aws_subnet.subnet_a.id, aws_subnet.subnet_b.id]
    assign_public_ip = true
    security_groups  = [aws_security_group.load_balancer_security_group.id]
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.target_group.arn
    container_name   = "checkoutapiv2"
    container_port   = 8080
  }
}

# Create RDS Subnet Group
resource "aws_db_subnet_group" "checkout_db_subnet_group" {
  name       = "checkout-db-subnet-group"
  subnet_ids = [aws_subnet.subnet_a.id, aws_subnet.subnet_b.id, aws_subnet.subnet_c.id]

  tags = {
    Name = "Checkout DB Subnet Group"
  }
}

# Update the RDS instance to use the subnet group
resource "aws_db_instance" "checkout_db" {
  identifier             = "checkout-db-new"
  engine                 = "postgres"
  instance_class         = "db.t4g.micro"
  username               = "postgres"
  password               = "admin12345678"
  db_name                = "checkout"
  allocated_storage      = 20
  storage_type           = "gp2"
  publicly_accessible    = true
  skip_final_snapshot    = true

  # Fix: Attach the subnet group
  db_subnet_group_name   = aws_db_subnet_group.checkout_db_subnet_group.name
  vpc_security_group_ids = [aws_security_group.load_balancer_security_group.id]
}
