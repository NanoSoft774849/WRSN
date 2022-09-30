using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.networking;
using System.Windows;
namespace ns.graph
{
    public class ns_communication
    {
        private int port;
        private ns_node thenode;
        private ns_ServerNode server;
        private ns_ClientNode Client;
        public string c_id;
        public ns_communication(int port , ns_node node )
        {
            this.port = port;
            this.thenode = node;
            this.c_id = node.tag;
        }
        private void printfx(string fmt , params object[] args)
        {
            this.thenode.printfx(fmt, args);
        }
        private void logException(Exception ex , string where)
        {
            this.printfx("Exception :{0} at:{1}", ex.Message, where);
        }
        private void Server_OnNewClient(ns_ServerNode server , ns_ClientNode client)
        {
            printfx("{0} is connected to {1} on port {2}", client.client_id, server.server_id, client.port);
            
        }
        private void Server_OnNewPacket(ns_ServerNode server , ns_ClientNode client , net_packet packet)
        {
            //printfx(" msg from {0}->{1}", client.client_id, server.server_id);
            if(packet.Type == packetType.assign_threshold)
            {
                this.thenode.OnAssignThresHoldPacketReceived(packet);
                return;
            }
            if(packet.Type == packetType.assign_importance)
            {
                this.thenode.OnAssignImportancePacketReceived(packet);
                return;
            }
            this.thenode.OnPacketReceivedServer(packet, client.theNode);
           // printfx("rx_packets:{0} -> tx_packets:{1}", client.rx_packets_count, client.tx_packets_count);
            client.theNode.PowerConfig.Calc_PacketDispPower();
            server.TheNode.PowerConfig.Calc_PacketDispPower();
            //client.SendMsgTo("Hi Server!", server._EndPoint);
        }
        public bool ServerHasClient(ns_node node )
        {
            string id = node.tag;
            return this.server.HasClient(id);
        }
        private void Server_OnClientDisconnected(ns_ServerNode server , ns_ClientNode client)
        {
            printfx("{0} disconnected from {1}", client.client_id, server.server_id);
        }
       private void server_rx_msg(ns_ServerNode ser, net_packet packet)
        {
            MessageBox.Show(packet.data.ToString());
        }
        private void server_disconnected(ns_ServerNode ser)
       {
           MessageBox.Show(ser.server_id + "is disconnected!");
       }
        private void create_server()
        {
            try
            {
                this.server = new ns_ServerNode(this.port, this.thenode);
                this.server.OnNewClient = Server_OnNewClient;
                this.server.OnNewPacket = Server_OnNewPacket;
                this.server.OnClientDisconnect = Server_OnClientDisconnected;
                this.server.Start();
                this.server.StartReceiving(server_rx_msg, server_disconnected); ;

            }catch(Exception ex)
            {
                string msg = string.Format("failed to create server:{0}-{1}", this.port, this.thenode.tag);
                logException(ex, msg);
            }
        }

        private void ClientOnPacket(ns_ClientNode c, net_packet packet)
        {
           // MessageBox.Show("Msg arrived:" + packet.data);
           // printfx("{0}-{1}:{2}", packet.src_addr, packet.dst_addr, packet.data);
            if(packet.Type== packetType.assign_threshold)
            {
                this.thenode.OnAssignThresHoldPacketReceived(packet);
                return;
            }
            if (packet.Type == packetType.assign_importance)
            {
                this.thenode.OnAssignImportancePacketReceived(packet);
                return;
            }
            this.thenode.OnPacketReceived(packet, c.theNode.ChildNodes[(n)=>
                {
                    return n.tag == packet.src_addr;
                }]);
            
           // printfx("{0}", packet.data);
        }
        public void ClientDisConnect(ns_ClientNode c)
        {
            printfx("{0} is disconnected", c.client_id);
        }
        private void create_client()
        {
            try
            {
                this.Client = new ns_ClientNode(this.port, this.thenode);
                this.Client.Start(ClientOnPacket, ClientDisConnect);
                //this.Client.
            }
            catch (Exception ex)
            {
                logException(ex, "create_client");
            }
        }
        public int getPort()
        {
            return this.port;
        }
        public void StartClient()
        {

            create_client();
        }
        public ns_node getTheNode()
        {
            return this.thenode;
        }
        public void StopClient()
        {
            if (this.Client != null)
            {
                this.Client.Close();
            }
        }
        public ns_ClientNode getClient()
        {
            return this.Client;
        }
        public ns_ServerNode getServer()
        {
            return this.server;
        }
        public void StartServer()
        {
            create_server();
        }
        public void ServerSendPacketTo(net_packet packet)
        {
            try
            {
                this.server.SendPacketTo(packet);

            }catch(Exception ex)
            {
                this.printfx("Exception in ServerSendPacketTo:{0}", ex.Message);

            }
        }
        public void StopServer()
        {
            try
            {
                this.server.Close();
            }
            catch (Exception)
            {
                printfx("Error in Stop {0}", this.thenode.tag);
            }
        }
        public void SendPacket(net_packet packet )
        {
            try
            {
                if (this.Client != null)
                {
                    this.Client.SendPacket(packet);
                }
            }
            catch (Exception ex)
            {
                printfx("error in sending msg:{0} :{1}", ex.Message, this.thenode.tag);

            }
        }
        public void SendMessageClient(string msg)
        {
            try
            {
                if (this.Client != null)
                {
                    this.Client.SendMsg(msg);
                }
            }
            catch (Exception ex)
            {
                printfx("error in sending msg:{0} :{1}", ex.Message, this.thenode.tag);

            }
        }

    }

// end ns_comm
    public class fxns_communication
    {
        private int port;
        private tcpServer Server;
        private Client client;
        //private delegate void __onpacketReceived(string from, byte[] buffer, int len);
        private ns_node theNode;
        private Dictionary<string, int> packets;
        public fxns_communication(int port  ,  ns_node node)
        {
            this.port = port;
            this.Server = new tcpServer();
            this.Server.server_id = node.tag;
            this.theNode = node;
            this.packets = new Dictionary<string, int>();

        }

