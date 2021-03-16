#pragma warning disable 0618
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System;
using System.Windows.Forms;
using System.Linq;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true)]
namespace Mp3CoverDroper.Implementation {

    class MessageBoxEx {

        private const string Caption = "Mp3CoverDroper";

        public static DialogResult Show(string text, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
            var f = new Form { TopMost = true };
            return MessageBox.Show(f, text, Caption, buttons, icon, defaultButton);
        }

        public static DialogResult Show(string text, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, string[] titles) {
            switch (buttons) {
            case MessageBoxButtons.OK:
                MessageBoxManager.OK = GetOr(titles, 0, "&OK");
                break;
            case MessageBoxButtons.OKCancel:
                MessageBoxManager.OK = GetOr(titles, 0, "&OK");
                MessageBoxManager.Cancel = GetOr(titles, 1, "&Cancel");
                break;
            case MessageBoxButtons.YesNo:
                MessageBoxManager.Yes = GetOr(titles, 0, "&Yes");
                MessageBoxManager.No = GetOr(titles, 1, "&No");
                break;
            case MessageBoxButtons.YesNoCancel:
                MessageBoxManager.Yes = GetOr(titles, 0, "&Yes");
                MessageBoxManager.No = GetOr(titles, 1, "&No");
                MessageBoxManager.Cancel = GetOr(titles, 2, "&Cancel");
                break;
            case MessageBoxButtons.RetryCancel:
                MessageBoxManager.Retry = GetOr(titles, 0, "&Retry");
                MessageBoxManager.Cancel = GetOr(titles, 1, "&Cancel");
                break;
            case MessageBoxButtons.AbortRetryIgnore:
                MessageBoxManager.Abort = GetOr(titles, 0, "&Abort");
                MessageBoxManager.Retry = GetOr(titles, 1, "&Retry");
                MessageBoxManager.Cancel = GetOr(titles, 2, "&Cancel");
                break;
            }
            MessageBoxManager.Register();
            var f = new Form { TopMost = true };
            var r = MessageBox.Show(f, text, Caption, buttons, icon, defaultButton);
            MessageBoxManager.Unregister();
            return r;
        }

        public static DialogResult Show(string text) {
            return Show(text, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, MessageBoxButtons buttons) {
            return Show(text, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, MessageBoxIcon icon) {
            return Show(text, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, MessageBoxButtons buttons, MessageBoxIcon icon) {
            return Show(text, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            return Show(text, buttons, MessageBoxIcon.None, defaultButton);
        }

        public static DialogResult Show(string text, string[] titles) {
            return Show(text, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, titles);
        }

        public static DialogResult Show(string text, MessageBoxButtons buttons, string[] titles) {
            return Show(text, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, titles);
        }

        public static DialogResult Show(string text, MessageBoxIcon icon, string[] titles) {
            return Show(text, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, titles);
        }

        public static DialogResult Show(string text, MessageBoxButtons buttons, MessageBoxIcon icon, string[] titles) {
            return Show(text, buttons, icon, MessageBoxDefaultButton.Button1, titles);
        }

        public static DialogResult Show(string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, string[] titles) {
            return Show(text, buttons, MessageBoxIcon.None, defaultButton, titles);
        }

        private static string GetOr(string[] array, int index, string defaultValue) {
            if (index >= 0 && index < array.Length) {
                return array[index];
            }
            return defaultValue;
        }
    }

    class MessageBoxManager {

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);
        private static readonly HookProc hookProc;
        private static readonly EnumChildProc enumProc;
        [ThreadStatic] private static IntPtr hHook;

        static MessageBoxManager() {
            hookProc = new HookProc(MessageBoxHookProc);
            enumProc = new EnumChildProc(MessageBoxEnumProc);
            hHook = IntPtr.Zero;
        }

        public static void Register() {
            if (hHook == IntPtr.Zero) {
                hHook = SetWindowsHookEx(WH_CALLWNDPROCRET, hookProc, IntPtr.Zero, AppDomain.GetCurrentThreadId());
            }
        }

        public static void Unregister() {
            if (hHook != IntPtr.Zero) {
                UnhookWindowsHookEx(hHook);
                hHook = IntPtr.Zero;
                OK = "&OK"; Cancel = "&Cancel"; Abort = "&Abort"; Retry = "&Retry"; Ignore = "&Ignore"; Yes = "&Yes"; No = "&No";
            }
        }

        public static string OK = "&OK", Cancel = "&Cancel", Abort = "&Abort", Retry = "&Retry", Ignore = "&Ignore", Yes = "&Yes", No = "&No";
        private const int MBOK = 1, MBCancel = 2, MBAbort = 3, MBRetry = 4, MBIgnore = 5, MBYes = 6, MBNo = 7;
        [ThreadStatic] private static int nButton;

        private static IntPtr MessageBoxHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode < 0) {
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            var msg = (CWPRETSTRUCT) Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));
            IntPtr hook = hHook;
            if (msg.message == WM_INITDIALOG) {
                var className = new StringBuilder(10);
                GetClassName(msg.hwnd, className, className.Capacity);
                if (className.ToString() == "#32770") {
                    nButton = 0;
                    EnumChildWindows(msg.hwnd, enumProc, IntPtr.Zero);
                    if (nButton == 1) {
                        IntPtr hButton = GetDlgItem(msg.hwnd, MBCancel);
                        if (hButton != IntPtr.Zero) {
                            SetWindowText(hButton, OK);
                        }
                    }
                }
            }
            return CallNextHookEx(hook, nCode, wParam, lParam);
        }

        private static bool MessageBoxEnumProc(IntPtr hWnd, IntPtr lParam) {
            var className = new StringBuilder(10);
            GetClassName(hWnd, className, className.Capacity);
            if (className.ToString() == "Button") {
                int ctlId = GetDlgCtrlID(hWnd);
                switch (ctlId) {
                case MBOK: SetWindowText(hWnd, OK); break;
                case MBCancel: SetWindowText(hWnd, Cancel); break;
                case MBAbort: SetWindowText(hWnd, Abort); break;
                case MBRetry: SetWindowText(hWnd, Retry); break;
                case MBIgnore: SetWindowText(hWnd, Ignore); break;
                case MBYes: SetWindowText(hWnd, Yes); break;
                case MBNo: SetWindowText(hWnd, No); break;
                }
                nButton++;
            }
            return true;
        }

        private const int WH_CALLWNDPROCRET = 12;
        private const int WM_INITDIALOG = 0x0110;
        [StructLayout(LayoutKind.Sequential)]
        public struct CWPRETSTRUCT { public IntPtr lResult; public IntPtr lParam; public IntPtr wParam; public uint message; public IntPtr hwnd; };
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr idHook);
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "GetWindowTextLengthW", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "GetWindowTextW", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);
        [DllImport("user32.dll")]
        private static extern int EndDialog(IntPtr hDlg, IntPtr nResult);
        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "GetClassNameW", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetDlgCtrlID(IntPtr hwndCtl);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
        [DllImport("user32.dll", EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
        private static extern bool SetWindowText(IntPtr hWnd, string lpString);
    }
}
