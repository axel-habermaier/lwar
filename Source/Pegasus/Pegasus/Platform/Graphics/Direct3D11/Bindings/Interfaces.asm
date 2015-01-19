.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object

		IL_ret: ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result QueryInterface (void* id, void* obj) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 0
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, void*, void*)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11FeatureLevel GetFeatureLevel () cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 37
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11FeatureLevel(native int)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateBlendState (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendStateDescription& desc, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState& state) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 20
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendStateDescription&, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateDepthStencilState (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilStateDescription& desc, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState& state) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 21
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilStateDescription&, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateRasterizerState (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerStateDescription& desc, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState& state) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 22
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerStateDescription&, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateSamplerState (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerStateDescription& desc, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState& state) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 23
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerStateDescription&, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateQuery (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11QueryDescription& desc, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query& query) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 24
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11QueryDescription&, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateBuffer (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BufferDescription& desc, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData* data, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer& buffer) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 3
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BufferDescription&, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData*, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateInputLayout (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputElement* inputs, int32 numInputs, void* signatureShader, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize shaderSize, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout& inputLayout) cil managed aggressiveinlining
	{
		.maxstack 9

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg 4
		ldarg 5
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 11
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputElement*, int32, void*, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreatePixelShader (void* byteCode, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize byteCodeSize, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassLinkage linkage, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader& shader) cil managed aggressiveinlining
	{
		.maxstack 8

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 15
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, void*, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassLinkage, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateVertexShader (void* byteCode, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize byteCodeSize, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassLinkage linkage, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader& shader) cil managed aggressiveinlining
	{
		.maxstack 8

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 12
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, void*, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PointerSize, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassLinkage, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateRenderTargetView (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource resource, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription* desc, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView& view) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 9
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetViewDescription*, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateDepthStencilView (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource resource, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewDescription* desc, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView& view) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 10
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilViewDescription*, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateShaderResourceView (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource resource, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription& desc, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView& view) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 7
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceViewDescription&, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateTexture1D (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Texture1DDescription& desc, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData* data, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource& texture) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 4
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Texture1DDescription&, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData*, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateTexture2D (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Texture2DDescription& desc, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData* data, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource& texture) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Texture2DDescription&, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData*, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource&)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateTexture3D (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Texture3DDescription& desc, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData* data, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource& texture) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device::Object
		ldind.i
		ldc.i4.s 6
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Texture3DDescription&, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData*, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource&)
		ret
	}

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::Object

		IL_ret: ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result EnumAdapters (int32 index, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapter* adapter) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::Object
		ldind.i
		ldc.i4.s 7
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapter*)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result CreateSwapChain (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device device, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChainDescription* desc, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain* swapChain) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::Object
		ldind.i
		ldc.i4.s 10
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Device, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChainDescription*, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain*)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result MakeWindowAssociation (native int hwnd, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIWindowAssociationFlags windowFlags) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFactory::Object
		ldind.i
		ldc.i4.s 8
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIWindowAssociationFlags)
		ret
	}

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapter
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapter::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapter::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapter::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapter::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapter::Object

		IL_ret: ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result GetDesc (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapterDescription* desc) cil managed aggressiveinlining
	{
		.maxstack 5

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapter::Object
		ldarg 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapter::Object
		ldind.i
		ldc.i4.s 8
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIAdapterDescription*)
		ret
	}

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::Object

		IL_ret: ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result Present (int32 syncInterval, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIPresentFlags presentFlags) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::Object
		ldind.i
		ldc.i4.s 8
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIPresentFlags)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result ResizeBuffers (int32 bufferCount, int32 width, int32 height, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat format, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChainFlags swapChainFlags) cil managed aggressiveinlining
	{
		.maxstack 9

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg 4
		ldarg 5
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::Object
		ldind.i
		ldc.i4.s 13
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, int32, int32, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChainFlags)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result GetBuffer (int32 buffer, void* id, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource& surface) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGISwapChain::Object
		ldind.i
		ldc.i4.s 9
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, int32, void*, [out] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource&)
		ret
	}

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDebug
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDebug::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDebug::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDebug::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDebug::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDebug::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDebug::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDebug::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result ReportLiveObjects (valuetype [System.Runtime]System.Guid apiId, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ReportingLevel level) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDebug::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIDebug::Object
		ldind.i
		ldc.i4.s 3
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype [System.Runtime]System.Guid, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ReportingLevel)
		ret
	}

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassLinkage
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassLinkage::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassLinkage::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassLinkage::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassLinkage::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassLinkage::Object

		IL_ret: ret
	}

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource::Object

		IL_ret: ret
	}

	.method public hidebysig instance void SetDebugName (string name) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] native int,
			[1] valuetype [System.Runtime]System.Guid
		)

		ldarg.1
		call native int [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::StringToHGlobalAnsi(string)
		stloc.0
		ldsfld valuetype [System.Runtime]System.Guid Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Guid::DebugObjectName
		stloc.1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource::Object
		ldloca.s 1
		ldarg.1
		callvirt instance int32 [System.Runtime]System.String::get_Length()
		ldloc.0
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource::Object
		ldind.i
		ldc.i4.s 5
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall int32(native int, native int, int32, native int)
		pop
		ldloc.0
		call void [System.Runtime.InteropServices]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		ret
	} 

	.method public hidebysig specialname static valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource op_Implicit (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer val) cil managed 
	{
		.maxstack 8
		.locals init (
			[0] valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource obj
		)

		ldloca.s 0
		initobj valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource
		ldloca.s 0
		ldarg.0
		ldfld native int valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer::Object
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource::Object
		ldloc.0
		ret
	}

}

