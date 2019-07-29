using System.Runtime.InteropServices;


namespace Mogster.Core.Helpers
{
    public class NativeMethods
    {
        public delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}
