locals {
  cidr_prefix        = tonumber(split("/", var.cidr)[1])
  small_subnet_mask  = 28 - cidr_prefix # IPs 16
  medium_subnet_mask = 26 - cidr_prefix # IPs 64
  large_subnet_mask  = 24 - cidr_prefix # IPs 256

  subnets = {
    key_vault = {
      bits = local.medium_subnet_mask
    }
    app_configuration = {
      bits = local.medium_subnet_mask
    }
    storage_accounts = {
      bits = local.small_subnet_mask
    }
    redis = {
      bits = local.small_subnet_mask
    }
    postgres = {
      bits = local.small_subnet_mask
      delegations = {
        fs = {
          name = "Microsoft.DBforPostgreSQL/flexibleServers"
          actions = [
            "Microsoft.Network/virtualNetworks/subnets/join/action"
          ]
        }
      }
    }
  }
}

data "azurerm_resource_group" "vnet" {
  name = var.resource_group_name
}


resource "azurerm_virtual_network" "vnet" {
  name                = "vnet-${var.metadata.solution}-${var.metadata.enviroment}-${var.metadata.instance}"
  address_space       = [var.cidr]
  location            = data.azurerm_resource_group.vnet.location
  resource_group_name = data.azurerm_resource_group.vnet.name
}

module "subnet" {
  source          = "hashicorp/subnets/cidr"
  base_cidr_block = var.cidr
  networks = [for key, value in local.subnets : {
    name     = key
    new_bits = value.subnet_mask
  }]
}

resource "azurerm_subnet" "vnet" {
  resource_group_name  = var.resource_group_name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = module.subnet[each.key]
  name                 = each.key

  dynamic "delegation" {
    content {
      name = each.key
      service_delegation {
        name    = each.value.name
        actions = each.value.actions
      }
    }

    for_each = try(each.value.delegations, {})
  }

  for_each = local.subnets
}
