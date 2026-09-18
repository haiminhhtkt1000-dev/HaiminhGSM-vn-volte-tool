using System.Diagnostics;
using System.Text;

namespace HaiminhGSM.VnVoLTETool;

public sealed class DeviceInfo
{
    public string Serial { get; init; } = "";
    public string Brand { get; init; } = "Unknown";
    public string Model { get; init; } = "Unknown";
    public string Android { get; init; } = "Unknown";
    public string OperatorNumeric { get; init; } = "Unknown";
    public string OperatorName { get; init; } = "Unknown";
    public string Region { get; init; } = "Unknown";
}

public static class AdbService
{
    public static string Run(string args)
    {
        try
        {
            using var p = Process.Start(new ProcessStartInfo("adb", args)
            {
                RedirectStandardOutput = true, RedirectStandardError = true,
                UseShellExecute = false, CreateNoWindow = true
            });
            if (p is null) return "";
            var output = p.StandardOutput.ReadToEnd();
            var error = p.StandardError.ReadToEnd();
            p.WaitForExit();
            return string.IsNullOrWhiteSpace(output) ? error : output;
        }
        catch (Exception ex) { return "ADB_ERROR: " + ex.Message; }
    }

    public static IReadOnlyList<string> Devices() => Run("devices")
        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
        .Skip(1).Select(x => x.Split('\t', ' ')[0])
        .Where(x => x.Length > 0 && !x.StartsWith("ADB_ERROR"))
        .ToList();

    public static string Shell(string serial, string command) => Run($"-s {serial} shell {command}").Trim();

    public static DeviceInfo Inspect(string serial) => new()
    {
        Serial = serial,
        Brand = Value(Shell(serial, "getprop ro.product.brand")),
        Model = Value(Shell(serial, "getprop ro.product.model")),
        Android = Value(Shell(serial, "getprop ro.build.version.release")),
        OperatorNumeric = Value(Shell(serial, "getprop gsm.operator.numeric")),
        OperatorName = Value(Shell(serial, "getprop gsm.operator.alpha")),
        Region = Value(Shell(serial, "getprop ro.product.locale"))
    };

    private static string Value(string value) => string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
}
