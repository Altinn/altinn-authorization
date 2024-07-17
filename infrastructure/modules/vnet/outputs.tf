output "id" {
  value = azurerm_virtual_network.vnet.id
}

output "name" {
  value = azurerm_virtual_network.vnet.name
}

output "subnets" {
  value = { for subnet in local.subnets : subnet.name =>
    {
      id            = azurerm_subnet.vnet[subnet.name].id
      name          = azurerm_subnet.vnet[subnet.name].name
      address_space = azurerm_subnet.vnet[subnet.name].address_space
    }
  }

  description = ""
}
