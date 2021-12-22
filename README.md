
# Official Headstart Solution
You can find the readme of the official Headstart solution here: https://github.com/ordercloud-api/headstart

# Why this Fork
The purpose of this fork is to give you a jump start on setting up a local installation of Sitecore OrderCloud applications.

Besides I wanted to make it possible that for the basic features no account is needed for the various third party services the Headstart solution relies on. So you don't have to deal with costs and regional market peculiarities of third party services right from the beginning.

I hope this helps some of you to avoid some pitfalls and to get a quicker impression of the Sitecore Headstart solution.

# Initial Setup - Step by Step
## Azure ressouces
In preparation for this minimal setup, you need to provide the following Azure resources.
The tables below document what resource information you need for the further installation process and where you can find it.

### [Azure Cosmos Database](https://docs.microsoft.com/en-us/azure/cosmos-db/introduction) 
You need to configure the Cosmos DB as Core (SQL) - the database itself is created automatically by the Headstart solution.

Property | Can be found in
--- | --- 
EndpointUri|Cosmos DB > Overview > URI
PrimaryKey|Cosmos DB > Keys > Primary Key


### [Storage Account](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-create?tabs=azure-portal) 

Property | Can be found in
--- | --- 
ConnectionString | Storage Account > Access Keys
BlobPrimaryEndpoint|Storage Account > Endpoints > Primary endpoint

