using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;


namespace mTs
{
    public class RandomGenerator
    {
        public int samples;

        public RandomGenerator(int _samples)
        {
            this.samples = _samples;
        }

        public static double mean(double[] arr)
        {
            double sum = 0;
            int N = arr.Length;
            foreach(double v in arr)
            {
                sum += v;
            }
            return sum / (double)(N);
        }
        public static double std_div(double[] arr)
        {
            double me = mean(arr);
            double std = 0;
            double N = arr.Length;
            foreach(var v in arr)
            {
                std += (v - me) * (v - me);
            }
            return Math.Sqrt(std / (N));
        }
        public static double[] ns_fibno2(double[] a, params int[] gx)
        {
            //a(n) = f(n-2)+f(n-1);
            int N = a.Length;
            int M = gx.Length;
            int n = 0;
            int m = 0;
            double fn =0.0;
            double me = mean(a);
            double std = std_div(a);
            for (n = 0; n < N; n++)
            {
                for (m = 0; m < M; m++)
                {
                    int i = n - gx[m];
                    if (i >= 0 && i < N) (fn) += Math.Abs(a[i] + 2*std);
                    else fn = a[n];
                }
                a[n] = fn == 0 ? a[n] : fn;
               // me = mean(a);
                std = std_div(a);
            }


                return a;
        }
        public static double[] ns_fibno(double[] arr)
        {
            // f(n) = f(n-1)+f(n-2);
            int N = arr.Length;
            int n = 0;
            double fn = 0;
            double fn_1 = 0;
            double fn_2 = 0;

            for (n = 0; n < N; n++)
            {
                if (n - 2 >= 0) fn_2 = arr[n - 2];
                if (n - 1 >= 0) fn_2 = arr[n - 1];
                fn = fn_1 + fn_2;
                arr[n] = fn;

            }

            return arr;
        }
        public static int generate_seed(int init)
        {
            long[] aseed = new long[32];
            long seed = 0;
            int n = 0;
            long fn =0;
           
            long fn_4 =0;
            long fn_7 = 0;
            fn = DateTime.Now.Ticks;
            aseed[0] = fn;
            // 1+x^4+x^7
            Random rn = new Random(init);
            for (n = 0; n < 32;n++)
            {
                //79, 83, 89, 97.
                fn = rn.Next(13,79);
                fn_4 = rn.Next(97,883);
                fn_7 = rn.Next(883, 1223);
                if (n - 7 >= 0) seed ^= aseed[n - 7];
                if (n - 4 >= 0) seed ^= aseed[n - 4];
                fn = (fn << 7) + (fn_4 << 13) + (fn_7 << 23);
                aseed[n] |= fn << 16;

                seed |= aseed[n];
            }

                return (int)seed;
        }
        public static double[] generate(int samples , int max)
        {
            double[] arr = new double[samples];
            int seed = (int)DateTime.Now.Ticks;
            Random rnd = new Random(seed);
            
            for (int i = 0; i < samples;i++)
            {
                arr[i] = generate_seed(seed);
                seed |= (int)DateTime.Now.Ticks;

            }

            arr = ns_fibno2(arr, 0);



                return arr;
        }

    }
    public class randomNet
    {
        public int NumberofNodes;
        public Dictionary<string, ns_node> Nodes;

        public int net_max_height;
        public int net_max_width;

        public randomNet(int num, int max_with, int max_height)
        {
            this.NumberofNodes = num;
            this.net_max_height = max_height;
            this.net_max_width = max_with;
            this.Nodes = new Dictionary<string, ns_node>();
        }
        private void _generateTheNet()
        {

            //Random rx = new Random(DateTime.Now.Millisecond);

            //Random ry = new Random(DateTime.Now.Millisecond);
            double[] rx = RandomGenerator.generate(this.NumberofNodes, this.net_max_width);
            double[] ry = RandomGenerator.generate(this.NumberofNodes, this.net_max_height);
            string node_name = "";
            double dx = 0;
            double dy = 0;
            //int j = 1;
            ns_point location = new ns_point();
            for (int i = 0; i < this.NumberofNodes; i++)
            {
                dx = (rx[i] % this.net_max_height);
                dy = (ry[i] % (this.net_max_height));
                if( dx <= 0)
                {
                    dx = RandomGenerator.generate_seed(DateTime.Now.Millisecond);
                    dx %= (this.net_max_width);
                }
                if (dy <= 0)
                {
                    dy = RandomGenerator.generate_seed(DateTime.Now.Millisecond);
                    dy %= (this.net_max_height);
                }

                location = new ns_point(dx, dy);
                node_name = string.Format("S_{0}", i + 1);
                sensor_node node = new sensor_node(node_name, location);
                this.Nodes.Add(node_name, node);

            }
        }
        private ns_node choice()
        {
            Random rx = new Random(DateTime.Now.Millisecond);
            int i = rx.Next(this.NumberofNodes) - 1;
            string node_name = string.Format("S_{0}", i + 1);
            if(i!=-1)
            {
                
                return this.Nodes[node_name];
            }
            return this.Nodes["S_1"];

        }
        private void construct_rand_links(int links)
        {
            

            Random ry = new Random(DateTime.Now.Millisecond);
            //string node_name = "";
          
            for (int i = 0; i < links; i++)
            {
                ns_node src = choice();
                ns_node dst = choice();
                if (src.tag == dst.tag) continue;
                double cost = ry.Next(30);
                this.Nodes[src.tag].addLink(dst,cost);

            }


        }
        public void GenerateRandomNet()
        {
            this._generateTheNet();
            //this.construct_rand_links(this.NumberofNodes + 1);
        }
    }
    public class ns_network
    {
        public Dictionary<string, ns_node> Nodes;

        public ns_network()
        {
            this.Nodes = new Dictionary<string, ns_node>();
        }
        public ns_network Add_Sensor(ns_point location)
        {

            return this;
        }
        public ns_network Add_MobileCharger(ns_point location)
        {
            return this;
        }
    }
}
