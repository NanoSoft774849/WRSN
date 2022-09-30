using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
 
namespace ns.graph
{
    public enum packetType
    {
        forward =0,
        ChargeRequest =1,
        Data  =2,
        assign_threshold=3,
        assign_importance =4,
    };
    public enum packetDirection
    {
        forward = 0,
        backward =1,
    }
   public class net_packet
    {
       public string src_addr;
       public string end_src = string.Empty;
       public string dst_addr;
       public string next_hop_addr;
       public string final_dst;
       public packetType Type;
       public packetDirection PacketDirection;
       public object data;
       private bool is_null;
       public int hop_count;
       public int numberofNodes;
       public int child_index;
       public int qlen;
       public double path_cost = 0;
       public int importance = 1;
       public net_packet()
       {
           this.is_null = true;
       }
       public bool isNull()
       {
           return this.is_null;
       }
       public net_packet(string src, string dst , packetType type)
       {
           this.src_addr = src;
           this.end_src = src;
           this.dst_addr = dst;
           this.Type = type;
           this.is_null = false;
           this.hop_count = 0;
           this.numberofNodes = 0;
           child_index = 0;
           this.qlen = int.MaxValue;
           this.PacketDirection = packetDirection.forward;
       }
       public net_packet(string src, string dst , packetType type, string fn_dst)
       {
           this.src_addr = src;
           this.dst_addr = dst;
           this.Type = type;
           this.is_null = false;
           this.final_dst = fn_dst;
           this.hop_count = 0;
           child_index = 0;
           this.qlen = int.MaxValue;
           this.PacketDirection = packetDirection.forward;

       }
       public net_packet(string src, string dst , object payload)
       {
           this.src_addr = src;
           this.dst_addr = dst;
           this.Type = packetType.Data;
           this.data = payload;
           this.is_null = false;
           this.qlen = int.MaxValue;
           this.PacketDirection = packetDirection.forward;

       }
       public void SetNumberofNodes(int N)
       {
           this.numberofNodes = N;
       }
       public void updatePathCost(ns_node n1, ns_node n2)
       {

       }
       public override string ToString()
       {
           return string.Format("src:{0} dst:{1} type:{2} data:{3}", this.src_addr, this.dst_addr, this.Type, this.data);
       }
       public string Encode()
       {
           string str = "";
           try
           {
               str = JsonConvert.SerializeObject(this);
           }
           catch (Exception)
           {
               return string.Empty;
           }
           return str;
       }
       public static net_packet Decode(string msg)
       {
           net_packet packet = new net_packet();
           try
           {
               packet = JsonConvert.DeserializeObject<net_packet>(msg);
               packet.is_null = false;
           }
           catch(Exception )
           {
               return packet;
           }
           return packet;
       }
    }
}
