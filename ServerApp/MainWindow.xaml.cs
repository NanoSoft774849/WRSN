using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using ns.networking;
using System.IO;
using Path = System.Windows.Shapes.Path;
using NodeInfo;
namespace ServerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel  _model = new MainViewModel();
        List<NodesInfo> myNodes;
        private tcpServer myServer;
        private static int node_id_counter ;
        private List<NodeGui> MyNetwork;
        private ClientCollection MyClients;
        private List<ns_Timers> MyTimers;
        public string ip_address { get; set; }
        
        public bool Start_button_state { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            node_id_counter = 1001;
            mcontentx.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 200;
            myNodes = new List<NodesInfo>();
            this.Start_button_state = true;
            MyNetwork = new List<NodeGui>();
            MyTimers = new List<ns_Timers>();
           // this.port_address = "8090";
            

            _model.NodesInfox = new ObservableCollection<NodesInfo>(myNodes);

            DataContext = _model;

           
            


           get_interfaces();

           double  aw = mcanvas.ActualWidth;
           double  ah = mcanvas.ActualHeight;

           double w = mcanvas.Width;
           double h = mcanvas.Height;
           double dh = mcanvas.DesiredSize.Height;
           double dw = mcanvas.DesiredSize.Width;
           string f = string.Format("dh:{0} , dw:{1} , w:{2} , h:{3}", dh, dw, w, h);
           logError(f);

           

           
        }
       
        private void CreateNode(NodesInfo info)
        {
            if (info == null) return;
            Path mnode = CreateEllipse(info.NodeId, info.X, info.Y, 10, 10);
            mnode.Fill = info.ClientType == "car" ? Brushes.Coral : Brushes.DarkCyan;
            mnode.Uid = string.Format("{0}",info.UiId);
            ContextMenu cmenu = new ContextMenu();
            cmenu.Items.Add(string.Format("Node ID : \t {0}", info.NodeId));
            cmenu.Items.Add(string.Format("Node X : \t {0}", info.X));
            cmenu.Items.Add(string.Format("Node Y : \t {0}", info.Y));
            cmenu.Items.Add(string.Format("Node Power : \t {0}", info.nodepwr));
            cmenu.Items.Add(string.Format("Node Type:\t {0} ", info.ClientType));
            cmenu.Items.Add(string.Format("Client : {0}", info.client));
            mnode.ContextMenu = cmenu;
            Label mtext = new Label();
            mtext.Margin = new Thickness(info.X - 5, info.Y - 25, 5, 0);
            mtext.Foreground = Brushes.Black;
            mtext.Content = info.NodeId;
            mtext.FontSize = 12.0;


            NodeGui node = new NodeGui() { node = mnode, text = mtext };
            MyNetwork.Add(node);

            this.Dispatcher.Invoke(() =>
                {
                    this.mcanvas.Children.Add(mnode);
                    this.mcanvas.Children.Add(mtext);

                });


        }

        private void Button_Stop_Sim(object sender , EventArgs args)
        {
            if (MyTimers.Count == 0)
            {
                logError("No Timers Created");
                return;
            }
            foreach( ns_Timers timer in MyTimers)
            {
                if( timer.State == ns_Timers.TimerState.Running)
                timer.stopTimer();
            }
        }
        private void Button_Start_Sim(object sender , EventArgs args)
        {
           if(MyTimers.Count == 0)
           {
               logError("No Timers Created");
               return;
           }
           foreach (ns_Timers timer in MyTimers)
           {
               if (timer.State == ns_Timers.TimerState.Created)
                   timer.StartTimer();
           }
        }
        public Path CreateEllipse(string node_id, double x, double y, double r1, double r2)
        {

            Path myPath = new Path();
            myPath.Stroke = System.Windows.Media.Brushes.Black;
            myPath.Fill = System.Windows.Media.Brushes.MediumSlateBlue;
            myPath.StrokeThickness = 0.66;
            //myPath.HorizontalAlignment = HorizontalAlignment.Left;
            //myPath.VerticalAlignment = VerticalAlignment.Center;
            //myPath.
           // myPath.Name = node_id;
            EllipseGeometry myEllipseGeometry = new EllipseGeometry();
            myEllipseGeometry.Center = new System.Windows.Point(x, y);
            myEllipseGeometry.RadiusX = r1;
            myEllipseGeometry.RadiusY = r2;
            myPath.Data = myEllipseGeometry;
            

            myPath.ToolTip = node_id;
            

            return myPath;

        }
        private void updateNodeCounter()
        {
            node_id_counter = node_id_counter + 1;
        }
        #region Server Handlers 

       
        private void Start(string ip, int port)
        {

            myServer = new tcpServer();
           
            try
            {

                myServer.Start(ip, port);


                this.myServer.OnNewClient = OnNewClientConnected;
                this.myServer.OnClientDisConnect = OnClientDisConnect;
                this.myServer.mOnRecv = ServerOnReceiveMsg;
               // this.myServer.OnMessageArriveEvent += ServerOnReceiveMsg;
                this.myServer.OnMsg = ServerOnInfoMsg;
                this.myServer.OnError = ServerOnErrorMsg;
                this.MyClients = this.myServer.myClients;


            }
            catch (Exception ex)
            {
                logError(ex.Message + ":ServerStart");
            }

            logError("App Start.........");

        }
        #region Server Events and Handlers
        private void ServerOnErrorMsg(string msg)
        {
            logError(msg);
        }
        private double calcEucludianDistance( double nx, double ny , double cx ,double cy)
        {
            double x = (nx - cx) * (nx - cx);
            double y = (ny - cy) * (ny - cy);
            return Math.Sqrt(x + y);
        }
        /// <summary>
        /// this simple algorithm based on distance .
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodesInfo SelectNearestCarfrom(NodesInfo node)
        {
            NodesInfo car = null;
            var nodes = _model.NodesInfox;
            List<NodesInfo> cars = new List<NodesInfo>();
            foreach( var c in nodes)
            {
                
                if( c.ClientType=="car" && c.State == NodesInfo.NodeState.availabe)
                {
                    cars.Add(c);
                }
            }
            double nx = node.X;
            double ny = node.Y;
            int min_index = 0;

            
            int i=0;
            double min =0;
            for (i = 0; i < cars.Count; i++)
            {
                var c = cars[i];
                double cx = c.X;
                double cy = c.Y;
                if( i ==0)
                {
                    min = calcEucludianDistance( cx, cy ,nx,ny);
                }
                double dist = calcEucludianDistance(cx, cy, nx, ny);
                min = min <= dist ? min : dist;
                min_index = min == dist ? i : min_index;
            }


            car = cars[min_index];
            if( car == null ) { logError("Car is Null"); return null;}
            logError(string.Format("MinIndex:{0} , carId:{1}", min_index, car.NodeId));


                

           

            return car;
        }
        private void OnChargeCommand(string client_id)
        {
            var _node = getNodeInfoByClientId(client_id);
            if (_node == null) { logError("can't find node Node is NULL"); return; }

            var nearest_car = SelectNearestCarfrom(_node);
            var n = MyNetwork.First((c) =>
            {


                return c.node.Uid == string.Format("{0}", _node.UiId);
            });
            var ncar = MyNetwork.First((c) =>
            {
                return c.node.Uid == string.Format("{0}", nearest_car.UiId);
            });
            int i = mcanvas.Children.IndexOf(n.node);
            Path m = (Path)mcanvas.Children[i];
            i = mcanvas.Children.IndexOf(ncar.node);
            Path cc = (Path)mcanvas.Children[i];
            if (n == null) { logError("can't find node"); return; }


            //logError(string.Format("Path index:{0}", i));
            if (i == -1) { logError("can't find node index -1"); return; }

            if (m == null) { logError("can't find node Path is null"); return; }

            this.Dispatcher.Invoke(() =>
            {
                _node.Status = "need charge";   
                this.mdata_grid.Items.Refresh();
                // mcanvas.Background = Brushes.Red;
                
                
                
                m.Fill = Brushes.Green;
               
                
                cc.Fill = Brushes.Magenta;
               


            });
            nearest_car.State = NodesInfo.NodeState.moving;
            //Save original location
            double temp_x = nearest_car.X;
            double temp_y = nearest_car.Y;
            ns_Physics.MoveToPoint(nearest_car, _node, (s) =>
            {
                logError(s);
            }, (res) =>
            {
                this.Dispatcher.Invoke(() =>
                    {
                        
                        ncar.SetLocation(res.x, res.y);
                        
                        if( res.State ==ns_Point.PointState.end)
                        {
                            logError("Here is the End");
                            nearest_car.X = res.x;
                            nearest_car.Y = res.y;
                            Goback(nearest_car, temp_x, temp_y, ncar);
                        }
                    });
               
            });
            // Car will go back to its own location 
            
        }
        private void Goback(NodesInfo car , double old_x, double old_y , NodeGui ncar)
        {
            ns_Physics.MoveToPoint(car, old_x, old_y, (s) => { logError(s); }, (res) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if( res.x<=0 || res.y<=0)
                    {
                        logError("Res is less or " + string.Format(">={0}", res.x));
                        return;
                    }
                    ncar.SetLocation(res.x, res.y);
                });
            });
        }
        private void updateClientSatus( string client_id , string status )
        {
            var _node = getNodeInfoByClientId(client_id);
            
            if (_node == null) { logError("can't find node Node is NULL"); return; }
            this.Dispatcher.Invoke(() =>
            {
                _node.Status = status;
                this.mdata_grid.Items.Refresh();
               // mcanvas.Background = Brushes.Red;
                var n = MyNetwork.First((c) =>
                {


                    return c.node.Uid == string.Format("{0}", _node.UiId);
                });
                if (n == null) { logError("can't find node"); return; }

                int i = mcanvas.Children.IndexOf(n.node);
                logError(string.Format("Path index:{0}", i));
                if (i == -1) { logError("can't find node index -1"); return; }
                Path m = (Path)mcanvas.Children[i];
                if (m == null) { logError("can't find node Path is null"); return; }
                m.Fill = Brushes.Red;
               

            });
            
            
            
           
        }
        private void updateUI( string client_id , NodesInfo info)
        {
            var _node = getNodeInfoByClientId(client_id);
            if (_node == null) return;
            this.Dispatcher.Invoke(() =>
                {
                    _node.NodeId = info.NodeId;
                    _node.nodepwr = info.nodepwr;
                    _node.X = info.X;
                    _node.Y = info.Y;
                    _node.ClientType = info.ClientType;
                    this.mdata_grid.Items.Refresh();
                });

        }
        private NodesInfo getNodeInfoByClientId(string client)
        {
            if (string .IsNullOrEmpty( client)  || this.MyClients[client]==null)
            {
                logError("client is not exist!.");
                return null;
            };
            NodesInfo _node = null;
            int _ui_id = this.MyClients[client].UiId;

            var nodes = _model.NodesInfox;

           
            _node = nodes.First((item) =>
                {
                    return item.UiId == _ui_id;
                     
                });
            



            return _node;
        }
        private void ServerOnInfoMsg(string msg)
        {
            logError(msg);
        }
        private void SendDeviceInfo(string from)
        {
            string file = App.GetDeviceInfoFile();
           string info =  System.IO.File.ReadAllText(file);
            if(this.MyClients[from]!=null)
            {
                this.MyClients[from].SendMsg(info);
                logMessage(info);
            }
        }
        private void CommandHandler(string from,string msg)
        {
            msg = msg.Trim();

            string _debug_msg = string.Format("{0}:{1}" + Environment.NewLine, from, msg);
            logMessage(_debug_msg);
            if(msg =="send device info")
            {
                SendDeviceInfo(from);
                return;
            }
            if(msg =="snapshot")
            {
                if(this.MyClients[from]!=null)
                {
                    this.MyClients[from].SendMsg("OK");
                    System.Threading.Thread.Sleep(1000);
                }

                OnSnapShotCmd(from);
                return;
            }

            if(msg.StartsWith("alarm") || msg.StartsWith("time"))
            {
                if(this.MyClients[from]!=null)
                {
                    this.MyClients[from].SendMsg("OK");
                }
                return;
            }
            if(msg=="done")
            {
                if(this.MyClients[from]!=null)
                {
                    this.MyClients[from].SendMsg("mcu:shutdown");
                }
            }

        }

        private void OnSnapShotCmd(string from)
        {
            string data_path = App.GetDataPath();
            string[] files = Directory.GetFiles(data_path);
            Random r = new Random(DateTime.Now.Millisecond);
            int len = files.Length;
            int index = r.Next(len);
            printf("file to be sent:{0}", files[index]);
            FileInfo info = new FileInfo(files[index]);
           // finfo_start='{{"file_ext":"{0}","cmd":"open","file_size":"{1}"}}'.format(file_ext,sizex)
            string ext = info.Extension;
            if (ext.Contains(".")) ext = ext.Replace(".","");
            string info_str = jsonfmt("file_ext", ext, "cmd", "open", "file_size", info.Length);
            printf("fmt:{0}", info_str);
            if(this.MyClients[from]!=null)
            {
                this.MyClients[from].SendMsg(info_str);
                System.Threading.Thread.Sleep(1000);
                this.MyClients[from].SendFile(files[index], (s) =>
                    {
                        printf("{0}", s);
                    });
                //finfo_end='{{"file_ext":"{0}","cmd":"close","written":"{1}"}}'.format(file_ext,num_bytes_written)
                info_str = jsonfmt("file_ext", ext, "cmd", "close", "written", info.Length);
                this.MyClients[from].SendMsg(info_str);
                printf("Done sending info:{0}", "OK");
            }

        }
        private string jsonfmt(params object[] args)
        {
            string fmt = "";
            int len = args.Length;
            int i = 0;
            for (i = 0; i < len; i++)
            {
                if (i % 2 == 0)
                {
                    fmt += string.Format("\"{0}\":", args[i]);
                    continue;
                }
                fmt += string.Format("\"{0}\"", args[i]);
                if (i != len - 1) fmt += ",";
            }
            return "{" + fmt + "}";
        }
        private void printf(string fmt, params object[] args)
        {
            string msg = string.Format(fmt, args) + Environment.NewLine;
            logMessage(msg);
        }
        private void ServerOnReceiveMsg(string from, byte[] buffer, int len)
        {

            string msg = ASCIIEncoding.ASCII.GetString(buffer, 0, len);
               if(len>0)
               {
                   CommandHandler(from, msg);
                   return;
               }
            string _debug_msg = string.Format("{0}:{1}" + Environment.NewLine, from, msg);
            logMessage(_debug_msg);
            NodesInfo _node = ns_app.decodeJsonMsg(msg, (s) => { /*logMessage(s);*/ });
            this.Dispatcher.Invoke(() =>
                {
                    
                    if( _node != null )
                    {
                        updateUI(from, _node);
                        CreateNode(_node);
                        return;
                    }
                    if( msg.Trim() == "charge")
                    {
                        OnChargeCommand(from);
                    }
                    

                });
           
        }

        private void OnClientDisConnect(string id)
        {
            
            updateClientSatus(id, "disconnected");
        }

        private void OnNewClientConnected(string client)
        {

            NodesInfo info = new NodesInfo(client, node_id_counter.ToString(), true, node_id_counter);
            MyClients[client].UiId = node_id_counter;
            this.Dispatcher.Invoke(() =>
                {
                    //myNodes.Add(info);
                    _model.NodesInfox.Add(info);
                    this.mdata_grid.Items.Refresh();
                });
            updateNodeCounter();
             MyClients[client].SendMsg("send device info");
            
        }

        #endregion 
        #endregion
        private string port_address
        {
            get
            {
                string port = this.txtbox_port.Text;
                //if( !System.Text.RegularExpressions.Regex.IsMatch(port,@"{\d+}"))
                //{
                //    return "20001";
                //}
                return port; 
            }
            set
            {
               this.txtbox_port.Text=value;// this.port
            }
        }

        #region Controller UI Events;
        private void Button_Start_Controller(object sender , EventArgs args)
        {
            try 
            {

            string  ip=  (string) iface.SelectedItem;
            if( string.IsNullOrEmpty(ip) || ip == null)
            {
                logError("No iterface selected. please select iterface then click Start Controller Button");
                return;
            }
            this.ip_address = ip;

            logError(string.Format("ip adress :{0} , port address :{1}", this.ip_address, this.port_address));

            string _port = this.port_address;
            int port = int.Parse(_port);
            port = port >= 65000 ? 20001 : port;

            Start(ip, port);

            }
            catch (Exception ex)
            {
                logError(ex.Message);
            }
          

        }
        private void Button_Stop_Controller(object sender , EventArgs args)
        {

        }

        private void Button_Click(object sender , EventArgs args)
        {
            Button bt = (Button)sender;

            string uid = bt.Uid;
            int index = mdata_grid.SelectedIndex;
            var _rec = _model.NodesInfox;
            try
            {
                NodesInfo info = _rec[index];
                if (info == null) return;
                string id = info.client;
                Client c = MyClients[id];
                if (c == null) return;
                c.SendMsg("Hello");
            }
            catch (Exception ex)
            {
                logError(ex.Message);
            }


        }
        #endregion

        #region some useful utilities
        // Get All interfaces and put them in the iface combox

        private void logError(string msg)
        {
           // MessageBox.Show(msg);
            logMessage(msg);
        }
        private void logMessage(string msg)
        {
            //MessageBox.Show(msg);
            this.Dispatcher.Invoke(() =>
            {
                debug_screen.Text += msg + Environment.NewLine;
            });

        }

        private string[] Ifaces
        {
            get
            {
                return NsDns.GetLocalHostInterfaces();
            }
            set
            {
                foreach( string s in value)
                {
                    this.iface.Items.Add(s);

                }
                this.iface.Items.Refresh();
            }
        }
        private void get_interfaces()
        {
            try
            {
                string[] ifaces = NsDns.GetLocalHostInterfaces();
                if (ifaces.Length == 0)
                {
                    logError("No interface is exist");
                    return;
                }
                foreach (string s in ifaces)
                {
                    iface.Items.Add(s);
                   
                }
                iface.Items.Refresh();
               // logError(string.Format("ifaces:{0}", ifaces));
            }
            catch(Exception ex)
            {
                logError(ex.Message);
            }
           
        }
        #endregion

    }

     public class NodeGui
        {
            public Path node;
            public Label text;

            public void SetLocation( double x , double y)
            {
                this.text.Margin = new Thickness(x-5, y-25, 5, 0);
                // mtext.Margin = new Thickness(info.X - 5, info.Y - 25, 5, 0);
                EllipseGeometry myEllipseGeometry = new EllipseGeometry();
                myEllipseGeometry.Center = new System.Windows.Point(x,y);
                myEllipseGeometry.RadiusX = 10.0;
                myEllipseGeometry.RadiusY = 10.0;
                this.node.Data = myEllipseGeometry;
            }
        }
        
}
