## Introduction ##
Product Inventory Management is big and complex problem. I wanted to deliver as complete solution as possible but i had to make some assumptions and decide to cut some features (eg. all CRUD operations for each entity - only skeleton controllers implementation has been made) to fit with the solution in reasonable time. This solution still maps the problem to Entities in very expandable and configurable way. The biggest shortcut made is currently lack of any tests - this should be addressed <b><u>ASAP</u></b>
I have removed the existing files as i could not find proper usage in my solution ¯\_(ツ)_/¯

## Assumptions ##
To avoid extensive complexity i have decided not to store information regarding costs of acquisition. The ideal place would be inventory operation where cost can be added. Another simplicity i made was adding manufacturer and supplier as just string property to Product and InventoryOperation classes while normally it should be modelled as separate entity.

## Other ##
I have provided sample Postman collection to perform some operations in the system. Those are located in: ITI.postman_collection.json file