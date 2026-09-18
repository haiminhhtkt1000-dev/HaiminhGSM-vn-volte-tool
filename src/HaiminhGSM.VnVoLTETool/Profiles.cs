namespace HaiminhGSM.VnVoLTETool;

public sealed record Profile(string Name, string Focus, string Advice);

public static class Profiles
{
    public static Profile ForBrand(string brand)
    {
        if (brand.Contains("samsung", StringComparison.OrdinalIgnoreCase)) return new("Samsung", "CSC / region / IMS", "Kiểm tra CSC, region, carrier config và trạng thái IMS.");
        if (brand.Contains("oppo", StringComparison.OrdinalIgnoreCase) || brand.Contains("realme", StringComparison.OrdinalIgnoreCase)) return new("OPPO / realme", "IMS / firmware / carrier config", "Kiểm tra firmware, region, SIM và IMS sau khi flash ROM.");
        if (brand.Contains("xiaomi", StringComparison.OrdinalIgnoreCase) || brand.Contains("redmi", StringComparison.OrdinalIgnoreCase) || brand.Contains("poco", StringComparison.OrdinalIgnoreCase)) return new("Xiaomi / Redmi / POCO", "ROM region / IMS", "Phân biệt Global, EEA, India, China và custom ROM trước khi chỉnh sửa.");
        if (brand.Contains("vivo", StringComparison.OrdinalIgnoreCase)) return new("vivo", "Region / SIM / IMS", "Kiểm tra region, SIM data, VoLTE và carrier provisioning.");
        return new("Khác", "MCC/MNC / IMS", "Kiểm tra model, firmware, SIM và khả năng hỗ trợ VoLTE.");
    }

    public static string Carrier(DeviceInfo d)
    {
        var s = (d.OperatorNumeric + " " + d.OperatorName).ToLowerInvariant();
        if (s.Contains("45201") || s.Contains("45204") || s.Contains("viettel")) return "Viettel";
        if (s.Contains("45202") || s.Contains("45207") || s.Contains("vinaphone")) return "VinaPhone";
        if (s.Contains("45203") || s.Contains("mobifone")) return "MobiFone";
        if (s.Contains("45205") || s.Contains("vietnamobile")) return "Vietnamobile";
        return "Chưa xác định";
    }
}
