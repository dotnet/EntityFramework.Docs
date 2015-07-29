- Bugs in Mono 4.0.2 may cause Entity Framework to crash when using async methods. This is resolved with Mono >4.2.0, which has not yet been publicly released. `See aspnet/EntityFramework#2708 <https://github.com/aspnet/EntityFramework/issues/2708>`_ on GitHub
- Migrations on SQLite do not support more complex schema changes due to limitations in SQLite itself.

- Bugs in beta 6. See `Workarounds`_.
    - Migrations requires that you have a "Startup" class in your project. `Issue #2357 <https://github.com/aspnet/EntityFramework/issues/2357>`_. 
    - Migrations adds an annotation that will cause a build error. `Issue #2545 <https://github.com/aspnet/EntityFramework/issues/2545>`_.