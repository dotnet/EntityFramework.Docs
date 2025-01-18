---
title: Creating a Model - EF6
description: Creating a Model in Entity Framework 6
author: SamMonoRT
ms.date: 07/05/2018
uid: ef6/modeling/index
---
# Creating a Model

An EF model stores the details about how application classes and properties map to database tables and columns. There are two main ways to create an EF model:

- **Using Code First**: The developer writes code to specify the model. EF generates the models and mappings at runtime based on entity classes and additional model configuration provided by the developer.

- **Using the EF Designer**: The developer draws boxes and lines to specify the model using the EF Designer. The resulting model is stored as XML in a file with the EDMX extension. The application's domain objects are typically generated automatically from the conceptual model.

## EF workflows

Both of these approaches can be used to target an existing database or create a new database, resulting in 4 different workflows.
Find out about which one is best for you:  

|                                           | I just want to write code...                                                                                                                   | I want to use a designer...                                                                                                                        |
|:------------------------------------------|:-----------------------------------------------------------------------------------------------------------------------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------|
| **I am creating a new database**          | [Use **Code First** to define your model in code and then generate a database.](xref:ef6/modeling/code-first/workflows/new-database)           | [Use **Model First** to define your model using boxes and lines and then generate a database.](xref:ef6/modeling/designer/workflows/model-first)   |
| **I need to access an existing database** | [Use **Code First** to create a code based model that maps to an existing database.](xref:ef6/modeling/code-first/workflows/existing-database) | [Use **Database First** to create a boxes and lines model that maps to an existing database.](xref:ef6/modeling/designer/workflows/database-first) |

### Watch the video: What EF workflow should I use?

This short video explains the differences, and how to find the one that is right for you.

**Presented By**: [Rowan Miller](https://romiller.com/)

![Which Workflow Thumb](../media/whichworkflow-thumb.png)
 [WMV](https://download.microsoft.com/download/8/F/8/8F81F4CD-3678-4229-8D79-0C63FFA3C595/HDI_ITPro_Technet_winvideo_ChoseYourWorkflow.wmv) | [MP4](https://download.microsoft.com/download/8/F/8/8F81F4CD-3678-4229-8D79-0C63FFA3C595/HDI_ITPro_Technet_mp4video_ChoseYourWorkflow.m4v) | [WMV (ZIP)](https://download.microsoft.com/download/8/F/8/8F81F4CD-3678-4229-8D79-0C63FFA3C595/HDI_ITPro_Technet_winvideo_ChoseYourWorkflow.zip)

If after watching the video you still don't feel comfortable deciding if you want to use the EF Designer or Code First, learn both!

## A look under the hood

Regardless of whether you use Code First or the EF Designer, an EF model always has several components:

- The application's domain objects or entity types themselves. This is often referred to as the object layer

- A conceptual model consisting of domain-specific entity types and relationships, described using the [Entity Data Model](xref:ef6/resources/glossary#entity-data-model). This layer is often referred to with the letter "C", for _conceptual_.

- A storage model representing tables, columns and relationships as defined in the database. This layer is often referred to with the later "S", for _storage_.  

- A mapping between the conceptual model and the database schema. This mapping is often referred to as "C-S" mapping.

EF's mapping engine leverages the "C-S" mapping to transform operations against entities - such as create, read, update, and delete - into equivalent operations against tables in the database.

The mapping between the conceptual model and the application's objects is often referred to as "O-C" mapping. Compared to the "C-S" mapping, "O-C" mapping is implicit and one-to-one: entities, properties and relationships defined in the conceptual model are required to match the shapes and types of the .NET objects. From EF4 and beyond, the objects layer can be composed of simple objects with properties without any dependencies on EF. These are usually referred to as Plain-Old CLR Objects (POCO) and mapping of types and properties is performed base on name matching conventions. Previously, in EF 3.5 there were specific restrictions for the object layer, like entities having to derive from the EntityObject class and having to carry EF attributes to implement the "O-C" mapping.
