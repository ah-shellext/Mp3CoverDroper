using System.Linq;
using System;
using System.Windows.Forms;
using Utils;

namespace Mp3CoverDroperApp
{
    class Program {

        private static string appPath = "Mp3CoverDroperApp";

        static void Main(string[] args) {

            // Flag:
            MessageBox.Show(string.Join(" ", args));
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

        static void Produce(bool needClear, string mp3Path, string[] imgPaths) {
            MessageBox.Show(mp3Path + "\n\n\n" + string.Join("\n", imgPaths));
        }
    }
}