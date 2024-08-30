﻿// Copyright (c) Richasy. All rights reserved.

namespace Mpv.Core.Enums.Render;

/// <summary>
/// Parameters for mpv_render_param (which is used in a few places such as
/// mpv_render_context_create().
/// Also see mpv_render_param for conventions and how to use it.
/// </summary>
public enum MpvRenderParamType
{
    /// <summary>
    /// Not a valid value, but also used to terminate a params array. Its value
    /// is always guaranteed to be 0 (even if the ABI changes in the future).
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// The render API to use. Valid for mpv_render_context_create(). <br/>
    ///
    /// Type: char*<br/>
    ///
    /// Defined APIs:<br/>
    ///
    ///   Mpv_RENDER_API_TYPE_OPENGL:<br/>
    ///      OpenGL desktop 2.1 or later (preferably core profile compatible to
    ///      OpenGL 3.2), or OpenGLES 2.0 or later.
    ///      Providing Mpv_RENDER_PARAM_OPENGL_INIT_PARAMS is required.
    ///      It is expected that an OpenGL context is valid and "current" when
    ///      calling mpv_render_* functions (unless specified otherwise). It
    ///      must be the same context for the same mpv_render_context.
    /// </summary>
    ApiType = 1,

    /// <summary>
    /// Required parameters for initializing the OpenGL renderer. Valid for
    /// mpv_render_context_create().
    /// Type: mpv_opengl_init_params*
    /// </summary>
    OpenGLInitParams = 2,

    /// <summary>
    /// * Describes a GL render target. Valid for mpv_render_context_render().
    /// Type: mpv_opengl_fbo*
    /// </summary>
    Fbo = 3,

    /// <summary>
    /// Control flipped rendering. Valid for mpv_render_context_render().
    /// Type: int*
    /// If the value is set to 0, render normally. Otherwise, render it flipped,
    /// which is needed e.g. when rendering to an OpenGL default framebuffer
    /// (which has a flipped coordinate system).
    /// </summary>
    FlipY = 4,

    /// <summary>
    /// Control surface depth. Valid for mpv_render_context_render().
    /// Type: int*
    /// This implies the depth of the surface passed to the render function in
    /// bits per channel. If omitted or set to 0, the renderer will assume 8.
    /// Typically used to control dithering.
    /// </summary>
    Depth = 5,

    /// <summary>
    /// ICC profile blob. Valid for mpv_render_context_set_parameter().
    /// Type: mpv_byte_array*
    /// Set an ICC profile for use with the "icc-profile-auto" option. (If the
    /// option is not enabled, the ICC data will not be used.)
    /// </summary>
    ICCProfile = 6,

    /// <summary>
    /// Ambient light in lux. Valid for mpv_render_context_set_parameter().
    /// Type: int*
    /// This can be used for automatic gamma correction.
    /// </summary>
    AmbientLight = 7,

    /// <summary>
    /// X11 Display, sometimes used for hwdec. Valid for
    /// mpv_render_context_create(). The Display must stay valid for the lifetime
    /// of the mpv_render_context.
    /// Type: Display*
    /// </summary>
    X11Display = 8,

    /// <summary>
    /// Wayland display, sometimes used for hwdec. Valid for
    /// mpv_render_context_create(). The wl_display must stay valid for the
    /// lifetime of the mpv_render_context.
    /// Type: struct wl_display*
    /// </summary>
    WLDisplay = 9,

