#include "Prelude.hpp"

using namespace OpenGL3;

VertexLayout* GraphicsDevice::InitializeVertexLayout(VertexLayoutDescription* desc)
{
	auto vertexLayout = PG_NEW(VertexLayout);
	vertexLayout->Obj = Allocate(&GraphicsDevice::glGenVertexArrays, "Vertex Input Layout");

	glBindVertexArray(vertexLayout->Obj);

	if (desc->IndexBuffer != nullptr)
	{
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, static_cast<Buffer*>(desc->IndexBuffer)->Obj);

		vertexLayout->IndexOffset = safe_static_cast<uint32>(desc->IndexOffset);
		vertexLayout->IndexType = Map(desc->IndexSize);
		vertexLayout->IndexSize = safe_static_cast<uint32>(GetIndexSizeInBytes(desc->IndexSize));
	}

	for (auto i = 0; i < desc->BindingsCount; ++i)
	{
		auto type = GetVertexDataType(desc->Bindings[i].Format);
		auto size = GetVertexDataComponentCount(desc->Bindings[i].Format);
		auto slot = static_cast<uint32>(desc->Bindings[i].Semantics);

		PG_ASSERT_NOT_NULL(desc->Bindings[i].VertexBuffer);

		glBindBuffer(GL_ARRAY_BUFFER, static_cast<Buffer*>(desc->Bindings[i].VertexBuffer)->Obj);
		glEnableVertexAttribArray(slot);

		// Color values must be normalized
		auto normalize = desc->Bindings[i].Semantics == DataSemantics::Color0 ||
			desc->Bindings[i].Semantics == DataSemantics::Color1 ||
			desc->Bindings[i].Semantics == DataSemantics::Color2 ||
			desc->Bindings[i].Semantics == DataSemantics::Color3;

		glVertexAttribPointer(slot, size, type, static_cast<int32>(normalize), desc->Bindings[i].Stride, ToPointer(desc->Bindings[i].Offset));
		glVertexAttribDivisor(slot, desc->Bindings[i].StepRate);
	}

	glBindVertexArray(0);
	return vertexLayout;
}

void GraphicsDevice::FreeVertexLayout(VertexLayout* vertexLayout)
{
	if (vertexLayout == nullptr)
		return;

	glDeleteVertexArrays(1, &vertexLayout->Obj);
	PG_DELETE(vertexLayout);
}

void GraphicsDevice::BindVertexLayout(VertexLayout* vertexLayout)
{
	// Do not actually bind the input layout here, as that causes all sorts of problems with buffer updates between
	// the binding of the input layout and the actual draw call using the input layout
	_boundVertexLayout = vertexLayout;
}