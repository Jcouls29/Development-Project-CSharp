This Api is very incomplete and rough around the edges (especially capture errors). Obviously, two hours is not enough for a completed product, especially a polished one.

## Explanations for various decisions
* I chose to use an sqlite database for simplicity and ease of use. I don't have MSSQL installed or setup, nor did I want to spend time on it. My MSSQL skills are also a bit rusty
* I chose EF Core as that's more idiomatic and familiar. I'm very comfortable with raw-dogging SQL and Dapper is easy enough to use, but with the latest EF Core features, dapper is less necessary.
* The migration feature of EF Core is also top-notch
* Records over Classes. I much prefer Records when dealing with data, due to the default immutability and value-based equality. Records generally use Parameters to define properties, but EFCore doesn't fully support that yet, so I used Properties instead.
I know you can get immutable properties in Classes as well but you don't get that sweet `with` keyword

## Tasks

### Product Api
- [x] Create a new product
- [x] List all products
- [x] Get a product by id
- [ ] Create/Update a product -- partially done
- [ ] Delete a product by id


#### Notes
* Assigning attributes isn't fully working. 
* Categories can't be assigned either, or filtered by
* There is zero validation

### Transactions Api
- [x] Search
- [x] Create a new transaction
- [x] List all transactions by Product Id
- [x] Get a transaction by id
- [x] Create/Update a transaction for single Product
- [x] Create/Update a transaction for multiple Products

#### Notes
* Definitely a rough prototype, no doubt has bugs
* Search probably could be fleshed out
* No validation


### Category Api
#### not implemented yet

### Tests
I have a project in-place and there is a single test but it's pretty much useless. There was no time.
