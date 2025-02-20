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
        }

        bool TrySetMicaBackdrop(bool useMicaAlt)
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                var micaBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop
                {
                    Kind = useMicaAlt
                        ? Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt
                        : Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base
                };
                this.SystemBackdrop = micaBackdrop;
                return true;
            }
            return false;
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
            return CallWindowProc(_prevWndProc, hWnd, msg, wParam, lParam);
        }

        private delegate IntPtr WndProcDelegate(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WndProcDelegate newProc);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

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
    }
}
