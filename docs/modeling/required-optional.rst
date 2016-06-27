Required/optional properties
============================

A property is considered optional if it is valid for it to contain ``null``. If ``null`` is not a valid value to be assigned to a property then it is considered to be a required property.

.. contents:: In this article:
    :depth: 3

Conventions
-----------

By convention, a property whose CLR type can contain null will be configured as optional (``string``, ``int?``, ``byte[]``, etc.). Properties whose CLR type cannot contain null will be configured as required (``int``, ``decimal``, ``bool``, etc.).

.. note::
    A property whose CLR type cannot contain null cannot be configured as optional. The property will always be considered required by Entity Framework.

Data Annotations
----------------

You can use Data Annotations to indicate that a property is required.

.. includesamplefile:: Modeling/DataAnnotations/Samples/Required.cs
        :language: c#
        :lines: 11-16
        :emphasize-lines: 4
        :linenos:

Fluent API
----------

You can use the Fluent API to indicate that a property is required.

.. includesamplefile:: Modeling/FluentAPI/Samples/Required.cs
        :language: c#
        :lines: 5-21
        :emphasize-lines: 7-9
        :linenos:
