# Changes made to the base project

1. Using a document-based format for unstructured Category and item attribute data. Unfortunately, SQL Server handles Json data poorly, but I didn't want to change to Cosmos or Postgres so I made the best I could of it.
2. Products and Inventory broken out into seperate APIs. In a distributed system it's reasonable that the warehouse would not handle both. This way, some kind of business development entity can manage products and the warehouse can manage inventory
3. Using Dapper for database access. It's just easier that way.

# Data samples
Because neither Json nor SQL Server's handling of JSON make schemas obvious, the Category field in the Product table should be a string containing a JSON list. For instance, a sports drink product could have the CAtegories field "["Food","Drinks","Sports Drinks"]"
The Attributes field of the InventoryItem table should be a string containing a JSON object. The key/value pairs would be the attribute type and value. For instance, a pair of pants could have "{"Color":"White","Size":"Large"}
I've included a .dacpac and a schema comparison file to set up the database. The connection string assumes integrated security. Typically, I'd set up the database for RBAC access only and allow the API identity determine what access it does and doesn't have.
