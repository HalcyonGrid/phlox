# phlox

Build Status
* Master branch: [![Build status](https://ci.appveyor.com/api/projects/status/3lktfajt8jwkjdfi/branch/master?svg=true)](https://ci.appveyor.com/project/HalcyonGrid/phlox/branch/master)
* Latest release: [https://github.com/HalcyonGrid/phlox/releases](https://github.com/HalcyonGrid/phlox/releases)

The InWorldz Phlox VM and LSL compiler

Contained within is a compiler for the Linden Scripting Language and a corresponding virtual machine that can run the compiled code.

ANTLR3 grammar files are included.

This source code can be used to execute LSL code, or to create tools that operate on LSL source code facilitating features like code completion or refactoring.


## Compiling

Simply open the solution file under `CompilerRunner` in Visual Studio 2017 or a recent version of MonoDevelop and build.  The resulting binaries will be located in the `bin` folder that will be produced as a result.
