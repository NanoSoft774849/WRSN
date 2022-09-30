using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ns.graph
{

    public class ns_canvas : Canvas
    {
        public ns_canvas()
        {
            
        }
        public void addChild(UIElement ele)
        {
            this.Children.Add(ele);
        }
        public UIElementCollection Elems
        {
            get
            {
                return this.Children;
            }
        }
    }
    public class net_cluster
    {
        public ns_point location;
        public double cluster_radius;
        public string cluster_id;
        public ns_list<ns_node> nodes;
        public net_cluster(string cluster_id)
        {
            this.cluster_id = cluster_id;
            this.location = new ns_point();
            this.nodes = new ns_list<ns_node>();
        }
        public net_cluster (string cluster_id , ns_point loc , double r)
        {
            this.cluster_id = cluster_id;
            this.location = loc;
            this.cluster_radius = r;
            this.nodes = new ns_list<ns_node>();
        }
        public net_cluster normalize()
        {
            this.nodes.Foreach((n) =>
                {
                    n.location.mod(2 * this.cluster_radius, 2 * this.cluster_radius);
                });
            return this;
        }

    }
    public class net_size
    {
        public double x;
        public double y;
        public double width;
        public double height;
       
        public net_size(double x , double y , double w, double h)
        {
            this.x = x;
            this.y = y;
            this.width = w;
            this.height = h;
          //  this.nodes = new ns_list<ns_node>();
        }
        public net_size ()
        {
            this.x = 0;
            this.y = 0;
            this.width =0;
            this.height = 0;
            //this.nodes = new ns_list<ns_node>();
        }
       
    }
    class ns_network_graph : Canvas
    {
        public ns_point location;
        public double width;
        public double height;

        public string net_id;
        public ns_list<ns_node> nodes;
        public ns_list<net_cluster> clusters;
        public ns_network_graph()
        {
            this.location = new ns_point(0, 0);
            this.width = 500;
            this.height = 500;
            this.net_id = "net0";
            this.nodes = new ns_list<ns_node>();
            this.clusters = new ns_list<net_cluster>();
        }
        public ns_network_graph(string net_id , ns_point location)
        {
            this.net_id = net_id;
            this.location = location;
            this.nodes = new ns_list<ns_node>();
            this.clusters = new ns_list<net_cluster>();
        }

        public ns_network_graph (string net_id , net_size dim)
        {
            this.net_id = net_id;
            this.location = new ns_point(dim.x, dim.y);
            this.width = dim.width;
            this.height = dim.height;
            this.nodes = new ns_list<ns_node>();
            this.clusters = new ns_list<net_cluster>();
        }
        public ns_network_graph add_node(ns_node node)
        {
            this.nodes.add(node);
            return this;
        }
        public ns_network_graph add_nodes(params ns_node[] fx_nodes)
        {
            foreach(ns_node n in fx_nodes)
            {
                this.add_node(n);
            }
            return this;
        }
        public ns_network_graph add_cluster(net_cluster cluster)
        {
            this.clusters.add(cluster);
            return this;
        }
        public ns_network_graph add_clusters(params net_cluster[] _clusters)
        {
            foreach(net_cluster c in _clusters)
            {
                this.add_cluster(c);
            }
            return this;
        }
        public ns_network_graph RenderNodes(ns_list<ns_node> nodes, UIElementCollection ele)
        {


            return this;
        }
    }
}
