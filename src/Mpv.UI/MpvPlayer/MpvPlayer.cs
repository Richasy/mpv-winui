using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mpv.Core;
using Mpv.Core.Args;
using Mpv.Core.Enums.Client;
using Mpv.Core.Enums.Player;
using MpvWinUI.Common;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;

namespace MpvWinUI;

public sealed partial class MpvPlayer : Control
{
    public MpvPlayer()
    {
        DefaultStyleKey = typeof(MpvPlayer);
    }

    protected override void OnApplyTemplate()
    {
        _positionBlock = (TextBlock)GetTemplateChild("PositionBlock");
        _renderControl = (RenderControl)GetTemplateChild("RenderControl");
        _playPauseButton = (Button)GetTemplateChild("PlayPauseButton");
        _skipForwardButton = (Button)GetTemplateChild("SkipForwardButton");
        _skipBackwardButton = (Button)GetTemplateChild("SkipBackwardButton");
        _playRateComboBox = (ComboBox)GetTemplateChild("PlayRateComboBox");
        _renderControl.Setting = new ContextSettings()
        {
            MajorVersion = 4,
            MinorVersion = 6,
            GraphicsProfile = OpenTK.Windowing.Common.ContextProfile.Compatability,
        };
        _renderControl.Render += OnRender;
        _playPauseButton.Click += OnPlayPauseButtonClick;
        _skipForwardButton.Click += OnSkipForwardButtonClick;
        _skipBackwardButton.Click += OnSkipBackwardButtonClick;
        _playRateComboBox.SelectionChanged += OnPlayRateSelectionChanged;
    }

    private void OnPlayRateSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = _playRateComboBox.SelectedItem as ComboBoxItem;
        var rate = Convert.ToDouble(item.Content);
        // Player?.SetProperty("speed", rate);
    }

    private void OnSkipBackwardButtonClick(object sender, RoutedEventArgs e)
    {
        if (Player.Position == null)
        {
            return;
        }
        Player?.Seek(Player.Position!.Value.Add(TimeSpan.FromSeconds(-10)));
    }

    private void OnSkipForwardButtonClick(object sender, RoutedEventArgs e)
    {
        if (Player.Position == null)
        {
            return;
        }
        Player?.Seek(Player.Position!.Value.Add(TimeSpan.FromSeconds(30)));
    }

    private void OnPlayPauseButtonClick(object sender, RoutedEventArgs e)
    {
        //Player?.TogglePlayPause();
    }

    private void OnRender(TimeSpan e)
    {
        Render();
    }

    public async Task OpenAsync(StorageFile file)
    {
        Player ??= new Player();

        if (!Player.Client.IsInitialized)
        {
            Player.PlaybackPositionChanged += OnPositionChanged;
            Player.PlaybackStateChanged += OnStateChanged;
            _renderControl.Initialize();
            Player.Client.SetProperty("vo", "libmpv");
            Player.Client.RequestLogMessage(MpvLogLevel.V);
            Player.LogMessageReceived += OnLogMessageReceived;
            var args = new InitializeArgument(default, func: RenderContext.GetProcAddress);
            await Player.InitializeAsync(args);
        }

        // await Player.OpenAsync(file.Path);
    }

    private void OnStateChanged(object sender, PlaybackStateChangedEventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (e.NewState == PlaybackState.Playing)
            {
                VisualStateManager.GoToState(this, "PlayState", false);
            }
            else if (e.NewState == PlaybackState.Paused || e.NewState == PlaybackState.None)
            {
                VisualStateManager.GoToState(this, "PauseState", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "LoadingState", false);
            }
        });
    }

    private void OnPositionChanged(object sender, PlaybackPositionChangedEventArgs e)
    {
        var duration = TimeSpan.FromSeconds(e.Duration);
        var position = TimeSpan.FromSeconds(e.Position);
        DispatcherQueue.TryEnqueue(() =>
        {
            _positionBlock.Text = $"{position:mm\\:ss} / {duration:mm\\:ss}";
        });
    }

    public void Play()
        => Player?.Play();

    public void Pause()
        => Player?.Pause();

    private void OnLogMessageReceived(object sender, LogMessageReceivedEventArgs e)
    {
        Debug.WriteLine($"[{e.Level}]\t{e.Prefix}: {e.Message}");
    }

    private void Render()
    {
        if (Player?.Client?.IsInitialized is not true)
        {
            return;
        }

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Player.RenderGL((int)(ActualWidth * _renderControl.ScaleX), (int)(ActualHeight * _renderControl.ScaleY), _renderControl.GetBufferHandle());
    }
}
