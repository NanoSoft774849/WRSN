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
    public class ns_units
    {
        public static string F1000(double value , string unit)
        {
            return string.Format("{0}{1}", value * 1000, unit);
        }
    }
    public class ns_link
    {
        public string link_id;
        public ns_node src;
        public ns_node dst;
        public ns_attributes link_attrs;
        public int cost;

        public ns_link(string link_id, ns_node src , ns_node dst)
        {
            this.dst = dst;
            this.src = src;
            this.link_id = link_id;
            this.link_attrs = new ns_attributes();
            double link_dist = src.location.edistance(dst.location);
            this.link_attrs.add("distance", link_dist);
        }
        public ns_link(ns_node src, ns_node dst)
        {
            this.dst = dst;
            this.src = src;
           // dst.ChildNodes.add(src);
            this.link_id = string.Format("link_{0}_{1}", src.tag, dst.tag);
            this.link_attrs = new ns_attributes();
            double link_dist = src.location.edistance(dst.location);
            this.link_attrs.add("distance", link_dist);
            Random delay = new Random((int)DateTime.Now.Millisecond);
           // Random bw = new Random(1000);

            this.set_link_attrs("delay", ns_units.F1000(delay.NextDouble(), "ns"),
                "bandwidth", ns_units.F1000(delay.NextDouble(), "Mbps"));
            this.cost = (int)(delay.Next(100));
        }
        public ns_link set_link_attrs(params object[] attrs)
        {
            this.link_attrs.SetAttributes(attrs);
            return this;
        }

        public ns_keyValuePair getAttribute(string key)
        {
            return this.link_attrs[key];
        }
        public bool HasAttribute(string key)
        {
            return this.link_attrs.hasAttribute(key);
        }
        public ContextMenu getContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            string item = string.Format("src:{0} dst:{1}", src.tag, dst.tag);
            menu.Items.Add(item);
            this.link_attrs.Foreach((kvp) =>
                {
                    item = string.Format("{0}:{1}", kvp.key, kvp.value);
                    menu.Items.Add(item);
                });

            return menu;
        }

        public static ns_point AddLinkx(ns_point src , ns_point dst, double cost,UIElementCollection ele )
        {
            Brush color = (src + dst).ColorFromPoint();
            double link_value = cost;

            Path link = new ns_pen().M(src).L(dst).SetStrokeColor(color).getPath();
            //link.StrokeStartLineCap = PenLineCap.Triangle;
          //  link = ns_line.Arrow(src, dst, 0.744, color);
            
            //link.ContextMenu = this.getContextMenu();
            double x = 0; //Math.Abs(src.location.x + dst.location.x) * 0.5;
            double y = 0;// Math.Abs(src.location.y + dst.location.y) * 0.5;
            ns_point center = src.lerp(dst, 0.55);
            x = center.x;
            y = center.y;
            //center = center.shift(-5, -5);
            Label label = new Label();

            //label.Content = string.Format("{0:0}", link_value);
            label.Margin = new Thickness(x, y, 0, 0);
            label.Foreground = color;
            label.ToolTip = string.Format("angle:{0} , dist:{1}", src.angledeg(dst),
                src.edistance(dst));


            {


                ele.Add(link);

                ele.Add(label);
            }
            return center.shifty(-35);
           
        }
        public static void RenderLink(ns_node src , ns_node dst , double cost ,  UIElementCollection ele)
        {
            Brush color = (src.location + dst.location).ColorFromPoint();
            double link_value = cost;

            Path link = new ns_pen().M(src.location).L(dst.location).SetStrokeColor(color).getPath();
            //link.StrokeStartLineCap = PenLineCap.Triangle;
            link = ns_line.Arrow(src.location, dst.location, 0.744, color);
            //link.ContextMenu = this.getContextMenu();
            double x = 0; //Math.Abs(src.location.x + dst.location.x) * 0.5;
            double y = 0;// Math.Abs(src.location.y + dst.location.y) * 0.5;
            ns_point center = src.location.lerp(dst.location, 0.55);
            x = center.x;
            y = center.y;
            //center = center.shift(-5, -5);
            Label label = new Label();
            
            label.Content = string.Format("{0:0}", link_value);
            label.Margin = new Thickness(x, y, 0, 0);
            label.Foreground = color;
            label.ToolTip = string.Format("angle:{0} , dist:{1}",src.location.angledeg(dst.location) ,
                src.location.edistance(dst.location));

            
            {


                ele.Add(link);

                ele.Add(label);
            }
           
        }
        public ns_link RenderTo(UIElementCollection ele)
        {
            Brush color = (src.location + dst.location).ColorFromPoint();
            double link_value = src.getLinkCost(dst);
            
            Path link = new ns_pen().M(this.src.location).L(this.dst.location).SetStrokeColor(color).getPath();
            //link.StrokeStartLineCap = PenLineCap.Triangle;
            link = ns_line.Arrow(this.src.location, this.dst.location, 0.744, color);
            link.ContextMenu = this.getContextMenu();
            double x =0; //Math.Abs(src.location.x + dst.location.x) * 0.5;
            double y = 0;// Math.Abs(src.location.y + dst.location.y) * 0.5;
            ns_point center = src.location.lerp(dst.location, 0.55);
             x = center.x;
             y = center.y;
            //center = center.shift(-5, -5);
            Label label = new Label();
            //int cost = Math.Abs(src.cost + dst.cost);
            //dst.cost = this.cost;
            //src.cost = this.cost;
            //src.cost =  dst.cost;
            label.Content = string.Format("{0:0}", link_value);
            label.Margin = new Thickness(x,y, 0, 0);
            label.Foreground = color;
            label.ToolTip = string.Format("{0}-{1}", src.tag, dst.tag);
           
            //mtext.Margin = new Thickness(this.location.x - 5, this.location.y - 25, 5, 0);
           // if (!src.link_render)
            {


                ele.Add(link);

                ele.Add(label);
            }
               //dst.link_render = true;// don't render again.
            //ele.Add(center.render(4));

            return this;
        }

    }
}
