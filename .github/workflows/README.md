# Field Day GitHub-CI Action Workflows

## Unity WebGL Build `WebGL.yml` 
This workflow will download Unity and create a WebGL build of the project, then capture the build as an artifact and deploy to a specificed host behind a GlobalProtect Firewall. 

This process requires various "secrets" to be configured in the project's 'Settings > Options > Secrets'.

* UNITY_LICENSE (see 'Unity Activiation' Below) 
* DEPLOY_DIR : The directory to deploy to, like "/httpdocs/play/game/ci"
* DEPLOY_HOST : The host to deploy to, like "fielddaylab.wisc.edu"
* DEPLOY_KEY : The ssh private key to login to the DEPLOY_HOST
* DEPLOY_USER : The ssh user to login to the DEPLOY_HOST 
* VPN_PASSWORD : The password to login to the VPN
* VPN_USERNAME : The user to login to the VPN

UNITY_VERSION is hardcoded into this file and is linked to the activation code. 

The trigger for this action is any push to any branch in the repository.

This build artifact is uploaded to a folder specific to this release version.
The WebGL build will be placed in DEPLOY_HOST:DEPLOY_DIR/BRANCH

## Unity Activation `unity-activation.yml`

[Should be disabled after first use]

This workflow shows how to grab the unity license for the GitHub action workflow.  The 
documentation for what to do with the license artifact that is produced from this workflow 
can be found on the [Unity CI Docs](https://unity-ci.com/docs/github/activation)

This worfklow only needs to be rerun when changing Unity versions 
