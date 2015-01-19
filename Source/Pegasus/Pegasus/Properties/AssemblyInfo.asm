.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                      
  .ver 4:0:0:0
}

.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )      
  .ver 4:0:0:0
}

.assembly extern System.Runtime
{
  .publickeytoken = (b03f5f7f11d50a3a)      
  .ver 4:0:0:0
}

.assembly extern System.Runtime.InteropServices
{
  .publickeytoken = (b03f5f7f11d50a3a)      
  .ver 4:0:0:0
}

.assembly Pegasus.IL
{
    .ver 1:0:0:0
	.custom instance void [mscorlib]System.Runtime.CompilerServices.InternalsVisibleToAttribute::.ctor(string) = (
		01 00 07 50 65 67 61 73 75 73 00 00 // Pegasus
	)

	.custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = (
		01 00 1a 2e 4e 45 54 46 72 61 6d 65 77 6f 72 6b
		2c 56 65 72 73 69 6f 6e 3d 76 34 2e 35 01 00 54
		0e 14 46 72 61 6d 65 77 6f 72 6b 44 69 73 70 6c
		61 79 4e 61 6d 65 12 2e 4e 45 54 20 46 72 61 6d
		65 77 6f 72 6b 20 34 2e 35
	)
}

.module Pegasus.IL.dll