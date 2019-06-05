using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ServerProjectInfiniteRunner
{
    class TransportIPv4 : ITransport
    {
        Socket socketTcp;
        Socket socketUdp;
        IPEndPoint endPoint;

        List<Socket> readSockets;
        List<Socket> socketsWaitingForWrite;
        Dictionary<Socket, byte[]> dataToSend;

        public TransportIPv4()
        {
            socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socketTcp.Blocking = false;
            socketUdp.Blocking = false;

            List<Socket> readSockets = new List<Socket>();
            List<Socket> socketsWaitingForWrite = new List<Socket>();
            Dictionary<Socket, byte[]> dataToSend = new Dictionary<Socket, byte[]>();
        }

        public void Bind(string address, int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(address), port);

            socketTcp.Bind(endPoint);
            socketTcp.Listen(5);

            socketUdp.Bind(endPoint);
        }

        public EndPoint CreateEndPoint()
        {
            return new IPEndPoint(0, 0);
        }
        
        public byte[] Recv(int bufferSize, ref EndPoint sender)
        {
            int rlen = -1;
            byte[] data = new byte[bufferSize];
            try
            {
                rlen = socketUdp.ReceiveFrom(data, ref sender);
                if (rlen <= 0)
                    return null;
            }
            catch
            {
                return null;
            }
            byte[] trueData = new byte[rlen];
            Buffer.BlockCopy(data, 0, trueData, 0, rlen);
            return trueData;
        }
        
        public bool Send(byte[] data, EndPoint endPoint)
        {
            bool success = false;
            try
            {
                int rlen = socketUdp.SendTo(data, endPoint);
                if (rlen == data.Length)
                    success = true;
            }
            catch
            {
                success = false;
            }
            return success;
        }
    }
}
