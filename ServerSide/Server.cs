using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Timers;
using System.Net.NetworkInformation;

namespace ns.networking
{
    

    public static class NsDns
    {
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }
        public static string GetHostByAddress(string Ip_port)
        {
            string ii = Ip_port;
            if (Ip_port.Contains(':'))
                ii = Ip_port.Split(':')[0];

            return Dns.GetHostByAddress(ii).HostName;
        }
        public static string[] GetInterfacesNames()
        {
            NetworkInterface[] ifaces = NetworkInterface.GetAllNetworkInterfaces();
            string[] _ifaces = new string[ifaces.Length];
            int i=0;
            foreach(NetworkInterface iface in ifaces)
            {
               // iface.NetworkInterfaceType==NetworkInterfaceType.
                _ifaces[i++] = string.Format("Name:{0} address:{1}", iface.Name, iface.GetIPProperties().DnsAddresses);
               
            }
            return _ifaces;
        }
        public static string[] GetLocalHostInterfaces()
        {
           

            IPAddress[] addresses = Dns.GetHostAddresses(GetHostName());


            string[] _list = new string[addresses.Length];
           
            int i = 0;
            foreach (IPAddress address in addresses)
            {
               
                if (address.GetAddressBytes().Length == 4)
                    _list[i++] = string.Format("{0}", address);
                ;


            }
            return _list;


        }
    }

    public enum ClientType
    {
        Sensor=1,
        http=2,
        Websocket=3,
    }
    public class HandshakeProtocol
    {
        public static string send_dev_info_cmd = "send device info";
        public static string sleep_cmd = "sleep";
        public static string start_cmd = "start";
        public static string wake_up_cmd = "wake up";
        public static string send_again_cmd = "send again";
        public static string send_ref_img_cmd = "send ref";
        public static string snapshot_cmd = "snapshot";
        public static string stop_cmd = "stop";
        public static string ping_cmd = "ping";
        public bool dev_info_cmd_sent;
        public bool sleep_cmd_sent;
        public bool start_cmd_sent;
        public bool wake_up_cmd_sent;
        public bool dev_info_received;
        public bool send_again_cmd_sent;
        public bool send_snapshot_cmd_sent;
        public bool control_cmd_sent;
        public bool control_cmd_response_received;
        public string sent_cmd;
        public HandshakeProtocol()
        {
            this.dev_info_cmd_sent = false;
            this.dev_info_received = false;
            this.send_again_cmd_sent = false;
            this.sleep_cmd_sent = false;
            this.start_cmd_sent = false;
            this.wake_up_cmd_sent = false;
            this.send_snapshot_cmd_sent = false;
            this.control_cmd_sent = false;
            this.control_cmd_response_received = false;
        }
    }
    public class ReceiveFileProtocol
    {
        public string file_name;
        public int file_size;
        public BinaryWriter bw;
        public bool isInProgress;
        public bool isEnd;
        public int rx_written;
        public int written;
        public string client_id;
        public Client mClient;
         
        public delegate void _OnError(string error, string where);
        public delegate void _OnReceiveFileComplete(ReceiveFileProtocol rx);
        public delegate void _OnPacketError( ReceiveFileProtocol rfp);
        public  _OnReceiveFileComplete OnReceiveFileComplete;
        public _OnReceiveFileComplete OnTimeout;
       
        public _OnError OnError;
        public Timer mTimer;
        public int rec_time_ms;
        private int timer_interval=100;//100ms;
        private double max_time=90;//120sec;//2mins // 
        public bool TimerIsRunning;
        public ReceiveFileProtocol(string fn)
        {
            written = 0;
            this.bw = new BinaryWriter(File.Open(fn, FileMode.Create));
            this.file_name = fn;
            rec_time_ms = 0;
            mTimer = new Timer(timer_interval);
            
            mTimer.Elapsed += mTimer_Elapsed;
            
            mTimer.Start();
            TimerIsRunning = true;
            rx_written = 0;

        }
        public ReceiveFileProtocol(BinaryWriter bwx)
        {
            written = 0;
            this.bw = bwx;
            rx_written = 0;
            rec_time_ms = 0;
            mTimer = new Timer(timer_interval);
            mTimer.Elapsed += mTimer_Elapsed;
            mTimer.Start();
            TimerIsRunning = true;
            rx_written = 0;
           //this.file_name = fn;
        }

        private void mTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.rec_time_ms = this.rec_time_ms + 1;
            double tt = this.rec_time_ms*0.1;//in sec
            if(tt>=max_time)
            {
                this.isInProgress = false;
                this.isEnd = true;
                bw.Close();
                this.mTimer.Stop();

                TimerIsRunning = false ;
                this.OnTimeout(this);
            }
        }
        
        
        public ReceiveFileProtocol OnInProgressWrite(byte[] buffer, int len,_OnReceiveFileComplete handler )
        {
            if (!this.isInProgress) return this;
            try
            {
                bool x= (this.written+len)<this.file_size;
                if(len<1024 && x)
                {
                    mClient.SendMsg("resend");
                    return this;
                }

                bw.Write(buffer, 0, len);

                this.written += len;
                mClient.SendMsg("ok");
                if(this.written>=this.file_size)
                {
                    bw.Close();
                    this.isInProgress = false;
                    this.isEnd = true;
                    this.written = 0;
                    this.mTimer.Stop();
                    handler(this);
                    mClient.SendMsg("ok");
                    return this;
                }
               
                

                return this;
            }
            catch(Exception ex)
            {
                OnError(ex.Message, "OnInProgressWrite");
                return this;
            }

        }
        public bool isReceivedSuccess()
        {
            var info = new FileInfo(file_name);
            return info.Length == file_size;
        }
        public ReceiveFileProtocol OnReceiveFileEnd()
        {

            this.isEnd = true;
            this.isInProgress = false;
            try
            {
                bw.Close();
                if (mTimer != null)
                {
                    TimerIsRunning = false;
                    mTimer.Stop();
                    mTimer.Close();
                }
            }
            catch(Exception ex)
            {
                OnError(ex.Message, "OnReceiveFileEnd");
            }

            return this;
        }

    }

    public class ClientCollection
    {
        private List<Client> ClientList;

        public delegate void _foreach(Client c);
        public ClientCollection()
        {
            ClientList = new List<Client>();
        }
        public ClientCollection Add(Client client)
        {
            if (this.IsExist(client)) return this;

            ClientList.Add(client);
            return this;
        }
        public Client getClientByDeviceCode(string dev_code)
        {
            int len = ClientList.Count;
            int i = 0;
            for (i = 0; i < len; i++)
            {
                if (ClientList[i].DeviceCode == dev_code)
                {

                    return ClientList[i];
                }
            }
            return null;
        }
        public Client getClientByDevId(int dev_id)
        {
           
            int len = ClientList.Count;
            int i = 0;
            for(i=0;i<len;i++)
            {
                if(ClientList[i].DeviceId==dev_id)
                {
                   
                    return ClientList[i]; 
                }
            }
            return null;
        }
        public void ForEach(_foreach for_each)
        {
            int len = this.ClientList.Count;
            for (int i = 0; i < len; for_each(this.ClientList[i]), i++) ;
        }

        public Client getClientById(string id)
        {

            return this.ClientList.Find(c => c.ID == id);
        }
        public ClientCollection RemoveClientById(string id)
        {
            if (!this.IsExist(id)) return this;

            this.ClientList.Remove(getClientById(id));

            return this;
        }
        public int length
        {
            get { return ClientList.Count; }
        }
        public void Clear()
        {
            ClientList.Clear();
        }
        public Client this[string id]
        {
            get
            {
                if (this.IsExist(id))
                {
                    return this.getClientById(id);
                }
                return null;

            }
        }

        public bool IsExist(string id)
        {
            return this.ClientList.Exists(c => c.ID == id);

        }

        public bool IsExist(Client client)
        {
            return ClientList.Exists(c => c.ID == client.ID);
        }
    }
    public class Client
    {
        public Socket ClientSocket;
        public object TheNode;
        private int buffer_size = 1024;
        private int rx_bytes_count;
        public string ID;
        private bool is_connected;
        public int UiId;
        public int DeviceId;
        public string DeviceCode;
        public ReceiveFileProtocol recv_file_proto;
        public HandshakeProtocol Flags;
        public delegate void _OnCommandHandler(Client client, byte[] buffer, int len);
        public delegate void OnMessageArrived(string from, byte[] buffer, int len);
        public delegate void _OnTimerElapsed(Client c);
        public _OnTimerElapsed TimerRoutine;
        public _OnTimerElapsed OnReachMaxTry;
        public event _OnCommandHandler CommandHandler;
        public OnMessageArrived MessageHandler;
        public _OnCommandHandler OnHttpRequest;
        public Timer send_cmd_again_timer;
        public int send_cmd_interval=1*1000;//30 second
        public int max_try_count=3;
        public int try_count;
        public bool TimerIsRunning;
        public ClientType ClientType;
        public delegate void _OnDisconnected(string c);
        public string localport;
        public _OnDisconnected OnDisConnected;
        public EndPoint mEndPoint;
        public Client(string id, Socket c)
        {
            this.ClientSocket = c;
            this.ID = id;
            this.is_connected = true;
            rx_bytes_count = 0;
            this.Flags = new HandshakeProtocol();
            
            try_count = 0;
            send_cmd_again_timer = new Timer(this.send_cmd_interval);
            send_cmd_again_timer.Elapsed += _TimerRoutine;
            TimerIsRunning = false;
            this.ClientType = new ClientType();
            this.ClientType = ClientType.Sensor;
        }
        public Client(Socket s)
        {
            this.ClientSocket = s;
            this.mEndPoint = s.RemoteEndPoint;
            this.ID = this.ClientSocket.RemoteEndPoint.ToString();
            this.localport = this.ID.Split(':')[1];
            this.is_connected = true;
            rx_bytes_count = 0;

            this.Flags = new HandshakeProtocol();
            try_count = 0;
            //send_cmd_again_timer = new Timer(this.send_cmd_interval);
            //send_cmd_again_timer.Elapsed += _TimerRoutine;
            TimerIsRunning = false;
            this.ClientType = new ClientType();
            this.ClientType = ClientType.Sensor;
        }
        // modified disable timers.
        public Client(string ip,int port=20001)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            this.ClientSocket = Connect(socket, new IPAddress[] { IPAddress.Parse(ip) }, port).Result;

            this.mEndPoint = this.ClientSocket.RemoteEndPoint;


            this.is_connected = true;
            rx_bytes_count = 0;

            this.Flags = new HandshakeProtocol();
            try_count = 0;
            //send_cmd_again_timer = new Timer(this.send_cmd_interval);
            //send_cmd_again_timer.Elapsed += _TimerRoutine;
            TimerIsRunning = false;
            this.ClientType = new ClientType();
            this.ClientType = ClientType.Sensor;
            
        }

        public Client(int port , string id)
        {
            this.ID = id;
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            this.ClientSocket = Connect(socket, new IPAddress[] { IPAddress.Loopback }, port).Result;

            this.localport = this.ClientSocket.RemoteEndPoint.ToString().Split(':')[1];
            this.mEndPoint = this.ClientSocket.RemoteEndPoint;
            this.is_connected = true;
            rx_bytes_count = 0;

            this.Flags = new HandshakeProtocol();
            try_count = 0;
            //send_cmd_again_timer = new Timer(this.send_cmd_interval);
            //send_cmd_again_timer.Elapsed += _TimerRoutine;
            TimerIsRunning = false;
            this.ClientType = new ClientType();
            this.ClientType = ClientType.Sensor;
        }
        private async Task<Socket> Connect(Socket sc,IPAddress[] address,int port)
        {
            try
            {
                await Task.Factory.FromAsync(
                    new Func<IPAddress[], int, AsyncCallback, object, IAsyncResult>(sc.BeginConnect),
                    new Action<IAsyncResult>(sc.EndConnect), address, port, null
                    ).ConfigureAwait(false);
                return sc;
            }
            catch(Exception)
            {
                return null;
            }
        }
        private void _TimerRoutine(object sender,EventArgs args)
        {
            this.try_count = this.try_count + 1;
            if(this.try_count>=this.max_try_count)
            {
                StopTimer();
                this.try_count = 0;
                TimerIsRunning = false;
                this.OnReachMaxTry(this);
            }
            if(this.TimerRoutine!=null)
            {
                TimerIsRunning = true;
                this.TimerRoutine(this);
            }
        }
        
        public void StopTimer()
        {
            if(this.send_cmd_again_timer!=null)
            {
                this.send_cmd_again_timer.Stop();
                this.TimerIsRunning = false;
                this.try_count = 0;
            }
        }
        public void startTimer()
        {
            if (this.send_cmd_again_timer != null)
            {
                this.send_cmd_again_timer.Start();
                this.TimerIsRunning = true;
                this.try_count = 0;
            }

        }
        public int get_rx_bytes()
        {
             return this.rx_bytes_count;
        }
        public Client updateBytesCount(int len)
        {
            this.rx_bytes_count += len;
            return this;
        }
        public Client reset_rx_bytes()
        {
            this.rx_bytes_count = 0;
            return this;
        }
        public string getHostName()
        {
            //string hostname = "";

            //this.ClientSocket.RemoteEndPoint.Serialize().
            //Dns.
            string[] ip = this.ID.Split(':');

            return Dns.GetHostByAddress(ip[0]).HostName;
        }
        public void Start(OnMessageArrived handler, _OnDisconnected dishandler)
        {
            this.MessageHandler = handler;
            this.is_connected = true;
            this.OnDisConnected = dishandler;

            Task.Run(() => OnNewMsg(handler, dishandler));
        }
        public NetworkStream Stream
        {
            get
            {
                if (this.IsConnected)
                {
                    return new NetworkStream(this.ClientSocket);
                }
                return null;
            }
        }
        public void Close()
        {
            //this.Stream.Close();
            if (this.IsConnected)
            {
                this.Stream.Close(3);

                this.ClientSocket.Close(3);
            }
        }

        public bool close(IAsyncResult res)
        {

            if (this.IsConnected && res.IsCompleted)
            {
                this.ClientSocket.Close();
                return true;
            }

            return false;
        }
    

        public void ControlCmdReceived()
        {
            this.Flags.control_cmd_sent = false;
            this.Flags.sent_cmd = "";
        }
        public bool SendLastCmd()
        {
            return this.SendCommand(this.Flags.sent_cmd);
        }
        public  bool SendCommand(string cmd)
        {
           

            try
            {
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(cmd);
                int len = bytes.Length;
                if(this.ClientSocket!=null)
                {
                    var st = new NetworkStream(this.ClientSocket);
                     
                    st.Write(bytes, 0, len);
                    this.Flags.control_cmd_sent = true;
                    this.Flags.sent_cmd = cmd;
                }
                return true;
            } 
            catch(Exception)
            {
                
                return false;
            }


           
        }
        

    
    public void SendFile(string _path,Action<string> msg_out)
        {
            if(!File.Exists(_path))  {msg_out(string.Format("File {0} is Not Exist",_path)); return;}
            
        try
        {
            //StreamReader reader=new StreamReader(_path);
            this.ClientSocket.SendFile(_path);
            

        }
        catch (Exception ex)
        {
            msg_out("Error in Send file function "+ex.Message);
        }
        finally
        {
            msg_out(string.Format("File {0} Sent successfully!.", _path));
        }
            

        }


    delegate void _DoJob(string fn);
    public IAsyncResult SendAsync(string fn,AsyncCallback _callback,object obj)
    {
        if (!File.Exists(fn)) return null;
       
        _DoJob job = ((s) =>
            {
                try
                {
                    this.ClientSocket.SendFile(s);
                   
                }
                catch (Exception)
                {
                    return;
                }




            });


        return job.BeginInvoke(fn, _callback, obj);

        
        
        

    }
    public void SendFile(string fn,Action<string> onException,Action<Client> Final)
    {
        if (!File.Exists(fn)) return;
        try
        {
            this.ClientSocket.SendFile(fn);
        }
        catch(Exception ex)
        {
            onException("SendAsync:" + ex.StackTrace);
        }
        finally
        {
            Final(this);
        }
    }
   public void Send(byte[] bytes)
    {
       try
       {
           
               this.ClientSocket.Send(bytes);
           
       }
       catch(Exception)
       {
           //if(this.MessageHandler!=null)
           //MessageHandler(this.ID, tobytes("error-->+" + ex.StackTrace), 0);
           return;
       }
           
    }
        public void SendMsg(string msg)
        {
            Socket mClient = this.ClientSocket;

            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(msg);
                this.Send(bytes);
            }

            catch (Exception)
            {
                return;
            }
               
            
                

            
        }
        private async Task OnNewMsg(OnMessageArrived handler, _OnDisconnected dhandler)
        {

            var stream = new NetworkStream(this.ClientSocket);
            // string s = string.Format("Msg From {0} {1} \t\n ", this.ID,DateTime.Now.ToString("hh:mm"));
            var buffer = new byte[this.buffer_size];
            do
            {
               // stream.A

                
                int len = buffer.Length;

                int byteRead = await stream.ReadAsync(buffer, 0, len).ConfigureAwait(false);
                if (byteRead == 0)
                {
                    // handler(this.ID, tobytes(" is Disconnected"), 0);
                    dhandler(this.ID);
                    this.is_connected = false;
                    break;
                }

                if (byteRead > 0) 
                {
                    //string msg = Encoding.ASCII.GetString(buffer, 0, byteRead).Trim();
                    //bool is_http = msg.StartsWith("GET") || msg.StartsWith("POST");
                    //is_http &= msg.Contains("HTTP");
                    //if(is_http)
                    //{
                    //    this.ClientType = ClientType.http;
                    //    this.OnHttpRequest(this, buffer, byteRead);
                    //   
                    //}
                    if(this.Flags.control_cmd_sent)
                    {
                        this.CommandHandler(this, buffer, byteRead);
                       
                    }
                    else
                    {
                        handler(this.ID, buffer, byteRead);
                        
                    }
                    //byteRead = -1;
                        
                }


            } while (true);
        }

        private byte[] tobytes(string msg)
        {
            return ASCIIEncoding.ASCII.GetBytes(msg);
        }

        public string HostName
        {
            get
            {
                if (this.IsConnected)
                {
                    return this.ClientSocket.RemoteEndPoint.ToString();
                }
                return null;
            }
        }

        public bool IsConnected
        {
            get
            {
                if (this.ClientSocket.Connected && this.ClientSocket != null && is_connected) return true;
                return false;
            }

            set
            {
                is_connected = value;
            }
        }


    }
    public class tcpServer
    {

        public delegate void EventHandler(string msg);
        public delegate void OnRecv(string from, byte[] buffer, int len);
        public delegate void _OnNewClient(string client);

        public _OnNewClient OnNewClient;
        public ClientCollection myClients;
        public delegate void _OnClientDisConnect(string id);

        public event OnRecv OnMessageArriveEvent;

        public _OnClientDisConnect OnClientDisConnect;
        // public delegate void _OnConnection(Socket s);
        //public On
        public OnRecv mOnRecv;
        public EventHandler OnMsg;
        public EventHandler OnError;
        public Socket myServerSocket;
        public string server_id;
        public void Start(string ip, int port = 8090)
        {
            //var endpoint = new IPEndPoint(IPAddress.Loopback, port); //test
           var endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            myClients = new ClientCollection();
            socket.Bind(endpoint);
            socket.Listen(int.MaxValue-1);//128
            myServerSocket = socket;
           
            //this.mClient = null;
            Task.Run(() => Listen(socket));
        }
        public void StartLoopback(int port ,  string server_id)
        {
            this.server_id = server_id;
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            myClients = new ClientCollection();
            socket.Bind(endpoint);
            socket.Listen(int.MaxValue - 1);//128
            myServerSocket = socket;
            //this.mClient = null;
            Task.Run(() => Listen(myServerSocket));
        }
        public void Close()
        {
            try
            {
                myServerSocket.Close();
            }
            catch(Exception ex)
            {
                OnError(string.Format("Error @ Close Socket:{0}",ex.Message));
            }
        }
        public void SendMsgTo(string msg , string c_id)
        {
            try
            {
                if (this.myClients.length == 0) return;
                  byte[] buffer = Encoding.ASCII.GetBytes(msg);
                Client c = this.myClients[c_id];
                NetworkStream stream = new NetworkStream(c.ClientSocket);
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SendMsgTo(string msg , EndPoint ep)
        {
            if (!this.myServerSocket.Connected)
            {
               // Exception ex = new Exception("MyServer is not Connected");

               // throw ex;
              
            }
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(msg);
                this.myServerSocket.SendTo(buffer, ep);
                
            }
            catch(Exception ex )
            {
                throw ex;
            }
        }
        public void SendMsg(string msg)
        {
            if(!this.myServerSocket.Connected)
            {
                Exception ex = new Exception("MyServer is not Connected");
              
                throw ex;
            }
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(msg);
                this.myServerSocket.Send(buffer);
                
               
            }catch(Exception)
            {
                return;
            }
        }
        private async Task Listen(Socket msocket)
        {
            do
            {

                var client = await Task.Factory.FromAsync(
                new Func<AsyncCallback, object, IAsyncResult>(msocket.BeginAccept),
                new Func<IAsyncResult, Socket>(msocket.EndAccept), null).ConfigureAwait(false);
               
               // client.r

                Client mc = new Client(client);
                mc.Start((m, buffer, len) =>
                {
                    //OnMessageArriveEvent(m, buffer, len);
                    this.mOnRecv(m, buffer, len);
                }, (id) =>
                {
                    //string m = string.Format("\n{0} is DisConnected!\n", disconnect);
                    this.OnClientDisConnect(id);
                   // myClients.RemoveClientById(id);


                    //OnMsg(m);
                });
                myClients.Add(mc);

                // OnMsg("Client Connected" + client.RemoteEndPoint.ToString());
                OnNewClient(client.RemoteEndPoint.ToString());


            } while (true);

        }

    }

    
}
