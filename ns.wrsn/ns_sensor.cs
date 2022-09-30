using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace ns.wrsn
{
    public class ns_sensor
    {
        public double SensorRange { get; set; }
        public string sensor_name { get; set; }
        public ns_attributes specifications { get; set; }
        public ns_sensor()
        {
            this.specifications = new ns_attributes();
        }
        public ns_sensor(string sensor_type , double sensitivty )
        {
            this.sensor_name = sensor_type;
            this.SensorRange = sensitivty;
            this.specifications = new ns_attributes();
        }
        public ns_sensor addSpecification(string spec_name , double spec_value)
        {
            this.specifications.add(spec_name, spec_value);
            return this;
        }
        public double getSpec(string name)
        {
            if(this.specifications.hasAttribute(name))
            {
                return (double)this.specifications.getAttributeValue(name);
            }
            return 0.0;
        }
        
    }
}
