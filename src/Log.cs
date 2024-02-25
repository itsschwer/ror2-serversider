using BepInEx.Logging;

namespace ServerSider
{
    internal static class Log
    {
        private static ManualLogSource _logSource;

        internal static void Init(ManualLogSource baseLogger)
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            Logger.Sources.Remove(baseLogger);
            _logSource = Logger.CreateLogSource(Plugin.GUID);
        }

        internal static void Debug(object data) => _logSource.LogDebug(data);
        internal static void Error(object data) => _logSource.LogError(data);
        internal static void Fatal(object data) => _logSource.LogFatal(data);
        internal static void Info(object data) => _logSource.LogInfo(data);
        internal static void Message(object data) => _logSource.LogMessage(data);
        internal static void Warning(object data) => _logSource.LogWarning(data);
    }
}
