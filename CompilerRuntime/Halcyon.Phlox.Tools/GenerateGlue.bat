bin\Debug\Halcyon.Phlox.Tools.exe defgen ..\..\grammar\funcs.txt ..\..\grammar\Shim.stg > ..\Halcyon.Phlox\Types\Defaults.cs
bin\Debug\Halcyon.Phlox.Tools.exe shimgen ..\..\grammar\funcs.txt ..\..\grammar\Shim.stg > ..\Halcyon.Phlox\Glue\SyscallShim.cs
bin\Debug\Halcyon.Phlox.Tools.exe apigen ..\..\grammar\funcs.txt ..\..\grammar\Shim.stg > ..\Halcyon.Phlox\Glue\ISystemAPI.cs