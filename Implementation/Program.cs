using Id3;
using Id3.Frames;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Implementation {

    class Program {

        private static readonly string[] supportedImageExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };

        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length <= 1) {
                ShowHelp();
                return;
            }

            // get the arguments
            var mp3Path = args[0];
            var imagePaths = args.Skip(1).ToArray();
            if (Path.GetExtension(mp3Path).ToLower() != ".mp3") {
                ShowError("The given mp3 file has a non-mp3 extension.");
                return;
            }
            if (!imagePaths.All(path => supportedImageExtensions.Contains(Path.GetExtension(path).ToLower()))) {
                ShowError("There are some images that are not supported.");
                return;
            }
            if (!File.Exists(mp3Path) || !imagePaths.All(path => File.Exists(path))) {
                ShowError("Some files given to Mp3CoverDroper is not found, please check first.");
                return;
            }

            // get the mp3 file
            Mp3 mp3;
            try {
                mp3 = new Mp3(mp3Path, Mp3Permissions.ReadWrite);
            } catch (Exception ex) {
                ShowError($"Failed to read mp3 file. Details:\n{ex}");
                return;
            }

            // process the mp3 file
            try {
                MainProcess(mp3, imagePaths);
            } catch (Exception ex) {
                ShowError($"Failed to execute the option. Details:\n{ex}");
            }
        }

        private static void ShowHelp() {
            MessageBox.Show($"Usage: {AppDomain.CurrentDomain.FriendlyName} [mp3 path] [image paths...]", "Mp3CoverDroper", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void ShowError(string message) {
            MessageBox.Show(message, "Mp3CoverDroper", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void MainProcess(Mp3 mp3, string[] images) {
            var tag = mp3.GetTag(Id3TagFamily.Version2X);
            if (tag == null) {
                tag = new Id3Tag();
                mp3.WriteTag(tag, Id3Version.V23, WriteConflictAction.Replace);
            }

            // ask option
            bool addCover;
            bool removeFirst;
            var originCoverCount = tag.Pictures.Count;
            if (originCoverCount == 0) {
                var ok = MessageBox.Show($"There is no cover in the given mp3 file, would you want to add the given {images.Length} cover(s) to this mp3 file?",
                    "Mp3CoverDroper", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                addCover = ok == DialogResult.Yes;
                removeFirst = false;
            } else {
                var ok = MessageBox.Show($"The mp3 file has {originCoverCount} cover(s), would you want to remove it first, and add the given {images.Length} cover(s) to this mp3 file?",
                    "Mp3CoverDroper", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                addCover = ok != DialogResult.Cancel;
                removeFirst = ok == DialogResult.Yes;
            }

            // handle tag
            if (!addCover) {
                return;
            }
            if (removeFirst) {
                tag.Pictures.Clear();
            }
            foreach (var image in images) {
                var newCover = new PictureFrame();
                try {
                    newCover.LoadImage(image);
                } catch (Exception ex) {
                    ShowError($"Failed to load image {image}. Details:\n{ex}");
                    return;
                }
                tag.Pictures.Add(newCover);
            }

            // write tag
            mp3.DeleteTag(Id3TagFamily.Version2X);
            if (!mp3.UpdateTag(tag)) {
                ShowError($"Failed to write cover(s) to mp3 file.");
                return;
            }

            string msg;
            if (removeFirst) {
                msg = $"Success to remove {originCoverCount} cover(s) and add {images.Length} cover(s) to mp3 file.";
            } else if (originCoverCount != 0) {
                msg = $"Success to add {images.Length} cover(s), now there are {images.Length + originCoverCount} covers in the mp3 file.";
            } else {
                msg = $"Success to add {images.Length} cover(s) to mp3 file.";
            }
            MessageBox.Show(msg, "Mp3CoverDroper", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
