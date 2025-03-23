# Define the name of the S3 bucket
variable "bucket_name" {
  description = "The name of the S3 bucket to store Terraform state"
  type        = string
  default     = "mein-terraform-state-bucket"
}

# Define the ACL for the bucket
variable "bucket_acl" {
  description = "The ACL setting for the S3 bucket"
  type        = string
  default     = "private"
}

# Public access settings
variable "block_public_acls" {
  description = "Whether to block public ACLs for the bucket"
  type        = bool
  default     = false
}

variable "block_public_policy" {
  description = "Whether to block public bucket policies"
  type        = bool
  default     = false
}

variable "ignore_public_acls" {
  description = "Whether to ignore public ACLs for the bucket"
  type        = bool
  default     = false
}

variable "restrict_public_buckets" {
  description = "Whether to restrict public access to the bucket"
  type        = bool
  default     = false
}
