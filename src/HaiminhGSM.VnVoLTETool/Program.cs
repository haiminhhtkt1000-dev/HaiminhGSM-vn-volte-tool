using System.Windows.Forms;
namespace HaiminhGSM.VnVoLTETool;
static class Program { [STAThread] static void Main() { ApplicationConfiguration.Initialize(); Application.Run(new MainForm()); } }
