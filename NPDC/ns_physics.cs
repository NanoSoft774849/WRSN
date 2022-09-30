using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace NPDC
{
    public class ns_Physics
    {

        public static ns_point goBackHome(mobile_charger car, Action<string > log , Action<ns_point> timer_handler)
        {
            ns_point p = car.getOriginLocation();
            return MoveToPoint(car, p.x, p.y, log, timer_handler);
        }
        public static ns_point MoveToPointX(mobile_charger  car, double x, double y,double v, Action<string> log, Action<ns_point> timer_handler)
        {
            ns_point res = new ns_point();
            ns_point node = new ns_point(x, y);
            double distance = car.location.edistance(node);

            double time = distance / v;

            return res;
        }
        public static ns_point MoveToPoint(mobile_charger  car, double x, double y, Action<string> log, Action<ns_point> timer_handler)
        {
            ns_point res = new ns_point();
            ns_point node = new ns_point(x,y);
          
           
            var x0 = car.location.x;
            var y0 = car.location.y;
            var x1 = node.x;
            var y1 = node.y;
            var t = 0.0;
            var ax = 30.795275591; // acceleration
            var vx = 0.001; // initial velocity .
            var tx = 0.0264583333 * 1000;// / 10; initial time;
            // log("Start Moving Car,,,,");
            var theta = car.location.angle2(node);
            var dx = car.location.edistance(node);

            ax = (x1 - x0) > 0 ? ax : -1 * ax; // positive or neg acceleration.

            var TimeRequired = tx * calcTime(vx, Math.Abs(ax), dx, Math.PI / 2);

            string _log = string.Format("Time Required:{0} , distance:{1}", TimeRequired, dx);
            // log(_log);
            System.Timers.Timer timer = new System.Timers.Timer(38);

            timer.Elapsed += (e, arg) =>
            {

                try
                {
                    var d = CalcDisplacement2(vx, ax, t / tx); // tx

                    var xf = (x0 + d * cos(theta));//*Math.cos(t);
                    var yf = (y0 + d * sin(theta));

                    res.x = xf;
                    res.y = yf;
                    //string fmt = string.Format("xf:{0} , yf:{1} , time:{2}", xf, yf, t);


                    //log(fmt);
                    if (Math.Ceiling(d) == Math.Ceiling(dx) || t >= TimeRequired)
                    {
                        //  log("Timer Stop.");
                       // car.MCStatus  =  Mc_Status.charging;
                        //car.ChargingInProcess();
                        //res.State = ns_Point.PointState.end;
                        car.location = node;
                        car.calc_power_consumed_per_distance(d);
                        //car.
                        timer.Stop();
                    }
                    timer_handler(res);

                    t = t + 1;
                }
                catch (Exception ex)
                {

                    log("Ex:"+ ex.Message);
                    timer.Stop();
                }

            };
            timer.Start();
            // log("Timer after Start.");

            return res;
        }
        public static ns_point MoveToPoint(mobile_charger  car, ns_node tnode, Action<string> log, Action<ns_point> timer_handler)
        {
            //ns_Point res = new ns_Point() { x = 0, y = 0, State = ns_Point.PointState.start };
            if(car.location == tnode.location)
            {
                car.MCStatus = Mc_Status.stationary;
                return car.location;
            }
            ns_point res = new ns_point();
            ns_point node = tnode.location;


            var x0 = car.location.x;
            var y0 = car.location.y;
            var x1 = node.x;
            var y1 = node.y;
            var t = 0.0;
            var ax = 1.795275591; // acceleration
            var vx = 0.001; // initial velocity .
            var tx = 0.0264583333 * 1000;// / 10; initial time;
            // log("Start Moving Car,,,,");
            var theta = car.location.angle2(node);
            var dx = car.location.edistance(node);

            ax = (x1 - x0) > 0  ? ax : -1 * ax; // positive or neg acceleration.
            //ax = theta > 0 ? -1 * ax : ax;
            var TimeRequired = tx * calcTime(vx, Math.Abs(ax), dx, Math.PI / 2);

            string _log = string.Format("Time Required:{0} , distance:{1}", TimeRequired, dx);
            log(_log);
            System.Timers.Timer timer = new System.Timers.Timer(38);

            timer.Elapsed += (e, arg) =>
            {

                try
                {
                    var d = CalcDisplacement2(vx, ax, t / tx); // tx

                    var xf = (x0 + d * cos(theta));//*Math.cos(t);
                    var yf = (y0 + d * sin(theta));

                    res.x = xf;
                    res.y = yf;
                    //string fmt = string.Format("xf:{0} , yf:{1} , time:{2}", xf, yf, t);


                    //log(fmt);
                    if (Math.Ceiling(d) == Math.Ceiling(dx) || t >= TimeRequired)
                    {
                        //  log("Timer Stop.");
                        _log = string.Format("v: {2} ,Time Required:{0} , distance:{1}", t, d,
                            dx/t);
                        log(_log);
                        car.MCStatus = Mc_Status.charging;
                        car.location = tnode.location;
                        car.ChargingInProcess();
                        car.SetCurrentChargeNode(tnode);
                       // tnode.ChargingInProcess();
                        //res.State = ns_Point.PointState.end;
                        timer.Stop();
                    }
                    timer_handler(res);

                    t = t + 1;
                }
                catch (Exception ex)
                {

                    log("Ex:" + ex.Message);
                    timer.Stop();
                }

            };
            timer.Start();
            // log("Timer after Start.");

            return res;
        }

        public static ns_point MoveToPointVx(mobile_charger car, ns_node tnode, Action<string> log, Action<ns_point> timer_handler)
        {
            //ns_Point res = new ns_Point() { x = 0, y = 0, State = ns_Point.PointState.start };
            if (car.location == tnode.location)
            {
                car.MCStatus = Mc_Status.stationary;
                return car.location;
            }
            ns_point res = new ns_point();
            ns_point node = tnode.location;


            var x0 = car.location.x;
            var y0 = car.location.y;
            var x1 = node.x;
            var y1 = node.y;
            var t = 1;// 0.0;
            
            var tx = 1000 / 38;// 0.0264583333 * 1000;// / 10; initial time;
            // log("Start Moving Car,,,,");
            var theta = car.location.angle2(node);
            var dx = car.location.edistance(node);
            var v_mc = car.mcv_speed;
            var a = (x1 - x0) > 0 ? 1 : -1 ; // positive or neg acceleration.
            //ax = theta > 0 ? -1 * ax : ax;
            var TimeRequired = 1000*dx/(38*v_mc);//tx * calcTime(vx, Math.Abs(ax), dx, Math.PI / 2);

            string _log = string.Format("Time Required:{0} , distance:{1}", TimeRequired, dx);
             //log(_log);
            System.Timers.Timer timer = new System.Timers.Timer(38);
           
            double d = 0;
            timer.Elapsed += (e, arg) =>
            {

                try
                {
                    d = v_mc * t*a / tx; 
                    /// m/s = > unit inve => s/m 

                    var xf = (x0 + d * cos(theta));
                    var yf = (y0 + d * sin(theta));

                    res.x = xf;
                    res.y = yf;
                   // string fmt = string.Format("xf:{0} , yf:{1} , time:{2}", xf, yf, t);


                    //log(fmt);
                    if (Math.Ceiling(d) == Math.Ceiling(dx) || t >= TimeRequired)
                    {
                        //  log("Timer Stop.");
                        _log = string.Format("v: {2} ,Time Required:{0} , distance:{1}", t, d,
                             1000*d*a /(38* t));
                        log(_log);
                        /// MCV arrived .
                        car.MCStatus = Mc_Status.charging;
                        car.location = tnode.location;
                        car.ChargingInProcess();
                        car.SetCurrentChargeNode(tnode);
                        // tnode.ChargingInProcess();
                        //res.State = ns_Point.PointState.end;
                        timer.Stop();
                    }
                    timer_handler(res);

                    t = (t + 1);///(div);
                }
                catch (Exception ex)
                {

                    log("Ex:" + ex.Message);
                    timer.Stop();
                }

            };
            timer.Start();
            // log("Timer after Start.");

            return res;
        }

        public static ns_point MoveToPointVx(mobile_charger car, ns_node tnode, double mcv_speed, Action<string> log, Action<ns_point> timer_handler)
        {
            //ns_Point res = new ns_Point() { x = 0, y = 0, State = ns_Point.PointState.start };
            if (car.location == tnode.location)
            {
                car.MCStatus = Mc_Status.stationary;
                return car.location;
            }
            ns_point res = new ns_point();
            ns_point node = tnode.location;


            var x0 = car.location.x;
            var y0 = car.location.y;
            var x1 = node.x;
            var y1 = node.y;
            var t = 1;// 0.0;

            var tx = 1000 / 38;// 0.0264583333 * 1000;// / 10; initial time;
            // log("Start Moving Car,,,,");
            var theta = car.location.angle2(node);
            var dx = car.location.edistance(node);
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
                        //  log("Timer Stop.");
                        _log = string.Format("v: {2} ,Time Required:{0} , distance:{1}", t, d,
                             1000 * d * a / (38 * t));
                        log(_log);
                        car.MCStatus = Mc_Status.charging;
                        car.location = tnode.location;
                        car.ChargingInProcess();
                        car.SetCurrentChargeNode(tnode);
                        // tnode.ChargingInProcess();
                        //res.State = ns_Point.PointState.end;
                        timer.Stop();
                    }
                    timer_handler(res);

                    t = (t + 1);///(div);
                }
                catch (Exception ex)
                {

                    log("Ex:" + ex.Message);
                    timer.Stop();
                }

            };
            timer.Start();
            // log("Timer after Start.");

            return res;
        }
        public static double sqr(double x)
        {
            return x * x;
        }
        /// <summary>
        /// find the Time required to reach a specific distance.
        /// </summary>
        /// <param name="v0"> initial velocity</param>
        /// <param name="a"> accelaration</param>
        /// <param name="d"> distance </param>
        /// <param name="theta"> angle </param>
        /// <returns></returns>
        public static double calcTime(double v0, double a, double d, double theta)
        {
            var b = v0 * sin(theta);
            // var a = 0.5 * a;

            var sq = (b * b) + (2 * a * d);
            return 0.5 * ((-b) + Math.Sqrt(sq)) / (0.5 * a);
        }
        /// <summary>
        /// calculate  the displacement 
        /// </summary>
        /// <param name="v0">initial velocity</param>
        /// <param name="vf"> final velocity </param>
        /// <param name="t">time</param>
        /// <returns></returns>
        public static double calcDisplacement(double v0, double vf, double t)
        {
            return 0.5 * (v0 + vf) * t;
        }
        /// <summary>
        /// calc displacement based on acceleration
        /// Newton third law of motion.
        /// </summary>
        /// <param name="v0"> initial velocity </param>
        /// <param name="acceleration"> acceleration </param>
        /// <param name="t"> time </param>
        /// <returns></returns>
        public static double CalcDisplacement2(double v0, double acceleration, double t)
        {
            var v = CalcVelocity(v0, acceleration, t);

            return ((v0 * t) + (0.5 * acceleration * t * t));
        }

        public static double CalcVelocity(double initial_velocity, double acceleration, double _time)
        {
            return (initial_velocity + (acceleration * _time));
        }


        public static double sin(double x)
        {
            return Math.Sin(x);
        }
        public static double cos(double x)
        {
            return Math.Cos(x);
        }
        public static double angle(double x0, double y0, double x1, double y1)
        {
            var theta = (y1 - y0) / (x1 - x0);
            var ang = Math.Atan2(y1-y0, x1-x0);
            return ang;
        }
        public static double todeg(double theta)
        {
            return theta * 180 / Math.PI;
        }
    }
}
