using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.graph;
namespace TestApplications
{
    class Program
    {
        static void Main(string[] args)
        {
            ns_attributes attr = new ns_attributes();
            attr.add("id", 1001);
            attr.add("tag", "node_id");
            attr.add("power", 10.11f).add("delay","10ms").add("bandwidth","100Mbps");
            attr[(item) => { return item.key == "id"; }].value = 101;
            attr.Foreach(item =>
                {
                    Console.WriteLine("{0} \t {1}", item.key, item.value);
                });
            Console.WriteLine("-----------------------");
            attr.SelectMany("id", "delay", "power").Foreach((item) =>
                {
                    Console.WriteLine("{0} \t {1}", item.key, item.value);
                });
        }
    }
}
