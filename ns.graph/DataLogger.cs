using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ns.graph
{
    public class DataLoader
    {
        public static List<double[]> LoadFiles(string[] files)
        {
            List<double[]> data = new List<double[]>();
            foreach(var file in files)
            {
                double[] d = loadFile(file);
                data.Add(d);
            }
            return data;
        }
        public static void ConcatFilesInto(string[] files , string outfile)
        {
            List<double[]> data = LoadFiles(files);
            //foreach()
            int i = 0, j = 0;
            int rows = data.Count , cols =0;
            string str = "";
            File.WriteAllText(outfile, "");
            for (i = 0; i < rows; i++)
            {
                cols = data[i].Length;
                str = "";
                for (j = 0; j < cols; j++)
                {
                    string sep = j == cols - 1 ? "\n" : "\t";
                    str += string.Format("{0}{1}", data[i][j], sep);
                }
                File.AppendAllText(outfile, str);
            }

        }
        public static double[] loadFile(string file)
        {
            var lines = File.ReadLines(file);
            int count = lines.Count();
            double[] data = new double[count];
            int i = 0;
            double d= 0.0;
            foreach(var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                d = double.Parse(line);
                data[i] = d;
                i++;
            }
            return data;
        }
        public static double GetMean(string fn , bool skipx)
        {
            double mean = 0.0;
            try
            {
                var lines = File.ReadLines(fn);
                double sum = 0.0;
                int count = lines.Count();
                count = count == 0 ? 1 : count;
                int i = 0 , j=0;
                double d = 0;
                foreach(string line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    if(i == 0)
                    {
                        i++;
                        continue ;
                    }
                    string[] cols = line.Split('\t');
                    d=0;
                    i++;
                    if(cols.Length == 1)
                    {
                        d = double.Parse(cols[0]);
                        sum += d;
                        continue;
                    }
                    j = 0;
                    foreach(string cell in cols)
                    {
                        if (string.IsNullOrEmpty(cell)) continue;
                        if( skipx && j==0)
                        {
                            j++;
                            continue;
                        }
                        d = double.Parse(cell);
                        sum += d;
                    }
                  

                }
                mean = (sum) / (count);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return mean;
        }
        public static Dictionary<int, List<double>> LoadDataFile(string fn, bool skipx)
        {
            Dictionary<int, List<double>> data = new Dictionary<int, List<double>>();
            try
            {
                // FileStream stream = File.OpenRead(fn);
                int line = 0;
              
                foreach (string str in File.ReadLines(fn))
                {
                    if (string.IsNullOrEmpty(str)) continue;
                    if (line == 0)
                    {
                        line++;
                        continue;
                    }
                    //printf("{0}:{1}\n", line, str);
                    string[] linedata = str.Split('\t');
                    List<double> list = new List<double>();
                    int i = 0;
                    foreach (string s in linedata)
                    {
                        double d = double.Parse(s);
                        if (skipx && i == 0)
                        {
                            i++;
                            continue;
                        }
                        list.Add(d);

                    }
                    if (list.Count > 0)
                    {
                        data[line] = list;
                    }

                    line++;
                }
               

            }
            catch (Exception ex)
            {
                // MessageBox.Show("Ex in Open file")
               // printf("Exception in open {0} msg:{1}\n", fn, ex.Message);
                throw ex;
            }
            return data;
        }
    }
    public class DataLogger
    {
        private string fn;
        private double t_counter;
        private bool auto_time;
        public DataLogger(string _fn)
        {
            this.fn = _fn;
            string fmt = string.Format("x\ty\n");
            this.auto_time = false;
            this.t_counter = 0;
            if(File.Exists(_fn))
            {
                File.WriteAllText(_fn, "");// deleete content;
               
               // Write(fmt);
                
            }
            else
            {
               // Write(fmt);
            }

        }
        public DataLogger(string _fn, bool auto_timer)
        {
            this.t_counter = 0;
            this.fn = _fn;
            this.auto_time = auto_timer;
            string fmt = string.Format("x\ty\n");
            if (File.Exists(_fn))
            {
                File.WriteAllText(_fn, "");// deleete content;

                Write(fmt);

            }
            else
            {
                Write(fmt);
            }

        }
        /// <summary>
        /// Append data to file content
        /// </summary>
        /// <param name="data"></param>
        public void Write(string data)
        {
            File.AppendAllText(this.fn, data);
        }
        public void Writeline(double value)
        {
            string data = string.Format("{0}\n", value);
            File.AppendAllText(this.fn, data);
        }
        public void AppendFormat(string fmt, params object[] args)
        {
            string value = string.Format(fmt,args);
            try
            {
                File.AppendAllText(this.fn, value);

            }catch (Exception)
            {
                return;
            }
        }
        public void WriteLineMultiCsv(params double[] args)
        {
            string row ="";
            int i = 0;
            int len = args.Length;
            string sep = "\t";
            for (i = 0; i < len; i++)
            {
                sep = i == len - 1 ? "\n" : ",";
                row += string.Format("{0}{1}", args[i], sep);

            }
            File.AppendAllText(this.fn, row);
        }
        public void AppendData(double t, double value)
        {
            string fmt = string.Format("{0}\t{1}\n", t, value);
            Write(fmt);
        }

        public void AppendData(double value)
        {
            if(this.auto_time)
            {
                AppendData(this.t_counter, value);
                this.t_counter++;
            }
        }
    }
}
