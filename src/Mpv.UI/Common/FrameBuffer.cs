﻿using Microsoft.UI.Xaml.Media;
using Silk.NET.DXGI;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Wgl;
using System;
using Silk.NET.Core.Native;
using OpenTK.Platform.Windows;
using Silk.NET.Direct3D11;
using System.Reflection;

namespace MpvWinUI.Common;

public unsafe class FrameBuffer : FrameBufferBase
{
    public RenderContext Context { get; }

    public int GLColorRenderBufferHandle { get; set; }

    public int GLDepthRenderBufferHandle { get; set; }

    public int GLFrameBufferHandle { get; set; }

    public IntPtr DxInteropColorHandle { get; set; }

    public override int BufferWidth { get; protected set; }
    public override int BufferHeight { get; protected set; }
    public override nint SwapChainHandle { get; protected set; }

    public FrameBuffer(
        RenderContext context,
        int frameBufferWidth,
        int frameBufferHeight,
        double compositionScaleX,
        double compositionScaleY)
    {
        Context = context;
        BufferWidth = Convert.ToInt32(frameBufferWidth * compositionScaleX);
        BufferHeight = Convert.ToInt32(frameBufferHeight * compositionScaleY);

        IDXGISwapChain1* swapChain;

        // SwapChain
        {
            SwapChainDesc1 swapChainDesc = new()
            {
                Width = (uint)BufferWidth,
                Height = (uint)BufferHeight,
                Format = Format.FormatB8G8R8A8Unorm,
                Stereo = 0,
                SampleDesc = new SampleDesc()
                {
                    Count = 1,
                    Quality = 0
                },
                BufferUsage = DXGI.UsageRenderTargetOutput,
                BufferCount = 2,
                Scaling = Scaling.Stretch,
                SwapEffect = SwapEffect.FlipDiscard,
                Flags = 0,
                AlphaMode = AlphaMode.Ignore,
            };

            ((IDXGIFactory2*)Context.DxDeviceFactory)->CreateSwapChainForComposition((IUnknown*)Context.DxDeviceHandle, &swapChainDesc, null, &swapChain);

            SwapChainHandle = (IntPtr)swapChain;
        }

        GLFrameBufferHandle = GL.GenFramebuffer();
    }

    public void Begin()
    {
        ID3D11Texture2D* colorbuffer;

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLFrameBufferHandle);

        // Texture2D
        {
            Guid guid = typeof(ID3D11Texture2D).GetTypeInfo().GUID;
            ((IDXGISwapChain1*)SwapChainHandle)->GetBuffer(0, &guid, (void**)&colorbuffer);
        }

        // GL
        {
            GLColorRenderBufferHandle = GL.GenRenderbuffer();
            GLDepthRenderBufferHandle = GL.GenRenderbuffer();

            DxInteropColorHandle = Wgl.DXRegisterObjectNV(Context.GlDeviceHandle, (nint)colorbuffer, (uint)GLColorRenderBufferHandle, (uint)RenderbufferTarget.Renderbuffer, WGL_NV_DX_interop.AccessReadWrite);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, (uint)GLColorRenderBufferHandle);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, GLDepthRenderBufferHandle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, BufferWidth, BufferHeight);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, (uint)GLDepthRenderBufferHandle);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.StencilAttachment, RenderbufferTarget.Renderbuffer, (uint)GLDepthRenderBufferHandle);
        }

        colorbuffer->Release();

        Wgl.DXLockObjectsNV(Context.GlDeviceHandle, 1, new[] { DxInteropColorHandle });

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLFrameBufferHandle);
        GL.Viewport(0, 0, BufferWidth, BufferHeight);
    }

    public void End()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        Wgl.DXUnlockObjectsNV(Context.GlDeviceHandle, 1, new[] { DxInteropColorHandle });

        Wgl.DXUnregisterObjectNV(Context.GlDeviceHandle, DxInteropColorHandle);

        GL.DeleteRenderbuffer(GLColorRenderBufferHandle);
        GL.DeleteRenderbuffer(GLDepthRenderBufferHandle);

        ((IDXGISwapChain1*)SwapChainHandle)->Present(0, 0);
    }

    public void UpdateSize(
        int framebufferWidth,
        int framebufferHeight,
        double compositionScaleX,
        double compositionScaleY)
    {
        BufferWidth = Convert.ToInt32(framebufferWidth * compositionScaleX);
        BufferHeight = Convert.ToInt32(framebufferHeight * compositionScaleY);

        ((IDXGISwapChain1*)SwapChainHandle)->ResizeBuffers(2, (uint)BufferWidth, (uint)BufferHeight, Format.FormatUnknown, 0);
        ((IDXGISwapChain2*)SwapChainHandle)->SetMatrixTransform(new Matrix3X2F { DXGI11 = 1.0f / (float)compositionScaleX, DXGI22 = 1.0f / (float)compositionScaleY });
    }

    public override void Dispose()
    {
        GL.DeleteFramebuffer(GLFrameBufferHandle);

        Wgl.DXUnregisterObjectNV(Context.GlDeviceHandle, DxInteropColorHandle);
        GL.DeleteRenderbuffer(GLColorRenderBufferHandle);
        GL.DeleteRenderbuffer(GLDepthRenderBufferHandle);

        GC.SuppressFinalize(this);
    }
}
