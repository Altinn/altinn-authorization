data "azurerm_client_config" "current" {}

data "azurerm_resource_group" "postgres_server" {
  name = var.resource_group_name
}

# https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#security
data "azurerm_role_definition" "key_vault_crypto_officer" {
  role_definition_id = "14b46e9e-c2b7-41b4-b07b-48a6ebf60603"
}


# https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/data-sources/user_assigned_identity
resource "azurerm_user_assigned_identity" "postgres_server" {
  name                = "mipsqlsrv${var.metadata.suffix}"
  resource_group_name = data.azurerm_resource_group.postgres_server.name
  location            = data.azurerm_resource_group.postgres_server.location
}

# https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/role_assignment
resource "azurerm_role_assignment" "key_vault_crypto_officer" {
  scope                            = var.key_vault_id
  principal_id                     = azurerm_user_assigned_identity.postgres_server.principal_id
  role_definition_name             = data.azurerm_role_definition.key_vault_crypto_officer.name
  skip_service_principal_aad_check = true
}

resource "azurerm_postgresql_flexible_server" "postgres_server" {
  name                = "psqlsrv${var.metadata.suffix}"
  resource_group_name = data.azurerm_resource_group.postgres_server.name
  location            = data.azurerm_resource_group.postgres_server.location
  version             = var.postgres_version

  delegated_subnet_id           = var.subnet_id
  private_dns_zone_id           = var.dns_zone
  public_network_access_enabled = false

  administrator_login    = "psqladmin"
  administrator_password = random_password.administrator_password.result
  zone                   = "1"

  storage_mb        = var.storage_mb
  auto_grow_enabled = true

  authentication {
    active_directory_auth_enabled = true
    password_auth_enabled         = true
    tenant_id                     = data.azurerm_client_config.current.tenant_id
  }

  customer_managed_key {
    key_vault_key_id                  = azurerm_key_vault_key.postgres_server.id
    primary_user_assigned_identity_id = azurerm_user_assigned_identity.postgres_server.id
  }

  identity {
    identity_ids = [azurerm_user_assigned_identity.postgres_server.id]
    type         = "UserAssigned"
  }

  storage_tier = "P30"
  sku_name     = "GP_Standard_D4s_v3"
}

# https://registry.terraform.io/providers/hashicorp/random/latest/docs/resources/password
resource "random_password" "administrator_password" {
  length = 30
}

# https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/key_vault_secret
resource "azurerm_key_vault_secret" "administrator_password" {
  name         = "Postgres--AdminPassword"
  content_type = "text/plain"
  key_vault_id = var.key_vault_id
  value        = random_password.administrator_password.result
}

# https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/key_vault_key
resource "azurerm_key_vault_key" "postgres_server" {
  name         = "Postgres"
  key_vault_id = var.key_vault_id
  key_type     = "RSA"
  key_size     = 2048

  key_opts = [
    "unwrapKey",
    "wrapKey"
  ]
}
