using System.IO;
using System.Windows.Media;

namespace MiniPlayerWpf;

// ai helped convert my previous project
public class MusicPlayer : IDisposable
{
    private readonly MediaPlayer mediaPlayer = new();
    private bool isPlaying;
    private bool mediaLoaded;
    private string? loadError;

    /// <summary>Raised whenever anything that affects CanPlay/CanStop changes.</summary>
    public event EventHandler? StateChanged;

    public MusicPlayer()
    {
        mediaPlayer.MediaOpened += (s, e) =>
        {
            mediaLoaded = true;
            OnStateChanged();
        };

        mediaPlayer.MediaEnded += (s, e) =>
        {
            mediaPlayer.Stop();
            isPlaying = false;
            OnStateChanged();
        };

        mediaPlayer.MediaFailed += (s, e) =>
        {
            mediaLoaded = false;
            isPlaying = false;
            loadError = e.ErrorException?.Message ?? "Could not play file";
            OnStateChanged();
        };
    }

    public bool IsPlaying => isPlaying;
    public bool CanPlay => mediaLoaded && !isPlaying;
    public bool CanStop => isPlaying;

    public string? PlayDisabledReason =>
        CanPlay ? null :
        isPlaying ? "Song is already playing" :
        loadError ?? "No song loaded";

    public string? StopDisabledReason => CanStop ? null : "No song is playing";

    public bool Open(string? path)
    {
        mediaPlayer.Stop();
        mediaPlayer.Close();
        isPlaying = false;
        mediaLoaded = false;
        loadError = null;

        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            loadError = "Song file not found";
            OnStateChanged();
            return false;
        }

        mediaPlayer.Open(new Uri(Path.GetFullPath(path)));
        OnStateChanged();
        return true;
    }

    public void Close()
    {
        mediaPlayer.Close();
        isPlaying = false;
        mediaLoaded = false;
    }

    public void Play()
    {
        if (!CanPlay) return;
        mediaPlayer.Play();
        isPlaying = true;
        OnStateChanged();
    }

    public void Stop()
    {
        mediaPlayer.Stop();
        isPlaying = false;
        OnStateChanged();
    }

    public void Dispose() => mediaPlayer.Close();

    private void OnStateChanged() => StateChanged?.Invoke(this, EventArgs.Empty);
}