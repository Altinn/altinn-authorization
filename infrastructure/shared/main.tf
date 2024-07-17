terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.112.0"
    }
  }

  # backend "azurerm" {
  #   use_azuread_auth = true
  # }
}


locals {
  environment = lower(var.environment)
  metadata = {
    solution    = "shared"
    environment = local.environment
    instance    = var.instance
  }
}

resource "azurerm_resource_group" "authorization" {
  name     = "rg-${local.metadata.solution}-${local.metadata.environment}-${local.metadata.instance}"
  location = "norwayeast"
}

module "vnet" {
  source              = "../modules/vnet"
  resource_group_name = azurerm_resource_group.authorization.name
  metadata            = local.metadata

  cidr = var.cidr

  depends_on = [azurerm_resource_group.authorization]
}

module "nat" {
  source              = "../modules/nat_gateway"
  resource_group_name = azurerm_resource_group.authorization.name
  metadata            = local.metadata

  subnets = module.vnet.subnets

  depends_on = [azurerm_resource_group.authorization]
}

module "dns" {
  source              = "../modules/dns"
  resource_group_name = azurerm_resource_group.authorization.name
  metadata            = local.metadata

  vnet_id = module.vnet.id

  depends_on = [azurerm_resource_group.authorization]
}

module "key_vault" {
  source              = "../modules/key_vault"
  resource_group_name = azurerm_resource_group.authorization.name
  metadata            = local.metadata

  ###! Changing key or value deletes and creates a new one!
  encryption_keys = {
    service_bus = "ServiceBusEncryptionKey"
  }

  dns_zones = [module.dns.zones["key_vault"].id]
  subnet_id = module.vnet.subnets["encryption"].id

  depends_on = [azurerm_resource_group.authorization]
}
