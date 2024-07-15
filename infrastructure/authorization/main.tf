terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.112.0"
    }
  }
}


resource "azurerm_resource_group" "auth" {
  name     = "rg-altinn-auth-001"
  location = "norwayeast"
}
