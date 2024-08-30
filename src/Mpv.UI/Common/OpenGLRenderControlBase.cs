using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;

namespace MpvWinUI.Common;

public abstract class OpenGLRenderControlBase<TFrame> : ContentControl where TFrame : FrameBufferBase
{
    protected Stopwatch _stopwatch = Stopwatch.StartNew();
    protected TimeSpan _lastRenderTime = TimeSpan.FromSeconds(-1);
    protected TimeSpan _lastFrameStamp;

    protected TFrame FrameBuffer { get; set; }

    public virtual void Initialize()
    {
        CompositionTarget.Rendering += OnRendering;
    }

    public virtual void Release()
    {
        CompositionTarget.Rendering -= OnRendering;
    }

    protected abstract void Draw();

    private void InvalidateVisual()
    {
        if (FrameBuffer != null)
        {
            Draw();
            _stopwatch.Restart();
        }
    }

    private void OnRendering(object sender, object e)
    {
        var args = (RenderingEventArgs)e;

        if (_lastRenderTime != args.RenderingTime)
        {
            InvalidateVisual();
            _lastRenderTime = args.RenderingTime;
        }
    }
}