    /// <summary>
    /// Better control about rendering and enabling some advanced features. Valid
    /// for mpv_render_context_create(). <br/>
    /// 
    /// This conflates multiple requirements the API user promises to abide if
    /// this option is enabled:<br/>
    ///
    ///  - The API user's render thread, which is calling the mpv_render_*()
    ///    functions, never waits for the core. Otherwise deadlocks can happen.
    ///    See "Threading" section.<br/>
    ///  - The callback set with mpv_render_context_set_update_callback() can now
    ///    be called even if there is no new frame.The API user should call the
    ///    mpv_render_context_update() function, and interpret the return value
    ///    for whether a new frame should be rendered.<br/>
    ///  - Correct functionality is impossible if the update callback is not set,
    ///    or not set soon enough after mpv_render_context_create() (the core can
    ///    block while waiting for you to call mpv_render_context_update(), and
    ///    if the update callback is not correctly set, it will deadlock, or
    ///    block for too long).<br/>
    /// 
    /// In general, setting this option will enable the following features (and
    /// possibly more):<br/>
    /// 
    ///  - "Direct rendering", which means the player decodes directly to a
    ///    texture, which saves a copy per video frame ("vd-lavc-dr" option
    ///    needs to be enabled, and the rendering backend as well as the
    ///    underlying GPU API/driver needs to have support for it).<br/>
    ///  - Rendering screenshots with the GPU API if supported by the backend
    ///    (instead of using a suboptimal software fallback via libswscale).<br/>
    /// 
    /// Warning: do not just add this without reading the "Threading" section
    /// above, and then wondering that deadlocks happen. The
    /// requirements are tricky. But also note that even if advanced
    /// control is disabled, not adhering to the rules will lead to
    /// playback problems. Enabling advanced controls simply makes
    /// violating these rules fatal.<br/>
    /// 
    /// Type: int*: 0 for disable (default), 1 for enable
    /// </summary>
    AdvancedControl = 10,

    /// <summary>
    /// Return information about the next frame to render. Valid for
    /// mpv_render_context_get_info().
    /// 
    /// Type: mpv_render_frame_info*
    /// 
    /// It strictly returns information about the _next_ frame. The implication
    /// is that e.g. mpv_render_context_update()'s return value will have
    /// Mpv_RENDER_UPDATE_FRAME set, and the user is supposed to call
    /// mpv_render_context_render(). If there is no next frame, then the
    /// return value will have is_valid set to 0.
    /// </summary>
    NextFrameInfo = 11,

    /// <summary>
    /// Enable or disable video timing. Valid for mpv_render_context_render().
    /// 
    /// Type: int*: 0 for disable, 1 for enable (default)
    /// 
    /// When video is timed to audio, the player attempts to render video a bit
    /// ahead, and then do a blocking wait until the target display time is
    /// reached. This blocks mpv_render_context_render() for up to the amount
    /// specified with the "video-timing-offset" global option. You can set
    /// this parameter to 0 to disable this kind of waiting. If you do, it's
    /// recommended to use the target time value in mpv_render_frame_info to
    /// wait yourself, or to set the "video-timing-offset" to 0 instead.
    /// 
    /// Disabling this without doing anything in addition will result in A/V sync
    /// being slightly off.
    /// </summary>
    BlockForTargetTime = 12,

    /// <summary>
    /// Use to skip rendering in mpv_render_context_render().
    /// 
    /// Type: int*: 0 for rendering (default), 1 for skipping
    /// 
    /// If this is set, you don't need to pass a target surface to the render
    /// function (and if you do, it's completely ignored). This can still call
    /// into the lower level APIs (i.e. if you use OpenGL, the OpenGL context
    /// must be set).
    /// 
    /// Be aware that the render API will consider this frame as having been
    /// rendered. All other normal rules also apply, for example about whether
    /// you have to call mpv_render_context_report_swap(). It also does timing
    /// in the same way.
    /// </summary>
    SkipRendering = 13,

    /// <summary>
    /// Deprecated. Not supported. Use Mpv_RENDER_PARAM_DRM_DISPLAY_V2 instead.
    /// Type : struct mpv_opengl_drm_params*
    /// </summary>
    DRMDisplay = 14,

    /// <summary>
    /// DRM draw surface size, contains draw surface dimensions.
    /// Valid for mpv_render_context_create().
    /// Type : struct mpv_opengl_drm_draw_surface_size*
    /// </summary>
    DRMDrawSurfaceSize = 15,

