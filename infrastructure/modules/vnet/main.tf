locals {
  cidr_prefix   = tonumber(split("/", var.cidr)[1])
  small_subnet  = 28 - local.cidr_prefix # Available IPs 11
  medium_subnet = 26 - local.cidr_prefix # Available IPs IPs 59
  large_subnet  = 24 - local.cidr_prefix # # Available IPs 251

  subnets = {
    application = {
      bits = local.large_subnet
    }
    encryption = {
      bits = local.small_subnet
    }
    databases = {
      bits = local.small_subnet
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
  name                = "vnet-${var.metadata.solution}-${var.metadata.environment}-${var.metadata.instance}"
  address_space       = [var.cidr]
  location            = data.azurerm_resource_group.vnet.location
  resource_group_name = data.azurerm_resource_group.vnet.name
}

module "subnet" {
  source          = "hashicorp/subnets/cidr"
  base_cidr_block = var.cidr
  networks = [for key, value in local.subnets : {
    name     = key
    new_bits = value.bits
  }]
}

resource "azurerm_subnet" "vnet" {
  resource_group_name  = var.resource_group_name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = [module.subnet.networks[index(module.subnet.networks.*.name, each.key)].cidr_block]
  name                 = each.key

  dynamic "delegation" {
    content {
      name = delegation.key
      service_delegation {
        name    = delegation.value.name
        actions = delegation.value.actions
      }
    }

    for_each = try(each.value.delegations, {})
  }

  for_each = local.subnets
}