.class private sequential ansi sealed beforefieldinit Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 0

	.field public native int Object

	.property instance bool IsInitialized()
	{
		.get instance bool Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::get_IsInitialized()
	}

	.method public hidebysig specialname instance bool get_IsInitialized () cil managed
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldc.i4.0
		conv.i
		ceq
		ldc.i4.0
		ceq
		ret
	}

	.method public hidebysig instance void Release () cil managed 
	{
		.maxstack 4

		ldc.i4.0
		conv.i
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		beq.s IL_ret

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		dup
		ldind.i
		ldc.i4.s 2
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall uint32(native int)
		pop

		ldarg.0
		ldc.i4.0
		conv.i
		stfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object

		IL_ret: ret
	}

	.method public hidebysig instance void Begin (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query query) cil managed aggressiveinlining
	{
		.maxstack 5

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 27
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query)
		ret
	}

	.method public hidebysig instance void End (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query query) cil managed aggressiveinlining
	{
		.maxstack 5

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 28
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result GetData (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query query, void* data, int32 size, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11QueryFlags dataFlags) cil managed aggressiveinlining
	{
		.maxstack 8

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 29
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Query, void*, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11QueryFlags)
		ret
	}

	.method public hidebysig instance void OMSetBlendState (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState state, float32* blendFactor, uint32 sampleMask) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 35
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11BlendState, float32*, uint32)
		ret
	}

	.method public hidebysig instance void OMSetRenderTargets (int32 numViews, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView* renderTargetViews, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView depthStencilView) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 33
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView*, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView)
		ret
	}

	.method public hidebysig instance void OMSetDepthStencilState (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState state, uint32 stencilRef) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 36
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilState, uint32)
		ret
	}

	.method public hidebysig instance void RSSetState (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState state) cil managed aggressiveinlining
	{
		.maxstack 5

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 43
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RasterizerState)
		ret
	}

	.method public hidebysig instance void RSSetViewports (int32 numViewports, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Viewport* viewports) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 44
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Viewport*)
		ret
	}

	.method public hidebysig instance void RSSetScissorRects (int32 numRects, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Rectangle* rects) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 45
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Rectangle*)
		ret
	}

	.method public hidebysig instance void VSSetShader (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader shader, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance* instances, int32 numInstances) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 11
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11VertexShader, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance*, int32)
		ret
	}

	.method public hidebysig instance void VSSetSamplers (int32 startSlot, int32 numSamplers, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState* states) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 26
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState*)
		ret
	}

	.method public hidebysig instance void VSSetConstantBuffers (int32 startSlot, int32 numBuffers, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer* buffers) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 7
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer*)
		ret
	}

	.method public hidebysig instance void VSSetShaderResources (int32 startSlot, int32 numViews, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView* views) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 25
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView*)
		ret
	}

	.method public hidebysig instance void PSSetShader (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader shader, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance* instances, int32 numInstances) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 9
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PixelShader, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ClassInstance*, int32)
		ret
	}

	.method public hidebysig instance void PSSetSamplers (int32 startSlot, int32 numSamplers, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState* states) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 10
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SamplerState*)
		ret
	}

	.method public hidebysig instance void PSSetConstantBuffers (int32 startSlot, int32 numBuffers, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer* buffers) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 16
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer*)
		ret
	}

	.method public hidebysig instance void PSSetShaderResources (int32 startSlot, int32 numViews, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView* views) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 8
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView*)
		ret
	}

	.method public hidebysig instance void IASetInputLayout (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout layout) cil managed aggressiveinlining
	{
		.maxstack 5

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 17
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11InputLayout)
		ret
	}

	.method public hidebysig instance void IASetPrimitiveTopology (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PrimitiveTopology topology) cil managed aggressiveinlining
	{
		.maxstack 5

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 24
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11PrimitiveTopology)
		ret
	}

	.method public hidebysig instance void IASetVertexBuffers (int32 startSlot, int32 numBuffers, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer* buffers, int32* strides, int32* offsets) cil managed aggressiveinlining
	{
		.maxstack 9

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg 4
		ldarg 5
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 18
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer*, int32*, int32*)
		ret
	}

	.method public hidebysig instance void IASetIndexBuffer (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer buffer, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat format, int32 offset) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 19
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Buffer, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.DXGIFormat, int32)
		ret
	}

	.method public hidebysig instance void Unmap (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource resource, int32 subResource) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 15
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource, int32)
		ret
	}

	.method public hidebysig instance valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result Map (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource resource, int32 subResource, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11MapMode mapMode, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11MapFlags mapFlags, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData* mappedResource) cil managed aggressiveinlining
	{
		.maxstack 9

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg 4
		ldarg 5
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 14
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Result(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11Resource, int32, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11MapMode, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11MapFlags, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11SubResourceData*)
		ret
	}

	.method public hidebysig instance void GenerateMips (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView view) cil managed aggressiveinlining
	{
		.maxstack 5

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 54
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11ShaderResourceView)
		ret
	}

	.method public hidebysig instance void ClearRenderTargetView (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView view, float32* color) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 50
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11RenderTargetView, float32*)
		ret
	}

	.method public hidebysig instance void ClearDepthStencilView (valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView view, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilClearFlags clearFlags, float32 depth, uint8 stencil) cil managed aggressiveinlining
	{
		.maxstack 8

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 53
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilView, valuetype Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DepthStencilClearFlags, float32, uint8)
		ret
	}

	.method public hidebysig instance void Draw (int32 vertexCount, int32 vertexOffset) cil managed aggressiveinlining
	{
		.maxstack 6

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 13
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32)
		ret
	}

	.method public hidebysig instance void DrawInstanced (int32 vertexCount, int32 instanceCount, int32 vertexOffset, int32 instanceOffset) cil managed aggressiveinlining
	{
		.maxstack 8

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg 4
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 21
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32, int32, int32)
		ret
	}

	.method public hidebysig instance void DrawIndexed (int32 indexCount, int32 indexOffset, int32 vertexOffset) cil managed aggressiveinlining
	{
		.maxstack 7

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 12
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32, int32)
		ret
	}

	.method public hidebysig instance void DrawIndexedInstanced (int32 indexCount, int32 instanceCount, int32 indexOffset, int32 vertexOffset, int32 instanceOffset) cil managed aggressiveinlining
	{
		.maxstack 9

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg 1
		ldarg 2
		ldarg 3
		ldarg 4
		ldarg 5
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 20
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int, int32, int32, int32, int32, int32)
		ret
	}

	.method public hidebysig instance void ClearState () cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 110
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int)
		ret
	}

	.method public hidebysig instance void Flush () cil managed aggressiveinlining
	{
		.maxstack 4

		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldarg.0
		ldfld native int Pegasus.Platform.Graphics.Direct3D11.Bindings.D3D11DeviceContext::Object
		ldind.i
		ldc.i4.s 111
		conv.i
		sizeof void*
		mul
		add
		ldind.i
		calli unmanaged stdcall void(native int)
		ret
	}

}

