using System.Windows.Forms;
using Id3;
using Id3.Frames;

namespace Utils
{

    public class Mp3CoverUtil {

        /// <summary>
        /// Get Mp3 File's Covers Backup
        /// </summary>
        public static PictureFrameList GetMp3Cover(string mp3Path) {
            MessageBox.Show("GetMp3Cover");
            Mp3 mp3 = new Mp3(mp3Path, Mp3Permissions.Read);
            Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
            PictureFrameList covers = tag.Pictures;
            return covers;
        }

        /// <summary>
        /// Restore Mp3 File's cover Backup
        /// </summary>
        public static bool RestoreCover(string mp3Path, PictureFrameList pictureFrames) {
            MessageBox.Show("RestoreCover");
            Mp3 mp3 = new Mp3(mp3Path, Mp3Permissions.Write);
            Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);

            tag.Pictures.Clear();
            foreach (var pictureFrame in pictureFrames)
                tag.Pictures.Add(pictureFrame);
            
            return mp3.WriteTag(tag, WriteConflictAction.Replace);
        }

        /// <summary>
        /// Clear Mp3 File's All Covers
        /// </summary>
        public static bool ClearMp3Cover(string mp3Path) {
            MessageBox.Show("ClearMp3Cover " + mp3Path);
            using (Mp3 mp3 = new Mp3(mp3Path, Mp3Permissions.Write)) {
                Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
                tag.Pictures.Clear();
                return mp3.WriteTag(tag, WriteConflictAction.Replace);
            }
        }

        /// <summary>
        /// Add Image As Cover To Mp3 File
        /// </summary>
        public static bool AddCoverToMp3(string mp3Path, string imgPath) {
            MessageBox.Show("AddCoverToMp3");
            using (Mp3 mp3 = new Mp3(mp3Path, Mp3Permissions.Write)) {
                Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
                
                PictureFrame newCover = new PictureFrame();
                newCover.LoadImage(mp3Path);
                tag.Pictures.Add(newCover);

                return mp3.WriteTag(tag, WriteConflictAction.Replace);
            }
        }
    }
}