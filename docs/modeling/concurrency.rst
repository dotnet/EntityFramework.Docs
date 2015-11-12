.. include:: /_shared/rc1-notice.txt

Concurrency Tokens
==================

If a property is configured as a concurrency token then EF will check that no other user has modified that value in the database when saving changes to that record. EF uses an optimistic concurrency pattern, meaning it will assume the value has not changed and try to save the data, but throw if it finds the value has been changed.

For example we may want to configure ``SocialSecurityNumber`` on ``Person`` to be a concurrency token. This means that if one user tries to save some changes to a Person, but another user has changed the ``SocialSecurityNumber`` then an exception will be thrown. This may be desirable so that your application can prompt the user to ensure this record still represents the same actual person before saving their changes.

.. contents:: In this article:
    :depth: 3

How concurrency tokens work in EF
---------------------------------

Data stores can enforce concurrency tokens by checking that any record being updated or deleted still has the same value for the concurrency token that was assigned when the context originally loaded the data from the database.

For example, relational database achieve this by including the concurrency token in the ``WHERE`` clause of any ``UPDATE`` or ``DELETE`` commands and checking the number of rows that were affected. If the concurrency token still matches then one row will be updated. If the value in the database has changed, then no rows are updated.

.. code-block:: sql

    UPDATE [Person] SET [Name] = @p1
    WHERE [PersonId] = @p0 AND [SocialSecurityNumber] = @p2;

Conventions
-----------

By convention, properties are never configured as concurrency tokens.

Data Annotations
----------------

You can use the Data Annotations to configure a property as a concurrency token.

.. literalinclude:: configuring/sample/EFModeling.Configuring.DataAnnotations/Samples/Concurrency.cs
        :language: c#
        :lines: 11-17
        :emphasize-lines: 4
        :linenos:

Fluent API
----------

You can use the Fluent API to configure a property as a concurrency token.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Concurrency.cs
        :language: c#
        :lines: 5-22
        :emphasize-lines: 7-9
        :linenos:
