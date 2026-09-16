namespace EditorsChoicePlugin.Configuration;

public static class FrontendInjectionMethods
{
    public const string Automatic = "automatic";

    public const string FileTransformation = "file-transformation";

    public const string JavaScriptInjector = "javascript-injector";

    public const string Direct = "direct";

    public const string Disabled = "disabled";

    public static string Normalize(string? value)
    {
        return value switch
        {
            FileTransformation => FileTransformation,
            JavaScriptInjector => JavaScriptInjector,
            Direct => Direct,
            Disabled => Disabled,
            _ => Automatic
        };
    }
}
