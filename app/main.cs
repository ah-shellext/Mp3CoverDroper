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

        bool isTFFlag = args[0] == "true" || args[0] == "false";
        if ((isTFFlag && args.Length <= 2) || args.Length <= 1) {
            ShowHelp();
            return;
        }

        string mp3Path;
        string[] imgPaths;

        if (isTFFlag) {
            // Mp3CoverDroperApp.exe true $PATH $IMGs
            mp3Path = args[1];
            imgPaths = args.Except(new string[] { args[0], args[1] }).ToArray();
            Produce(args[0] == "true", mp3Path, imgPaths.ToArray());
        } 
        else {
            // Mp3CoverDroperApp.exe $PATH $IMGs
            mp3Path = args[0];
            imgPaths = args.Except(new string[] { args[0] }).ToArray();
 
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
    }

    static void ShowHelp() {
        Utils.MessageBoxEx.Show($"Usage: {appPath} $Mp3Path $ImgPaths", "エラー");
    }

    /// <summary>
    /// Handle mp3 file
    /// </summary>
    private static void Produce(bool needClear, string mp3Path, string[] imgPaths) {
        Mp3 mp3;
        try {
            mp3 = new Mp3(mp3Path, Mp3Permissions.ReadWrite);
        }
        catch (Exception) {
            if (!Utils.AdminUtil.isAdmin()) {
                string flag = needClear ? "true" : "false";
                string[] args = {$"\"{flag}\" \"{mp3Path}\""};
                foreach (var imgPath in imgPaths) 
                    args = args.Append($"\"{imgPath}\"").ToArray();
                
                Utils.AdminUtil.getAdmin(args);
                return;
            }
            else throw;
        }

        // backup
        PictureFrameList pictureFrames = CoverUtil.GetMp3Cover(mp3);
        
        string msg = $"{imgPaths.Count()}つ のカバーは${(needClear ? "追加し" : "置き換え")}ました。";

        if (needClear) {
            if (!(CoverUtil.ClearMp3Cover(mp3))) {
                Restore(mp3, pictureFrames, true);
                return;
            }
        }

        foreach (var img in imgPaths) {
            // !!!
            if (!CoverUtil.AddCoverToMp3(mp3, img)) {
                Restore(mp3, pictureFrames, false);
                return;
            }
        }

        MessageBox.Show(msg, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        mp3.Dispose();
    }
    
    /// <summary>
    /// File handle failed
    /// </summary>  
    private static void Restore(Mp3 mp3, PictureFrameList pictureFrames, bool isDel) {
        string flag = isDel ? "削除" : "追加";
        if (pictureFrames == null) {
            MessageBox.Show($"mp3 ファイルのカバーの{ flag }は失敗しました。");
        }
        else {
            if (!CoverUtil.RestoreCover(mp3, pictureFrames)) { 
                // Auth
                MessageBox.Show($"mp3 ファイルのカバーの{ flag }は失敗しました、ファイル還元も失敗しました。");
                return;
            }
            MessageBox.Show($"mp3 ファイルのカバーの{ flag }は失敗しました、元のカバーを戻ります。");
        }
    }
}