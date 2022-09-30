using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.graph
{
    public class pvp_t<T>
    {
        public int priority;
        public T value;
        public pvp_t(int _prio , T value)
        {
            this.value = value;
            this.priority = _prio;
        }
        public pvp_t()
        {
           // this.value = value;
            this.priority = -1;
        }
    }
    public class pvp
    {
        public int priority;
        public object value;
        public pvp(int _priority , object _val)
        {
            this.priority = _priority;
            this.value = _val;
        }
    }
    /// <summary>
    /// implement priority Queue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ns_pqueue<T> : List<pvp_t<T>>
    {
        public delegate void __foreach(T item, int i);
        public ns_pqueue()
        {
           
        }
        public ns_pqueue<T> ns_insert(int priority , T value)
        {
            this.Add(new pvp_t<T>(priority, value));
            return this;
        }
        public delegate bool __pull(pvp_t<T> first, pvp_t<T> second);
        /// <summary>
        /// pull the element depending on a function that return bool.
        /// 
        /// </summary>
        /// <param name="pull_del"></param>
        /// <returns></returns>
        public pvp_t<T> ns_pull(__pull pull_del)
        {
            int i = 0;
            int count = this.Count;
            pvp_t<T> first = this[i];
            for (i = 0; i < count; i++) 
            {
                if(pull_del(this[i], first))
                {
                    first = this[i];
                }
            }
            this.Remove(first);
            return first;
        }
        /// <summary>
        /// pull the element with max priority then remove it.
        /// </summary>
        /// <returns></returns>
        public pvp_t<T> ns_pull_pmax()
        {
            return this.ns_pull((first, second) =>
                {
                    return first.priority >= second.priority;
                });
        }
        /// <summary>
        /// pull the element with min priority
        /// </summary>
        /// <returns></returns>
        public pvp_t<T> ns_pull_pmin()
        {
            return this.ns_pull((first, second) =>
                {
                    return first.priority <= second.priority;
                });
        }
        public ns_pqueue<T> ns_remove(pvp_t<T> item)
        {
            this.Remove(item);
            return this;
        }
        public delegate bool __contains(pvp_t<T> item);
        public bool ns_contains(__contains contain_del)
        {
            int i = 0;
            int count = this.Count;
            for(i=0;i<count;i++)
            {
                if (contain_del(this[i])) return true;
            }
            return false;
        }
        public delegate bool __getter(pvp_t<T> item);
        public pvp_t<T> get(__getter get_del)
        {
            pvp_t<T> item = this[0];
            int i = 0;
            int count = this.Count;
            for (i = 0; i < count; i++)
            {
                if (get_del(this[i])) return this[i];
            }
            

            return item;
        }
    }
    public class ns_heap<T> : List<pvp_t<T>>
    {
        public ns_heap()
        {

        }
        public delegate void _foeach(pvp_t<T> ele, int i);
        public ns_heap<T> ns_foreach(_foeach __foreach)
        {
            int i=0;
            int count = this.Count;
            for (i = 0; i < count; __foreach(this[i], i), i++) ;
            return this;
        }
        public ns_heap<T> ns_insert(int p, T value)
        {
            pvp_t<T> _pvp = new pvp_t<T>(p, value);

            return ns_insert(_pvp);
        }
        public pvp_t<T> pull()
        {
            pvp_t<T> highest = this[0];
            int i = 0;
            int count = this.Count;

            for (i = 0; i < count; i++)
            {
                if (this[i].priority >= highest.priority)
                {
                    highest = this[i];
                }
            }
            this.Remove(highest);
            return highest;       
        }
        public ns_heap<T> ns_insert(pvp_t<T> node)
        {


            this.Add(node);
            //pvp_t<T> highest = this[0];
            //int i=0;
           
            
            return this;
        }
        
    }
    public class spot
    {
        public double fscore;
        public double gscore;
        public ns_node node;
        public spot(ns_node thenode , double f, double g)
        {
            this.fscore = f;
            this.gscore = g;
           
        }
        public spot(ns_node thenode)
        {
            this.node = thenode;
        }

    }
    public class AStar
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <param name="h"> hueristic function</param>
        ///  return Math.Abs(Math.Cos(src.mdistance(dst))+Math.Cos(src.edistance(dst))); the best 
        public static double heuristic(ns_node src, ns_node dst)
        {
            //return Math.PI;///2* Math.Sin(src.edistance(dst) / src.mdistance(dst));
          // return Math.Abs(Math.Cos(src.mdistance(dst))+Math.Cos(src.edistance(dst)));// eucludian distance.
           // return src.location.lerp(dst.location, 0.78).magnitude();
            return Math.Abs(Math.Cos(src.getLinkCost(dst)));
        }

        public static double sigmoid(ns_node src, ns_node dst)
        {
            double md = src.mdistance(dst);
            return 10.0/(1+Math.Pow(Math.E,-1*md));
        }

        public static bool Contains(ns_pqueue<spot> openset , ns_node node)
        {
            bool ex = openset.ns_contains((n) =>
                {
                    return n.value.node.tag == node.tag;
                });
            return ex;
        }
        public static ns_pqueue<spot> AStar_algorithm2(ns_node src , ns_node dst)
        {
            spot start = new spot(src);
            start.gscore = 0;
            start.fscore = 0;
            ns_pqueue<spot> openSet = new ns_pqueue<spot>();
            ns_pqueue<spot> closeSet = new ns_pqueue<spot>();
            openSet.ns_insert((int)start.fscore, start);
            while(openSet.Count>0)
            {
                spot node = openSet.ns_pull_pmax().value;

                double node_fscore = node.fscore;
                double node_gsocre = node.gscore;
                ns_node current = node.node;
                /*
                 * if neighbor in OPEN and cost less than g(neighbor):
                 remove neighbor from OPEN, because new path is better
                 if neighbor in CLOSED and cost less than g(neighbor): ⁽²⁾
                 remove neighbor from CLOSED
                 if neighbor not in OPEN and neighbor not in CLOSED:
                 set g(neighbor) to cost
                 add neighbor to OPEN
                 set priority queue rank to g(neighbor) + h(neighbor)
                 set neighbor's parent to current
                 * 
                 * 
                 * 
                 */
                if(!Contains(closeSet,current))
                {
                    closeSet.ns_insert((int)node_fscore, node);
                }
              if(current.tag==dst.tag)
              {
                  closeSet.ns_insert((int)node_fscore, node);
                  return closeSet;
              }
                int i = 0;
                int count = current.ChildNodes.Count;
                var neigbors = current.ChildNodes;
                for (i = 0; i < count; i++)
                {
                    ns_node neigbore = neigbors[i];
                    double cost = node_gsocre + heuristic(current, neigbore);
                    //#1
                    if(Contains(openSet,neigbore))
                    {
                      var ele   =   openSet.Find((p) =>
                            {
                                return p.value.node.tag == neigbore.tag;
                            });
                        if(cost<ele.value.gscore) //
                        {
                            /// remove this from opeset
                            openSet.Remove(ele);
                        }
                    }
                    //end #1

                    //#2
                    if (Contains(closeSet, neigbore))
                    {
                        var ele = closeSet.Find((p) =>
                        {
                            return p.value.node.tag == neigbore.tag;
                        });
                        if (cost < ele.value.gscore) //
                        {
                            /// remove this from opeset
                            closeSet.Remove(ele);
                        }
                    }
                    //end #2
                    //3
                    if(!Contains(openSet, neigbore) && !Contains(closeSet,neigbore))
                    {
                        spot n_spot = new spot(neigbore);
                        n_spot.gscore = cost;
                        n_spot.fscore = cost + heuristic(current, neigbore);
                        openSet.ns_insert((int)n_spot.fscore, n_spot);
                    }
                    //end 3

                }



            }
            return closeSet;

        }
        public static ns_pqueue<spot> AStar_algorithm(ns_node src, ns_node dst, double h , Action<string> loger)
        {
            spot start = new spot(src);
            ns_pqueue<spot> openSet = new ns_pqueue<spot>();
            ns_pqueue<spot> CloseSet = new ns_pqueue<spot>();//empty
            //add the src node to the queue.
            start.gscore = 0;
            start.fscore = 0;// start.gscore + heuristic(src, dst);
            openSet.ns_insert((int)start.fscore, start); 

            while (openSet.Count>0)
            {
                pvp_t<spot> pvp = openSet.ns_pull_pmax();

                ns_node current = pvp.value.node;
                if(current.tag == dst.tag)
                {
                    //construct_path(pvp, dst);
                    spot sp=new spot(dst);
                    CloseSet.ns_insert(0, sp);//add the last node to the path.
                    return CloseSet;
                }
               // openSet.ns_remove(current); // currently removed by function above.
                bool exists = false;
                 exists = CloseSet.ns_contains((item) =>
                {
                    return item.value.node.tag == current.tag;
                });
                //if(!exists)
                    CloseSet.Add(pvp);

                var neigbors = current.ChildNodes;
                int count = neigbors.Count;
                for (int i = 0; i < count; i++)
                {
                    
                    ns_node n = neigbors[i];
                    spot neighbor = new spot(n);
                     exists = CloseSet.ns_contains((item) =>
                    {
                        return item.value.node.tag == n.tag;/// node already exists.
                    });
                     if (exists) continue;// if already exist don't do any thing.

                    neighbor.gscore =  pvp.value.gscore + 1; // increase g by 1
                    double fscore = neighbor.gscore + heuristic(n, current);
                    double ten_dist = pvp.value.gscore;// +current.edistance(n) / 100;
                    //string s = string.Format("src:{0} \t dst:{1} cost :{2}", current.tag, n.tag, current.cost);
                    //loger(s);
                    exists = openSet.ns_contains((item) =>
                    {
                        return item.value.node.tag == neighbor.node.tag;
                    });
                    
                    if (ten_dist < neighbor.gscore)
                    {
                        
                        
                        neighbor.gscore = ten_dist;
                        neighbor.fscore = ten_dist + h;
                        //openSet.ns_insert((int)fscore, neighbor);

                    }
                    if (!exists)
                    {
                        openSet.ns_insert((int)fscore, neighbor);
                    }

                }
            }
            return CloseSet;// if empty then the dst is not reached.


        }




        private static void construct_path(pvp_t<spot> current, ns_node dst)
        {
          
        }

        
    }
    public class di_node
    {
        public ns_node node;
        public bool visited;
        public double tentive_distance;
        public di_node(ns_node n)
        {
            this.node = n;
            this.tentive_distance = 0;
            this.visited = false;
        }
    }
    public class ns_routing
    {

        public void ns_shortes_path(ns_node src, ns_node dst)
        {

            //if (src.tag == dst.tag) return;
            int i = 0;
            int j = 0;

            int c = src.ChildNodes.Count;
            for (i = 0; i < c; i++)
            {
                ns_node n = src.ChildNodes[i];
                for (j = 0; j < n.ChildNodes.Count; j++)
                {

                }

            }
        }
        public static double Min(double x, double y)
        {
            return x <= y ? x : y;
        }
        public static double Max(double x, double y)
        {
            return x >= y ? x : y;
        }
        public static ns_node NodeWithMinDistance(ns_node_collection Q, ns_node src)
        {
            int i = 0, k = 0;
            int c = Q.Count;
            double min_dist = 0.0;
            for (i = 0; i < c; i++)
            {
                ns_node n = Q[i];
                double dist = src.edistance(n);
                if (i == 0)
                    min_dist = dist;
                min_dist = Min(min_dist, dist);
                k = min_dist == dist ? i : k;
            }
            return Q[k];
        }
        public struct result
        {
            double[] dist;
            ns_node_collection prev;
            Action<string> loger;
            public result(double[] dist, ns_node_collection prev , Action<string> loger)
            {
                this.dist = dist;
                this.prev = prev;
                this.loger = loger;
            }

            public result PrinttPrev()
            {

                int i = 0;
                int c = this.prev.Count;
                for (i = 0; i < c; i++)
                {
                    this.loger(string.Format("[{0}]:{1}", i, this.prev[i].tag));
                }


                    return this;
            }
            public result Printdist()
            {
                int i=0;
                foreach(double d in this.dist)
                {
                    this.loger(string.Format("[{0}]:{1}", i, d));
                    i++;
                }
                return this;
            }
        }
        public static result xDijkstra(ns_node_collection graph, ns_node src, Action<string> loger)
        {
            double[] dist = new double[graph.Count];
            ns_node_collection Q = new ns_node_collection();
            ns_node_collection prev = new ns_node_collection();
            graph.Foreach((n, j) =>
                {
                    dist[j] = double.NaN;
                    Q.AddNode(n);
                    prev.add(new ns_node());
                });
            int i = graph.getIndexOf(src);
            //if (i == -1) return;
            dist[i] = 0;
            while (Q.IsNotEmpty())
            {
                ns_node u = NodeWithMinDistance(Q, src);
                double u_dist = u.edistance(src);
                int index = Q.getIndexOf(u);
                dist[index] = u_dist;
                Q.Remove(u);

                // for each neighbor v of u still in Q:
                u.ChildNodes.Foreach((v, k) =>
                    {
                        // length(u, v)
                        if (Q.Contains(v))
                        {
                            string fmt = string.Format("src:{0} ----> dst:{1}", u.tag, v.tag);
                            loger(fmt);
                            double alt = u_dist + u.edistance(v);
                            double v_dist = src.edistance(v);
                            int ii = Q.getIndexOf(v);
                            dist[ii] = v_dist;
                            if (alt < v_dist)
                            {
                                 fmt = string.Format("v_dist:{0} ----> u_dist:{1}", v_dist, u_dist);
                                loger(fmt);
                                dist[ii] = alt;
                                prev[k] = u;
                            }
                        }

                    });

            }
            return new result(dist, prev,loger);

        }
        public static result xDijkstra_ShortestPath(ns_node_collection graph, ns_node src , ns_node target, Action<string> loger)
        {
            double[] dist = new double[graph.Count];
            ns_node_collection Q = new ns_node_collection();
            ns_node_collection prev = new ns_node_collection();
            graph.Foreach((n, j) =>
            {
                dist[j] = double.NaN;
                Q.AddNode(n);
                prev.add(new ns_node());
            });
            int i = graph.getIndexOf(src);
            //if (i == -1) return;
            dist[i] = 0;
            i = 0;
            prev[0] = src;
            i = 1;
            while (Q.IsNotEmpty())
            {
                ns_node u = NodeWithMinDistance(Q, src);
                double u_dist = src.edistance(u);
                int index = Q.getIndexOf(u);
                dist[index] = u_dist;
                Q.Remove(u);

                // for each neighbor v of u still in Q:
                if (u.ChildNodes.Contains(target))
                {


                    prev[i] = u;

                    i++;
                }
                loger(string.Format("src:{0} ----------> u:{1} -> dist:{2}", src.tag, u.tag, u_dist));
                if(u.tag == target.tag)
                {
                    prev[i] = target;
                    break;
                }
            }
              

            
            return new result(dist, prev, loger);

        }
        
        
        
    }
}
