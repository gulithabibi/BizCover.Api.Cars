
Note for testing:
1. To call this API need authentication, to get authentication can run this:
   [HttpPost]
   https://localhost:port/api/auth/token
   
2. To add car can run this:
   [HttpPost]
   https://localhost:port/api/v1/addcar
   
   Sample Request Json:
   {"id":1001,"make":"Cina","model":"Geely","year":2012,"countryManufactured":"Cina","colour":"Red","price":12000}

3. To Update car can run this:
   [HttpPost]
   https://localhost:port/api/v1/UpdateCar
   
   Sample Request Json:
   {"id":10,"make":"Jerman","model":"Ram 2500","year":2006,"countryManufactured":"Czech Republic","colour":"Blue","price":50000}

4. To Get All Car, can run this:
   [HttpGet]
   https://localhost:port/api/v1/carlist
   
5. To Get calculation discount, can run this:
   [HttpPost]
   https://localhost:port/api/v1/Calculation
   
   Sample Request Json:
   {"Cars":[{"id":1},{"id":200},{"id":3},{"id":4},{"id":1000}]}
   
   
Additional Information:
- Get token, using JWT, with type bearer token.
- Logging, using third party provider (NLog).
   
   