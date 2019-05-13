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
        public void TestFastJoin()
        {
            Packet join = new Packet((byte)0);
            transport.ClientEnqueue(join, "tester", 0);
            server.SingleStep();
            Assert.That(server.numOfClients, Is.EqualTo(1));
            Assert.That(server.Rooms.Count, Is.EqualTo(1));

        }

        [Test]
        public void TestJoin()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "tester", 1);
            server.SingleStep();

            Assert.That(server.numOfClients, Is.EqualTo(1));
            Assert.That(server.numOfRooms, Is.EqualTo(2));
        }

        [Test]
        public void TestDoubleFastJoin()
        {
            Packet join = new Packet((byte)0);
            transport.ClientEnqueue(join, "tester", 0);
            transport.ClientEnqueue(join, "tester", 0);

            server.SingleStep();
            server.SingleStep();
            
            Assert.That(server.numOfClients, Is.EqualTo(1));

        }

        [Test]
        public void TestDoubleJoin()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "tester", 1);
            transport.ClientEnqueue(join, "tester", 1);
            server.SingleStep();
            server.SingleStep();

            Assert.That(server.numOfClients, Is.EqualTo(1));
            Assert.That(server.numOfRooms, Is.EqualTo(2));

        }

        [Test]
        public void TestWrongJoinPacket()
        {
            Packet join = new Packet((byte)0, (byte)1);
            transport.ClientEnqueue(join, "tester", 0);
            server.SingleStep();
            Assert.That(server.numOfClients, Is.EqualTo(0));
        }
        
        [Test]
        public void TestTwoFastJoinRoom()
        {
            Packet join = new Packet((byte)0);
            transport.ClientEnqueue(join, "tester", 0);
            transport.ClientEnqueue(join, "Samba", 0);

            server.SingleStep();
            server.SingleStep();

            Assert.That(server.numOfClients, Is.EqualTo(2));
            Assert.That(server.numOfRooms, Is.EqualTo(1));

        }

        [Test]
        public void TestTwoJoinRoom()
        {
            Packet join = new Packet((byte)0);
            transport.ClientEnqueue(join, "tester", 1);
            transport.ClientEnqueue(join, "Samba", 1);

            server.SingleStep();

            server.SingleStep();

            Assert.That(server.numOfClients, Is.EqualTo(2));
            Assert.That(server.numOfRooms, Is.EqualTo(3));

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
