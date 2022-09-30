//using ns.graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.graph
{
    public class ns_charge_average
    {
        public double average_response_time;
        public double average_power;
        public double average_charge_duration;
        public double average_charge_rate;
        public string node_id;
        public ns_charge_average(string node_id)
        {
            this.average_charge_duration = 0;
            this.average_charge_rate = 0;
            this.average_power = 0;
            this.average_response_time = 0;
            this.node_id = node_id;
        }
        public ns_charge_average FromStats(List<ns_node_stats> stats)
        {
            double m = 0.0;// / stats.Count;
            foreach (var item in stats)
            {
                if ((item.isCharged))
                {
                    this.average_response_time += item.getResponseTime();
                    this.average_power += item.power_at_charge_end;
                    this.average_charge_rate += item.getChargeRatio();
                    this.average_charge_duration += item.getChargingDuration();
                    item.isRead = true;
                    m++;
                }
            }
            return this.divideBy(m);
        }
        public ns_charge_average FromStats(Queue<ns_node_stats> stats)
        {
            double m = 0.0;// / stats.Count;
            int count = stats.Count;
            int i = 0;
            while (stats.Count > 0)
            {
                var item = stats.Peek();
                if ((item.isCharged))
                {
                    this.average_response_time += item.getResponseTime();
                    this.average_power += item.power_at_charge_end;
                    this.average_charge_rate += item.getChargeRatio();
                    this.average_charge_duration += item.getChargingDuration();
                    item.isRead = true;
                    m++;
                    stats.Dequeue();
                }
                if (i >= count) break;
                i++;
            }


            return this.divideBy(m);
        }
        public string ExportAsString()
        {
            string str = "";
            str = string.Format("{4}\t{0}\t{1}\t{2}\t{3}\n",
                this.average_response_time, this.average_charge_duration, this.average_charge_rate,
                this.average_power, this.node_id);
            return str;
        }
        public string GetAsString()
        {
            string str = "";
            str = string.Format("{0}\t{1}\t{2}\t{3}\n",
                this.average_response_time, this.average_charge_duration, this.average_charge_rate,
                this.average_power);
            return str;
        }
        public ns_charge_average divideBy(double value)
        {
            if (value == 0 || value == 1) return this;
            this.average_charge_duration /= value;
            this.average_charge_rate /= value;
            this.average_power /= value;
            this.average_response_time /= value;
            return this;
        }
    }
    public class charge_statistics
    {

        public Dictionary<string, ns_node_stats> Stats;

        public charge_statistics()
        {
            this.Stats = new Dictionary<string, ns_node_stats>();
        }

        public bool contains(string id)
        {
            return this.Stats.ContainsKey(id);
        }
        public bool contains(ns_node node)
        {
            return contains(node.tag);
        }
        public void Add_item(string id, ns_node_stats node_stats)
        {

            if (this.contains(id))
            {

                this.Stats[id] = node_stats;
                this.Stats[id].round_count += 1;
                return;
            }
            node_stats.round_count = 1;
            this.Stats[id] = node_stats;
        }
        public ns_node_stats Get(ns_node id)
        {
            if (this.contains(id))
            {

                return this.Stats[id.tag];
            }
            ns_node_stats stats = new ns_node_stats(id);
            this.Add_item(id.tag, stats);
            return stats;
        }
        public ns_node_stats GetNodeStats(string id)
        {
            if (!this.contains(id)) return null;
            return this.Stats[id];
        }

        public int GetCount()
        {
            return this.Stats.Count;
        }




    }
    public class ns_node_stats
    {
        public string node_id { get; set; }
        public int round_count;
        /// <summary>
        /// The time the mobile charger receive a charge request from the corresponding node.
        /// </summary>
        public DateTime charge_req_time { get; set; }
        /// <summary>
        ///The time that the MCV reach the corresponding node.
        /// </summary>
        public DateTime charger_reach_time { get; set; }
        /// <summary>
        /// The time taken by mCV to charge the correponding node.
        /// </summary>
        public DateTime charging_start_time { get; set; }
        /// <summary>
        /// Charge tour
        /// </summary>
        public int charge_tour { get; set; }

        public bool isRead { get; set; }
        /// <summary>
        /// the time when MCV ends charging the correponding node.
        /// </summary>
        public DateTime charging_end_time { get; set; }
        public double power_at_charge_start { get; set; }
        public double power_at_charge_end { get; set; }
        public bool isCharged { set; get; }
        public bool reachThere { get; set; }
        private ns_node theNode;
        public int queue_len_at { get; set; }

        public ns_node_stats(ns_node node)
        {
            this.node_id = node.tag;
            this.theNode = node;
            this.isRead = false;
            this.round_count = 0;
            this.queue_len_at = 0;
        }
        /// <summary>
        /// the time the customer arrived at the queue.
        /// </summary>
        public void SetChargeReqTimeNow()
        {
            this.charge_req_time = DateTime.Now;
        }
        public void SetChargerReachTimeNow()
        {
            this.charger_reach_time = DateTime.Now;
            this.reachThere = true;
        }
        public void SetChargingStartTimeNow()
        {
            this.charging_start_time = DateTime.Now;
            this.power_at_charge_start = this.theNode.PowerConfig.BatteryCapacity;
        }
        public void SetChargingEndTimeNow()
        {
            this.charging_end_time = DateTime.Now;
            this.power_at_charge_end = this.theNode.PowerConfig.BatteryCapacity;
            this.isCharged = true;
        }
        /// <summary>
        /// the time that the node is waited to its turn in the queue;
        /// </summary>
        public double getWaitingTime()
        {
            var resp_time = DateTime.Now.Subtract(this.charge_req_time);
            return resp_time.TotalSeconds;
        }
        /// <summary>
        /// the time the customer being the queue, before he got served.
        /// </summary>
        /// <returns></returns>
        public double getWaitingInQueueTime()
        {
            var resp_time = this.charging_start_time.Subtract(this.charge_req_time);
            return resp_time.TotalSeconds;
        }
        /// <summary>
        ///  time interval that starts from
        //WCV receiving a charging request and ends when the WCV
        //arrives at the corresponding node.
        /// </summary>
        /// <returns></returns>
        public double getResponseTime()
        {
            if (!this.reachThere) return 0;
            var resp_time = this.charger_reach_time.Subtract(this.charge_req_time);
            return resp_time.TotalSeconds;
        }
        public double getChargingDuration()
        {
            if (!this.isCharged) return 0;
            var duration = this.charging_end_time.Subtract(this.charging_start_time);
            return duration.TotalSeconds;
        }
        public double getChargeRatio()
        {
            if (!this.isCharged) return 0;
            return this.power_at_charge_end - this.power_at_charge_start;
        }
        public double getArrivalTime()
        {
            var du = DateTime.Now.Subtract(this.charge_req_time).TotalSeconds;
            return du;
        }
        public string ExportAsString()
        {
            string str = "";
            str = string.Format("{0}\t{1}\t{2}\t{3}\n", this.getResponseTime(), this.getChargingDuration(),
                this.getChargeRatio(), this.power_at_charge_end);
            return str;
        }
    }
}
