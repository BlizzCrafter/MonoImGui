using System.IO;
using System.Runtime.InteropServices;
using MonoImGui;
using MonoImGui.Data;
using Serilog;

// Ensure DPI-Awareness isn't lost in case you create a dotnet tool.
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    [DllImport("user32.dll")]
    static extern bool SetProcessDPIAware();

    SetProcessDPIAware();
}

// Create the logs directory.
Directory.CreateDirectory(AppSettings.LogsPath);

// The general log file should always regenerate.
if (File.Exists(AppSettings.AllLogPath)) File.Delete(AppSettings.AllLogPath);

// Create the serilog logger.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.File(AppSettings.AllLogPath,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
        rollOnFileSizeLimit: true)
    .WriteTo.File(AppSettings.ImportantLogPath,
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
        rollOnFileSizeLimit: true)
    .WriteTo.Debug(
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
    .WriteTo.Console(
    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
    .WriteTo.MonoSink(
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
    .CreateLogger();

// Log that main initialize begins.
MonoLog.LogInfoHeadline(FontAwesome.Flag, "INITIALIZE");

// Main initialize.
using var game = new Main();
game.Run();