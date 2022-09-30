
//#define UAV
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
using System.Timers;

namespace ns.wrsn
  
{
   
    public enum Mc_Status
    {
        stationary=0,
        charging=1,
        travelling=2,

    };
    public class mobile_charger : ns_node
    {
        
        
        /// <summary>
        /// a queue for storing charge requests.
        /// </summary>
        private PriorityQueue<double, ns_node> ChargingQ;
        private ns_point old_location;
        public double mcv_speed = 5;
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
        public bool enable_load_balance_algorithm = true;
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
            /// 1 sec -> 1 joule / joule/sec
            this.ChargeProgressTimer = new Timer(1*1000); // 1 s = 1000 ms;
            // timer routine - > 1 sec -
            this.ChargeProgressTimer.Elapsed += ChargeProgressTimer_Elapsed;
           // this.ChargeProgressTimer.Elapsed+=ChargeProgressTimer_Elapsed;
        }
       private double getChargeTimerInterval()
        {
          // this.current_charged_node.PowerConfig.m
           /// remaining energy
            double residual_bat = this.current_charged_node.PowerConfig.BatteryCapacity;
           // how many joules per second.
            double charge_rate = this.current_charged_node.PowerConfig.ChargeRate;
           // 1000
            double max_batt = this.current_charged_node.PowerConfig.getMaxBatteryCapacity();
            double charge_time = Math.Abs(max_batt - residual_bat) / charge_rate;// second
            //printfx("E_r:{0} , E_max:{1} C_t:{2} S_n:{3}", residual_bat, max_batt, charge_time, this.current_charged_node.tag);
            this.ChargeTimerInterval = (int)(charge_time);
          // printfx("charge_time:{0} of node {1}", this.ChargeTimerInterval, this.current_charged_node.tag);
            return this.ChargeTimerInterval;
        }
       private bool do_partial = false;
        private double getAveragePowerInQ()
       {
           double count = 0.0;
           double sum_p = 0.0;
           
           this.ChargingQ.Foreach((item) =>
               {
                   sum_p += item.Value.PowerConfig.BatteryCapacity;
                   
                   count++;
               });
           
           return (sum_p) / (count);
       }
        private double getAverageThreshold()
        {
            double count = 0.0;
            double sum_p = 0.0;

            this.ChargingQ.Foreach((item) =>
            {
                sum_p += item.Value.PowerConfig.energy_threshold;

                count++;
            });

            return (sum_p) / (count);
        }
        private double get_charge_time_guassian(List<double> ar, double cr)
       {
           double res = 0.0;
           int count = ar.Count;
           double[] win = ns.graph.ns_window_functions.Guassian_window(count, 0.45, cr);
           double mu = ns_window_functions.Mean(win);
           int i = 0;
           double before = 0.0;
           printfx("guassian_mean:{0}", mu);
            foreach(double x in win)
            {
                res += (ar[i] * x);
                before += ar[i] / (cr);
                i++;
            }
            res = res / (count * mu * cr);
            before = before / (count * mu);
            
            printfx("Guassian_time:{0}, before:{1} ,count:{2},partial:{3}\n", res, before, count, this.do_partial);

           res = Math.Min(before, res);

            return res;
       }
        public double optimization_constant = 54.6;
        private double get_adaptable_time(int count, double er, double cr, double emax)
        {
            double max_av = count /this.optimization_constant;
            if(count>=0)
            {
               // double f = count;// Math.Max(count, er);
                double fx = ((emax - er) / cr) * Math.Exp(-max_av);
                printfx("fx_time:{0}, opconst:{1}\n", fx, max_av);
               
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
#if UAV
        //// The energy consumed in hovering
        public double hovering_energy = 1;// 1 joule/sec

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#endif
        private void ChargeProgressTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
               /// UAV -> Hovering 
               /// Hovering = charging time.
                /// 
                if (this.c_time >= this.ChargeTimerInterval)
                {
                #if UAV
                    this.PowerConfig.BatteryCapacity -= (this.hovering_energy * (this.c_time));
                #endif
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
        private string prio_fn = "prio.dat";
        private void to_latex_table(string str)
        {
            System.IO.File.AppendAllText(prio_fn, str);

        }
        private void updatePriority()
        {
            double count = 0;
            string str = "";
            this.ChargingQ.Foreach((item) =>
                {
                    double w_time = this.stats.Get(item.Value).getWaitingTime();
                    //w_time = w_time == 0 ? 1 : w_time;
                    double en = item.Value.PowerConfig.BatteryCapacity;
                    en = en > 0 ? en : 1;// insure that no zero 
                    double dist = this.edistance(item.Value);

                    /// distance * energy/ waiting_time
                    var px = dist ;/// (w_time);
                    /// scheduling policy
                    item.priority = px;
                    count++;
                    //{0:0.000},{1:0.000}
                    printfx("id:{0},prio:{1},wtime:{2}", item.Value.tag, item.priority, w_time);
                    str += string.Format("${0}$ & {1:0.00} & {2:0.00} & {3:0.00} & {4:0.00} \\\\ \n", item.Value.tag, en, dist, w_time, px);
                });
            this.dead_nodes.Writeline(count);
            to_latex_table(str);
            to_latex_table("---------------------\n");
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
                /*if (this.enable_threshold_switch)
                {
                    double len = this.ChargingQ.size();
                    double bc = this.current_charged_node.PowerConfig.BatteryCapacity;
                    double new_thres = bc * Math.Exp(-len / this.optimization_constant);
                    double w_thr = this.current_charged_node.PowerConfig.threshold_switch;
                    this.current_charged_node.PowerConfig.energy_threshold = w_thr;
                }*/
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
            //this.__init_charger_Timer();
            //getChargeTimerInterval();
            // Joule/s -> s/Joule
            this.ChargeTimerInterval = this.calc_average_charging_time();
            //if (this.ChargeTimerInterval <= 5) this.ChargeTimerInterval = 20;
           // printfx("ChargeTimerInterval:{0}\n", this.ChargeTimerInterval);
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
        private void onDatePacket(net_packet packet)
        {
            
            if (packet.Type == packetType.Data)
            {
                if (packet.final_dst == this.tag)
                {
                    printfx("Packet received from :{0} at {1}", packet.end_src, this.tag);
                    return;
                }
                //packet.src_addr = this.tag;
                this.ns_send_packet(packet);
                //this.ns_
            }
        }
        private bool load_balance_accept(graph.net_packet packet)
        {
            if (packet.Type != packetType.ChargeRequest)
            {
                onDatePacket(packet);
                return false;
            }
            if (!this.enable_load_balance_algorithm) return true;// accept.

            int count = this.ChargingQ.size();
            int qlen = packet.qlen;
            if(qlen == int.MaxValue && count == 0)
            {
                return true; // accept;
            }
            if (qlen == count)
            {
                printfx("Electing Myself...{0}:{1}", count, this.tag);
                return true;// accept elects itself as leader.
            }
            if (qlen > count) return false;
            if(qlen < count )
            {
                packet.qlen = count;
                printfx("ReSend Charge Request...{0}:{1}", count, this.tag);
                this.sendChargeRequestPacket(packet);
                return false;
            }
            return false;
        }
        // charinging requests handler.
        public override void OnPacketReceived(graph.net_packet packet , ns_node from )
        {
           // base.OnPacketReceived(packet);
           // printfx("MC HAS Receivd MSg from :{0}--Node:{1}", packet.src_addr , from.tag);
           // if (!load_balance_accept(packet)) return;
            if (packet.final_dst != this.tag) return;
            int count = this.ChargingQ.size();
          
            if (packet.Type == packetType.ChargeRequest)
            {
                
                if(from.tag != packet.src_addr)
                {
                   
                    from = graph_algorithms.Dijkstra_GetNode(from, packet.src_addr);
                   
                }
                //int count = this.ChargingQ.size();
                this.PowerConfig.charge_request_packets_rx += 1;
                // E_r <= E_th
                // to avoid 
                from.NotifyChargeRequestReceived(true);
                printfx("{0}:Charge Request from {1} qlen:{2}", this.tag, from.tag, count);
                string str = "";
                if (!this.Contains(from))
                {
                    //if(!Check(from))
                    // distance * E_r 
                    // Min Priority -> value min
                    // 3 
                    double prio = this.edistance(from) * from.PowerConfig.BatteryCapacity;
                    // *from.PowerConfig.energy_consumptionRate;
                    str = string.Format("${0}$ & {1:0.00} & {2:0.00} & {3:0.00} & {4:0.00} \\\\ \n", from.tag, from.PowerConfig.BatteryCapacity,
                        this.edistance(from), 1, prio);
                    to_latex_table(str);
                    //
                    this.ChargingQ.Enqueue(from, prio);
                    create_stats(from);
                   
                   // queue_len.AppendData(count);
                }

                if (this.MCStatus == Mc_Status.stationary)
                {
                    go_to_charge();
                    this.MCStatus = Mc_Status.travelling;
                }
            }
           
        }
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
            // Select the node with highest priority.
            // Queue
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
           System.IO.File.WriteAllText(this.prio_fn, "");
          // this.max_battery_capacity = 100;
       }

    }
}
