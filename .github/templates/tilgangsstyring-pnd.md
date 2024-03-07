---
title: Deploy & Patching W{{ date | date('ww') }}
labels: 'kind/deploy_patch, team/tilgangsstyring'
---
## Tuesday: Prod-deploy

- [ ] Deploy updates to APIM production (if updated since last deploy)
  - [ ] Check [APIM deploys to TT02](https://dev.azure.com/brreg/altinn-studio-ops/_build?definitionId=125) for updates to `#authorization` and `#access-management`. If none of them have been deployed the last week, no production APIM deploy is needed.
  - [ ] Deploy products with changes [to APIM production](https://dev.azure.com/brreg/altinn-studio-ops/_build?definitionId=128).
- [ ] Deploy[^1] all platform components to production[^2].
  - [ ] Deploy [Authorization] to production.
  - [ ] Deploy [Delegation Events] to production.
  - [ ] Deploy [Access Management] to production.
  - [ ] Verify [new pods startup](https://portal.azure.com/#@ai-dev.no/resource/subscriptions/1ab2d164-1861-4ff8-be8c-069c3ee3b70a/resourceGroups/altinnplatform-prod-rg/providers/Microsoft.ContainerService/managedClusters/platform-prod-01-aks/workloads) without errors.
  - [ ] Verify [function app deployment](https://portal.azure.com/#@ai-dev.no/resource/subscriptions/1ab2d164-1861-4ff8-be8c-069c3ee3b70a/resourceGroups/altinnplatform-prod-rg/providers/Microsoft.Web/sites/altinn-prod-delegation-func/appServices).
  - [ ] Grab a coffee.
  - [ ] Verify no errors reported for the components in Slack channels [#alerts-prod](https://altinndevops.slack.com/archives/C014H7WPSUB) or [#alerts-prod-critical](https://altinndevops.slack.com/archives/C012108PYBV).
  - [ ] Post-deploy [Authorization] and [Access Management] if everything looks good.
  - [ ] Post-deploy [Delegation Events] if everything looks good.
- [ ] Deploy frontend to production (if a new release was made and deployed to TT02 last week)
  - [ ] Check [Access Management Frontend Releases](https://github.com/Altinn/altinn-access-management-frontend/releases) if a new release was made last week and [deployed to TT02](https://github.com/Altinn/altinn-access-management-frontend/deployments/TT02)
  - [ ] Deploy new release of [Access Management Frontend] to production.
  - [ ] Verify [new revision of container app](https://portal.azure.com/#@ai-dev.no/resource/subscriptions/1ab2d164-1861-4ff8-be8c-069c3ee3b70a/resourceGroups/accessmanagementui-prod-rg/providers/Microsoft.App/containerApps/altinn-prod-amui-app/revisionManagement) started without error.
- [ ] If any of the above steps or links needs correction [update the automatic issue template](https://github.com/Altinn/altinn-authorization/blob/main/.github/templates/tilgangsstyring-pnd.md).

## Wednesday: TT02-deploy

- [ ] Deploy updates to APIM TT02 (if updated since last deploy)
  - [ ] Check [APIM deploys to AT](https://dev.azure.com/brreg/altinn-studio-ops/_build?definitionId=124) for updates to `#authorization` and `#access-management`. If none of them have been deployed the last week, no TT02 APIM deploy is needed.
  - [ ] Deploy products with changes [to TT02 APIM](https://dev.azure.com/brreg/altinn-studio-ops/_build?definitionId=125).
- [ ] Deploy[^1] all platform components to TT02[^3].
  - [ ] Deploy [Authorization] to TT02.
  - [ ] Deploy [Delegation Events] to TT02.
  - [ ] Deploy [Access Management] to TT02.
  - [ ] Verify [new pods startup](https://portal.azure.com/#@ai-dev.no/resource/subscriptions/dd6d3e08-a70f-4f71-8847-781ddc5d8468/resourceGroups/altinnplatform-tt02-rg/providers/Microsoft.ContainerService/managedClusters/platform-tt02-02-aks/workloads) without errors.
  - [ ] Verify [function app deployment](https://portal.azure.com/#@ai-dev.no/resource/subscriptions/dd6d3e08-a70f-4f71-8847-781ddc5d8468/resourceGroups/altinnplatform-tt02-rg/providers/Microsoft.Web/sites/altinn-tt02-delegation-func/appServices).
  - [ ] Grab a coffee.
  - [ ] Verify [Authorization K6 automated test](https://dev.azure.com/brreg/altinn-studio/_build?definitionId=414) results.
  - [ ] Verify [Authorization Bruno automated test](https://dev.azure.com/brreg/altinn-studio/_build?definitionId=480) results.
  - [ ] Verify [Access Management K6 automated test](https://dev.azure.com/brreg/altinn-studio/_build?definitionId=412) results.
  - [ ] Verify [Access Management Bruno automated test](https://dev.azure.com/brreg/altinn-studio/_build?definitionId=475) results.
  - [ ] Verify no errors reported for the components in Slack channels [#alerts-prod](https://altinndevops.slack.com/archives/C014H7WPSUB) or [#alerts-prod-critical](https://altinndevops.slack.com/archives/C012108PYBV) (Yes, TT02 alerts also goes here).
  - [ ] Post-deploy [Authorization] and [Access Management] if everything looks good.
  - [ ] Post-deploy [Delegation Events] if everything looks good.
- [ ] Deploy frontend to TT02 (if a draft release with changes is waiting for deploy)
  - [ ] Check [Access Management Frontend Releases](https://github.com/Altinn/altinn-access-management-frontend/releases) if a release draft is waiting.
  - [ ] Publish the draft as the next release as `v{year}.{releaseNumber}`
  - [ ] Deploy new release of [Access Management Frontend] to TT02.
  - [ ] Verify [new revision of container app](https://portal.azure.com/#@ai-dev.no/resource/subscriptions/dd6d3e08-a70f-4f71-8847-781ddc5d8468/resourceGroups/accessmanagementui-tt02-rg/providers/Microsoft.App/containerApps/altinn-tt02-amui-app/revisionManagement) started without error.
  - [ ] Verify [Cypress automated tests](https://github.com/Altinn/altinn-access-management-frontend/actions/workflows/cypress.yml) results.
- [ ] If any of the above steps or links needs correction [update the automatic issue template](https://github.com/Altinn/altinn-authorization/blob/main/.github/templates/tilgangsstyring-pnd.md).

## Thursday: Patching

### Go through all github pull-requests from the dependency bots in all repos

- [ ] [Authorization](https://github.com/Altinn/altinn-authorization/pulls)
- [ ] [Access Management](https://github.com/Altinn/altinn-access-management/pulls)
- [ ] [Access Management Frontend](https://github.com/Altinn/altinn-access-managment-frontend/pulls)

[Authorization]: https://dev.azure.com/brreg/altinn-studio/_release?_a=releases&view=mine&definitionId=23
[Delegation Events]: https://dev.azure.com/brreg/altinn-studio/_release?_a=releases&view=mine&definitionId=33
[Access Management]: https://dev.azure.com/brreg/altinn-studio/_release?_a=releases&view=mine&definitionId=37
[Access Management Frontend]: https://github.com/Altinn/altinn-access-management-frontend/actions/workflows/deploy-to-environment.yml

[^1]: Approve pending prod releases by clicking the blue production chip and clicking approve. ![image-20240306133832594](https://raw.githubusercontent.com/Altinn/altinn-authorization/main/.github/images/ado-pending-approval-screen.png)
[^2]: Look for any blue production chips. ![image-20240306133137061](https://raw.githubusercontent.com/Altinn/altinn-authorization/main/.github/images/ado-prod-button.png)
[^3]: Look for any blue TT02 chips. ![tt02-button](https://raw.githubusercontent.com/Altinn/altinn-authorization/main/.github/images/ado-tt02-button.png)
