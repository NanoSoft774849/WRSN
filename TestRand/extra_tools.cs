using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace TestRand
{
     public class hex_node_struct
        {
            public double q;
            public double r;
            public ns_point loc;
            public int i_index;
            public int j_index;
            public string tag;
            public hex_node_struct(double _q , double _r , ns_point _p)
            {
                this.q = _q;
                this.r = _r;
                this.loc = _p;
            }
        }
    public class topo_construct
    {

        public topo_construct()
        {

        }
        public static hex_node_struct get_bs(List<hex_node_struct> list)
        {
            foreach( var t in list)
            {
                if (t.j_index == t.i_index && t.i_index ==0) return t;
            }
            return null;
        }

        public static List<hex_node_struct> sort_topo(List<hex_node_struct> list, double cr)
        {
            PriorityQueue<double, hex_node_struct> frontier = new PriorityQueue<double, hex_node_struct>((p1, p2) =>
            {
                return p1.priority <= p2.priority;
            });
            hex_node_struct bs = get_bs(list);
            frontier.Enqueue(bs, 0);
            double cost = 0;
            Dictionary<string, double> cost_so_far = new Dictionary<string, double>();
            cost_so_far[bs.tag] = 0;
            List<hex_node_struct> net = new List<hex_node_struct>();
            net.Add(bs);
            Dictionary<string, hex_node_struct> parentof = new Dictionary<string, hex_node_struct>();
            while(!frontier.IsEmpty())
            {
                var current = frontier.First().Value;

                foreach( var point in list)
                {
                    var next = point;
                    if (next.tag == current.tag) continue;
                    cost = next.loc.edistance(current.loc);
                    if (cost <= cr && !cost_so_far.ContainsKey(next.tag))
                    {
                        cost_so_far[next.tag] = cost;
                        double priority = cost;
                        string old_tag = next.tag;

                        frontier.insert(next, priority);
                        net.Add(next);
                        parentof[old_tag] = current;
                        
                       // ns_link.RenderLink(current, next, cost, mcanvas.Children);
                        //current.addLink(next, cost);
                    }



                }
            }
            return net;
        }
    }
}
