using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.graph
{
    public class ns_window_functions
    {
        public static double fact(int n)
        {
            double res =1;
            for (int i = 1; i <= n; i++)
                res *= i;
            return res;
        }
        private static double downx(int i)
        {
            return Math.Pow(2, i) * fact(i) * (2 * i + 1);
        }
        private static double onePowMinusI(int i)
        {
            return i % 2 == 0 ? 1 : -1;
        }

        public static double erf(double x)
        {
            return erf(10, x);
        }
        public static double erf(int samples, double x)
        {
            double sum = 0.0;

            int i = 0;
            double fx = 0;
            for (i = 0; i < samples; i++)
            {
                fx = Math.Pow(x, (2 * i) + 1) * onePowMinusI(i) / (downx(i));
                sum += fx;
            }


            return sum / samples;
        }
        public static double Mean(double[] ar)
        {
            double sum = 0.0;
            foreach(var x in ar)
            {
                sum += x;
            }
            return sum / (ar.Length);
        }
        public static double getGuassianSample(int i, double alpha, double amp, int N)
        {
            double N_2 = N / 2.0;
            double exp = ((i - N_2) / (alpha * N_2));
            double res = amp * Math.Exp(-0.5 * exp * exp);
            return res;
        }
        public static double GetMyThreshold(int i , double alpha , double beta_0, double beta_1 , int N)
        {
          //  double N_2 = N / 2.0;
            double exp = ((i) / (alpha * N));
            double res = beta_0 + (beta_1 * Math.Exp(-0.5 * exp * exp));
            return res;
        }
        public static double ln(double x)
        {
            return Math.Log(x);// ln
        }
        public static double Exp(double x)
        {
            return Math.Exp(x);
        }
        public static double[] RampBydelta(double start, double end , double dx)
        {
            double N = Math.Ceiling(1 + ((start - end) / dx));
            //N = (int)N;
            double[] win = new double[(int)N];
            int i = 0;
            for (i = 0; i < N; i++)
            {
                win[i] = (start + i * dx);

            }
            return win;
        }
        public static double[] logRampWindow(int N, double start , double end)
        {
            //x = [ln(end) – ln(start)]/m
            double dx = (ln(start) - ln(end)) / (N);
            double[] win = new double[N];
            int i = 0;
            for (i = 0; i < N; i++)
            {
                win[i] = (start + i * dx);

            }
            return win;
        }
        public static double[] rampWindow(int N  ,double start, double end)
        {
            double dx = (end - start) / (N);
            int i = 0;
            double[] win = new double[N];
            
            for (i = 0; i < N; i++)
            {
                win[i] = (start + i * dx);
                
            }
            return win;
        }
        public static double[] Guassian_window(int nSamples, double alpha , double amp)
        {
            int N = nSamples;
            int i = 0;
            double N_2 = N / 2.0;
            double[] w = new double[N];
            for (i = 0; i < N; i++)
            {
                double exp = ((i - N_2) / (alpha * N_2));
                double res = amp * Math.Exp(-0.5 * exp * exp);

                w[i] = res;
            }
            return w;
        }
        public static double ham_thres(double a0,double a1,int nSamples,int i)
        {
            double res = a0 + (a1 * Math.Cos(2 * Math.PI * i / nSamples));
            return Math.Abs(res);
        }
        public static double[] hanning_window(int nSamples , double a0, double a1, double amp)
        {
            double[] w = new double[nSamples];
            int j = nSamples / 2;
            int i = 0;
            for (i = -j; i < j; i++)
            {
                double res = a0 + (a1 * Math.Cos(2 * Math.PI * i / nSamples));
                w[j+i] = res;
            }
            return w;
        }
    }
}
