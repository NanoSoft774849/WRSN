using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
using System.Timers;
//using ns.wrsn;
namespace mTs
  
{
   
    public enum Mc_Status
    {
        stationary=0,
        charging=1,
        travelling=2,

    };
    public class mobile_charger : ns_node
    {
        
        

        private PriorityQueue<double, ns_node> ChargingQ;
        private PriorityQueue<double, ns_node> spatial_queue;
        private PriorityQueue<double, ns_node> temp_queue;
        public double mcv_speed = 1;
        private ns_point old_location;
        public Mc_Status MCStatus = new Mc_Status();
        private Timer ChargeProgressTimer;
        private double ChargeTimerInterval = 30;//*1000;// 30 secc;
        private ns_node current_charged_node;
        private double c_time = 0;
        private int tours_count = 0;
        private int number_of_charged_node = 0;
        private DataLogger dead_nodes;
        private DataLogger queue_len;
        // the travelling power cost
        private double mc_consumed_power_per_m = 0.05; //
        private charge_statistics stats = new charge_statistics();
        public int N_sensor = 200;
        public mobile_charger(string mob_id ) : base(mob_id)
        {

            this.ChargingQ = new PriorityQueue<double, ns_node>(Compare);
            this.NodeType = Node_TypeEnum.Mobile_Charger;
            this.MCStatus = Mc_Status.stationary;
            this.old_location = this.location;
            this.current_charged_node = new ns_node();
            this.__init_charger_Timer();
            this.c_time = 0;
            this.tours_count = 0;
            __init__();
        }

        public mobile_charger(string mob_id , double x, double y) : base(mob_id , x , y)
        {

            this.ChargingQ = new PriorityQueue<double, ns_node>(Compare);
            this.MCStatus = Mc_Status.stationary;
            this.NodeType = Node_TypeEnum.Mobile_Charger;

            this.old_location = this.location;
            this.current_charged_node = new ns_node();
            this.__init_charger_Timer();
            this.c_time = 0;
            this.tours_count = 0;
            __init__();
        }
        public mobile_charger(string mob_id, ns_point loc)
            : base(mob_id, loc)
        {

            this.ChargingQ = new PriorityQueue<double, ns_node>(Compare);
            this.NodeType = Node_TypeEnum.Mobile_Charger;
            this.MCStatus = Mc_Status.stationary;
            this.old_location = this.location;
            this.__init_charger_Timer();
            this.current_charged_node = new ns_node();
            this.c_time = 0;
            this.tours_count = 0;
            __init__();
        }
       
        private void __init_charger_Timer()
        {
            this.ChargeProgressTimer = new Timer(1*1000);
            this.ChargeProgressTimer.Elapsed += ChargeProgressTimer_Elapsed;
        }
       private double getChargeTimerInterval()
        {
          // this.current_charged_node.PowerConfig.m
            double residual_bat = this.current_charged_node.PowerConfig.BatteryCapacity;
            double charge_rate = this.current_charged_node.PowerConfig.ChargeRate;
            double max_batt = this.current_charged_node.PowerConfig.getMaxBatteryCapacity();
            double charge_time = Math.Abs(max_batt - residual_bat) / charge_rate;
            //printfx("E_r:{0} , E_max:{1} C_t:{2} S_n:{3}", residual_bat, max_batt, charge_time, this.current_charged_node.tag);
            this.ChargeTimerInterval = (int)(charge_time);
          // printfx("charge_time:{0} of node {1}", this.ChargeTimerInterval, this.current_charged_node.tag);
            return this.ChargeTimerInterval;
        }
       //private bool do_partial = false;
       
        
       
