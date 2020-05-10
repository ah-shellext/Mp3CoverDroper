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
        public static void getAdmin(string[] args) {
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = Application.ExecutablePath,
                Arguments = string.Join(" ", args),
                Verb = "runas"
            };
            Process.Start(psi);
            Application.Exit();
        }
    }
}
