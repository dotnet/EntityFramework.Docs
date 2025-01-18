---
title: Self-tracking entities - EF6
description: Self-tracking entities in Entity Framework 6
author: SamMonoRT
ms.date: 10/23/2016
uid: ef6/fundamentals/disconnected-entities/self-tracking-entities/index
---
# Self-tracking entities

> [!IMPORTANT]
> We no longer recommend using the self-tracking-entities template. It will only continue to be available to support existing applications. If your application requires working with disconnected graphs of entities, consider other alternatives such as [Trackable Entities](https://trackableentities.github.io/), which is a technology similar to Self-Tracking-Entities that is more actively developed by the community, or writing custom code using the low-level change tracking APIs.

In an Entity Framework-based application, a context is responsible for tracking changes in your objects. You then use the SaveChanges method to persist the changes to the database. When working with N-Tier applications, the entity objects are usually disconnected from the context and you must decide how to track changes and report those changes back to the context. Self-Tracking Entities (STEs) can help you track changes in any tier and then replay these changes into a context to be saved.  

Use STEs only if the context is not available on a tier where the changes to the object graph are made. If the context is available, there is no need to use STEs because the context will take care of tracking changes.  

This template item generates two .tt (text template) files:  

- The **\<model name\>.tt** file generates the entity types and a helper class that contains the change-tracking logic that is used by self-tracking entities and the extension methods that allow setting state on self-tracking entities.  
- The **\<model name\>.Context.tt** file generates a derived context and an extension class that contains **ApplyChanges** methods for the **ObjectContext** and **ObjectSet** classes. These methods examine the change-tracking information that is contained in the graph of self-tracking entities to infer the set of operations that must be performed to save the changes in the database.  

## Get Started  

To get started, visit the [Self-Tracking Entities Walkthrough](xref:ef6/fundamentals/disconnected-entities/self-tracking-entities/walkthrough) page.  

## Functional Considerations When Working with Self-Tracking Entities  
> [!IMPORTANT]
> We no longer recommend using the self-tracking-entities template. It will only continue to be available to support existing applications. If your application requires working with disconnected graphs of entities, consider other alternatives such as [Trackable Entities](https://trackableentities.github.io/), which is a technology similar to Self-Tracking-Entities that is more actively developed by the community, or writing custom code using the low-level change tracking APIs.

Consider the following when working with self-tracking entities:  

- Make sure that your client project has a reference to the assembly containing the entity types. If you add only the service reference to the client project, the client project will use the WCF proxy types and not the actual self-tracking entity types. This means that you will not get the automated notification features that manage the tracking of the entities on the client. If you intentionally do not want to include the entity types, you will have to manually set change-tracking information on the client for the changes to be sent back to the service.  
- Calls to the service operation should be stateless and create a new instance of object context. We also recommend that you create object context in a **using** block.  
- When you send the graph that was modified on the client to the service and then intend to continue working with the same graph on the client, you have to manually iterate through the graph and call the **AcceptChanges** method on each object to reset the change tracker.  

    > If objects in your graph contain properties with database-generated values (for example, identity or concurrency values), Entity Framework will replace values of these properties with the database-generated values after the **SaveChanges** method is called. You can implement your service operation to return saved objects or a list of generated property values for the objects back to the client. The client would then need to replace the object instances or object property values with the objects or property values returned from the service operation.  
- Merging graphs from multiple service requests may introduce objects with duplicate key values in the resulting graph. Entity Framework does not remove the objects with duplicate keys when you call the **ApplyChanges** method but instead throws an exception. To avoid having graphs with duplicate key values follow one of the patterns described in the following blog: [Self-Tracking Entities: ApplyChanges and duplicate entities](https://go.microsoft.com/fwlink/?LinkID=205119&clcid=0x409).  
- When you change the relationship between objects by setting the foreign key property, the reference navigation property is set to null and not synchronized to the appropriate principal entity on the client. After the graph is attached to the object context (for example, after you call the **ApplyChanges** method), the foreign key properties and navigation properties are synchronized.  

    > Not having a reference navigation property synchronized with the appropriate principal object could be an issue if you have specified cascade delete on the foreign key relationship. If you delete the principal, the delete will not be propagated to the dependent objects. If you have cascade deletes specified, use navigation properties to change relationships instead of setting the foreign key property.  
- Self-tracking entities are not enabled to perform lazy loading.  
- Binary serialization and serialization to ASP.NET state management objects is not supported by self-tracking entities. However, you can customize the template to add the binary serialization support. For more information, see [Using Binary Serialization and ViewState with Self-Tracking Entities](https://go.microsoft.com/fwlink/?LinkId=199208).  

## Security Considerations  

The following security considerations should be taken into account when working with self-tracking entities:  

- A service should not trust requests to retrieve or update data from a non-trusted client or through a non-trusted channel. A client must be authenticated: a secure channel or message envelope should be used. Clients' requests to update or retrieve data must be validated to ensure they conform to expected and legitimate changes for the given scenario.  
- Avoid using sensitive information as entity keys (for example, social security numbers). This mitigates the possibility of inadvertently serializing sensitive information in the self-tracking entity graphs to a client that is not fully trusted. With independent associations, the original key of an entity that is related to the one that is being serialized might be sent to the client as well.  
- To avoid propagating exception messages that contain sensitive data to the client tier, calls to **ApplyChanges** and **SaveChanges** on the server tier should be wrapped in exception-handling code.  
