using System.Text.Json;

namespace HaiminhGSM.VnVoLTETool;

public sealed class RebootGuard
{
    public bool Enabled { get; set; }
    public string Carrier { get; set; } = "";
    public DateTimeOffset SavedAt { get; set; }
    public List<string> Checks { get; set; } = new();
}

public static class GuardStore
{
    public static string PathName => Path.Combine(AppContext.BaseDirectory, "reboot_guard.json");
    public static void Save(RebootGuard guard) => File.WriteAllText(PathName, JsonSerializer.Serialize(guard, new JsonSerializerOptions { WriteIndented = true }));
    public static RebootGuard Load() => File.Exists(PathName) ? JsonSerializer.Deserialize<RebootGuard>(File.ReadAllText(PathName)) ?? new() : new();
}
