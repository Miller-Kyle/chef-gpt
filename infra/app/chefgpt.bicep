param location string
param environmentName string
param configurationName string
param insightsName string
param storageAccountName string
param functionIdentityName string
param uniqueValue string

@description('Configuration Object')
var configuration = {
  name: 'chefgpt'
  displayName: 'chefgpt'
  tags: {
    'azd-env-name': environmentName
    'azd-service-name': 'chefgpt'
  }
}

resource configStore 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: configurationName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource functionIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: functionIdentityName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: insightsName
}

module serverfarm 'br/public:avm/res/web/serverfarm:0.2.2' = {
  name: '${configuration.name}-server-farm'
  params: {
    name: '${uniqueValue}sf'
    skuCapacity: 1
    skuName: 'S1'
    location: location
  }
}

module functionApp 'br/public:avm/res/web/site:0.3.8' = {
  name: '${configuration.name}-functionapp-module'
  params: {
    name: '${uniqueValue}fa'
    location: location
    kind: 'functionapp'
    tags: configuration.tags
    managedIdentities: {
      userAssignedResourceIds: [
        functionIdentity.id
      ]
    }
    serverFarmResourceId: serverfarm.outputs.resourceId
    siteConfig: {
      alwaysOn: true
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'AzureWebJobsStorage__accountName'
          value: storageAccount.name
        }
        {
          name: 'AzureWebJobsStorage__credential'
          value: 'managedIdentity'
        }
        {
          name: 'AzureWebJobsStorage__clientId'
          value: functionIdentity.properties.clientId
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'APP_CONFIGURATION_ENDPOINT'
          value: configStore.properties.endpoint
        }
        {
          name: 'MANAGED_IDENTITY_CLIENT_ID'
          value: functionIdentity.properties.clientId
        }
      ]
    }
    httpsOnly: true
  }
}
