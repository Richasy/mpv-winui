using Microsoft.UI.Xaml.Controls;
using Mpv.Core;
using MpvWinUI.Common;

namespace MpvWinUI;

public sealed partial class MpvPlayer
{
    private RenderControl _renderControl;
    private TextBlock _positionBlock;
    private Button _playPauseButton;
    private Button _skipForwardButton;
    private Button _skipBackwardButton;
    private ComboBox _playRateComboBox;


    public Player Player { get; private set; }
}
