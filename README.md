# #develop (short for SharpDevelop) - Fork by Gabs

## Overview

**SharpDevelop (Fork by Gabs)** is a customized legacy fork of the #develop IDE. This fork focuses on restoring maximum compatibility with nostalgic and legacy development environments, streamlining templates, resolving HTTPS/TLS connectivity issues with older package repositories, and locking the project framework targets.

### Fork Highlights & Customizations

1. **Target Framework & Architecture Defaults**
   - **Target Framework Downgrade**: All core projects and active assemblies are downgraded to **.NET Framework 4.5.2** to fit legacy environment constraints.
   - **32-Bit (x86) Default Target**: Configured the build targets and default project templates to target **32-bit x86** architecture out-of-the-box for maximum execution compatibility.

2. **NuGet TLS Connection & Security Fixes**
   - **TLS 1.2 Enablement**: Standardized connections to support TLS 1.2. The PowerShell script environments, package restore processes, and HTTP registry configurations are preset to enforce `StrongCrypto` and TLS 1.2 protocols. This resolves the NuGet connection errors (*"The underlying connection was closed: An unexpected error occurred on a send"*).

3. **Streamlined Templates & AddIns**
   - **C# Only Focus**: Excluded non-essential project template trees like Silverlight, WCF, WPF, and VB.NET templates. 
   - **Cleaned AddIns**: Excluded resource-intensive and modern VCS add-ins (Git, SVN, Profilers, F# integration, C++, Wix, etc.) in the solution to keep the IDE extremely lightweight.

4. **IDE Branding & Version Consistency**
   - **Title Bar Customization**: Branded the main window title to show `"SharpDevelop (Fork by Gabs)"`.
   - **Assembly Version Lock**: Locked the build assembly version at `5.2.0.5290` to avoid startup popup dependency conflicts after new git commits.

Looking for the tech notes (Fine Art of Commenting, Coding Style Guide, and more)? These can be found as rtf files in [doc/technotes](doc/technotes)

## How To Compile
 #Develop can be compiled using the supplied `.bat` files (`debugbuild.bat` or `releasebuild.bat`), or in #Develop itself.

## System Requirements (running #Develop)

 - Windows 7 SP1 or higher.
 - [.NET 4.5.2 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net452)
 
## Extended Requirements (building #Develop)

 - [.NET 4.5.2 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net452)
 - Build tools:
   - [Microsoft Build Tools 2013](https://www.microsoft.com/en-us/download/details.aspx?id=40760) (For support MSBuild 12.0)
   - [Microsoft Build Tools 2015](https://www.microsoft.com/en-us/download/details.aspx?id=48159) (For support MSBuild 14.0)
   - And one of the
       - [Visual Studio 2017 Build Tools](https://aka.ms/vs/15/release/vs_buildtools.exe)
       - [Visual Studio 2019 Build Tools](https://aka.ms/vs/16/release/vs_buildtools.exe)
       - [Visual Studio 2022 Build Tools](https://aka.ms/vs/17/release/vs_buildtools.exe)
 - [Windows SDK](http://www.microsoft.com/en-us/download/details.aspx?id=3138) (optional; C++ compiler needed for profiler)
 - If you have cloned the SD git repository: git must be available on your PATH
 
## Libraries and Integrated tools (some links is not actual):

* [Avalon Dock](http://avalondock.codeplex.com/) : New BSD License (BSD) (thanks to **Adolfo Marinucci**)
* [Graph#](https://graphsharp.codeplex.com/)
* [IQToolkit](https://iqtoolkit.codeplex.com/)
* [Irony](https://irony.codeplex.com/)
* [ITextSharp](http://sourceforge.net/projects/itextsharp/)
* [log4Net](https://github.com/apache/log4net)
* Mono T4
* [Mono.Cecil](https://github.com/jbevain/cecil): MIT License (thanks to **Jb Evain**)
* [Sharp Svn](https://sharpsvn.open.collab.net/)
* [SQLite](https://sqlite.org/)
* [WPFToolkit](https://wpf.codeplex.com/)

## Integrated Tools (packaged with #Develop):

* [IronPython](http://ironpython.net/)
* [IronRuby](https://ironruby.codeplex.com/)
* [NuGet](https://nuget.codeplex.com/)
* [NUnit](http://www.nunit.org/)
* [OpenCover](https://github.com/OpenCover/opencover)
* [WiX](https://wix.codeplex.com/)

## Reusable Libraries (Part of #Develop):

* [AvalonEdit](http://avalonedit.net/)
* [Debugger.Core](https://github.com/icsharpcode/SharpDevelop/tree/master/src/AddIns/Debugger/Debugger.Core)
* [ICSharpCode.Core](https://github.com/icsharpcode/SharpDevelop/tree/master/src/Main/Core)
* [ICSharpCode.Decompiler](https://github.com/icsharpcode/SharpDevelop/tree/master/src/Libraries/ICSharpCode.Decompiler)
* [NRefactory](https://github.com/icsharpcode/NRefactory)
* [SharpTreeView](https://github.com/icsharpcode/SharpDevelop/tree/master/src/Libraries/SharpTreeView)
* [WPF Designer]( https://github.com/icsharpcode/SharpDevelop/tree/master/src/AddIns/DisplayBindings/WpfDesign)

## #Develop Contributors

### Developers

* [Mike KrÃ¼ger](https://github.com/mkrueger) (Project Founder)
* [Daniel Grunwald](https://github.com/dgrunwald) (Technical Lead)
* [Andreas Weizel](https://github.com/Rpinski)
* [Matt Ward](https://github.com/mrward)
* [David Srbecky](https://github.com/dsrbecky)(Debugger)
* [Siegfried Pammer](https://github.com/siegfriedpammer)
* [Peter Forstmeier]( https://github.com/PeterForstmeier)(#Develop Reports)	

### Non-Developers

* Christoph Wille (PM)
* Bernhard Spuida (Kalfaktor)

### Past Developers (Non-Exhaustive List)

* [Mike KrÃ¼ger](https://github.com/mkrueger) (Project Founder)
* Alexandre Semenov
* Andrea Paatz
* Christian Hornung
* David Alpert
* Denis ERCHOFF
* Dickon Field
* Georg Brandl
* Ifko Kovacka
* Itai Bar-Haim
* Ivan Shumilin
* John Reilly
* John Simons
* Justin Dearing
* Markus Palme
* Mathias Simmack
* Matt Everson
* Nathan Allan
* Nikola Kavaldjiev
* Philipp Maihart
* Poul Staugaard
* Robert Pickering
* Robert Zaunere
* Roman Taranchenko
* Russell Wilkins
* Scott Ferrett
* Sergej Andrejev
* Shinsaku Nakagawa
* Tomasz Tretkowski
* Troy Simpson

###### Copyright 2014 AlphaSierraPapa for the SharpDevelop team. SharpDevelop is distributed under the MIT license.

