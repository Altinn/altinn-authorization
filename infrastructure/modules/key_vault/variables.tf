variable "encryption_keys" {
  type    = map(string)
  default = {}
}

variable "metadata" {
  type = object({
    solution    = string
    environment = string
    instance    = string
  })
}

variable "resource_group_name" {
  type = string
}

variable "subnet_id" {
  type = string
}

variable "dns_zones" {
  type = list(string)
}
