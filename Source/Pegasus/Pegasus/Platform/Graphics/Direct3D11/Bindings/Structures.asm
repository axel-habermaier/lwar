.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 4

	.field private int32 _value

	.method public hidebysig specialname rtspecialname instance void .ctor (bool val) cil managed 
	{
		.maxstack 8

		ldarg.0
		ldarg.1
		brtrue.s IL_0007

		ldc.i4.0
		br.s IL_0008

		IL_0007: ldc.i4.1

		IL_0008: stfld int32 Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool::_value
		ret
	}

	.method public hidebysig specialname static bool op_Implicit (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool booleanValue) cil managed 
	{
		.maxstack 8

		ldarga.s booleanValue
		ldfld int32 Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool::_value
		ldc.i4.0
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig specialname static valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool op_Implicit (bool boolValue) cil managed 
	{
		.maxstack 8

		ldarg.0
		newobj instance void Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool::.ctor(bool)
		ret
	} 
}

.class private sequential ansi sealed Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Rectangle
	extends [System.Runtime]System.ValueType
{
	.field public int32 Left
	.field public int32 Top
	.field public int32 Right
	.field public int32 Bottom
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Viewport
	extends [System.Runtime]System.ValueType
{
	.pack 4
	.size 0

	.field public float32 X
	.field public float32 Y
	.field public float32 Width
	.field public float32 Height
	.field public float32 MinDepth
	.field public float32 MaxDepth
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendStateDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool AlphaToCoverageEnable
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool IndependentBlendEnable
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetBlendDescription RenderTarget0
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetBlendDescription RenderTarget1
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetBlendDescription RenderTarget2
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetBlendDescription RenderTarget3
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetBlendDescription RenderTarget4
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetBlendDescription RenderTarget5
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetBlendDescription RenderTarget6
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetBlendDescription RenderTarget7

} 

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetBlendDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool IsBlendEnabled
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendOption SourceBlend
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendOption DestinationBlend
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendOperation BlendOperation
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendOption SourceAlphaBlend
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendOption DestinationAlphaBlend
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendOperation AlphaBlendOperation
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ColorWriteMaskFlags RenderTargetWriteMask
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilStateDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool IsDepthEnabled
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthWriteMask DepthWriteMask
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Comparison DepthComparison
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool IsStencilEnabled
	.field public uint8 StencilReadMask
	.field public uint8 StencilWriteMask
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilOperationDescription FrontFace
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilOperationDescription BackFace
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilOperationDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11StencilOperation FailOperation
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11StencilOperation DepthFailOperation
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11StencilOperation PassOperation
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Comparison Comparison
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerStateDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11FillMode FillMode
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11CullMode CullMode
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool IsFrontCounterClockwise
	.field public int32 DepthBias
	.field public float32 DepthBiasClamp
	.field public float32 SlopeScaledDepthBias
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool IsDepthClipEnabled
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool IsScissorEnabled
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool IsMultisampleEnabled
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool IsAntialiasedLineEnabled
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerStateDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Filter Filter
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11TextureAddressMode AddressU
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11TextureAddressMode AddressV
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11TextureAddressMode AddressW
	.field public float32 MipLodBias
	.field public int32 MaximumAnisotropy
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Comparison ComparisonFunction
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Color BorderColor
	.field public float32 MinimumLod
	.field public float32 MaximumLod
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BufferDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public int32 SizeInBytes
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ResourceUsage Usage
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BindFlags BindFlags
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11CpuAccessFlags CpuAccessFlags
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ResourceOptionFlags OptionFlags
	.field public int32 StructureByteStride
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Data
	.field public int32 RowPitch
	.field public int32 SlicePitch
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Color
	extends [System.Runtime]System.ValueType
{
	.pack 4
	.size 0

	.field public float32 Red
	.field public float32 Green
	.field public float32 Blue
	.field public float32 Alpha
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputElement
		extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int SemanticName
	.field public int32 SemanticIndex
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat Format
	.field public int32 Slot
	.field public int32 AlignedByteOffset
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputClassification Classification
	.field public int32 InstanceDataStepRate
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize
	extends [System.Runtime]System.ValueType
{
	.pack 4
	.size 0
	.field private native int _size

	.method public hidebysig specialname rtspecialname instance void .ctor (int32 size) cil managed 
	{
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: newobj instance void [System.Runtime]System.IntPtr::.ctor(int32)
		IL_0007: stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize::_size
		IL_000c: ret
	}

	.method public hidebysig specialname static valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize op_Implicit (int32 val) cil managed 
	{
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: newobj instance void Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize::.ctor(int32)
		IL_0006: ret
	}
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11QueryDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11QueryType Type
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11QueryFlags Flags
}

.class private explicit ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.class nested public sequential ansi sealed beforefieldinit Texture2DMultisampledArrayResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 FirstArraySlice
		.field public int32 ArraySize
	}

	.class nested public sequential ansi sealed beforefieldinit Texture1DResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0
		
		.field public int32 MipSlice
	} 

	.class nested public sequential ansi sealed beforefieldinit Texture2DMultisampledResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 UnusedFieldNothingToDefine
	}

	.class nested public sequential ansi sealed beforefieldinit Texture3DResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MipSlice
		.field public int32 FirstDepthSlice
		.field public int32 DepthSliceCount
	}

	.class nested public sequential ansi sealed beforefieldinit Texture2DArrayResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MipSlice
		.field public int32 FirstArraySlice
		.field public int32 ArraySize
	}

	.class nested public sequential ansi sealed beforefieldinit Texture2DResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MipSlice
	}

	.class nested public explicit ansi sealed beforefieldinit BufferResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field [0] public int32 FirstElement
		.field [0] public int32 ElementOffset
		.field [4] public int32 ElementCount
		.field [4] public int32 ElementWidth
	}

	.class nested public sequential ansi sealed beforefieldinit Texture1DArrayResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MipSlice
		.field public int32 FirstArraySlice
		.field public int32 ArraySize

	} 

	.field [0] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat Format
	.field [4] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDimension Dimension
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription/BufferResource Buffer
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription/Texture1DResource Texture1D
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription/Texture1DArrayResource Texture1DArray
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription/Texture2DResource Texture2D
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription/Texture2DArrayResource Texture2DArray
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription/Texture2DMultisampledResource Texture2DMS
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription/Texture2DMultisampledArrayResource Texture2DMSArray
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription/Texture3DResource Texture3D
}

