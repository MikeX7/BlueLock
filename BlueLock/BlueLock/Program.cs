using System;
using System.Diagnostics;
using System.Globalization;
using System.Timers;
using System.Windows.Forms;
using InTheHand.Net;
using Timer = System.Timers.Timer;


namespace BlueLock
{
    public class Program
    {
        
        private static Timer _lockCheckTimer;

        private static void Main(string[] args)
        {
            //Console.WriteLine(Properties.Settings.Default.LockDeviceAdress);
            //Console.ReadLine();

            if (Properties.Settings.Default.LockDeviceAdress == 0 || (args.Length > 0 && args[0] == "setup"))
                Setup();
            else
            {                
                _lockCheckTimer = new Timer
                {
                    Interval = Properties.Settings.Default.LockCheckInterval * 1000,
                    AutoReset = false
                };
                

                _lockCheckTimer.Elapsed += LockCheck;

                _lockCheckTimer.Start();

                Console.ReadLine();
            }
        }
              
        private static void LockCheck(object sender, ElapsedEventArgs e)
        {
            LockCheck();
        }

        /// <summary>
        /// Check if the lock device is within range and if not, lock the PC
        /// </summary>
        private static void LockCheck()
        {
            if (!BtDeviceScanner.IsDeviceInRange(new BluetoothAddress(Properties.Settings.Default.LockDeviceAdress)))
            {
                Console.WriteLine("Locking PC.");
            }

            _lockCheckTimer.Start(); // Wait for the device check to finish and then reset the timer
        }

        /// <summary>
        /// Start the setup process, where the user selects the device to use
        /// </summary>
        private static void Setup()
        {
            Console.Write("Welcome to ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("BlueLock");
            Console.ResetColor();
            Console.WriteLine("\nThis app allows you to use a bluetooth device as a way to lock your computer. After selecting a paired bluetooth device, BlueLock will periodically check if it's within range or not, and if it is not, then it will lock the computer. (Same thing as pressing Win + L).");
            Console.WriteLine("\nPress enter to continue...");
            Console.ReadLine();

            Console.Clear();

            DeviceSelector();
        }

        /// <summary>
        /// Allows the user to select a BT device which we will use a lock check condition
        /// </summary>
        private static void DeviceSelector()
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

            Console.WriteLine("\n Write the ID of the device you want to use:");

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

            Console.WriteLine("Enter the interval at which I should check, if the selected device is within range, in seconds: (default value is 50) ");
            SendKeys.SendWait("50");

            var lockCheckIntervalText = Console.ReadLine();

            int lockCheckInterval;

            while (!Int32.TryParse(lockCheckIntervalText, out lockCheckInterval) || lockCheckInterval < 5) 
            {
                Console.WriteLine("The inserted interval is invalid, or too small, try again...");
                lockCheckIntervalText = Console.ReadLine();
            }

            Properties.Settings.Default["LockCheckInterval"] = lockCheckInterval;
            Properties.Settings.Default.Save();

            BootStarter.ToggleAutostart(true);

            Console.WriteLine("\nThe check will be executed every {0} seconds.", lockCheckInterval);
            Console.WriteLine("\nThis concludes the setup.");
            Console.WriteLine("\nPress enter to finish the setup...");
            
            Console.ReadLine();

            Process.Start("BlueLock.exe"); // As this application closes, start a new instance of it so it can start doing its thing            
        }




    }
}
