locals {
  size = 20
  subnets = {
    key_vault = {
      ip_addresses = 1
    }
    app_configuration = {
      ip_addresses = 1
    }
    storage_accounts = {
      ip_addresses = 1
    }
    redis = {
      ip_addresses = 1
    }
    postgres = {
      ip_addresses = 2
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
  address_space       = var.cidr
  location            = data.azurerm_resource_group.vnet.location
  resource_group_name = data.azurerm_resource_group.vnet.name
}


resource "azurerm_subnet" "vnet" {
  resource_group_name  = var.resource_group_name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = ""
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
