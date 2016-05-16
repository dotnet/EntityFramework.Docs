.. include:: /_shared/rc2-notice.txt

.. include:: _shared/relational-specific-note.txt

Sequences
=========

A sequence generates a sequential numeric values in the database. Sequences are not associated with a specific table.

Conventions
-----------

By convention, sequences are not introduced in to the model.

Data Annotations
----------------

You can not configure a sequence using Data Annotations.

Fluent API
----------

You can use the Fluent API to create a sequence in the model.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/Sequence.cs
        :language: c#
        :lines: 5-15
        :emphasize-lines: 7
        :linenos:

You can also configure additional aspect of the sequence, such as its schema, start value, and increment.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/SequenceConfigured.cs
        :language: c#
        :lines: 5-15
        :emphasize-lines: 7-9
        :linenos:

Once a sequence is introduced, you can use it to generate values for properties in your model. For example, you can use :doc:`default-values` to insert the next value from the sequence.

.. literalinclude:: /modeling/configuring/sample/EFModeling.Configuring.FluentAPI/Samples/Relational/SequenceUsed.cs
        :language: c#
        :lines: 5-26
        :emphasize-lines: 11-13
        :linenos:
