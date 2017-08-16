using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
