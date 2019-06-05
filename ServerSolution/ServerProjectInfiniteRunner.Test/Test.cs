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
        FakeClock clock;
        FakeTransport transport;
        Server server;

        [SetUp]
        public void SetupTest()
        {
            clock = new FakeClock();
            transport = new FakeTransport();
            server = new Server(transport,clock);
        }

        [Test]
        public void TestServerStart()
        {
            Assert.That(server.numOfRooms, Is.EqualTo(1));
        }

        [Test]
        public void TestZeroNow()
        {
            Assert.That(Server.CurrentClock.GetNow(), Is.EqualTo(0));
        }

        [Test]
        public void TestZeroClientOnStart()
        {
            Assert.That(server.numOfClients, Is.EqualTo(0));
        }

        [Test]
        public void TestJoin()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "tester", 0);
            server.SingleStep();
            Assert.That(server.numOfClients, Is.EqualTo(1));
        }

        [Test]
        public void TestWrongJoin()
        {
            Packet join = new Packet((byte)1, 1);
            transport.ClientEnqueue(join, "tester", 0);
            server.SingleStep();
            Assert.That(server.numOfClients, Is.EqualTo(0));
        }

        [Test]
        public void TestJoinSameClient()
        {
            Packet Join = new Packet(Server.COMMAND_JOIN);
            transport.ClientEnqueue(Join, "tester", 0);
            transport.ClientEnqueue(Join, "tester", 0);
            server.SingleStep();
            server.SingleStep();

            Assert.That(server.numOfClients, Is.EqualTo(1));
        }

        [Test]
        public void TestJoinSameAddresClient()
        {
            Packet Join = new Packet(Server.COMMAND_JOIN);
            transport.ClientEnqueue(Join, "tester", 0);
            server.SingleStep();
            transport.ClientEnqueue(Join, "tester", 1);
            server.SingleStep();

            Assert.That(server.numOfClients, Is.EqualTo(2));
        }
        
        [Test]
        public void TestDoubleJoin()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "tester", 0);
            transport.ClientEnqueue(join, "tester", 0);
            server.SingleStep();
            server.SingleStep();
            Assert.That(server.numOfClients, Is.EqualTo(1));
        }

        [Test]
        public void TestTwoJoin()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);
            server.SingleStep();
            server.SingleStep();
            Assert.That(server.numOfClients, Is.EqualTo(2));
        }

        [Test]
        public void TestAddRoom()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);
            transport.ClientEnqueue(join, "Client3", 0);
            server.SingleStep();
            server.SingleStep();
            server.SingleStep();

            Assert.That(server.numOfRooms, Is.EqualTo(2));
        }

        [Test]
        public void TestWelcomePacketCommand()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            server.SingleStep();
            Assert.That(transport.ClientDequeue().data[0], Is.EqualTo(Server.COMMAND_WELCOME));
        }

        [Test]
        public void TestWelcomePacketIdRoom()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            server.SingleStep();
            uint idRoom = BitConverter.ToUInt32(transport.ClientDequeue().data, 5);
            Assert.That(idRoom, Is.EqualTo(0));
        }

        [Test]
        public void TestWelcomePacketIdPlayer()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            server.SingleStep();
            uint idPlayer = BitConverter.ToUInt32(transport.ClientDequeue().data, 1);
            Assert.That(idPlayer, Is.EqualTo(1));
        }

        [Test]
        public void TestPlayerConnect1PacketCommand()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            
            Assert.That(transport.ClientDequeue().data[0],
                Is.EqualTo(Server.COMMAND_P_CONNECTED));
        }

        [Test]
        public void TestPlayerConnect1PacketIdPlayer()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            uint idPlayer1 = BitConverter.ToUInt32(transport.ClientDequeue().data, 1);

            Assert.That(idPlayer1, Is.EqualTo(2));
        }

        [Test]
        public void TestPlayerConnect1PacketIdRoom()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            uint idRoom1 = BitConverter.ToUInt32(transport.ClientDequeue().data, 4);
            
            Assert.That(idRoom1, Is.EqualTo(0));
        }

        [Test]
        public void TestPlayerConnect2PacketCommand()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            
            Assert.That(transport.ClientDequeue().data[0],
                Is.EqualTo(Server.COMMAND_P_CONNECTED));
        }

        [Test]
        public void TestPlayerConnect2PacketIdPlayer()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            uint idPlayer1 = BitConverter.ToUInt32(transport.ClientDequeue().data, 1);

            Assert.That(idPlayer1, Is.EqualTo(1));
        }

        [Test]
        public void TestPlayerConnect2PacketIdRoom()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            uint idRoom1 = BitConverter.ToUInt32(transport.ClientDequeue().data, 4);

            Assert.That(idRoom1, Is.EqualTo(0));
        }

        [Test]
        public void TestSetUpOp1PacketCommand()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0,10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);

            server.SingleStep();
            
            Assert.That(transport.ClientDequeue().data[0],
                Is.EqualTo(Server.COMMAND_SETUP_OP));
        }

        [Test]
        public void TestSetUpOpPacketIdPlayer()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);

            server.SingleStep();

            uint idPlayer = BitConverter.ToUInt32(transport.ClientDequeue().data, 1);

            Assert.That(idPlayer, Is.EqualTo(1));
        }

        [Test]
        public void TestSetUpOpPacketIdRoom()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);

            server.SingleStep();

            uint idRoom = BitConverter.ToUInt32(transport.ClientDequeue().data, 5);

            Assert.That(idRoom, Is.EqualTo(0));
        }

        [Test]
        public void TestWrongSetUpPacket()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1);
            transport.ClientEnqueue(Setup, "Client1", 0);

            server.SingleStep();
            
            Assert.That(() => transport.ClientDequeue(), 
                Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void TestPlayerBecameIntangible()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            
            Packet intangible = new Packet
                (Server.COMMAND_INTANGIBLE, 1, 0, false);
            transport.ClientEnqueue(intangible, "Client1", 0);

            server.SingleStep();

            Assert.That(server.GetClient(0, 1).Avatar.GetIsCollisionAffected()
                , Is.EqualTo(false));
        }

        [Test]
        public void TestPlayerBecameTangible()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            
            Packet intangible = new Packet
                (Server.COMMAND_INTANGIBLE, 1, 0, true);
            transport.ClientEnqueue(intangible, "Client1", 0);

            server.SingleStep();

            Assert.That(server.GetClient(0, 1).Avatar.GetIsCollisionAffected(), Is.EqualTo(true));
        }

        [Test]
        public void TestWrongIntangiblePacket()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet intangible = new Packet
                (Server.COMMAND_INTANGIBLE);
            transport.ClientEnqueue(intangible, "Client1", 0);

            Assert.That(() => transport.ClientDequeue(),
                Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void TestIntangibleOpPacket()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet intangible = new Packet
                (Server.COMMAND_INTANGIBLE, 1, 0, true);
            transport.ClientEnqueue(intangible, "Client1", 0);

            server.SingleStep();

            Assert.That(transport.ClientDequeue().data[0],
                Is.EqualTo(Server.COMMAND_INTANGIBLE_OP));
        }

        [Test]
        public void TestIntangibleOpTangible()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet intangible = new Packet
                (Server.COMMAND_INTANGIBLE, 1, 0, true);
            transport.ClientEnqueue(intangible, "Client1", 0);

            server.SingleStep();

            bool isIntangible = BitConverter.ToBoolean(transport.ClientDequeue().data, 5);

            Assert.That(isIntangible, Is.EqualTo(true));
        }

        [Test]
        public void TestIntangibleOpIntangible()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            server.SingleStep();

            transport.ClientEnqueue(join, "Client2", 0);
            server.SingleStep();
            
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet intangible = new Packet
                (Server.COMMAND_INTANGIBLE, 1, 0, false);
            transport.ClientEnqueue(intangible, "Client1", 0);

            server.SingleStep();

            bool isIntangible = BitConverter.ToBoolean(transport.ClientDequeue().data, 5);

            Assert.That(isIntangible, Is.EqualTo(false));
        }

        [Test]
        public void TestCollisionPlayerPlayer()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            server.GetClient(0, 1).Avatar.SetPosition(10f, 10f, 10f);
            server.GetClient(0, 2).Avatar.SetPosition(10f, 10f, 10f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();
            
            Assert.That(collision.Collider, Is.EqualTo(null));
        }

        [Test]
        public void TestCollisionPlayerPlayerDeltaX()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            server.GetClient(0, 1).Avatar.SetPosition(10f, 10f, 10f);
            server.GetClient(0, 2).Avatar.SetPosition(10f, 10f, 10f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();

            Assert.That(collision.DeltaX, Is.EqualTo(0.0f));
        }

        [Test]
        public void TestCollisionPlayerPlayerDeltaY()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            server.GetClient(0, 1).Avatar.SetPosition(10f, 10f, 10f);
            server.GetClient(0, 2).Avatar.SetPosition(10f, 10f, 10f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();

            Assert.That(collision.DeltaY, Is.EqualTo(0.0f));
        }

        [Test]
        public void TestNotCollisionPlayerPlayer()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            server.GetClient(0, 1).Avatar.SetPosition(200f, 200f, 200f);
            server.GetClient(0, 2).Avatar.SetPosition(10f, 10f, 10f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();

            Assert.That(collision.Collider, Is.EqualTo(null));
        }

        [Test]
        public void TestCollisionPlayerObstacle()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();
            
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            server.GetClient(0, 1).Avatar.SetPosition(10f, 10f, 10f);

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 2);
            obstacle.SetPosition(10f, 10f, 10f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();
            
            Assert.That(collision.Collider, Is.TypeOf<Avatar>());
        }

        [Test]
        public void TestCollisionPlayerObstacleDeltaX()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            server.GetClient(0, 1).Avatar.SetPosition(10f, 10f, 10f);

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 2);
            obstacle.SetPosition(10f, 10f, 10f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();

            Assert.That(collision.DeltaX, Is.EqualTo(18.5f));
        }

        [Test]
        public void TestCollisionPlayerObstacleDeltaY()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            server.GetClient(0, 1).Avatar.SetPosition(10f, 10f, 10f);

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 2);
            obstacle.SetPosition(10, 10, 10);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();

            Assert.That(collision.DeltaY, Is.EqualTo(28.5f));
        }

        [Test]
        public void TestNotCollisionPlayerObstacle()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            server.GetClient(0, 1).Avatar.SetPosition(10f, 10f, 10f);

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 2);
            obstacle.SetPosition(200f, 200f, 200f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();

            Assert.That(collision.Collider, Is.EqualTo(null));
        }

        [Test]
        public void TestCollisionObstacleObstacle()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);

            server.SingleStep();

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 2);
            Obstacle obstacle1 = SpawnManager.Spawn(server.GetRoom(0), 2);

            obstacle.SetPosition(100f, 100f, 100f);
            obstacle1.SetPosition(100f, 100f, 100f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();

            Assert.That(collision.Collider, Is.Null);
        }

        [Test]
        public void TestCollisionObstacleObstacleDeltaX()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);

            server.SingleStep();

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 2);
            Obstacle obstacle1 = SpawnManager.Spawn(server.GetRoom(0), 2);

            obstacle.SetPosition(100f, 100f, 100f);
            obstacle1.SetPosition(100f, 100f, 100f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();

            Assert.That(collision.DeltaY, Is.EqualTo(0.0f));
        }

        [Test]
        public void TestCollisionObstacleObstacleDeltaY()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);

            server.SingleStep();

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 2);
            Obstacle obstacle1 = SpawnManager.Spawn(server.GetRoom(0), 2);

            obstacle.SetPosition(100f, 100f, 100f);
            obstacle1.SetPosition(100f, 100f, 100f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();

            Assert.That(collision.DeltaY, Is.EqualTo(0.0f));
        }

        [Test]
        public void TestNotCollisionObstacleObstacle()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);

            server.SingleStep();

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 2);
            Obstacle obstacle1 = SpawnManager.Spawn(server.GetRoom(0), 2);

            obstacle.SetPosition(-200f, -200f, -200f);
            obstacle1.SetPosition(200f, 200f, 200f);

            server.Process();

            Collision collision = UpdateManager.GetCollisionInfo();

            Assert.That(collision.Collider, Is.Null);
        }

        [Test]
        public void TestSpawn()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 2);

            server.Process();
            
            Assert.That(transport.ClientDequeue().data[0], Is.EqualTo(Server.COMMAND_SPAWN));
        }

        [Test]
        public void TestDestroy()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);
            transport.ClientEnqueue(join, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();

            Packet Setup = new Packet
                (Server.COMMAND_SETUP, 1, 0, 10f, 10f, 10f, 12f, 12f, 20f, 20f, 20f);
            transport.ClientEnqueue(Setup, "Client1", 0);
            transport.ClientEnqueue(Setup, "Client2", 0);

            server.SingleStep();
            server.SingleStep();

            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();
            transport.ClientDequeue();

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 2);
            obstacle.SetPosition(100.0f, 0.0f, 0.0f);

            server.Process();

            transport.ClientDequeue();

            server.Process();

            Assert.That(transport.ClientDequeue().data[0], Is.EqualTo(Server.COMMAND_SPAWN));
        }

        [Test]
        public void TestUpdateObstacle()
        {
            Packet join = new Packet((byte)1);
            transport.ClientEnqueue(join, "Client1", 0);

            server.SingleStep();

            Obstacle obstacle = SpawnManager.Spawn(server.GetRoom(0), 1);
            obstacle.SetPosition(100f, 100f, 100f);

            clock.IncreaseTimeStamp(1);

            server.Process();

            Assert.That(obstacle.Position.X, Is.EqualTo(100 + (obstacle.Velocity.X)));
        }
    }
}
