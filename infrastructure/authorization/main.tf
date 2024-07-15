terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.112.0"
    }
  }

  backend "azurerm" {
    use_azuread_auth = true
  }
}


locals {
  environment = lower(var.environment)
  metadata = {
    solution    = "authorization"
    environment = local.environment
    instance    = var.instance
  }
}

resource "azurerm_resource_group" "authorization" {
  name     = "rg-${local.metadata.environment}-${local.metadata.instance}"
  location = "norwayeast"
}

# module "vnet" {
#   source   = "../modules/vnet"
#   metadata = local.metadata

#   cidr                = var.cidr
#   resource_group_name = azurerm_resource_group.authorization.name
# }
