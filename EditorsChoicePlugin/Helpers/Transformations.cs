using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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

        string script = $"<script FileTransformation=\"true\" plugin=\"EditorsChoice\" defer=\"defer\" src=\"{basePath}/EditorsChoice/script\"></script>";

        string text = Regex.Replace(payload.Contents!, "(</body>)", $"{script}$1");

        return text;
    }
}

public class PatchRequestPayload
{
    [JsonPropertyName("contents")]
    public string? Contents { get; set; }
}