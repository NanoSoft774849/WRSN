using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace ns.wrsn
{
   public class wsn_network :ns_canvas
    {

    }
    
    public class wsn_network_cluster : ns_canvas
    {


        public string cluster_id;

        public ns_list<sensor_node> Sensors;
        public ns_list<mobile_charger> mobileChargers;
        public ns_list<base_station> BaseStations;
         
       
        public wsn_network_cluster( string id )
        {
            this.cluster_id = id;
            init();
            this.Width = 200;
            this.Height = 200;
            ns_rect rect = new ns_rect(new ns_point(0, 0), 200, 200);
            rect.RenderTo(this.Children);
           
        }
        public wsn_network_cluster ( string id ,  double x , double y , double w, double h )
        {
            this.Height = h;
            this.Width = w;
            this.Margin = new System.Windows.Thickness(x, y, 0, 0);
            this.cluster_id = id;
            ns_rect rect = new ns_rect(new ns_point(0, 0), w, h);
            rect.RenderTo(this.Children);
            init();
        }
        private void init()
        {
            this.Sensors = new ns_list<sensor_node>();
            this.mobileChargers = new ns_list<mobile_charger>();
            this.BaseStations = new ns_list<base_station>();
            
        }
        public wsn_network_cluster AddSensors(params sensor_node[] sensors)
        {
            foreach(sensor_node sen in sensors)
            {
                this.Sensors.add(sen);
            }

            return this;
        }
        public wsn_network_cluster AddMobileCharges(params mobile_charger[] mcs)
        {
            foreach( mobile_charger mc in mcs)
            {
                this.mobileChargers.add(mc);
            }
            return this;
        }
        public wsn_network_cluster AddBaseStations(params base_station[] bss)
        {
            foreach(base_station bs in bss)
            {
                this.BaseStations.add(bs);
            }
            return this;
        }
        public wsn_network_cluster Render()
        {
            this.Sensors.Foreach((s) =>
           {
               s.location = s.location.mod(this.Width, this.Height);
               s.RenderTo(this.Children);
           });
            this.mobileChargers.Foreach((m) =>
                {
                    m.location = m.location.mod(this.Width, this.Height);
                    m.RenderTo(this.Children);
                });
            this.BaseStations.Foreach((bs) =>
                {
                    bs.location = bs.location.mod(this.Width, this.Height);
                    bs.RenderTo(this.Children);
                });
            return this;
        }

    }
}
