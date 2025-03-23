output "ecr_image_url" {
  value = aws_ecr_repository.checkoutapiv2.repository_url
}

output "ecs_service_name" {
  value = aws_ecs_service.checkout_service.name
}

output "db_endpoint" {
  value = aws_db_instance.checkout_db.endpoint
}

output "load_balancer_dns" {
  value = aws_alb.application_load_balancer.dns_name
}