.class private explicit ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	// Nested Types
	.class nested public explicit ansi sealed beforefieldinit BufferResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field [0] public int32 FirstElement
		.field [0] public int32 ElementOffset
		.field [4] public int32 ElementCount
		.field [4] public int32 ElementWidth
	} 

	.class nested public sequential ansi sealed beforefieldinit ExtendedBufferResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 FirstElement
		.field public int32 ElementCount
		.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewExtendedBufferFlags Flags
	} 

	.class nested public sequential ansi sealed beforefieldinit Texture1DResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MostDetailedMip
		.field public int32 MipLevels
	} 

	.class nested public sequential ansi sealed beforefieldinit Texture1DArrayResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MostDetailedMip
		.field public int32 MipLevels
		.field public int32 FirstArraySlice
		.field public int32 ArraySize
	} 

	.class nested public sequential ansi sealed beforefieldinit Texture2DResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MostDetailedMip
		.field public int32 MipLevels
	} 

	.class nested public sequential ansi sealed beforefieldinit Texture2DArrayResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MostDetailedMip
		.field public int32 MipLevels
		.field public int32 FirstArraySlice
		.field public int32 ArraySize
	} 

	.class nested public sequential ansi sealed beforefieldinit Texture2DMultisampledResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 UnusedFieldNothingToDefine
	} // end of class Texture2DMultisampledResource

	.class nested public sequential ansi sealed beforefieldinit Texture2DMultisampledArrayResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 FirstArraySlice
		.field public int32 ArraySize
	} 

	.class nested public sequential ansi sealed beforefieldinit Texture3DResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MostDetailedMip
		.field public int32 MipLevels
	} 

	.class nested public sequential ansi sealed beforefieldinit TextureCubeResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MostDetailedMip
		.field public int32 MipLevels
	} 

	.class nested public sequential ansi sealed beforefieldinit TextureCubeArrayResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MostDetailedMip
		.field public int32 MipLevels
		.field public int32 First2DArrayFace
		.field public int32 CubeCount
	} 

	.field [0] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat Format
	.field [4] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDimension Dimension
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/BufferResource Buffer
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/Texture1DResource Texture1D
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/Texture1DArrayResource Texture1DArray
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/Texture2DResource Texture2D
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/Texture2DArrayResource Texture2DArray
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/Texture2DMultisampledResource Texture2DMS
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/Texture2DMultisampledArrayResource Texture2DMSArray
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/Texture3DResource Texture3D
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/TextureCubeResource TextureCube
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/TextureCubeArrayResource TextureCubeArray
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription/ExtendedBufferResource BufferEx
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Texture1DDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public int32 Width
	.field public int32 MipLevels
	.field public int32 ArraySize
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat Format
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ResourceUsage Usage
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BindFlags BindFlags
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11CpuAccessFlags CpuAccessFlags
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ResourceOptionFlags OptionFlags
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Texture2DDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public int32 Width
	.field public int32 Height
	.field public int32 MipLevels
	.field public int32 ArraySize
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat Format
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISampleDescription SampleDescription
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ResourceUsage Usage
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BindFlags BindFlags
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11CpuAccessFlags CpuAccessFlags
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ResourceOptionFlags OptionFlags
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Texture3DDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public int32 Width
	.field public int32 Height
	.field public int32 Depth
	.field public int32 MipLevels
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat Format
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ResourceUsage Usage
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BindFlags BindFlags
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11CpuAccessFlags CpuAccessFlags
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ResourceOptionFlags OptionFlags
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISampleDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public int32 Count
	.field public int32 Quality
}

