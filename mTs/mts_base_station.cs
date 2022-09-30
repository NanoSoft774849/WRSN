using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace mTs
{
    public class base_station : ns_node
    {

        public base_station (string  bs_id):base(bs_id)
        {
            this.Radius = 15;
            this.NodeType = Node_TypeEnum.BaseStation;
            __init__();
        }
        private void __init__()
        {
            this.PowerConfig.Comm_Range = 200;
            this.PowerConfig.initial_energy = 1000;
            this.PowerConfig.PacketEnergy = 0.0001;
            this.PowerConfig.energy_consumptionRate = 0.001;
            this.PowerConfig.energy_threshold = 20;
            this.PowerConfig.BatteryCapacity = 5000;
            this.PowerConfig.BatteryDischargeRate = 0.5;
            this.PowerConfig.Temperature = 30;
        }
        public base_station (string bs_id , double x , double y ) : base(bs_id, x, y)
        {
          
            this.Radius = 8;
            this.NodeType = Node_TypeEnum.BaseStation;
            __init__();
        }
        public base_station (string bs_id , ns_point bs_loc) : base ( bs_id, bs_loc)
        {
            this.Radius = 8;
            this.NodeType = Node_TypeEnum.BaseStation;
            __init__();
        }
        
    }
}
