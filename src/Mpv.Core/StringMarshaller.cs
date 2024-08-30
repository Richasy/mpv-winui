using System.Runtime.InteropServices.Marshalling;
using System.Runtime.InteropServices;

namespace Mpv.Core;

[CustomMarshaller(typeof(string), MarshalMode.ManagedToUnmanagedOut, typeof(StringMarshaller))]
internal static class StringMarshaller
{
    public static string ConvertToManaged(IntPtr unmanaged)
        => Marshal.PtrToStringUTF8(unmanaged) ?? string.Empty;
}
