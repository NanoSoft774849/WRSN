using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace ns.wrsn
{
    /// <summary>
    /// Radio Module for WSN Node.
    /// </summary>
    public class ns_rf_module
    {
        public Rf_Type RFType { get; set; }
        public double ComRange { get; set; }
        public double DataRate { get; set; }
        public ns_attributes specifications { get; set; }
        public string Name { get; set; }
        public ns_rf_module()
        {
            this.RFType = Rf_Type.WIFI;
            this.ComRange = 200; // 100 meter
            this.specifications = new ns_attributes();
            this.Name = "wifi";
        }
        public ns_rf_module (Rf_Type type  , double comrange ,  string name)
        {
            this.RFType = type;
            this.specifications = new ns_attributes();
            this.Name = name;
            this.ComRange = comrange;
        }
        public ns_rf_module addSpecification(string spec_name, double spec_value)
        {
            this.specifications.add(spec_name, spec_value);
            return this;
        }
        public double getSpec(string name)
        {
            if (this.specifications.hasAttribute(name))
            {
                return (double)this.specifications.getAttributeValue(name);
            }
            return 0.0;
        }

    }
    public enum Rf_Type
    {
        WIFI=0,
        BT=1,
        Zigbee,
        GSM,
        LORA,
        FM,
        AM
    };
}