*If you want to know a little more about what service is used for what, check out the official [readme](https://github.com/ordercloud-api/headstart#provisioning-azure-resources).*


## Middleware
This section describes how to set up a local middleware of the Headstart solution. In summary, a marketplace is created and seeded with initial data and the local middleware is set up and made known to ordercloud.io.

1. Create a new marketplace at [ordercloud.io](https://portal.ordercloud.io) and remember the unique identifier as `MarketplaceID`. 

    > Hint: Make sure to specify "US West" as the region. Other regions are currently not supported by the Headstart solution and various errors will occur.
1. Open the Headstart solution: `src/Middleware/Headstart.sln`
1. Update the following settings in the `appSettings.json` file at the root directory of the Headstart.API:
   * OrderCloudSettings:MarketplaceID
   * StorageAccountSettings:ConnectionString
   * StorageAccountSettings:BlobPrimaryEndpoint
   * CosmosSettings:DatabaseName (Chooese any name)
   * CosmosSettings:EndpointUri
   * CosmosSettings:PrimaryKey
   
   *For more detailed information please read this part of the official [Readme](https://github.com/ordercloud-api/headstart/blob/development/src/Middleware/src/Headstart.Common/AppSettingsReadme.md).*

1. Run the Headstart.API project. After the successful start, the 'Headstart Middleware API Documentation' should open in the browser at https://localhost:5001/index.html.

1. Start seeding the marketplace with a POST request to the `/seed` endpoint of the middleware with the following body:
    ```
    {
        "PortalUsername": "YourUsername",
        "PortalPassword": "YourPassword",
        "InitialAdminUsername": "AnyName",
        "InitialAdminPassword": "AnyPassword",
        "MiddlewareBaseUrl": "https://localhost:5001",
        "MarketplaceID": "IdOfYourMarketplace",
        "OrderCloudSettings": {
            "Environment": "sandbox",
            "WebhookHashKey": "AnyKey"
        },
        "StorageAccountSettings": {
            "ConnectionString": "YourConnectionString"	
        }
    }
    ```
    > Note: For now it's ok to use localhost as MiddlewareBaseUrl, because it's mandatory but not used for local installations.

    *For more detailed information read this part of the official [documentation](https://github.com/ordercloud-api/headstart/blob/development/src/Middleware/src/Headstart.Common/Models/Misc/EnvironmentSeed.cs).*

1. The successfull response contains the following settings of the preconfigured API clients of your marketplace. You will need these data in the further process.
    API Client | Configuration
    --- | --- 
    Middlware | ClientID<br/>ClientSecret
    Seller | ClientID
    Buyer | ClientID_LOCAL

1. Complete the configuration of the middleware by updating the following settings in `appSettings.json`:
   * OrderCloudSettings:MiddlewareClientID
   * OrderCloudSettings:MiddlewareClientSecret
1. Run/Restart the Headstart.API project. 

### Expose your local middleware
1. To expose your local server you can use [ngrok](https://ngrok.com/product):
    * Run `ngrok authtoken XXXYourToken`
    * Run `ngrok http https://localhost:5001`        
    * Extract public ngrok url, like `https://5ed8-37-201-144-21.ngrok.io`
1. Afterwards you need to persist the ngrok url to the relevant integration event in [ordercloud portal](https://portal.ordercloud.io/console). That basically means you have to update the `CustomImplementationUrl` of the integration event with ID 'HeadStartCheckoutLOCAL' ([PATCH Partially update an integration event](https://ordercloud.io/api-reference/seller/integration-events/patch))'.

## Frontend Applications
This section describes how to set up the frontend application for buyers (the ecommerce website) and the backend application to administrate the shop (the admin interface).

### Preparation
1. Open Visual Studio Code
1. Installl Angular CLI: `npm install -g @angular/cli`
1. Install and build the Headstart SDK
    1. Open folder `src/UI/SDK `
    1. Install dependencies: `npm install`
    1. Build the project: `npm run build` 

### Buyer Application
1. Open folder `src/UI/Buyer`
1. Install dependencies: `npm install`
1. Configure App:
    * `src/assets/appConfigs/defaultbuyer-test.json`:
        * clientID = Id of local Buyer API Client
        * translateBlobUrl = Enter your storage account name
    * `src/environments/environment.local.ts`:
        * localBuyerApiClient = Id of local Buyer API Client

1. Build the project: `npm run build` 
1. Open http://localhost:4300/ and enjoy 

*For more detailed information read the official [readme of buyer application](https://github.com/ordercloud-api/headstart/blob/development/src/UI/Buyer/README.md).*

### Admin/ Seller Application
1. Open folder `src/UI/Seller/src`
1. Install dependencies: `npm install`
1. Configure App:
    * `src/assets/appConfigs/defaultadmin-test.json`:
        * clientID = Id of Seller API Client
        * translateBlobUrl = Enter your storage account name
        * blobStorageUrl = Enter your storage account name
1. Build the project: `npm run build` 
1. Open http://localhost:4200/ 
1. Login with your seeded `InitialAdminUsername` and enjoy 

*For more detailed information read the official [readme of seller application](https://github.com/ordercloud-api/headstart/blob/development/src/UI/Seller/README.md).*

# First steps
* If you don't have any experience with Sitecore OrderCloud, the Headstart team has made a suggestion for getting started with the Headstart application in this section of the [readme](https://github.com/ordercloud-api/headstart#validating-setup)
* Specifics of this solution:
    * The shipping costs are always 0, because the [EasyPost](https://www.easypost.com/signup) connection has been disabled.
    * Each address is valid because the US address validation by [SmartyStreets](https://www.smartystreets.com/pricing) has been disabled.
* You find some creditcard data in this section of the official [Readme](https://github.com/ordercloud-api/headstart#credit-cards).

# Tips & Tricks
* If you've any issue with authorization or tokens, use [jwt.io](https://jwt.io), it saves a lot of time!
* `ngrok http 5001` does not work, you will get a 500 server error at checkout. [This article](https://camerondwyer.com/2019/09/23/using-ngrok-to-get-a-public-https-address-for-a-local-server-already-serving-https-for-free/) explains why.
* In case of any error, take a closer look at the response of the previous integration event (so called Worksheet). For example, I found this error message in the middle of it: 
```
...
"ShipEstimateResponse": {
		"ShipEstimates": null,
		"HttpStatusCode": 503,
		"UnhandledErrorBody": "ngrok gateway error\nThe server returned an invalid or incomplete HTTP response.\r\n\r\nERR_NGROK_3004\r\n",
		"xp": null
	},
    ...
```