        public double optimization_constant = 4;
        private double get_adaptable_time(int count, double er, double cr, double emax)
        {
            if(count>0)
            {
                double fx = ((emax - er) / cr) * Math.Exp(-count / this.optimization_constant);
                printfx("fx_time:{0}, opconst:{1}\n", fx, this.optimization_constant);
               /*if (fx < 10) 
                {
                    emax *= 0.50;
                    return (emax - er) / cr;
                }*/
               return fx;
            }
            if (count <= 5) return (emax - er) / cr;
            
            if (count <= 10)
            {
                emax *= 0.90;
                return (emax - er) / cr;
            }
            
            if (count <= 15)
            {
                emax *= 0.85;
                return (emax - er) / cr; 
            }


            if (count <= 20)
            {
                emax *= 0.75;
                return (emax - er) / cr;
            }
            
            if (count <= 25)
            {
                emax *= 0.60;
                return (emax - er) / cr;
            }
            emax *= 0.50;
            return (emax - er) / cr;



        }

        private double calc_average_charging_time()
        {

            int count = this.ChargingQ.size();
            double residual_bat = this.current_charged_node.PowerConfig.BatteryCapacity;
            double charge_rate = this.current_charged_node.PowerConfig.ChargeRate;
            double max_batt = this.current_charged_node.PowerConfig.getMaxBatteryCapacity();
            double time = get_adaptable_time(count, residual_bat, charge_rate, max_batt);
            printfx("charge time:{0}", time);
            return time;
            
        }
        
        public void calc_power_consumed_per_distance(double distance)
        {
           double p = distance * this.mc_consumed_power_per_m;
           this.PowerConfig.BatteryCapacity -= p;
        }
        public void calc_power_consumed_in_charging()
        {
            this.PowerConfig.BatteryCapacity -= this.PowerConfig.ChargeRate;
        }

        private void ChargeProgressTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
               

