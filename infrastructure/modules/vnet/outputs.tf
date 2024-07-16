output "id" {
  value = azurerm_virtual_network.vnet.id
}

output "name" {
  value = azurerm_virtual_network.vnet.name
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
