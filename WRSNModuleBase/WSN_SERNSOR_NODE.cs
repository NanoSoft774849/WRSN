using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;

namespace WRSNModuleBase
{

    public enum SensorNodeState
    {
        dead=0,
        alive=1
    };
    public abstract class WSN_SERNSOR_NODE : ns_node
    {
        public SensorNodeState SensorState;
        public WSN_SERNSOR_NODE():base()
        {
            this.NodeType = Node_TypeEnum.Sensor_node;
            init_sensor();
        }
        public WSN_SERNSOR_NODE(string id , ns_point location):base(id,location)
        {
            this.NodeType = Node_TypeEnum.Sensor_node;
            init_sensor();
        }
        public WSN_SERNSOR_NODE(string id):base(id)
        {
            this.NodeType = Node_TypeEnum.Sensor_node;
            init_sensor();
        }
        public WSN_SERNSOR_NODE(string id, double x, double y):base(id,x,y)
        {
            this.NodeType = Node_TypeEnum.Sensor_node;
            init_sensor();
        }
        public abstract void init_sensor();

       
    }
}
