using Microsoft.Win32;
using SharpShell.Attributes;
using SharpShell.SharpDropHandler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mp3CoverDroper.Extension {

    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".mp3")]
    public class Extension : SharpDropHandler {

        private readonly string[] supportedImageExtensions = { ".jpg", ".jpeg", ".png" };

        protected override void DragEnter(DragEventArgs dragEventArgs) {
            var supported = DragItems.All(di => supportedImageExtensions.Contains(Path.GetExtension(di).ToLower()));
            dragEventArgs.Effect = supported ? DragDropEffects.Link : DragDropEffects.None;
        }

        protected override void Drop(DragEventArgs dragEventArgs) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string[] args = { $"\"{SelectedItemPath}\"" };
            foreach (var imagePath in DragItems) {
                args = args.Append($"\"{imagePath}\"").ToArray();
            }

            // get implementation executable file
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AoiHosizora\Mp3CoverDroper");
            if (key == null) {
                MessageBox.Show(new Form { TopMost = true }, @"You have not set Mp3CoverDroper's registry setting, please check the Implementation key from HKEY_CURRENT_USER\SOFTWARE\AoiHosizora\Mp3CoverDroper.",
                   "Mp3CoverDroper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var executablePath = key.GetValue("Implementation") as string;
            executablePath = executablePath.Trim('"');
            if (string.IsNullOrWhiteSpace(executablePath) || !File.Exists(executablePath)) {
                MessageBox.Show(new Form { TopMost = true }, @"Mp3CoverDroper's implementation application file is not found, please check the Implementation key from HKEY_CURRENT_USER\SOFTWARE\AoiHosizora\Mp3CoverDroper.",
                   "Mp3CoverDroper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // call implementation
            var process = new Process();
            var info = new ProcessStartInfo(executablePath, string.Join(" ", args)) {
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.StartInfo = info;
            process.Start();
        }
    }
}
