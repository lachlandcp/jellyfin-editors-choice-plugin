using EditorsChoicePlugin.Configuration;
using MediaBrowser.Common.Configuration;
using Microsoft.Extensions.Logging;

namespace EditorsChoicePlugin.Helpers;

public static class FrontendRegistration
{
    public static bool TryRegisterConfigured(
        PluginConfiguration configuration,
        IApplicationPaths applicationPaths,
        string basePath,
        ILogger logger)
    {
        string method = FrontendInjectionMethods.Normalize(configuration.FrontendInjectionMethod);

        switch (method)
        {
            case FrontendInjectionMethods.FileTransformation:
                return FileTransformationRegistrar.TryRegister(logger);

            case FrontendInjectionMethods.JavaScriptInjector:
                return JavaScriptInjectorRegistrar.TryRegister(basePath, logger);

            case FrontendInjectionMethods.Direct:
                return DirectScriptInjector.TryInject(applicationPaths, basePath, logger);

            case FrontendInjectionMethods.Disabled:
                logger.LogInformation("Editors Choice frontend injection is disabled.");
                return true;

            default:
                if (FileTransformationRegistrar.TryRegister(logger))
                {
                    return true;
                }

                if (JavaScriptInjectorRegistrar.TryRegister(basePath, logger))
                {
                    return true;
                }

                logger.LogInformation(
                    "Neither File Transformation nor JavaScript Injector was available. Falling back to direct index.html injection.");
                return DirectScriptInjector.TryInject(applicationPaths, basePath, logger);
        }
    }
}
