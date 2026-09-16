using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EditorsChoicePlugin.Helpers;

public static class JavaScriptInjectorRegistrar
{
    private const string AssemblyName = "Jellyfin.Plugin.JavaScriptInjector";
    private const string PluginInterfaceTypeName = "Jellyfin.Plugin.JavaScriptInjector.PluginInterface";
    private const string ScriptId = "70bb2ec1-f19e-46b5-b49a-942e6b96ebae-editors-choice";

    public static bool TryRegister(string basePath, ILogger logger)
    {
        try
        {
            Plugin? plugin = Plugin.Instance;
            if (plugin is null)
            {
                logger.LogWarning("Editors Choice plugin instance was not available for JavaScript Injector registration.");
                return false;
            }

            Assembly? assembly = AssemblyLoadContext.All
                .SelectMany(context => context.Assemblies)
                .FirstOrDefault(candidate =>
                    candidate.FullName?.Contains(AssemblyName, StringComparison.OrdinalIgnoreCase) ?? false);

            if (assembly is null)
            {
                logger.LogDebug("JavaScript Injector plugin was not found.");
                return false;
            }

            Type? pluginInterfaceType = assembly.GetType(PluginInterfaceTypeName);
            MethodInfo? registerMethod = pluginInterfaceType?.GetMethod("RegisterScript");
            if (registerMethod is null)
            {
                logger.LogWarning("JavaScript Injector registration interface was not found.");
                return false;
            }

            JObject payload = new JObject
            {
                { "id", ScriptId },
                { "name", "Editors Choice loader" },
                { "script", BuildLoaderScript(basePath) },
                { "enabled", true },
                { "requiresAuthentication", false },
                { "pluginId", plugin.Id.ToString() },
                { "pluginName", plugin.Name },
                { "pluginVersion", typeof(JavaScriptInjectorRegistrar).Assembly.GetName().Version?.ToString() ?? string.Empty }
            };

            object? result = registerMethod.Invoke(null, new object?[] { payload });
            if (result is not true)
            {
                logger.LogWarning("JavaScript Injector rejected the Editors Choice registration.");
                return false;
            }

            logger.LogInformation("Editors Choice registered with JavaScript Injector.");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to register Editors Choice with JavaScript Injector.");
            return false;
        }
    }

    public static string BuildLoaderScript(string basePath)
    {
        string scriptUrl = JsonConvert.SerializeObject($"{basePath}/EditorsChoice/script");

        return $$"""
            (() => {
                'use strict';

                if (document.querySelector('script[plugin="EditorsChoice"], script[data-plugin="EditorsChoice"]')) {
                    return;
                }

                const script = document.createElement('script');
                script.defer = true;
                script.dataset.plugin = 'EditorsChoice';
                script.src = {{scriptUrl}};
                (document.head || document.documentElement).appendChild(script);
            })();
            """;
    }
}
