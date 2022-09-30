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
using System.Timers;
using ns.networking;
using System.IO;
using Path = System.Windows.Shapes.Path;
namespace ns.graph
{
    /// <summary>
    /// Set of Nodes.
    /// 
    /// </summary>
    public class ns_node_collection : ns_list<ns_node>
    {
        public ns_node_collection()
        {

        }
        /// <summary>
        /// if the Node is already Exist don't add it otherwise add the node .
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public ns_node_collection AddNode(ns_node n)
        {
            if (this.Exists((f) => { return n.tag == f.tag; })) return this;
            
            this.add(n);
            return this;
        }
        public bool Contains(ns_node n)
        {
            return this.Exists((f) => { return f.tag == n.tag; });
        }
        public ns_node_collection AddNodes(params ns_node[] nodes)
        {
            foreach(ns_node n in nodes)
            {
                this.AddNode(n);
            }
            return this;
        }

        public void Print(Action<string > __print)
        {
            this.Foreach((n) =>
                {
                    __print(n.tag);
                });
        }
        public Path RenderPath(Brush color , double thickness)
        {
            ns_pen pen = new ns_pen();
            pen.Strokecolor = color;
            pen.thickness = thickness;
            this.Foreach((n, i) =>
                {
                    if(i==0)
                    {
                        pen.M(n.location);
                        
                    }
                    else
                    {
                        pen.L(n.location);
                    }

                });
            return pen.getPath();
        }
        public int getIndexOf(ns_node n)
        {
            int i = 0;
            int c = this.Count;
            for(i=0;i<c;i++)
            {
                if (n.tag == this[i].tag) return i;
            }
            return -1;
        }
        public ns_node_collection RemoveNode(ns_node n)
        {
            this.Remove(n);
            return this;
        }
        public ns_node getNodebyTag(string tag)
        {
            ns_node nn = new ns_node();

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].tag == tag)
                {
                    return this[i];
                }
            }


            return nn;
        }
        
    }
    public enum Node_TypeEnum
    {
        Sensor_node =0,
        Mobile_Charger =1,
        BaseStation =2
    };
    
    public class node_PowerConfig
    {
        public double initial_energy { get; set; }
        public double residual_energy { get; set; }
        public double energy_consumptionRate { get; set; }
        public double energy_threshold { get; set; }
        public double Comm_Range { get; set; }
        public double PacketEnergy { get; set; }
        public double Sleep_mode_En { get; set; }
        public double threshold_switch { get; set; }
        /// <summary>
        /// Battery capacity in AH // amp.hour
        /// </summary>
        public double BatteryCapacity { get; set; }
        public double BatteryDischargeRate { get; set; }
        public double Temperature { get; set; }
        public int charge_request_packets_tx { get; set; }
        public int charge_request_packets_rx { get; set; }
        public int forward_packets { get; set; }
        public double ChargeRate { get; set; }
        private StackPanel _sp;
        private bool is_render = false;
        private string power_plot;
        private bool pfirst;
        private double time;
        private ns_point scalePoint;
       // private Brush brush;
        private ns_node thenode;
        private double xmax_power;
        private string energy_data_file_name;
        private string packet_data_file_name;
        // measure power every samples_time
        private int samples_time = 10;// 10*3=30;
        public node_PowerConfig()
        {
           // __init__();

            this._sp = new StackPanel();
            this.is_render = false;
            this.power_plot = "";
            this.pfirst = true;
            this.scalePoint = new ns_point(1, 1);
            this.time = 0;
            this.xmax_power = this.BatteryCapacity;

           // this._sp.
        }
        public void SetScale(double _time, double y)
        {
            this.scalePoint = new ns_point(_time, y);

        }
        public double getMaxBatteryCapacity()
        {
            return this.initial_energy;
        }
        public double getChargeTime()
        {
            return Math.Abs((this.initial_energy - this.BatteryCapacity) / (this.ChargeRate));
        }
        private string dead_file_name;
        public void SetTheNode(ns_node node)
        {
            this.thenode = node;
            this.energy_data_file_name = string.Format("eng_{0}.dat", node.tag);
            this.packet_data_file_name = string.Format("packets_{0}.dat", node.tag);
            this.dead_file_name = string.Format("dead_{0}.dat", node.tag);
        }
        public long dead_at()
        {
            DateTime now = DateTime.Now;
            DateTime start = this.thenode.getStartTime();
            long fx = (now.Millisecond - start.Millisecond)*1000;
            return fx;
        }
        private bool sleep_mode_enable = false;
        public void EnableSleepMode(bool en)
        {
            this.sleep_mode_enable = en;
        }
        public void Calc_dispRate()
        {
            //appendtoData();
            if ((this.BatteryCapacity > this.energy_threshold) )
            {
                
                
                this.BatteryCapacity = this.BatteryCapacity - this.energy_consumptionRate;
                return;
            }
            if (this.sleep_mode_enable)
            {
                this.BatteryCapacity = this.BatteryCapacity - this.Sleep_mode_En;
                return;
            }
            if ((this.BatteryCapacity - this.energy_consumptionRate) > 0)
            {
                this.BatteryCapacity = this.BatteryCapacity - this.energy_consumptionRate;
            }
           // this.max_power += 1 + this.BatteryDischargeRate;
            
        }
        
        private void appendtoData()
        {
            double xscale = this.scalePoint.x;
            double yscale = this.scalePoint.y;
            ns_point p = new ns_point(this.time, this.BatteryCapacity);//.scale(1 / xscale, 1 / yscale);
            if(pfirst )
            {
                File.WriteAllText(this.energy_data_file_name, "");// clear.
                //string fmt = "x\ty\t\n";
                //File.WriteAllText(this.energy_data_file_name, fmt);
                //this.power_plot += p.moveto();
                this.time = 0;
                this.pfirst = false;
                return;
            }
            //this.power_plot += p.lineto();
            if (time % samples_time == 0)
            {
                string c = string.Format("{0}\n", this.BatteryCapacity);
                //File.AppendAllText(this.energy_data_file_name, p.toLatexDataFilePoint());
                File.AppendAllText(this.energy_data_file_name, c);
            }
            this.time++;
        }
        public double getTime()
        {
            return this.time;
        }
        public Path GetPowerPlot()
        {
            Path mp = new Path();
            mp.Stroke = this.thenode.location.ColorFromPoint();
            mp.ToolTip = string.Format("Power of {0}", this.thenode.tag);
            mp.StrokeThickness = 1.5;
            mp.Data = Geometry.Parse(this.power_plot);
            return mp;
        }
        public void Calc_PacketDispPower()
        {
            if (this.BatteryCapacity > 1) 
            {
                this.BatteryCapacity = this.BatteryCapacity - this.PacketEnergy;
            }
           // this.max_power += 1 + this.PacketEnergy;
        }
        private void _init_ui()
        {
            if (is_render) return;
            is_render = true;
            createTextBox("Comm_Range", this.Comm_Range, (s, v) =>
            {
                TextBox tb = s as TextBox;
                try
                {
                    string txt = tb.Text;
                    double value = double.Parse(txt);
                    this.Comm_Range = value;
                }
                catch (Exception) { }
            });
            //--------
            createTextBox("BatteryCapacity", this.BatteryCapacity, (s, v) =>
            {
                TextBox tb = s as TextBox;
                try
                {
                    string txt = tb.Text;
                    double value = double.Parse(txt);
                    this.BatteryCapacity = value;
                }
                catch (Exception) { }
            });
            //---
            createTextBox("BatteryDischargeRate", this.BatteryDischargeRate, (s, v) =>
            {
                TextBox tb = s as TextBox;
                try
                {
                    string txt = tb.Text;
                    double value = double.Parse(txt);
                    this.BatteryDischargeRate = value;
                }
                catch (Exception) { }
            });
            createTextBox("initial_energy", this.initial_energy, (s, v) =>
                {
                    TextBox tb = s as TextBox;
                    try
                    {
                        string txt = tb.Text;
                        double value = Double.Parse(txt);
                        this.initial_energy = value;
                    }
                    catch (Exception) { }
                });
            //----------
            createTextBox("residual_energy", this.residual_energy, (s, v) =>
            {
                TextBox tb = s as TextBox;
                try
                {
                    string txt = tb.Text;
                    double value = double.Parse(txt);
                    this.residual_energy = value;
                }
                catch (Exception) { }
            });
            //----------
            createTextBox("energy_consumptionRate", this.energy_consumptionRate, (s, v) =>
            {
                TextBox tb = s as TextBox;
                try
                {
                    string txt = tb.Text;
                    double value = double.Parse(txt);
                    this.energy_consumptionRate = value;
                }
                catch (Exception) { }
            });
            ///------
            ///
            createTextBox("Temperature", this.Temperature, (s, v) =>
            {
                TextBox tb = s as TextBox;
                try
                {
                    string txt = tb.Text;
                    double value = double.Parse(txt);
                    this.Temperature = value;
                }
                catch (Exception) { }
            });
            //---
            
            //
            createTextBox("energy_threshold", this.energy_threshold, (s, v) =>
            {
                TextBox tb = s as TextBox;
                try
                {
                    string txt = tb.Text;
                    double value = double.Parse(txt);
                    this.energy_threshold = value;
                }
                catch (Exception) { }
            });
            //
            createTextBox("energy_threshold", this.energy_threshold, (s, v) =>
            {
                TextBox tb = s as TextBox;
                try
                {
                    string txt = tb.Text;
                    double value = double.Parse(txt);
                    this.energy_threshold = value;
                }
                catch (Exception) { }
            });
            //
            createTextBox("PacketEnergy", this.PacketEnergy, (s, v) =>
            {
                TextBox tb = s as TextBox;
                try
                {
                    string txt = tb.Text;
                    double value = double.Parse(txt);
                    this.PacketEnergy = value;
                }
                catch (Exception) { }
            });


        }
        private void __init__()
        {
            this.initial_energy = 100;
            this.residual_energy = 0;
            this.energy_consumptionRate = 0.002;
            this.energy_threshold = 30;
            this.Comm_Range = 100;// wifi
            this.PacketEnergy = 1;
           
            this.BatteryCapacity = 2500; // 2500ampH
            this.BatteryDischargeRate = 0.002;
            this.Temperature = 25;// room Temperature;
            this.charge_request_packets_rx = 0;
            this.charge_request_packets_tx = 0;
            this.forward_packets = 0;
            this.ChargeRate = 2;//
            this.Sleep_mode_En = 0.001;
        }
        public bool Charge()
        {
            this.BatteryCapacity += this.ChargeRate;
            if(this.BatteryCapacity>=this.xmax_power)
            {
                return true;
            }
            return false;
        }
        
        public StackPanel getUiPanel()
        {
            _init_ui();
            return this._sp;
        }
        public void AddUIChild(UIElement ele)
        {
            this._sp.Children.Add(ele);
        }
        public void createTextBox(string _label, double value , TextChangedEventHandler __handler )
        {
            Label l = new Label();
            l.Content = _label;
            TextBox tb = new TextBox();
            tb.Text = value.ToString();
            tb.TextChanged += __handler;
            this._sp.Children.Add(l);
            this._sp.Children.Add(tb);
            
        }
       
        
    }
    /// <summary>
    /// 2D node -> x,y 
    /// </summary>
    public class ns_node
    {
        public string tag;
        private ns_attributes Attributes = new ns_attributes();
        public ns_node_collection ChildNodes;
        private Queue<ns_node> neigborsQ ;//= new Queue<ns_node>(); 
        public ns_point location;
        public Node_TypeEnum NodeType;
        public bool visited = false;
        private int net_id;
        private double cost;
        private Timer mTimer = new Timer();
        public delegate void __printf(string fmt, params object[] args);
        private __printf printf;
        public node_PowerConfig PowerConfig { get; set; }
        public double Radius { get; set; }

        private int server_port;
        private int client_port;
        private bool hasServer = false;
        private bool hasClient = false;
        

        private ns_communication Com_ModuleServer;

       

        public ns_node()
        {
            this.Attributes = new ns_attributes();
            
            this.ChildNodes = new ns_node_collection();
            this.location = new ns_point(0, 0);
           
            this.visited = false;
            
            gencost();
            this.Radius = 8;
            this.PowerConfig = new node_PowerConfig();
            this.PowerConfig.SetTheNode(this);
            this.neigborsQ = new Queue<ns_node>();
            this.tag = string.Empty;
        }
        
        public ns_node(string _tag)
        {
            this.tag = _tag;
            this.Attributes = new ns_attributes();
            this.ChildNodes = new ns_node_collection();
            location = new ns_point(0, 0);
           
           
            this.visited = false;
            
            gencost();
            this.Radius = 8;
            this.PowerConfig = new node_PowerConfig();
            this.neigborsQ = new Queue<ns_node>();
            this.PowerConfig.SetTheNode(this);
        }

        public ns_node(string _tag, double x , double y)
        {
            this.tag = _tag;
            this.Attributes = new ns_attributes();
            this.ChildNodes = new ns_node_collection();
            this.location = new ns_point(x, y);
         
            this.visited = false;
           
            gencost();
            this.Radius = 8;
            this.PowerConfig = new node_PowerConfig();
            this.neigborsQ = new Queue<ns_node>();
            this.PowerConfig.SetTheNode(this);
        }
        public ns_node(string _tag , ns_point loc)
        {
            this.tag = _tag;
            this.Attributes = new ns_attributes();
            this.ChildNodes = new ns_node_collection();
            this.location = loc;
          
            this.visited = false;
            this.Radius = 8;
            this.PowerConfig = new node_PowerConfig();
            this.neigborsQ = new Queue<ns_node>();
            this.PowerConfig.SetTheNode(this);
            gencost();
        }
        public void SetNetId(int id)
        {
            this.net_id = id;
            this.server_port = 50001 + id;
            this.client_port = 45000 + id;
        }
        public int getPort()
        {
            return this.server_port;
        }
        private List<ns_communication> Com_Clients = new List<ns_communication>();
       // private Dictionary<int, ns_node> clientDict = new Dictionary<int, ns_node>();
        public void init_com_Module()
        {
            //if (this.server_port > 24444 && this.hasServer) 
            //this.Com_ModuleServer = new ns_communication(this.server_port, this);
            //if(this.hasClient)
            //{
            //    foreach (int port in this.ServerPorts)
            //    {
            //
            //        
            //            var client = new ns_communication(port, this);
            //            this.Com_Clients.Add(client);
            //        
            //    }
            //}
        }
        public int getTxPackets()
        {
            
            return this.tx_packets;
        }
        public int getServerPort()
        {
            return this.server_port;
        }
        
        public int getRxPackets()
        {
            int packets = 0;
            try
            {


                if (this.hasServer && this.Com_ModuleServer != null)
                {
                    var ser = this.Com_ModuleServer.getServer();//.rx_packets;

                    packets = ser != null ? ser.rx_packets : 0;


                }

            }
            catch (Exception)
            {
                return -1;
            }


            return this.rx_packets;
        }
        private DateTime StartTime;
        public TimeSpan getLifeTime()
        {
            DateTime now = DateTime.Now;
            return now.Subtract(this.StartTime);
        }
        public bool isDead
        {
            get
            {
                return this.PowerConfig.BatteryCapacity < 1;
            }
        }
        private int sector_id;
        public int SectorId
        {
            get
            {
                return this.sector_id;
            }
            set
            {
                this.sector_id = value;
            }
        }
        public string getLifeTimeString()
        {
            TimeSpan span = this.getLifeTime();
            //node.getLifeTime().ToString(@"dd\.hh\:mm\:ss")
            return span.ToString(@"dd\.hh\:mm\:ss");
        }
        private bool already_connected = false;
        public bool Start()
        {
            this.StartTime = DateTime.Now;
            //if (this.already_connected) return true;
            if(this.ChildNodes.Count>0)
            {
                this.Com_ModuleServer = new ns_communication(this.server_port, this);
                this.Com_ModuleServer.StartServer();
                this.already_connected = true;
                //printfx("Server {0} is started listening on port {1}", this.tag, this.server_port);
                this.ChildNodes.Foreach(n =>
                    {
                        var c = new ns_communication(this.server_port, n);
                        n.already_connected = true;
                        c.StartClient();
                        this.Com_Clients.Add(c);
                      //  printfx("{0} is connected to {1} on port {2}", n.tag, this.tag, this.server_port);
                    });
            }
            StartPowerTimer();
            return this.already_connected;
        }
        public DateTime getStartTime()
        {
            return this.StartTime;
        }
        private bool first_charge_request_sent = false;
        private double charge_req_sent_time = 0;
        public double FirstChargeRequestTime(bool set)
        {
            if (!set) return this.charge_req_sent_time;
            if (set && !this.first_charge_request_sent)
            {
                this.charge_req_sent_time = DateTime.Now.Subtract(this.StartTime).TotalSeconds;
                this.first_charge_request_sent = true;
               
            }
            return this.charge_req_sent_time;
        }
        public void xStart()
        {
            this.StartTime = DateTime.Now;
            if (this.hasServer)
            {
                if (this.Com_ModuleServer != null)
                {
                    this.Com_ModuleServer.StartServer();
                    printfx("{0} is listening on port {1}", this.tag, this.server_port);
                }
            }
            if (this.Com_Clients.Count > 0) 
            {
                printfx("Node {0} clients:", this.tag);
                foreach(var c in this.Com_Clients)
                {

                    printfx("client {0}:{1}", c.c_id, c.getPort());
                    
                    c.StartClient();
                }
                printfx("-------------------");
            }
            StartPowerTimer();
        }
        private Timer net_Timer;
        private Timer PowerConsumptionTimer;
        public void StartPowerTimer()
        {
            this.PowerConsumptionTimer = new Timer(1000);//3*10=30
            this.PowerConsumptionTimer.Elapsed += PowerConsumptionTimer_Elapsed;
            this.PowerConsumptionTimer.Start();
        }
        public delegate void __OnPowerReachThreshold(ns_node node);
        public event __OnPowerReachThreshold OnPowerDown;
        private bool charge_request_sent = false;
        void PowerConsumptionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            double power = this.PowerConfig.BatteryCapacity;
            double threshold = this.PowerConfig.energy_threshold;
            //only send charge request and wait.
           /* if(power<=threshold && !this.charge_request_sent)
            {
                                            
               // OnExPowerDown();
              
            }*/
            // ensure that charge request feedback from MC.
            // this value is set to true by mobile charge when it received
            // the charge request from the corresponding node.
            // it also set to false when MC finish charge the nodes.
            if(power<=threshold && !this.charge_request_received)
            {
                OnExPowerDown();
            }
            this.PowerConfig.Calc_dispRate();
        }
        public void set_charge_request_toggle()
        {
            this.charge_request_sent = !this.charge_request_sent;
        }
        public virtual void OnExPowerDown()
        {
            printfx("Power is reach threshold:{0}", this.tag);
            //this.charge_request_sent = true;
            set_charge_request_toggle();
           
        }
        public ns_node getBS()
        {
            return this.BaseStation;
        }
        private void SetGuassianThreshold(int n, int N, double alpha)
        {
            if(this.NodeType == Node_TypeEnum.Sensor_node)
            {
                int half_n = N / 2;
                if (n <= half_n) 
                {
                    n = half_n - (n);
                }
                
                this.PowerConfig.energy_threshold = ns_window_functions.getGuassianSample(n, alpha, 30, N) + 10;
            }
            
        }
        private int tx_packets = 0;
        private bool is_already_sent_assign_packet = false;
        private int node_index = 0;
        private ns_node BaseStation;
       // private int node_importance = 1;

        public int getChildCount()
        {
            return this.ChildNodes.Count;
        }
        
        public void SetBaseStation(ns_node bs)
        {
            this.BaseStation = bs;
        }
      //  public ns_node GetBaseStation()
        public List<ns_node> GetPathToBs()
        {
            return graph_algorithms.Dijkstra(this, this.BaseStation);
        }
        public List<ns_node> GetPathTo(ns_node node)
        {
            return graph_algorithms.Dijkstra(this, node);
        }
        
        public bool SendPacketTo(ns_node dst, string msg)
        {
            //packet.src_addr = this.tag;
            //packet.final_dst = dst.tag;
            var path = this.GetPathTo(dst);
            if(path == null)
            {
                printf("NO path between {0}:{1}", this.tag, dst.tag);
                return false; ;
            }
            if (path != null)
            {
                ns_node first = this;
                string last = path.Last().tag;
                foreach (var node in path)
                {
                    if (first.tag == node.tag) continue;
                    //printfx("{0} next :{1}", this.tag, node.tag);
                    net_packet packet = new net_packet(this.tag, node.tag, packetType.Data, last);

                    packet.data = msg;

                    first.SendPacket(packet);
                    first = node;
                    //if (node.NodeType == Node_TypeEnum.Mobile_Charger) break;
                }

            }
            return true;
            
        }
        private bool charge_request_received = false;
        public void NotifyChargeRequestReceived(bool value)
        {
            this.charge_request_received = value;
        }
        public bool IsChargeRequestReceived()
        {
            return this.charge_request_received;
        }
        public double getNodeHopIndex()
        {
            if (this.node_index == 0) return 1;
            return this.node_index;
        }
        private string thres_parent_node;
       
        public virtual void OnAssignThresHoldPacketReceived(net_packet packet)
        {
           // Random r = new Random(DateTime.Now.Millisecond);
           // int n = r.Next(packet.hop_count + packet.child_index, packet.numberofNodes);
           // this.SetGuassianThreshold(n, packet.numberofNodes, 0.3);

           
            if (!this.is_already_sent_assign_packet)
            {
                
                this.thres_parent_node = packet.src_addr;// very important.
                this.is_already_sent_assign_packet = true;
                if (this.NodeType == Node_TypeEnum.Sensor_node)
                {
                   int i = packet.child_index;
                   this.PowerConfig.energy_threshold = ns_window_functions.GetMyThreshold(i, 0.45, 5, 35, 50);
                  
                    packet.child_index = packet.child_index + 1;
                    printf("{0} \t {1} \t {2}", this.tag, i, this.PowerConfig.energy_threshold);
                }
            }

            //printfx("cindex\t{0}\t src:\t{1} dst:\t{2}", packet.child_index, this.tag , this.thres_parent_node);
            packet.hop_count = packet.hop_count + 1;
           
            //this.node_index = n;
            // don't send the packet again
            if (this.ThrsQueue.Count == 0)
            {
               // int b = this.node_index;
               // this.node_index = Math.Max(n, this.node_index);
               // printf("{0} index:{1}-{2}", b, this.node_index);
               // packet.hop_count = this.node_index;
               
                if (this.tag != this.BaseStation.tag)
                {
                    var parent = this.ChildNodes.get((n) =>
                        {
                            return n.tag == this.thres_parent_node;
                        },this);
                    if (parent.tag != this.tag)
                    {
                        printfx("{0} queue is zero back toparent :{1}", this.tag, parent.tag);

                        parent.SendEngThresholdPacketbyFlooding(packet);
                    }
                }
                
                return;
            }
            this.SendEngThresholdPacketbyFlooding(packet);
        }
        private ns_node DequeueThres(string old_src)
        {
            // select from the queue any node except that.
            ns_node node = this.ThrsQueue.Dequeue();
            if( node.tag == old_src && (this.ChildNodes.Count >1) )
            {
                node = this.ThrsQueue.Dequeue();
            }
            return node;
        }
        public int GetImportanceValue()
        {
            return this.node_importance;
        }
        private int node_importance = 1;
        private string importance_parent_node = string.Empty;

        private ns_node get_node_byTag(string tag)
        {
            var parent = this.ChildNodes.get((n) =>
            {
                return n.tag == tag;
            }, this);
            return parent;
        }
        public ns_node GetFirstImportant()
        {
            if (this.ImportanceQueue.Count == 0) return new ns_node(string.Empty);
            return this.ImportanceQueue.Dequeue();
        }
       
        private void remove_node_from_importance_queue(string src)
        {
            if (this.ImportanceQueue.Count == 0) return;
            bool exists = false;
            foreach( var item in this.ImportanceQueue)
            {
                if(item.tag == src)
                {
                    exists = true;
                    break;
                }
            }
           // if( this.ImportanceQueue.Contains())
            if(exists)
            {
                var node =  this.ImportanceQueue.Dequeue();
                if( node.tag != src)
                {
                    this.ImportanceQueue.Enqueue(node);
                    this.remove_node_from_importance_queue(src);
                }
             return;
            }
            
           // this.ImportanceQueue.
        }
        private bool thres_already_set = false;
        public bool EnergyThresholdSet
        {
            get
            {
                return thres_already_set;
            }
            set
            {
                thres_already_set = value;
            }
        }
        public void SetEnergyThreshold(double value)
        {
            this.PowerConfig.energy_threshold = value;
            this.EnergyThresholdSet = true;
        }
        private void SetThreshold()
        {
            if (thres_already_set) return;// don't set.
            Random r = new Random(DateTime.Now.Millisecond);
            double importance = (this.node_importance + 1);// *this.layer_index;// *(new Random().NextDouble());
            double pi = Math.PI;
            double thres = 5 + (35 * Math.Exp(-1 * pi / importance));
            this.PowerConfig.energy_threshold = thres + r.NextDouble();


        }
        private int layer_index = 1;
        private int layer_index_used = 1;
        public int LayerIndex
        {
            get
            {
                return this.layer_index_used;
            }
            set
            {
                this.layer_index_used = value;
            }
        }
        public void SetLayerIndex(int layer)
        {
            this.layer_index_used = layer;
        }
        public int getLayerIndex()
        {
            return this.layer_index_used;
        }

        public void OnAssignImportancePacketReceived(net_packet packet)
        {
            //printfx("imp packet from {0} to {1}", packet.src_addr, packet.dst_addr);
            if(this.importance_parent_node == string.Empty && this.NodeType!=Node_TypeEnum.BaseStation)
            {
                this.importance_parent_node = packet.src_addr;
                this.layer_index = packet.hop_count + 1;
               // this.node_importance = this.ChildNodes.Count;
            }
            remove_node_from_importance_queue(packet.src_addr);
            if (packet.PacketDirection == packetDirection.backward)
            {
                this.node_importance += packet.importance;
            }
            if( this.ImportanceQueue.Count ==0 )
            {
               // this.node_importance = packet.importance == 0 ? 1 : packet.importance;
                printfx("{0}:{1}:{2}\n", this.tag, this.node_importance, this.layer_index);
                SetThreshold();
                if (this.importance_parent_node != string.Empty)
                {

                    packet.importance = this.node_importance;// +1;
                    packet.dst_addr = this.importance_parent_node;
                    packet.src_addr = this.tag;
                    packet.PacketDirection = packetDirection.backward;
                    this.OnSendAssignImportancePacket(packet);

                }
                return;
            }
            
            
            string dst = this.ImportanceQueue.Dequeue().tag;
            packet.dst_addr = dst;
            packet.hop_count = this.layer_index;
            packet.PacketDirection = packetDirection.forward;
            packet.importance = 0;// this.node_importance;
            this.OnSendAssignImportancePacket(packet);
        }
        public void OnSendAssignImportancePacket(net_packet packet)
        {
            if (packet.Type != packetType.assign_importance) return;
                
                //if(this.NodeType == Node_TypeEnum.BaseStation)

            if (this.Com_ModuleServer != null)
            {
                string old_src = packet.src_addr;
                // printfx("Server Id:{0}", this.Com_ModuleServer.c_id);
                // To avoid back forth problem of the packet.
               // ns_node dst_node = this.DequeueThres(old_src);

                // if (this.is_already_sent_assign_packet) return;
                var server = this.Com_ModuleServer.getServer();
                if (server == null) return;
                packet.src_addr = this.tag;

                foreach (var cc in server.Clients)
                {
                    //printf("server:{0} client:{1}", this.tag, cc.Key);

                    string dst = cc.Value.client_id;
                    if (dst != packet.dst_addr) continue;
                   // packet.dst_addr = dst;

                    if (cc.Value.SendPacket(packet))
                    {
                       // printfx("assign_importance sent from {0} to {1}", this.tag, dst);
                        //this.tx_packets++;
                    }
                    break;
                }
            }

        }
        public void SendEngThresholdPacketbyFlooding(net_packet packet)
        {
           // net_packet packet = new net_packet(this.)
            printf("hop count:{0}", packet.hop_count);
            if (packet.Type != packetType.assign_threshold) return;
           if (this.ThrsQueue.Count == 0)
           {
               if (this.tag != this.BaseStation.tag)
               {
                   var parent = this.ChildNodes.get((n) =>
                   {
                       return n.tag == this.thres_parent_node;
                   }, this);
                   if (parent.tag != this.tag)
                   {
                       printfx("{0} queue is zero back toparent :{1}", this.tag, parent.tag);

                       parent.SendEngThresholdPacketbyFlooding(packet);
                   }
               }
           }

            if (this.Com_ModuleServer != null)
            {
                string old_src = packet.src_addr;
                // printfx("Server Id:{0}", this.Com_ModuleServer.c_id);
                // To avoid back forth problem of the packet.
                ns_node dst_node = this.DequeueThres(old_src);

               // if (this.is_already_sent_assign_packet) return;
                var server = this.Com_ModuleServer.getServer();
                if (server == null) return;
                packet.src_addr = this.tag;
                
                foreach (var cc in server.Clients)
                {
                    //printf("server:{0} client:{1}", this.tag, cc.Key);

                    string dst = cc.Value.client_id;
                    if (dst != dst_node.tag) continue;
                    packet.dst_addr = dst;
                    
                    if (cc.Value.SendPacket(packet))
                    {
                        printfx("assign_packet sent from {0} to {1}", this.tag, dst);
                        //this.tx_packets++;
                    }
                    break;
                }
            }

        }
        public void ForwardChargeRequestPacket(net_packet packet)
        {
            if (this.Com_ModuleServer != null)
            {


                var server = this.Com_ModuleServer.getServer();
                if (server == null) return;
                foreach (var cc in server.Clients)
                {
                    if (packet.src_addr == cc.Key) continue;

                    {
                        if (cc.Value.SendPacket(packet))
                        {
                            printfx("forward packet sent from {0} to {1}", this.tag, cc.Key);
                            //this.tx_packets++;
                        }

                    }
                }
            }
        }
        public void ns_send_packet(net_packet packet)
        {
            if (this.Com_ModuleServer != null)
            {


                var server = this.Com_ModuleServer.getServer();
                if (server == null) return;
                string from = packet.src_addr;
                packet.src_addr = this.tag;
                foreach (var cc in server.Clients)
                {
                    if (from == cc.Key) continue;
                    
                      packet.dst_addr = cc.Key;
                    
                        // packet.src_addr = this.tag;
                      
                    {
                        if (cc.Value.SendPacket(packet))
                        {
                           
                            printfx("forward packet sent from {0} to {1}", this.tag, cc.Key);
                            if (cc.Key == packet.final_dst) break;
                            //this.tx_packets++;
                        }

                    }
                   
                }
            }
        }
        public void sendChargeRequestPacket(net_packet packet)
        {
            if (this.Com_ModuleServer != null)
            {
                

                var server = this.Com_ModuleServer.getServer();
                if (server == null) return;
                string from = packet.src_addr;
                foreach (var cc in server.Clients)
                {
                    if (cc.Key == from) continue;// don't send back to src.
                    
                    
                        if (cc.Value.SendPacket(packet))
                        {
                            printfx("charge packet sent from {0} to {1}", this.tag, cc.Key);
                            //this.tx_packets++;
                        }
                       
                    
                }
            }
        }
        public void SendPacket(net_packet packet)
        {
            string dst = packet.dst_addr;
            bool sent = false;
            if(packet.Type == packetType.ChargeRequest && packet.src_addr==this.tag)
            {
                this.PowerConfig.charge_request_packets_tx += 1;// count charge requests;
            }
            if(packet.src_addr !=this.tag)
            {
                this.PowerConfig.forward_packets += 1;//count forwarded packets;
            }
           
            if(!sent)
            {
                if(this.Com_ModuleServer!=null)
                {
                   // printfx("Server Id:{0}", this.Com_ModuleServer.c_id);

                    var server = this.Com_ModuleServer.getServer();
                    if (server == null) return;
                    foreach(var cc in server.Clients)
                    {
                        //printf("server:{0} client:{1}", this.tag, cc.Key);
                        if(cc.Key== dst)
                        {
                            if (cc.Value.SendPacket(packet))
                            {
                               // printfx("packet sent from {0} to {1}", this.tag, dst);
                                this.tx_packets++;
                            }
                            break;
                        }
                    }
                }
            }

        }
        public void SendPacket(string packet)
        {
            if(this.Com_ModuleServer!=null)
            {
                var server = this.Com_ModuleServer.getServer();

                //foreach(var cc in server.Clients)
                //{
                //   // printf("server:{1} clx:{0}", cc.Key , this.tag);
                //    cc.Value.SendMsg("Hello"); // server will send msg to client
                //}
            }
            if(this.Com_Clients.Count>0)
            {
                //this.Com_ModuleClient.SendMessageClient(packet);
                foreach (var c in this.Com_Clients)
                {
                    c.SendMessageClient(packet); // client will send msg to server.
                    //printfx("cId:{0}", c.c_id);
                    this.tx_packets++;
                }

            }
            else
            {
                if(this.net_Timer!=null)
                {
                    this.net_Timer.Stop();
                }
            }
        }
        private int net_timer_start_sec = 20;
        private int net_timer_end_sec = 40;
        public void SetNetTimerTimeRange(int start_sec, int end_sec)
        {
           
            
                this.net_timer_end_sec = Math.Max(start_sec, end_sec);
                this.net_timer_start_sec = Math.Min(start_sec, end_sec);

             this.net_timer_end_sec = this.net_timer_end_sec == start_sec ? start_sec + 10 : this.net_timer_end_sec;
        }
        public void StartNetTimer(Random time)
        {
            //Random time = new Random(DateTime.Now.Millisecond);
            int interval = time.Next(this.net_timer_start_sec * 1000, this.net_timer_end_sec * 1000);// 2s - 14 s
            this.net_Timer = new Timer(interval);
            this.net_Timer.Elapsed += net_Timer_Elapsed;
            this.net_Timer.Start();
        }
        public void StartNetTimer(double interval)
        {
            printf("{0},int:{1}sec", this.tag, interval);
            //Random time = new Random(DateTime.Now.Millisecond);
            //int interval = time.Next(this.net_timer_start_sec * 1000, this.net_timer_end_sec * 1000);// 2s - 14 s
            this.net_Timer = new Timer(interval * 1000);
            this.net_Timer.Elapsed += net_Timer_Elapsed;
            this.net_Timer.Start();
        }

        void net_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.NodeType == Node_TypeEnum.Sensor_node)
            {
                string msg = string.Format("Hello from {0}", this.tag);

                this.SendPacketTo(this.BaseStation, msg);
               // SendPacket(msg);
            }
        }
        public void Stop()
        {
            if (this.Com_ModuleServer != null)
            {
                this.Com_ModuleServer.StopServer();
                //printfx("Node Stopped [Server:{0}] ...", this.tag);
            }
            if (this.Com_Clients.Count > 0) 
            {
                
                foreach (var c in this.Com_Clients)
                {
                    c.StopClient();
                }
                this.ServerPorts.Clear();
                this.Com_Clients.Clear();
            }
            if(this.net_Timer!=null)
            {
                this.net_Timer.Stop();
            }
            if(this.PowerConsumptionTimer!=null)
            {
                this.PowerConsumptionTimer.Stop();
            }
        }

        public delegate void _packet_anim(List<ns_node> relays , ns_node src);
        private _packet_anim packet_anim_handler;
        public ns_node SetPacketAnimationHandler(_packet_anim handler)
        {
            this.packet_anim_handler = handler;
            return this;
        }
        public void AnimatePacketTransmission(List<ns_node> relays)
        {
            if(this.packet_anim_handler!=null)
            {
                this.packet_anim_handler(relays, this);
            }
        }
        public ns_node SetPrintfHandler(__printf __handler)
        {
            this.printf = __handler;
            return this;
        }
        public void printfx(string fmt , params object[] args)
        {
            if(this.printf!=null)
            {
                this.printf(fmt, args);
            }
        }
        public Timer getTimer()
        {
            return this.mTimer;
        }
        private void gencost()
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            this.cost = r.Next(100);
            Random rx = new Random(DateTime.Now.Millisecond);
            AddAttribute("energy", rx.Next(1, 30));
            AddAttribute("threshold", rx.NextDouble());
            AddAttribute("dissipation_rate", rx.NextDouble());
            AddAttribute("com_range", 30);
        }
        public double getEn()
        {
            return this.HasAttribute("energy") ? double.Parse(this.getAttr_value("energy").ToString()) : 0;
        }
        public ns_node SetLocation(double x, double y)
        {
            this.location = new ns_point(x, y);
         
            return this;
        }
        public ns_attributes GetAttrs()
        {
            return this.Attributes;
        }
        public int getAttrsCount()
        {
            return this.Attributes.Count;
        }
        private string json_cns = "";
        public string JsonChildNodes
        {
            get
            {
                if (!string.IsNullOrEmpty(this.json_cns)) return json_cns;
                string json = "";
                this.ChildNodes.Foreach((n) =>
                    {
                        json += string.Format("{0},", n.tag);
                    });

                return json;
            }
            set
            {
                this.json_cns = value;
            }
        }
        
        
        private List<ns_keyValuePair> __json_attrs = new List<ns_keyValuePair>();
        public List<ns_keyValuePair> Links
        {
            get
            {
                return this.__json_attrs;
                
            }
            set
            {
                this.__json_attrs = value;
                
            }
        }
        public ns_node SetLocation(ns_point p)
        {
            return SetLocation(p.x, p.y);
        }
        public bool HasChildNodes()
        {
            return this.ChildNodes.Count > 0;
        }
        public bool HasAttributes()
        {
            return this.Attributes.Count > 0;
        }
        public bool HasAttribute(string at_name)
        {
            return this.Attributes.hasAttribute(at_name);
        }
        public ns_node AddAttribute(string key, object value)
        {
            if(this.HasAttribute(key))
            {
                this.Attributes[key].value = value;
                return this;
            }
            this.Attributes.add(key, value);
            this.__json_attrs.Add(new ns_keyValuePair(key, value));
            return this;
        }
        public ns_node SetAttributes(params object[] attrs)
        {
            this.Attributes.SetAttributes(attrs);
           
            return this;
        }
        private List<ns_link> __ui_links = new List<ns_link>();
        public List<ns_link>  getUi_links()
        {
            return this.__ui_links;
        }
        private void add_ui_link(ns_link link)
        {
            this.__ui_links.Add(link);
        }
        public ns_node AddChildNode(ns_node n)
        {
           
            this.ChildNodes.AddNode(n);
            return this;
        }
        public object getAttr_value(string key)
        {
            if (this.__json_attrs == null) return null;

            if (this.__json_attrs.Count > 0)
            {
                var kvp = this.__json_attrs.Find((a) =>
                {
                    return a.key == key;
                });
                if (kvp == null) return null;
                return kvp.value;
            } 

            if (this.HasAttribute(key))
                return this.Attributes[key];
            
            return null;
        }
        public double edistance(ns_node node)
        {
            return this.location.edistance(node.location);
        }
        public bool HasChild(ns_node n2)
        {
            return this.ChildNodes.Exists((f) => { return n2.tag == f.tag; });
        }
        
        public void SetServerPort(int port)
        {
            
            this.client_port = port;
            this.ServerPorts.Add(port);
            this.hasClient= true;
          
        }
        public void SetClientPort(int port )
        {
            this.server_port = port;
            this.hasServer = true;
           
        }
        private List<int> ServerPorts = new List<int>();
        public ns_node addLink(ns_node dst , double link_cost)
        {
            

            this.AddChildNode(dst);
            this.add_child_toQ(dst);
            dst.add_child_toQ(this); //1 
            this.hasServer = true;
            dst.AddChildNode(this);
            // dst.SetServerPort(this.server_port);
              dst.hasServer = true;//
            this.SetServerPort(dst.server_port);//

            string at_name = string.Format("{0}-{1}", this.tag, dst.tag);
            string at_name2 = string.Format("{0}-{1}", dst.tag, this.tag);
            if(!this.HasAttribute(at_name))
                this.AddAttribute(at_name, link_cost);
            this.__json_attrs.Add(new ns_keyValuePair(at_name, link_cost));
            if (!dst.HasAttribute(at_name2))
                dst.AddAttribute(at_name2, link_cost);
            //ns_link link = new ns_link(this, dst);
            // this.add_ui_link(link);
            return this;
        }
       public void SendToClient(ns_node client , string msg)
        {
            string id = client.tag;
            net_packet packet = new net_packet(this.tag, client.tag, packetType.Data);
            packet.data = msg;
            try
            {
                
                ns_ServerNode server = this.Com_ModuleServer.getServer();
                if (server != null && server.HasClient(id))
                {
                    server.SendPacketTo(packet);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                this.printfx("Exception :{0} sending data to {0} from {1}", ex.Message, id, this.tag);
                string msgfx = string.Format("Exception :{0} sending data to {0} from {1}", ex.Message, id, this.tag);
                MessageBox.Show(msgfx);
            }
        }

       private Queue<ns_node> ThrsQueue = new Queue<ns_node>();
       private Queue<ns_node> ImportanceQueue = new Queue<ns_node>();
        private void add_child_toQ(ns_node node)
        {
            this.neigborsQ.Enqueue(node);
            this.ThrsQueue.Enqueue(node);
            this.ImportanceQueue.Enqueue(node);
           // this.printfx("{0} is added to the Q of {1}", node.tag, this.tag);
        }
        public Queue<ns_node> getChildQ()
        {
            return this.neigborsQ;
        }
        public ns_node DequeueChild()
        {
            ns_node node = this.neigborsQ.Dequeue();
            if(node!=null)
            {
                this.printfx("{0} is DeQu from the Q", node.tag);
            }
            return node;
        }

        
        public double mdistance(ns_node dst)
        {
            return this.location.mdistance(dst.location);
        }
        
        public virtual double getLinkCost(ns_node dst)
        {
            string at_name = at_name = string.Format("{0}-{1}", this.tag.Trim(), dst.tag.Trim());
            try
            {

                object value = this.getAttr_value(at_name);
                if (value == null)
                {
                    return 12;// double.NaN;
                }
               return double.Parse(value.ToString());
               // return 1;
            }catch (Exception)
            {
                //MessageBox.Show(at_name);
               // throw ex;
                return 120.0;
            }
        }
        public ns_node addLink(ns_node dst)
        {
           
            this.AddChildNode(dst);
           // dst.AddChildNode(this);
            return this;
        }
        public ns_node Linkto(params ns_node[] ns_nodes)
        {
           foreach(ns_node n in ns_nodes)
           {
               this.addLink(n);
           }
            return this;
        }
        public void printChildsNodes(Action<string> _loger)
        {
            this.ChildNodes.Foreach((n) =>
                {
                    _loger(n.tag);
                });
        }
       
        private bool renderd = false;

        public void PathTo(ns_node dst, ns_node_collection nodes)
        {
            if (nodes.Contains(this) || nodes.Contains(dst)) return;
            if(this.HasChild(dst))
            {
                nodes.AddNode(this);
                nodes.AddNode(dst);
                return;
            }
            if (dst.tag == this.tag)
            {
                nodes.AddNode(dst);
                return;
            };
            if (!this.HasChildNodes()) return;
            nodes.AddNode(this);
            this.ChildNodes.Foreach((n) =>
            {
                n.PathTo(dst, nodes);
            });
        }
        public void getConnectedNodes(ns_node_collection nodes)
        {
            if (nodes.Contains(this)) return;
            nodes.AddNode(this);
            this.ChildNodes.Foreach((n) =>
                {
                    n.getConnectedNodes(nodes);
                });
        }
       
        public bool IsNull()
        {
            try
            {
                return string.IsNullOrEmpty(this.tag);
            } catch (Exception )
            {
                return true;
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        //public void RenderCos
        private Path UiNode = new Path();

        public Path getUiNode()
        {
            this.UiNode.Tag = this;
            return this.UiNode;
        }
        public bool IsRendered()
        {
            return this.renderd;
        }
       // private bool csharp_rendered = false;
        private Label _nodeText = new Label();
        public Label getNodeText()
        {
            return this._nodeText;
        }
        public void RenderAsCSharp()
        {
            
        }
        public void RenderOnly()
        {
            if (this.renderd) return;
            ns_circle cir = new ns_circle(this.tag, this.location, this.Radius);
            //ele.Add(this.location.R);
            cir.color = this.location.ColorFromPoint();

            //cir.RenderTo(ele);
            this.UiNode = cir.UiNode;
            this._nodeText = cir.Text;
            this.renderd = true;
        }

        private int _node_row = 1;
        public int NodeRow
        {
            get
            {
                return _node_row;
            }
            set
            {
                _node_row = value;
            }
       
        }
        private int _node_col = 1;
        public int NodeCol
        {
            get
            {
                return _node_col;
            }
            set
            {
                _node_col = value;
            }
        }
       // private bool link_rendered = false;
       // private Dictionary<string, bool> rendered_link_dict = new Dictionary<string, bool>();

        public void __RenderTo(UIElementCollection ele,  Dictionary<string, bool> rendered_link_dict)
        {
            if (this.renderd) return;
            ns_circle cir = new ns_circle(this.tag, this.location, this.Radius);
            cir.Layer_Index = this.LayerIndex;
            cir.column = this.NodeCol;
            cir.row = this.NodeRow;
            //ele.Add(this.location.R);
            cir.color = this.location.ColorFromPoint();
            //var px = cir.Render(cir.color);
           // px.ToolTip = string.Format("{0}", this.LayerIndex);
            //ele.Add(px);
            cir.RenderTo(ele);
            this.UiNode = cir.UiNode;
            this._nodeText = cir.Text;
            this.renderd = true;
            if (this.HasChildNodes())
            {
                this.ChildNodes.Foreach((n) =>
                {

                   
                    string k1 = string.Format("{0}_{1}", this.tag, n.tag);
                    string k2 = string.Format("{0}_{1}", n.tag, this.tag);
                    bool cd = rendered_link_dict.ContainsKey(k2) && rendered_link_dict.ContainsKey(k1);
                    if (!cd)
                    {
                      
                        ns_link link = new ns_link(this, n);
                        // this.add_ui_link(link);
                        if (!rendered_link_dict.ContainsKey(k1))
                        rendered_link_dict.Add(k1, true);
                        if (!rendered_link_dict.ContainsKey(k2))
                        rendered_link_dict.Add(k2, true);

                       // rendered_link_dict[k2] = true;
                        link.RenderTo(ele);
                        
                       // n.renderd = true;
                    }
                    n.__RenderTo(ele, rendered_link_dict); 
                    //n.link_render = true;

                 
                });
            }
            else
                return;
        }
         public void RenderTo(UIElementCollection ele)
        {
            Dictionary<string, bool> xrendered_link_dict = new Dictionary<string, bool>();
            __RenderTo(ele, xrendered_link_dict);
        }
         private int rx_packets = 0;
        public virtual void OnPacketReceived(net_packet packet , ns_node from)
        {
            this.rx_packets++;
        }
        public virtual void OnPacketReceivedServer(net_packet packet , ns_node from)
        {
            this.rx_packets++;
        }
        public void updateNodeLocation(ns_point p, Window wind)
        {
            wind.Dispatcher.Invoke(() =>
                {
                    Path mp = this.getUiNode();
                });
        }
        public void SwitchThreshold()
        {//
            // if 
            double temp = this.PowerConfig.threshold_switch;
            double old_thres = this.PowerConfig.energy_threshold;
            double en = this.PowerConfig.BatteryCapacity;
            this.PowerConfig.threshold_switch = this.PowerConfig.energy_threshold;
            this.PowerConfig.energy_threshold = temp;
        }
        public virtual bool ChargingInProcess()
        {
            return false;
        }
         private bool latex_render = false;
        private string toLatexCoordinate()
         {
             return this.location.toLatexCoordinate(this.tag, 50);
         }
        public string rptag()
        {
            var tag = this.tag;
            if( tag.Contains("_"))
            {
                return tag.Replace("_", "");
            }
            return tag;
        }
        private string addLatexLink(ns_node dst )
         {
             ns_point p = (this.location) + (dst.location);
             ns_point ccost = this.location.lerp(dst.location, 0.50).shift(-1.5, -1.5);
             string c_name = string.Format("{0}_{1}", this.rptag(), dst.rptag());
             string color = p.toLatexColor(c_name);
           // string _link = string.Format("\\draw[dashed , color={2}] {0}--{1};\n", this.location.toLatexPoint(50), dst.location.toLatexPoint(50), c_name);
            string _link = string.Format("\\draw[dashed , color={2}] ({0})--({1});\n", this.rptag(), dst.rptag(), c_name);

            string cost = string.Format("{0}", this.GetImportanceValue()+1);
            string _link_cost = ccost.latexDrawTextAt(cost, c_name);
            return color + _link;// +_link_cost;
         }
        private string latex_text = "";
        public string toLatex(Dictionary<string, bool> rendered_link_dict)
         {
             string ss = this.latex_text;
             if (this.latex_render) return this.latex_text;
             ss += this.location.toLatexCircle(this.tag, this.Radius, 50, this.tag);
             this.latex_render = true;
             if (this.HasChildNodes())
             {
                 this.ChildNodes.Foreach((n)=>
                 {
                     string k1 = string.Format("{0}_{1}", this.tag, n.tag);
                     string k2 = string.Format("{0}_{1}", n.tag, this.tag);
                     bool cd = rendered_link_dict.ContainsKey(k2) && rendered_link_dict.ContainsKey(k1);
                     if (!cd)
                     {
                         if (!rendered_link_dict.ContainsKey(k1))
                             rendered_link_dict.Add(k1, true);
                         if (!rendered_link_dict.ContainsKey(k2))
                             rendered_link_dict.Add(k2, true);

                         ss += this.addLatexLink(n);
                     }
                 });
             }
             this.latex_text = ss;
             return this.latex_text;
         }
        public void xRenderTo(UIElementCollection ele)
        {
            if (this.renderd) return;
            ns_circle cir = new ns_circle(this.tag, this.location, this.Radius);
            //ele.Add(this.location.R);
            cir.color = this.location.ColorFromPoint();
            var px = cir.Render(cir.color);
            px.ToolTip = string.Format("{0}", this.layer_index);
            ele.Add(px);
            //cir.RenderTo(ele);
            this.UiNode = cir.UiNode;
            this._nodeText = cir.Text;
            this.renderd = true;
            if (this.HasChildNodes())
            {
                this.ChildNodes.Foreach((n) =>
                    {
                        string k1 = string.Format("{0}_{1}", this.tag, n.tag);
                        string k2 = string.Format("{0}_{1}", n.tag, this.tag);
                       // bool cd = rendered_link_dict.ContainsKey(k1) && rendered_link_dict.ContainsKey(k2);
                       

                            

                            
                        

                         if (!false)
                         {
                             ns_link link = new ns_link(this, n);
                            // this.add_ui_link(link);
                           //  rendered_link_dict[k1] = true;
                            // rendered_link_dict[k2] = true;
                             link.RenderTo(ele);
                         }
                            //n.link_render = true;
                        
                        n.RenderTo(ele);
                    });
            }
            else
                return;
        }

    }
}
