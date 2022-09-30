using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.graph
{
    public class motions
    {
        public static double sin(double x)
        {
            return Math.Sin(x);
        }
        public static double cos(double x)
        {
            return Math.Cos(x);
        }

        public static void TravelTo(ns_point ui_point , Dictionary<string, ns_node> path, double speed , Action<ns_point> motion_handler)
        {
            Action<ns_point, ns_point> act = (p1, p2) =>
            {
                return;
            };
            Queue<ns_node> nodes = new Queue<ns_node>();
            foreach (var node in path)
            {
                var point = node.Value.location;
                nodes.Enqueue(node.Value);
              //  ui_point = MoveToPointVx(ui_point, point, speed, motion_handler, act);
            }
            MoveToPointVxPath(ui_point, nodes, speed, motion_handler);
        }
        public static void TravelToPQ(ns_point ui_point, Dictionary<string, ns_node> path, double speed, Action<ns_point> motion_handler)
        {
            Action<ns_point, ns_point> act = (p1, p2) =>
            {
                return;
            };
            PriorityQueue<double, ns_node> nodes = new PriorityQueue<double, ns_node>((p1, p2) =>
            {
                return p1.priority <= p2.priority;
            });
            foreach (var node in path)
            {
                if( node.Value.location == ui_point) continue;
                var point = node.Value.location;
                var priority = point.edistance(ui_point);
                nodes.Enqueue(node.Value, priority);
                //  ui_point = MoveToPointVx(ui_point, point, speed, motion_handler, act);
            }
            MoveToPointVxPathPQ(ui_point, nodes, speed, motion_handler);
        }
        public static void TravelTo(ns_point ui_point , List<ns_node> path , double speed , Action<ns_point> motion_handler)
        {
            Action<ns_point, ns_point> act = (p1, p2) =>
            {
                return;
            };
            Queue<ns_node> nodes = new Queue<ns_node>();
            foreach (var node in path)
            {
               // var point = node.Value.location;
                nodes.Enqueue(node);
                //  ui_point = MoveToPointVx(ui_point, point, speed, motion_handler, act);
            }
            MoveToPointVxPath(ui_point, nodes, speed, motion_handler);
        }
        public static void updatePriorities(PriorityQueue<double , ns_node> pq , ns_point cur)
        {
            pq.Foreach((n) =>
                {
                    n.priority = cur.edistance(n.Value.location);
                });
        }
        public static bool MoveToPointVxPathPQ(ns_point car, PriorityQueue<double, ns_node> queue, double mcv_speed, Action<ns_point> motion_handler)
        {
            //ns_Point res = new ns_Point() { x = 0, y = 0, State = ns_Point.PointState.start };
            if (queue.size() == 0) return false;
            ns_point res = new ns_point();
            updatePriorities( queue , car);
            ns_point node = queue.Dequeue().Value.location ;

            if (car == node)
            {
                if (queue.size() == 0) return false;
                //if (queue.Count != 0)
                MoveToPointVxPathPQ(res, queue, mcv_speed, motion_handler);

            };
           
            var x0 = car.x;
            var y0 = car.y;
            var x1 = node.x;
            var y1 = node.y;
            var t = 1;// 0.0;

            var tx = 1000 / 38;// 0.0264583333 * 1000;// / 10; initial time;
            // log("Start Moving Car,,,,");
            var theta = car.angle2(node);
            var dx = car.edistance(node);
            var v_mc = mcv_speed;
            var a = (x1 - x0) > 0 ? 1 : -1; // positive or neg acceleration.
            //ax = theta > 0 ? -1 * ax : ax;
            var TimeRequired = 1000 * dx / (38 * v_mc);//tx * calcTime(vx, Math.Abs(ax), dx, Math.PI / 2);

            string _log = string.Format("Time Required:{0} , distance:{1}", TimeRequired, dx);
            //log(_log);
            System.Timers.Timer timer = new System.Timers.Timer(38);

            double d = 0;
            timer.Elapsed += (e, arg) =>
            {

                try
                {
                    d = v_mc * t * a / tx;

                    var xf = (x0 + d * cos(theta));
                    var yf = (y0 + d * sin(theta));

                    res.x = xf;
                    res.y = yf;
                    // string fmt = string.Format("xf:{0} , yf:{1} , time:{2}", xf, yf, t);


                    //log(fmt);
                    if (Math.Ceiling(d) == Math.Ceiling(dx) || t >= TimeRequired)
                    {

                        timer.Stop();
                        //stop_handler(car , tnode);
                        if (queue.size() != 0)
                            MoveToPointVxPathPQ(res, queue, mcv_speed, motion_handler);
                    }
                    motion_handler(res);

                    t = (t + 1);///(div);
                }
                catch (Exception)
                {

                    // log("Ex:" + ex.Message);
                    timer.Stop();
                }

            };
            timer.Start();
            // log("Timer after Start.");

            return true;
        }
        public static bool MoveToPointVxPath(ns_point car, Queue<ns_node> queue, double mcv_speed, Action<ns_point> motion_handler)
        {
            //ns_Point res = new ns_Point() { x = 0, y = 0, State = ns_Point.PointState.start };
            if (queue.Count == 0) return false;
            ns_point res = new ns_point();
            ns_point node = queue.Dequeue().location;

            if (car == node)
            {
                if (queue.Count == 0) return false;
                //if (queue.Count != 0)
                MoveToPointVxPath(res, queue, mcv_speed, motion_handler);

            };

            var x0 = car.x;
            var y0 = car.y;
            var x1 = node.x;
            var y1 = node.y;
            var t = 1;// 0.0;

            var tx = 1000 / 38;// 0.0264583333 * 1000;// / 10; initial time;
            // log("Start Moving Car,,,,");
            var theta = car.angle2(node);
            var dx = car.edistance(node);
            var v_mc = mcv_speed;
            var a = (x1 - x0) > 0 ? 1 : -1; // positive or neg acceleration.
            //ax = theta > 0 ? -1 * ax : ax;
            var TimeRequired = 1000 * dx / (38 * v_mc);//tx * calcTime(vx, Math.Abs(ax), dx, Math.PI / 2);

            string _log = string.Format("Time Required:{0} , distance:{1}", TimeRequired, dx);
            //log(_log);
            System.Timers.Timer timer = new System.Timers.Timer(38);

            double d = 0;
            timer.Elapsed += (e, arg) =>
            {

                try
                {
                    d = v_mc * t * a / tx;

                    var xf = (x0 + d * cos(theta));
                    var yf = (y0 + d * sin(theta));

                    res.x = xf;
                    res.y = yf;
                    // string fmt = string.Format("xf:{0} , yf:{1} , time:{2}", xf, yf, t);


                    //log(fmt);
                    if (Math.Ceiling(d) == Math.Ceiling(dx) || t >= TimeRequired)
                    {
                        
                        timer.Stop();
                        //stop_handler(car , tnode);
                        if(queue.Count!=0)
                        MoveToPointVxPath(res, queue, mcv_speed, motion_handler);
                    }
                    motion_handler(res);

                    t = (t + 1);///(div);
                }
                catch (Exception)
                {

                   // log("Ex:" + ex.Message);
                    timer.Stop();
                }

            };
            timer.Start();
            // log("Timer after Start.");

            return true;
        }

        ///-----------
        ///
        public static ns_point MoveToPointVx(ns_point car, ns_point tnode, double mcv_speed, Action<ns_point> motion_handler, Action<ns_point, ns_point> stop_handler)
        {
            //ns_Point res = new ns_Point() { x = 0, y = 0, State = ns_Point.PointState.start };

            ns_point res = new ns_point();
            ns_point node = tnode;

            if (car == tnode) return car;

            var x0 = car.x;
            var y0 = car.y;
            var x1 = node.x;
            var y1 = node.y;
            var t = 1;// 0.0;

            var tx = 1000 / 38;// 0.0264583333 * 1000;// / 10; initial time;
            // log("Start Moving Car,,,,");
            var theta = car.angle2(node);
            var dx = car.edistance(node);
            var v_mc = mcv_speed;
            var a = (x1 - x0) > 0 ? 1 : -1; // positive or neg acceleration.
            //ax = theta > 0 ? -1 * ax : ax;
            var TimeRequired = 1000 * dx / (38 * v_mc);//tx * calcTime(vx, Math.Abs(ax), dx, Math.PI / 2);

            string _log = string.Format("Time Required:{0} , distance:{1}", TimeRequired, dx);
            //log(_log);
            System.Timers.Timer timer = new System.Timers.Timer(38);

            double d = 0;
            timer.Elapsed += (e, arg) =>
            {

                try
                {
                    d = v_mc * t * a / tx;

                    var xf = (x0 + d * cos(theta));
                    var yf = (y0 + d * sin(theta));

                    res.x = xf;
                    res.y = yf;
                    // string fmt = string.Format("xf:{0} , yf:{1} , time:{2}", xf, yf, t);


                    //log(fmt);
                    if (Math.Ceiling(d) == Math.Ceiling(dx) || t >= TimeRequired)
                    {

                        timer.Stop();
                        stop_handler(car, tnode);
                    }
                    motion_handler(res);

                    t = (t + 1);///(div);
                }
                catch (Exception)
                {

                    // log("Ex:" + ex.Message);
                    timer.Stop();
                }

            };
            timer.Start();
            // log("Timer after Start.");

            return res;
        }
    }
}