        public void SendMessageClient(string msg)
        {
            try
            {
                if(this.client!=null)
                {
                    this.client.SendMsg(msg);
                }

            }catch (Exception ex)
            {
                this.theNode.printfx("Exception in SendMsgClient ;{0}", ex.Message);
            }
        }

        private void ClientOnMsg(string from , byte[] buffer , int len)
        {
            int i = 1;
            string msg = Encoding.ASCII.GetString(buffer, 0, len);

            //net_packet packet = net_packet.Decode(msg);
            //if (!packet.isNull())
            //{

            //}
           

            if (this.packets.ContainsKey(from))
            {
                i = this.packets[from];
                this.packets[from] = i + 1;
            }
            else
            {
                this.packets[from] = 1;
            }
            this.theNode.printfx("{0}->{1}:{2} : {3}", from, this.theNode.tag, msg, i);
        }
        private void ServerOnMsg(string from, byte[] buffer, int len)
        {
            int i = 1;
            string msg = Encoding.ASCII.GetString(buffer, 0, len);
            Client c = Server.myClients[from];
            //net_packet packet = net_packet.Decode(msg);
            //if (!packet.isNull())
            //{
            //    if(packet.Type == packetType.forward)
            //    {

            //    }
            //} 

           // Client c = this.Server.myClients[from];
            //c.SendMsg("Reply from :" + this.theNode.tag);
            this.Server.SendMsgTo("Reply from server", c.mEndPoint);

            if (this.packets.ContainsKey(from))
            {
                i = this.packets[from];
                this.packets[from] = i + 1;
            }
            else
                this.packets[from] = 1;
            this.theNode.printfx("{0}->{1}:{2} : {3}", from, this.theNode.tag, msg, i);
        }
        public void StartClient()
        {
            this.client = new Client(this.port, this.theNode.tag);
            this.client.TheNode = this.theNode;
            this.client.MessageHandler = ClientOnMsg;
               
            this.client.OnDisConnected = (id) =>
                {
                    this.theNode.printfx("{0} is disconnected", id);
                };
           
        }
        public void StopClient()
        {
            if(this.client!=null)
            {
                this.client.Close();
            }
        }

        public Client getClient()
        {
            return this.client;
        }
        public tcpServer getServer()
        {
            return this.Server;
        }
        private void assign_client_toNode(string c)
        {
          //  this.theNode.printfx("{0} is connectedxx: {1}", c, theNode.tag);
            
            var q = this.theNode.getChildQ();
           
            ns_node node = q.Dequeue();
            //if(node!=null)
            {
                this.theNode.printfx("{0}:{2} is connected to {1}", node.tag, theNode.tag, c);
                Server.myClients[c].ID = node.tag;
                node.AddAttribute("ip", c);
                theNode.AddAttribute(node.tag, c);
            }
            
        }
        public void StartServer()
        {
            this.Server.mOnRecv =  ServerOnMsg;
               
            this.Server.OnClientDisConnect = (c) =>
                {
                    this.theNode.printfx("{0} is disconnected", c);
                };
            this.Server.OnNewClient = assign_client_toNode;
                
            try
            {
                this.Server.StartLoopback(this.port, this.theNode.tag);
            }
            catch (Exception ex)
            {
                this.theNode.printfx("Exception in Starting Node {0} --> {1}", this.theNode.tag, ex.Message);
            }
        }
        public void StopServer()
        {
            try
            {
                this.Server.Close();
            }
            catch (Exception )
            {
                this.theNode.printfx("Error in Stop {0}", this.theNode.tag);
            }
        }
    }
}
