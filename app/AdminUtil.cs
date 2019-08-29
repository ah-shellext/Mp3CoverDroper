using System.Linq;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace Utils
{
    public class AdminUtil {
         /// <summary>
        /// Check have administrator authority
        /// </summary>
        public static bool isAdmin() {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();  
            WindowsPrincipal principal = new WindowsPrincipal(identity);  
            return principal.IsInRole(WindowsBuiltInRole.Administrator);  
        }
        
        /// <summary>
        /// Get administrator authority
        /// </summary>
        public static void getAdmin(bool needClear, string mp3Path, string[] imgPaths) {
            string flag = needClear ? "true" : "false";
            string[] args = {$"\"{flag}\" \"{mp3Path}\""};
            foreach (var imgPath in imgPaths) 
                args = args.Append($"\"{imgPath}\"").ToArray();
            
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Application.ExecutablePath;
            psi.Arguments = string.Join(" ", args);
            psi.Verb = "runas";
            try {
                Process.Start(psi);
                Application.Exit();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}