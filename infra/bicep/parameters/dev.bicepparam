using '../main.bicep'

param location = 'eastus'
param environmentName = 'dev'
param containerRegistryServer = 'myregistry.azurecr.io'
param containerRegistryName = 'myregistry'
param containerAppName = 'ca-dev-agentic-ai-for-devops'
param imageName = 'agentic-ai-for-devops'
param imageTag = 'latest'
