﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemonstrationVirtualDevice
{
    public class ClientBroadcastReceiver
    {
        private static Thread broadcastReceiverThread;
        private static readonly UdpClient client = new UdpClient(8888);
        private static byte[] responseData = Encoding.ASCII.GetBytes("device_verified");

        public static void Run()
        {
            (broadcastReceiverThread = new Thread(Listen)).Start();
        }

        private static void Listen()
        {
            while (true)
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                string requestString = Encoding.ASCII.GetString(client.Receive(ref endPoint));
                client.Send(responseData, responseData.Length, endPoint);
            }
        }


    }
}
