using System;
using System.Linq;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;


namespace BlueLock
{
    /// <summary>
    /// Handles communication via bluetooth and checking for devices in range
    /// </summary>
    public static class BtDeviceScanner
    {
        /// <summary>
        /// Get a list of paired devices, regardless of them being in range or not
        /// </summary>
        public static BluetoothDeviceInfo[] GetDeviceList()
        {            
            Console.WriteLine("Scanning for paired bluetooth devices...");

            var client = new BluetoothClient();

            var pairedDevices = client.DiscoverDevices(10, true, true, false);

            Console.WriteLine("Scan finished.");
            
            return pairedDevices;
        }

        /// <summary>
        /// Check if the given device is within range and has bluetooth turned on
        /// </summary>
        /// <param name="adress">Bluetooth address of the device to check.</param>
        public static bool IsDeviceInRange(BluetoothAddress adress)
        {            
            var device = GetDeviceList().FirstOrDefault(d => d.DeviceAddress == adress);

            if (device == null || device.InstalledServices.Length <= 0)
                return false;

            try
            {
                // Ask the device about the first service in the list, if it responds then we know it is in range and has bluetooth on, if it does not, then it's out of range.                
                device.GetServiceRecords(device.InstalledServices.First());

                Console.WriteLine("Device in range.");

                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Phone out of range.");
            }

            return false;
        }
    }
}
