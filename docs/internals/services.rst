Understanding EF Services
=========================

.. contents:: `In this article`
  :local:
  :depth: 1

Entity Framework executes as a collection of services working together. A
service is a reusable component. A service is typically an
implementation of an interface. Services are is available to other services via
`dependency injection (DI) <https://wikipedia.org/wiki/Dependency_injection>`_,
which is implemented in EF using `Microsoft.Extensions.DependencyInjection
<https://docs.asp.net/en/latest/fundamentals/dependency-injection.html>`_.

This article covers some fundamentals principles for understanding how EF uses
services and DI.

Categories of Services
----------------------

Services fall into one or more categories.

Context services
  Services that are related to a specific instance of  ``DbContext``. They
  provide functionality for working with the user model and context options.

Provider services
  Provider-specific implementations of services. For example, SQLite uses
  "provider services" to customize the behavior of SQL generation, migrations,
  and file I/O as needed.

Design-time services
  Services used when a developer is creating an application. For example, EF
  commands uses design-time services to execute migrations and code generation
  (aka scaffolding).

User services
  A user can define custom services to interact with EF. These are written in
  application code, not provider code. For example, users can provide an
  implementation of ``IModelCustomizer`` for controlling how a model is created.


Services Initialization
-----------------------

In order to be used by EF, all services must be registered in a service
collection before or during ``DbContext`` initialization. Registration is done
by adding components to into the ``ServiceCollection`` that EF will eventually
use to create the final ``ServiceProvider``. By the time initialization is
complete, ``DbContext`` has an instance of ``ServiceProvider`` that includes the
collection of provider services, context services, and user services necessary
for operation.

.. note::
  ``ServiceProvider`` is not to be confused with a "provider's services".

Initialization Steps
^^^^^^^^^^^^^^^^^^^^

EF will initialize services in the following order. Services registered later
can override or remove previously registered services.

1. ``DbContext`` obtains an initial set of services. This initial set can be
   internal or external. An external set must be found in order to use *user*
   and *design-time services*. The initial set is discovered using these steps:
  a. An **external** ``ServiceProvider`` can be passed in as a constructor
     parameter.
  b. If no constructor parameter is defined *and* ``DbContext`` is resolved from
     a service provider (i.e. the context was registered with
     ``AddDbContext<T>()``), then ``DbContextActivator`` will use the
     **external** service provider from which the context was resolved.
  c. If no external services can be found, ``DbContext`` creates an empty
     **internal** ``ServiceProvider`` and calls ``.AddEntityFramework()`` to
     add essential services.
2. Extract any ``DbContextOptions`` relevant to the current DbContext type from
   the initial service provider. These options are normally added to the
   initial service provider with a call to ``.AddDbContext<T>()``. If no options
   are in the service provider, EF creates an empty set of options.
3. Finish getting options by calling ``DbContext.OnConfiguring()``. This can
   overwrite options extracted in step 2.
4. Add all *provider services* given in the provider-specific implementation of
   ``IDatabaseProvider.GetProviderServices()``. This is typically found from the
   provider specific options extension (``IDbContextOptionsExtension``) which is
   added to options when the user calls the "UseProvider()" extension method.
   See :doc:`writing-a-provider` for more details.
5. Create a service scope. Any services with scoped `service lifetime`_ will
   only operate within this scope.
6. Initialize *context services* from this service scope.
7. As configured in options, select one and only one set of *provider services*
   from ``IDatabaseServices``.

Together, the scoped service provider from step 5, the context services from
step 6, and the selected provider services from step 7 are the final set of
services used by EF.


Service Lifetime
----------------

EF services can be registered with different lifetime options. The suitable
option depends on how the service is used and implemented.

Transient
  Transient lifetime services are created each time they are injected into other
  services. This isolates each instance of the service. For example,
  ``MigrationsScaffolder`` should not be reused, therefore it is registered as
  transient.

Scoped
  Scoped lifetime services are created once per ``DbContext`` instance. This is
  used to isolate instance of ``DbContext``. For example, ``StateManager``
  is added as scoped because it should only track entity states for one context.

Singleton
  Singleton lifetime services exists once per service provider and span all
  scopes. Each time the service is injected, the same instance is used. For
  example, ``IModelCustomizer`` is a singleton because it is idempotent, meaning
  each call to ``IModelCustomizer.Customize()`` does not change the customizer.

Required Provider Services
--------------------------

EF providers must register a basic set of services. These required services are
defined as properties on ``IDatabaseProviderServices``. Provider writers may
need to implement some services from scratch. Others have partial or complete
implementations in EF's library that can be reused.

Additional Information
----------------------

EF uses `Microsoft.Extensions.DependencyInjection
<https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/>`_ to
implement DI. Documentation for this library `is available on docs.asp.net
<https://docs.asp.net/en/latest/fundamentals/dependency-injection.html>`_.

`"System.IServiceProvider"
<http://dotnet.github.io/api/System.IServiceProvider.html>`_ is defined in the
.NET base class library.
