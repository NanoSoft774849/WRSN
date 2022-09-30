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
    public class ns_point3d
    {
        public double x;
        public double y;
        public double z;
        public ns_point3d(double _x , double _y, double _z)
        {
            this.x = _x;
            this.y = _y;
            this.z = _z;
        }

        /// <summary>
        /// (x0,y
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double edistance(ns_point3d p)
        {
            var x = this.x - p.x;
            var y = this.y - p.y;
            var z = this.z - p.z;

            x *= x;
            y *= y;
            z *= z;
            return Math.Sqrt(x + y + z);
        }
        public double norm()
        {
            /*var x = this.x*this.x;
            var y = this.y*this.y;
            var z= this.z*this.z;*/

            return Math.Sqrt(this.DotProduct(this));
        }
        public double DotProduct(ns_point3d p)
        {
            var x = this.x * p.x;
            var y = this.y * p.y;
            var z = this.z * p.z;
            return x + y + z;
        }
        public double angle( ns_point3d p)
        {
            var n_A = this.norm();
            var n_B = p.norm();
            var dp = DotProduct(p);
            return Math.Acos(dp / (n_A * n_B));
        }
    }
    public class ns_point 
    {
        public double x;
        public double y;
        public ns_point(double _x, double _y)
        {
            this.x = _x;
            this.y = _y;
           
            
        }
        public ns_point()
        {
            this.x = 0;
            this.y = 0;
        }
        public ns_point (Point p)
        {
            this.x = p.X;
            this.y = p.Y;
        }
        public Point toPoint()
        {
            return new Point(this.x, this.y);
        }
        public string toJson()
        {
            return string.Format("\"x\":{0},\"y\":{1}", this.x, this.y);
        }
        public Border ToUI()
        {
            Border bb = new Border();
            StackPanel sp = new StackPanel();
            Label x_ = new Label();
            x_.Foreground = this.ColorFromPoint();
            Label y_ = new Label();
            y_.Foreground = this.ColorFromPoint();
            x_.Content = "Location X:";
            y_.Content = "Location Y:";
            TextBox tbx = new TextBox();
            tbx.Text = x.ToString();
            tbx.TextChanged += tbx_TextChanged;
           
            TextBox tby = new TextBox();
            tby.Text = this.y.ToString();
            tby.TextChanged += tby_TextChanged;
            bb.BorderBrush = this.ColorFromPoint();
            bb.BorderThickness = new Thickness(1);
            sp.Children.Add(x_);
            sp.Children.Add(tbx);
            sp.Children.Add(y_);
            sp.Children.Add(tby);
            bb.Child = sp;

            return bb;

        }

        void tby_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tby = sender as TextBox;
            string txt = tby.Text;
            if (string.IsNullOrEmpty(txt)) return;
            try
            {
                this.y = double.Parse(txt);
            }
            catch (Exception) { return; };
        }

        void tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tby = sender as TextBox;
            string txt = tby.Text;
            if (string.IsNullOrEmpty(txt)) return;
            try
            {
                this.x = double.Parse(txt);
            }
            catch (Exception) { return; };
        }
        public static ns_point operator +(ns_point p1, ns_point p2)
        {
            return new ns_point(p1.x + p2.x, p1.y + p2.y);
        }

        public static ns_point operator -(ns_point p1, ns_point p2)
        {
            return new ns_point(p1.x - p2.x, p1.y - p2.y);
        }
        public static ns_point operator /(ns_point p, double r)
        {
            if (r == 0)
                return p;
            return new ns_point(p.x / r, p.x / 2);
        }
        public static ns_point operator*(ns_point p , double r)
        {
            return new ns_point(p.x * r, p.y * r);
        }
        public string toLatexCircle(double r , double scale)
        {
            //\node[mark size=3pt,color=red] at (0,-1) {\pgfuseplotmark{*}};
            string latex = string.Format("\\node[mark size={0}pt,color=red] at ({1:0.00},{2:0.00}) ", r, this.x / scale, this.y / scale);
            latex += "{\\pgfuseplotmark{*}};";
            return latex;
        }
        public string toLatexColor(string name)
        {
            //	
            double r = (this.x % 255) / 255;
            double g = (this.y % 255) / 255;
            double b = (this.edistance(new ns_point()) % 255) / 255;
            string _color = "\\definecolor{" + name + "}{rgb}{" + string.Format("{0:0.00},{1:0.00},{2:0.00}", r, g, b) + "};\n";
            return _color;

        }
        public string toLatexCircle(string label , double r , double scale)
        {
            label = label.Replace("_", "_{") + "}";
            label = "$" + label + "$";
            string latex = string.Format("\\node[mark size={0}pt,color=red , label=above:{3}] at ({1},{2}) ", r, this.x / scale, this.y / scale, label);
            latex += "{\\pgfuseplotmark{*}};";
            return latex;
        }
        public string latexDrawTextAt(string txt  , string color)
        {
            //\node[color=red] at (3.13,3.27) {10};
            return "\\node[color=" + color + "] at " + this.toLatexPoint(50) + " {" + txt + "};\n";
        }
        public string toLatexCircle(string label , double r , double scale , string color)
        {
            string coord= toLatexCoordinate(label, scale);
            string cord = label;
            if( cord.Contains("_"))
            {
                cord = cord.Replace("_", "");
            }
            label = label.Replace("_", "_{") + "}";
            label = "$" + label + "$";
            string  latex = coord;
            // latex += string.Format("\\node[mark size={0}pt,color={4} , label=above:{3}] at ({1},{2}) ", r, this.x / scale, this.y / scale, label , color);
            //latex += "{\\pgfuseplotmark{*}};\n";

            latex += string.Format("\\node[mark size={0}pt,color={1} , label=above:{2}] at ({3}) ", r, color, label, cord);
            latex += "{\\pgfuseplotmark{*}};\n";
           
            return toLatexColor(color)+latex;
        }
        public string toLatexPoint()
        {
            return string.Format("({0:0.000},{1:0.000})", this.x, this.y);
        }
        public string toLatexCoordinate(string var_name, double scale)
        {
            if(var_name.Contains("_"))
            {
                var_name = var_name.Replace("_","");// this will make it easy.
            }
            return string.Format("\\coordinate ({0}) at {1};\n", var_name, toLatexPoint(scale));
        }
        public string toLatexPoint(double scale)
        {
            return string.Format("({0:0.000},{1:0.000})", this.x / scale, this.y / scale);
        }

        public static ns_point operator*(double r , ns_point p)
        {
            return new ns_point(p.x * r, p.y * r);
        }
        public  ns_point scale(double r)
        {
            return new ns_point(this.x * r, this.y * r);
        }
        public ns_point scale(double xs, double ys)
        {
            return new ns_point(this.x * xs, this.y * ys);
        }

        public static bool operator==(ns_point p1  , ns_point p2)
        {
            return (p1.x == p2.x && p1.y == p2.y);
        }
        /// <summary>
        /// calc linear inter of the point
        /// 
        /// </summary>
        /// <param name="p1">end point</param>
        /// <param name="tao">interplotion constant from [0-1]</param>
        /// <returns></returns>
        public ns_point lerp(ns_point p1 , double tao)
        {
            if (tao > 1) return new ns_point();// 
            ns_point px = this * ((1 - tao)) + (p1) * (tao);
            return px;
        }
        public double magnitude()
        {
            return Math.Sqrt((this.x * this.x) + (this.y * this.y));
        }
        public static bool operator !=(ns_point p1, ns_point p2)
        {
            return (p1.x != p2.x && p1.y != p2.y);
        }
        /// <summary>
        /// dot product
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double dot(ns_point p)
        {
            return (this.x * p.x) + (this.y * p.y);
        }
        /// <summary>
        /// calc the ecludian distance
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double edistance(ns_point p)
        {
            double dx = (this.x - p.x) ;
            double dy = (this.y - p.y);

            return Math.Sqrt((dx * dx) + (dy * dy));
        }
        /// <summary>
        /// Calc the Manhatan distance between two point vector
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double mdistance(ns_point p)
        {
            ns_point pp = this - p;
            return Math.Abs(pp.x) + Math.Abs(pp.y);
        }
        /// <summary>
        /// the angle between two points in radians
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double angle(ns_point p)
        {
            
            ns_point fx = this - p;
            double inRads = Math.Atan2(fx.y, fx.x);
            return inRads; // Math.PI+ang --> this is good 
            
        }
        public double angle2(ns_point p)
        {
            ns_point fx = this - p;
            double inRads = Math.Atan(fx.y / fx.x);
            return inRads; // Math.PI+ang --> this is good 
        }
        /// <summary>
        /// calc the angle in degree between the two points;
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double angledeg(ns_point p)
        {
            return angle(p) * 180.0 / Math.PI;
        }
        /// <summary>
        /// cross product 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double cross(ns_point p)
        {
            /// p1xp2 = |p1|*|p2|sin(theta)
            /// |p1| norm of first point 
            /// |p2| norm of second point
            /// theta is the angle between p1 and p2
            double theta = this.angle(p);
            double n1 = this.norm();
            double n2 = p.norm();
            return n1 * n2 * Math.Sin(theta);

        }
        public ns_point shiftx(double r)
        {
            return this + new ns_point(r, 0);
        }
        public ns_point shifty(double r)
        {
            return this + new ns_point(0, r);
        }
        /// <summary>
        /// norm of point ax+by
        /// </summary>
        /// <returns></returns>
        public double norm()
        {
            return this.edistance(new ns_point(0, 0));
        }
        public override string ToString()
        {
            return string.Format(" {0} {1} ", this.x, this.y);
        }
        public string lineto()
        {
            return string.Format("L{0}", this.ToString());
        }
        public string moveto()
        {
            return string.Format("M{0}", this.ToString());
        }
        public string hlineto()
        {
            return string.Format("H{0}", this.x);
        }
        public string vlineto()
        {
            return string.Format("V{0}", this.y);
        }
        public ns_point mod(double xm, double ym)
        {
            return new ns_point(this.x % xm, this.y % ym);
        }
        public ns_point shift(double x, double y)
        {
            return this + new ns_point(x, y);
        }
        /// <summary>
        /// to Radians
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public double toR(double deg)
        {
            return (deg % 360.0) * Math.PI / 180.0;
        }
        public ns_point rotate(double ang_deg)
        {
            double beta = toR(ang_deg);
            double x = (this.x * Math.Cos(beta)) - (this.y) * Math.Sin(beta);
            double y = (this.x * Math.Sin(beta)) + (this.y * Math.Cos(beta));
            return new ns_point(x, y);
        }
        public Path render(double r , Brush color)
        {
            Path p = new Path();
            string data = string.Format("M {0} A {2} {2} 0 1 1 {1}", this, this.shift(0.5, 0.5), r);
            //#7cb5ec
            // Color f = Color.FromArgb(255, (byte)0x7c, (byte)0xb5, (byte)0xec);
            // Color f = this.ColorFromPoint();
            Brush bb = this.ColorFromPoint();
            p.Fill = bb;
            p.Stroke = color;
            p.StrokeThickness = 1.0;
            p.Data = Geometry.Parse(data);
            p.ToolTip = "(" + this.ToString() + ")";
            return p;
        }
        public Path renderNoFill(double r)
        {
            Path p = new Path();
            string data = string.Format("M {0} A {2} {2} 0 1 1 {1}", this, this.shift(0.5, 0.5), r);
            //#7cb5ec
            // Color f = Color.FromArgb(255, (byte)0x7c, (byte)0xb5, (byte)0xec);
            // Color f = this.ColorFromPoint();
            Brush bb = this.ColorFromPoint();
           // p.Fill = bb;
            p.Stroke = bb;
            p.StrokeThickness = 1.0;
            p.Data = Geometry.Parse(data);
            p.ToolTip = "(" + this.ToString() + ")";
            return p;
        }
        public string GetRenderStr(double r)
        {
            string data = string.Format("M {0} A {2} {2} 0 1 1 {1}", this, this.shift(0.5, 0.5), r);
            return data;
        }
        public ns_point Renderfx(double r)
        {
            //d="M 58 106.88004999999998 A 4 4 0 1 1 58.00399999933334 106.88004800000014 Z";
            Path p = new Path();
            string data = string.Format("M {0} A {2} {2} 0 1 1 {1}", this, this.shift(0.5, 0.5), r);
            //#7cb5ec
            // Color f = Color.FromArgb(255, (byte)0x7c, (byte)0xb5, (byte)0xec);
            // Color f = this.ColorFromPoint();
            Brush bb = this.ColorFromPoint();
            p.Fill = bb;
            p.Stroke = bb;
            p.StrokeThickness = 1.0;
            p.Data = Geometry.Parse(data);
            p.ToolTip = "(" + this.ToString() + ")";
            
            return this;
        }
        public string toLatexDataFilePoint()
        {
            return string.Format("{0}\t {1}\n", this.x, this.y);
        }
        public Path render(double r)
        {
            //d="M 58 106.88004999999998 A 4 4 0 1 1 58.00399999933334 106.88004800000014 Z";
            Path p = new Path();
            string data = string.Format("M {0} A {2} {2} 0 1 1 {1}", this, this.shift(0.5, 0.5), r);
            //#7cb5ec
           // Color f = Color.FromArgb(255, (byte)0x7c, (byte)0xb5, (byte)0xec);
           // Color f = this.ColorFromPoint();
            Brush bb = this.ColorFromPoint();
            p.Fill = bb ;
            p.Stroke = bb;
            p.StrokeThickness = 1.0;
            p.Data = Geometry.Parse(data);
            p.ToolTip = "(" + this.ToString() + ")";
           
            return p;
        }
        public void RenderTo_withLabel(string label , double r, UIElementCollection eles)
        {
            Path p = new Path();
            string data = string.Format("M {0} A {2} {2} 0 1 1 {1}", this, this.shift(0.5, 0.5), r);
            //#7cb5ec
            // Color f = Color.FromArgb(255, (byte)0x7c, (byte)0xb5, (byte)0xec);
            // Color f = this.ColorFromPoint();
            Brush bb = this.ColorFromPoint();
            p.Fill = bb;
            p.Stroke = bb;
            p.StrokeThickness = 1.0;
            p.Data = Geometry.Parse(data);
            p.ToolTip = "(" + this.ToString() + ")";
            Label mtext = new Label();
            mtext.Content = label;
            double s = Math.Log10(r);
            ns_point pp = this.shift(0, -Math.PI * r/2);
            mtext.Margin = new Thickness(pp.x, pp.y, 0, 0);

            eles.Add(p);
            eles.Add(mtext);
        }
        public double ThresholdFromLocation(double alpha)
        {
            double r = (this.x);
            double g = (this.y);
            double b = (this.edistance(new ns_point()));
            double a = (this.magnitude());
            return  (r + g* + b) / (a);
        }
        public double ThresholdFromLocation(double c1,double c2, double c3)
        {
            double r = (this.x);
            double g = (this.y);
            double b = (this.edistance(new ns_point()));
            double a = (this.magnitude());
            return (r * c1 + g * c2 + b * c2);// / (a);
        }
        public Brush ColorFromPoint()
        {
            byte r = (byte) (this.x % 255);
            byte g = (byte)(this.y % 255);
            byte b = (byte)(this.edistance(new ns_point()) % 255);
            byte a = (byte)(this.magnitude() % 255);
            return new SolidColorBrush(Color.FromArgb(236, r, g, b));
        }
        
    }
   
   
}
