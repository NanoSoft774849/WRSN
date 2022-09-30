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
    public class ns_color
    {
        public byte r;
        public byte g;
        public byte b;

    }
  
    public class ns_poly
    {
        /// <summary>
        /// n:values from 1-360
        ///  2 -line 
        ///  3- triangle
        ///  4- square 
        ///  5-pentagon
        ///  6-hexagon
        ///  and so ...
        /// </summary>
        /// <param name="center"></param>
        /// <param name="n"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Path CreatePolygon(ns_point center, int n, double radius)
        {
            ns_pen pen = new ns_pen();
            ns_point p = new ns_point();
            List<ns_point> points = new List<ns_point>();

            //pen.M(center);
           
            for (int i = 0; i <= n; i++)
            {
                double angle = (360 / n) * i;// -(360 / 2 * n);
                var angle_rad = angle * Math.PI / 180;
                double x = center.x + radius * Math.Cos(angle_rad);
                double y = center.y + radius * Math.Sin(angle_rad);
                p = new ns_point(x, y);
                points.Add(p);
                if (i == 0)
                {
                    pen.M(p);
                    continue;
                }
                pen.L(p);
            }
            pen.Strokecolor = p.ColorFromPoint();
            pen.thickness = 1;
            pen.FillColor = p.ColorFromPoint();
            Path pxp = pen.getPath();
            pxp.Fill = p.ColorFromPoint();
            return pxp;
        }
    }
    public class ns_rect
    {
        public ns_point start;
        public double width;
        public double height;

        public string id;
        public bool fill;
        public Brush color;
        public Brush StrokeColor;
        public double thickness;
        public ns_rect(ns_point _start , double w , double h)
        {
            this.start = _start;
            this.width = w;
            this.height = h;
            fill = false;
            this.color = Brushes.Green;
            this.StrokeColor = Brushes.Green;
            this.thickness = 1.0;
        }

        public ns_point tl()
        {
            return this.start + new ns_point(0, this.height);
        }
        public ns_point tr()
        {
            return this.start + new ns_point(this.width, this.height);
        }
        public ns_point br()
        {
            return this.start + new ns_point(this.width, 0);
        }
        public ns_point bl()
        {
            return this.start - new ns_point(this.width, 0);
        }
        public ns_point center()
        {
            return (this.start + new ns_point(this.width / 2, this.height / 2));
        }
        public string RenderTo(UIElementCollection ele)
        {
            Path p = new Path();
            string data = this.start.moveto();
            data += this.br().lineto();
            data += this.tr().lineto();
            data += this.tl().lineto();
            data += this.start.lineto();
            //data += "Z";//close path
            p.Data = Geometry.Parse(data);
            p.StrokeThickness = this.thickness;
            p.Stroke = this.StrokeColor;
            if (this.fill)
                p.Fill = this.color;
            ele.Add(p);
            //ns_circle c0 = new ns_circle("s", this.start, 5);
            //ns_circle c1 = new ns_circle("br", this.br(), 5);
            //ns_circle c3 = new ns_circle("tr", this.tr(), 5);
            //ns_circle c4 = new ns_circle("tl", this.tl(), 5);
            //ns_circle ce = new ns_circle("c", this.center(), 5);
            //c0.RenderTo(ele);
            //c1.RenderTo(ele);
            //c3.RenderTo(ele);
            //c4.RenderTo(ele);
            //ce.RenderTo(ele);
            //ele.Add(ns_line.Create(br(), tl(), 1));
            //ele.Add(ns_line.Create(this.start, tr(), 1));
            return data;

        }
    }
    public class ns_line
    {
        public string id;
        public ns_point start;
        public ns_point end;
        public double thickness;
        public ns_line(string id , ns_point _start , ns_point _end)
        {
            this.id = id;
            this.start = _start;
            this.end = _end;
            this.thickness = 0.354;
        }
        public Path Render(Brush color)
        {
            Path mp = new Path();
            string _fmt = string.Format("M {0} {1} L {2} {3}", this.start.x, this.start.y, this.end.x, this.end.y);
            mp.Data = Geometry.Parse(_fmt);
            mp.Stroke = color;
            
            mp.StrokeThickness = this.thickness;
           
            return mp;
        }
        public static Path Arrow(ns_point p1, ns_point p2, double thickness)
        {
            //   ns_pen line = new ns_pen().M(20, 20).lineto(500, 20).L(495,17).M(500,20).L(495,23)

            double angle = 180 + p1.angledeg(p2);
            ns_point head = new ns_point(p2.x, p2.y) + new ns_point(-5, -3).rotate(angle);
            ns_point h2 = new ns_point(p2.x, p2.y) + new ns_point(-5, 3).rotate(angle);
            ns_pen pen = new ns_pen().M(p1).L(p2).L(head).M(p2).L(h2);
            pen.thickness = thickness;
            return pen.getPath();
        }
        public static Path Arrow(ns_point p1, ns_point p2, double thickness , Brush color)
        {
            //   ns_pen line = new ns_pen().M(20, 20).lineto(500, 20).L(495,17).M(500,20).L(495,23)

            double angle = 180 + p1.angledeg(p2);
            ns_point head = new ns_point(p2.x, p2.y) + new ns_point(-5, -3).rotate(angle);
            ns_point h2 = new ns_point(p2.x, p2.y) + new ns_point(-5, 3).rotate(angle);
            ns_pen pen = new ns_pen().M(p1).L(p2).L(head).M(p2).L(h2);
            pen.thickness = thickness;
            pen.SetStrokeColor(color);
            return pen.getPath();
        }
        public static Path Arrow(ns_point p1, ns_point p2, double thickness , double arrow_head)
        {
            ns_pen pen = new ns_pen().M(p1).L(p2).L(p2.shift(-arrow_head, -arrow_head)).M(p2).L(p2.shift(0, arrow_head));
            pen.thickness = thickness;
            return pen.getPath();
        }
        public static Path Create(ns_point p1, ns_point p2, double thickness)
        {
            Path mp = new Path();
            string _fmt = string.Format("M {0} {1} L {2} {3}", p1.x, p1.y, p2.x, p2.y);
            mp.Data = Geometry.Parse(_fmt);
            mp.Stroke = Brushes.Green;// (p1 + p2).ColorFromPoint();
            mp.StrokeThickness = thickness;
            return mp;
        }
        public static Path Create(ns_point p1, ns_point p2, double thickness , Brush color)
        {
            Path mp = new Path();
            string _fmt = string.Format("M {0} {1} L {2} {3}", p1.x, p1.y, p2.x, p2.y);
            mp.Data = Geometry.Parse(_fmt);
            mp.Stroke = color;
            mp.StrokeThickness = thickness;
            return mp;// Arrow(p1, p2, thickness);
        }
        public static Path Create(ns_circle c1 , ns_circle c2)
        {
            return Create(c1.location, c2.location, 0.354);
        }


    }
    public class ns_circle
    {
        public string id;
        public ns_point location;
        public double radius;
        public Brush color;
        public ns_attributes attrs;
        public int Layer_Index;
        public int column;
        public int row;
        public ns_circle(string id  , double x, double y , double radius)
        {
            this.id = id;
            this.location = new ns_point(x, y);
            this.radius = radius;
            this.color = Brushes.DarkBlue;
            this.attrs = new ns_attributes();
           

        }
        public ns_circle(double x , double y , double r) 
        {
            this.id = "";
            this.location = new ns_point(x, y);
            this.radius = r;
            this.color = Brushes.DarkBlue;
            this.attrs = new ns_attributes();
        }
        public ns_circle(string id, ns_point loc , double r) 
        {
            this.id = id;
            this.location = loc;
            this.radius = r;
            this.color = Brushes.DarkBlue;
            this.attrs = new ns_attributes();
        }
        public ns_circle() :base()
        {
            this.radius = 10;
            this.location = new ns_point(0, 0);
            this.id = "";
            this.color = Brushes.DarkBlue;
            this.attrs = new ns_attributes();
        }
        private Path Cr_Path;
        public Path RenderCr(Brush color)
        {
            Path mp = new Path();

            EllipseGeometry mg = new EllipseGeometry(this.location.toPoint(), 10*this.radius, 10*this.radius);
            //string cc = string.Format("M {0} {1} q {2} -300 300 0 ", x, y, x + radius, y + radius);
            mp.Data = mg;
            mp.Stroke = color;
            mp.Visibility = Visibility.Hidden;
            return mp;
        }
        public Path UiNode = new Path(); 
        public Path Render(Brush color)
        {
            Path mp = new Path();

            EllipseGeometry mg = new EllipseGeometry(this.location.toPoint(), this.radius, this.radius);
            //string cc = string.Format("M {0} {1} q {2} -300 300 0 ", x, y, x + radius, y + radius);
           mp.Data = mg;
           
           mp.Fill = color;
           mp.Name = id;
           SolidColorBrush cc = color as SolidColorBrush;
           string cv = cc.Color.ToString();
           var fx = Math.Sqrt((this.row) * (this.row) + (this.column) * (this.column));
            mp.ToolTip = string.Format("({0},({1:0.000},{2:0.000}),({3},{4}) ,{5})",this.Layer_Index, this.location.x, this.location.y ,
                this.row, this.column , fx);

            //this.node = mp;
            //mp.MouseDown += mp_MouseDown;
           // mp.MouseEnter += mp_MouseEnter;
            //mp.MouseLeave += mp_MouseLeave;
            ////mp.MouseMove += mp_MouseMove;
            //mp.MouseUp += mp_MouseUp;
            //mp.MouseEnter += mp_MouseMove;

            this.UiNode = mp;
            return mp;
        }

        void mp_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Cr_Path != null)
            {
                this.Cr_Path.Visibility = Visibility.Visible;
            }
        }
        private bool isMoving = false;
        void mp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.isMoving = false;
            if (this.Cr_Path != null)
            {
                this.Cr_Path.Visibility = Visibility.Hidden;
            }
        }

        void mp_MouseMove(object sender, MouseEventArgs e)
        {
            Path mp = (Path)sender;
            if(this.isMoving)
            {
                Point p = e.GetPosition(null);
                ns_point pp = new ns_point(p);
                this.location = new ns_point(p);
                //pp = pp.shift(-10, -10);
              //  MessageBox.Show(pp.ToString());
                EllipseGeometry ep = new EllipseGeometry(pp.toPoint(), this.radius, this.radius);
                EllipseGeometry cr = new EllipseGeometry(pp.toPoint(), 5*this.radius, 5*this.radius);
                mp.Data = ep;
               
                if(this.Cr_Path!=null)
                {
                    this.Cr_Path.Visibility = Visibility.Visible;
                    this.Cr_Path.Data = cr;
                }
                //SetTextLoc();
            }
        }

        void mp_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isMoving = false;
            if (this.Cr_Path != null)
            {
                this.Cr_Path.Visibility = Visibility.Hidden;
            }
        }

        void mp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton==MouseButtonState.Pressed)
            {
                this.isMoving = true;
                if (this.Cr_Path != null)
                {
                    this.Cr_Path.Visibility = Visibility.Visible;
                }
            }
        }
        public Label Text = new Label();

        private void SetTextLoc()
        {
            this.Text.Margin = new Thickness(this.location.x, this.location.y - 30 - this.radius/2, 0, 0);
        }
        public Label CreateLabel()
        {
           Label mtext = new Label();
            mtext.Content = this.id;
            mtext.Name = this.id;
            mtext.Margin = new Thickness(this.location.x, this.location.y - 30 - this.radius / 2, 0, 0);
            mtext.Foreground = Brushes.White;
           // mtext.Background = Brushes.Blue;
            this.Text = mtext;

            return mtext;
        }
        public ns_circle RenderTo(UIElementCollection ele)
        {
            Path p = Render(this.color);
            this.Cr_Path = RenderCr(this.color);
            Label text = CreateLabel();
             text.Name = this.id;
            ele.Add(p);
            ele.Add(Cr_Path);
            ele.Add(text);
           
            return this;
        }
    }

    class gui
    {
    }
}
