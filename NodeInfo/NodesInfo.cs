

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Timers;
namespace NodeInfo
{
    public class ns_Timers
    {
        public Timer mTimer;
        public string Timer_id;
        public delegate void Timer_Routine(ns_Timers _timer);
        public enum TimerState
        {
            Created =0, 
            Started,
            Running ,
            Stopped,
            Suspended
        };
        public TimerState State;
        private double timer_period;

        public ns_Timers(Timer timer , string id)
        {
            this.mTimer = timer;
            this.Timer_id = id;
            this.State = TimerState.Created;
            this.timer_period = mTimer.Interval;
        }
        public ns_Timers ( double time_ms , string id , Timer_Routine handler)
        {
            this.timer_period = time_ms > 0 ? time_ms : 1000;
            this.mTimer = new Timer(time_ms > 0 ? time_ms : 1000);
            this.Timer_id = id;
            this.State = TimerState.Created;
            this.mTimer.Elapsed += (e, arg) =>
                {
                    handler(this);
                };
        }
        public ns_Timers(double time_ms, string id)
        {
            this.timer_period = time_ms > 0 ? time_ms : 1000;
            this.mTimer = new Timer(time_ms > 0 ? time_ms : 1000);
            this.Timer_id = id;
            this.State = TimerState.Created;
        }
        public ns_Timers StartTimer()
        {
            this.mTimer.Start();
            this.State = TimerState.Started;
            return this;
        }

