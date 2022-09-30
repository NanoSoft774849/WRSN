using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace ns.wrsn
{
    public class base_station : ns_node
    {

        public base_station (string  bs_id):base(bs_id)
        {
            this.Radius = 15;
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
        }
        public base_station (string bs_id , double x , double y ) : base(bs_id, x, y)
        {
          
            this.Radius = 8;
            this.NodeType = Node_TypeEnum.BaseStation;
            __init__();
        }
        public base_station (string bs_id , ns_point bs_loc) : base ( bs_id, bs_loc)
        {
            this.Radius = 8;
            this.NodeType = Node_TypeEnum.BaseStation;
            __init__();
        }
        public override void OnPacketReceivedServer(net_packet packet, ns_node from)
        {
            //base.OnPacketReceivedServer(packet, from);
            // printfx("server Packet received from :{0} at {1}", packet.src_addr, packet.dst_addr);
           /* if (packet.Type == packetType.ChargeRequest)
            {
                packet.src_addr = this.tag;
                this.sendChargeRequestPacket(packet);
                return;
            }
            if (packet.Type == packetType.Data)
            {
                if (packet.final_dst == this.tag)
                {
                   // printfx("Packet received from :{0} at {1}", packet.end_src, this.tag);
                    return;
                }
               // packet.src_addr = this.tag;
                //this.ns_send_packet(packet);
                //this.ns_
            }*/
        }
        public override void OnPacketReceived(graph.net_packet packet, ns_node from)
        {
            // printfx("Packet received from :{0} at {1}", packet.src_addr, packet.dst_addr);
           /* if (packet.Type == packetType.ChargeRequest)
            {
                packet.src_addr = this.tag;
                this.sendChargeRequestPacket(packet);
                return;
            }
            if (packet.Type == packetType.Data)
            {
                if (packet.final_dst == this.tag)
                {
                    //printfx("Packet received from :{0} at {1}", packet.end_src, this.tag);
                    return;
                }
               // packet.src_addr = this.tag;
                //this.ns_send_packet(packet);
                //this.ns_
            }*/
        }
    }
}
