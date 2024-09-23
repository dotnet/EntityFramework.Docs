---
title: Architecture - EF Core
description: The internal architecture of Entity Framework Core
author: SamMonoRT
ms.date: 11/27/2023
uid: core/miscellaneous/internals/index
---
# EF Core Architecture

In 2010, Scott Hanselman associated Entity Framework with a magic unicorn. That's because there's a lot of magic going on behind the scenes that users generally doesn't have to think about. If you've ever wondered how some of that magic works, these docs are for you. They cover the architecture and implementation details of various parts of EF Core.

## Articles

Here are the articles we've written so far. Use issue [#1920](https://github.com/dotnet/EntityFramework.Docs/issues/1920) to let us know what else you'd like to see!

- [Design-time tools](xref:core/miscellaneous/internals/tools)

## Community Standup

The EF team produces community standup videos, where we discuss various aspects of .NET and data access. Some of these have explored EF internals, and can be a good way to understand the general architecture of EF.

- [How to Add a Feature to EF Core](https://www.youtube.com/watch?v=9OMxy1wal1s&list=PLdo4fOcmZ0oX0ObHwBrJ0vJpZ7PiYMqeA)
- [Internal Dependency Injection](https://www.youtube.com/watch?v=pYhe-Mt0HzI&list=PLdo4fOcmZ0oX0ObHwBrJ0vJpZ7PiYMqeA)
- [DbContext Configuration and Lifetime](https://www.youtube.com/watch?v=NPgFlqXPbK8&list=PLdo4fOcmZ0oX0ObHwBrJ0vJpZ7PiYMqeA)
- [IQueryable, LINQ and the EF Core query pipeline](https://www.youtube.com/watch?v=1Ld3dtnTrMw&list=PLdo4fOcmZ0oX0ObHwBrJ0vJpZ7PiYMqeA)
- [Model Building](https://www.youtube.com/watch?v=FYz0rAxQkC8&list=PLdo4fOcmZ0oX0ObHwBrJ0vJpZ7PiYMqeA)

## See also

- [ASP.NET Core Architecture Series](https://www.youtube.com/playlist?list=PLdo4fOcmZ0oUDW57sHWGT9Cfp9lAEx3lU)