    /// <summary>
    /// DRM display, contains drm display handles.
    /// Valid for mpv_render_context_create().
    /// Type : struct mpv_opengl_drm_params_v2*
    /// </summary>
    DRMDisplayV2 = 16,

    /// <summary>
    /// Mpv_RENDER_API_TYPE_SW only: rendering target surface size, mandatory.
    /// Valid for Mpv_RENDER_API_TYPE_SW | mpv_render_context_render().
    /// Type: int[2] (e.g.: int s[2] = { w, h }; param.data = s[0];)
    /// 
    /// The video frame is transformed as with other VOs.Typically, this means
    /// the video gets scaled and black bars are added if the video size or
    /// aspect ratio mismatches with the target size.
    /// </summary>
    SWSize = 17,

    /// <summary>
    /// Mpv_RENDER_API_TYPE_SW only: rendering target surface pixel format,
    /// mandatory.
    /// Valid for Mpv_RENDER_API_TYPE_SW | mpv_render_context_render().
    /// Type: char* (e.g.: char *f = "rgb0"; param.data = f;)
    /// 
    /// Valid values are:
    ///  "rgb0", "bgr0", "0bgr", "0rgb"
    ///      4 bytes per pixel RGB, 1 byte (8 bit) per component, component bytes
    ///      with increasing address from left to right (e.g. "rgb0" has r at
    ///      address 0), the "0" component contains uninitialized garbage (often
    ///      the value 0, but not necessarily; the bad naming is inherited from
    ///      FFmpeg)
    ///      Pixel alignment size: 4 bytes
    ///  "rgb24"
    ///      3 bytes per pixel RGB. This is strongly discouraged because it is
    ///      very slow.
    ///      Pixel alignment size: 1 bytes
    ///  other
    ///      The API may accept other pixel formats, using mpv internal format
    /// names, as long as it's internally marked as RGB, has exactly 1
    /// plane, and is supported as conversion output.It is not a good idea
    ///      to rely on any of these. Their semantics and handling could change.
    /// </summary>
    SWFormat = 18,

    /// <summary>
    /// Mpv_RENDER_API_TYPE_SW only: rendering target surface bytes per line,
    /// mandatory.
    /// Valid for Mpv_RENDER_API_TYPE_SW | mpv_render_context_render().
    /// Type: size_t*
    /// 
    /// This is the number of bytes between a pixel (x, y) and (x, y + 1) on the
    /// target surface. It must be a multiple of the pixel size, and have space
    /// for the surface width as specified by Mpv_RENDER_PARAM_SW_SIZE.
    /// 
    /// Both stride and pointer value should be a multiple of 64 to facilitate
    /// fast SIMD operation. Lower alignment might trigger slower code paths,
    /// and in the worst case, will copy the entire target frame. If mpv is built
    /// with zimg (and zimg is not disabled), the performance impact might be
    /// less.
    /// In either cases, the pointer and stride must be aligned at least to the
    /// pixel alignment size. Otherwise, crashes and undefined behavior is
    /// possible on platforms which do not support unaligned accesses (either
    /// through normal memory access or aligned SIMD memory access instructions).
    /// </summary>
    SWStride = 19,

    /// <summary>
    /// Mpv_RENDER_API_TYPE_SW only: rendering target surface pixel data pointer,
    /// mandatory.
    /// Valid for Mpv_RENDER_API_TYPE_SW | mpv_render_context_render().
    /// Type: void*
    ///
    /// This points to the first pixel at the left/top corner (0, 0). In
    /// particular, each line y starts at (pointer + stride * y). Upon rendering,
    /// all data between pointer and (pointer + stride * h) is overwritten.
    /// Whether the padding between (w, y) and (0, y + 1) is overwritten is left
    /// unspecified (it should not be, but unfortunately some scaler backends
    /// will do it anyway). It is assumed that even the padding after the last
    /// line (starting at bytepos(w, h) until (pointer + stride * h)) is
    /// writable.
    ///
    /// See Mpv_RENDER_PARAM_SW_STRIDE for alignment requirements.
    /// </summary>
    SWPointer = 20,
}
