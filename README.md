# pillar
Pillar is an E-Commerce API, designed to cut down the cost of implementing a full-packed shopping app, mostly monolithic, from scratch. 
This API lets you plug in any front-end application, without having to bother about backend implementation. 
It facilitates most e-commerce functions, from customer accounts to checkout payments.

## Platform and Framework
This API was built with the .NET Core framework, and can run on Linux, Windows, and macOS platforms

## Tools and Technologies
#### IDE
Visual Studio Community 2017

#### Major Technologies
1. ASP.NET Core 2.2 with C#
1. Entity Framework Core v2.2.4
2. MySql (Can be re-configured with a different database)

#### Dependencies
1. Microsoft.EntityFrameWorkCore v2.2.4
2. Pomelo.EntityFrameworkCore.MySql v2.1.2
3. Serilog.AspNetCore 
4. Stripe.net
5. Microsoft.Extensions.Logging

## Architecture
This project uses a multi-layered architecture, leveraging an 'MVC-like' pattern. It captilizes on separation of concerns
and heavily relies on the 'Repository-Pattern' approach of Entity Framework for "domain - data access" integration.
Below is a diagram that illustrates this architecture:

![Pillar-Simple-Architecture](https://github.com/EddyQay/pillar/blob/master/Resources/simple_architecture_s.png)

## Project Structure
#### Controllers:
This directory contains all the controllers used in the API. The controllers are the gateway to the API, 
such that they recieve all requests coming to the API, and then route them to the appropriate Workers to process them.

#### DomainModels:
This directory contains data structures that are purposefully built to hold data for manipulation by the workers, 
for exchanging data in a customized way within the application, and for presenting and accepting data to and from
customer requests. They do not map to the database, rather, they encapsulate data models or individual properties.

#### Models:
Contains all data models that map directly to database tables, and the DatabaseContext or the Repository. Models stored in 
here are entity objects that represent tables in the database. They define the Data Access Layer of the application.

#### Workers:
This directory contains classes that have the main business logic of the API. They have direct access 
to the Data Access Layer of the application, and it is where the Workers for the entire application are located.

#### Utilities:
Contains classes that handle functions such as secrets generation, token management, and other functions that otherwise do not fit
into the data access layer. They are the primary business

#### Middlewares:
This directory contains classes that plug directly into the .NET Core frawework flow. The implementations in the classes mostly
override the primary functions in .NET Core, for what they do. Example: A middleware class called 'CustomAuthorizeAttribute' 
that overrides the 'AuthorizeAttribute' in .NET Core.

#### Logs:
The Logs folder contains all logs for the API. Whenever there's something to log, a log file is created in
this log directory.

#### appsettings.json
In this file contains the settings for the API. Contents generally include database connection strings, passwords,
and other values. For security reasons, it is generally discouraged to store sensitive data in this file, since it 
gets checked into source control, and hence might by publicly visible. Secrets vaults and secrets.json file are avaialble
to setup for a more production ready environment. This can be easily configured.

#### Startup.cs
This file contains the major configuration for the entire application. Configuration settings setup here take in effect
globally in the entire application, and it is the first place looked when the application strarts up.

## Recommendations

#### A half of the daily active users comes from United States. How do you design a new system to handle this case?
I could think about a few things that could matter with geographical region.
If the concern is to satisfy the U.S. more since thy hold a larger user popuplation, then

1. Currency conversion: Since most of the users are from the U.S., then it would makes sense to set the base currency for
prices to USD. so that most customers would have a freindly pricing flow.

2. Second has to do with latency. The farther away the application is from their user, the slower it is to get to them.
So it best to host the aplication closer to the greater user region.

However, if the concern is about bridging the gap between users, so that people in other regions would equally patronize, then
I could think of globalization using localization. This could be in the form of 

1. Lanuguage: This could matter really in the documentation of the API, so that develpoers who are not familiar with English
could equally access the resouces in their language, and thiswould increase patronage in other regions outside th U.S 

2. Currency inclusions: include currency conversion for various countries, so people can benefit from a friendly picing model

#### The current system can support 100,000 daily active users. How do you design a new system to support 1,000,000 daily active users?
I think aside hosting region, I can think of multi-threading and multi-processing.
1. Asynchronous programming utilizes multiple threads that leverage the computing power of their hosts, to respond to muiltiple
requests concurrently without waiting time. THis greatly enhances the ability to have more users at a time without, it having a toll
on performance. This API performs all requests asynchronously.

2. Multi-processing: Depending on the capabilities of the host machine, applications could target the muitlple cores for multiple
processes. This would be fully useful on multi-core processors, but greatly inefficeint on single core processors.

## Security
This API upholds all the major security standards that help withstand cyber attacks:
1. SQL Injection - All inputs are encoded to filter out dangerous characters, and this is done by the 'ApiController' attribute
2. XSS and Cross Site Request Forgery Attack - This API does not use cookies in any way, so the attack surface for cookie, spoofing and cross-site scripting is negligible
beacause no sensitive data is autmomatically sent in every users' request to all sites.
3. Replay Attacks: the JWT implementation in this API marks every token for replay, and therefore validates the tokens against 
replay attacks, and the tokens are tamper-resistant.
4. ASP.NET Core has in-built functionality that guards against other major security attacks, including DDOS.

## Deployment:
The API can be tested locally, and it is supposd to run without problems.
However, after hosting the API, the DNS on the hosting provider's end had issues with name resolutions so could
access the database server. I am still trying to find a way around this.
the api is hosted Here:
>  http://pillar1.azurewebsites.net

I enabled debugging on the server for testing purposes.


