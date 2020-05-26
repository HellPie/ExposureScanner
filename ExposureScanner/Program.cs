using System;
using System.Collections.Generic;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Security.Cryptography;

namespace ExposureScanner {
    class Program {
        private static void Main(string[] args) {
            Console.WriteLine("Configuring BLE advertisement filter...");
            BluetoothLEAdvertisementFilter filter = new BluetoothLEAdvertisementFilter();
            filter.Advertisement.DataSections.Add(new BluetoothLEAdvertisementDataSection(3, CryptographicBuffer.DecodeFromHexString("6FFD")));
            Console.WriteLine("Starting BLE advertisement watcher...");
            BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher(filter);
            watcher.Received += OnReceivedBleAdvertisement;
            watcher.Start();
            while(true) {
                //
            }
        }

        private static void OnReceivedBleAdvertisement(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args) {
            Console.WriteLine(args.Timestamp.ToString("HH:mm:ss") + " - " + args.BluetoothAddress.ToString("X"));
            Console.WriteLine("    - Name: " + args.Advertisement.LocalName);
            Console.WriteLine("    - Strength: " + args.RawSignalStrengthInDBm + "dBm");


            if(args.Advertisement.Flags.HasValue) {
                DebugFlags(args.Advertisement.Flags.Value);
            }

            if(args.Advertisement.ServiceUuids.Count != 0) {
                Console.WriteLine("    - Service UUIDs: " + string.Join("\n          - ", args.Advertisement.ServiceUuids));
            }

            DebugDataSections(args.Advertisement.DataSections);
            DebugManufacturerDataSections(args.Advertisement.ManufacturerData);
        }

        private static void DebugFlags(BluetoothLEAdvertisementFlags flags) {
            if(flags == BluetoothLEAdvertisementFlags.None) {
                return;
            }

            Console.WriteLine("    - Flags:");
            foreach(BluetoothLEAdvertisementFlags flag in Enum.GetValues(typeof(BluetoothLEAdvertisementFlags))) {
                if(flags.HasFlag(flag) && flag != BluetoothLEAdvertisementFlags.None) {
                    Console.WriteLine("          - " + flag);
                }
            }
        }

        private static void DebugDataSections(ICollection<BluetoothLEAdvertisementDataSection> sections) {
            if(sections.Count == 0) {
                return;
            }

            Console.WriteLine("    - Data Sections:");
            foreach(BluetoothLEAdvertisementDataSection section in sections) {
                Console.WriteLine("          - Type: " + section.DataType.ToString("X"));
                Console.WriteLine("          - Data: " + CryptographicBuffer.EncodeToHexString(section.Data));
            }
        }

        private static void DebugManufacturerDataSections(ICollection<BluetoothLEManufacturerData> sections) {
            if(sections.Count == 0) {
                return;
            }

            Console.WriteLine("    - Data Sections:");
            foreach(BluetoothLEManufacturerData section in sections) {
                Console.WriteLine("          - Company ID: " + section.CompanyId.ToString("X"));
                Console.WriteLine("          - Data: " + CryptographicBuffer.EncodeToHexString(section.Data));
            }
        }
    }
}
