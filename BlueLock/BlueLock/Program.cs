using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using InTheHand.Net;
using Timer = System.Timers.Timer;


namespace BlueLock
{
    public class Program
    {
        private static DateTime _lastTimeLockDeviceInRange; // When was the last time we detected the lock device
        private static Timer _lockCheckTimer;

        private static void Main(string[] args)
        {            

            if (Properties.Settings.Default.LockDeviceAdress == 0 || (args.Length > 0 && args[0] == "setup"))
                SetupManager.Setup();
            else
            {
                if ((args.Length == 0 || args[0] != "console")) // Only hide the window if we don't receive an argument to show it
                    WindowToggler.ShowWindow(WindowToggler.WindowState.Hide);                

                _lockCheckTimer = new Timer
                {
                    Interval = Properties.Settings.Default.LockCheckInterval * 1000,
                    AutoReset = false
                };


                _lockCheckTimer.Elapsed += (sender, eventArgs) => LockCheck();

                _lockCheckTimer.Start();

                while (true) // This will prevent the application from being closed by pressing enter
                    Console.ReadLine();
            }
        }                      

        /// <summary>
        /// Check if the lock device is within range and if not, lock the PC
        /// </summary>
        private static void LockCheck()
        {
            if (!BtDeviceScanner.IsDeviceInRange(new BluetoothAddress(Properties.Settings.Default.LockDeviceAdress)))
            {
                if ((DateTime.Now - _lastTimeLockDeviceInRange).TotalSeconds > Properties.Settings.Default.LockTimeout) // Even though the device is not in range, don't lock the computer until it is not in range for the lock timeout amount of seconds
                {
                    Console.WriteLine("Locking PC.");

                    try
                    {
                        //Locker.LockWorkStation();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            else
                _lastTimeLockDeviceInRange = DateTime.Now;

            _lockCheckTimer.Start(); // Wait for the device check to finish and then reset the timer
        }

    




    }
}
