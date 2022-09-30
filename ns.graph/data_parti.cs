using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.graph
{
    public class IndexValuePair<T>
    {
        public int index;
        public T value;
        public IndexValuePair(T value, int index)
        {
            this.index = index;
            this.value = value;
        }
    }
    public class minMax
    {
        double min;
        double max;
        int min_index;
        int max_index;
        public minMax(double max, double min, int max_i, int min_i)
        {
            this.max = max;
            this.min = min;
            this.min_index = min_i;
            this.max_index = max_i;
        }
    }
    public class boolKey
    {
        public bool status;
        public string groupkey;
        public string notkey;
        public boolKey(bool stat, string key)
        {
            this.groupkey = key;
            this.status = stat;
            this.notkey = string.Format("not_{0}", key);
        }
    }
    class data_part
    {
        /// <summary>
        /// this is crazy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="va"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public delegate boolKey _fx<T>(T va, Dictionary<string, _fx<T>> d);

        public static bool IsInRange(double val, double min, double max)
        {
            return val <= max && val >= min;
        }

        public static Dictionary<string, List<T>> Partition<T>(T[] ar, params _fx<T>[] pps)
        {

            int i = 0;
            int len = ar.Length;
            Dictionary<string, List<T>> mdict = new Dictionary<string, List<T>>();
            Dictionary<string, _fx<T>> md = new Dictionary<string, _fx<T>>();
            for (i = 0; i < len; i++)
            {
                foreach (_fx<T> fx in pps)
                {
                    boolKey bk = fx(ar[i], md);
                    md[bk.groupkey] = fx;
                    if (bk.status)
                    {
                        if (mdict.ContainsKey(bk.groupkey))
                        {
                            mdict[bk.groupkey].Add(ar[i]);
                            continue;
                        }
                        mdict[bk.groupkey] = new List<T>();
                        mdict[bk.groupkey].Add(ar[i]);
                    }
                    else
                    {
                        if (mdict.ContainsKey(bk.notkey))
                        {
                            mdict[bk.notkey].Add(ar[i]);
                            continue;
                        }
                        mdict[bk.notkey] = new List<T>();
                        mdict[bk.notkey].Add(ar[i]);
                    }
                }
            }
            return mdict;
        }
        public static minMax ns_MaxMin(double[] ar)
        {
            int i = 0;
            int len = ar.Length;
            double max = ar[0];
            int max_index = 0;
            double min = ar[0];
            int min_index = 0;
            for (i = 0; i < len; i++)
            {
                max = max >= ar[i] ? max : ar[i];
                min = min <= ar[i] ? min : ar[i];
                max_index = max == ar[i] ? i : max_index;
                min_index = min == ar[i] ? i : min_index;
            }
            return new minMax(max, min, max_index, min_index);
            // return new IndexValuePair<double>(max, max_index);
        }

    }
}
