using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ServerProjectInfiniteRunner.Test
{
    public class Test
    {
        FakeTransport transport;
        Server server;

        [SetUp]
        public void SetupTest()
        {
            transport = new FakeTransport();
            server = new Server(transport);
        }


        [Test]
        public void TestJoin()
        {
            Packet join = new Packet((byte)0);
            transport.ClientEnqueue(join, "tester", 0);
            server.SingleStep();
            Assert.That(server.numOfClient, Is.EqualTo(1));
        }

        [Test]
        public void TestDoubleJoin()
        {
            Packet join = new Packet((byte)0);
            transport.ClientEnqueue(join, "tester", 0);
            transport.ClientEnqueue(join, "tester", 0);
            server.SingleStep();
            server.SingleStep();

            for (int i = 0; i < server.Clients.Count; i++)
            {
                Console.WriteLine(server.Clients[i]);
            }
            Assert.That(server.numOfClient, Is.EqualTo(1));
        }

        [Test]
        public void TestWrongJoinPacket()
        {
            Packet join = new Packet((byte)0, (byte)1);
            transport.ClientEnqueue(join, "tester", 0);
            server.SingleStep();
            Assert.That(server.numOfClient, Is.EqualTo(0));
        }

        [Test]
        public void TestJoinRoom()
        {
            Packet join = new Packet((byte)0);
            transport.ClientEnqueue(join, "tester", 0);
            server.SingleStep();
            
            Assert.That(server.Rooms.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestTwoJoinRoom()
        {
            Packet join = new Packet((byte)0);
            transport.ClientEnqueue(join, "tester", 0);
            transport.ClientEnqueue(join, "Samba", 0);

            server.SingleStep();
            server.SingleStep();

            Assert.That(server.numOfClient, Is.EqualTo(2));
        }

        [Test]
        public void TestJoinWithFullRoom()
        {
            Packet join = new Packet((byte)0);
            transport.ClientEnqueue(join, "tester", 0);
            transport.ClientEnqueue(join, "Samba", 0);
            
            server.SingleStep();
            server.SingleStep();

            transport.ClientEnqueue(join, "Alberto", 0);
            
            server.SingleStep();
            
            Assert.That(server.Rooms.Count, Is.EqualTo(2));
        }
    }
}
