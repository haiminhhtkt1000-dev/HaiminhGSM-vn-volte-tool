using System.Text;

namespace HaiminhGSM.VnVoLTETool;

public sealed class MainForm : Form
{
    readonly ListBox devices = new() { Dock = DockStyle.Fill };
    readonly TextBox report = new() { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true };
    readonly Label status = new() { Dock = DockStyle.Bottom, AutoSize = true };
    string serial = "";

    public MainForm()
    {
        Text = "HaiminhGSM VN VoLTE Toolkit"; Width = 1100; Height = 700; StartPosition = FormStartPosition.CenterScreen;
        var refresh = new Button { Text = "Refresh ADB", Dock = DockStyle.Top };
        var diagnose = new Button { Text = "Diagnose VoLTE", Dock = DockStyle.Top };
        var safe = new Button { Text = "Generate Safe Fix", Dock = DockStyle.Top };
        var guard = new Button { Text = "Enable Reboot Guard", Dock = DockStyle.Top };
        var export = new Button { Text = "Export Report", Dock = DockStyle.Top };
        refresh.Click += (_, _) => RefreshDevices(); diagnose.Click += (_, _) => Diagnose(); safe.Click += (_, _) => SafeFix(); guard.Click += (_, _) => EnableGuard(); export.Click += (_, _) => Export();
        devices.SelectedIndexChanged += (_, _) => { if (devices.SelectedItem is string s) serial = s; };
        var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) }; left.Controls.Add(devices); left.Controls.Add(export); left.Controls.Add(guard); left.Controls.Add(safe); left.Controls.Add(diagnose); left.Controls.Add(refresh); left.Controls.Add(status);
        var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 330 }; split.Panel1.Controls.Add(left); split.Panel2.Padding = new Padding(10); split.Panel2.Controls.Add(report); Controls.Add(split);
        Shown += (_, _) => RefreshDevices();
    }

    void RefreshDevices()
    {
        devices.Items.Clear(); var list = AdbService.Devices();
        foreach (var d in list) devices.Items.Add(d);
        if (list.Count > 0) { serial = list[0]; devices.SelectedIndex = 0; status.Text = $"Đã tìm thấy {list.Count} thiết bị"; }
        else { devices.Items.Add("Không tìm thấy ADB device"); status.Text = "Bật USB debugging và cài Android Platform Tools"; }
    }

    DeviceInfo? Current()
    {
        if (string.IsNullOrWhiteSpace(serial) || serial.StartsWith("Không")) { report.Text = "Hãy kết nối và chọn điện thoại qua ADB."; return null; }
        return AdbService.Inspect(serial);
    }

    void Diagnose()
    {
        var d = Current(); if (d is null) return; var p = Profiles.ForBrand(d.Brand); var c = Profiles.Carrier(d);
        report.Text = $"HaiminhGSM VN VoLTE report\r\n\r\nSerial: {d.Serial}\r\nBrand: {d.Brand}\r\nModel: {d.Model}\r\nAndroid: {d.Android}\r\nMCC/MNC: {d.OperatorNumeric}\r\nOperator: {d.OperatorName}\r\nRegion: {d.Region}\r\nCarrier: {c}\r\n\r\nProfile: {p.Name}\r\nFocus: {p.Focus}\r\nAdvice: {p.Advice}\r\n\r\nIMS checks:\r\n- Kiểm tra VoLTE trong Settings > Mobile network\r\n- Kiểm tra SIM data và tín hiệu LTE/5G\r\n- Kiểm tra IMS registration bằng logcat/diagnostic của thiết bị\r\n- Không ghi EFS/NV/modem hoặc IMEI tự động";
    }

    void SafeFix()
    {
        var d = Current(); if (d is null) return; var c = Profiles.Carrier(d);
        var path = Path.Combine(AppContext.BaseDirectory, "vn_volte_safe_fix.txt");
        File.WriteAllText(path, $"VN VoLTE Safe Fix\r\nCarrier: {c}\r\n1. Kiểm tra SIM và data.\r\n2. Bật VoLTE trong Settings.\r\n3. Kiểm tra region/CSC/ROM.\r\n4. Kiểm tra IMS sau reboot.\r\n5. Không flash sai model và không ghi EFS/NV.");
        report.Text = $"Đã tạo hướng dẫn an toàn:\r\n{path}\r\n\r\nKhông tự động sửa modem vì có thể mất IMEI, mất sóng hoặc brick máy.";
    }

    void EnableGuard()
    {
        var d = Current(); if (d is null) return; var g = new RebootGuard { Enabled = true, Carrier = Profiles.Carrier(d), SavedAt = DateTimeOffset.Now, Checks = new() { "Kiểm tra SIM", "Bật VoLTE", "Kiểm tra IMS", "Kiểm tra carrier/CSC sau reboot" } }; GuardStore.Save(g); report.Text = $"Đã bật Reboot Guard.\r\nCấu hình: {GuardStore.PathName}\r\n\r\nTính năng này lưu checklist và trạng thái kiểm tra; không thể ép firmware/carrier policy trái phép luôn giữ VoLTE.";
    }

    void Export() { var path = Path.Combine(AppContext.BaseDirectory, "vn_volte_report.txt"); File.WriteAllText(path, report.Text); status.Text = "Đã xuất: " + path; }
}
