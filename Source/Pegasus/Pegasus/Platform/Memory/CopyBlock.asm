.class public auto ansi abstract sealed beforefieldinit Pegasus.Platform.Memory.CopyBlock
	extends System.Object
{
	.method public hidebysig static void Copy(void* destination, void* source, int32 byteCount) cil managed
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		ldarg.2
		cpblk
		ret
	}
} 