output "vnet" {
  value = {
    id : azurerm_virtual_network.vnet.id
    name = azurerm_virtual_network.vnet.name
  }

  description = ""
}

output "subnets" {
  value = { for key, value in local.subnets : key =>
    {
      id   = azurerm_subnet.vnet[key].id
      name = azurerm_subnet.vnet[key].name
    }
  }

  description = ""
}
