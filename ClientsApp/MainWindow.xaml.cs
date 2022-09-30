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
using NodeInfo;
using ns.networking;
namespace ClientsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel _model = new MainViewModel();
        List<NodesInfo> myNodes;
        ClientCollection MyClients;
        private List<ns_Timers> MyTimers;
        public MainWindow()
        {

            InitializeComponent();
            mcontentx.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 200;
            myNodes = new List<NodesInfo>();
            MyClients = new ClientCollection();

           _model = new MainViewModel();
            MyTimers = new List<ns_Timers>();
           _model.NodesInfox = new ObservableCollection<NodesInfo>(myNodes);

            DataContext = _model;
        }
        private void logError(string msg)
        {
            logMessage(msg);
        }
        private void Button_Stop_Sim(object sender, EventArgs args)
        {
            if (MyTimers.Count == 0)
            {
                logError("No Timers Created");
                return;
            }
            foreach (ns_Timers timer in MyTimers)
            {
                if (timer.State == ns_Timers.TimerState.Running)
                    timer.stopTimer();
            }
        }
        private void Button_Start_Sim(object sender, EventArgs args)
        {
            if (MyTimers.Count == 0)
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
        private NodesInfo getNodeFromClientId(int id)
        {
            NodesInfo info = null;
            var nodes = _model.NodesInfox;

            info = nodes.First((n) =>
                {
                    return n.UiId == id;
                });

            return info;
        }
        private void Button_Click(object sender, EventArgs args)
        {
            Button bt = (Button)sender;

            

            string uid = bt.Uid;
            int index = mdata_grid.SelectedIndex;
            var _rec = _model.NodesInfox;
            try
            {
                NodesInfo info = _rec[index];
                if (info == null) return;
                string id = info.NodeId;
                Client c = MyClients[id];
                if (c == null) return;
                if( uid == "req_charge")
                {
                    c.SendMsg("charge");
                    return;
                }
                if( uid =="close")
                {
                    c.Close();
                    this.Dispatcher.Invoke(() =>
                        {
                            _model.NodesInfox.RemoveAt(index);

                        });
                    return;
                }
                if( uid =="info")
                {
                    return;
                }
                if(uid =="ping")
                {
                    c.SendMsg("hello");
                }
            }
            catch(Exception ex)
            {
                logMessage(ex.Message);
            }

        }
        private void logMessage(string msg)
        {
            //MessageBox.Show(msg);
            this.Dispatcher.Invoke(() =>
                {
                    debug_screen.Text += msg + Environment.NewLine;
                });
            
        }

        private int Number_of_cars
        {
            get
            {
                string _n = number_of_cars.Text;
                if (string.IsNullOrEmpty(_n))
                {
                    logMessage("Number of Cars is Null");
                    return 1;
                }
                try
                {
                    int n = int.Parse(_n);
                    return n;
                }
                catch (Exception ex)
                {
                    logMessage(ex.Message);
                    return 1;
                }

            }
            set
            {
                number_of_cars.Text = value.ToString();
            }
        }
        private int Number_of_nodes
        {
            get
            {
                string _n = number_of_nodes.Text;
                if (string.IsNullOrEmpty(_n))
                {
                    logMessage("Number of Nodes is Null");
                    return 1;
                }
                try
                {
                    int n = int.Parse(_n);
                    return n;
                }
                catch(Exception ex)
                {
                    logMessage(ex.Message);
                    return 1;
                }
                
            }
            set
            {
                number_of_nodes.Text = value.ToString();
            }
        }
       private void OnMessageHandler(string from , byte[] buffer, int len)
        {
            Client c = MyClients[from];
            string msg = Encoding.ASCII.GetString(buffer,0, len);

            string lmsg = string.Format("[{0}]:{1}", from, msg);

           if(msg.Trim() == "send device info")
           {
               try
               {
                   NodesInfo info = getNodeFromClientId(c.UiId);

                   string info_msg = ns_app.EncodeObject(info);
                   logMessage(info_msg);
                   c.SendMsg(info_msg);
               }
               catch (Exception ex)
               {
                   logMessage("Serialize error:"+ex.Message);
               }
               

           }
            


            this.Dispatcher.Invoke(() =>
                {
                    logMessage(lmsg);
                });
            
        }
        private void OnDisconnect(string id)
       {
           string msg = "server closed!";
           msg = string.Format("[{0}]:{1}", id, msg);
           this.Dispatcher.Invoke(() =>
           {
               logMessage(msg);
           });
       }
       //private void log_debug_msg()
        private void but_closeClients(object sender , EventArgs args)
        {
            var clients = this.MyClients;
            if( clients == null || clients.length==0)
            {
                logMessage(" NO Client");
                return;
            }
            clients.ForEach(c =>
                {

                    c.Close();
                    logMessage(string.Format("{0} is disconnected", c.ID));
                    

                });
            this.MyClients.Clear();
            this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        _model.NodesInfox.Clear();
                       // mdata_grid.Items.Clear();
                    }
                    catch (Exception ex)
                    {
                        logMessage(ex.Message);
                    }
                   
                    
                });
            if( MyTimers.Count!=0)
            {
                foreach(ns_Timers t in MyTimers)
                {
                    if(t.State!=ns_Timers.TimerState.Stopped)
                    {
                        t.stopTimer();
                    }
                }
                MyTimers.Clear();
            }
        }
        private void but_generate(object sender , EventArgs args)
        {
            int n_nodes = this.Number_of_nodes%255;
            int n_cars = this.Number_of_cars%255;

            string ip_port = this.ip_port.Text;
            if( string .IsNullOrEmpty( ip_port ))
            {
                logMessage(" ip:port Text box is empty");
                return;
            }
            ns_ip_port xip_port = ns_ip_port.Parse(ip_port);
            int count = MyClients.length;
            Random _time = new Random((int)DateTime.Now.Ticks / 10000);
            for (int i = 0; i < n_nodes+ n_cars; i++)
            {
                try
                {
                    int id = 1001 + i + count;
                    Client c = new Client(xip_port.ip_address, xip_port.port);
                    if (c == null)
                    {
                        logMessage("Can't connect to Server ");
                        break;
                    }


                    c.ID = string.Format("{0}", id);
                    c.UiId = id;
                    string type = i >= n_nodes ? "car" : "node";
                    string c_id = c.ClientSocket.LocalEndPoint.ToString();
                    c.Start(OnMessageHandler, OnDisconnect);
                    MyClients.Add(c);
                    NodesInfo info = NodesInfo.generateRandomNode(c_id, id, type, 1500, 1500);
                    if(i < n_nodes )
                    {
                        double time = _time.Next(120) * 1000;

                         ns_Timers timer = new ns_Timers(time, string.Format("{0}", id), (obj) =>
                        {
                            logMessage(string.Format("{0} is Now Running", obj.Timer_id));
                            c.SendMsg("charge");
                            obj.stopTimer();
                        });
                         
                         MyTimers.Add(timer);
                         logMessage(string.Format("Timer_id:{0} , time:{1} s", id, time / 1000));
                    }
                   
                    this.Dispatcher.Invoke(() =>
                    {
                        //myNodes.Add(info);
                        _model.NodesInfox.Add(info);
                        this.mdata_grid.Items.Refresh();
                    });

                } 
                catch (Exception ex)
                {
                    logMessage(ex.Message);
                    break;
                }
            }
                


            

        }

    }
}
