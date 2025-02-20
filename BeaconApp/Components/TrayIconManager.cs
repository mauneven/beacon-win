using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace beacon
{
    public class TrayIconManager : IDisposable
    {
        private const uint NIM_ADD = 0x00000000;
        private const uint NIM_DELETE = 0x00000002;
        private const uint NIF_MESSAGE = 0x00000001;
        private const uint NIF_ICON = 0x00000002;
        private const uint NIF_TIP = 0x00000004;
        private const uint IMAGE_ICON = 1;
        private const uint LR_LOADFROMFILE = 0x00000010;

        private const uint MF_STRING = 0x00000000;
        private const uint TPM_RIGHTBUTTON = 0x00000002;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadImage(IntPtr hInst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool AppendMenu(IntPtr hMenu, uint uFlags, UIntPtr uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyMenu(IntPtr hMenu);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int TrackPopupMenuEx(IntPtr hMenu, uint uFlags, int x, int y, IntPtr hWnd, IntPtr lpTpm);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NOTIFYICONDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uID;
            public uint uFlags;
            public uint uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
            public uint dwState;
            public uint dwStateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;
            public uint uTimeoutOrVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;
            public uint dwInfoFlags;
            public Guid guidItem;
            public IntPtr hBalloonIcon;
        }

        private NOTIFYICONDATA _iconData;
        private bool _iconAdded;
        private IntPtr _windowHandle;
        const int WM_APP = 0x8000;

        public bool IsExiting { get; private set; } = false;

        public TrayIconManager(Window window)
        {
            _windowHandle = WindowNative.GetWindowHandle(window);

            _iconData = new NOTIFYICONDATA();
            _iconData.cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA));
            _iconData.hWnd = _windowHandle;
            _iconData.uID = 0;
            _iconData.uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP;
            _iconData.uCallbackMessage = WM_APP + 1;
            _iconData.szTip = "Beacon App";
            _iconData.hIcon = LoadIcon("C:\\Users\\Maune\\OneDrive\\Documents\\code\\beacon\\Assets\\beacon.ico");

            _iconAdded = Shell_NotifyIcon(NIM_ADD, ref _iconData);
        }

        public void ShowContextMenu()
        {
            IntPtr hMenu = CreatePopupMenu();
            if (hMenu != IntPtr.Zero)
            {
                AppendMenu(hMenu, MF_STRING, (UIntPtr)100, "Open");
                AppendMenu(hMenu, MF_STRING, (UIntPtr)101, "Exit");

                GetCursorPos(out POINT pt);

                SetForegroundWindow(_windowHandle);

                TrackPopupMenuEx(hMenu, TPM_RIGHTBUTTON, pt.X, pt.Y, _windowHandle, IntPtr.Zero);

                PostMessage(_windowHandle, 0, IntPtr.Zero, IntPtr.Zero);

                DestroyMenu(hMenu);
            }
        }

        public void ShowMainWindow()
        {
            if (App.CurrentMainWindow != null)
            {
                ShowWindow(_windowHandle, SW_SHOW);
                App.CurrentMainWindow.Activate();
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_SHOW = 5;

        private IntPtr LoadIcon(string iconPath)
        {
            return LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 0, 0, LR_LOADFROMFILE);
        }

        public void Dispose()
        {
            if (_iconAdded)
            {
                Shell_NotifyIcon(NIM_DELETE, ref _iconData);
            }
        }
    }
}
