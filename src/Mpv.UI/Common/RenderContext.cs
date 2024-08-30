﻿using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using System.Threading;
using System;
using OpenTK.Windowing.Common;
using System.Reflection;
using OpenTK.Graphics.Wgl;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MpvWinUI.Common;

public unsafe class RenderContext
{
    private static IGraphicsContext _sharedContext;
    private static ContextSettings _sharedContextSettings;
    private static int _sharedContextReferenceCount;
    private static OpenTK.IBindingsContext _sharedBindingContext;

    public Format Format { get; }

    public IntPtr DxDeviceFactory { get; }

    public IntPtr DxDeviceHandle { get; }

    public IntPtr DxDeviceContext { get; }

    public IntPtr GlDeviceHandle { get; }

    public IGraphicsContext GraphicsContext { get; }

    public RenderContext(ContextSettings settings)
    {
        IDXGIFactory2* factory;
        ID3D11Device* device;
        ID3D11DeviceContext* devCtx;

        // Factory
        {
            Guid guid = typeof(IDXGIFactory2).GetTypeInfo().GUID;
            DXGI.GetApi(null).CreateDXGIFactory2(0, &guid, (void**)&factory);
        }

        // Device
        {
            var flags = CreateDeviceFlag.BgraSupport | CreateDeviceFlag.VideoSupport;
            D3D11.GetApi(null).CreateDevice(null, D3DDriverType.Hardware, 0, Convert.ToUInt32(flags), null, 0, D3D11.SdkVersion, &device, null, &devCtx);
        }

        DxDeviceFactory = (IntPtr)factory;
        DxDeviceHandle = (IntPtr)device;
        DxDeviceContext = (IntPtr)devCtx;

        GraphicsContext = GetOrCreateSharedOpenGLContext(settings);
        GlDeviceHandle = Wgl.DXOpenDeviceNV((IntPtr)device);
    }

    public static IntPtr GetProcAddress(string name)
    {
        if(_sharedBindingContext == null)
        {
            return IntPtr.Zero;
        }

        return _sharedBindingContext.GetProcAddress(name);
    }

    private static IGraphicsContext GetOrCreateSharedOpenGLContext(ContextSettings settings)
    {
        if (_sharedContext == null)
        {
            NativeWindowSettings windowSettings = NativeWindowSettings.Default;
            windowSettings.StartFocused = false;
            windowSettings.StartVisible = false;
            windowSettings.NumberOfSamples = 0;
            windowSettings.APIVersion = new Version(settings.MajorVersion, settings.MinorVersion);
            windowSettings.Flags = ContextFlags.Offscreen | settings.GraphicsContextFlags;
            windowSettings.Profile = settings.GraphicsProfile;
            windowSettings.WindowBorder = WindowBorder.Hidden;
            windowSettings.WindowState = WindowState.Minimized;
            NativeWindow nativeWindow = new(windowSettings);

            _sharedBindingContext = new GLFWBindingsContext();
            Wgl.LoadBindings(_sharedBindingContext);

            _sharedContext = nativeWindow.Context;
            _sharedContextSettings = settings;

            _sharedContext.MakeCurrent();
        }
        else
        {
            if (!ContextSettings.WouldResultInSameContext(settings, _sharedContextSettings))
            {
                throw new ArgumentException($"The provided {nameof(ContextSettings)} would result" +
                                                $"in a different context creation to one previously created. To fix this," +
                                                $" either ensure all of your context settings are identical, or provide an " +
                                                $"external context via the '{nameof(ContextSettings.ContextToUse)}' field.");
            }
        }

        Interlocked.Increment(ref _sharedContextReferenceCount);

        return _sharedContext;
    }
}