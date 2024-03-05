## Product Api Infrastructure
### Overview
I like to keep things simple if possible. Some things purposed here could be considered optional, such as around the Event Hub. 

When it comes to the code, I prefer modular monoliths over microservices. Easier to debug, test, develop, and deploy (at least in my experience). Lambda/Functions have their use-case, I just find that a rarity.

Anything intensive or very long-running should belong in a Job, separate from the Api. 
You could throw these in Functions but if this is something done quite often, it could be more cost-effective to have a Job.

I prefer logging only to stdout and have zero logging dependencies (besides the actual logger lib). Scrapers are very efficient. 
I wonder if one could do this with Events and modify the Scraper, but it's not something I've done. 

Ideally, the application itself should be able to run with minimal dependencies. 

### Assumptions
* There is a migration from on-prem to Azure Cloud
* The database is still on-prem
* on-prem is a bit of a black box
* The majority of the infrastructure is in Azure
* Authorization happens at two stages, the API Management and the API itself. The API Management could morph the token to provide the necessary data, requiring less work for the Api, or it would just validate a range of Apis (such as /admin/*).
The Api will mostly just validate the token and role but should rarely reach out to AD. 

### Flow
* The user makes a request to the API
* WAF Validates the request
* CDN will return cached data if available
* CDN routes to API Management
* API Management authorizes the request
* API Management routes to Legacy or New API
* New Api returns cached Redis data if available
* New Api logs to stdout
* Scraper (possibly Azure itself) scrapes the logs


### Logging
* Logs are written to stdout
* Logs are scraped by a scraper
* Logs are possibly sent Azure Monitor or 3rd party logging aggregator
* Logs are sent to Azure Blob for historical purposes (could be queried by Analytics)

### Events
#### *much like logs but with a purpose*

* Events are written directly to Event Hub
* Event Hub logs ALL events to Azure Blog
* Implement Filters based on future feature requirements will specify where events get sent
* Any Event can needs to be acted on *once*, such as processing a payment, will get sent to the Azure Queue
* Events are sent to Service Bus, mostly for 3rd party integrations (I find it easier this way but I could be wrong)
* Some Jobs can be triggered by Event Hub or even Queue -- generally, I prefer this, for simplicity
* Some Azure Functions can be triggered by Event Hub or even Queue
