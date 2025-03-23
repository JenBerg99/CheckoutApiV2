variable "region" {
  default = "eu-north-1"
}

variable "db_password" {
  description = "Password for the PostgreSQL database"
  type        = string
  sensitive   = true
  default     = "admin12345678"
}

variable "db_name" {
  description = "Database name"
  type        = string
  default     = "checkout"
}

variable "db_user" {
  description = "Database username"
  type        = string
  default     = "postgres"
}

variable "service_name" {
  description = "Name of the ECS service"
  type        = string
  default     = "checkout-api-service"
}

variable "ecs_memory" {
  description = "Memory for ECS task"
  type        = number
  default     = 512
}

variable "ecs_cpu" {
  description = "CPU for ECS task"
  type        = number
  default     = 256
}

variable "container_port_8080" {
  description = "Container port for HTTP traffic"
  type        = number
  default     = 8080
}

variable "container_port_8081" {
  description = "Container port for secondary HTTP traffic"
  type        = number
  default     = 8081
}
