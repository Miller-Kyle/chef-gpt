targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string

@minLength(3)
@maxLength(20)
@description('Used to name all resources')
param resourceName string = 'chefgpt'

@description('Specify the Azure AI API Key.')
@secure()
param azureAiApiKey string

@description('Specify the GPT Azure Studio Endpoint.')
param gptEndpoint string

@description('Specify the Dall-E Azure Studio Endpoint.')
param dallEEndpoint string

#disable-next-line no-unused-vars
var resourceToken = (uniqueString(subscription().id, environmentName, location))
var resourceGroupName = '${environmentName}-${resourceName}-${resourceToken}'
var uniqueValue = '${replace(configuration.name, '-', '')}${uniqueString(group.outputs.resourceId, configuration.name)}'

@description('Configuration Object')
var configuration = {
  name: 'chefgpt'
  displayName: 'Chef GPT Resources'
  tags: {
    'azd-env-name': environmentName
  }
  telemetry: false
  lock: {}
  logs: {
    sku: 'PerGB2018'
    retentionInDays: 30
  }
  storage: {
    sku: 'Standard_LRS'
  }
  insights: {
    sku: 'web'
  }
}

module group 'br/public:avm/res/resources/resource-group:0.2.3' = {
  name: resourceGroupName
  params: {
    name: resourceGroupName
    location: location
    lock: configuration.lock
    tags: configuration.tags
    enableTelemetry: configuration.telemetry
  }
}

module managedidentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.1.3' = {
  name: '${configuration.name}-user-managed-identity'
  scope: resourceGroup(resourceGroupName)
  params: {
    name: '${uniqueValue}mi'
    location: location
    lock: configuration.lock
    tags: configuration.tags
    enableTelemetry: configuration.telemetry
  }
}

module logAnalytics 'br/public:avm/res/operational-insights/workspace:0.2.1' = {
  name: '${configuration.name}-log-analytics'
  scope: resourceGroup(resourceGroupName)
  params: {
    name: '${uniqueValue}la'
    location: location
    lock: configuration.lock
    tags: configuration.tags
    enableTelemetry: configuration.telemetry

    skuName: configuration.logs.sku
    dataRetention: configuration.logs.retentionInDays
  }
}

module storageAccount 'br/public:avm/res/storage/storage-account:0.6.6' = {
  name: '${configuration.name}-storage-account'
  scope: resourceGroup(resourceGroupName)
  params: {
    name: '${uniqueValue}sa'
    skuName: configuration.storage.sku
    location: location
    enableTelemetry: configuration.telemetry
    allowSharedKeyAccess: false
    roleAssignments: [
      {
        principalId: managedidentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'Contributor'
      }
      {
        principalId: managedidentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'Storage Account Contributor'
      }
      {
        principalId: managedidentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'Storage Table Data Contributor'
      }
      {
        principalId: managedidentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'Storage Blob Data Owner'
      }
    ]

    diagnosticSettings: [
      {
        metricCategories: [
          {
            category: 'AllMetrics'
          }
        ]
        name: 'customSetting'
        workspaceResourceId: logAnalytics.outputs.resourceId
      }
    ]

    allowBlobPublicAccess: false
    blobServices: {
      deleteRetentionPolicyDays: 9
      deleteRetentionPolicyEnabled: true
      diagnosticSettings: [
        {
          metricCategories: [
            {
              category: 'AllMetrics'
            }
          ]
          name: 'customSetting'
          workspaceResourceId: logAnalytics.outputs.resourceId
        }
      ]
      lastAccessTimeTrackingPolicyEnabled: true
    }

    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
  }
}

module insights 'br/public:avm/res/insights/component:0.2.1' = {
  name: '${configuration.name}-insights'
  scope: resourceGroup(resourceGroupName)
  params: {
    name: '${uniqueValue}ai'
    location: location
    enableTelemetry: configuration.telemetry
    kind: configuration.insights.sku
    workspaceResourceId: logAnalytics.outputs.resourceId
    
    diagnosticSettings: [
      {
        metricCategories: [
          {
            category: 'AllMetrics'
          }
        ]
        name: 'customSetting'
        workspaceResourceId: logAnalytics.outputs.resourceId
      }
    ]
  }
}

module appConfiguration 'configuration.bicep' = {
  name: '${configuration.name}-configuration'
  scope: resourceGroup(resourceGroupName)
  params: {
    configname: configuration.name
    location: location
    tags: configuration.tags
    uniqueValue: uniqueValue

    identityName: managedidentity.outputs.name
    
    azureAiApiKey: azureAiApiKey
    gptEndpoint: gptEndpoint
    dallEEndpoint: dallEEndpoint
  }
}

module chefgpt 'app/chefgpt.bicep' = {
  name: '${configuration.name}-chefgpt-fa'
  scope: resourceGroup(resourceGroupName)
  params: {
    uniqueValue: uniqueValue
    environmentName: environmentName
    location: location
    functionIdentityName: managedidentity.outputs.name
    configurationName: appConfiguration.outputs.name
    insightsName: insights.outputs.name
    storageAccountName: storageAccount.outputs.name
  }
}

output APP_CONFIGURATION_ENDPOINT string = appConfiguration.outputs.endpoint
