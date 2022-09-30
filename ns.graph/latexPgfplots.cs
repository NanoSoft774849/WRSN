using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ns.graph
{
    public class latexpgfp_DataTable
    {
        public latexpgfp_DataTable(string data_file_name)
        {

        }
        public latexpgfp_DataTable()
        {

        }
        public static void plotPointsSimple(List<ns_point> points, string data_file_name)
        {
            //string fmt = "{0} \t {1}\n";
            string data = "x_0\t f(x)\n";
            foreach(ns_point p in points)
            {
                //data +=string.Format()
                data += p.toLatexDataFilePoint();
            }
            File.WriteAllText(data_file_name, data);
        }
        public static void PlotNodeCoordinates(Dictionary<string, ns_node> nodes, string fn_name)
        {
            string data = "x\ty\tlabel\n";
            foreach(var v in nodes)
            {
                string type = v.Value.NodeType==Node_TypeEnum.Sensor_node?"S":"M";
                type = v.Value.NodeType == Node_TypeEnum.BaseStation ? "B" : type;
                data += string.Format("{0}\t{1}\t{2}\n", v.Value.location.x, v.Value.location.y, type);
            }
            File.WriteAllText(fn_name, data);
        }
        public static void Example1()
        {
            string data_file = "sine.dat";
            int n = 100, i=0;
            string data = "x\t y\n";// tab seperated value.!
            for ( i = 0; i < n; i++)
            {
                double x = i;
                double y = 50 * Math.Sin(2 * Math.PI * i * 10.1 / 600);
                data += new ns_point(x, y).toLatexDataFilePoint();
            }
            File.WriteAllText(data_file, data);

        }
    }
    public class latexPgfplots
    {
    }
}
