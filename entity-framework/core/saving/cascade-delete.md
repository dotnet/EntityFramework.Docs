---
title: Cascade Delete - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: ee8e14ec-2158-4c9c-96b5-118715e2ed9e
uid: core/saving/cascade-delete
---
# Cascade Delete

Cascade delete is commonly used in database terminology to describe a characteristic that allows the deletion of a row to automatically trigger the deletion of related rows. A closely related concept also covered by EF Core delete behaviors is the automatic deletion of a child entity when it's relationship to a parent has been severed--this is commonly known as "deleting orphans".

EF Core implements several different delete behaviors and allows for the configuration of the delete behaviors of individual relationships. EF Core also implements conventions that automatically configure useful default delete behaviors for each relationship based on the [requiredness of the relationship](../modeling/relationships.md#required-and-optional-relationships).

## Delete behaviors
Delete behaviors are defined in the *DeleteBehavior* enumerator type and can be passed to the *OnDelete* fluent API to control whether the deletion of a principal/parent entity or the severing of the relationship to dependent/child entities should have a side effect on the dependent/child entities.

There are three actions EF can take when a principal/parent entity is deleted or the relationship to the child is severed:
* The child/dependent can be deleted
* The child's foreign key values can be set to null
* The child remains unchanged

> [!NOTE]  
> The delete behavior configured in the EF Core model is only applied when the principal entity is deleted using EF Core and the dependent entities are loaded in memory (that is, for tracked dependents). A corresponding cascade behavior needs to be setup in the database to ensure data that is not being tracked by the context has the necessary action applied. If you use EF Core to create the database, this cascade behavior will be setup for you.

For the second action above, setting a foreign key value to null is not valid if foreign key is not nullable. (A non-nullable foreign key is equivalent to a required relationship.) In these cases, EF Core tracks that the foreign key property has been marked as null until SaveChanges is called, at which time an exception is thrown because the change cannot be persisted to the database. This is similar to getting a constraint violation from the database.

There are four delete behaviors, as listed in the tables below.

### Optional relationships
For optional relationships (nullable foreign key) it _is_ possible to save a null foreign key value, which results in the following effects:

| Behavior Name               | Effect on dependent/child in memory    | Effect on dependent/child in database  |
|:----------------------------|:---------------------------------------|:---------------------------------------|
| **Cascade**                 | Entities are deleted                   | Entities are deleted                   |
| **ClientSetNull** (Default) | Foreign key properties are set to null | None                                   |
| **SetNull**                 | Foreign key properties are set to null | Foreign key properties are set to null |
| **Restrict**                | None                                   | None                                   |

### Required relationships
For required relationships (non-nullable foreign key) it is _not_ possible to save a null foreign key value, which results in the following effects:

| Behavior Name         | Effect on dependent/child in memory | Effect on dependent/child in database |
|:----------------------|:------------------------------------|:--------------------------------------|
| **Cascade** (Default) | Entities are deleted                | Entities are deleted                  |
| **ClientSetNull**     | SaveChanges throws                  | None                                  |
| **SetNull**           | SaveChanges throws                  | SaveChanges throws                    |
| **Restrict**          | None                                | None                                  |

In the tables above, *None* can result in a constraint violation. For example, if a principal/child entity is deleted but no action is taken to change the foreign key of a dependent/child, then the database will likely throw on SaveChanges due to a foreign constraint violation.

At a high level:
* If you have entities that cannot exist without a parent, and you want EF to take care for deleting the children automatically, then use *Cascade*.
  * Entities that cannot exist without a parent usually make use of required relationships, for which *Cascade* is the default.
* If you have entities that may or may not have a parent, and you want EF to take care of nulling out the foreign key for you, then use *ClientSetNull*
  * Entities that can exist without a parent usually make use of optional relationships, for which *ClientSetNull* is the default.
  * If you want the database to also try to propagate null values to child foreign keys even when the child entity is not loaded, then use *SetNull*. However, note that the database must support this, and configuring the database like this can result in other restrictions, which in practice often makes this option impractical. This is why *SetNull* is not the default.
* If you don't want EF Core to ever delete an entity automatically or null out the foreign key automatically, then use *Restrict*. Note that this requires that your code keep child entities and their foreign key values in sync manually otherwise constraint exceptions will be thrown.

> [!NOTE]
> In EF Core, unlike EF6, cascading effects do not happen immediately, but instead only when SaveChanges is called.

> [!NOTE]  
> **Changes in EF Core 2.0:** In previous releases, *Restrict* would cause optional foreign key properties in tracked dependent entities to be set to null, and was the default delete behavior for optional relationships. In EF Core 2.0, the *ClientSetNull* was introduced to represent that behavior and became the default for optional relationships. The behavior of *Restrict* was adjusted to never have any side effects on dependent entities.

## Entity deletion examples

The code below is part of a [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Saving/Saving/CascadeDelete/) that can be downloaded and run. The sample shows what happens for each delete behavior for both optional and required relationships when a parent entity is deleted.

[!code-csharp[Main](../../../samples/core/Saving/Saving/CascadeDelete/Sample.cs#DeleteBehaviorVariations)]

Let's walk through each variation to understand what is happening.

### DeleteBehavior.Cascade with required or optional relationship

```
  After loading entities:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  After deleting blog '1':
    Blog '1' is in state Deleted with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  Saving changes:
    DELETE FROM [Posts] WHERE [PostId] = 1
    DELETE FROM [Posts] WHERE [PostId] = 2
    DELETE FROM [Blogs] WHERE [BlogId] = 1

  After SaveChanges:
    Blog '1' is in state Detached with 2 posts referenced.
      Post '1' is in state Detached with FK '1' and no reference to a blog.
      Post '2' is in state Detached with FK '1' and no reference to a blog.
```

* Blog is marked as Deleted
* Posts initially remain Unchanged since cascades do not happen until SaveChanges
* SaveChanges sends deletes for both dependents/children (posts) and then the principal/parent (blog)
* After saving, all entities are detached since they have now been deleted from the database

### DeleteBehavior.ClientSetNull or DeleteBehavior.SetNull with required relationship

```
  After loading entities:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  After deleting blog '1':
    Blog '1' is in state Deleted with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  Saving changes:
    UPDATE [Posts] SET [BlogId] = NULL WHERE [PostId] = 1

  SaveChanges threw DbUpdateException: Cannot insert the value NULL into column 'BlogId', table 'EFSaving.CascadeDelete.dbo.Posts'; column does not allow nulls. UPDATE fails. The statement has been terminated.
```

* Blog is marked as Deleted
* Posts initially remain Unchanged since cascades do not happen until SaveChanges
* SaveChanges attempts to set the post FK to null, but this fails because the FK is not nullable

### DeleteBehavior.ClientSetNull or DeleteBehavior.SetNull with optional relationship

```
  After loading entities:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  After deleting blog '1':
    Blog '1' is in state Deleted with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  Saving changes:
    UPDATE [Posts] SET [BlogId] = NULL WHERE [PostId] = 1
    UPDATE [Posts] SET [BlogId] = NULL WHERE [PostId] = 2
    DELETE FROM [Blogs] WHERE [BlogId] = 1

  After SaveChanges:
    Blog '1' is in state Detached with 2 posts referenced.
      Post '1' is in state Unchanged with FK 'null' and no reference to a blog.
      Post '2' is in state Unchanged with FK 'null' and no reference to a blog.
```

* Blog is marked as Deleted
* Posts initially remain Unchanged since cascades do not happen until SaveChanges
* SaveChanges attempts sets the FK of both dependents/children (posts) to null before deleting the principal/parent (blog)
* After saving, the principal/parent (blog) is deleted, but the dependents/children (posts) are still tracked
* The tracked dependents/children (posts) now have null FK values and their reference to the deleted principal/parent (blog) has been removed

### DeleteBehavior.Restrict with required or optional relationship

```
  After loading entities:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  After deleting blog '1':
    Blog '1' is in state Deleted with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  Saving changes:
  SaveChanges threw InvalidOperationException: The association between entity types 'Blog' and 'Post' has been severed but the foreign key for this relationship cannot be set to null. If the dependent entity should be deleted, then setup the relationship to use cascade deletes.
```

* Blog is marked as Deleted
* Posts initially remain Unchanged since cascades do not happen until SaveChanges
* Since *Restrict* tells EF to not automatically set the FK to null, it remains non-null and SaveChanges throws without saving

## Delete orphans examples

The code below is part of a [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Saving/Saving/CascadeDelete/) that can be downloaded and run. The sample shows what happens for each delete behavior for both optional and required relationships when the relationship between a parent/principal and its children/dependents is severed. In this example, the relationship is severed by removing the dependents/children (posts) from the collection navigation property on the principal/parent (blog). However, the behavior is the same if the reference from dependent/child to principal/parent is instead nulled out.

[!code-csharp[Main](../../../samples/core/Saving/Saving/CascadeDelete/Sample.cs#DeleteOrphansVariations)]

Let's walk through each variation to understand what is happening.

### DeleteBehavior.Cascade with required or optional relationship

```
  After loading entities:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  After making posts orphans:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Modified with FK '1' and no reference to a blog.
      Post '2' is in state Modified with FK '1' and no reference to a blog.

  Saving changes:
    DELETE FROM [Posts] WHERE [PostId] = 1
    DELETE FROM [Posts] WHERE [PostId] = 2

  After SaveChanges:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Detached with FK '1' and no reference to a blog.
      Post '2' is in state Detached with FK '1' and no reference to a blog.
```

* Posts are marked as Modified because severing the relationship caused the FK to be marked as null
  * If the FK is not nullable, then the actual value will not change even though it is marked as null
* SaveChanges sends deletes for dependents/children (posts)
* After saving, the dependents/children (posts) are detached since they have now been deleted from the database

### DeleteBehavior.ClientSetNull or DeleteBehavior.SetNull with required relationship

```
  After loading entities:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  After making posts orphans:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Modified with FK 'null' and no reference to a blog.
      Post '2' is in state Modified with FK 'null' and no reference to a blog.

  Saving changes:
    UPDATE [Posts] SET [BlogId] = NULL WHERE [PostId] = 1

  SaveChanges threw DbUpdateException: Cannot insert the value NULL into column 'BlogId', table 'EFSaving.CascadeDelete.dbo.Posts'; column does not allow nulls. UPDATE fails. The statement has been terminated.
```

* Posts are marked as Modified because severing the relationship caused the FK to be marked as null
  * If the FK is not nullable, then the actual value will not change even though it is marked as null
* SaveChanges attempts to set the post FK to null, but this fails because the FK is not nullable

### DeleteBehavior.ClientSetNull or DeleteBehavior.SetNull with optional relationship

```
  After loading entities:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  After making posts orphans:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Modified with FK 'null' and no reference to a blog.
      Post '2' is in state Modified with FK 'null' and no reference to a blog.

  Saving changes:
    UPDATE [Posts] SET [BlogId] = NULL WHERE [PostId] = 1
    UPDATE [Posts] SET [BlogId] = NULL WHERE [PostId] = 2

  After SaveChanges:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Unchanged with FK 'null' and no reference to a blog.
      Post '2' is in state Unchanged with FK 'null' and no reference to a blog.
```

* Posts are marked as Modified because severing the relationship caused the FK to be marked as null
  * If the FK is not nullable, then the actual value will not change even though it is marked as null
* SaveChanges sets the FK of both dependents/children (posts) to null
* After saving, the dependents/children (posts) now have null FK values and their reference to the deleted principal/parent (blog) has been removed

### DeleteBehavior.Restrict with required or optional relationship

```
  After loading entities:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Unchanged with FK '1' and reference to blog '1'.
      Post '2' is in state Unchanged with FK '1' and reference to blog '1'.

  After making posts orphans:
    Blog '1' is in state Unchanged with 2 posts referenced.
      Post '1' is in state Modified with FK '1' and no reference to a blog.
      Post '2' is in state Modified with FK '1' and no reference to a blog.

  Saving changes:
  SaveChanges threw InvalidOperationException: The association between entity types 'Blog' and 'Post' has been severed but the foreign key for this relationship cannot be set to null. If the dependent entity should be deleted, then setup the relationship to use cascade deletes.
```

* Posts are marked as Modified because severing the relationship caused the FK to be marked as null
  * If the FK is not nullable, then the actual value will not change even though it is marked as null
* Since *Restrict* tells EF to not automatically set the FK to null, it remains non-null and SaveChanges throws without saving

## Cascading to untracked entities

When you call *SaveChanges*, the cascade delete rules will be applied to any entities that are being tracked by the context. This is the situation in all the examples shown above, which is why SQL was generated to delete both the principal/parent (blog) and all the dependents/children (posts):

```sql
    DELETE FROM [Posts] WHERE [PostId] = 1
    DELETE FROM [Posts] WHERE [PostId] = 2
    DELETE FROM [Blogs] WHERE [BlogId] = 1
```

If only the principal is loaded--for example, when a query is made for a blog without an `Include(b => b.Posts)` to also include posts--then SaveChanges will only generate SQL to delete the principal/parent:

```sql
    DELETE FROM [Blogs] WHERE [BlogId] = 1
```

The dependents/children (posts) will only be deleted if the database has a corresponding cascade behavior configured. If you use EF to create the database, this cascade behavior will be setup for you.
