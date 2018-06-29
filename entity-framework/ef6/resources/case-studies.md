---
title: "Case Studies for Entity Framework - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers
ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: cd5d3ae3-717d-4095-a2ef-0e8fd72b1a2f
caps.latest.revision: 3
---
# Microsoft Case Studies for Entity Framework
The case studies on this page highlight a few real-world production projects that have employed Entity Framework.
> [!NOTE]
> The detailed case studies originally linked when this page as first published are no longer available on the Microsoft website. Therefore the links have been removed.

## Epicor
Epicor is a large global software company (with over 400 developers) that develops Enterprise Resource Planning (ERP) solutions for companies in more than 150 countries.
Their flagship product, Epicor 9, is based on a Service-Oriented Architecture (SOA) using the .NET Framework.
Faced with numerous customer requests to provide support for Language Integrated Query (LINQ), and also wanting to reduce the load on their back-end SQL Servers, the team decided to upgrade to Visual Studio 2010 and the .NET Framework 4.0.
Using the Entity Framework 4.0, they were able to achieve these goals and also greatly simplify development and maintenance.
In particular, the Entity Framework’s rich T4 support allowed them to take full control of their generated code and automatically build in performance-saving features such as pre-compiled queries and caching.

> “We conducted some performance tests recently with existing code, and we were able to reduce the requests to SQL Server by 90 percent.
That is because of the ADO.NET Entity Framework 4.” – Erik Johnson, Vice President, Product Research  

## Veracity Solutions
Having acquired an event-planning software system that was going to be difficult to maintain and extend over the long-term, Veracity Solutions used Visual Studio 2010 to re-write it as a powerful and easy-to-use Rich Internet Application built on Silverlight 4.
Using .NET RIA Services, they were able to quickly build a service layer on top of the Entity Framework that avoided code duplication and allowed for common validation and authentication logic across tiers.  

> “We were sold on the Entity Framework when it was first introduced, and the Entity Framework 4 has proven to be even better.
Tooling is improved, and it’s easier to manipulate the .edmx files that define the conceptual model, storage model, and mapping between those models...
With the Entity Framework, I can get that data access layer working in a day—and build it out as I go along.
The Entity Framework is our de facto data access layer; I don’t know why anyone wouldn’t use it.” – Joe McBride, Senior Developer

## NEC Display Solutions of America
NEC wanted to enter the market for digital place-based advertising with a solution to benefit advertisers and network owners and increase its own revenues.
In order to do that, it launched a pair of web applications that automate the manual processes required in a traditional ad campaign.
The sites were built using ASP.NET, Silverlight 3, AJAX and WCF, along with the Entity Framework in the data access layer to talk to SQL Server 2008.

> “With SQL Server, we felt we could get the throughput we needed to serve advertisers and networks with information in real time and the reliability to help ensure that the information in our mission-critical applications would always be available”- Mike Corcoran, Director of IT

## Darwin Dimensions
Using a wide range of Microsoft technologies, the team at Darwin set out to create Evolver - an online avatar portal that consumers could use to create stunning, lifelike avatars for use in games, animations, and social networking pages.
With the productivity benefits of the Entity Framework, and pulling in components like Windows Workflow Foundation (WF) and Windows Server AppFabric (a highly-scalable in-memory application cache), the team was able to deliver an amazing product in 35% less development time.
Despite having team members split across multiple countries, the team following an agile development process with weekly releases.

 > “We try not to create technology for technology’s sake. As a startup, it is crucial that we leverage technology that saves time and money.
 .NET was the choice for fast, cost-effective development.” – Zachary Olsen, Architect  

## Silverware
With more than 15 years of experience in developing point-of-sale (POS) solutions for small and midsize restaurant groups, the development team at Silverware set out to enhance their product with more enterprise-level features in order to attract larger restaurant chains.
Using the latest version of Microsoft’s development tools, they were able to build the new solution four times faster than before.
Key new features like LINQ and the Entity Framework made it easier to move from Crystal Reports to SQL Server 2008 and SQL Server Reporting Services (SSRS) for their data storage and reporting needs.

> “Effective data management is key to the success of SilverWare – and this is why we decided to adopt SQL Reporting.” - Nicholas Romanidis, Director of IT/Software Engineering
