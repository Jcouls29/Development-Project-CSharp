# Sample implementation for Product API
 
## Features
 
- Get all Product
- Insert Product
- Temporary persistence implemented using a singleton DI instance  of Products Repository 
## Remarks
- Metadata can be implemented using a metadata repository using the Metadata class. The metadata class contains productid and a
    Dictionary of key value pairs that could contain any number of entries. The dictionary makes sure the same same key is not 
    added twice. 
- Category can be implemented using a Category repository using the category class, The Category class contains the productId and a     hashset for adding unique categories a product belongs to.
- The repository should be inside a service, because of the time constraint the repository was injected directly to the constructor
