using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Net.Bluetooth;


namespace BlueLock
{
    public class Program
    {
        private static void Main(string[] args)
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

            foreach (var device in devices)
                Console.WriteLine("{0} | {1} | {2}", ++index, device.DeviceName, device.LastSeen);

            Console.ResetColor();

            Console.WriteLine("\n Write the ID of the device you want to use:");

            var deviceIdText = Console.ReadLine();

            int deviceId;

            while (!Int32.TryParse(deviceIdText, out deviceId) || deviceId > devices.Length)
            {
                Console.WriteLine("The inserted ID is invalid, try again...");
                deviceIdText = Console.ReadLine();
            }

            var selectedDevice = devices[deviceId - 1];

            Console.WriteLine("\nYou have selected the {0}: {1} as a locking device, this concludes the setup. \nBlueLock will now lock this computer if it detects that {1} isn't in range.", deviceId, selectedDevice.DeviceName);
            Console.WriteLine("Press enter to finish the setup...");

            Console.ReadLine();
        }




    }
}
