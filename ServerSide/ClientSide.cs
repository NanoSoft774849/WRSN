using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace ns.networking
{
    public class ns_tcpClient
    {


        public string ip_address;
        public int port_address;
        public Socket ClientSocket;
        public delegate void _OnReceive(byte[] buffer, int len);
        public delegate void _EventHandler(mEventHandler mhandler);
        public _EventHandler MyEventHandler;
        public _OnReceive OnMessageReceived;

        public ns_tcpClient()
        {

        }

        public void Start(string ip, int port = 2234)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
           
           // socket.ConnectAsync()
            //socket.co
            //socket.Co
           
            Task.Run(() => Connect(socket,endpoint));
        }

        private async Task Connect(Socket mscoket, IPEndPoint remote)
        {
            mEventHandler mhandler = new mEventHandler();
            mhandler.Where = "Connect function";
            try
            {

                mscoket.Connect(remote);

                mhandler.Message = "Connection Succeed!.";

                MyEventHandler(mhandler);

                await ReadAsync(mscoket);
            }
            catch (System.ArgumentException arg_exp)
            {
                mhandler.Message = arg_exp.Message;

                MyEventHandler(mhandler);
                return;
            }
            catch (SocketException socket_exp)
            {
                mhandler.Message = string.Format("Socket Exception:{0}", socket_exp.Message);
                MyEventHandler(mhandler);
                return;
            }
            catch(ObjectDisposedException obj_exp)
            {
                mhandler.Message = string.Format("ObjectDisposedException:{0}", obj_exp.Message);
                MyEventHandler(mhandler);
                return;
            }
            catch (InvalidOperationException ex)
            {
                mhandler.Message = string.Format("ObjectDisposedException:{0}", ex.Message);
                MyEventHandler(mhandler);
                return;
            }
            finally
            {
                
                
            }

        }
        private async Task ReadAsync(Socket msocket)
        {

            var stream = new NetworkStream(msocket);
            mEventHandler mhandler = new mEventHandler();
            mhandler.Where = "ReadAsync";
            // string s = string.Format("Msg From {0} {1} \t\n ", this.ID,DateTime.Now.ToString("hh:mm"));
            var buffer = new byte[1024];
            do
            {


                int byteRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                if (byteRead == 0)
                {
                    // handler(this.ID, tobytes(" is Disconnected"), 0);
                   // dhandler(this.ID);
                    //this.is_connected = false;
                    mhandler.Message = string.Format("Connection Closed!:@{0}",DateTime.Now.ToString("dd-hh:mm:ss"));
                    MyEventHandler(mhandler);
                    break;
                }

                if (byteRead > 0)
                {
                    OnMessageReceived(buffer, byteRead);
                   
                }


            } while (true);
        }   

        
    }
}
