using System;
using System.IO;

namespace MonoImGui
{
    public static class AppSettings
    {
        public static readonly string Title = "MonoImGui";
        public static readonly string Author = "BlizzCrafter";
        public static readonly string Description = "This template project makes it easier to start a new MonoGame tool with ImGui.NET, Serilog and FontAwesome integrations.";
        public static readonly string License = "The MIT License (MIT)";
        public static readonly string GitHubRepoURL = "https://github.com/BlizzCrafter/MonoImGui";

        public static readonly string LocalContentPath = Path.Combine(AppContext.BaseDirectory, "Content");
        public static readonly string LogsPath = Path.Combine(AppContext.BaseDirectory, "logs");
        public static readonly string AllLogPath = Path.Combine(LogsPath, "log.txt");
        public static readonly string ImportantLogPath = Path.Combine(LogsPath, "important-log.txt");

        public static readonly bool ImGuiINI = false;
    }
}
