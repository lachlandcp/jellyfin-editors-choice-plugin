using System.Text.Json.Serialization;
using MediaBrowser.Common.Net;

namespace EditorsChoicePlugin.Helpers;

public static class Transformations
{
    public static string IndexTransformation(PatchRequestPayload payload)
    {
        NetworkConfiguration networkConfiguration = Plugin.Instance!.ServerConfigurationManager.GetNetworkConfiguration();

        string basePath = "";
        if (!string.IsNullOrWhiteSpace(networkConfiguration.BaseUrl))
        {
            basePath = $"/{networkConfiguration.BaseUrl.TrimStart('/').Trim()}";
        }

        string contents = ScriptMarkup.RemoveExisting(payload.Contents ?? string.Empty);
        string script = ScriptMarkup.Build(basePath, "FileTransformation=\"true\"");
        int bodyClosing = contents.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);

        return bodyClosing < 0 ? contents : contents.Insert(bodyClosing, script);
    }
}

public class PatchRequestPayload
{
    [JsonPropertyName("contents")]
    public string? Contents { get; set; }
}
