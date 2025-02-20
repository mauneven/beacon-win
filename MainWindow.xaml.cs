using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace beacon
{
    public sealed partial class MainWindow : Window
    {
        private const int WM_GETMINMAXINFO = 0x0024;
        private const int GWL_WNDPROC = -4;
        private static IntPtr _prevWndProc = IntPtr.Zero;
        private static WndProcDelegate? _newWndProc;

        private const int MinWidth = 500;
        private const int MinHeight = 600;
        private const int SW_HIDE = 0;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_LBUTTONDBLCLK = 0x0203;

        private const int WM_APP = 0x8000;
        private const int WM_SHOW_CONTEXT_MENU = WM_APP + 2;
        private const int WM_SHOW_MAIN_WINDOW = WM_APP + 3;

        private const int WM_COMMAND = 0x0111;

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public MainWindow()
        {
            this.InitializeComponent();
            MainFrame.Navigate(typeof(beacon.BeaconApp.Pages.Home.Home));
            this.TrySetMicaBackdrop(true);

            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            _newWndProc = new WndProcDelegate(CustomWndProc);
            _prevWndProc = SetWindowLongPtr(hWnd, GWL_WNDPROC, _newWndProc);

            const uint SWP_NOMOVE = 0x0002;
            const uint SWP_NOZORDER = 0x0004;
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, MinWidth, MinHeight, SWP_NOMOVE | SWP_NOZORDER);

            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Closing += AppWindow_Closing;
        }

        private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            args.Cancel = true;
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            ShowWindow(hWnd, SW_HIDE);
        }

        private static IntPtr CustomWndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                mmi.ptMinTrackSize.x = MinWidth;
                mmi.ptMinTrackSize.y = MinHeight;
                Marshal.StructureToPtr(mmi, lParam, true);
                return IntPtr.Zero;
            }

            if (msg == WM_APP + 1)
            {
                if (App.TrayIconManagerInstance?.IsExiting == true)
                    return IntPtr.Zero;

                int lParamInt = lParam.ToInt32();
                if (lParamInt == WM_RBUTTONUP)
                {
                    PostMessage(hWnd, WM_SHOW_CONTEXT_MENU, IntPtr.Zero, IntPtr.Zero);
                    return IntPtr.Zero;
                }
                else if (lParamInt == WM_LBUTTONUP || lParamInt == WM_LBUTTONDBLCLK)
                {
                    PostMessage(hWnd, WM_SHOW_MAIN_WINDOW, IntPtr.Zero, IntPtr.Zero);
                    return IntPtr.Zero;
                }
            }
            else if (msg == WM_SHOW_CONTEXT_MENU)
            {
                App.TrayIconManagerInstance?.ShowContextMenu();
                return IntPtr.Zero;
            }
            else if (msg == WM_SHOW_MAIN_WINDOW)
            {
                App.TrayIconManagerInstance?.ShowMainWindow();
                return IntPtr.Zero;
            }
            else if (msg == WM_COMMAND)
            {
                int commandId = wParam.ToInt32() & 0xFFFF;
                if (commandId == 100)
                {
                    App.TrayIconManagerInstance?.ShowMainWindow();
                    return IntPtr.Zero;
                }
                else if (commandId == 101)
                {
                    if (App.TrayIconManagerInstance != null)
                    {
                        App.TrayIconManagerInstance.Dispose();
                    }
                    Application.Current.Exit();
                    return IntPtr.Zero;
                }
            }

            return CallWindowProc(_prevWndProc, hWnd, msg, wParam, lParam);
        }

        private delegate IntPtr WndProcDelegate(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WndProcDelegate newProc);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        bool TrySetMicaBackdrop(bool useMicaAlt)
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                var micaBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop
                {
                    Kind = useMicaAlt ? Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt : Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base
                };
                this.SystemBackdrop = micaBackdrop;
                return true;
            }
            return false;
        }
    }
}
