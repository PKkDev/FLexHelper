using System;
using System.Linq;

using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using LibUsbDotNet.LibUsb;

using System.Management;

using Device.Net;
using Usb.Net.Windows;
using Hid.Net.Windows;

using System.Threading.Tasks;

namespace FLexHelper.Worker
{
    public static class TestWorkerUSB
    {

        public static async Task Start()
        {
            try
            {
                using (var context = new UsbContext())
                {
                    //Get a list of all connected devices
                    var usbDeviceCollection = context.List();

                    //Narrow down the device by vendor and pid
                    var selectedDevice = usbDeviceCollection.FirstOrDefault(d => d.ProductId == 0x5100 && d.VendorId == 0x13FE);

                    //Open the device
                    selectedDevice.Open();

                    //Get the first config number of the interface
                    var res = selectedDevice.ClaimInterface(selectedDevice.Configs[0].Interfaces[0].Number);

                    //Open up the endpoints
                    var writeEndpoint = selectedDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
                    var readEnpoint = selectedDevice.OpenEndpointReader(ReadEndpointID.Ep01);

                    //Create a buffer with some data in it
                    var buffer = new byte[64];
                    buffer[0] = 0x3f;
                    buffer[1] = 0x23;
                    buffer[2] = 0x23;

                    //Write three bytes
                    writeEndpoint.Write(buffer, 3000, out var bytesWritten);

                    var readBuffer = new byte[64];

                    //Read some data
                    readEnpoint.Read(readBuffer, 3000, out var readBytes);

                    selectedDevice.Close();
                    selectedDevice.Dispose();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e);
                Console.WriteLine(e);
            }

            try
            {
                //Register the factory for creating Hid devices. 
                var hidFactory =
                    new FilterDeviceDefinition()
                    .CreateWindowsHidDeviceFactory();

                //Register the factory for creating Usb devices.
                var usbFactory =
                    new FilterDeviceDefinition()
                    .CreateWindowsUsbDeviceFactory();

                //Join the factories together so that it picks up either the Hid or USB device
                var factories = hidFactory.Aggregate(usbFactory);
                // var factories = usbFactory;


                //Get connected device definitions
                var deviceDefinitions = (await factories.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).ToList();

                if (deviceDefinitions.Count == 0)
                {
                    //No devices were found
                    throw new Exception("(deviceDefinitions.Count == 0");
                }

                //Get the device from its definition
                var device = deviceDefinitions.FirstOrDefault(x => x.ProductId == 20736 && x.VendorId == 5118);
                var trezorDevice = await factories.GetDeviceAsync(device).ConfigureAwait(false);

                //Initialize the device
                await trezorDevice.InitializeAsync().ConfigureAwait(false);

                //Create the request buffer
                var buffer = new byte[65];
                buffer[0] = 0x00;
                buffer[1] = 0x3f;
                buffer[2] = 0x23;
                buffer[3] = 0x23;

                //Write and read the data to the device
                var readBuffer = await trezorDevice.WriteAndReadAsync(buffer).ConfigureAwait(false);

                trezorDevice.Close();
                trezorDevice.Dispose();

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e);
                Console.WriteLine(e);
            }


            // browse all USB WMI physical disks
            var m = new ManagementObjectSearcher("select DeviceID, Model from Win32_DiskDrive");
            var g = m.Get();
            foreach (var drive in g)
            {
                // associate physical disks with partitions
                var h = drive["DeviceID"];
                var partition = new ManagementObjectSearcher(
                    $"associators of {{Win32_DiskDrive.DeviceID='{h}'}} where AssocClass = Win32_DiskDriveToDiskPartition").First();
                if (partition != null)
                {
                    // associate partitions with logical disks (drive letter volumes)
                    var hh = partition["DeviceID"];
                    ManagementObject logical = new ManagementObjectSearcher(
                        $"associators of {{Win32_DiskPartition.DeviceID='{hh}'}} where AssocClass= Win32_LogicalDiskToPartition").First();

                    if (logical != null)
                    {
                        var hhh = logical["Name"];
                        // finally find the logical disk entry
                        ManagementObject volume = new ManagementObjectSearcher(
                            $"select FreeSpace, Size, VolumeName from Win32_LogicalDisk where Name='{hhh}'").First();

                        var name = logical["Name"].ToString();
                        var model = drive["Model"].ToString();
                        var volume2 = volume["VolumeName"].ToString();
                        var freeSpace = (ulong)volume["FreeSpace"];
                        var size = (ulong)volume["Size"];
                    }
                }
            }
        }

        public static ManagementObject First(this ManagementObjectSearcher searcher)
        {
            ManagementObject result = null;
            foreach (ManagementObject item in searcher.Get())
            {
                result = item;
                break;
            }
            return result;
        }
    }
}
