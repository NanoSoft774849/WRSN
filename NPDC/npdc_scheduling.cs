using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace NPDC
{
    public class task_reports
    {
        public int count;
        public double total_energy;
        public double task_arrival_time;
        public double task_release_time;
        public double task_complete_time;
        public double task_response_time;
        public int dead_nodes;
        public int sectorId;
        public double qwaiting_time;
        public task_reports()
        {
            this.count = 0;
            this.total_energy = 0.0;
            this.task_arrival_time = 0.0;
            this.task_complete_time = 0.0;
            this.task_release_time = 0.0;
            this.dead_nodes = 0;
            this.qwaiting_time = 0.0;
            
        }
        
        private static double frac(double x, double y)
        {
            if (y == 0) return 0.0;
            return x / y;
        }
        public static task_reports Get(ns_task_set set, int sector)
        {
            task_reports report = new task_reports();
            int N = set.Count;
            report.count = N;
            report.sectorId = sector;
            foreach( var task in set)
            {
                report.dead_nodes += (task.The_node.PowerConfig.BatteryCapacity < 1) ? 1 : 0;
                report.task_arrival_time += frac(task.TaskArriveTime, N);
                report.task_complete_time += frac(task.TaskServiceTime, N);
                report.task_response_time += frac(task.TaskResponseTime, N);
                report.total_energy += frac(task.The_node.PowerConfig.BatteryCapacity, N);
                report.qwaiting_time += frac(task.QueueWaitingTime, N);
            }

            return report;
        }
        /// <summary>
        /// if you want to group by sector.
        /// statistics by sector.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public static List<task_reports> GetBySector(Dictionary<int, ns_task_set> dict)
        {
            List<task_reports> reports = new List<task_reports>();
            foreach(var fx in dict)
            {
                var report = Get(fx.Value, fx.Key);
                reports.Add(report);
            }
            return reports;
        }
        
    }
    public class ns_task_set : List<ns_charge_task>
    {

        public ns_task_set()
        {

        }
       public ns_task_set addTask(ns_charge_task task)
        {
            if (this.isExist(task))
            {
               // task.ChangeTaskStatus(ns_task_status.created);
                this.UpdateTaskStatus(task, ns_task_status.created);
                return this;
            };
            this.Add(task);
            return this;
        }
        public bool isExist(ns_charge_task task)
       {
           if (this.Count == 0) return false;
            foreach(var t in this)
            {
                if (t.task_id == task.task_id) return true;
            }
            return false;
       }
        public bool isExist(string task_id)
        {
            if (this.Count == 0) return false;
            foreach (var t in this)
            {
                if (t.task_id == task_id) return true;
            }
            return false;
        }
        public ns_charge_task getTaskById(string task_id)
        {
            foreach(var task in this)
            {
                if (task.task_id == task_id) return task;
            }
            return null;
        }
        public ns_charge_task getTask(ns_node node)
        {
            return getTaskById(node.tag);
        }
        /// <summary>
        /// Check if the task set is schedulable.
        ///  A task set is schedulable if each task \in task set is schedulable.
        /// </summary>
        /// <returns></returns>
        public bool is_schedulable(mobile_charger mcv)
        {
            bool yes = true;
            foreach(var t in this)
            {
                if( t.task_status == ns_task_status.created)
                yes &= t.is_Schedulable(mcv);
            }
            return yes;
        }

       // public 
        public ns_task_set getSchedulable_tasks(mobile_charger mcv)
        {
           // if (this.is_schedulable(mcv)) return this;
            ns_task_set schedulable_tasks = new ns_task_set();

            foreach(var task in this)
            {
                bool sc = task.is_Schedulable(mcv);
                sc &=(task.task_status == ns_task_status.created);
                if( sc )
                {
                    schedulable_tasks.addTask(task);
                }
            }
            return schedulable_tasks;
        }
        public ns_task_set getNonSchedulable_Tasks(mobile_charger mcv)
        {
            ns_task_set schedulable_tasks = new ns_task_set();

            foreach (var task in this)
            {
                bool sc = !task.is_Schedulable(mcv);
                sc &= (task.task_status == ns_task_status.created);
                if (sc)
                {
                    schedulable_tasks.addTask(task);
                }
            }
            return schedulable_tasks;
        }
        
        public ns_charge_task this[ string task_id ]
        {
            get
            {
                return this.First(t =>
                    {
                        return t.task_id == task_id;
                    });
            }
        }
        private PriorityQueue<double , mobile_charger> ToPq(Dictionary<string, mobile_charger> MCvs)
        {
            PriorityQueue<double, mobile_charger> pq = new PriorityQueue<double, mobile_charger>(Compare);
            foreach(var mc in MCvs)
            {
                var mcv = mc.Value;
                int ql = mcv.GetQueueLen();
                pq.Enqueue(mcv, ql);
            }
            return pq;
        }
        private bool Compare(PriorityValuePair<double, mobile_charger> p1, PriorityValuePair<double, mobile_charger> p2)
        {
            return p1.priority <= p2.priority;
        }
        private mobile_charger getNearestMCv(Dictionary<string,mobile_charger> mcvs, ns_charge_task task)
        {
            ns_node nodex = task.The_node;
            double min_qlen = 0.0;
            int i = 0;
            string mc_id = "";
            double ql = 0.0;
            foreach (var mc in mcvs)
            {
                var node = mc.Value;
                var dist = nodex.edistance(node);
                if (node.PowerConfig.BatteryCapacity > 300)
                {
                    ql = (node.GetQueueLen() + 1); // ensure zero with minimum distance.
                    if (i == 0)
                    {

                        min_qlen = ql * dist;
                        i++;
                        mc_id = node.tag;
                        continue;
                    }
                    double qlen = ql * dist;
                    min_qlen = qlen <= min_qlen ? qlen : min_qlen;
                    mc_id = min_qlen == qlen ? node.tag : mc_id;
                    i++;
                }
            }
           // return this.MobileChargers[mc_id];
            return mcvs[mc_id];
        }
        /// <summary>
        /// Assign Tasks to MCVs , 
        /// first check if tasks is schedulable with the MCVs
        /// Then Select the closer MCVs to the node. 
        /// the minumum distance to the node will be add to the MCV queue.
        /// </summary>
        /// <param name="MCVs"></param>
        /// 
        public void AssignTasks(Dictionary<string, mobile_charger> MCVs, double Tr)
        {
            int K = (int)Math.Floor((double)this.Count / MCVs.Count);

            var set = this;
            int i = 0;
            foreach (var task in set)
            {
                if (task.task_status != ns_task_status.created) continue;
                var mcv = getNearestMCv(MCVs, task);
                if (task.is_Schedulable(mcv))
                {
                    this.getTask(task.The_node).ChangeTaskStatus(ns_task_status.assigned);
                    task.task_status = ns_task_status.assigned;
                    task.assigned_to = mcv.tag;
                    mcv.PushTask(task);
                    i++;
                }
            }

        }
        public delegate double __order_(ns_node n1, ns_node n2);
       public ns_task_set OrderByDistanceDesc(ns_node node, __order_ _oder)
        {
            ns_task_set set = new ns_task_set();
            var setx = this.OrderByDescending(task =>
                {
                    return _oder(task.The_node, node);
                }).Reverse();
           foreach(var t in setx)
           {
               set.addTask(t);
           }
           return set;
        }
       public void UpdateAssignedMCV(ns_charge_task task , mobile_charger mcv)
       {
           var tasksx = this.getTask(task);
           tasksx.assigned_to = mcv.tag;
       }
        public ns_charge_task getTask(ns_charge_task tsk)
       {
            foreach(var t in this)
            {
                if (t.task_id == tsk.task_id) return t;
            }
            return this.getTaskById(tsk.task_id);
       }
        
        public ns_task_set NodesInMySector(mobile_charger mcv, __assign_task fxr)
        {
            ns_task_set set = new ns_task_set();
            foreach( var task in this)
            {
                var node = task.The_node;
                if( fxr(task, mcv))
                {
                    set.addTask(task);
                }
            }
            return set;
        }
        public void AssignTasks_ByOrder(Dictionary<string, mobile_charger> mcvs)
       {
            __order_ fx = (n1, n2)=>
                {
                    return n1.edistance(n2) / n1.GetImportanceValue();
                };
            __assign_task tf = (t1, mcvx) =>
                {
                    return t1.task_status == ns_task_status.created && mcvx.IsInMyPrimaryLayers(t1);
                };
            __assign_task tf2 = (t1, mcvx) =>
            {
                return t1.task_status != ns_task_status.completed && mcvx.IsInMyLayer(t1);
            };

            int max_count = 4;// (int)mcvs.First().Value.optimization_constant;
            int i = 0;
            int qlen = 0;
            bool remove = false;
            foreach(var mc in mcvs)
            {
                var mcv = mc.Value;
                qlen = mcv.GetQueueLen();
                var set = this.OrderByDistanceDesc(mcv, fx);
                if (set.IsEmpty) continue;
                var tasks = this.NodesInMyLayer(mcv, tf);

                if (tasks.IsEmpty) 
                {
                    tasks = this.NodesInMyLayer(mcv, tf2);
                    remove = true;// remove all tasks 
                }
                i = 0;
                foreach( var task in tasks)
                {
                    if (task.task_status != ns_task_status.created && !remove) continue;
                    //@ begin note:
                    //// if the MCV completed all the tasks assigned to it.
                    /// then it will assign tasks to it. from other MCVs who share the 
                    /// same layer.
                    if( remove && task.task_status == ns_task_status.assigned)
                    {
                        string a = task.assigned_to;
                        if( a!=string.Empty && mcvs.ContainsKey(a) && a!=mcv.tag)
                        {
                            mcvs[task.assigned_to].RemoveTask(task); //remove the task.
                        }
                    }
                    // @end note
                    if( task.is_Schedulable(mcv))
                    {
                        if ((i + qlen) >= max_count) break;
                        i++;
                        this.UpdateTaskStatus(task, ns_task_status.assigned);
                        task.task_status = ns_task_status.assigned;
                        this.UpdateAssignedMCV(task, mcv);
                        //task.assigned_to = mcv.tag;
                        mcv.PushTask(task);
                        continue;
                    }
                   // i++;
                }

            }
       }

        public void AssignTasks_BySector(Dictionary<string, mobile_charger> mcvs)
        {
            __order_ fx = (n1, n2) =>
            {
                return n1.edistance(n2);// / n1.GetImportanceValue();
            };
            __assign_task tf = (t1, mcvx) =>
            {
                var node = t1.The_node;
                return t1.task_status == ns_task_status.created && node.SectorId == mcvx.SectorId;
            };
            __assign_task tf2 = (t1, mcvx) =>
            {
                var node = t1.The_node;
                return t1.task_status != ns_task_status.completed &&  node.SectorId == mcvx.SectorId;
            };

            int max_count = 7;// (int)mcvs.First().Value.optimization_constant;
            int i = 0;
            int qlen = 0;
            double scaler = 0.0;
            //bool remove = false;
            foreach (var mc in mcvs)
            {
                var mcv = mc.Value;
                qlen = mcv.GetQueueLen();
                if (qlen >= max_count) continue;
               // var set = this;
                var set = this.OrderByDistanceDesc(mcv, fx);
                //if (set.IsEmpty) continue;
                var tasks = set.NodesInMySector(mcv, tf);

                if (tasks.IsEmpty)
                {
                    tasks = set.NodesInMySector(mcv, tf2);
                    //remove = true;// remove all tasks 
                }
                i = 0;
                scaler = 0.0;
                foreach (var task in tasks)
                {
                    if (task.task_status == ns_task_status.assigned) continue;
                    if (task.task_status == ns_task_status.released) continue;
                    if (task.task_status == ns_task_status.completed) continue;
                    
                    // @end note
                    //if (task.is_Schedulable(mcv))
                    {
                        if ((i + qlen) >= max_count) break;
                        i++;
                        this.UpdateTaskStatus(task, ns_task_status.assigned);
                        task.task_status = ns_task_status.assigned;
                        this.UpdateAssignedMCV(task, mcv);
                        scaler += task.The_node.GetImportanceValue();
                        //task.assigned_to = mcv.tag;
                        mcv.PushTask(task);
                        continue;
                    }
                    // i++;
                }
                // set the scaler here
                if (scaler != 0.0 && i > qlen) 
                    mcv.ChargeTimeScaler = scaler;
               // scaler = 0.0;// reset. the scaler 

            }
        }
        public void AssignTasks_BySectorDirectPush(Dictionary<string, mobile_charger> mcvs)
        {
            __order_ fx = (n1, n2) =>
            {
                return n1.edistance(n2);// / n1.GetImportanceValue();
            };
            __assign_task tf = (t1, mcvx) =>
            {
                var node = t1.The_node;
                return t1.task_status == ns_task_status.created && node.SectorId == mcvx.SectorId;
            };
            __assign_task tf2 = (t1, mcvx) =>
            {
                var node = t1.The_node;
                return t1.task_status != ns_task_status.completed && node.SectorId == mcvx.SectorId;
            };

            //int max_count = 7;// (int)mcvs.First().Value.optimization_constant;
            int i = 0;
            int qlen = 0;
           // double scaler = 0.0;
            //bool remove = false;
            foreach (var mc in mcvs)
            {
                var mcv = mc.Value;
                qlen = mcv.GetQueueLen();
               // if (qlen >= max_count) continue;
                 var set = this;
                //var set = this.OrderByDistanceDesc(mcv, fx);
                //if (set.IsEmpty) continue;
                var tasks = set.NodesInMySector(mcv, tf);

                if (tasks.IsEmpty)
                {
                    tasks = set.NodesInMySector(mcv, tf2);
                    //remove = true;// remove all tasks 
                }
                i = 0;
                //scaler = 0.0;
                foreach (var task in tasks)
                {
                    if (task.task_status == ns_task_status.assigned) continue;
                    if (task.task_status == ns_task_status.released) continue;
                    if (task.task_status == ns_task_status.completed) continue;

                    // @end note
                    //if (task.is_Schedulable(mcv))
                    {
                       // if ((i + qlen) >= max_count) break;
                        i++;
                        this.UpdateTaskStatus(task, ns_task_status.assigned);
                        task.task_status = ns_task_status.assigned;
                        this.UpdateAssignedMCV(task, mcv);
                       // scaler += task.The_node.GetImportanceValue();
                        //task.assigned_to = mcv.tag;
                        mcv.PushTask(task);
                        continue;
                    }
                    // i++;
                }
                // set the scaler here
              //  if (scaler != 0.0 && i > qlen)
                   // mcv.ChargeTimeScaler = scaler;
                // scaler = 0.0;// reset. the scaler 

            }
        }
       public bool IsEmpty
        {
           get
            {
                return this.Count == 0;
            }
        }
       public void AssignTasks_LayeredBased(Dictionary<string, mobile_charger> MCVs)
         {
             var set = this;
             int i = 0;
             foreach (var task in set)
             {
                 if (task.task_status != ns_task_status.created) continue;
                 var mcv = getMCV(MCVs, task);
                 if (task.is_Schedulable(mcv))
                 {
                     this.getTask(task.The_node).ChangeTaskStatus(ns_task_status.assigned);
                     task.task_status = ns_task_status.assigned;
                     mcv.PushTask(task);
                     i++;
                 }
             }
         }
        /// <summary>
        /// look for an assignedd tasks
        /// </summary>
        /// <param name="mcv"></param>
        /// <returns></returns>
        public ns_task_set NodesInMyLayer(mobile_charger mcv)
       {
           ns_task_set set = new ns_task_set();
            foreach(var task in this)
            {
                if(task.task_status!=ns_task_status.completed)
                {
                    bool is_in_layer = mcv.IsNodeInMyLayer(task.The_node);
                    if (!is_in_layer) continue;
                    set.addTask(task);
                }
                
            }
           return set;
       }
        public delegate bool __assign_task(ns_charge_task task, mobile_charger mcv);
        public ns_task_set NodesInMyLayer(mobile_charger mcv, __assign_task assign)
        {
            ns_task_set set = new ns_task_set();
            foreach (var task in this)
            {
                if (assign(task, mcv))
                {
                    //bool is_in_layer = mcv.IsNodeInMyLayer(task.The_node);
                    //if (!is_in_layer) continue;
                    if (task.IsReleased) continue; // don't add.
                    set.addTask(task);
                }

            }
            return set;
        }
        public void AddNearestTask(mobile_charger mcv,Dictionary<string,mobile_charger> mcvs)
        {
            var set = NodesInMyLayer(mcv, (task, mcvx) =>
            {
                return task.task_status != ns_task_status.completed && mcvx.IsInMyLayer(task);
            });
            if (set.Count == 0) return;
            var order_set = set.OrderByDistanceDesc(mcv, (n, m) =>
            {
                return n.edistance(m);
            });
            var first = order_set.First();
            // closer node 
            if(first.IsAssigned)
            {
                string assigned_to = first.assigned_to;
                if (assigned_to == string.Empty || assigned_to == mcv.tag 
                    || !mcvs.ContainsKey(assigned_to)) return;
               // first.assigned_to = mcv.tag;
                this.UpdateAssignedMCV(first, mcv);
                mcvs[assigned_to].RemoveTask(first);
            }
            mcv.PushTask(first); // next node.

        }
        public void AssignSingleTaskFx(mobile_charger mcv,Dictionary<string,mobile_charger> mcvs)
        {
            var set = NodesInMyLayer(mcv, (task, mcvx) =>
                {
                    return task.task_status != ns_task_status.completed && mcvx.IsInMyLayer(task);
                });
            if (set.Count == 0) return;

            var order_set = set.OrderByDistanceDesc(mcv, (n, m) =>
                {
                    return n.edistance(m);
                });
            var first = order_set.First();
            int qlen = mcvs[first.assigned_to].GetQueueLen();
            int take = qlen >= 4 ? 2 : 1;
            var taken = order_set.Take(take);
            bool release = false;
            foreach (var task_ in taken)
            {
                this.UpdateTaskStatus(task_, ns_task_status.assigned);
                if (task_.assigned_to != string.Empty) 
                mcvs[task_.assigned_to].RemoveTask(task_);// remove task from MCV's queue.
                //task_.assigned_to = mcv.tag;
                UpdateAssignedMCV(task_, mcv);
                mcv.PushTask(task_);
                release = true;
            }
            if(release)
            mcv.ReleaseTasks();
            

        }
        public void AssignSingleTaskFxUseSector(mobile_charger mcv, Dictionary<string, mobile_charger> mcvs)
        {
            var set = NodesInMyLayer(mcv, (task, mcvx) =>
            {
                var node = task.The_node;
                return task.task_status != ns_task_status.completed && mcvx.SectorId == node.SectorId;
            });
            if (set.Count == 0) return;

            var order_set = set.OrderByDistanceDesc(mcv, (n, m) =>
            {
                return n.edistance(m);
            });
            var first = order_set.First();
            int qlen = mcvs[first.assigned_to].GetQueueLen();
            int take = qlen >= 4 ? 2 : 1;
            var taken = order_set.Take(take);
            bool release = false;
            foreach (var task_ in taken)
            {
                this.UpdateTaskStatus(task_, ns_task_status.assigned);
                if (task_.assigned_to != string.Empty)
                    mcvs[task_.assigned_to].RemoveTask(task_);// remove task from MCV's queue.
                //task_.assigned_to = mcv.tag;
                UpdateAssignedMCV(task_, mcv);
                mcv.PushTask(task_);
                release = true;
            }
            if (release)
                mcv.ReleaseTasks();


        }
        public delegate bool _count_of(ns_charge_task task);
        public int Countof(_count_of count_fx)
        {
            int sum = 0;
            foreach(var task in this)
            {
                if(count_fx(task))
                {
                    sum++;
                }
            }
            return sum;
        }
        public void AssignSingleTask(mobile_charger mcv, Dictionary<string, mobile_charger> mcvs)
        {
           // ns_charge_task task = this[0];
            
            var set = NodesInMyLayer(mcv);
            if (set.getCount() == 0) return;
            double min_dist = 0;
            double dist = 0.0;
            string min_task_id = set[0].task_id;
            int i = 0;
            foreach( var tk in set)
            {
                dist = mcv.edistance(tk.The_node);
                if( i==0)
                {
                    min_dist = dist;
                    min_task_id = tk.task_id;
                    i++;
                    continue;
                }
                //min_qlen = qlen <= min_qlen ? qlen : min_qlen;
                min_dist = min_dist <= dist ? min_dist : dist;
                min_task_id = min_dist == dist ? tk.task_id : min_task_id;
                i++;
            }
            var task =  set.getTaskById(min_task_id);
            mcv.printfx("Smart Switch: {0}->{1}", mcv.tag, task.task_id);
            mcv.PushTask(task);
            
            mcv.ReleaseTasks();
            string assigned_to = task.assigned_to;
            if(!string.IsNullOrEmpty(assigned_to))
            {
                mcvs[assigned_to].RemoveTask(task);
                
            }

        }
       private mobile_charger getMCV(Dictionary<string, mobile_charger> MCVs, ns_charge_task task)
       {
           string id = MCVs.First().Key;
           ns_node node = task.The_node;
           double min_dist = 0;

           foreach( var mc in MCVs)
           {
               var mcv = mc.Value;
               var distance = node.edistance(mcv);
               if(mcv.IsInMyPrimaryLayers(task))
               {
                   if( distance < min_dist && min_dist!=0)
                   {
                       id = mcv.tag;
                       min_dist = distance;
                       continue;
                   }
                   min_dist = distance;
                   id = mcv.tag;

               }

           }

           return MCVs[id];
       }
        public void RemoveTask(string task_id)
        {
            var task = getTaskById(task_id);
            this.Remove(task);
        }
        public int getCount()
        {
            return this.Count;
        }
        public void UpdateTaskStatus(ns_node node , ns_task_status status)
        {
            var task = getTask(node);
            task.task_status = status;
            updateTimeNow(task);
           // Remove(task);
        }
        private void updateTimeNow(ns_charge_task task)
        {
            var now = DateTime.Now;
            if( task.task_status == ns_task_status.created)
            {
                task.Task_creationTime = now;
                return;
            }
            if (task.task_status == ns_task_status.completed)
            {
                task.CompleteTime = now;
                return;
            }

            if (task.task_status == ns_task_status.released)
            {
                task.ReleaseTime = now;
                return;
            }
            if (task.task_status == ns_task_status.assigned)
            {
                task.AssignTime = now;
                return;
            }
        }
        public void UpdateTaskStatus(ns_charge_task task, ns_task_status status)
        {
            foreach (var t in this)
            {
                if (t.task_id == task.task_id)
                {
                    t.task_status = status;
                    updateTimeNow(t);
                    break;
                }
            }

        }
        //// Reports Start Here. 
        public delegate bool __report_(ns_charge_task task);

        public task_reports getReport(__report_ report_del)
        {
            task_reports report = new task_reports();
            foreach(var task in this)
            {
                if(report_del(task))
                {
                    report.count += 1;
                    report.total_energy += task.The_node.PowerConfig.BatteryCapacity;
                    continue;
                }
            }

            return report;
        }
        public ns_task_set GetSet(__report_ report_del)
        {
            ns_task_set set = new ns_task_set();
            foreach (var task in this)
            {
                if (report_del(task))
                {
                    set.Add(task);
                    //report.count += 1;
                   // report.total_energy += task.The_node.PowerConfig.BatteryCapacity;
                    continue;
                }
            }
            return set;
        }
        /// <summary>
        /// for every sector theres a set of nodes
        /// </summary>
        /// <param name="report_del"></param>
        /// <returns></returns>
        public Dictionary<int, ns_task_set> GroupBySector(__report_ report_del)
        {
            Dictionary<int, ns_task_set> dict = new Dictionary<int, ns_task_set>();
            var set = GetSet(report_del);
            ///if (set.Count == 0) return dict;
            foreach( var task in set)
            {
                int sector = task.SectorId;
                if( dict.ContainsKey(sector))
                {
                    dict[sector].Add(task);
                    continue;
                }
                dict[sector] = new ns_task_set();
                dict[sector].Add(task);
               // continue;
            }

            return dict;
        }
    }


    public enum ns_task_status
    {
        created = 0 , 
        released = 1,
        completed = 2,
        assigned = 3,
        re_assigned =4,
    }
    public class ns_charge_task
    {
        // worest execution time.
        public double C_i ;//{ get; set; } // 
        // Arrival time of a task 
        public double t_a ;//{ get; set; }

        // inter-arrival time of the task.
        public double T_i ;//{ get; set; }
        // the deadline of the task.
        public double deadline ;//{ get; set; }

        /// <summary>
        /// task priority.
        /// </summary>
        public double priority;// { get; set; }
        /// <summary>
        /// The response time of a task. it's the estimated time that an MCV will reach
        /// the reach the node.
        /// </summary>
        public double R_i ;//{ get; set; }
        public string task_id;
        public ns_task_status task_status;// { get; set; }
        public ns_node The_node;// we'll used it later.
        public int task_importance;
        public DateTime Task_creationTime;
        public DateTime ReleaseTime;
        public DateTime CompleteTime;
        public DateTime AssignTime;

        /// <summary>
        /// this is tasks is assigned to an MCV with the correspoding ID denoted as assigned_to
        /// </summary>
        public string assigned_to = string.Empty;
        public ns_charge_task(ns_node node, mobile_charger mcv)
        {
            this.task_id = node.tag;
            this.task_status = ns_task_status.created;
            this.T_i = 1;
            this.The_node = node;
            this.Task_creationTime = DateTime.Now;
            init(node, mcv);
        }
        public static double getTimeDifferenceInSecs(DateTime dt1, DateTime dt2)
        {
            return dt2.Subtract(dt2).TotalSeconds;
        }
        public ns_charge_task (ns_node node)
        {
            this.task_id = node.tag;
            this.task_status = ns_task_status.created;
            this.T_i = 1;
            this.The_node = node;
            this.task_importance = node.GetImportanceValue();
            this.Task_creationTime = DateTime.Now;

        }
        private void init(ns_node node , mobile_charger mcv)
        {
            double e_max = node.PowerConfig.BatteryCapacity;
            double e_c = node.PowerConfig.energy_consumptionRate;

            this.deadline = e_max / e_c;

            double distance = mcv.edistance(node);

            double t_travel = distance / mcv.mcv_speed;

            this.R_i = t_travel;

            this.priority = e_max * distance;// initial priority.
        }
        public double getWaitingTime()
        {
            return DateTime.Now.Subtract(this.Task_creationTime).TotalSeconds;
        }
        public double TravelTime(mobile_charger mcv)
        {
            double distance = mcv.edistance(this.The_node);

            return distance / mcv.mcv_speed;
        }
        public double getResponseTime(mobile_charger mcv)
        {
            return this.TravelTime(mcv);
        }
        public double TaskArriveTime
        {
            get
            {   if( this.task_status == ns_task_status.created)
                return DateTime.Now.Subtract(this.Task_creationTime).TotalSeconds;
            return 0;
            }
        }
        /// <summary>
        /// release_time - assigned_time;
        /// </summary>
        public double TaskResponseTime
        {
            get
            {
               // if (this.task_status == ns_task_status.released)
                    return this.ReleaseTime.Subtract(this.AssignTime).TotalSeconds;
               // return 0.0;
            }
        }
        /// <summary>
        /// the time a node wait, not assigned nor completed.
        /// </summary>
        public double QueueWaitingTime
        {
            get
            {
                //if( this.task_status == ns_task_status.assigned)
                return this.ReleaseTime.Subtract(this.Task_creationTime).TotalSeconds;
            }
        }
        /// <summary>
        /// the charging time.
        /// complete_time - release_time.
        /// </summary>
        public double TaskServiceTime
        {
            get
            {
               // if (this.task_status == ns_task_status.completed)
                    return CompleteTime.Subtract(ReleaseTime).TotalSeconds;
                //return 0;
            }
        }
        public bool IsAssigned
        {
            get
            {
                return this.task_status == ns_task_status.assigned;
            }
        }
        public bool IsReleased
        {
            get
            {
                return this.task_status == ns_task_status.released;
            }
        }
        public double getDeadLine()
        {
            double e_max = this.The_node.PowerConfig.BatteryCapacity;
            double e_c = this.The_node.PowerConfig.energy_consumptionRate;
            return e_max / e_c;
        }
        /// <summary>
        /// CHeck if the task is schedulable with corresponding MCV.
        /// </summary>
        /// <param name="mcv"></param>
        /// <returns></returns>
        public bool is_Schedulable(mobile_charger mcv)
        {
            double distance = mcv.edistance(this.The_node);

            double R_i =  distance / mcv.mcv_speed;

            double e_max = this.The_node.PowerConfig.BatteryCapacity;
            double e_c = this.The_node.PowerConfig.energy_consumptionRate;

            double deadline = e_max / e_c;

            return R_i <= deadline;
        }
        public void ChangeTaskStatus(ns_task_status status)
        {
            this.task_status = status;
        }
        public int SectorId
        {
            get
            {
                return this.The_node.SectorId;
            }
        }
    }

    public class scheduling
    {

    }
}
