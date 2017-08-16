using System.Runtime.InteropServices;

namespace BlueLock
{
    /// <summary>
    /// Handles locking and unlocking the pc
    /// </summary>
    public static class Locker
    {

        /// <summary>
        /// Lock the computer (same as pressing Win + L)
        /// </summary>
        [DllImport("user32")]
        public static extern void LockWorkStation();
    }
}
