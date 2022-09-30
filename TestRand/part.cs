using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRand
{
    public class pigeonhole
    {
        public delegate void _printf(string fmt , params object[] args);
        //private void example1()
        //{

        //    sensor_node s0 = new sensor_node("s0", 10, 200);

        //    sensor_node s1 = new sensor_node("s1", 100, 100);

        //    sensor_node s2 = new sensor_node("s2", 200, 100);

        //    sensor_node s3 = new sensor_node("s3", 300, 100);

        //    sensor_node s4 = new sensor_node("s4", 400, 200);
        //    sensor_node s8 = new sensor_node("s8", 200, 200);

        //    sensor_node s7 = new sensor_node("s7", 100, 300);
        //    sensor_node s6 = new sensor_node("s6", 200, 300);
        //    sensor_node s5 = new sensor_node("s5", 300, 300);


        //    s0.addLink(s1, 4);
        //    s0.addLink(s7, 8);
        //    s1.addLink(s2, 8);
        //    s2.addLink(s3, 7);
        //    s3.addLink(s4, 4);
        //    s4.addLink(s5, 10);
        //    s3.addLink(s5, 14);
        //    s5.addLink(s2, 4);
        //    s5.addLink(s6, 2);
        //    s6.addLink(s8, 6);
        //    s6.addLink(s7, 1);
        //    s7.addLink(s8, 7);
        //    s7.addLink(s1, 11);
        //    s8.addLink(s2, 2);


        //    s0.RenderTo(mcanvas.Children);


        //   dijkstra(s0, s5);
        //   greedy_best_first_search(s0, s5);

        //}
        //private void q3()
        //{
        //    int i = 5401;
        //    int len = 9999;
        //    int count = 0;
        //    for (i = 5401; i < len; i++) 
        //    {
        //        string ss = i.ToString();
        //        if (ss.Contains("2") || ss.Contains("7")) continue;
        //        count++;
        //    }
        //    printf("Count:{0}", count);

        //}
        //private void q1()
        //{
        //    Random rr = new Random(DateTime.Now.Millisecond);
        //    double y = 0;
        //    double sum1 = 0, sum2 = 0.0;
        //    double N = 200;
        //    Dictionary<double, double> mdic = new Dictionary<double, double>();
        //    for(int i=0;i<N;i++)
        //    {
        //        y = rr.Next(100) / N * 1F;
        //        if (mdic.ContainsKey(y))
        //            mdic[y] += (1 / N);
        //        else
        //            mdic[y] = (1 / N);

        //    }
        //    sum1 = 0;
        //    sum2 = 0;
        //    foreach(var kvp in mdic)
        //    {
        //        y= (kvp.Key) * (kvp.Value);
        //        sum1 += y;
        //        sum2 += (y * y);
        //    }
        //    double E_y = sum1;
        //    double E_y2 = Math.Sqrt(sum2);
        //    printf("Ey:{0} \t E_y2:{1}", E_y, E_y2);
        //}
        /// <summary>
        /// Theorem 3.1.1 :
        /// if n+1 objects is distrubuted over n boxes then at least there is one box that contains
        /// two or more objects.
        /// #proof :
        /// by controduction 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="prinf"></param>
        /// 
        //private void topo1()
        //{
        //    base_station bs = new base_station("bss", 300, 300);
        //    double s = 150;
        //    sensor_node s0 = new sensor_node("s0", 50, 170 + s);

        //    s0.ThresholdEnergy = 2;
        //    s0.Energy = 10;
        //    s0.EnergydissipationRate = 0.1;
        //    s0.bs = bs;
        //    s0.OnPowerReachThreshold += s0_OnPowerReachThreshold;

        //    sensor_node s1 = new sensor_node("s1", 80, 120 + s);
        //    sensor_node s2 = new sensor_node("s2", 90, 40 + s);
        //    sensor_node s3 = new sensor_node("s3", 130, 30 + s);
        //    sensor_node s4 = new sensor_node("s4", 150, 80 + s);

        //    sensor_node s5 = new sensor_node("s5", 100, 180 + s);
        //    sensor_node s6 = new sensor_node("s6", 170, 150 + s);
        //    sensor_node s7 = new sensor_node("s7", 130, 110 + s);
        //    sensor_node s8 = new sensor_node("s8", 220, 50 + s);

        //    s0.printf = printfx;
        //    s1.printf = printfx;
        //    s2.printf = printfx;
        //    s3.printf = printfx;
        //    s4.printf = printfx;
        //    s5.printf = printfx;
        //    s7.printf = printfx;
        //    s6.printf = printfx;
        //    s8.printf = printfx;
        //    bs.printf = printfx;
        //    //s1.printf = s0.printf;


        //    s0.addLink(s1, 0);
        //    s1.addLink(s2, 1);
        //    s1.addLink(s7, 2);
        //    s2.addLink(s3, 3);
        //    s3.addLink(s4, 4);
        //    s3.addLink(s8, 5);
        //    s4.addLink(bs, 6);
        //    s4.addLink(s8, 7);
        //    s0.addLink(s5, 8);
        //    s5.addLink(s6, 9);
        //    s6.addLink(s7, 10);
        //    s6.addLink(bs, 11);
        //    s4.addLink(s7, 12);
        //    s8.addLink(bs, 13);

        //    //mobile_charger mc1 = new mobile_charger("mc1", 285, 50);
        //    //mobile_charger mc2 = new mobile_charger("mc2", 120, 150);

        //    s0.RenderTo(mcanvas.Children);
        //    //wsn_network_cluster c1 = new wsn_network_cluster("c1", 50, 300, 200, 200);
        //    //c1.AddSensors(s0, s1, s3, s4);
        //    //c1.AddMobileCharges(mc1);

        //    //c1.Background = Brushes.Black;
        //    //mcanvas.Children.Add(s0.Render());


        //    s1.SetDel();
        //    s2.SetDel();
        //    s3.SetDel();
        //    s4.SetDel();
        //    s5.SetDel();
        //    s6.SetDel();
        //    s7.SetDel();
        //    s8.SetDel();
        //    s0.Start();
        //    // Draw(s0.getPathToBs());
        //    //example1();
        //}
        public static void theorem3_1_1(int n  , _printf printf)
        {
            int[] boxes = new int[n]; // we have n boxes
            int i = 0;
            /// init all boxes to zero item.
            for(i=0;i<n;i++)
            {
                boxes[i] = 0;
            }
            for (i = 0; i < n + 1; i++)
            {
                boxes[i % n] += 1;
            }
            foreach(int j in boxes)
            {
                printf("{0}", j);
            }
        }
        public static string toBin(int n)
        {
            return Convert.ToString(n, 2);
        }
        public static int bitClear(int num  , int i , _printf printf)
        {
            string bf = toBin(num);
            int x = num &~ (1 << i);
            string af = toBin(x);
            printf("before:{0} \t after:{1}", bf, af);
            return x;
        }
        public static int bitSet(int num , int i , _printf printf)
        {
            string bf = toBin(num);
            int x = num | (1 << i);
            string af = toBin(x);
            printf("before:{0} \t after:{1}", bf, af);
            return x;
        }
    }
}
