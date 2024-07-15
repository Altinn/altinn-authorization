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


resource "azurerm_resource_group" "auth" {
  name     = "rg-altinn-authorization-${var.environment}-001"
  location = "norwayeast"
}
