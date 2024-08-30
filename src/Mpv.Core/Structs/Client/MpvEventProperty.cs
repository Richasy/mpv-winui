using System.Runtime.InteropServices;
using Mpv.Core.Enums.Client;

namespace Mpv.Core.Structs.Client;

[StructLayout(LayoutKind.Sequential)]
public struct MpvEventProperty
{
    public string Name;

    public MpvFormat Format;
    
    public IntPtr DataPtr; //Expand to all formats
}