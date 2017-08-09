---
title: "Entity Framework N-Tier - EF6"
author: divega
ms.date: "2016-10-23"
ms.prod: "entity-framework"
ms.author: divega
ms.manager: avickers


ms.technology: entity-framework-6
ms.topic: "article"
ms.assetid: 12138003-a373-4817-b1b7-724130202f5f
caps.latest.revision: 3
---
# Entity Framework N-Tier
In an Entity Framework-based application, a context is responsible for tracking changes in your objects. You then use the SaveChanges method to persist the changes to the database. When working with N-Tier applications, the entity objects are usually disconnected from the context and you must decide how to track changes and report those changes back to the context. This topic discusses different options that are available when using Entity Framework in N-Tier applications.  

You can use one of the following options to transfer data between tiers and track changes on a disconnected tier when working with Entity Framework.  

## ASP.NET Web API  

ASP.NET Web API is a framework that makes it easy to build HTTP services that reach a broad range of clients, including browsers and mobile devices. ASP.NET Web API is an ideal platform for building RESTful applications on the .NET Framework. The ASP.NET Web API tooling in Visual Studio makes it easy to scaffold a Web API controller from your Entity Framework model. ASP.NET Web API is great if you want a lot of control over how your data is retrieved and saved by clients. For more information, see [ASP.NET Web API](http://www.asp.net/web-api).  

## WCF Data Services  

WCF Data Services enables you to create services that use the Open Data Protocol (OData) to expose and consume data over the Web or intranet. OData uses the same Entity Data Model as Entity Framework, making it easy to build an OData end point using your Entity Framework Model. WCF Data Services is great if you want a quick and easy way to expose your model and data over the web. For more information, see [WCF Data Services](https://msdn.microsoft.com/library/cc668772.aspx).  

## Entity Framework APIs  

If you don't want to use an existing N-Tier solution, Entity Framework provides APIs that allow you to apply changes made on a disconnected tier. You have various design options when it comes to implementing the logic to apply these changes. [Programming Entity Framework: DbContext](http://shop.oreilly.com/product/0636920022237.do) by Julia Lerman and Rowan Miller, talks about this subject in great detail. For more information, see [Add/Attach and Entity States](../ef6/entity-framework-add-and-attach-and-entity-states.md). ***Be aware that this can be a complex problem to solve and we would recommend using one of the solutions listed above if possible.***  

## Self-Tracking Entities  

There is a Self-Tracking Entities code generation template that can be used with models created using the EF Designer. This template will generate entity classes that contain logic to track changes made on a disconnected tier. A set of extension methods is also generated to apply those changes to a context. **We no longer recommend using the STE template, it continues to be available to support existing applications - we recommend using one of the solution listed above.** Self-Tracking Entities can not be used with Code First models. For more information, see [Self-Tracking Entities](../ef6/entity-framework-self-tracking-entities.md).  
