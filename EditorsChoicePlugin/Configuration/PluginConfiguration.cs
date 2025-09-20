using MediaBrowser.Model.Plugins;

namespace EditorsChoicePlugin.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public PluginConfiguration() { }

    public string EditorUserId { get; set; } = "";

    public bool DoScriptInject { get; set; } = true;

    public bool FileTransformation { get; set; } = false;

    public bool ShowRandomMedia { get; set; } = true;

    public string Mode { get; set; } = "";

    public int RandomMediaCount { get; set; } = 5;

    public float MinimumRating { get; set; } = 0.0f;

    public int MinimumCriticRating { get; set; } = 0;

    public int MaximumParentRating { get; set; } = -2;

    public string[] FilteredLibraries { get; set; } = [];

    public string[] SelectedCollections { get; set; } = [];

    public bool EnableAutoplay { get; set; } = true;

    public int AutoplayInterval { get; set; } = 10;

    public string NewTimeLimit { get; set; } = "1month";

    public bool ShowRating { get; set; } = true;

    public bool ShowDescription { get; set; } = true;

    public bool HideOnTvLayout { get; set; } = false;

    public bool ReduceImageSize { get; set; } = false;

    public int BannerHeight { get; set; } = 360;

    public bool ShowPlayed { get; set; } = true;

    public string? Heading { get; set; }
}
