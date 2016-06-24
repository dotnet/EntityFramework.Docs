===========================
Writing a Database Provider
===========================

.. contents:: `In this article`
  :local:

EF Core is designed to be extensible. It provides general purpose building
blocks that are intended for use in multiple providers. The purpose of this
article is to provide basic guidance on creating a new provider that is
compatible with EF Core.

.. tip::
  `EF Core source code is open-source <https://github.com/aspnet/EntityFramework>`_.
  The best source of information is the code itself.

.. tip::
  This article shows snippets from an empty EF provider. You can view the `full
  stubbed-out provider
  <https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/Miscellaneous/Internals/WritingAProvider>`_
  on GitHub.

.. _entry-point:

DbContext Initialization
------------------------

A user's interaction with EF begins with the ``DbContext`` constructor. Before
the context is available for use, it initializes **options** and **services**.
We will example both of these to understand what they represent and how EF
configures itself to use different providers.

Options
^^^^^^^

``Microsoft.EntityFrameworkCore.Infrastructure.DbContextOptions`` is the API
surface for **users** to configure ``DbContext``. Provider writers are
responsible for creating API to configure options and to make services
responsive to these options. For example, most providers require a connection
string. These options are typically created using ``DbContextOptionsBuilder``.

Services
^^^^^^^^

``System.IServiceProvider`` is the main interface used for interaction with services.
EF makes heavy use of `dependency injection (DI)
<https://wikipedia.org/wiki/Dependency_injection>`_. The ``ServiceProvider``
contains a collection of services available for injection. Initialization uses
``DbContextOptions`` to add additional services if needed and select
a scoped set of services that all EF operations will use during execution.

See also :doc:`services`.

.. note::
  EF uses `Microsoft.Extensions.DependencyInjection <https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/>`_
  to implement dependency injection. Documentation for this project
  `is available on docs.asp.net <https://docs.asp.net/en/latest/fundamentals/dependency-injection.html>`_.

Plugging in a Provider
----------------------

As explained above, EF uses options and services. Each provider must create API
so users to add provider-specific options and services. This API is best created
by using extension methods.

.. tip::
  When defining an extension method, define it in the namespace of the object
  being extended so Visual Studio auto-complete will include the extension
  method as a possible completion.

The `Use` Method
^^^^^^^^^^^^^^^^

By convention, providers define a ``UseX()`` extension on ``DbContextOptionsBuilder``.
This configures **options** which it typically takes as arguments to method.

::

  optionsBuilder.UseMyProvider("Server=contoso.com")

The ``UseX()`` extension method creates a provider-specific implementation of
``IDbContextOptionsExtension`` which is added to the collection of extensions
stored within ``DbContextOptions``. This is done by a call to the a hidden API
``IDbContextOptionsBuilderInfrastructure.AddOrUpdateExtension``.

.. includesamplefile:: Miscellaneous/Internals/WritingAProvider/EntityFrameworkCore.ProviderStarter/Extensions/MyProviderDbContextOptionsExtensions.cs
  :language: csharp
  :caption: An example implementation of the "Use" method
  :linenos:
  :lines: 6-19

.. tip::
  The ``UseX()`` method can also be used to return a special wrapper around
  ``DbContextOptionsBuilder`` that allows users to configure multiple options
  with chained calls. See ``SqlServerDbContextOptionsBuilder`` as an example.

The `Add` Method
^^^^^^^^^^^^^^^^

By convention, providers define a ``AddX()`` extension on
``EntityFrameworkServicesBuilder``. This configures **services** and does not
take arguments.

``EntityFrameworkServicesBuilder`` is a wrapper around ``ServiceCollection``
which is accessible by calling ``GetInfrastructure()``. The ``AddX()`` method
should register services in this collection to be available for dependency
injection.

In some cases, users may call the `Add` method directly. This is
done when users are configuring a service provider manually and use this service
provider to resolve an instance of ``DbContext``. In other cases, the `Add` method
is called by EF upon service initialization. For more details on service
initialization, see :doc:`services`.

A provider *must register* an implementation of ``IDatabaseProvider``.
Implementing this in-turn requires configuring several more required services.
Read more about working with services in :doc:`services`.

EF provides many complete or partial implementations of the required services to
make it easier for provider-writers. For example, EF includes a class
``DatabaseProvider<TProviderServices, TOptionsExtension>`` which can be used in
service registration to hook up a provider.

.. includesamplefile:: Miscellaneous/Internals/WritingAProvider/EntityFrameworkCore.ProviderStarter/Extensions/MyProviderServiceCollectionExtensions.cs
  :language: csharp
  :linenos:
  :caption: An example implementation of the "Add" method
  :lines: 12-36

Next Steps
----------

With these two extensibility APIs now defined, users can now configure their
"DbContext" to use your provider. To make your provider functional, you will
need to implement other services.

Reading the source code of other providers is an excellent way to learn how to
create a new EF provider. See :doc:`/providers/index` for a list of current EF
providers and to find links to their source code (if applicable).

``Microsoft.EntityFrameworkCore.Relational`` includes an extensive library of services designed for relational providers. In many cases, these services need little or no modification to work for multiple relational databases.

For more information on other internal parts of EF, see :doc:`index`.
