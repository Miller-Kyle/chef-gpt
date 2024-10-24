
param uniqueValue string
param location string
param configname string
param tags object
param identityName string
param gptEndpoint string
param dallEEndpoint string

@secure()
param azureAiApiKey string

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: identityName
}

module keyvault 'br/public:avm/res/key-vault/vault:0.3.4' = {
  name: '${configname}-keyvault'
  params: {
    name: '${uniqueValue}kv'
    location: location
    enableTelemetry: false
    
    // Assign Tags
    tags: tags

    enablePurgeProtection: false
    
    // Configure RBAC
    enableRbacAuthorization: true
    roleAssignments:  [
      {
        principalId: identity.properties.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'Key Vault Secrets User'
      }
    ]

    // Configure Secrets
    secrets: {
      secureList: [ 
        {
          name: 'azure-ai-api-key'
          value: azureAiApiKey
        }
      ]
    }
  }
}

var label = 'ChefGpt'

var settings = [
  {
    name: 'ChefGpt:Sentinel'
    value: '0'
    contentType: 'text/plain'
    label: label
  }
  {
    name: 'ChefGpt:AzureAiStudioConfiguration:GptEndpoint'
    value: gptEndpoint
    contentType: 'text/plain'
    label: label
  }
  {
    name: 'ChefGpt:AzureAiStudioConfiguration:DallEEndpoint'
    value: dallEEndpoint
    contentType: 'text/plain'
    label: label
  }
  {
    name: 'ChefGpt:GptConfiguration:SystemPrompt' // The safety parameters are adapted from the sample safety instructions in Azure AI Studio, they are not complete and should not be assumed to be correct.
    value: 'placeholder' 
    contentType: 'text/plain'
    label: label
  }
  // Key Vault Secret Reference
  {
    name: 'ChefGpt:AzureAiStudioConfiguration:ApiKey'
    value: string({uri: '${keyvault.outputs.uri}secrets/azure-ai-api-key'})
    contentType: 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'
    label: label
  }
]

var featureFlags = [
]


module  configurationStore './app-configuration/main.bicep' = {
  name: '${configname}-app-config'
  params: {
    name: '${uniqueValue}ac'
    location: location
    tags: tags

    roleAssignments: [  
      {
        principalIds: [
          identity.properties.principalId
        ]
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'App Configuration Data Reader'
      }
    ]
    
    keyValues: settings
    featureFlags: featureFlags
  }
}

output name string = configurationStore.outputs.name
output endpoint string = configurationStore.outputs.endpoint
