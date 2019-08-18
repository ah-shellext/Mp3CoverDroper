using System.Linq;
using System;
using System.Windows.Forms;
using Utils;
using Id3.Frames;
using Id3;

class Program {

    private static string appPath = "Mp3CoverDroperApp";

    static void Main(string[] args) {

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Flag:
        if (args.Length <= 1)
            PrintHelp();

        string mp3Path = args[0];
        string[] imgPaths = args.Except(new string[] { args[0] }).ToArray();

        // Msgbox:
        DialogResult ok = Utils.MessageBoxEx.Show(
            $"\"{mp3Path}\" に選択した {imgPaths.Count()}つ のイメージをカバーとして追加しますか、またはカバーを全部置き換えますか？",
            "カバー編集",
            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
            new string[] { "追加する", "置き換える", "キャンセル" }
        );

        // Handle:
        if (ok == DialogResult.Cancel) return;
        try {
            Produce(ok == DialogResult.No, mp3Path, imgPaths.ToArray());
        }
        catch (Exception ex) {
            MessageBox.Show(ex.ToString());
        }
    }

    static void PrintHelp() {
        Console.WriteLine($"Usage: {appPath} $Mp3Path $ImgPaths");
        Application.Exit();
    }

    private static void Produce(bool needClear, string mp3Path, string[] imgPaths) {
        using (Mp3 mp3 = new Mp3(mp3Path, Mp3Permissions.ReadWrite)) {
            PictureFrameList pictureFrames = Mp3CoverUtil.GetMp3Cover(mp3);
            if (needClear) {
                // ??
                if (!(Mp3CoverUtil.ClearMp3Cover(mp3))) {
                    Restore(mp3, pictureFrames, true);
                    return;
                }
            }
            foreach (var img in imgPaths) {
                if (!Mp3CoverUtil.AddCoverToMp3(mp3, img)) {
                    Restore(mp3, pictureFrames, false);
                    return;
                }
            }
            MessageBox.Show($"{imgPaths.Count()}つ のカバーは追加しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    
    private static void Restore(Mp3 mp3, PictureFrameList pictureFrames, bool isDel) {
        string flag = isDel ? "削除" : "追加";
        if (pictureFrames == null) {
            MessageBox.Show($"mp3 ファイルのカバーの{ flag }は失敗しました。");
        }
        else {
            if (!Mp3CoverUtil.RestoreCover(mp3, pictureFrames)) { 
                // Auth
                MessageBox.Show($"mp3 ファイルのカバーの{ flag }は失敗しました、ファイル還元も失敗しました。");
                return;
            }
            MessageBox.Show($"mp3 ファイルのカバーの{ flag }は失敗しました、元のカバーを戻ります。");
        }
    }
}