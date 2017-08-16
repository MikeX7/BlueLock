using System;
using System.Runtime.InteropServices;

namespace BlueLock
{
    /// <summary>
    /// Shows or hides the application window
    /// </summary>
    public static class WindowToggler
    {
        public enum WindowState
        {
             Show = 5,
             Hide = 0
        }
        
        private const int HideWindow = 0;
        private const int DisplayWindow = 5;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, WindowState nCmdShow);

        /// <summary>
        /// Set the state of the application window
        /// </summary>
        /// <param name="windowState">Show or hide the window</param>
        public static void ShowWindow(WindowState windowState)
        {
            ShowWindow(GetConsoleWindow(), windowState);
        }
    }
}
