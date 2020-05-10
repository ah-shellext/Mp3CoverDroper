using System.Linq;
using System;
using System.Windows.Forms;
using Utils;
using Id3.Frames;
using Id3;
using System.Security;

class Program {

    private static string appPath = "Mp3CoverDroperApp";

    static void Main(string[] args) {

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Flag:
        if (args.Length <= 1) {
            ShowHelp();
            return;
        }
        bool isFlag = args[0] == "true" || args[0] == "false";
        bool isOverwrite = args[0] == "true";
        if (isFlag && args.Length <= 2) {
            ShowHelp();
            return;
        }

        if (isFlag) {
            // Mp3CoverDroperApp.exe true $PATH [$IMG]
            string mp3Path = args[1];
            string[] imgPaths = args.Except(new string[] { args[0], args[1] }).ToArray();
            Handle(isOverwrite, mp3Path, imgPaths);
        } 
        else {
            // Mp3CoverDroperApp.exe $PATH [$IMG]
            string mp3Path = args[0];
            string[] imgPaths = args.Except(new string[] { args[0] }).ToArray();
 
            DialogResult ok = Utils.MessageBoxEx.Show(
                $"\"{mp3Path}\" に選択した {imgPaths.Count()}つ のイメージをカバーとして追加しますか、またはカバーを全部置き換えますか？",
                "カバー編集", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                new string[] { "追加する", "置き換える", "キャンセル" }
            );
            if (ok == DialogResult.Cancel) {
                return;
            }
            try {
                Handle(ok == DialogResult.No, mp3Path, imgPaths);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// args show help
    /// </summary>
    static void ShowHelp() {
        Utils.MessageBoxEx.Show($"Usage: {appPath} $Mp3Path $ImgPaths", "エラー");
    }

    /// <summary>
    /// handle mp3 file
    /// </summary>
    private static void Handle(bool needClear, string mp3Path, string[] imgPaths) {
        Mp3 mp3;
        try {
            mp3 = new Mp3(mp3Path, Mp3Permissions.ReadWrite);
        } catch (Exception) {
            if (!Utils.AdminUtil.isAdmin()) {
                string flag = needClear ? "true" : "false";
                string[] args = {$"\"{flag}\" \"{mp3Path}\""};
                foreach (var imgPath in imgPaths) {
                    args = args.Append($"\"{imgPath}\"").ToArray();
                }
                try {
                    Utils.AdminUtil.getAdmin(args);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            } else {
                throw;
            }
        }

        // backup
        PictureFrameList pictureFrames = CoverUtil.GetMp3Cover(mp3);

        if (needClear && !(CoverUtil.ClearMp3Cover(mp3))) {
            Restore(mp3, pictureFrames, true);
            return;
        }
        foreach (var img in imgPaths) {
            // !!!
            if (!CoverUtil.AddCoverToMp3(mp3, img)) {
                Restore(mp3, pictureFrames, false);
                return;
            }
        }

        MessageBox.Show($"{imgPaths.Count()}つのカバー{(needClear ? "を追加し" : "に置き換え")}ました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        mp3.Dispose();
    }
    
    /// <summary>
    /// File handle failed
    /// </summary>  
    private static void Restore(Mp3 mp3, PictureFrameList pictureFrames, bool isDel) {
        string action = isDel ? "削除" : "追加";
        string flag = "";
        if (pictureFrames == null) {
            flag = $"mp3 ファイルのカバーの{ action }は失敗しました。";
        } else if (!CoverUtil.RestoreCover(mp3, pictureFrames)) { 
            flag = $"mp3 ファイルのカバーの{ action }は失敗しましたが、ファイル還元も失敗しました。";
        } else {
            flag = $"mp3 ファイルのカバーの{ action }は失敗しましたが、元のカバーを戻りました。";
        }
        MessageBox.Show(flag, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
