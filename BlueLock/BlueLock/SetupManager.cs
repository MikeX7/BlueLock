using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlueLock
{
    public static class SetupManager
    {
        /// <summary>
        /// Start the setup process
        /// </summary>
        public static void Setup()
        {
            Console.Write("Welcome to ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("BlueLock");
            Console.ResetColor();
            Console.WriteLine("\nThis app allows you to use a bluetooth device as a way to lock your computer. After selecting a paired bluetooth device, BlueLock will periodically check if it's within range or not, and if it is not, then it will lock the computer. (Same thing as pressing Win + L).");
            Console.WriteLine("\nPress enter to continue...");
            Console.ReadLine();

            Console.Clear();

            // Displays a series of setup choices to the user
            DevicePicker();
            CheckIntervalSelector();
            LockTimeoutSelector();
            StartOnBootSetup();
            SetupConcluder();
        }

        /// <summary>
        /// Select the bluetooth device which will be used for lock checking
        /// </summary>
        private static void DevicePicker()
        {
            var devices = BtDeviceScanner.GetDeviceList();

            if (devices.Length == 0) // No paired devices were found, give the user some debug tips and exit
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nNo paired devices were found.\n");
                Console.ResetColor();
                Console.WriteLine("Follow these steps, to see if they can solve the problem: ");
                Console.WriteLine(" - Check if your computer supports bluetooth, or if it has a bluetooth adapter connected to it.");
                Console.WriteLine(" - Check if you have correct and up-to-date drivers installed for your bluetooth adapter.");
                Console.WriteLine(" - Make sure the the device is paired with your computer and is visible in the list of paired devices, in your bluetooth management app.");
                Console.WriteLine("\nAfter you followed the steps above, restart BlueLock let it scan for paired devices once more.\n");
                Console.WriteLine("Press enter to exit BlueLock...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nI managed to find {0} paired device/s: \n", devices.Length);

            Console.ForegroundColor = ConsoleColor.Green;


            Console.WriteLine("ID | NAME | LAST SEEN");

            Console.ForegroundColor = ConsoleColor.Cyan;

            var index = 0;

            foreach (var device in devices) // List all found devices so the user can select which one he wants to use
                Console.WriteLine("{0} | {1} | {2}", ++index, device.DeviceName, device.LastSeen);

            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n Write the ID of the device you want to use:");
            Console.ResetColor();

            var deviceIdText = Console.ReadLine();

            int deviceId;

            while (!Int32.TryParse(deviceIdText, out deviceId) || deviceId > devices.Length) // This allows the user to select which device he wants to use and it makes sure the selected device exists
            {
                Console.WriteLine("The inserted ID is invalid, try again...");
                deviceIdText = Console.ReadLine();
            }

            var selectedDevice = devices[deviceId - 1];

            // Save the selected device info into the settings file, so we can load it after app launch
            Properties.Settings.Default["LockDeviceName"] = selectedDevice.DeviceName;
            Properties.Settings.Default["LockDeviceAdress"] = selectedDevice.DeviceAddress.ToInt64();


            Console.WriteLine("\nYou have selected the {0}: {1} as a locking device. \nI will now lock this computer if I detect that {1} isn't within range.\n", deviceId, selectedDevice.DeviceName);
        }

        /// <summary>
        /// Sets an interval at which the device-in-range check will be performed
        /// </summary>
        private static void CheckIntervalSelector()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter the interval at which I should check, if the selected device is within range, in seconds: (default value is 50) ");
            Console.ResetColor();
            SendKeys.SendWait("50");

            var lockCheckIntervalText = Console.ReadLine();
            int lockCheckInterval;

            while (!Int32.TryParse(lockCheckIntervalText, out lockCheckInterval) || lockCheckInterval < 5)
            {
                Console.WriteLine("The inserted interval is invalid, or too small, try again...");
                lockCheckIntervalText = Console.ReadLine();
            }

            Properties.Settings.Default["LockCheckInterval"] = lockCheckInterval;

            Console.WriteLine("\nThe check will be executed every {0} seconds.", lockCheckInterval);
        }

        /// <summary>
        /// Sets a timeout after which the computer will be lock, if the lock device is not detected and does not come within range during this timeout
        /// </summary>
        private static void LockTimeoutSelector()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter the amount of time in seconds, after which the PC should be locked, if the lock device is not detected during it: ");            
            Console.ResetColor();
            SendKeys.SendWait("30");

            var lockTimeoutText = Console.ReadLine();
            int lockTimeout;

            while (!Int32.TryParse(lockTimeoutText, out lockTimeout) || lockTimeout < 1)
            {
                Console.WriteLine("The inserted timeout is invalid, or too small, try again...");
                lockTimeoutText = Console.ReadLine();
            }

            Properties.Settings.Default["LockTimeout"] = lockTimeout;

            Console.WriteLine("\nThe PC will lock, if the lock device is missing for longer than {0} seconds.", lockTimeout);
        }

        /// <summary>
        /// Sets if the BlueLock should auto start when the user logs in 
        /// </summary>
        private static void StartOnBootSetup()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nWould you like for BlueLock to start automatically after boot? y/n");
            Console.ResetColor();
            Console.WriteLine("If you select NO, you can also achieve have the BlueLock start automatically when you log in, by creating a shortcut for the BlueLock.exe file and placing it into \n'C:\\Users\\[Username]\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup' folder. ");

            var autoBoot = Console.ReadLine();

            while (autoBoot != "y" && autoBoot != "n")
            {
                Console.WriteLine("Invalid option, try again... insert y for yes or n for no.");
                autoBoot = Console.ReadLine();
            }

            if (autoBoot == "y")
                Console.WriteLine("BlueLock was set to autoboot.");
            else
                Console.WriteLine("BlueLock autoboot was removed.");

            BootStarter.ToggleAutostart(autoBoot == "y");
        }

        /// <summary>
        /// Conclude the setup process and save changes
        /// </summary>
        private static void SetupConcluder()
        {            
            Properties.Settings.Default.Save();

            Console.WriteLine("\nThis concludes the setup.");
            Console.WriteLine("\nPress enter to finish the setup...");

            Console.ReadLine();

            Process.Start("BlueLock.exe"); // As this application closes, start a new instance of it so it can start doing its thing            
        }
    }
}
