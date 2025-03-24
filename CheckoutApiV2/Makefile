AWS_ACCOUNT_ID = $(shell aws sts get-caller-identity --query "Account" --output text)
AWS_REGION = eu-north-1
REPO_NAME = checkoutapiv2
IMAGE_TAG = latest

# ================================
# AWS ECR Login
# ================================
login:
	aws ecr get-login-password --region $(AWS_REGION) | docker login --username AWS --password-stdin $(AWS_ACCOUNT_ID).dkr.ecr.$(AWS_REGION).amazonaws.com

# ================================
# Terraform Bootstrap Setup
# ================================
init:
	cd ../Terraform-Main && terraform init

# ================================
# Docker Build & Push
# ================================
build:
	docker build -t $(REPO_NAME) -f ./Dockerfile .

tag:
	docker tag $(REPO_NAME):latest  $(AWS_ACCOUNT_ID).dkr.ecr.$(AWS_REGION).amazonaws.com/$(REPO_NAME):latest

push: login build tag
	docker push $(AWS_ACCOUNT_ID).dkr.ecr.$(AWS_REGION).amazonaws.com/$(REPO_NAME):latest

# ================================
# Deployment with Terraform
# ================================
deployBase:
	cd ../Terraform-Main && terraform apply -auto-approve

deploy: push
	cd ../Terraform-Main && terraform apply -auto-approve

deployAll: init deployBase deploy
	aws ecs update-service --cluster checkoutapiv2 --service checkout-service --force-new-deployment
