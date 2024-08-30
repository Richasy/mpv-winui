# MPV WinUI

This is a test project primarily aimed at integrating libmpv into WinUI3.

After some modifications, libmpv uses OpenGL to render content, while XAML only supports DirectX content. Therefore, the core task is to convert the OpenGL content into DirectX content required by XAML through a swapchain.

Thanks to the insights provided by this article: [[WinUI 3] 如何利用 D3D11 在 SwapChainPanel 控件上绘制 OpenGL（UWP通用)](https://www.cnblogs.com/xymfblogs/p/17218256.html)

Currently, this player core is already used in the [Richasy/Bili.Copilot](https://github.com/Richasy/Bili.Copilot) project.