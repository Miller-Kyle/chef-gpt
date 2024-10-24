@description('Required. Name of the key.')
param name string

@description('Required. Feature flag enabled/disabled.')
param enabled bool

@description('Required. Description of the feature flag.')
param featureDescription string

@description('Optional. Name of the Label.')
param label string = ''

@description('Conditional. The name of the parent app configuration store. Required if the template is used in a standalone deployment.')
param appConfigurationName string

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: appConfigurationName
}

var featureFlagValue = {
  id: name
  description: featureDescription
  enabled: enabled
}

var featureFlagName = '.appconfig.featureflag~2F${name}'

resource featureFlag 'Microsoft.AppConfiguration/configurationStores/keyValues@2022-05-01' = {
  parent: appConfiguration
  name: empty(label) ? featureFlagName : '${featureFlagName}$${label}'
  properties: {
    value: string(featureFlagValue)
    contentType: 'application/vnd.microsoft.appconfig.ff+json;charset=utf-8'
  }
}

@description('The name of the feature flag.')
output name string = featureFlag.name

@description('The resource ID of the key values.')
output resourceId string = featureFlag.id

@description('The resource group the batch account was deployed into.')
output resourceGroupName string = resourceGroup().name
