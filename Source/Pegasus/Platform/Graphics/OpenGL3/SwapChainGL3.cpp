#include "Prelude.hpp"

using namespace OpenGL3;

SwapChain* GraphicsDevice::InitializeSwapChain(NativeWindow* window)
{
	if (SDL_GL_MakeCurrent(window->GetSDLWindow(), _context) != 0)
		PG_DIE("Failed to make OpenGL context current: %s.", SDL_GetError());

	auto swapChain = PG_NEW(SwapChain);
	swapChain->Window = window->GetSDLWindow();
	swapChain->BackBuffer.SwapChain = swapChain;

	return swapChain;
}

void GraphicsDevice::FreeSwapChain(SwapChain* swapChain)
{
	if (swapChain == nullptr)
		return;

	if (SDL_GL_MakeCurrent(_contextWindow, _context) != 0)
		PG_DIE("Failed to make OpenGL context current: %s.", SDL_GetError());

	PG_DELETE(swapChain);
}

void GraphicsDevice::PresentSwapChain(SwapChain* swapChain)
{
	if (SDL_GL_MakeCurrent(swapChain->Window, _context) != 0)
		PG_DIE("Failed to make OpenGL context current: %s.", SDL_GetError());

	SDL_GL_SwapWindow(swapChain->Window);
}

void GraphicsDevice::ResizeSwapChain(SwapChain* swapChain, int32 width, int32 height)
{
	swapChain->BackBuffer.Width = width;
	swapChain->BackBuffer.Height = height;
}

RenderTarget* GraphicsDevice::GetBackBuffer(SwapChain* swapChain)
{
	return &swapChain->BackBuffer;
}