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
    public class ns_pen
    {
        public string data;
        public Brush Strokecolor;
        public double thickness;
        public Brush FillColor;
        public Dictionary<string, List<ns_point>> dict = new Dictionary<string,List<ns_point>>();
        public ns_pen()
        {
            this.data = "";
            Color f = Color.FromArgb(255, (byte)0x7c, (byte)0xb5, (byte)0xec);
            this.Strokecolor = new SolidColorBrush(f);
            this.thickness = 1.0;
        }
        public ns_pen(string _data)
        {
            this.data = _data;
            Color f = Color.FromArgb(255, (byte)0x7c, (byte)0xb5, (byte)0xec);
            this.Strokecolor = new SolidColorBrush(f);
        }
        public ns_pen ( Brush stroke , double thickness)
        {
            this.data = "";
            //Color f = Color.FromArgb(255, (byte)0x7c, (byte)0xb5, (byte)0xec);
            this.Strokecolor = stroke;
            this.thickness = thickness;
        }
        public static ns_pen operator +(ns_pen pen, string s)
        {
            pen.data += " " + s;
            return pen;
        }
        private void AddPointtoDict(string tag , ns_point mp)
        {
            if(this.dict.ContainsKey(tag))
            {
                this.dict[tag].Add(mp);
                return;
            }
            List<ns_point> ml = new List<ns_point>();
            ml.Add(mp);
            this.dict[tag] = ml;
        }
        public ns_pen moveto(double x, double y)

        {
            AddPointtoDict("M", new ns_point(x, y));
            return this + new ns_point(x, y).moveto();
        }
        public ns_pen M(double x, double y)
        {
            AddPointtoDict("M", new ns_point(x, y));
            return this + new ns_point(x, y).moveto();
        }
        public ns_pen moveto(ns_point p)
        {
            AddPointtoDict("M", p);
            return this + p.moveto();
        }
        public ns_pen M(ns_point p)
        {
            AddPointtoDict("M", p) ;
            return this + p.moveto();
        }

        public ns_pen lineto(double x, double y)
        {
            AddPointtoDict("L", new ns_point(x, y));
            return this + new ns_point(x, y).lineto();
        }
        public ns_pen L(double x, double y)
        {
            AddPointtoDict("L", new ns_point(x, y));
            return this + new ns_point(x, y).lineto();
        }
        public ns_pen lineto(ns_point p)
        {
            AddPointtoDict("L", p);
            return this + p.lineto();
        }
        public ns_pen L(ns_point p)
        {
            AddPointtoDict("L", p);

            return this + p.lineto();
        }
        public ns_pen vlineto( double y)
        {
            AddPointtoDict("V", new ns_point(0,y));

            return this + new ns_point(0, y).vlineto();
        }
        public ns_pen V(double y)
        {
            AddPointtoDict("V", new ns_point(0, y));

            return this + new ns_point(0, y).vlineto();
        }

        public ns_pen hlineto(double x)
        {
            AddPointtoDict("H", new ns_point(x, 0));

            return this + new ns_point(x, 0).hlineto();
        }
        public ns_pen H(double x)
        {
            AddPointtoDict("V", new ns_point(x, 0));

            return this + new ns_point(x,0).hlineto();
        }

        /// <summary>
        /// A rx ry x-axis-rotation large-arc-flag sweep-flag x y
        ///a rx ry x-axis-rotation large-arc-flag sweep-flag dx dy
        /// </summary>
        /// <param name="rx"> x radius</param>
        /// <param name="ry"> y radius</param>
        /// <param name="x_ax_rota"> roatation in x axis</param>
        /// <param name="l_arg_flag">large arc flag</param>
        /// <param name="sweep_flag">sweep flag</param>
        /// <param name="x">end point x</param>
        /// <param name="y">end point y</param>
        /// <returns></returns>
        public ns_pen Arc(double rx, double ry, double x_ax_rota, double l_arg_flag, double sweep_flag, double x, double y)
        {
            data += string.Format("A {0} {1} {2} {3} {4} {5} {6}", rx, ry, x_ax_rota, l_arg_flag, sweep_flag, x, y);
            return this;
        }
        public ns_pen A(double rx, double ry, double x_ax_rota, double l_arg_flag, double sweep_flag, double x, double y)
        {
            data += string.Format("A {0} {1} {2} {3} {4} {5} {6}", rx, ry, x_ax_rota, l_arg_flag, sweep_flag, x, y);
            return this;
        }
        public ns_pen Arc(double rx, double ry, double x, double y)
        {
            return Arc(rx, ry, 0, 0, 0, x, y);
        }
        public ns_pen A(double rx, double ry, double x, double y)
        {
            return Arc(rx, ry, 0, 0, 0, x, y);
        }
        public ns_pen A(ns_point rxy, ns_point xy)
        {
            return A(rxy.x, rxy.y, xy.x, xy.y);
        }

        /// <summary>
        /// Draw Bézier Curves
        /// /*
        //     *  C x1 y1, x2 y2, x y
        //        (or)
        //        c dx1 dy1, dx2 dy2, dx dy
        //     */
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public ns_pen CubicCurve(double x1, double y1, double x2, double y2, double x, double y)
        {

            this.data += string.Format(" C {0} {1}, {2} {3}, {4} {5}", x1, y1, x2, y2, x, y);
            return this;
        }
        public ns_pen C(double x1, double y1, double x2, double y2, double x, double y)
        {

            this.data += string.Format(" C {0} {1}, {2} {3}, {4} {5}", x1, y1, x2, y2, x, y);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1">Control point p1</param>
        /// <param name="p2">Control point p2</param>
        /// <param name="p">End point the end of the curve.</param>
        /// <returns></returns>
        public ns_pen C(ns_point p1, ns_point p2 , ns_point p)
        {
            this.data += string.Format(" C {0}, {1}, {2}", p1, p2, p);
            return this;
        }

        /// <summary>
        /// Q x1 y1, x y
        /// </summary>
        /// <param name="x1"> X1 Start point of the arc</param>
        /// <param name="y1">Start point of the Qrc</param>
        /// <param name="x">end point x</param>
        /// <param name="y">end point y</param>
        /// <returns></returns>
        public ns_pen QuadraticCurve(double x1, double y1, double x, double y)
        {

            this.data += string.Format(" Q {0} {1} , {2} {3}", x1, y1, x, y);
            return this;
        }
        public ns_pen Q(double x1, double y1, double x, double y)
        {
            this.data += string.Format(" Q {0} {1} , {2} {3}", x1, y1, x, y);
            return this;
        }

        public ns_pen Q(ns_point p1, ns_point p2)
        {
            this.data += string.Format(" Q {0} , {1}", p1, p2);
            return this;
        }
        public ns_pen Z()
        {
            return this + "Z";
        }
        public ns_pen closePath()
        {
            return this.Z();
        }
        /// short functions Start here
        /// 

        public string getData()
        {
            return this.data;
        }
        public ns_pen SetStrokeColor(Brush color)
        {
            this.Strokecolor = color;
            return this;
        }
       
       public string toLatex(double scale)
        {
           //if(string.IsNullOrEmpty(this.dta))
            if (this.dict.Count == 0) return "";
            string latex = "";
           
           foreach(var p in this.dict)
           {
              foreach(var pp in p.Value)
              {
                  latex += pp.toLatexPoint(scale) + "--";
              }
           }
           if(latex.Length>2)
           latex = latex.Substring(0, latex.Length - 2);
           latex = string.Format("\\draw {0};\n", latex);
           return latex;
        }
       public string toLatex(double scale = 50, double thickness_mm = 0.1, string unit ="mm", string color = "red")
       {
           //if(string.IsNullOrEmpty(this.dta))
           if (this.dict.Count == 0) return "";
           string latex = "";

           foreach (var p in this.dict)
           {
               foreach (var pp in p.Value)
               {
                   latex += pp.toLatexPoint(scale) + "--";
               }
           }
           if (latex.Length > 2)
               latex = latex.Substring(0, latex.Length - 2);
           latex = string.Format("\\draw[color={0}, line width={1}{2}]  {3};\n",color, thickness_mm, unit, latex);
           return latex;
       }
        public Path getPath()
        {
            Path mp = new Path();

            mp.Data = Geometry.Parse(this.data);
            mp.StrokeThickness = this.thickness;
            mp.Stroke = this.Strokecolor;
            mp.AllowDrop = true;
            if(this.FillColor!=null)
            {
                mp.Fill = this.FillColor;
            }

            mp.MouseEnter += mp_MouseEnter;
            mp.MouseLeave += mp_MouseLeave;
            mp.MouseDown += mp_MouseDown;
            mp.MouseUp += mp_MouseUp;
            mp.MouseMove += mp_MouseMove;
           
            
          //  mp.Drop += mp_Drop;
           // mp.MouseMove += mp_MouseMove;
            return mp;

        }
        private bool moving = false;
        void mp_MouseMove(object sender, MouseEventArgs e)
        {
            Path p = sender as Path;
            if(moving)
            {
                Point pp = e.GetPosition(null);
                p.Margin = new Thickness(pp.X, pp.Y, 0, 0);
            }
        }

      
        public Path getPath(Brush fill)
        {
            Path mp = new Path();

            mp.Data = Geometry.Parse(this.data);
            mp.StrokeThickness = this.thickness;
            mp.Stroke = this.Strokecolor;
            mp.Fill = fill;

            mp.MouseEnter += mp_MouseEnter;
            mp.MouseLeave += mp_MouseLeave;
            mp.MouseDown += mp_MouseDown;
            mp.MouseUp += mp_MouseUp;
            mp.MouseMove += mp_MouseMove;
            
           // mp.Dr
            return mp;
        }

        private void mp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            moving = false;
        }

        void mp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton== MouseButtonState.Pressed)
            {
                this.moving = true;
            }
        }

        void mp_MouseLeave(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        void mp_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        
        public static Path p(string data, Brush stroke, double thickness)
        {
            Path mp = new Path();
            mp.Stroke = stroke;
            mp.StrokeThickness = thickness;
            mp.Data = Geometry.Parse(data);
            return mp;
        }
        public static Path p(string data, Brush stroke, double thickness, Brush fill)
        {
            Path mp = new Path();
            mp.Stroke = stroke;
            mp.StrokeThickness = thickness;
            mp.Data = Geometry.Parse(data);
            mp.Fill = fill;
            return mp;
        }

    }
}
