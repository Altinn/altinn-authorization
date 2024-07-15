output "vnet" {
  value = {
    id : azurerm_virtual_network.vnet.id
    name = azurerm_virtual_network.vnet.name
  }

  description = ""
}

output "subnets" {
  value = { for subnet in local.subnets : subnet =>
    {
      id   = azurerm_virtual_network.vnet[subnet].id
      name = azurerm_virtual_network.vnet[subnet].name
    }
  }

  description = ""
}
