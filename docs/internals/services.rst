Understanding EF Services
=========================

.. contents:: `In this article`

Entity Framework executes as a collection of services working together. A
service is a reusable component. In C#, a services is typically an
implementation of an interface. Services are is available to other services via
`dependency injection (DI) <https://wikipedia.org/wiki/Dependency_injection>`_.
This article covers some fundamentals principals for understanding how EF uses
services and DI.

Categories of Services
----------------------

Services fall into one or more categories.

Context services
  Services that related to interaction with a specific instance of
  ``DbContext``. These services provide functionality for working with the user
  model and context options.

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

1. Start with the ``ServiceProvider`` injected in the DbContext constructor (*user* and *design-time services*). If no services are provided, create a new instance.
2. Extract any ``DbContextOptions`` relevant to the current DbContext type from service provider.
3. Finish getting options by calling ``DbContext.OnConfiguring()``. This can overwrite options extracted in step 2.
4. Add all services found in extensions added to ``DbContextOptions.Extensions`` (*provider services*).
5. Create a service scope. Any services whose lifespan is defined as "scoped" will only operate within this scope. This is used to isolate instance of ``DbContext``.
6. Initialize *context services* from this service scope.
7. As configured in options, select one and only one set of *provider services* from ``IDatabaseServices``.

Together, the scoped service provider from step 5, the context services from
step 6, and the selected provider services from step 7 are the final set of
services used by EF.

Service Lifetimes
-----------------

EF services can be registered with different lifetime options. The suitable
option depends on how the service is used and implemented.

Transient
  Transient lifetime services are created each time they are requested.

Scoped
  Scoped lifetime services are created once per ``DbContext`` instance.

Singleton
  Singleton lifetime services are created the first time they are requested, and
  then every subsequent request will use the same instance.

Instance
  You can choose to add an instance directly to the services container. If you
  do so, this instance will be used for all subsequent requests (this technique
  will create a Singleton-scoped instance). One key difference between Instance
  services and Singleton services is that the Instance service is created in
  during service initialization, while the Singleton service is lazy-loaded the
  first time it is requested.


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

`"System.IServiceProvider" <http://dotnet.github.io/api/System.IServiceProvider.html>`_ is defined in the .NET base class library.
