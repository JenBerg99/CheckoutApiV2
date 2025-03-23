AWS_ACCOUNT_ID = $(shell aws sts get-caller-identity --query "Account" --output text)
AWS_REGION = eu-north-1
REPO_NAME = checkoutapiv2
IMAGE_TAG = latest
ECR_URL = $(shell cd ../Terraform-Main && terraform output -raw ecr_image_url)

# ================================
# AWS ECR Login
# ================================
login:
	aws ecr get-login-password --region $(AWS_REGION) | docker login --username AWS --password-stdin $(AWS_ACCOUNT_ID).dkr.ecr.$(AWS_REGION).amazonaws.com

# ================================
# Terraform Bootstrap Setup
# ================================
bootstrap:
	cd ../Terraform-Bootstrap && terraform init && terraform apply -auto-approve

# ================================
# Docker Build & Push
# ================================
build:
	docker build -t $(REPO_NAME) -f ./Dockerfile .

tag:
	docker tag $(REPO_NAME):latest $(ECR_URL)

push: login build tag
	docker push $(ECR_URL)

# ================================
# Deployment with Terraform
# ================================
deployBase:
	cd ../Terraform-Main && terraform apply -auto-approve

deploy: push
	cd ../Terraform-Main && terraform apply -auto-approve

restartService: push deploy
	aws ecs update-service --cluster checkout-cluster --service checkout-api-service --force-new-deployment
