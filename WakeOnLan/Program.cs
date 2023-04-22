using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WakeOnLan
{
    internal static class Program
    {
        private static unsafe void Main(string[] args)
        {
            Console.WriteLine("Enter MAC Address of device to wake up!");

            var macAddress = Console.ReadLine();
            
            var udpClient = new UdpClient();
    
            // enable UDP broadcasting for UdpClient
            udpClient.EnableBroadcast = true;

            //https://benniroth.com/blog/2021-6-21-csharp-wake-over-lan/
            const int PrefixLength = 6, MacAddressLength = 6, MacAddressRepetitions = 16;

            const int DgramBufferLength = PrefixLength + (MacAddressLength * 6);
            
            var dgramBuffer = stackalloc byte[DgramBufferLength];

            *((int*) dgramBuffer) = -1;
            
            *((short*) dgramBuffer) = -1;

            ref var macAddrStart = ref MemoryMarshal.GetReference(macAddress.AsSpan());
            
            var _1 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref macAddrStart, 2), NumberStyles.HexNumber);
            
            var _2 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref macAddrStart, 3 * 1), 2), NumberStyles.HexNumber);
            
            var _3 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref macAddrStart, 3 * 2), 2), NumberStyles.HexNumber);
            
            var _4 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref macAddrStart, 3 * 3), 2), NumberStyles.HexNumber);
            
            var _5 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref macAddrStart, 3 * 4), 2), NumberStyles.HexNumber);
            
            var _6 = byte.Parse(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref macAddrStart, 3 * 5), 2), NumberStyles.HexNumber);
            
            var macAddrCurrent = dgramBuffer + PrefixLength;

            var macAddrBufferThreshold = macAddrCurrent + (MacAddressLength * MacAddressRepetitions);
            
            // repeat MAC-address 16 times in the datagram
            
            for (; macAddrCurrent != macAddrBufferThreshold; macAddrCurrent += MacAddressLength)
            {
                macAddrCurrent[0] = _1;
                
                macAddrCurrent[1] = _2;
                
                macAddrCurrent[2] = _3;
                
                macAddrCurrent[3] = _4;
                
                macAddrCurrent[4] = _5;
                
                macAddrCurrent[5] = _6;
            }

            // send datagram using UDP and port 0
            udpClient.Send(new ReadOnlySpan<byte>(dgramBuffer, DgramBufferLength), new IPEndPoint(IPAddress.Broadcast, 0));
            
            udpClient.Close();
        }
    }
}