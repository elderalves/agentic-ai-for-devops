@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('Deployment environment.')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string

@description('Name of the Azure Container App to deploy.')
param containerAppName string

@description('Name of the container image to deploy (without registry or tag).')
param imageName string

@description('Tag of the container image to deploy.')
param imageTag string

var tags = {
  environment: environmentName
  project: 'agentic-ai-for-devops'
}

var acrPullRoleDefinitionId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
var containerRegistryName = 'acr${environmentName}${uniqueString(resourceGroup().id)}'
var containerAppIdentityName = 'id-${environmentName}-agentic-ai-for-devops'

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: containerRegistryName
  location: location
  tags: tags
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}

resource containerAppIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: containerAppIdentityName
  location: location
  tags: tags
}

// Granted to the user-assigned identity (not the Container App's own identity) so that
// AcrPull is in place before the Container App tries to pull its image on first create.
// Scoping this to the Container App instead creates a circular dependency: the role
// assignment would need the app to finish provisioning, but the app can't finish
// provisioning without the pull permission.
resource acrPullRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(containerRegistry.id, containerAppIdentity.id, acrPullRoleDefinitionId)
  scope: containerRegistry
  properties: {
    roleDefinitionId: acrPullRoleDefinitionId
    principalId: containerAppIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: 'log-${environmentName}-agentic-ai-for-devops'
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: 'cae-${environmentName}-agentic-ai-for-devops'
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsWorkspace.properties.customerId
        sharedKey: logAnalyticsWorkspace.listKeys().primarySharedKey
      }
    }
  }
}

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${containerAppIdentity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
      }
      registries: [
        {
          server: containerRegistry.properties.loginServer
          identity: containerAppIdentity.id
        }
      ]
    }
    template: {
      containers: [
        {
          name: imageName
          image: '${containerRegistry.properties.loginServer}/${imageName}:${imageTag}'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
  dependsOn: [
    acrPullRoleAssignment
  ]
}

output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.id
output logAnalyticsWorkspaceName string = logAnalyticsWorkspace.name
output containerAppsEnvironmentId string = containerAppsEnvironment.id
output containerAppsEnvironmentName string = containerAppsEnvironment.name
output containerAppsEnvironmentDefaultDomain string = containerAppsEnvironment.properties.defaultDomain
output containerAppId string = containerApp.id
output containerAppName string = containerApp.name
output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn
output containerRegistryName string = containerRegistry.name
output containerRegistryLoginServer string = containerRegistry.properties.loginServer
output containerAppIdentityId string = containerAppIdentity.id