                if (this.c_time >= this.ChargeTimerInterval)
                {
                    this.c_time = 0;
                    this.ChargeProgressTimer.Stop();
                    this.StopChargerTimer();
                    this.number_of_charged_node++;
                    printfx("Time's up:{0}\n", this.ChargeTimerInterval);

                    return;
                }
                this.c_time += 1;
                // if the reach max capacity stop the timer.
                if(this.current_charged_node.ChargingInProcess())
                {
                    this.c_time = 0;
                    this.number_of_charged_node++;
                    this.ChargeProgressTimer.Stop();
                    this.StopChargerTimer();
                    return;
                }
                // the power consumed during charging process.
                this.calc_power_consumed_in_charging();

            }catch (Exception ex)
            {
                printfx("ex at Timer:{0} in {1}\n", ex.Message, "ChargeProgressTimer_Elapsed");
                printfx("InnerEx:{0}, Souce:{1}\n", ex.InnerException, ex.Source);
            }
           
        }
        public int GetQueueLen()
        {
            return this.ChargingQ.size();
        }
        public int getNumberOfChargedNodes()
        {
            int ret = this.number_of_charged_node;
            this.number_of_charged_node = 0;
            return ret;
        }
        private void updatePriority()
        {
            double count = 0;
            
            this.ChargingQ.Foreach((item) =>
                {
                    double w_time = this.stats.Get(item.Value).getWaitingTime();
                    //w_time = w_time == 0 ? 1 : w_time;
                   // double en = item.Value.PowerConfig.BatteryCapacity;
                    //en = en > 0 ? en : 1;// insure that no zero 
                    var px = this.getPriority(item.Value);
                    item.priority = px;
                    count++;
                    printfx("id:{0},prio:{1},wtime:{2}", item.Value.tag, item.priority, w_time);
                });
            this.dead_nodes.Writeline(count);
        }
        /// <summary>
        /// while charging check if there is a node whose power less
        /// than the current node and the power of the current is greater than e.g 50%
        /// then quit charge and go to charge the node with min
        /// </summary>
        /// <returns> false --> to continue charging the current node
        /// true --> quit charging and go to charge the node with minimum power.
        /// </returns>
        private bool ShouldQuit()
        {
            int qsize = this.ChargingQ.size();
            //if (qsize == 1) return false;
            //updatePriority();
            ns_node current = this.current_charged_node;
            double power = current.PowerConfig.BatteryCapacity;
            bool ret = false;
            this.ChargingQ.Foreach((n) =>
                {
                    if(current.tag != n.Value.tag)
                    {
                        double np = n.Value.PowerConfig.BatteryCapacity;
                        ret |= (np < power);
                    }
                });

            return ret && (power >= 60);
        }
        public delegate void __OnChargeNodeDone(mobile_charger mc, ns_node_stats service_stats);
        public event __OnChargeNodeDone OnNodeChargeDoneStats;
        private void printOutStatistics(ns_node node)
        {
           var node_stats =  this.stats.Get(node);
           node_stats.queue_len_at = this.ChargingQ.size();
            if(this.OnNodeChargeDoneStats!=null)
            {
                this.OnNodeChargeDoneStats(this, node_stats);
            }
        }
        private string report_folder;
        public void SetReportFolder(string folder)
        {
            this.report_folder = folder;
        }
        public bool enable_threshold_switch = false;
        private void StopChargerTimer()
        {
            try
            {
                printfx("{0}", "Timer Stopped");
                this.MCStatus = Mc_Status.stationary;
                this.current_charged_node.set_charge_request_toggle();
                this.current_charged_node.NotifyChargeRequestReceived(false);
                
                this.c_time = 0;
                this.stats.Get(current_charged_node).SetChargingEndTimeNow();
                printOutStatistics(this.current_charged_node);
                if (!this.ChargingQ.IsEmpty())
                {
                    updatePriority();// very important.
                    go_to_charge();
                    //this.MCStatus = Mc_Status.travelling;
                }
                else
                {
                    if (this.ChargeProgressTimer != null)
                    {
                        this.ChargeProgressTimer.Stop();
                    }
                }
            } catch(Exception ex)
            {
                printfx("Exception at :{0} @StopTimer", ex.Message);
            }
        }
       
        private void StartChargeTimer()
        {
           
            this.ChargeTimerInterval = this.getChargeTimerInterval();
            
            this.MCStatus = Mc_Status.charging;
            this.stats.Get(this.current_charged_node).SetChargingStartTimeNow();
            if (this.ChargeProgressTimer != null)
            {
                //this.charg_timer_running = true;
                this.ChargeProgressTimer.Start();
                printfx("{0}", "Timer Start");
            }
            else
            {
                printfx("{0}", "Timer is null");
            }
        }
       
        private bool Compare(PriorityValuePair<double, ns_node> p1, PriorityValuePair<double, ns_node> p2)
        {
            return p1.priority <= p2.priority;
        }
        private bool compMax(PriorityValuePair<double, ns_node> p1, PriorityValuePair<double, ns_node> p2)
        {
            return p1.priority >= p2.priority;
        }
        private bool compMin(PriorityValuePair<double, ns_node> p1, PriorityValuePair<double, ns_node> p2)
        {
            return p1.priority <= p2.priority;
        }
        /// <summary>
        /// get the sensor node with the highest priority .
        /// 
        /// </summary>
        /// <returns></returns>
       public override void OnPacketReceivedServer(net_packet packet , ns_node from)
        {
            //printfx(" {0} Server from :{1} , type:{2}", this.tag, from.tag, packet.Type);
        }
        
       
        public void create_stats(ns_node node)
        {
            ns_node_stats stats = new ns_node_stats(node);
            stats.SetChargeReqTimeNow();
            node.FirstChargeRequestTime(true);// set the charge request time.
            this.stats.Add_item(node.tag, stats);
        }
        private void push_charging_path_mission(List<ns_node> path)
        {
            foreach(var from in path)
            {
                if (!this.Contains(from))
                {
                    double prio = this.mdistance(from) * from.PowerConfig.BatteryCapacity / from.PowerConfig.energy_consumptionRate;
                    this.ChargingQ.Enqueue(from, prio);
                    create_stats(from);
                }
            }
            if (this.MCStatus == Mc_Status.stationary)
            {
                go_to_charge();
                this.MCStatus = Mc_Status.travelling;
            }
        }
        private double init_temp_prio(ns_node from)
        {
            return from.PowerConfig.BatteryCapacity / (from.PowerConfig.energy_consumptionRate);
        }
        public override void OnPacketReceived(ns.graph.net_packet packet, ns_node from)
        {
            try
            {


                if (packet.final_dst != this.tag) return;
                int count = this.ChargingQ.size();


                if (packet.Type == packetType.ChargeRequest)
                {

                    if (from.tag != packet.src_addr)
                    {

                        from = graph_algorithms.Dijkstra_GetNode(from, packet.src_addr);

                    }
                    //int count = this.ChargingQ.size();
                    this.PowerConfig.charge_request_packets_rx += 1;
                    from.NotifyChargeRequestReceived(true);
                    printfx("{0}:Charge Request from {1} qlen:{2}", this.tag, from.tag, count);

                    if (!this.Contains(from))
                    {
                        //if(!Check(from))
                        create_stats(from);
                        var dist = this.edistance(from);
                        var temp = this.init_temp_prio(from);
                        spatial_queue.Enqueue(from, dist);
                        temp_queue.Enqueue(from, temp);
                        double prio = getPriority(from);
                        this.ChargingQ.Enqueue(from, prio);


                        // queue_len.AppendData(count);
                    }

                    if (this.MCStatus == Mc_Status.stationary)
                    {
                        go_to_charge();
                        this.MCStatus = Mc_Status.travelling;
                    }
                }


            }
            catch (Exception ex)
            {
                printfx("ex:{0} in OnPacketReceived", ex.Message);
                return;
            }

        }
        //
        public bool Contains(ns_node from)
        {
            return this.ChargingQ.Contains((p) =>
                {
                    return p.Value.tag == from.tag;
                });
        }
        public delegate void OnMCvTravelling(mobile_charger mcv);
        private OnMCvTravelling TravelHandler;
        public void SetTravelHandler(OnMCvTravelling handler)
        {
            this.TravelHandler = handler;
        }
        private bool go_back_home = false;
        public ns_node getNodeToBeCharge()
        {
            /*if(this.do_partial)
            {
                var pnode = this.ChargingQ.Peak();
                printfx("Partial Node to be Charge :{0} , priority:{1}", pnode.Value.tag, pnode.priority);

                this.current_charged_node = pnode.Value;
                return pnode.Value;
            }*/
            var node = this.ChargingQ.Dequeue();// FCFS /// first .PR Dequeue();
            //var node = this.ChargingQ.
            printfx("Node to be Charge :{0} , priority:{1}", node.Value.tag, node.priority);

            this.current_charged_node = node.Value;
           return node.Value;
        }
        /// <summary>
        /// when the charger reach the correponding node , it calls the function.
        /// </summary>
        /// <param name="node"></param>
        public void SetCurrentChargeNode(ns_node node)
        {
            this.current_charged_node = node;
            this.stats.Get(node).SetChargerReachTimeNow();
        }
        public ns_point getOriginLocation()
        {
            return this.old_location;
        }
        public bool isBackhome()
        {
            return this.go_back_home;
        }
       public void go_to_charge()
        {
            this.tours_count++;
            this.TravelHandler(this);
            
        }
        public int getTourCount()
       {
           return this.tours_count;
       }
       public override bool ChargingInProcess()
       {
         
           StartChargeTimer();
           return true;
          
       }
        public charge_statistics get_statistics()
       {
           return this.stats;
       }
        private void updateSpatialQueue()
        {
            try
            {
                this.spatial_queue.Foreach((item) =>
                    {
                        var node = item.Value;
                        var dist = this.edistance(node);
                        item.priority = dist;
                    });
            } catch (Exception ex)
            {
                printfx("ex:{0} in updateSpatialQueue", ex.Message);
                return;
            }
        }
        private void updateTempQueue()
        {
            try
            {
                this.temp_queue.Foreach((item) =>
                    {
                        var node = item.Value;
                        var wt = node.PowerConfig.BatteryCapacity / (node.PowerConfig.energy_consumptionRate);
                        item.priority = wt;
                    });
            } catch (Exception ex)
            {
                printfx("ex:{0} in updateTempQueue", ex.Message);
                return;
            }
        }
       private void __init__()
       {
           Random r = new Random(DateTime.Now.Millisecond);
           this.PowerConfig.Comm_Range = 100;
           this.PowerConfig.initial_energy = 8500;
           this.PowerConfig.PacketEnergy = 0.0001;
           this.PowerConfig.energy_consumptionRate = 0.0003;
           this.PowerConfig.energy_threshold = r.Next(30, 60);
           //this.SensingRadius = 3;
           this.PowerConfig.Temperature = r.Next(20, 40);
           this.PowerConfig.BatteryCapacity = 8500;
           this.PowerConfig.BatteryDischargeRate = 0;
           this.PowerConfig.ChargeRate = 3;
           this.PowerConfig.Sleep_mode_En = r.NextDouble() * 0.1;
           string fn_name = string.Format("{0}_dead_nodes.dat", this.tag);
           string q_fn = string.Format("{0}_queue_len.dat", this.tag);
           this.dead_nodes = new DataLogger(fn_name);
           this.queue_len = new DataLogger(q_fn, true);
           this.temp_queue = new PriorityQueue<double, ns_node>(compMax, compMin);
           this.spatial_queue = new PriorityQueue<double, ns_node>(compMax, compMin);
          // this.max_battery_capacity = 100;
       }

        private double joint_prio(ns_node n_i, int N, double alpha, double beta)
       {
           double p1 = spatial_prio(n_i, N);
           double p2 = temp_prio(n_i, N);
           // p1 = Math.ab
           double p = p1 * alpha + beta * p2 + Math.Log10(p1 * p2 + 1);

           printfx("sp:{0} , tp={1} , jp={2} , node:{3}", p1, p2, p, n_i.tag);
           return p;
       }

        private double getPriority(ns_node node)
        {
            int size = this.ChargingQ.size();
            if(size <=1)
            {
                return this.edistance(node);
            }
            updateSpatialQueue();
            updateTempQueue();
            //ns_node first = this.ChargingQ.ToList().First().Value;
           // ns_node last = this.ChargingQ.ToList().Last().Value;
            double alpha = 1.0;
            double beta = 1.0;
            int N = this.N_sensor;
            //var prio = 
            return joint_prio(node, N, alpha, beta);
        }
        private double spatial_prio( ns_node n_i,int N)
       {
           if (this.temp_queue.size() <= 1) return this.edistance(n_i);
            //updateSpatialQueue();
            var nearest = spatial_queue.TopMin();
            var far = spatial_queue.TopMax();
            

           double d_i = n_i.edistance(this);
           double d_plus = far.priority;
           double d_min = nearest.priority;

           double up = Math.Abs(d_i - d_min)*(N);
           double down = Math.Abs(d_plus - d_min);
           if (down == 0) return d_i;



           return Math.Abs(Math.Ceiling(up / down));
       }
        private double temp_prio(ns_node n_i, int N)
       {
           double t_i = n_i.PowerConfig.BatteryCapacity / (n_i.PowerConfig.energy_consumptionRate);
           if (this.temp_queue.size() <= 1) return t_i;
           updateTempQueue();
           

           var first = temp_queue.TopMax().priority;
           var last = temp_queue.TopMin().priority;
            if( first == last)
            {
                return t_i;
            }
            //var node_stats =  this.stats.Get(last);
            double t_plus = first;
           // node_stats = this.stats.Get(first);
            double t_min = last;
            if (t_min == t_plus) return t_i;
            double lambda_i = Math.Ceiling(Math.Abs(t_i - t_min) * N / Math.Abs(t_plus - t_min));
           // printfx("temp_prio:{0}-->{1}", n_i.tag, lambda_i);
           return lambda_i;
       }

    }
}
