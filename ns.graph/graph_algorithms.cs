using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ns.graph
{
    public class graph_algorithms
    {

        public  delegate void _printf(string fmt, params object[] args);
        public static _printf printf;

        private static void printfx(string fmt , params object[] args)
        {
            if(printf!=null)
            {
                printf(fmt, args);
            }
        }
        private static List<ns_node> ConstructPath(Dictionary<string,ns_node> parentof , ns_node start , ns_node current)
        {
            List<ns_node> path = new List<ns_node>();
           
            while (current.tag != start.tag)
            {
                path.Add(current);
                current = parentof[current.tag];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }
        //
        public static void Dijkstra_connect(Dictionary<string , ns_node> nodes, double max_dist)
        {
            PriorityQueue<double, ns_node> frontier = new PriorityQueue<double, ns_node>((d1, d2) =>
            {
                return d1.priority <= d2.priority;
            });
            ns_node src = nodes.First().Value;
            frontier.Enqueue(src, 0);
            Dictionary<string, double> cost_so_far = new Dictionary<string, double>();
            cost_so_far[src.tag] = 0;
            Dictionary<string, ns_node> parentof = new Dictionary<string, ns_node>();
            List<ns_node> path = new List<ns_node>();
            ns_node dst = src;
            while(!frontier.IsEmpty())
            {
                var cur_item = frontier.Top();
                ns_node current = cur_item.Value;
                
                foreach(var kvp in nodes)
                {
                    ns_node next = kvp.Value;
                    if(next.tag == current.tag) continue;
                    double cost = current.edistance(next);
                    if(cost <=max_dist && !cost_so_far.ContainsKey(next.tag))
                    {
                        cost_so_far[next.tag] = cost;
                        double priority = cost;
                        frontier.insert(next, priority);

                        parentof[next.tag] = current;
                    }
                }
            }

        }
        public static List<ns_node> GetChargingPath(ns_node src, string dst_tag)
        {
            PriorityQueue<double, ns_node> frontier = new PriorityQueue<double, ns_node>((d1, d2) =>
            {
                return d1.priority <= d2.priority;
            });
            frontier.Enqueue(src, 0);
            Dictionary<string, double> cost_so_far = new Dictionary<string, double>();
            cost_so_far[src.tag] = 0;
            Dictionary<string, ns_node> parentof = new Dictionary<string, ns_node>();
            List<ns_node> path = new List<ns_node>();
            ns_node dst = src;
            while (!frontier.IsEmpty())
            {
                var cur_item = frontier.Top();
                ns_node current = cur_item.Value;

                path.Add(current);
                if (current.tag == dst_tag)
                {
                     dst = current;
                    
                    break;
                }

                current.ChildNodes.Foreach((next, i) =>
                {
                    double new_cost = cost_so_far[current.tag] + current.getLinkCost(next);


                    if ((!cost_so_far.ContainsKey(next.tag)) || (new_cost < cost_so_far[next.tag]))
                    {
                        cost_so_far[next.tag] = new_cost;
                        double priority = new_cost;
                        frontier.insert(next, priority);
                        // printfx("camefrom:{0}  current:{1} , cost:{2}", next.tag, current.tag , new_cost);
                        parentof[next.tag] = current;
                    }
                });


            }
            if (!parentof.ContainsKey(dst.tag))
             return null;// no path.
            return ConstructPath(parentof, src, dst);
        }
        public static ns_node Dijkstra_GetNode(ns_node src, string dst_tag)
        {
            PriorityQueue<double, ns_node> frontier = new PriorityQueue<double, ns_node>((d1, d2) =>
            {
                return d1.priority <= d2.priority;
            });
            frontier.Enqueue(src, 0);
            Dictionary<string, double> cost_so_far = new Dictionary<string, double>();
            cost_so_far[src.tag] = 0;
            Dictionary<string, ns_node> parentof = new Dictionary<string, ns_node>();
            List<ns_node> path = new List<ns_node>();
            ns_node dst = src;
            while (!frontier.IsEmpty())
            {
                var cur_item = frontier.Top();
                ns_node current = cur_item.Value;

                path.Add(current);
                if (current.tag == dst_tag)
                {
                   // dst = current;
                    return current;
                    //break;
                }

                current.ChildNodes.Foreach((next, i) =>
                {
                    double new_cost = cost_so_far[current.tag] + current.getLinkCost(next);


                    if ((!cost_so_far.ContainsKey(next.tag)) || (new_cost < cost_so_far[next.tag]))
                    {
                        cost_so_far[next.tag] = new_cost;
                        double priority = new_cost;
                        frontier.insert(next, priority);
                        // printfx("camefrom:{0}  current:{1} , cost:{2}", next.tag, current.tag , new_cost);
                        parentof[next.tag] = current;
                    }
                });


            }
            //if (!parentof.ContainsKey(dst.tag))
              //  return null;// no path.
            return src;
        }

        public static List<ns_node> Dijkstra_GetNearestMC(ns_node src)
        {
            PriorityQueue<double, ns_node> frontier = new PriorityQueue<double, ns_node>((d1, d2) =>
            {
                return d1.priority <= d2.priority;
            });
            frontier.Enqueue(src, 0);
            Dictionary<string, double> cost_so_far = new Dictionary<string, double>();
            cost_so_far[src.tag] = 0;
            Dictionary<string, ns_node> parentof = new Dictionary<string, ns_node>();
            List<ns_node> path = new List<ns_node>();
            ns_node dst = src;
            while (!frontier.IsEmpty())
            {
                var cur_item = frontier.Top();
                ns_node current = cur_item.Value;

                path.Add(current);
                if (current.NodeType == Node_TypeEnum.Mobile_Charger)
                {
                    dst = current;
                    

                    break;
                }

                current.ChildNodes.Foreach((next, i) =>
                {
                    double new_cost = cost_so_far[current.tag] + current.getLinkCost(next);


                    if ((!cost_so_far.ContainsKey(next.tag)) || (new_cost < cost_so_far[next.tag]))
                    {
                        cost_so_far[next.tag] = new_cost;
                        double priority = new_cost;
                        frontier.insert(next, priority);
                        // printfx("camefrom:{0}  current:{1} , cost:{2}", next.tag, current.tag , new_cost);
                        parentof[next.tag] = current;
                    }
                });


            }
            if (!parentof.ContainsKey(dst.tag))
                return null;// no path.
            return ConstructPath(parentof, src, dst);
        }
        //
        public static List<ns_node> Dijkstra(ns_node src , ns_node dst)
        {
            PriorityQueue<double, ns_node> frontier = new PriorityQueue<double, ns_node>((d1, d2) =>
            {
                return d1.priority <= d2.priority;
            });
            frontier.Enqueue(src, 0);
            Dictionary<string, double> cost_so_far = new Dictionary<string, double>();
            cost_so_far[src.tag] = 0;
            Dictionary<string, ns_node> parentof = new Dictionary<string, ns_node>();
            List<ns_node> path = new List<ns_node>();
            //ns_node current = src;
            while(!frontier.IsEmpty())
            {
                var cur_item = frontier.Top();
                ns_node current = cur_item.Value;
                 
                path.Add(current);
                if(current.tag == dst.tag)
                {
                    
                    break;
                }
                
                current.ChildNodes.Foreach((next, i) =>
                    {
                        double new_cost = cost_so_far[current.tag] + current.getLinkCost(next);
                        

                        if ((!cost_so_far.ContainsKey(next.tag)) || (new_cost < cost_so_far[next.tag])) 
                        {
                            cost_so_far[next.tag] = new_cost;
                            double priority = new_cost;
                            frontier.insert(next, priority);
                           // printfx("camefrom:{0}  current:{1} , cost:{2}", next.tag, current.tag , new_cost);
                            parentof[next.tag] = current;
                        }
                    });


            }
            if (!parentof.ContainsKey(dst.tag))
                return null;// no path.
            return  ConstructPath(parentof, src, dst);
        }

        public delegate double __heuristic(ns_node src, ns_node sink);
        public static List<ns_node> Astar(ns_node src, ns_node dst , __heuristic _h)
        {
            PriorityQueue<double, ns_node> frontier = new PriorityQueue<double, ns_node>((d1, d2) =>
            {
                return d1.priority <= d2.priority;
            });
            frontier.Enqueue(src, 0);
            Dictionary<string, double> cost_so_far = new Dictionary<string, double>();
            cost_so_far[src.tag] = 0;
            Dictionary<string, ns_node> parentof = new Dictionary<string, ns_node>();
            List<ns_node> path = new List<ns_node>();
            
            while (!frontier.IsEmpty())
            {
                var cur_item = frontier.Top();
                ns_node current = cur_item.Value;
               
                path.Add(current);
                if (current.tag == dst.tag)
                {
                    
                    break;
                }

                current.ChildNodes.Foreach((next, i) =>
                {
                    double new_cost = cost_so_far[current.tag] + current.getLinkCost(next);
                    

                    if ((!cost_so_far.ContainsKey(next.tag)) || (new_cost < cost_so_far[next.tag]))
                    {
                        cost_so_far[next.tag] = new_cost + _h(current, next);
                        double priority = new_cost;
                        frontier.insert(next, priority);
                        //printfx("camefrom:{0}  current:{1} , cost:{2}", next.tag, current.tag, new_cost);
                        parentof[next.tag] = current;
                    }
                });


            }
            if (!parentof.ContainsKey(dst.tag))
                return null;// no path.
            return ConstructPath(parentof, src, dst);
        }

        public static List<ns_node> hueristic_best_first_search(ns_node src, ns_node dst, __heuristic _h)
        {
            PriorityQueue<double, ns_node> frontier = new PriorityQueue<double, ns_node>((d1, d2) =>
            {
                return d1.priority <= d2.priority;
            });
            frontier.Enqueue(src, 0);
            Dictionary<string, ns_node> parentof = new Dictionary<string, ns_node>();
            while (!frontier.IsEmpty())
            {
                var fx = frontier.Top();
                ns_node current = fx.Value;
                if (current.tag == dst.tag)
                {
                    break;
                }
                current.ChildNodes.Foreach((next) =>
                {
                    double cost = current.getLinkCost(next) + _h(current, next);
                    if (!parentof.ContainsKey(next.tag))
                    {
                        frontier.Enqueue(next, cost);
                        parentof[next.tag] = current;
                    }

                });
            }
            if (!parentof.ContainsKey(dst.tag)) return null;
            return ConstructPath(parentof, src, dst);
        }
        public static List<ns_node> greedy_best_first_search(ns_node src, ns_node dst)
        {
            PriorityQueue<double, ns_node> frontier = new PriorityQueue<double, ns_node>((d1, d2) =>
            {
                return d1.priority <= d2.priority;
            });
            frontier.Enqueue(src, 0);
            Dictionary<string, ns_node> parentof = new Dictionary<string, ns_node>();
            while(!frontier.IsEmpty())
            {
                var fx = frontier.Top();
                ns_node current = fx.Value;
                if(current.tag == dst.tag)
                {
                    break;
                }
                current.ChildNodes.Foreach((next) =>
                    {
                        double cost = current.getLinkCost(next);
                        if(!parentof.ContainsKey(next.tag))
                        {
                            frontier.Enqueue(next, cost);
                            parentof[next.tag] = current;
                        }

                    });
            }
            if (!parentof.ContainsKey(dst.tag)) return null;
            return ConstructPath(parentof, src, dst);
        }
        public static List<ns_node> breadth_first_search(ns_node src , ns_node dst)
        {
            Queue<ns_node> frontier = new Queue<ns_node>();
            frontier.Enqueue(src);
            Dictionary<string, ns_node> parentof = new Dictionary<string, ns_node>();
            while(frontier.Count>0)
            {
                ns_node current = frontier.Dequeue();

                if (current.tag == dst.tag) break;

                current.ChildNodes.Foreach((next) =>
                    {
                        if(!parentof.ContainsKey(next.tag))
                        {
                            parentof[next.tag] = current;
                            frontier.Enqueue(next);
                        }

                    });
            }
            if (!parentof.ContainsKey(dst.tag))
                return null;

            return ConstructPath(parentof, src, dst);
        }


    }

}
