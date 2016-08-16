Maximum Length
==============

Configuring a maximum length provides a hint to the data store about the appropriate data type to use for a given property. Maximum length only applies to array data types, such as ``string`` and ``byte[]``.

.. note::
    Entity Framework does not do any validation of maximum length before passing data to the provider. It is up to the provider or data store to validate if appropriate. For example, when targeting SQL Server, exceeding the maximum length will result in an exception as the data type of the underlying column will not allow excess data to be stored.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, it is left up to the database provider to choose an appropriate data type for properties. For properties that have a length, the database provider will generally choose a data type that allows for the longest length of data. For example, Microsoft SQL Server will use ``nvarchar(max)`` for ``string`` properties (or ``nvarchar(450)`` if the column is used as a key).

Data Annotations
----------------

You can use the Data Annotations to configure a maximum length for a property. In this example, targeting SQL Server this would result in the ``nvarchar(500)`` data type being used.

.. includesamplefile:: Modeling/DataAnnotations/Samples/MaxLength.cs
        :language: c#
        :lines: 11-16
        :emphasize-lines: 4
        :linenos:

Fluent API
----------

You can use the Fluent API to configure a maximum length for a property. In this example, targeting SQL Server this would result in the ``nvarchar(500)`` data type being used.

.. includesamplefile:: Modeling/FluentAPI/Samples/MaxLength.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 7-9
        :linenos:
