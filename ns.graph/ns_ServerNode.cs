using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Timers;
using System.Net.Sockets;
namespace ns.graph
{
    public class ns_ClientNode
    {
        public Socket ClientSocket;
        public int buffer_size = 1024;
        public delegate void _OnPacketArrived(ns_ClientNode c, net_packet packet);
        public _OnPacketArrived OnNewPacket;

        public delegate void _OnClientDisconnect(ns_ClientNode c);
        public _OnClientDisconnect OnDisconnected;
        public ns_node theNode;
        public string client_id;
        public int rx_packets_count = 0;
        public int tx_packets_count = 0;
        public int port;
        public ns_ClientNode(Socket socket)
        {
            this.ClientSocket = socket;
            this.rx_packets_count = 0;
            this.tx_packets_count = 0;
            this.client_id = socket.RemoteEndPoint.ToString();
            this.port = int.Parse(socket.RemoteEndPoint.ToString().Split(':')[1]);
        }
        public ns_ClientNode (Socket socket , ns_node node)
        {
            this.ClientSocket = socket;
            this.theNode = node;
            this.client_id = node.tag;
            this.rx_packets_count = 0;
            this.tx_packets_count = 0;
            this.port = int.Parse(socket.RemoteEndPoint.ToString().Split(':')[1]);
        }
        public ns_ClientNode(int port , ns_node node )
        {
            this.client_id = node.tag;
            this.theNode = node;
            this.rx_packets_count = 0;
            this.tx_packets_count = 0;
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            

            this.ClientSocket = Connect(socket, new IPAddress[] { IPAddress.Loopback }, port).Result;

            this.port = int.Parse(this.ClientSocket.RemoteEndPoint.ToString().Split(':')[1]);
           
        }
        private async Task<Socket> Connect(Socket sc, IPAddress[] address, int port)
        {
            try
            {
                await Task.Factory.FromAsync(
                    new Func<IPAddress[], int, AsyncCallback, object, IAsyncResult>(sc.BeginConnect),
                    new Action<IAsyncResult>(sc.EndConnect), address, port, null
                    ).ConfigureAwait(false);
                return sc;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void Start(_OnPacketArrived handler, _OnClientDisconnect dishandler)
        {
            Task.Run(() => Run(handler, dishandler));
        }
        public bool SendMsg(string msg )
        {

            try
            {
               
                NetworkStream stream = new NetworkStream(this.ClientSocket);
                byte[] buffer = Encoding.ASCII.GetBytes(msg);
                stream.Write(buffer, 0, buffer.Length);
                
            }catch(Exception ex)
            {
                this.theNode.printfx("Exception[sendMsg] :{0}", ex.Message);
                return false;
            }
            this.tx_packets_count++;
            return true;
        }
        public bool SendPacket(net_packet packet)
        {
            try
            {
                string msg = packet.Encode();
                return this.SendMsg(msg);
            }catch(Exception ex)
            {
                this.theNode.printfx("Exception :{0}", ex.Message);
                return false;
            }
        }
        public void SendMsgTo(string msg , EndPoint epoint)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(msg);
                this.ClientSocket.SendTo(buffer, epoint);

            }catch(Exception ex)
            {
                this.theNode.printfx("{0} Error in SendTo {1}", this.client_id, ex.Message);
            }
        }
        public void Close()
        {
            try
            {
                this.ClientSocket.Close();

            }catch(Exception ex)
            {
                this.theNode.printfx("error in closing {0} : {1}", ex.Message, this.client_id);

            }
        }
        private async Task Run(_OnPacketArrived handler, _OnClientDisconnect dhandler)
        {

            var stream = new NetworkStream(this.ClientSocket);
            // string s = string.Format("Msg From {0} {1} \t\n ", this.ID,DateTime.Now.ToString("hh:mm"));
            var buffer = new byte[this.buffer_size];
            do
            {
                // stream.A


                int len = buffer.Length;

                int byteRead = await stream.ReadAsync(buffer, 0, len).ConfigureAwait(false);
                if (byteRead <= 0)
                {
                    dhandler(this);
                    break;
                }

                if (byteRead > 0)
                {
                    string msg = Encoding.ASCII.GetString(buffer, 0 , byteRead);
                    this.rx_packets_count++;
                    net_packet packet = net_packet.Decode(msg);
                    //packet.data = msg;
                  
                    if(packet.isNull())
                    {
                        packet = new net_packet(this.client_id, "", packetType.Data);
                        packet.data = msg;
                        //packet.src_addr = this.client_id;
                    }
                    handler(this, packet);
                    
                }


            } while (true);
        }

        //end
    }
   public class ns_ServerNode
    {
       public string server_id;
       public ns_node TheNode;
       public Socket ServerSocket;
       public Dictionary<string , ns_ClientNode > Clients;
       public delegate void _OnNewClient(ns_ServerNode server , ns_ClientNode client);
       public delegate void _OnClientDisConnected(ns_ServerNode server, ns_ClientNode client);
       public delegate void _OnPacketArrived(ns_ServerNode server, ns_ClientNode from , net_packet packet);
       public _OnNewClient OnNewClient;
       public _OnClientDisConnected OnClientDisconnect;
       public _OnPacketArrived OnNewPacket;
       public int port;
       public int tx_packets;
       public int rx_packets;
       public EndPoint _EndPoint;
       public ns_ServerNode(int port )
       {
           //this.server_id = server_id;
           //create_server(port);
           this.port = port;
           this.Clients = new Dictionary<string, ns_ClientNode>();
           this.tx_packets = 0;
           this.rx_packets = 0;
       }
       public ns_ServerNode(int port , ns_node node)
       {
           this.server_id = node.tag;
           this.TheNode = node;
           this.port = port;
           this.Clients = new Dictionary<string, ns_ClientNode>();
           this.tx_packets = 0;
           this.rx_packets = 0;
          
       }
       public void Start()
       {
           create_server(this.port);
       }
       public void Close()
       {
           try
           {
               this.ServerSocket.Close();
           }
           catch(Exception ex)
           {
               this.TheNode.printfx("Error in closing server:{0} ->{1}", ex.Message, this.server_id);
           }
       }
       private void create_server(int port)
       {
           var endpoint = new IPEndPoint(IPAddress.Loopback, port);
           this._EndPoint = endpoint;
           this.ServerSocket  = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

           this.ServerSocket.Bind(endpoint);
           this.ServerSocket.Listen(int.MaxValue - 1);//128
           //this.TheNode.printfx("Node {0} is Started", this.server_id);
           //this.mClient = null;
           Task.Run(() => Listen(ServerSocket));
       }
       public void StartReceiving(Server__OnPacketArrived phandler , _OnServerDisconnect dishandler)
       {
           Task.Run(() => Run(phandler, dishandler));
       }
       public bool HasClient(ns_ClientNode c)
       {
           return this.Clients.ContainsKey(c.client_id);
       }
       public bool HasClient(string id)
       {
           return this.Clients.ContainsKey(id);
       }
       public void SendPacketTo(net_packet packet)
       {
           string dst = packet.dst_addr;
           try
           {
               if (this.Clients.ContainsKey(dst))
               {
                  // packet.src_addr = this.server_id;

                   this.Clients[dst].SendPacket(packet);


                   this.rx_packets++;
               }
               else
               {
                   this.TheNode.printfx("Client {1} not connected to {0}", this.server_id, dst);
               }


           }catch(Exception ex)
           {
               this.TheNode.printfx("Error in Server :{0} -->{1}", this.server_id, ex.Message);
           }
       }
       public void SendMsgTo(string  msg , string id)
       {
           try
           {
              // string id = client.client_id;
               if (this.Clients.ContainsKey(id))
               {
                   this.Clients[id].SendMsg(msg);


                   this.rx_packets++;
               }
               else
               {
                   this.TheNode.printfx("Client {1} not connected to {0}", this.server_id, id);
               }
           }
           catch (Exception ex)
           {
               this.TheNode.printfx("Error in Server :{0} -->{1}", this.server_id, ex.Message);
           }
       }
       public void SendMsgTo(string msg , ns_ClientNode client)
       {
           try
           {
               string id = client.client_id;
               if (this.Clients.ContainsKey(id))
               {
                   this.Clients[id].SendMsg(msg);


                   this.rx_packets++;
               }
               else
               {
                   this.TheNode.printfx("Client {1} not connected to {0}", this.server_id, id);
               }
           }
           catch(Exception ex)
           {
               this.TheNode.printfx("Error in Server :{0} -->{1}", this.server_id, ex.Message);
           }
       }
       private int buffer_size = 1024;
       public delegate void _OnServerDisconnect(ns_ServerNode ser);
       public delegate void Server__OnPacketArrived(ns_ServerNode ser, net_packet packet);
       private async Task Run(Server__OnPacketArrived handler, _OnServerDisconnect dhandler)
       {

           var stream = new NetworkStream(this.ServerSocket);
           // string s = string.Format("Msg From {0} {1} \t\n ", this.ID,DateTime.Now.ToString("hh:mm"));
           var buffer = new byte[this.buffer_size];
           do
           {
               // stream.A


               try
               {
                   int len = buffer.Length;

                   int byteRead = await stream.ReadAsync(buffer, 0, len).ConfigureAwait(false);
                   if (byteRead <= 0)
                   {
                       dhandler(this);
                       break;
                   }

                   if (byteRead > 0)
                   {
                       string msg = Encoding.ASCII.GetString(buffer, 0, byteRead);
                       this.rx_packets++;
                       net_packet packet = new net_packet(this.server_id, "", packetType.Data);
                       packet.data = msg;
                        handler(this, packet);
                      // this.TheNode.printfx("[Server:{0}] {1}", this.server_id, msg);
                       if (packet.isNull())
                       {
                           packet = new net_packet(this.server_id, "", packetType.Data);
                           packet.data = msg;
                           //packet.src_addr = this.client_id;
                       }


                   }
               }
               catch (Exception ex)
               {

                   this.TheNode.printfx("Server :{0} error in Run :{1}", this.server_id, ex.Message);
               }


           } while (true);
       }

       private async Task Listen(Socket msocket)
       {
           do
           {

               var client = await Task.Factory.FromAsync(
               new Func<AsyncCallback, object, IAsyncResult>(msocket.BeginAccept),
               new Func<IAsyncResult, Socket>(msocket.EndAccept), null).ConfigureAwait(false);

               // client.r
              var q = this.TheNode.getChildQ();
              ns_node node = q.Dequeue();
              
              ns_ClientNode mc = new ns_ClientNode(client, node);

               this.OnNewClient(this, mc);
               if (!this.Clients.ContainsKey(mc.client_id))
               {
                   this.Clients[mc.client_id] = mc;
               }  
               mc.Start((_client , packet) =>
               {
                   this.rx_packets++;
                   this.OnNewPacket(this, _client, packet);
               }, (_client) =>
               {
                  
                   this.OnClientDisconnect(this, _client);
               });
                           
               

           } while (true);

       }
    }
}
