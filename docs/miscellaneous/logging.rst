Logging
=======

.. contents:: `In this article:`
    :local:

.. include:: /_shared/sample.txt
.. _sample: https://github.com/aspnet/EntityFramework.Docs/tree/master/docs/miscellaneous/logging/sample

Create a logger
---------------

The first step is to create an implementation of ``ILoggerProvider`` and ``ILogger``.
 * ``ILoggerProvider`` is the component that decides when to create instances of your logger(s). The provider may chose to create different loggers in different situations.
 * ``ILogger`` is the component that does the actual logging. It will be passed information from the framework when certain events occur.

Here is a simple implementation that logs a human readable representation of every event to a text file and the Console.

.. literalinclude:: logging/sample/EFLogging/MyLoggerProvider.cs
        :language: csharp
        :linenos:

.. tip::
  The arguments passed to the Log method are:
   * ``logLevel`` is the level (e.g. Warning, Info, Verbose, etc.) of the event being logged
   * ``eventId`` is a library/assembly specific id that represents the type of event being logged
   * ``state`` can be any object that holds state relevant to what is being logged
   * ``exception`` gives you the exception that occurred if an error is being logged
   * ``formatter`` uses state and exception to create a human readable string to be logged

Register your logger
--------------------

ASP.NET Core
^^^^^^^^^^^^

In an ASP.NET Core application, you register your logger in the Configure method of Startup.cs::

  public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
  {
      loggerFactory.AddProvider(new MyLoggerProvider());

      ...
  }

Other applications
^^^^^^^^^^^^^^^^^^

In your application startup code, create and instance of you context and register your logger.

.. note::
  You only need to register the logger with a single context instance. Once you have registered it, it will be used for all other instances of the context in the same AppDomain.

.. literalinclude:: logging/sample/EFLogging.ConsoleApp/Program.cs
        :language: csharp
        :linenos:
        :lines: 12-17

Filtering what is logged
------------------------

The easiest way to filter what is logged, is to adjust your logger provider to only return your logger for certain categories of events. For EF, the category passed to your logger provider will be the type name of the component that is logging the event.

For example, here is a logger provider that returns the logger only for events related to executing SQL against a relational database. For all other categories of events, a null logger (which does nothing) is returned.

.. literalinclude:: logging/sample/EFLogging/MyFilteredLoggerProvider.cs
        :language: csharp
        :linenos:
        :emphasize-lines: 9-13, 17-22
