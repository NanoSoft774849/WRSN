using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
using System.Timers;
namespace Nayab
{
    public class sensor_node : ns_node
    {

        
        public double SensingRadius { get; set; }
        private double max_battery_capacity;
        
       
        public sensor_node()
            : base()
        {
            
           // this.net_queues = new net_statistics();
            this.NodeType = Node_TypeEnum.Sensor_node;
        }
        public sensor_node(string node_id)
            : base(node_id)
        {
           
            this.NodeType = Node_TypeEnum.Sensor_node;
            __init__();
        }
        public sensor_node(string node_id, double x, double y)
            : base(node_id, x, y)
        {
            
            this.NodeType = Node_TypeEnum.Sensor_node;
            __init__();

        }
        public sensor_node(string node_id, ns_point location)
            : base(node_id, location)
        {
           // this.mTimer = new Timer();
           
            this.NodeType = Node_TypeEnum.Sensor_node;
            __init__();

        }
        
        private void __init__()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            this.PowerConfig.Comm_Range = 100;
            this.PowerConfig.initial_energy = 100;
            this.PowerConfig.PacketEnergy = 0.12;
            this.PowerConfig.energy_consumptionRate = 0.02;
            this.PowerConfig.energy_threshold = r.Next(20, 40);
            this.SensingRadius = 3;
            this.PowerConfig.Sleep_mode_En = 0.004;
            this.PowerConfig.Temperature = r.Next(20, 40);
            this.PowerConfig.BatteryCapacity = 100;
            this.PowerConfig.BatteryDischargeRate = r.NextDouble();
            this.PowerConfig.ChargeRate = 1.5;
            this.max_battery_capacity = 100;
        }
        private void __init__power_timer()
        {
           // this.PowerMonitorTimer = new Timer(this.PowerTimerInterval);
        }

        private Dictionary<string, mobile_charger> MobileChargers;
        public void SetMobileChargers(Dictionary<string,mobile_charger> mcs)
        {
            this.MobileChargers = mcs;
        }

        

       
       
        public void Shutdown()
        {
            this.Stop();
        }
        private void send_the_charge_request(List<ns_node> path)
        {
            if (path != null)
            {
                ns_node first = this;
                string last = path.Last().tag;
                foreach (var node in path)
                {
                    if (first.tag == node.tag) continue;
                    //printfx("{0} next :{1}", this.tag, node.tag);
                    net_packet packet = new net_packet(this.tag, node.tag, packetType.ChargeRequest, last);

                    packet.data = "Charge Request from " + this.tag;

                    first.SendPacket(packet);
                    first = node;
                    //if (node.NodeType == Node_TypeEnum.Mobile_Charger) break;
                }

            }
        }
         public void XOnExPowerDown()
        {
            try
            {
                base.OnExPowerDown();
                net_packet packet = new net_packet(this.tag, "", packetType.ChargeRequest);
                
                packet.data = "Charge Request from " + this.tag;

                this.SendPacket(packet);
                this.PowerConfig.charge_request_packets_tx += 1;
            }
             catch (Exception ex)
            {
                printfx("Exception onExPower:{0}:{1}", this.tag, ex.Message);
            }
        }
        public override void OnExPowerDown()
        {
            
           
           // printfx("Can't find MC {0}", this.tag);
            try
            {
                base.OnExPowerDown();
                ns_node mc = GetNearestMc();
                if(mc.tag != string.Empty)
                {
                    var fxpath = graph_algorithms.Dijkstra(this, mc);
                    if (fxpath != null)
                    {
                        send_the_charge_request(fxpath);
                        printfx("from GetNearestCar:{0}-{1}", this.tag, mc.tag);
                        return;
                    }
                   
                }
                List < ns_node > path = graph_algorithms.Dijkstra_GetNearestMC(this);
                //var item = path.Last();
                send_the_charge_request(path);
            } catch(Exception ex)
            {
                printfx("Error in OnFxPower:{0}-{1}", this.tag, ex.Message);
            }
           
        }
       private mobile_charger GetNearestMc()
        {
            if (this.MobileChargers != null && this.MobileChargers.Count > 0) 
            {
                
                int min_qlen = 0;
                int i = 0;
                string mc_id ="";

                foreach (var mc in this.MobileChargers)
                {
                    var node = mc.Value;
                    if (node.PowerConfig.BatteryCapacity > 300)
                    {
                        if (i == 0)
                        {

                            min_qlen = node.GetQueueLen();
                            i++;
                            mc_id = node.tag;
                            continue;
                        }
                        int qlen = node.GetQueueLen();
                        min_qlen = qlen <= min_qlen ? qlen : min_qlen;
                        mc_id = min_qlen == qlen ? node.tag : mc_id;
                        i++;
                    }
                }
                return this.MobileChargers[mc_id];
            }
            return new mobile_charger(string.Empty) ;
        }
        public override double getLinkCost(ns_node dst)
        {
            if(dst.NodeType == Node_TypeEnum.Mobile_Charger)
            {
                mobile_charger mc = dst as mobile_charger;
                return mc.GetQueueLen();
            }
           return Math.Abs(this.edistance(dst));
           //return Math.Abs(this.PowerConfig.BatteryCapacity - dst.PowerConfig.BatteryCapacity);// base.getLinkCost(dst);
        }
        public override bool ChargingInProcess()
        {
            //printfx("node {0} is charged", this.tag);
            //this.PowerConfig.BatteryCapacity += this.PowerConfig.;
           // ChargeDone();
            //StartPowerTimer();// again waatch.
           // set_charge_request_toggle();
            this.PowerConfig.BatteryCapacity += this.PowerConfig.ChargeRate;
            printfx("{0} \t {1}%", this.tag, this.PowerConfig.BatteryCapacity);
            if (this.PowerConfig.BatteryCapacity >= this.max_battery_capacity) 
            {
                return true;
            }

            return false;
        }
        
    }
}
