using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpDropHandler;
using Utils;

using Id3.Frames;
using System.Runtime.InteropServices;

[ComVisible(true)]
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
        DialogResult ok = Utils.MessageBoxEx.Show(
            $"\"{SelectedItemPath}\" に選択した {DragItems.Count()}つ のイメージをカバーとして追加しますか、または置き換わりますか？", 
            "カバー編集", 
            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
            new string[] { "追加する", "置き換わる", "キャンセル" }
        );

        if (ok == DialogResult.Cancel) return;

        Produce(true, SelectedItemPath, DragItems.ToArray());
    }

    private void Produce(bool needClear, string mp3Path, string[] imgPaths) {

        MessageBox.Show(mp3Path + "\n\n\n" + string.Join("\n", imgPaths));

        // PictureFrameList pictureFrames = Mp3CoverUtil.GetMp3Cover(mp3Path);
        MessageBox.Show("1");

        if (needClear) {
            if (!(Mp3CoverUtil.ClearMp3Cover(mp3Path))) {
                // Restore(pictureFrames, true);
                MessageBox.Show($"mp3 ファイルのカバーの削除は失敗しました、ファイル還元も失敗しました。");
                return;
            }
        }

        MessageBox.Show("2");

        foreach (var img in imgPaths) {
            if (!Mp3CoverUtil.AddCoverToMp3(mp3Path, img)) {
                // Restore(pictureFrames, false);
                MessageBox.Show($"mp3 ファイルのカバーの追加は失敗しました、ファイル還元も失敗しました。");
                return;
            }
        }
        MessageBox.Show("3");
    }

    /// <summary>
    /// Restore Cover
    /// <summary>
    /// <param name="pictureFrames">PictureFrameList</param>
    /// <param name="isDel">Restore when ClearMp3Cover or AddCoverToMp3</param>
    private void Restore(PictureFrameList pictureFrames, bool isDel) {
        string flag = isDel ? "削除" : "追加";
        MessageBox.Show("flag");
        if (!Mp3CoverUtil.RestoreCover(SelectedItemPath, pictureFrames)) { 
            // Auth
            MessageBox.Show($"mp3 ファイルのカバーの{ flag }は失敗しました、ファイル還元も失敗しました。");
            return;
        }
        MessageBox.Show("RestoreCover");
        MessageBox.Show($"mp3 ファイルのカバーの{ flag }は失敗しました、元のカバーを戻ります。");
    }
}