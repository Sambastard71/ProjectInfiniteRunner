using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerProjectInfiniteRunner.Test
{
    public struct FakeData
    {
        public FakeEndPoint endPoint;
        public byte[] data;
    }

    class FakeTransport : ITransport
    {
        FakeEndPoint boundAddress;

        Queue<FakeData> recvQueue; 
        Queue<FakeData> sendQueue;

        public FakeTransport()
        {
            recvQueue = new Queue<FakeData>();
            sendQueue = new Queue<FakeData>();
        }

        public int NumOfPacket
        {
            get
            {
                return sendQueue.Count;
            }
        }


        public void ClientEnqueue(FakeData data)
        {
            recvQueue.Enqueue(data);
        }

        public void ClientEnqueue(Packet packet, string address, int port)
        {
            recvQueue.Enqueue( new FakeData() { data = packet.GetData(), endPoint =  new FakeEndPoint(address, port) });
        }

        public FakeData ClientDequeue()
        {
            FakeData packet = sendQueue.Dequeue();
            Console.WriteLine("The command recived is: " + packet.data[0]);
            return packet;
        }

        public void Bind(string address, int port)
        {
            boundAddress = new FakeEndPoint(address, port);
        }

        public EndPoint CreateEndPoint()
        {
            return new FakeEndPoint();
        }

        public byte[] Recv(int bufferSize, ref EndPoint sender)
        {
            FakeData fakeData = recvQueue.Dequeue();
            if (fakeData.data.Length > bufferSize)
            {
                return null;
            }

            sender = fakeData.endPoint;
            return fakeData.data;
        }

        public bool Send(byte[] data, EndPoint endPoint)
        {
            FakeData fakeData = new FakeData();
            fakeData.data = data;
            fakeData.endPoint = endPoint as FakeEndPoint;
            sendQueue.Enqueue(fakeData);
            return true;
        }
    }
}
