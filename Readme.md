
# eCommerce Web API Application

This is a sample **eCommerce Web API** application built with **C# 11** and **.Net 7**. The **Onion** software architecture is used. The application uses **SQL Server** as the main database, **Redis** as the shopping cart database, and **CQRS** as the main structure using **MediatR**. The application handles all commands with **EF Core 7** and all queries with **Dapper**. It also implements **event publishing** and uses **RabbitMQ** as a message broker with the **polling publisher** pattern. The documentation is handled with **Swagger**, and there is a **docker** file and a **docker-compose.yml** file for easy deployment. **Integration tests** are implemented using **NUnit**. The application follows the **Code-First approach**, and the database **migration** handles both creation and seeding of sample data.


## APIs

The following APIs are included in the application:
- CRUD for Product Specifications
- CRUD for Categories
- CRUD for Products
- Get similar products to each product (Based on category and price range by 10% diff)
- Register Customer, Edit Customer info and Customers list (CRU for Customers)
- List of countries, provinces and cities
- CRUD for Customer Addresses
- Update shopping cart of each customer, List of cart items for each customer (CRUD for cart)
- Place order by migrating cart including items and apply tax on each price (Reduce each item's product quantity by event publishing of order placed as the quantity of each of them)
- Orders list of each customer
- Add invoice (manually or automated) for each order
- List all invoices


## Tech Stack

**Programming language:** C# 11

**Backend framework:** .Net 7

**Software architecture:** Onion architecture, CQRS

**CQS implementation:** MediatR

**Database:** SQL Server, Redis

**ORM:** EF Core, Dapper

**Message broker:** RabbitMQ

**Documentation:** Swagger

**Deployment tool:** Docker

**Test type:** Integration test

**Test tool:** NUnit


## Getting started

To run the application locally, follow these steps:

1. Clone the repository to your local machine.

2. Place you Sql Server connection string, Redis connection info and RabbitMQ configs in the `appsettings.Development.json` file.

3. Place you Sql Server connection string, Redis connection info for **Test environment** and RabbitMQ configs in the `appsettings.Test.json` file.
**Note:** If you have not any other instances for your redis and Redis and RabbitMQ, do not worry! It handles automatically and you just need to change the name of you SQL Server DB name!

4. Navigate to the project root directory and run docker-compose up to start the application and its dependencies.

5. Once the application is running, you can access the Swagger documentation by navigating to http://localhost:<port>/swagger.

6. Run the application!


## Contributing

Contributions are welcome! If you find any issues or have any suggestions, please submit a pull request or create an *issue* in the repository.


## Authors

- [Sina Bahmanpour](mailto:mrmaster9494@gmail.com)
