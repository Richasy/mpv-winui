﻿using OpenTK.Windowing.Common;
using System.Diagnostics.CodeAnalysis;

namespace MpvWinUI.Common;

public sealed class ContextSettings
{
    public int MajorVersion { get; set; } = 3;

    public int MinorVersion { get; set; } = 3;

    public ContextFlags GraphicsContextFlags { get; set; } = ContextFlags.Default;

    public ContextProfile GraphicsProfile { get; set; } = ContextProfile.Core;

    public IGraphicsContext ContextToUse { get; set; }

    public static bool WouldResultInSameContext([NotNull] ContextSettings a, [NotNull] ContextSettings b)
    {
        if (a.MajorVersion != b.MajorVersion)
        {
            return false;
        }

        if (a.MinorVersion != b.MinorVersion)
        {
            return false;
        }

        if (a.GraphicsProfile != b.GraphicsProfile)
        {
            return false;
        }

        if (a.GraphicsContextFlags != b.GraphicsContextFlags)
        {
            return false;
        }

        return true;
    }
}
