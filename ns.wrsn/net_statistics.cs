using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace ns.wrsn
{
    public class net_statistics
    {
        public PriorityQueue<double, net_packet> ReceivedPackets;
        public int ReceivedPacketsCount;
        public PriorityQueue<double, net_packet> PacketsReadyToTransmit;
        public int TransmittedPacketsCount;

        public net_statistics ( )
        {
            this.PacketsReadyToTransmit = new PriorityQueue<double, net_packet>((p1, p2) =>
                {
                    return p1.priority >= p2.priority;
                });
            this.ReceivedPackets = new PriorityQueue<double, net_packet>((p1, p2) =>
            {
                return p1.priority >= p2.priority;
            });
            this.ReceivedPacketsCount = 0;
            this.TransmittedPacketsCount = 0;
        }
        public net_packet NextToTransmit()
        {
            return this.PacketsReadyToTransmit.Dequeue().Value;
        }
        public void AddPacketToTransmit(net_packet packet , double prio)
        {
            this.PacketsReadyToTransmit.Enqueue(packet, prio);
            this.TransmittedPacketsCount++;
        }
        public void AddReceivedPacket(net_packet packet , double prio)
        {
            this.ReceivedPackets.Enqueue(packet, prio);
            this.ReceivedPacketsCount++;
           // packet.OnReceive();
        }
    }
}
