using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace NPDC
{
    public class ns_sector
    {
        public double start_angle;
        public double end_angle;
        public int sector_id;
        private List<ns_node> nodes;
        public double average_arrival_rate;
        public ns_sector(int sect_id, double start, double end)
        {
            this.start_angle = Math.Min(start,end);
            this.end_angle = Math.Max(start, end);
            this.sector_id = sect_id;
            this.nodes = new List<ns_node>();
            this.average_arrival_rate = 10;
        }
        /// <summary>
        /// add node to sector if not exist in list
        /// if node.Sector_id != this.sector_id 
        /// don't add this node.
        /// 
        /// </summary>
        /// <param name="node"></param>
        public void add_node(ns_node node)
        {
            bool exist = contains(node);
            if (node.SectorId != this.sector_id) return;
            if (exist) return;
            this.nodes.Add(node);
        }
        public void RemoveNode(ns_node node)
        {
            if( contains(node))
            {
                this.nodes.Remove(node);
            }
            return;
        }
        public List<ns_node> getNodes()
        {
            return this.nodes;
        }
        /// <summary>
        /// get the number of nodes in the sector.
        /// </summary>
        public int Count
        {
            get
            {
                return this.nodes.Count;
            }
        }
        /// <summary>
        /// check if the sector conatins node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool contains(ns_node node)
        {
            if( this.nodes.Count == 0) return false;

            foreach( var nodex in this.nodes)
            {
                if(nodex.tag == node.tag) return true;
            }
            return false;
        }
        public bool IsInSector(double angle)
        {
           if( angle <=0 && this.start_angle<=0 && this.end_angle<=0)
           {
               angle = abs(angle);

               return angle >= abs(this.start_angle) && angle <= abs(this.end_angle);
           }
            if( angle >=0 && this.start_angle >=0 && this.end_angle>=0)
            {
                return angle >= abs(this.start_angle) && angle <= abs(this.end_angle);
            }
            return angle >= this.start_angle && angle <= this.end_angle;
        }
        private double abs(double x)
        {
            return Math.Abs(x);
        }
        /// <summary>
        /// set the threshold value of the nodes in the sector.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta_0"></param>
        /// <param name="beta_1"></param>
        public void SetThreshold(double alpha, double beta_0, double beta_1, ns_node bs)
        {
            int i = 0;
            int N = this.Count;
            /// first order them 
            var sorted_nodes = this.nodes.OrderByDescending((n) =>
                {
                    return n.edistance(bs);
                }).Reverse();
            foreach( var node in sorted_nodes)
            {
                double thrs = ns_window_functions.GetMyThreshold(i, alpha, beta_0, beta_1, N);
                node.SetEnergyThreshold(thrs);
            }
        }
    }
    public enum LayerType
    {
        primary =0,
        secondary =1,
        shared =2
    }
   public class ns_layer
    {
       public int layer_id ;
       public LayerType LayerType;
       private int layer_importance;
       public ns_layer ( int id, LayerType _type)
       {
           this.LayerType = _type;
           this.layer_id = id;
       }
       private List<ns_node> __nodes = new List<ns_node>();
       public List<ns_node> NodesInLayer
       {
           get
           {
               return __nodes;
           }
           set
           {
               __nodes = value;
           }
       }
       public int Importance
       {
           get
           {
               if (__nodes.Count == 0) return 1;
               return __nodes.Sum(n =>
                   {
                       return n.GetImportanceValue();
                   });
           }
           
       }

    }
}
