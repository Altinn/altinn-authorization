[private]
@default:
  just --list

# Install node packages required to run scripts - uses pnpm to install the packages
[private]
@install-script-packages:
  #!pwsh
  pushd .github/scripts
  pnpm install

[private]
@install-script-packages-frozen:
  #!pwsh
  pushd .github/scripts
  pnpm install --frozen-lockfile

