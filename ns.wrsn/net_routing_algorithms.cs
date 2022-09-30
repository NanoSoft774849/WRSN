using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace ns.wrsn
{
    public class node_cmp : IComparer<ns_node>
    {

        public int Compare(ns_node n1, ns_node n2)
        {
            if (n1 == n2)
            {
                return 0;
            }
            return 1;
        }
    }
    public class net_routing_algorithms
    {

       
        public delegate double heuristic(ns_node src, ns_node dst);
        public static SortedSet<ns_node> AStar(ns_node src, ns_node dst, heuristic __h)
        {
            ns_pqueue<ns_node> frontier = new ns_pqueue<ns_node>();
            SortedSet<ns_node> CloseSet = new SortedSet<ns_node>(new node_cmp());

            //ns_list<ns_keyValuePair> cost_so_far = new ns_list<ns_keyValuePair>();
            Dictionary<string, double> cost_so_far = new Dictionary<string, double>();
            Dictionary<string, ns_node> camefrom = new Dictionary<string, ns_node>();
            cost_so_far.Add(src.tag, 0);
            int cost = 0;
            frontier.ns_insert(cost, src);


            while (frontier.Count > 0)
            {
                var fx = frontier.ns_pull_pmin();
                ns_node current = fx.value;
               // printf("current:{0} cost:{1}", current.tag, cost_so_far[current.tag]);
                if (current == dst)
                {
                    CloseSet.Add(dst);

                    break;
                }
                if (!contains(CloseSet, current))
                    CloseSet.Add(current);

                current.ChildNodes.Foreach((next, i) =>
                {
                    //if(cost_so_far[next.tag]!=dou)

                    double new_cost = cost_so_far[current.tag] + current.getLinkCost(next);

                    if (!cost_so_far.ContainsKey(next.tag) || new_cost < cost_so_far[next.tag])
                    {

                        cost_so_far[next.tag] = new_cost + __h(current, next);
                        //printf("current cost:{0} next cost:{1}", cost_so_far[current.tag], cost_so_far[next.tag]);
                        //printf("current:{0} next:{1}", current.tag, next.tag);
                        bool exist = frontier.ns_contains((p) =>
                        {
                            return p.value.tag == next.tag;
                        });
                        if (!exist)
                            frontier.ns_insert((int)new_cost, next);
                        if (!camefrom.ContainsKey(next.tag))
                        {
                            camefrom[current.tag] = next;
                        }

                    }
                    // if (next == dst) break;


                });


            } // end while

           // Draw(CloseSet);
            return CloseSet;
        }
        private static bool contains(SortedSet<ns_node> nodes, ns_node node)
        {

            foreach (ns_node n in nodes)
            {
                if (n == node) return true;
            }
            return false;
        }
        private static bool contains(SortedSet<sensor_node> nodes, ns_node node)
        {

            foreach (ns_node n in nodes)
            {
                if (n == node) return true;
            }
            return false;
        }
        public static SortedSet<sensor_node> dijkstra(sensor_node src, ns_node dst)
        {
            ns_pqueue<sensor_node> frontier = new ns_pqueue<sensor_node>();
            SortedSet<sensor_node> CloseSet = new SortedSet<sensor_node>(new node_cmp());

            //ns_list<ns_keyValuePair> cost_so_far = new ns_list<ns_keyValuePair>();
            Dictionary<string, double> cost_so_far = new Dictionary<string, double>();
            Dictionary<string, ns_node> camefrom = new Dictionary<string, ns_node>();
            cost_so_far.Add(src.tag, 0);
            int cost = 0;
            frontier.ns_insert(cost, src);


            while (frontier.Count > 0)
            {
                var fx = frontier.ns_pull_pmin();
                sensor_node current = fx.value;
                //printf("current:{0} cost:{1}", current.tag, cost_so_far[current.tag]);
                if (current == dst)
                {
                    //CloseSet.Add(dst);

                    break;
                }
                if (!contains(CloseSet, current))
                    CloseSet.Add(current);

                current.ChildNodes.Foreach((next, i) =>
                {
                    //if(cost_so_far[next.tag]!=dou)

                    double new_cost = cost_so_far[current.tag] + current.getLinkCost(next);

                    if (!cost_so_far.ContainsKey(next.tag) || new_cost < cost_so_far[next.tag])
                    {

                        cost_so_far[next.tag] = new_cost;
                        //printf("current cost:{0} next cost:{1}", cost_so_far[current.tag], cost_so_far[next.tag]);
                        //printf("current:{0} next:{1}", current.tag, next.tag);
                        bool exist = frontier.ns_contains((p) =>
                        {
                            return p.value.tag == next.tag;
                        });
                        if (!exist)
                            if(next.GetType()==typeof(sensor_node))
                            frontier.ns_insert((int)new_cost, (sensor_node)next);
                        if (!camefrom.ContainsKey(next.tag))
                        {
                            camefrom[current.tag] = next;
                        }

                    }
                    // if (next == dst) break;


                });


            } // end while

            //Draw(CloseSet);
            return CloseSet;
        }
    }
}
