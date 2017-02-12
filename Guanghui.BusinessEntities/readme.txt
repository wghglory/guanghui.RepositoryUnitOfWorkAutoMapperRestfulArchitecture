You may wonder, we already have database entities to interact with database then why do we need Business Entities?
The answer is as simple as that, we are trying to follow a proper structure of communication, 
and one would never want to expose the database entities to the end client, 
in our case is Web API, it involves lot of risk. Hackers may manipulate the details and get accessto your database.
Instead we’ll use database entities in our business logic layer and use Business Entities as transfer objects 
to communicate between business logic and Web API project. So business entities may have different names but, 
their properties remains same as database entities. In our case we’ll add same name business entity classes 
appendint word "Entity" to them in our BusinessEntity project. 