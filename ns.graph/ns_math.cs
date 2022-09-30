using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.graph
{
    public class ns_math
    {
        public static double cosr(double theta)
        {
            return Math.Cos(theta);
        }
        public static double cosd(double theta)
        {
            return cosr(toRadian(theta));
        }
        public static double toRadian(double angle_deg)
        {
            return (angle_deg % 360) * Math.PI / 180.0;
        }
        public static double todeg(double angle_rad)
        {
            return 180 * angle_rad / Math.PI;
        }
        public static ulong fact(uint n)
        {
            if (n == 0 || n == 1 )
                return 1;
            return (ulong) (n * fact(n - 1));
        }
        public static double perm(uint n, uint k)
        {
            return fact(n) / (fact(n - k));
        }
        public static double Comb(uint n, uint k)
        {
            return perm(n, k) / fact(k);
        }
        private static uint __fib(uint n , Dictionary<uint , uint> mdic)
        {
            if (n >= 0 && n <= 2) return 1;
            if (mdic.ContainsKey(n - 1))
            {
                uint x = __fib(n - 2 , mdic);
                mdic[n - 2] = x;
                return mdic[n - 1] + x;
            }
            if (mdic.ContainsKey(n - 2))
            {
                uint y = __fib(n - 1 , mdic);
                mdic[n - 1] = y;
                return y + mdic[n - 2];
            }
            if (mdic.ContainsKey(n - 1) && mdic.ContainsKey(n - 2))
            {
                return mdic[n - 1] + mdic[n - 2];
            }
            mdic[n] = __fib(n - 1 , mdic) + __fib(n - 2 , mdic);
            return mdic[n];
        }
        /// <summary>
        /// fibnocci squence.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static uint fib(uint n)
        {
            Dictionary<uint, uint> mdic = new Dictionary<uint, uint>();
            return __fib(n, mdic);
        }

    }
    public class ns_derivatives
    {
        public  enum deriveType
        {
            second_order_central = 0,
            fourth_order_central = 1,
            forward,
            backward


        };
        public static double[] derive(double[] xt, double dt , deriveType _type)
        {
            double[] yt = new double[xt.Length];
            int i = 0;
            int len = xt.Length;
            if (dt == 0) dt = 0.111;
            for (i = 0; i < len; i++)
            {
                double x_i_plus_1 = i + 1 >= len ? 0 : xt[i + 1];//x(i+1)
                double x_i_minus_1 = i - 1 < 0 ? 0 : xt[i - 1];//x(i-1)
                double xi = xt[i];//x(i)
                double x_i_plus_2 = i + 2 > len ? 0 : xt[i + 2];//x(i+2)
                double x_i_minus_2 = i - 2 < 0 ? 0 : xt[i - 2];//x(i-2)
                if(_type == deriveType.second_order_central)
                {
                    
                    yt[i] = (x_i_plus_1 - x_i_minus_1) * (1 / (2 * dt));

                    continue;
                }
                if(_type == deriveType.fourth_order_central)
                {
                    //
                    yt[i] = (1 / (12 * dt)) * (-x_i_plus_2 + (8 * x_i_plus_1) - (8 * x_i_minus_1) + (x_i_minus_2));
                    continue;
                }
                if(_type == deriveType.forward)
                {
                    yt[i] = (x_i_plus_1 - xi) * (1 / dt);
                    continue;
                }
                if(_type==deriveType.backward)
                {
                    yt[i] = ( xi- x_i_minus_1) * (1 / dt);
                }

            }
            return yt;
        }
    }
    public class ns_integral
    {
        public enum integral_Type
        {
            trapzoidal = 0,
            simpson_1 = 2,
            simpson_2 = 3,
            bode
        };
       
    }
    
}
