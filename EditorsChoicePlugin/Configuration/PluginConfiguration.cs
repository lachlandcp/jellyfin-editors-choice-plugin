using MediaBrowser.Model.Plugins;

namespace EditorsChoicePlugin.Configuration;

public class PluginConfiguration : BasePluginConfiguration {
    public PluginConfiguration(){}

    public string EditorUserId {get; set; } = "";

    public bool DoScriptInject {get; set;} = true;

    public bool ShowRandomMedia {get; set; } = true;

    public int RandomMediaCount {get; set; } = 5;

}