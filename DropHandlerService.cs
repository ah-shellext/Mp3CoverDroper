using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpDropHandler;
using Utils;

[COMServerAssociation(AssociationType.ClassOfExtension, ".mp3")]
public class Mp3CoverDroper : SharpDropHandler {

    private string[] supportImgExt = {".jpg", ".png", ".bmp", ".jpeg"};

    protected override void DragEnter(DragEventArgs dragEventArgs) {
        dragEventArgs.Effect =
            DragItems.All(
                di => supportImgExt.Contains(Path.GetExtension(di))
            ) ? DragDropEffects.Link : DragDropEffects.None;
    }

    protected override void Drop(DragEventArgs dragEventArgs) {
            // XsdFilePath = SelectedItemPath,
            // XmlFilePaths = DragItems
        DialogResult ok = Utils.MessageBoxEx.Show(
            $"\"{SelectedItemPath}\" に選択したイメージをカバーとして追加しますか、または置き換わりますか？", 
            "カバー編集", 
            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
            new string[] { "追加する", "置き換わる", "キャンセル" }
        );

        if (ok == DialogResult.Yes) {
            MessageBox.Show("追加する");
        }
        else if (ok == DialogResult.No) {
            MessageBox.Show("置き換わる");
            Mp3CoverUtil.ClearMp3Cover(SelectedItemPath);
        }
        else { // Cancel
            return;
        }

        foreach (var img in DragItems) {
            Mp3CoverUtil.AddCoverToMp3(img, SelectedItemPath);
        }
    }
}