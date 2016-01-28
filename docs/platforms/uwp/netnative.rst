.NET Native
===========

Fixing Reflection Errors
------------------------

Applications using EF on Universal Windows Platform may encounter errors due to current limitations in .NET Native.
These errors usually appear as ``MissingRuntimeArtifactException`` or ``MissingMetadataException``, which are casued by incomplete reflection support in .NET Native. In most cases,
these errors can be resolved by adding the proper `Runtime Directive <https://msdn.microsoft.com/en-us/library/dn600639(v=vs.110).aspx>`_.

.. note::
    The following blog post includes more detail about how to correctly fix reflection errors on .NET Native:

        `.NET Native Deep Dive: Help! I Hit a MissingMetadataException! <http://blogs.msdn.com/b/dotnet/archive/2014/05/21/net-native-deep-dive-help-i-hit-a-missingmetadataexception.aspx>`_

.. tip::
    This troubleshooter will help generate the appropriate entry for your runtime directives file:

        `MissingMetadataException troubleshooter <http://dotnet.github.io/native/troubleshooter/type.html>`_


Additional Reading
------------------
For more information, the following articles may be helpful.

 - `.NET Native – What it means for Universal Windows Platform (UWP) developers <https://blogs.windows.com/buildingapps/2015/08/20/net-native-what-it-means-for-universal-windows-platform-uwp-developers/>`_
 - `Reflection and .NET Native <https://msdn.microsoft.com/en-us/library/dn600640(v=vs.110).aspx>`_
 - `Compiling Apps with .NET Native <https://msdn.microsoft.com/en-us/library/dn584397(v=vs.110).aspx>`_