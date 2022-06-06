using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneration
{
    [StructLayout(LayoutKind.Sequential)]
    struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    internal class Controller
    {
        [DllImport("USER32.DLL")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
        [DllImport("USER32.DLL")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        //[DllImport("USER32.DLL")]
        //public static extern bool SetForegroundWindow(IntPtr hWnd);
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const int SLEEP_TIME = 20;
        public static void SetResolution(int width, int height, IntPtr window)
        {
            ExecuteCommand("mat_setvideomode " + width + " " + height + " 1", window);
            Thread.Sleep(20000);
        }
        public static void SetReplayFile(string path, IntPtr window)
        {
            ExecuteCommand("playdemo " + path, window);
            Thread.Sleep(20000);
        }
        public static void GotToTick(int tick, IntPtr window)
        {
            ExecuteCommand("demo_gototick " + tick, window);
            Thread.Sleep(10000);
        }
        public static void Pause(IntPtr window)
        {
            ExecuteCommand("demo_pause", window);
            Thread.Sleep(1000);
        }
        public static void GoToPlayer(int player, IntPtr window)
        {
            ExecuteCommand("spec_player " + player, window);
            Thread.Sleep(1000);
        }
        public static Bitmap TakeScreenshot(IntPtr window)
        {
            Rect rect = new Rect();
            GetWindowRect(window, ref rect);
            Rectangle bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

            Bitmap result = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(result))
            {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }
        private static void ExecuteCommand(string command, IntPtr window)
        {
            if (window == IntPtr.Zero)
            {
                throw new Exception("Window not found.");
            }
            OpenConsole(window);
            Thread.Sleep(SLEEP_TIME);
            for (int i = 0; i < command.Length; i++)
            {
                WriteChar(window, command[i]);
                Thread.Sleep(SLEEP_TIME);
            }
            PressEnter(window);
            CloseConsole(window);

            //Thread.Sleep(SLEEP_TIME);
            //keybd_event((byte)0xA0, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            //Thread.Sleep(SLEEP_TIME);
            //keybd_event((byte)0xA0, 0, KEYEVENTF_KEYUP | 0, 0);
            //Thread.Sleep(SLEEP_TIME);
            //keybd_event((byte)0xA5, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            //Thread.Sleep(SLEEP_TIME);
            //keybd_event((byte)0xA5, 0, KEYEVENTF_KEYUP | 0, 0);
        }
        private static void PressEnter(IntPtr window)
        {
            PostMessage(window, 0x0104, 0x0D, 0);
        }
        private static void OpenConsole(IntPtr window)
        {
            //SetForegroundWindow(window);
            ////Thread.Sleep(1000);
            ////keybd_event((byte)0x73, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            ////keybd_event((byte)0x73, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            ////Thread.Sleep(1000);
            ////keybd_event((byte)0x73, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            ////keybd_event((byte)0x73, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            ////keybd_event((byte)0x73, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);

            ////keybd_event((byte)0x73, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);



            ////PostMessage(window, 0x0105, 0x73, 0);
            //while (true) 
            //{
            //    Thread.Sleep(1000);
            //    PostMessage(window, 0x0100, 0xDC, 0x390000);
            //    Thread.Sleep(1000);
            //    PostMessage(window, 0x0101, 0xDC, 0x390000);
            //}

        }
        private static void CloseConsole(IntPtr window)
        {
            //PostMessage(window, 0x0104, 0x1B, 0);
        }
        private static void WriteChar(IntPtr window, char c)
        {
            if (c >= 'a' && c <= 'z')
            {
                PostMessage(window, 0x0104, 0x41 + (c - 'a'), 0);
            }
            else if (c >= 'A' && c <= 'Z')
            {
                keybd_event((byte)0xA0, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                Thread.Sleep(SLEEP_TIME);
                PostMessage(window, 0x0104, 0x41 + (c - 'A'), 0);
                Thread.Sleep(SLEEP_TIME);
                keybd_event((byte)0xA0, 0, KEYEVENTF_KEYUP | 0, 0);
            }
            else if (c >= '0' && c <= '9')
            {
                PostMessage(window, 0x0104, 0x30 + (c - '0'), 0);
            }
            else if (c == ' ')
            {
                PostMessage(window, 0x0104, 0x20, 0);
            }
            else if (c == '_')
            {
                keybd_event((byte)0xA0, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                Thread.Sleep(SLEEP_TIME);
                PostMessage(window, 0x0104, 0xBD, 0);
                Thread.Sleep(SLEEP_TIME);
                keybd_event((byte)0xA0, 0, KEYEVENTF_KEYUP | 0, 0);
            }
            else if (c == '/')
            {
                keybd_event((byte)0xA0, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                Thread.Sleep(SLEEP_TIME);
                PostMessage(window, 0x0104, '7', 0);
                Thread.Sleep(SLEEP_TIME);
                keybd_event((byte)0xA0, 0, KEYEVENTF_KEYUP | 0, 0);
            }
            else if (c == '\\')
            {
                keybd_event((byte)0xA5, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                Thread.Sleep(SLEEP_TIME);
                PostMessage(window, 0x0104, 0xDB, 0);
                Thread.Sleep(SLEEP_TIME);
                keybd_event((byte)0xA5, 0, KEYEVENTF_KEYUP | 0, 0);
            }
            else if (c == '-')
            {
                PostMessage(window, 0x0104, 0xBD, 0);
            }
            else if (c == '^')
            {
                PostMessage(window, 0x0104, 0xDC, 0);
            }
            else if (c == ':')
            {
                keybd_event((byte)0xA0, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                Thread.Sleep(SLEEP_TIME);
                PostMessage(window, 0x0104, 0xBE, 0);
                Thread.Sleep(SLEEP_TIME);
                keybd_event((byte)0xA0, 0, KEYEVENTF_KEYUP | 0, 0);
            }
            else if (c == '.')
            {
                PostMessage(window, 0x0104, 0xBE, 0);//c0
            }
            else if (c == '(')
            {
                keybd_event((byte)0xA0, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                Thread.Sleep(SLEEP_TIME);
                PostMessage(window, 0x0104, '8', 0);
                Thread.Sleep(SLEEP_TIME);
                keybd_event((byte)0xA0, 0, KEYEVENTF_KEYUP | 0, 0);
            }
            else if (c == ')')
            {
                keybd_event((byte)0xA0, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                Thread.Sleep(SLEEP_TIME);
                PostMessage(window, 0x0104, '9', 0);
                Thread.Sleep(SLEEP_TIME);
                keybd_event((byte)0xA0, 0, KEYEVENTF_KEYUP | 0, 0);
            }
            else
            {
                throw new Exception("unknown char");
            }
        }
    }
}
