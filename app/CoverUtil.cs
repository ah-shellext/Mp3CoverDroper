using System;
using System.Linq;
using System.Windows.Forms;
using Id3;
using Id3.Frames;

namespace Utils
{

    public class CoverUtil {

        /// <summary>
        /// Get Mp3 File's Covers Backup
        /// </summary>
        public static PictureFrameList GetMp3Cover(Mp3 mp3) {
            Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
            if (tag == null) {
                Id3Tag newTag = new Id3Tag();
                mp3.WriteTag(newTag, Id3Version.V23, WriteConflictAction.Replace);
                return null;
            }
            PictureFrameList covers = tag.Pictures;
            return covers;
        }

        /// <summary>
        /// Restore Mp3 File's cover Backup
        /// </summary>
        public static bool RestoreCover(Mp3 mp3, PictureFrameList pictureFrames) {
            Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);

            tag.Pictures.Clear();
            foreach (var pictureFrame in pictureFrames)
                tag.Pictures.Add(pictureFrame);
            
            // Ex
            return mp3.UpdateTag(tag);
        }

        /// <summary>
        /// Clear Mp3 File's All Covers !!!
        /// </summary>
        public static bool ClearMp3Cover(Mp3 mp3) {
            Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);

            if (tag.Pictures != null) {
                tag.Pictures.Clear();
            }
            
            mp3.DeleteTag(Id3TagFamily.Version2X);
            return mp3.UpdateTag(tag);
            // return mp3.WriteTag(tag, WriteConflictAction.Replace);
        }

        /// <summary>
        /// Add Image As Cover To Mp3 File !!!
        /// </summary>
        public static bool AddCoverToMp3(Mp3 mp3, string imgPath) {
            Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
            
            PictureFrame newCover = new PictureFrame();
            newCover.LoadImage(imgPath);
            tag.Pictures.Add(newCover);

            return mp3.UpdateTag(tag);
        }
    }
}
