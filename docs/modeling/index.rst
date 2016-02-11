Modeling
========

Entity Framework uses a set of conventions to build a model based on the shape of your entity classes. You can specify additional configuration to supplement and/or override what was discovered by convention.

This article covers configuration that can be applied to a model targeting any data store and that which can be applied when targeting any relational database. Providers may also enable configuration that is specific to a particular data store. For documentation on provider specific configuration see the the :doc:`/providers/index` section.

In this section you can find information about conventions and configuration for the following:

.. toctree::
   :titlesonly:

   included-types
   included-properties
   keys
   generated-properties
   required-optional
   max-length
   concurrency
   shadow-properties
   relationships
   indexes
   alternate-keys
   inheritance
   backing-field
   relational/index

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/modeling/configuring/sample

Methods of configuration
------------------------

Fluent API
^^^^^^^^^^

You can override the ``OnModelCreating`` method in your derived context and use the ``ModelBuilder`` API to configure your model. This is the most powerful method of configuration and allows configuration to be specified without modifying your entity classes. Fluent API configuration has the highest precedence and will override conventions and data annotations.

.. literalinclude:: configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Required.cs
        :language: c#
        :lines: 5-15
        :emphasize-lines: 5-10
        :linenos:

Data Annotations
^^^^^^^^^^^^^^^^

You can also apply attributes (known as Data Annotations) to your classes and properties. Data annotations will override conventions, but will be overwritten by Fluent API configuration.

.. literalinclude:: configuring/sample/EFModeling.Configuring.DataAnnotations/Samples/Required.cs
        :language: c#
        :lines: 11-16
        :emphasize-lines: 4
        :linenos:
