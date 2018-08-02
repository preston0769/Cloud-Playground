
// Resource Group ==> Cluster ==> Nodes ==> App 


// create a resource groud
az group create --name myAKSCluster --location eastus

//create aks cluster
az aks create --resource-group myAKSCluster --name myAKSCluster --node-count 1 --enable-addons monitoring --generate-ssh-keys
 
 //use kubenetes command line
 //If locally use azure cli, then install kubernetes cli is required
 az aks install-cli

 //Before create use kubectl, need to get kubernetes credentials into local workspace
 az aks get-credentials --resource-group myAKSCluster --name myAKSCluster

 //Use this command to get running nodes and their status
 kubectl get nodes


//Start application by runing configuration file(.yaml)
kubectl apply -f azure-vote.yaml


//Before a service is up and runing, obtained public ip, run following to watch a service
//It should be updated every 2 minutes
kubectl get service azure-vote-front --watch

// GO TO PUBLIC IP ADDRESS


//Delete cluster 
az group delete --name myAKSCluster --yes --no-wait


//Above command will not delete  service principles, so need to run following commnd to delete them
az ad app list --query "[?displayName=='myAKSCluster'].{Name:displayName,Id:appId}" --output table
az ad app delete --id <appId>