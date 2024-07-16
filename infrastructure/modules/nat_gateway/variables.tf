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

variable "subnets" {
  type = map(object({
    id   = string
    name = string
  }))
}
