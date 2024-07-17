data "azurerm_client_config" "current" {}

# https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#security
data "azurerm_role_definition" "key_vault_administrator" {
  role_definition_id = "00482a5a-887f-4fb3-b363-3b7fe8e74483"
}

data "azurerm_resource_group" "key_vault" {
  name = var.resource_group_name
}

resource "random_string" "key_vault" {
  length  = 4
  lower   = true
  numeric = false
  upper   = false
}

# https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/key_vault
resource "azurerm_key_vault" "key_vault" {
  name                          = "kvencryption${random_string.key_vault.result}"
  resource_group_name           = data.azurerm_resource_group.key_vault.name
  tenant_id                     = data.azurerm_client_config.current.tenant_id
  sku_name                      = "standard"
  location                      = data.azurerm_resource_group.key_vault.location
  enable_rbac_authorization     = true
  purge_protection_enabled      = true
  soft_delete_retention_days    = 30
  public_network_access_enabled = true

  network_acls {
    bypass         = "AzureServices"
    default_action = "Allow"
  }
}

# https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/role_assignment
resource "azurerm_role_assignment" "key_vault_administrator" {
  scope                = azurerm_key_vault.key_vault.id
  principal_id         = data.azurerm_client_config.current.object_id
  role_definition_name = data.azurerm_role_definition.key_vault_administrator.name
}

# https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/private_endpoint
resource "azurerm_private_endpoint" "key_vault" {
  name                          = "pe-${azurerm_key_vault.key_vault.name}"
  location                      = data.azurerm_resource_group.key_vault.location
  resource_group_name           = data.azurerm_resource_group.key_vault.name
  subnet_id                     = var.subnet_id
  custom_network_interface_name = "nic-${azurerm_key_vault.key_vault.name}"

  private_service_connection {
    name                           = azurerm_key_vault.key_vault.name
    private_connection_resource_id = azurerm_key_vault.key_vault.id
    is_manual_connection           = false
    subresource_names              = ["vault"]
  }

  private_dns_zone_group {
    name                 = azurerm_key_vault.key_vault.name
    private_dns_zone_ids = var.dns_zones
  }
}

# https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/key_vault_key
resource "azurerm_key_vault_key" "key_vault" {
  name         = each.value
  key_vault_id = azurerm_key_vault.key_vault.id
  key_type     = "RSA"
  key_size     = 2048

  depends_on = [azurerm_role_assignment.key_vault_administrator]

  key_opts = [
    "decrypt",
    "encrypt",
    "sign",
    "unwrapKey",
    "verify",
    "wrapKey"
  ]

  for_each = var.encryption_keys
}
