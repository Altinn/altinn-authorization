output "zones" {
  value = { for key, value in local.zones : key =>
    {
      id   = azurerm_private_dns_zone.dns[key].id
      name = value
  } }
}
