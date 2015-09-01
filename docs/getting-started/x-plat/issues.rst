- Bugs in Mono 4.0.2 may cause Entity Framework to crash when using async methods. This is resolved with `Mono 4.2.0 <http://www.mono-project.com/docs/about-mono/releases/4.2.0/>`_, which is available as an alpha release (at time of writing).
- Migrations on SQLite do not support more complex schema changes due to limitations in SQLite itself.

- Bugs in beta 7. See `Workarounds`_.
    - Migrations requires that you have a "Startup" class in your project. `Issue #2357 <https://github.com/aspnet/EntityFramework/issues/2357>`_.
