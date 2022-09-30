
#define use_sectors
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace NPDC
{
    /// <summary>
    /// Controller for task scheduling and charge requets stats.
    /// </summary>
    public class base_station : ns_node
    {
        /// <summary>
        /// using dictionary better than list for fast access.
        /// </summary>
        public Dictionary<string,mobile_charger> MCVs;
        public ns_task_set charging_tasks;
        private charge_statistics stats = new charge_statistics();
        public double Optimization_constant = 10;
        /// <summary>
        /// this indicates, that revisting the nodes with minimum energy.
        /// </summary>
        public bool set_maximize_energy_target = true;
        public base_station (string  bs_id):base(bs_id)
        {
            this.Radius = 15;
            this.NodeType = Node_TypeEnum.BaseStation;
            
            __init__();
        }
        public base_station(string bs_id, double x, double y)
            : base(bs_id, x, y)
        {

            this.Radius = 8;
            this.NodeType = Node_TypeEnum.BaseStation;
            __init__();
        }
        public base_station(string bs_id, ns_point bs_loc)
            : base(bs_id, bs_loc)
        {
            this.Radius = 8;
            this.NodeType = Node_TypeEnum.BaseStation;
            __init__();
        }

        private void __init__()
        {
            this.PowerConfig.Comm_Range = 200;
            this.PowerConfig.initial_energy = 1000;
            this.PowerConfig.PacketEnergy = 0.0001;
            this.PowerConfig.energy_consumptionRate = 0.001;
            this.PowerConfig.energy_threshold = 20;
            this.PowerConfig.BatteryCapacity = 5000;
            this.PowerConfig.BatteryDischargeRate = 0.5;
            this.PowerConfig.Temperature = 30;

            this.MCVs = new Dictionary<string, mobile_charger>();
            this.stats = new charge_statistics();
            this.charging_tasks = new ns_task_set();

        }
        //------------------- init done--------
        public void AddMCV(mobile_charger mcv)
        {
            mcv.SetBS(this);
            this.MCVs.Add(mcv.tag, mcv);

        }
        /// <summary>
        /// Here is the function for creating and assigning charging tasks.
        /// </summary>
        /// <param name="from"></param>
        private void AddChargeTask(ns_node from)
        {
            ns_charge_task task = new ns_charge_task(from);
            this.charging_tasks.addTask(task);
            this.Assign_charge_tasks();
        }
        /// <summary>
        /// Assign charge tasks to the MCVs.
        /// 
        /// </summary>
        private void Assign_charge_tasks()
        {
            #if use_sectors
            this.charging_tasks.AssignTasks_BySectorDirectPush(this.MCVs);
            this.ReleaseTasks();
            return;
            #endif
        #if use_layers
           
            this.charging_tasks.AssignTasks_ByOrder(this.MCVs);
           this.ReleaseTasks();
           // this.charging_tasks.AssignSingleTaskFx()
         #endif
        }
        private void ReleaseTasks()
        {
            foreach(var mcv in this.MCVs)
            {
                mcv.Value.ReleaseTasks();
            }
        }
        public void create_stats(ns_node node)
        {
            ns_node_stats stats = new ns_node_stats(node);
            stats.SetChargeReqTimeNow();
            node.FirstChargeRequestTime(true);// set the charge request time.
            this.stats.Add_item(node.tag, stats);
            AddChargeTask(node);
        }
        public override void OnPacketReceivedServer(net_packet packet, ns_node from)
        {
            if(packet.Type == packetType.ChargeRequest)
            {
                this.OnChargeRequestReceived(packet, from);
                return;
            }
            
        }
        private ns_node getActualNode(net_packet packet, ns_node from)
        {
            if(packet.src_addr != from.tag)
            {
              return graph_algorithms.Dijkstra_GetNode(from, packet.src_addr);
            }
            return from;
        }
        private void OnChargeRequestReceived(net_packet packet, ns_node from)
        {
           
            from = getActualNode(packet, from);
            printfx("BS:charge request:{0}", from.tag);
            this.PowerConfig.charge_request_packets_rx += 1;
            from.NotifyChargeRequestReceived(true);// very important.
            AddChargeTask(from);

        }
        /// <summary>
        /// When the MCV complete to charge a node.
        /// instead of update Priorities BS will do that for you man.
        /// but, what's the purpose.
        /// When MCV go to charge a node. then next assigned node should be near to the MCV in order
        /// to increase the MCV efficiency.
        /// </summary>
        /// <param name="mcv"></param>
        public void Reassign_NewTasks(mobile_charger mcv)
        {
#if use_layers
            this.charging_tasks.AssignSingleTaskFx(mcv, this.MCVs);
#endif
#if use_sectorsx
            this.charging_tasks.AssignSingleTaskFxUseSector(mcv, this.MCVs);
#endif
            return; // disable this function.
        }
        /// <summary>
        /// Push a charge request a function called from Sensor Node.
        /// When its energy reaches the the threshold value.
        /// </summary>
        /// <param name="from"></param>
        public void PushChargeRequest(ns_node from)
        {
            printfx("BS:charge request:{0}", from.tag);
            this.PowerConfig.charge_request_packets_rx += 1;
            from.NotifyChargeRequestReceived(true);// very important.
            AddChargeTask(from);
        }
        /// <summary>
        /// When an MCV has completed all the tasks assigned to it.
        /// it will ask the BS if there are nodes to be charged.
        /// </summary>
        /// <param name="mcv">the MCV</param>
        public void OnChargeTasksCompleted(mobile_charger mcv , ns_node node)
        {
           /* int qlen = mcv.GetQueueLen();
            if(qlen == 0)
            {
                printfx("{0} OnChargeTasksCompleted", mcv.tag);
                // when the MCV has completed all the tasks assigned to it.
                // re check the availabe near nodes to me, and assign them
                /// look for nodes in My layer that has not charged yet.
                /// 
              //  this.charging_tasks.AssignSingleTask(mcv, this.MCVs);
                this.charging_tasks.AssignSingleTaskFx(mcv, this.MCVs);
                return;
            }*/
          
            Assign_charge_tasks();
        }
        /// <summary>
        /// When the correponding node is charged, this function will be called in the MCV class.
        /// To notify that a node is already charged.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mcv"></param>
        public void NotifyNodeCharged(ns_node node, mobile_charger mcv)
        {
            this.charging_tasks.UpdateTaskStatus(node, ns_task_status.completed);
            // Reassign_NewTasks(mcv);
            //this.charging_tasks.AddNearestTask(mcv, this.MCVs);

        }
        /// <summary>
        /// Notify the current status of the task.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mcv"></param>
        /// <param name="status"></param>
        public void NotfiyStatus(ns_node node, mobile_charger mcv, ns_task_status status)
        {
            this.charging_tasks.UpdateTaskStatus(node, status);
        }
        public override void OnPacketReceived(net_packet packet, ns_node from)
        {
            if(packet.Type == packetType.ChargeRequest)
            {
                this.OnChargeRequestReceived(packet, from);
                return;
            }
            
        }
    }
}
