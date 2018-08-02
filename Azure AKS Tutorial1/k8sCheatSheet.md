## Kubectl Context and Configuration
## ##
# Show Merged kubeconfig settings.
kubectl config view 

# use multiple kubeconfig files at the same time and view merged config
KUBECONFIG=~/.kube/config:~/.kube/kubconfig2 kubectl config view

# Get the password for the e2e user
kubectl config view -o jsonpath='{.users[?(@.name == "e2e")].user.password}'

# Display the current-context
kubectl config current-context              
# set the default context to my-cluster-name
kubectl config use-context my-cluster-name  

# add a new cluster to your kubeconf that supports basic auth
kubectl config set-credentials kubeuser/foo.kubernetes.com --username=kubeuser --password=kubepassword

# set a context utilizing a specific username and namespace.
kubectl config set-context gce --user=cluster-admin --namespace=foo \
  && kubectl config use-context gce


## Creating Objects
##  ##
kubectl create -f ./my-manifest.yaml           # create resource(s)
kubectl create -f ./my1.yaml -f ./my2.yaml     # create from multiple files
kubectl create -f ./dir                        # create resource(s) in all manifest files in dir
kubectl create -f https://git.io/vPieo         # create resource(s) from url
kubectl run nginx --image=nginx                # start a single instance of nginx
kubectl explain pods,svc                       # get the documentation for pod and svc manifests

# Create multiple YAML objects from stdin
cat <<EOF | kubectl create -f -
apiVersion: v1
kind: Pod
metadata:
  name: busybox-sleep
spec:
  containers:
  - name: busybox
    image: busybox
    args:
    - sleep
    - "1000000"
---
apiVersion: v1
kind: Pod
metadata:
  name: busybox-sleep-less
spec:
  containers:
  - name: busybox
    image: busybox
    args:
    - sleep
    - "1000"
EOF

# Create a secret with several keys
cat <<EOF | kubectl create -f -
apiVersion: v1
kind: Secret
metadata:
  name: mysecret
type: Opaque
data:
  password: $(echo -n "s33msi4" | base64)
  username: $(echo -n "jane" | base64)
EOF

## Viewing, Finding Resources
## ##

# Get commands with basic output
kubectl get services                          # List all services in the namespace
kubectl get pods --all-namespaces             # List all pods in all namespaces
kubectl get pods -o wide                      # List all pods in the namespace, with more details
kubectl get deployment my-dep                 # List a particular deployment
kubectl get pods --include-uninitialized      # List all pods in the namespace, including uninitialized ones

# Describe commands with verbose output
kubectl describe nodes my-node
kubectl describe pods my-pod

kubectl get services --sort-by=.metadata.name # List Services Sorted by Name

# List pods Sorted by Restart Count
kubectl get pods --sort-by='.status.containerStatuses[0].restartCount'

# Get the version label of all pods with label app=cassandra
kubectl get pods --selector=app=cassandra rc -o \
  jsonpath='{.items[*].metadata.labels.version}'

# Get all running pods in the namespace
kubectl get pods --field-selector=status.phase=Running

# Get ExternalIPs of all nodes
kubectl get nodes -o jsonpath='{.items[*].status.addresses[?(@.type=="ExternalIP")].address}'

# List Names of Pods that belong to Particular RC
# "jq" command useful for transformations that are too complex for jsonpath, it can be found at https://stedolan.github.io/jq/
sel=${$(kubectl get rc my-rc --output=json | jq -j '.spec.selector | to_entries | .[] | "\(.key)=\(.value),"')%?}
echo $(kubectl get pods --selector=$sel --output=jsonpath={.items..metadata.name})

# Check which nodes are ready
JSONPATH='{range .items[*]}{@.metadata.name}:{range @.status.conditions[*]}{@.type}={@.status};{end}{end}' \
 && kubectl get nodes -o jsonpath="$JSONPATH" | grep "Ready=True"

# List all Secrets currently in use by a pod
kubectl get pods -o json | jq '.items[].spec.containers[].env[]?.valueFrom.secretKeyRef.name' | grep -v null | sort | uniq

# List Events sorted by timestamp
kubectl get events --sort-by=.metadata.creationTimestamp



## Updating Resources
## ##
kubectl rolling-update frontend-v1 -f frontend-v2.json           # Rolling update pods of frontend-v1
kubectl rolling-update frontend-v1 frontend-v2 --image=image:v2  # Change the name of the resource and update the image
kubectl rolling-update frontend --image=image:v2                 # Update the pods image of frontend
kubectl rolling-update frontend-v1 frontend-v2 --rollback        # Abort existing rollout in progress
cat pod.json | kubectl replace -f -                              # Replace a pod based on the JSON passed into stdin

# Force replace, delete and then re-create the resource. Will cause a service outage.
kubectl replace --force -f ./pod.json

# Create a service for a replicated nginx, which serves on port 80 and connects to the containers on port 8000
kubectl expose rc nginx --port=80 --target-port=8000

# Update a single-container pod's image version (tag) to v4
kubectl get pod mypod -o yaml | sed 's/\(image: myimage\):.*$/\1:v4/' | kubectl replace -f -

kubectl label pods my-pod new-label=awesome                      # Add a Label
kubectl annotate pods my-pod icon-url=http://goo.gl/XXBTWq       # Add an annotation
kubectl autoscale deployment foo --min=2 --max=10                # Auto scale a deployment "foo"