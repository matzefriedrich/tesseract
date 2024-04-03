This project is a fork of the https://github.com/charlesw/tesseract repository.

## Why another fork?

### The solution has been migrated to .NET 8.0, and ...

* conditional compilation symbols have been removed.
* The code has been cleaned and reformated to automatically adopt the latest syntax sugar. Thus, the project is no
  longer compatible with its upstream source. If you must rely on an already-known upgrade path, stick to the original
  project instead.
* support for Tesseract.Drawing has been removed (might get restored).

### The aim of this fork is...

...having a project that works with the latest .NET Framework version (because .NET 4.8 and .NET Core 3.1 are dowdy).
The dynamic runtime approach (based on DotNetInterop) is excellent but is overhead and adds complexity that I´d like to
eliminate. Besides that, the evolvability and stability of the codebase do not meet my needs yet, so it requires tweaks.

**Improvements to code readability**. There are some low-hanging fruits like:

* Enablement of strict nullability (and addressing all compiler warnings)
* Linearization of code-flow that leads to reduced nesting, for instance,
    * by preferring `using`-declarations instead of `using`-blocks if it does not extend a resource`s lifetime
    * by inverting conditional statements (exit early)
* Inlining of out-variables and moving variable declarations closer to their usage
* Replacement of custom guard clauses with built-in guard clauses
* Substitution of `string.Format` statements by string interpolation
* ...

**Aligning the wrapper API with SOTA architectural concepts as found in the .NET space**, for instance:

* Consistent usage of DI; the goal is to bring hidden dependencies to the surface and turn singleton implementations
  into service configurations with a singleton lifetime behavior (reduces static cling and boosts idempotency).
* Adoption of the module system introduced by .NET Core (improved separation of concerns, reliable abstractions,
  interfaces, and primitive types)
* Changing stateful types into stateless types (some classes need to perform probable erroneous state-checks on objects
  whose type implements the `IDisposable` interface)
* Adoption of performance benchmarks (no more performance measurements in unit tests)
* Evaluation of compilation-time code generators (as a replacement for dynamic code based on reflection emit)
* ...

## Prerequisites

* Visual Studio 2022, or Jetbrains Rider 2023+
* Visual Studio 2019 C++ runtime; see https://visualstudio.microsoft.com/downloads/
* Tesseract language files; see https://github.com/tesseract-ocr/tessdata/

## Build

Run the following command from the Visual Studio command prompt to restore referenced Nuget packages and build the
projects:

````bash
$ cd src
$ msbuild ./Tesseract.sln /p:Configuration=Release
````

## License

Copyright 2012-2022 Charles Weld.

Licensed under the [Apache License, Version 2.0][apache2] (the "License"); you
may not use this software except in compliance with the License. You may obtain
a copy of the License at:

[http://www.apache.org/licenses/LICENSE-2.0][apache2]

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.

#### InteropDotNet

Copyright 2014 Andrey Akinshin
Project URL: https://github.com/AndreyAkinshin/InteropDotNet
Distributed under the MIT License: http://opensource.org/licenses/MIT

### Contributors

* [charlesw](https://github.com/charlesw) (Charles Weld)

A big thanks to GitHub and all of Tesseract's contributors:

* [AndreyAkinshin](https://github.com/AndreyAkinshin)
* [jakesays](https://github.com/jakesays)
* [peters](https://github.com/peters)
* [nguyenq](https://github.com/nguyenq)
* [Sojin1989](https://github.com/Sojin1989)
* [jeschergui](https://github.com/jeschergui)

Also thanks to the following projects\resources without which this project would not exist in its current form:

* [InteropDotNet](https://github.com/AndreyAkinshin/InteropDotNet) - For developing a dynamic interop system that allows
  tesseract to be used from both mono and .net.
* [Reactive Extensions](http://rx.codeplex.com/) - The basic idea from which the build\packaging system is built on.
* [TwainDotNet](https://github.com/tmyroadctfig/twaindotnet) - Batch build script
* [Tesseract-dot-net](https://code.google.com/p/tesseractdotnet) - The original dot net wrapper that started all this.
* [Interop with Native Libraries](http://www.mono-project.com/Interop_with_Native_Libraries) - Stacks of useful
  information about c# P/Invoke and Marshalling

[apache2]: http://www.apache.org/licenses/LICENSE-2.0

[tesseract-ocr]: https://github.com/tesseract-ocr/tesseract
