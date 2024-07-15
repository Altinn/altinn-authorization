locals {
  size = 22
  subnets = {
    key_vault = {
      address_space = 4
    }
    app_configuration = {
      address_space = 2
    }
    storage_accounts = {
      address_space = 3
    }
    redis = {
      address_space = 4
    }
    postgres = {
      address_space = 5
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
