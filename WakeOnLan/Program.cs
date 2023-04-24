using System;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WakeOnLan
{
    internal static class Program
    {
        [SkipLocalsInit]
        private static unsafe void Main(string[] args)
        {
            string macAddress;

            IPAddress ip;
            
            if (args.Length != 2)
            {
                Console.WriteLine("Enter MAC Address of device to wake up!");

                macAddress = Console.ReadLine();
            
                Console.WriteLine("Enter IP of device!");

                ip = IPAddress.Parse(Console.ReadLine());
            }

            else
            {
                ref var firstArg = ref MemoryMarshal.GetArrayDataReference(args);

                macAddress = firstArg;
                
                ip = IPAddress.Parse(Unsafe.Add(ref firstArg, 1));
            }

            var udpClient = new UdpClient();
    
            //Enable UDP broadcasting for UDPClient
            udpClient.EnableBroadcast = true;
            
            //https://en.wikipedia.org/wiki/Wake-on-LAN
            const int PrefixLength = 6, MacAddressLength = 6, MacAddressRepetitions = 16;

            const int DgramBufferLength = PrefixLength + (MacAddressLength * MacAddressRepetitions);

            ref var macAddrStart = ref MemoryMarshal.GetReference(macAddress.AsSpan());
            
            var _1 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref macAddrStart, 2), NumberStyles.HexNumber);
            
            var _2 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref macAddrStart, 3 * 1), 2), NumberStyles.HexNumber);
            
            var _3 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref macAddrStart, 3 * 2), 2), NumberStyles.HexNumber);
            
            var _4 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref macAddrStart, 3 * 3), 2), NumberStyles.HexNumber);
            
            var _5 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref macAddrStart, 3 * 4), 2), NumberStyles.HexNumber);
            
            var _6 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref macAddrStart, 3 * 5), 2), NumberStyles.HexNumber);
            
            var dgramBuffer = stackalloc byte[DgramBufferLength];

            *((int*) dgramBuffer) = -1;
            
            *((short*) (dgramBuffer + sizeof(int))) = -1;
                
            var macAddrCurrent = dgramBuffer + PrefixLength;

            var macAddrBufferThreshold = dgramBuffer + DgramBufferLength;
            
            //Repeat mac address 16 times
            for (; macAddrCurrent != macAddrBufferThreshold; macAddrCurrent += MacAddressLength)
            {
                macAddrCurrent[0] = _1;
                
                macAddrCurrent[1] = _2;
                
                macAddrCurrent[2] = _3;
                
                macAddrCurrent[3] = _4;
                
                macAddrCurrent[4] = _5;
                
                macAddrCurrent[5] = _6;
            }

            var dgramSpan = new ReadOnlySpan<byte>(dgramBuffer, DgramBufferLength);

            // send datagram using UDP and port 0
            udpClient.Send(dgramSpan, new IPEndPoint(IPAddress.Broadcast, 0));
            
            udpClient.Close();

            Console.WriteLine("Wake on lan sent!");

            var ping = new Ping();
            
            while (true)
            {
                var response = ping.Send(ip);

                if (response.Status != IPStatus.Success)
                {
                    continue;
                }
                
                break;
            }
            
            Console.WriteLine("Target machine is online!");
        }
    }
}