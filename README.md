# HaiminhGSM VN VoLTE Toolkit

Windows Forms .NET 8 toolkit for safe Vietnamese VoLTE diagnostics, focused on OPPO, realme, Samsung, Xiaomi/Redmi/POCO and vivo.

## Build

Requires Windows, .NET 8 SDK and Android Platform Tools (`adb` on PATH).

```powershell
dotnet build
dotnet run --project src/HaiminhGSM.VnVoLTETool/HaiminhGSM.VnVoLTETool.csproj
```

## Included

- ADB device discovery and basic device inspection
- Vietnamese carrier detection: Viettel, VinaPhone, MobiFone, Vietnamobile
- Brand-specific guidance for the requested manufacturers
- Safe-fix checklist generation
- Reboot Guard checklist persistence
- Report export

The tool deliberately does not write IMEI, EFS, NV or modem partitions and does not bypass bootloader/carrier security. VoLTE availability still depends on the device modem, firmware, region/CSC, SIM and operator provisioning.