        public ns_Timers  stopTimer()
        {
            this.mTimer.Stop();
            this.State = TimerState.Stopped;
            return this;
        }
        public ns_Timers SetHandler(Timer_Routine handler)
        {
            this.State = TimerState.Running;
            this.mTimer.Elapsed += (e,args) =>
                {
                    handler(this);
                };
            return this;
        }
    
    }
    public class Iterator<T>
    {
        private T[] _items;
        private object obj;
        public Iterator(T[] _xitems)
        {
            this._items = _xitems;
        }
        public Iterator(T[] _xitems, object _obj)
        {
            this._items = _xitems;
            obj = _obj;
        }
        public Iterator(List<T> _list)
        {
            this._items = _list.ToArray();
        }
        public delegate void _foreach(T item);
        public delegate bool _find(T item);
        public delegate bool _filter(T item);
        public delegate void _foreachi(T item, int i);
        public delegate void _foreach_obj(T ite, object _obj);
        public delegate bool _orderBy(T item);
        public delegate void _foreachInRange(T item, int index);
        public delegate void _foreach_inparallel(T item1, T item2);
        public delegate T _filter_and_Edit(T item);
        // public delegate bool _filterpat(params _filter[] _filters);
        public int Count
        {
            get { return _items.Length; }
        }
        public T this[int i]
        {
            get { return _items[i]; }
            set { _items[i] = value; }
        }
        public Iterator<T> foreachInRange(int st, int end, _foreachInRange _foreach)
        {
            int n = this.Count;
            int i = 0;
            for (i = st; i < end % n; _foreach(this[i % n], i), i++) ;

            return this;
        }
        public Iterator<T> InParallel(T item1_def, T item2_def, T[] _items2, _foreach_inparallel _inparallel)
        {
            int n = this.Count;
            int i = 0;
            int m = _items2.Length;

            int max = m > n ? m : n;
            for (i = 0; i < max; _inparallel(i >= n ? item1_def : this[i], i >= m ? item2_def : _items2[i]), i++) ;


            return this;
        }
        public Iterator<T> InParallel(int _count, T item1_def, T item2_def, T[] _items2, _foreach_inparallel _inparallel)
        {
            int n = this.Count;
            int i = 0;
            int m = _items2.Length;

            int max = _count;
            for (i = 0; i < max; _inparallel(i >= n ? item1_def : this[i], i >= m ? item2_def : _items2[i]), i++) ;


            return this;
        }
        public Iterator<T> InterLeaveBym(int m)
        {
            m = m < 0 ? -m : m;
            int n = this.Count;
            m = m % n;
            if (m == 0) return this;
            T[] temp = new T[n];
            int k1 = (n / m) + 1;
            int k2 = (n % m);
            int _j = 0;
            for (int i = 0; i < m; i++)
            {
                k1 = (i < k2) ? (1 + (n / m)) : (n / m);
                for (int j = 0; j < k1; j++)
                {
                    int _k = (j * m) + i;
                    temp[_j] = this[_k];
                    _j++;
                }
            }


            return new Iterator<T>(temp);
        }
        public Iterator<T> RotateByn(int j, bool _right)
        {
            int i = 0;
            int n = this.Count;
            int k = 0;
            T[] _temp = new T[n];

            for (i = 0; i < n; i++)
            {
                k = (n + j + i) % (n);
                k = _right ? (n - 1 - k) : k;
                _temp[k] = this[i];
            }

            return new Iterator<T>(_temp);
        }
        public Iterator<T> OrderBy(_orderBy _order)
        {
            T max = this[0];
            int tempIndex = 0;
            int j = 0, i = 0;
            int len = this.Count;
            T tempv = max;
            for (i = 0; i < len; i++)
            {
                tempv = this[i];
                max = tempv;
                tempIndex = i;
                for (j = 0; j < len - i; j++)
                {
                    max = _order(this[i + j]) ? this[i + j] : max;
                    tempIndex = _order(this[i + j]) ? i + j : tempIndex;
                }

                this[i] = max;
                this[tempIndex] = tempv;
            }


            return this;
        }
        public int IndexOf(_find __find)
        {
            int i = 0;
            for (i = 0; i < this.Count && !__find(this[i]); i++) ;
            return i == this.Count ? -1 : i;
        }
        public Iterator<T> Foreach(_foreach _for)
        {
            int i = 0, len = this.Count;
            for (i = 0; i < len; _for(this[i]), i++) ;


            return this;
        }
        public delegate bool _max(T item1, T item2);
        public T Max(_max _max)
        {
            T max = this[0];
            int i = 0;
            for (i = 0; i < this.Count; i++)
            {
                max = _max(max, this[i]) ? max : this[i];
            }
            return max;
        }
        public Iterator<T> ForeachDo(_foreach_obj _for)
        {
            int i = 0, len = this.Count;
            for (i = 0; i < len; _for(this[i], this.obj), i++) ;


            return this;
        }
        public Iterator<T> Foreach(_foreachi _for)
        {
            int i = 0, len = this.Count;
            for (i = 0; i < len; _for(this[i], i), i++) ;


            return this;
        }
        public int CountOf(_filter _fil)
        {
            int i = 0, j = 0;
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i])) j++;
            }
            return j;
        }
        public int CountOf(_filterIndex _fil)
        {
            int i = 0, j = 0;
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i], i)) j++;
            }
            return j;
        }
        public Iterator<T> concat(params T[][] args)
        {
            int len = this.Count;
            int i = 0;
            int c = args.Length;
            for (i = 0; i < c; i++)
            {
                len += args[i].Length;
            }
            T[] _concat = new T[len];

            this.Foreach((item, j) =>
            {
                _concat[j] = item;
            });
            int index = (this.Count);
            for (i = 0; i < c; i++)
            {
                Iterator<T> iter = new Iterator<T>(args[i]);
                iter.Foreach((item, k) =>
                {
                    _concat[index + k] = item;
                });
                index += (iter.Count);
            }




            return new Iterator<T>(_concat);
        }
        public Iterator<T> filter_params(params _filter[] _filters)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_filters[0]);
            T[] mItems = new T[c];
            for (i = 0; i < this.Count; i++)
            {
                if (_isMatch(this[i], _filters))
                {
                    mItems[j] = this[i];
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }
        public bool _isMatch(T item, params _filter[] _filters)
        {

            Iterator<_filter> nx_filters = new Iterator<_filter>(_filters);
            return nx_filters.filter(p => p(item)).Count == _filters.Length;
        }
        public delegate bool _filterIndex(T item, int i);

        public Iterator<T> filter(_filterIndex _fil)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_fil);
            T[] mItems = new T[c];
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i], i))
                {
                    mItems[j] = this[i];
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }
        public Iterator<T> filter(_filter _fil)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_fil);
            T[] mItems = new T[c];
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    mItems[j] = this[i];
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }
        public delegate T _editor(T item);

        public Iterator<T> edit(_editor _edit)
        {
            int i = 0;

            for (i = 0; i < this.Count; i++)
            {
                T item = _edit(this[i]);
                this[i] = item;
            }
            return this;
        }
        public delegate T filter_edit_index(T item, int i);

        public Iterator<T> filterEdit(_filter _fil, filter_edit_index _edit)
        {
            int i = 0;

            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    this[i] = _edit(this[i], i);
                }
            }
            return this;
        }
        public Iterator<T> filterEdit(_filter _fil, filter_edit_index _if, filter_edit_index _else)
        {
            int i = 0;

            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    this[i] = _if(this[i], i);
                    continue;
                }
                this[i] = _else(this[i], i);
            }
            return this;
        }
        public Iterator<T> filterEdit(_filter _fil, _filter_and_Edit _edit)
        {
            int i = 0;

            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    this[i] = _edit(this[i]);
                }
            }
            return this;
        }
        public Iterator<T> filter(int start , _filter _fil)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_fil);
            T[] mItems = new T[c];
            for (i = start; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    mItems[j] = this[i];
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }
        public Iterator<T> filter(_filter _fil, filter_edit_index _edit)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_fil);
            T[] mItems = new T[c];
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    mItems[j] = _edit(this[i], i);
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }

        public Iterator<T> filter(_filter _fil, _filter_and_Edit _edit)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_fil);
            T[] mItems = new T[c];
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    mItems[j] = _edit(this[i]);
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }
        public delegate void _inter_leave(params T[] items);
        public delegate void __inter(params Iterator<T>[] __iters);
        public Iterator<T> interleave( _inter_leave interleave , params T[] items)
        {

            interleave(items);
            return null;
        }
        public static Iterator<T> Foreach_(__inter iter ,    params Iterator<T>[] items )
        {
            int c = items.Length;
            int i = 0;
            for ( i=0;i<c;i++)
            {
                iter(items[0], items[1]);
            }
            return null;
        }

    }


    public class Randomizer
    {

        public int seed;
        public Random rand;

        public Randomizer ( int s)
        {
            this.seed = s;

        }
        public Randomizer()
        {
            this.seed = (int) DateTime.Now.Ticks;
            this.rand = new Random(this.seed);
        }
        public double  GenerateRandom(int r_min , int r_max)
        {
            return this.rand.Next(r_min, r_max);
        }
        public double GenerateRand( double[] _ar , double radius, int r_max, int r_min , int c)
        {
            int n = c;
            double ele = 0;
            bool ok = true;
            int count = 0;
            while (true)
            {
                ele = GenerateRandom(r_min, r_max);

                for (int i = 0; i < n; i++)
                {
                    ok &= Math.Abs(_ar[i] - ele) >= radius;
                }
                count++;
                if( count>=n)
                {
                    ele = ele + (radius) ;
                    ele %= r_max;
                }
                if (ok || count>=n) break; //prevent finite loop
            }
            return ele;
        }
        public double GenerateRand(double prev , double radius , int r_max , int r_min)
        {
            double ele = prev;
            
            do
            {
                ele = GenerateRandom(r_min, r_max);

            } while (Math.Abs(prev - ele) >= radius);
                return ele;
        }
        public ns_Point[] GenerateDistinctPoints(int n , int r_min, int r_max, double d)
        {
            ns_Point[] p1 = GenerateRandomPoints(n, r_min, r_max, d);
            ns_Point[] p2 = GenerateRandomPoints(n, r_min, r_max, d);
            Iterator<ns_Point> it1 = new Iterator<ns_Point>(p1);
            Iterator<ns_Point> it2 = new Iterator<ns_Point>(p2);
            ns_Point[] res = new ns_Point[n];
            it1.Foreach((p, i) =>
                {
                   int c=  it1.filter(i+1, (pp) =>
                        {
                            return p.calcDistance(pp) >= d;
                        }).Count;
                    if( c>0 )
                    {
                        res[i] = p;
                    }
                });
            

            return res;

        }
        public Iterator<ns_Point> GenerateInterLeavedRandoms(int n , int r_min, int r_max, double radius, int inter_leave_by )
        {
            ns_Point[] points = GenerateRandomPoints(n, r_min, r_max, radius);
            return new Iterator<ns_Point>(points).InterLeaveBym(inter_leave_by);
        }
        public  static double Mean(double[] a)
        {
            double m =0.0;
            int len = a.Length;
            new Iterator<double>(a).Foreach((x) =>
                {
                    m += (x / len);
                });
            return m;
        }
        public ns_Point[] GenerateRandomPoints(int n, int r_min, int r_max , double radius)
        {
            ns_Point[] points = new ns_Point[n];

            double[] x = GenerateRandoms(n, r_max, r_min, radius);
            double[] y = GenerateRandoms(n, r_max, r_min, 0);
           
            Random rr = new Random();
            ns_Point prev = new ns_Point(0, 0);
            ns_Point max_point = new ns_Point(0, 0);
            ns_Point min_point = new ns_Point(0, 0);
            for (int i = 0; i < n; i++)
            {
                int j = rr.Next(r_min, r_max);
                int k = rr.Next(r_min, r_max);
                ns_Point p = new ns_Point(j, k);

                if (i == 0) { points[i] = p; prev = p; min_point = p; max_point = p; continue; }
              
                if (i != 0 && p.calcDistance(prev) >= radius)
                {
                    points[i] = p;
                }
                else
                {

                    points[i] = max_point.Mean(p-min_point);
                    prev = max_point.Mean(p - min_point);
                }

                prev = p;
                min_point = p.Min(min_point);
                max_point = p.Max(max_point);
            }


                return points;
        }
        public double[] GenerateRandoms(int n, int r_max, int r_min , double radius)
        {
            double[] _ar = new double[n];
            
            int i = 0;
            double x = GenerateRandom(r_min ,r_max);
            _ar[0] = x;
            Random r = new Random((int) DateTime.Now.Ticks);
            while( i<n )
            {

                x = r.Next(r_min%(r_max), r_max);
                r_min += (int)radius;

                _ar[i] = x;

                i++;
            }

                return _ar;
        }
    }
    // public delegate string _nanofx(params object[] args);

    public class ns_Point
    {
        public double x;
        public double y;
        public enum PointState
        {
            start = 0,
            end = 1
        };
        public PointState State;
        public ns_Point ( double _x, double _y)
        {
            this.x = _x;
            this.y = _y;
        }
        public ns_Point ()
        {
            this.x = 0;
            this.y = 0;
            this.State = PointState.end;
        }
        public static ns_Point  operator+ ( ns_Point p1, ns_Point p2)
        {
            return new ns_Point(p1.x + p2.x, p1.y + p2.y);
        }
        public static ns_Point operator -(ns_Point p1, ns_Point p2)
        {

            return new ns_Point(Math.Abs(p1.x - p2.x), Math.Abs(p1.y - p2.y));
        }
        public static ns_Point operator /( ns_Point p , double n)
        {
            if (n == 0) return p;
            return new ns_Point(p.x / n, p.y / n);
        }
        public static ns_Point operator*(ns_Point p , double f)
        {
            return new ns_Point(p.x *f, p.y * f);
        }
        public double calcDistance(ns_Point p1)
        {
            double _x = p1.x;
            double _y = p1.y;
            double x_2 = (this.x - _x)*(this.x-_x);
            double y_2 = (this.y - _y) * (this.y - _y);
            return Math.Sqrt(x_2 + y_2);
        }
        
         public ns_Point Min(ns_Point p)
        {
            return new ns_Point(this.x < p.x ? this.x : p.x, this.y < p.y ? this.y : p.y);
        }
         public ns_Point Max(ns_Point p)
         {
             return new ns_Point(this.x > p.x ? this.x : p.x, this.y > p.y ? this.y : p.y);
         }
        public ns_Point Mean(ns_Point p)
         {
             return (this + p) * 0.5;

         }
        
        public  double theta(ns_Point p)
        {
            double x0, y0,  x1, y1;
            x0 = p.x;
            y0 = p.y;
            y1 = this.y;
            x1 = this.x;
            double _theta = (y1 - y0) / (x1 - x0);
            var ang = Math.Atan(_theta);
            return ang;
        }
        public static double todeg(double theta)
        {
            return theta * 180 / Math.PI;
        }
    }
    public class ns_Physics
    {
       
        public static ns_Point MoveToPoint(NodesInfo car ,double x , double y ,Action<string> log,  Action<ns_Point> timer_handler )
        {
            ns_Point res = new ns_Point();
            NodesInfo node = new NodesInfo();
            node.X = x;
            node.Y = y;
            var x0 = car.X;
            var y0 = car.Y;
            var x1 = node.X;
            var y1 = node.Y;
            var t = 0.0;
            var ax = 30.795275591; // acceleration
            var vx = 0.001; // initial velocity .
            var tx = 0.0264583333 * 1000;// / 10; initial time;
            // log("Start Moving Car,,,,");
            var theta = angle(car, node);
            var dx = CalcEucludianDistance(car, node);

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
                        car.State = NodesInfo.NodeState.availabe;
                        res.State = ns_Point.PointState.end;
                        timer.Stop();
                    }
                    timer_handler(res);

                    t = t + 1;
                }
                catch (Exception ex)
                {

                  log("Error in Timer routine:Mo" + ex.Message);
                    timer.Stop();
                }

            };
            timer.Start();
            // log("Timer after Start.");

            return res;
        }
        public static ns_Point MoveToPoint(NodesInfo car, NodesInfo node , Action<string> log , Action<ns_Point> timer_handler)
        {
            ns_Point res = new ns_Point() { x = 0, y = 0 , State = ns_Point.PointState.start};

            var x0 = car.X;
            var y0 = car.Y;
            var x1 = node.X;
            var y1 = node.Y;
            var t = 0.0;
            var ax = 30.795275591; // acceleration
            var vx = 0.001; // initial velocity .
            var tx = 0.0264583333 * 1000;// / 10; initial time;
           // log("Start Moving Car,,,,");
            var theta = angle(car, node);
            var dx = CalcEucludianDistance(car, node);
            
            ax = (x1 - x0) > 0 ? ax : -1 * ax; // positive or neg acceleration.

            var TimeRequired=  tx * calcTime(vx, Math.Abs(ax), dx, Math.PI / 2);

            string _log = string.Format("Time Required:{0} , distance:{1}", TimeRequired, dx);
            log(_log);
            System.Timers.Timer timer = new System.Timers.Timer(38);
           
            timer.Elapsed += (e,arg) =>
                {

                    try
                    {
                        var d = CalcDisplacement2(vx, ax, t / tx); // tx

                        var xf = (x0 + d * cos(theta));//*Math.cos(t);
                        var yf = (y0 + d * sin(theta));

                        res.x = xf;
                        res.y = yf;
                        //string fmt = string.Format("xf:{0} , yf:{1} , time:{2}", xf, yf, t);

                       
                       // log(fmt);
                        if (Math.Ceiling(d) == Math.Ceiling(dx) || t >= TimeRequired)
                        {
                            log("Timer Stop.");
                            car.State = NodesInfo.NodeState.availabe;
                            res.State = ns_Point.PointState.end;
                            timer.Stop();
                        }
                        timer_handler(res);
                        t = t + 1;
                    }
                    catch (Exception ex)
                    {

                        log("Error in Timer routine:" + ex.Message);
                        timer.Stop();
                    }

                };
            timer.Start();
           // log("Timer after Start.");

            return res;
        }
        public static double angle(NodesInfo p1, NodesInfo p2)
        {
            return angle(p1.X, p1.Y, p2.X, p2.Y);
        }
        public static double CalcEucludianDistance(NodesInfo n1 , NodesInfo n2)
        {
            double x = sqr(n1.X - n2.X);
            double y = sqr(n1.Y - n2.Y);
            return Math.Sqrt(x + y);
        }
        public static double sqr(double x)
        {
            return x*x;
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
            var ang = Math.Atan(theta);
            return ang;
        }
        public static double todeg(double theta)
        {
            return theta * 180 / Math.PI;
        }
    }
    public class ns_ip_port
    {
        public int port;
        public string ip_address;

        public static ns_ip_port Parse(string ip_port)
        {
            ns_ip_port ip_portx = new  ns_ip_port() { ip_address = "127.0.0.1 ", port = 20001 };
            if ( string.IsNullOrEmpty(ip_port))
            {
                return ip_portx;
            }
            try{
            string[] sp = ip_port.Split(':');
            if (sp.Length == 0) { return ip_portx; }
            string ip = sp[0];
            int port = int.Parse(sp[1]);
            return new ns_ip_port() { ip_address = ip, port = port };

            }
            catch (Exception )
            {
                return ip_portx;
            }
            
        }
    }
    public class DeviceInfo
    {

        public int DeviceId { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public int DepId { get; set; }
        /// <summary>
        /// 线路ID
        /// </summary>
        public int LineId { get; set; }
        /// <summary>
        /// 线路名称
        /// </summary>
        public string LineName { get; set; }
        /// <summary>
        /// 行别
        /// </summary>
        public byte LineType { get; set; }
        /// <summary>
        /// 设备所在位置
        /// </summary>
        public float Location { get; set; }
        /// <summary>
        /// 标定图片路径
        /// </summary>
        public string CailIImgPath { get; set; }
        /// <summary>
        /// 状态 0：正常 1：停用
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
    public class ns_app
    {
        public static string EncodeJson(object obj)
        {
            return  JsonConvert.SerializeObject(obj);
        }
        public static NodesInfo decodeJsonMsg(string msg, Action<string> error_handler)
        {
            NodesInfo _info = null;

            try
            {
                _info = JsonConvert.DeserializeObject<NodesInfo>(msg);
            }
            catch (Exception ex)
            {
                error_handler("decodeJsonMsg" + ex.Message);

            }
            return _info;
        }
        public static string EncodeObject(NodesInfo info)
        {
            if (info == null) return "object is null";
            string m = "";
             m = JsonConvert.SerializeObject(info);

            return m;
        }
    }
    public class NodesInfo
    {
        public string client { get; set; }
        public string NodeId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Status { get; set; }
        public double nodepwr { get; set; }
        public double Distance { get; set; }
        public string ClientType { get; set; }
        public int UiId { get; set; }
        public enum NodeState
        {
            availabe=0,
            busy,
            moving,
            charged
        };
        public NodeState State;
        public NodesInfo() { this.UiId = -1; }
        public NodesInfo(string id, double x, double y, double pwr)
        {
            this.NodeId = id;
            this.X = x;
            this.Y = y;
            this.nodepwr = pwr;
            this.Distance = 0;
            //this.
            this.Status = "unknown";
            this.UiId = -1;
            this.State = NodeState.availabe;
        }
        public NodesInfo(string client_id, string node_id, bool status, int ui_id)
        {
            this.client = client_id;
            this.X = 0;
            this.Y = 0;
            this.Distance = 0;
            this.NodeId = node_id;
            this.Status = status ? "connected" : "disconnected";
            this.UiId = ui_id;
            this.State = NodeState.availabe;
        }
        public static NodesInfo generateRandomNode(string client, int id, string type, int max_x, int max_y)
        {
            NodesInfo info = new NodesInfo();
            info.client = client;
            info.NodeId = id.ToString();
            info.UiId = id;
            info.ClientType = type;
            long seed = DateTime.Now.Ticks / 10 * (id + 1);
            Random n = new Random((int)seed);
            info.X = n.Next(max_x);
            info.Y = n.Next(max_y);
            info.nodepwr = n.Next(100);
            info.Status = "connected";
            info.State = NodeState.availabe;

            return info;
        } 
        public static NodesInfo generateRandomNode(int id , string type , int max_x , int max_y )
        {
            NodesInfo info = new NodesInfo() ;
            info.client = id.ToString();
            info.NodeId = id.ToString();
            info.UiId = id;
            info.ClientType = type;
            long seed = DateTime.Now.Ticks/10*(id+1);
            Random n = new Random((int) seed);
            info.X = n.Next(max_x);
            info.Y = n.Next(max_y);
            info.nodepwr = n.Next(100);// 100% percent means battery is full .
            info.Status = "connected";
            info.State = NodeState.availabe;

            return info;
        }

        private string calc_distance(double x, double y)
        {
            return "0";
        }

    }
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<NodesInfo> xNodesInfo;
        private ObservableCollection<DeviceInfo> xNodeDetails;

        public ObservableCollection<NodesInfo> NodesInfox
        {
            get { return xNodesInfo; }
            set
            {
                xNodesInfo = value;
                RaisePropertyChanged("NodesAdded");
            }
        }
        public ObservableCollection<DeviceInfo> NodeDetails
        {
            get { return xNodeDetails; }
            set
            {
                xNodeDetails = value;
                RaisePropertyChanged("DetailsAdded");
            }
        }
    }
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
