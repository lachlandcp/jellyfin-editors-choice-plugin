using MediaBrowser.Model.Plugins;

namespace EditorsChoicePlugin.Configuration;

public class PluginConfiguration : BasePluginConfiguration {
    public PluginConfiguration(){}

    public string EditorUserId {get; set; } = "";

    public bool DoScriptInject {get; set;} = true;

    public bool ShowRandomMedia {get; set; } = true;

    public int RandomMediaCount {get; set; } = 5;

    public float MinimumRating {get; set; } = 0.0f;

    public int MinimumCriticRating {get; set; } = 0;
}