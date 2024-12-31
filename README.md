# MO Expense Tracker

Mo Expense Tracker is a multi-user web api for tracking user's expenditure. It was implemented to learn dotnet.

## Application

### Core

Core has functionality or object that is shared across the api such as exception handling, endpoint filters (for validation), response objects...

### Data

Data has the database context

### Features

It says break your api into features and so it was done. There are four featuresc as of this time. There are the Account, Auth, Category and Expense. Each feature may have a controller, dao, dto, endpoint and validation. (There was a time I wanted to remove the dao).

### Migrations

Migrations has the table migrations created to the tables (models).

### Models

Models represent the table for users, categories and expenses.

### Important files

- Program.cs - the entry point into the app
- MoExpenseTracker.http - used as the api client to make api requests

## Some resources

How to CRUD

- [ASP.NET Core Full Course For Beginners](https://www.youtube.com/watch?v=AhAxLiGC7Pc) by Julio Casal
- [Build CRUD with .NET 6 Web API & Entity Framework Core](https://www.youtube.com/watch?v=wtFs4356xp4) by Mohamad Lawand

How to use ef core and relationship (I didn't really much of these but you'd understand)

- [Getting Started with Entity Framework Core in .NET](https://www.youtube.com/watch?v=2t88FOeQ898) by Nick Chapsas
- [DbContext, Code First Migrations and Database Creation with EFCore](https://www.youtube.com/watch?v=A4tpHy__LN0) by ISeeSharp
- [1 to 1, 1 to Many and Many to Many Relationships with EFCore in 2023](https://www.youtube.com/watch?v=9sXXfq0GDYI)  by ISeeSharp

How to connect to a database

- [Getting Started with Entity Framework Core in .NET](https://www.youtube.com/watch?v=2t88FOeQ898) by Nick Chapsas

How to do authentication and authorization in dotnet

- [How to implement JWT authentication in ASP.NET Core](https://www.infoworld.com/article/2336284/how-to-implement-jwt-authentication-in-aspnet-core.html) by Joydip Kanjilal
- [JWT Authentication in ASP.NET Core Minimal API](https://dotnettutorials.net/lesson/jwt-authentication-in-asp-net-core-minimal-api) by dotnettutorials

How to hash password - hash and verify

- [Best Practices for Hashing and Salting Passwords in .NET](https://www.youtube.com/watch?v=Sh_PxjTmBug) by Code Maze
- [Best Practices for Secure Password Hashing in .NET (Stop Storing Passwords in Plain Text!)](https://www.youtube.com/watch?v=J4ix8Mhi3rs) by Milan Jovanović
- [How to hash a password](https://stackoverflow.com/questions/4181198/how-to-hash-a-password/73125177#73125177) by [arad](https://stackoverflow.com/users/7734384/arad)

How to handle exceptions (global)

- [Global Exception Handling in Asp.Net Core Web API using IExceptionHandler](https://www.youtube.com/watch?v=bEYlNuwTSms) by Nitish Kaushik
- [The New Global Error Handling in ASP.NET Core 8](https://www.youtube.com/watch?v=uOEDM0c9BNI)  by Milan Jovanović

How to route endpoints (group routes)

- [NET Core 7 : Routing - UseRouting vs UseEndpoints // Map vs MapGet vs MapPost](https://www.youtube.com/watch?v=NCZzYxzHrN8) by hikitoc
-[How To Organize Minimal API Endpoints Inside Of Clean Architecture](https://www.youtube.com/watch?v=GCuVC_qDOV4)  by Milan Jovanović

How to create extension methods

- [What are Extension Methods in C#? When to use extension methods in real applications?](https://www.youtube.com/watch?v=JDNPJyiu3Ec) by Interview Happy

How to validate request

- [New AddEndpointFilter in .Net 7.0, filters in Minimal API](https://www.youtube.com/watch?v=rOr-7sNKUds) by Tech In Talk (endpoint filters)
- [Coding Shorts: Minimal API Endpoint Filters for Model Validation](https://www.youtube.com/watch?v=_S-r6SxLGn4) by Shawn Wildermuth (+endpoint filters & fluent validation)
- [Creating your first validator](https://docs.fluentvalidation.net/en/latest/start.html) by Fluentvalidation docs

MISC (extra information, won't hurt to watch, read or run through)

- [Mastering Minimal API's Dotnet 7](https://www.youtube.com/playlist?list=PLlfN4N9fXldtMXcDqtJFstYVtjX_Xt-gY) by Tech In Talk (dotnet)
- [Back-end Web Development with .NET](https://www.youtube.com/playlist?list=PLdo4fOcmZ0oWunQnm3WnZxJrseIw2zSAk) by dotnet (dotnet)
- [IResult/Results/TypedResult or IActionResult/ActionResult/controllerBase methods?](https://www.reddit.com/r/dotnet/comments/172wumc/iresultresultstypedresult_or/) (difference between IResults, ITypeResults, ...)
- [.NET dependency injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes) (difference between scoped, singleton, transcient)
