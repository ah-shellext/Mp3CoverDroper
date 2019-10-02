using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpDropHandler;

using System.Runtime.InteropServices;
using System;

[ComVisible(true)]
[COMServerAssociation(AssociationType.ClassOfExtension, ".mp3")]
public class Mp3CoverDroper : SharpDropHandler {

    private string[] supportImgExt = {".jpg", ".png", ".bmp", ".jpeg"};
    private string app_path = "Mp3CoverDroperApp.exe";

    protected override void DragEnter(DragEventArgs dragEventArgs) {
        dragEventArgs.Effect =
            DragItems.All(
                di => supportImgExt.Contains(Path.GetExtension(di))
            ) ? DragDropEffects.Link : DragDropEffects.None;
    }

    protected override void Drop(DragEventArgs dragEventArgs) {

        // https://github.com/dwmkerr/sharpshell/issues/278

        // Flag:
        string[] args = {$"\"{SelectedItemPath}\""};
        foreach (var imgPath in DragItems) {
            args = args.Append($"\"{imgPath}\"").ToArray();
        }

        // Mp3CoverDroperApp.exe $PATH $IMGs

        // Process:
        Process process = new Process();
        var info = new ProcessStartInfo(app_path, string.Join(" ", args));
        process.StartInfo = info;
        process.Start();
    }
}