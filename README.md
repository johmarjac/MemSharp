# MemSharp
MemSharp is a library written in C++/CLI which can be injected into any process. It sets up a TCP Server and allows to dynamically load and unload assemblies to the process.

# Setup
1. Download any injector.
2. Open up MemSharp solution.
3. Open up MemSharpClient -> Program.cs and set the Working Directory Path according to your needs
4. Build the whole solution
5. Grab the injector and inject the MemSharpHost.dll to your target process.
6. Launch the client and look at Program.cs on how to use it.
7. Create a new project targeting .NET Standard 2.0 and reference MemSharpCommon library.
8. Create a class which inherits from `Script` class.
9. Do your stuff in that class.
10. If you compile your library with `/unsafe` you can access the target process' memory using C# Pointer Arithmetic.

Have Fun

# FAQ
* Q: Why does my target process crash when I inject the MemSharpHost library?

* A: You actually have to copy over `Ether.Network.dll` and `MemSharpCommon.dll` to the target process location. (Yes I know that's not very cool, but I may write an injector which will do that for you.)
