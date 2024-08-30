using System.Reflection;
using System.Runtime.InteropServices;

namespace Mpv.Core;

/// <summary>
/// Libraries import resolver
/// </summary>
public static class Resolver
{
    /// <summary>
    /// Dictionary containing pointers to native libraries
    /// </summary>
    private static readonly Dictionary<string, nint> _libraries = [];
    private static string _customMpvPath = string.Empty;

    /// <summary>
    /// Set import resolver if needed
    /// </summary>
    public static void SetResolver()
    {
        if (_libraries.Count == 0)
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), LibraryImportResolver);
        }
    }

    /// <summary>
    /// Set custom path for libmpv-2.dll
    /// </summary>
    /// <param name="path">The custom path to libmpv-2.dll</param>
    public static void SetCustomMpvPath(string path)
    {
        _customMpvPath = path;
    }

    /// <summary>
    /// Resolve native libraries
    /// </summary>
    /// <param name="libraryName">The string representing a library</param>
    /// <param name="assembly">The assembly loading a native library</param>
    /// <param name="searchPath">The search path</param>
    private static nint LibraryImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (_libraries.TryGetValue(libraryName, out var lib))
        {
            return lib;
        }

        var filename = string.Empty;
        if (libraryName == "mpv" && !string.IsNullOrEmpty(_customMpvPath))
        {
            filename = _customMpvPath;
        }
        else
        {
            filename = libraryName switch
            {
                "mpv" => "libmpv-2.dll",
                "gl" => "opengl32.dll",
                _ => libraryName
            };
        }

        _libraries[libraryName] = NativeLibrary.Load(filename, assembly, searchPath);
        return _libraries[libraryName];
    }
}