.class private abstract sealed auto ansi beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid
	extends [System.Runtime]System.Object
{
	.field public static initonly valuetype [System.Runtime]System.Guid DebugObjectName

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		.maxstack 8

		ldstr "429b8c22-9188-4b0c-8742-acb0bf85c200"
		newobj instance void [System.Runtime]System.Guid::.ctor(string)
		stsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		ret
	}
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapterDescription
	extends [System.Runtime]System.ValueType
{
	.field public uint64 Description0
	.field public uint64 Description1
	.field public uint64 Description2
	.field public uint64 Description3
	.field public uint64 Description4
	.field public uint64 Description5
	.field public uint64 Description6
	.field public uint64 Description7
	.field public uint64 Description8
	.field public uint64 Description9
	.field public uint64 Description10
	.field public uint64 Description11
	.field public uint64 Description12
	.field public uint64 Description13
	.field public uint64 Description14
	.field public uint64 Description15
	.field public uint64 Description16
	.field public uint64 Description17
	.field public uint64 Description18
	.field public uint64 Description19
	.field public uint64 Description20
	.field public uint64 Description21
	.field public uint64 Description22
	.field public uint64 Description23
	.field public uint64 Description24
	.field public uint64 Description25
	.field public uint64 Description26
	.field public uint64 Description27
	.field public uint64 Description28
	.field public uint64 Description29
	.field public uint64 Description30
	.field public uint64 Description31
	.field public int32 VendorId
	.field public int32 DeviceId
	.field public int32 SubsystemId
	.field public int32 Revision
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize DedicatedVideoMemory
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize DedicatedSystemMemory
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize SharedSystemMemory
	.field public int64 Luid
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result
	extends [System.Runtime]System.ValueType
{
	.size 4
	.field private int32 _result

	.property instance bool Success()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result::get_Success()
	}

	.property instance bool IsOk()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result::get_IsOk()
	}

	.property instance bool IsFalse()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result::get_IsFalse()
	}

	.method public hidebysig specialname instance bool get_Success () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld int32 Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result::_result
		ldc.i4.0
		clt
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig specialname instance bool get_IsOk () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld int32 Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result::_result
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig specialname instance bool get_IsFalse () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld int32 Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result::_result
		ldc.i4.1
		ceq
		ret
	}
}

.class private explicit ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	// Nested Types
	.class nested public sequential ansi sealed beforefieldinit Texture2DResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MipSlice
	}

	.class nested public sequential ansi sealed beforefieldinit Texture1DArrayResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MipSlice
		.field public int32 FirstArraySlice
		.field public int32 ArraySize
	}

	.class nested public sequential ansi sealed beforefieldinit Texture2DArrayResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MipSlice
		.field public int32 FirstArraySlice
		.field public int32 ArraySize
	} 

	.class nested public sequential ansi sealed beforefieldinit Texture1DResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 MipSlice
	}

	.class nested public sequential ansi sealed beforefieldinit Texture2DMultisampledResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field private int32 UnusedFieldNothingToDefine
	}

	.class nested public sequential ansi sealed beforefieldinit Texture2DMultisampledArrayResource
		extends [System.Runtime]System.ValueType
	{
		.pack 0
		.size 0

		.field public int32 FirstArraySlice
		.field public int32 ArraySize
	} 

	.field [0] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat Format
	.field [4] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewDimension Dimension
	.field [8] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewFlags Flags
	.field [12] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewDescription/Texture1DResource Texture1D
	.field [12] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewDescription/Texture1DArrayResource Texture1DArray
	.field [12] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewDescription/Texture2DResource Texture2D
	.field [12] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewDescription/Texture2DArrayResource Texture2DArray
	.field [12] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewDescription/Texture2DMultisampledResource Texture2DMS
	.field [12] public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewDescription/Texture2DMultisampledArrayResource Texture2DMSArray
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChainDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIModeDescription ModeDescription
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISampleDescription SampleDescription
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIUsage Usage
	.field public int32 BufferCount
	.field public native int OutputHandle
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Bool IsWindowed
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapEffect SwapEffect
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChainFlags Flags
} 

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIModeDescription
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public int32 Width
	.field public int32 Height
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIRational RefreshRate
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat Format
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDisplayModeScanlineOrder ScanlineOrdering
	.field public valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDisplayModeScaling Scaling
}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIRational
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public int32 Numerator
	.field public int32 Denominator
}