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
    public class fx_rect 
    {
        public double width;
        public double height;
        public ns_point location;
        public fx_rect()
        {
            this.width = 0;
            this.height = 0;
            this.location = new ns_point();
       
        }
        public fx_rect(double x , double y, double w, double h )
        {
            this.width = w;
            this.height = h;
            this.location = new ns_point(x, y);
        }
        public ns_point center()
        {
            return this.location.shift(this.width / 2, this.height / 2);
        }
        public ns_point tl()
        {
            return this.location;
        }
        public ns_point tr()
        {
            return this.location.shift(this.width, 0);
        }

        public ns_point bl()
        {
            return this.location.shift(0, this.height);
        }
        public ns_point br()
        {
            return this.location.shift(this.width, this.height);
        }
        public fx_rect rotate(double angdeg)
        {
            this.location = this.center().rotate(angdeg);
          //  this.location = this.location.shift(-this.width / 2, -this.height / 2);
            return this;
        }
        public Path Render()
        {
            ns_pen pen = new ns_pen();
            pen.M(this.tl()).L(this.tr()).L(this.br()).L(this.bl()).Z();
            return pen.getPath();
        }
        public Path Render(Brush color , double thickness)
        {
            ns_pen pen = new ns_pen();
            pen.SetStrokeColor(color);
            pen.thickness = thickness;
            pen.M(this.tl()).L(this.tr()).L(this.br()).L(this.bl()).Z();
            return pen.getPath();
        }
        public Path Render(double rotate_angle)
        {
            ns_pen pen = new ns_pen();
            double a = rotate_angle;
            pen.M(this.tl()).L(this.tr().rotate(a)).L(this.br()).L(this.bl().rotate(a)).Z();
            return pen.getPath();
        }
    }
    public  class fx_graph : graph.user_gui.graph_indicator
    {

        public ns_point location;
        public double width;
        public double height;
        public fx_graph( double w , double h ,double x, double y ) :base (w,h,x,y)
        {
            this.Width = w;
            this.Height = h;
            this.Margin = new Thickness(x, y, 0, 0);
        
            this.location = new ns_point(0, 0);// Path location start at the corner of the canvas.
            this.width = w;
            this.height = h;
        }
        public fx_graph SetBg(Brush color)
        {
            this.graph.Background = color;
            return this;
        }
        public fx_graph DrawPen(ns_pen pen)
        {
            this.graph.Children.Add(pen.getPath());
            return this;
        }
        public fx_graph AddPlot(Path mp)
        {
            this.graph.Children.Add(mp);
            return this;
        }
        public void Clear()
        {
            this.graph.Children.Clear();
        }
        public fx_graph plotData(string data )
        {
            Path mp = new Path();
            mp.Stroke = new ns_point(30, 480).ColorFromPoint();
            this.graph.Children.Add(mp);
            return this;
        }
       
        public fx_graph DrawXyAxis()
        {
           //ns_rect rect = new ns_rect(new ns_point(0,0), this.Width, this.Height);
           // rect.color = this.Background;
           // rect.fill = true;
           // rect.StrokeColor = Brushes.DarkGoldenrod;
           // rect.RenderTo(this.graph.Children);
            return this;
        
        }
        private void PutLabelTextX(double i , ns_point shift)
        {
            Label label = new Label();
            label.Content = string.Format("{0}", i);
            label.Margin = new Thickness(shift.x, 0, 0, 0);
            this.xaxis.Children.Add(label);
        }
        private void PutLabelTextY(double i , ns_point s)
        {
            Label label = new Label();
            label.Content = string.Format("{0}", i);
            label.Margin = new Thickness(10, s.y, 0, 0);
            this.yaxis.Children.Add(label);
        }
        public fx_graph plotAxisX(double auto_scale)
        {
            Rect cr = this.CanvasRect;
            double n = (cr.Width) / (auto_scale);
            ns_point s = this.location;
            ns_point end = new ns_point(0, 0);
            double j = 0;
            for (double i = 0; i < n; i++)
            {
                // end = new ns_point(s.x + auto_scale * (i + 1), this.height);
               
                s = s.shiftx(auto_scale);
                PutLabelTextX(j, s.shiftx(-10));
                end = new ns_point(s.x, s.y + cr.Height);
                this.graph.Children.Add(ns_line.Create(s, end, 0.133334));

              
               
                j += auto_scale;
            }
            return this;
        }
        public delegate ns_point _plotter(double x);

        public fx_graph plotFx(int N, _plotter fx)
        {
           
            return this.Plotfunction(N, Brushes.Coral, fx);
        }
        public fx_graph plotFx(int N , Brush color , _plotter fx)
        {
            return this.Plotfunction(N, color, fx);
        }
        public ns_point center()
        {
            return this.location.shift(this.Width / 2, this.Height / 2);
        }
        public fx_graph Plotfunction(int N, Brush color, _plotter fx)
        {
            string data = "";

            int len = N;
            //ns_point s = this.center().shift(-1 * this.Width / 2, 0);
            double h = this.CanvasRect.Height;
            double w = this.CanvasRect.Width;
           
          
            ns_point s = new ns_point(0,this.CanvasRect.Height);// this.center().shift(-1 * this.Width / 2, 0);
           // s= new ns_point(this.)
            for (double i = 0; i < len; i++)
            {
                ns_point p = fx(i);
                if (i == 0)
                {
                    data = (s.shift(p.x,-p.y)).moveto();//.shift(s.x, s.y).mod(2 * this.width, 2 * this.height).moveto();
                    continue;
                }
                data += (s.shift(p.x, -p.y)).mod(w, h).lineto();//.shift(s.x, s.y).mod(2 * this.width, 2 * this.height).lineto();
            }
            Path mp = new Path();
            mp.StrokeThickness = 1.633;
            mp.Stroke = color;
            mp.Data = Geometry.Parse(data);
            this.graph.Children.Add(mp);

            return this;
        }

        

        public fx_graph plotPoints(List<ns_point> points, Brush color)
        {
            string data = "";
            Canvas mc = this.graph;
            
            int len = points.Count;
            ns_point s = this.center();//.shift(-1 * this.width / 2, 0);
            data = points[0].moveto();//.shift(s.x, s.y).mod(this.width, this.height).moveto();
            // data = s.moveto();
            for (int i = 1; i < len; i++)
            {
                data += points[i].lineto();

            }
            Path mp = new Path();
            mp.StrokeThickness = 1.633;
            mp.Stroke = color;
            mp.Data = Geometry.Parse(data);
            mc.Children.Add(mp);
            return this;
        }
        public fx_graph plotAxisY(double auto_scale)
        {
            Rect cr = this.CanvasRect;
            double n = (cr.Height) / (auto_scale);
            ns_point s = this.location;
            ns_point end = new ns_point(0, 0);
            Canvas mc = this.graph;
            double j = cr.Height;
            for (double i = 0; i < n; i++)
            {
                // end = new ns_point(s.x + auto_scale * (i + 1), this.height);
                s = s.shifty(auto_scale);
                PutLabelTextY(j, s.shifty(-10));
                end = new ns_point(s.x + cr.Width, s.y);
                mc.Children.Add(ns_line.Create(s, end, 0.333334, Brushes.DarkGoldenrod));
                j -= auto_scale;
            }

            return this;

        }

    }
    public class ns_graph
    {
        // public ns_list<ns_point> points;
        public double width;
        public double height;
        public Brush backColor;

        public bool has_x_axis;
        public bool has_y_axis;
        public ns_point location;
        public Canvas render_element;
        public ns_graph(double x, double y, double w, double h)
        {
            this.location = new ns_point(x, y);
            this.height = h;
            this.width = w;
            this.backColor = Brushes.White;
            this.has_x_axis = true;
            this.has_y_axis = true;
            this.render_element = new Canvas();
        }
        public ns_graph setRenderElement(Canvas g)
        {
            this.render_element = g;
            return this;
        }

        public ns_graph PlotXyAxis(UIElementCollection p)
        {
            ns_rect rect = new ns_rect(this.location, this.width, this.height);
            rect.color = Brushes.White;
            rect.fill = true;
            rect.StrokeColor = Brushes.DarkGoldenrod;
            rect.RenderTo(p);
            return this;
        }
        public ns_graph plotAxisX(double auto_scale, UIElementCollection parent)
        {
            double n = (this.width) / (auto_scale);
            ns_point s = this.location;
            ns_point end = new ns_point(0, 0);
            for (double i = 0; i < n; i++)
            {
                // end = new ns_point(s.x + auto_scale * (i + 1), this.height);
                s = s.shiftx(auto_scale);
                end = new ns_point(s.x, s.y + this.height);
                parent.Add(ns_line.Create(s, end, 0.133334));



            }
            return this;
        }
        
        public ns_graph plotAxisY(double auto_scale, UIElementCollection parent)
        {
            double n = (this.height) / (auto_scale);
            ns_point s = this.location;
            ns_point end = new ns_point(0, 0);
            Canvas mc = this.render_element;
            for (double i = 0; i < n; i++)
            {
                // end = new ns_point(s.x + auto_scale * (i + 1), this.height);
                s = s.shifty(auto_scale);
                end = new ns_point(s.x + this.width, s.y);
                mc.Children.Add(ns_line.Create(s, end, 0.333334, Brushes.DarkGoldenrod));



            }
           
            return this;

        }
        public ns_point tl()
        {
            return this.location + new ns_point(0, this.height);
        }
        public ns_point tr()
        {
            return this.location + new ns_point(this.width, this.height);
        }
        public ns_point br()
        {
            return this.location + new ns_point(this.width, 0);
        }
        public ns_point bl()
        {
            return this.location - new ns_point(this.width, 0);
        }
        public ns_point center()
        {
            return (this.location + new ns_point(this.width / 2, this.height / 2));
        }
        public ns_point centerPoint()
        {
            return (this.location.shift(this.width / 2, this.height / 2));
        }
        public delegate ns_point _plotter(double x);

        public ns_graph plotFx(int N, _plotter fx)
        {
            if (this.render_element == null)
                return this;
            return this.Plotfunction(N, this.render_element.Children, Brushes.Coral, fx);
        }
        public ns_graph plotFx(int N, Brush color, _plotter fx)
        {
            if (this.render_element == null)
                return this;
            return this.Plotfunction(N, this.render_element.Children, color, fx);
        }
        public ns_graph Plotfunction(int N, UIElementCollection ele, Brush color, _plotter fx)
        {
            string data = "";

            int len = N;
            ns_point s = this.center().shift(-1 * this.width / 2, 0);
            // data = points[0].shift(s.x, s.y).mod(this.width, this.height).moveto();
            // data = s.moveto();
            for (double i = 0; i < len; i++)
            {
                // data += points[i].shift(s.x, s.y).mod(2 * this.width, 2 * this.height).lineto();
                //data += points[i].mod(this.width, this.height).lineto();
                if (i == 0)
                {
                    data = fx(i).shift(s.x, s.y).mod(2 * this.width, 2 * this.height).moveto();
                    continue;
                }
                data += fx(i).shift(s.x, s.y).mod(2 * this.width, 2 * this.height).lineto();
            }
            Path mp = new Path();
            mp.StrokeThickness = 1.633;
            mp.Stroke = color;
            mp.Data = Geometry.Parse(data);
            ele.Add(mp);

            return this;
        }

        public ns_graph plotPoints(List<ns_point> points, UIElementCollection ele, Brush color)
        {
            string data = "";
            Canvas mc = this.render_element;
            mc.Width = this.width;
            mc.Height = this.height;
            mc.Margin = new Thickness(this.location.x, this.location.y, 0, 0);
            mc.Background = Brushes.Black;
            int len = points.Count;
            ns_point s = this.center();//.shift(-1 * this.width / 2, 0);
            data = points[0].moveto();//.shift(s.x, s.y).mod(this.width, this.height).moveto();
            // data = s.moveto();
            for (int i = 1; i < len; i++)
            {
                data += points[i].lineto();
               
            }
            Path mp = new Path();
            mp.StrokeThickness = 1.633;
            mp.Stroke = color;
            mp.Data = Geometry.Parse(data);
            mc.Children.Add(mp);
            ele.Add(mc);

            return this;
        }

    }
    public class examples
    {
        public delegate void _printf(string fmt, params object[] objs);
        public static void Partx(_printf printf)
        {
            double[] ar = { 1, 2, 3, 4, 5, -1, 3, -4, 1, 2, 3, 5, -6, 7, 8, 9, 11, 20, 13, 24, 45, 16, 27, 18 };

            var dict = data_part.Partition(ar, (v, d) =>
            {
                return new boolKey(v % 2 == 0, "even");
            }, (v2, d) =>
            {
                return new boolKey(v2 % 2 != 0, "odd");
            }, (v3, d) =>
            {
                return new boolKey(v3 < 0, "minus");
            }, (v, d) =>
            {
                //return new boolKey(HowManyDigits(v) >= 2, "2_digits");
                return new boolKey(v > 4, "gthan3");
            });
            foreach (string key in dict.Keys)
            {
                printf("value of {0}", key);
                string s = "";
                foreach (var d in dict[key])
                {
                    s += string.Format("{0},", d);
                }
                printf(s);
                printf("---------------------");
            }
        }
        public static double PI
        {
            get
            {
                return Math.PI;
            }
        }
        public static double PI2
        {
            get
            {
                return 2 * Math.PI;
            }
        }
        public static ns_node[,] CreateGridNetget(int rows, int cols, UIElementCollection p)
        {
            int i = 0;
            int j = 0;
            int sum = 0;
            ns_node prev = new ns_node();
            ns_node[,] nodes = new ns_node[rows, cols];
            double scale = 70;

            for (i = 0; i < rows; i++)
            {
                //sum += i;
                for (j = 0; j < cols; j++)
                {
                    string tag = string.Format("n{0}", sum);
                    double x = scale + (scale * i);
                    double y = scale + (scale * j);
                    //ns_point location = new ns_point(x, y);
                    ns_node node = new ns_node(tag, x, y);
                    nodes[i, j] = node;

                    sum++;

                }

            }
            //#---------------
            for (i = 0; i < rows; i++)
            {
                for (j = 0; j < cols; j++)
                {
                    ns_node node = nodes[i, j];


                    Random r = new Random((int)DateTime.Now.Ticks);
                    if (i < rows - 1)
                    {
                        ns_node n1 = nodes[i + 1, j];
                        node.addLink(n1, r.Next(10));
                    }
                    if (i > 0)
                    {
                        ns_node n2 = nodes[i - 1, j];
                        node.addLink(n2, r.Next(20));
                    }
                    if (j < cols - 1)
                    {
                        ns_node n3 = nodes[i, j + 1];
                        node.addLink(n3, r.Next(30));
                    }
                    if (j > 0)
                    {
                        ns_node n4 = nodes[i, j - 1];
                        node.addLink(n4, r.Next(30));
                    }



                    node.RenderTo(p);

                }

            }
            return nodes;
        }
        public static void CreateGridNet(int rows , int cols , UIElementCollection p)
        {
            int i = 0;
            int j = 0;
            int sum = 0;
            ns_node prev = new ns_node();
            ns_node[,] nodes = new ns_node[rows,cols];
            double scale = 70;

            for (i = 0; i < rows; i++)
            {
                //sum += i;
                for (j = 0; j < cols; j++)
                {
                    string tag = string.Format("n{0}", sum);
                    double x = scale + (scale * i);
                    double y = scale + (scale * j);
                    //ns_point location = new ns_point(x, y);
                    ns_node node = new ns_node(tag, x, y);
                    nodes[i, j] = node;

                    sum++;

                }

            }
            //#---------------
            for (i = 0; i < rows; i++) 
            {
                for (j = 0; j < cols; j++)
                {
                    ns_node node = nodes[i, j];
                    
                    
                        Random r = new Random((int)DateTime.Now.Ticks);
                        if (i <rows-1)
                        {
                            ns_node n1 = nodes[i + 1, j];
                            node.addLink(n1, r.Next(10));
                        }
                        if (i > 0)
                        {
                            ns_node n2 = nodes[i - 1, j];
                            node.addLink(n2, r.Next(20));
                        }
                        if (j < cols - 1)
                        {
                            ns_node n3 = nodes[i, j + 1];
                            node.addLink(n3, r.Next(30));
                        }
                        if (j > 0)
                        {
                            ns_node n4 = nodes[i, j - 1];
                            node.addLink(n4, r.Next(30));
                        }


                    
                    node.RenderTo(p);

                }

            }

            ns_pqueue<spot> path = AStar.AStar_algorithm(nodes[0, 0], nodes[7, 7], 1, (s) =>
                    {
                        //logmsg(s);
                    });
            ns_pqueue<spot> path2 = AStar.AStar_algorithm2(nodes[0, 0], nodes[7, 7]);
            
            int c = path.Count;
            int c2 = path2.Count;
            MessageBox.Show("Count1:" + c.ToString() + ", Count2:" + c2.ToString());
           
            ns_pen pen = new ns_pen();
            pen.SetStrokeColor(Brushes.Red);
            pen.thickness = 3;
            for (i = 0; i < c; i++)
            {
                ns_point node = path.ns_pull_pmax().value.node.location;
                if (i == 0)
                {
                    pen.moveto(node);
                    continue;
                }
                pen.L(node);
            }
            p.Add(pen.getPath());
           //--------------
            pen = new ns_pen();
            pen.SetStrokeColor(Brushes.Green);
            pen.thickness = 3;
            for (i = 0; i < c2; i++)
            {
                ns_point node = path2.ns_pull_pmin().value.node.location;
                if (i == 0)
                {
                    pen.moveto(node);
                    continue;
                }
                pen.L(node);
            }
            p.Add(pen.getPath());
        }
        /// <summary>
        /// fm the message frequency
        /// fc the carrier frequncy
        /// Am the message amlitude
        /// Ac the carrier amplitude
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public static fx_graph example_am_modulation(double fm, double fc, double Am, double Ac)
        {
            double m = Am / Ac;
            fx_graph fx = new fx_graph(500, 300, 100, 100);
            fx.SetBg(Brushes.Black);
            fx.DrawXyAxis().plotAxisX(50).plotAxisY(30);
            double fsm = 1 * fm;
            double fsc = 1 * fc;
            fx.plotFx(400, Brushes.Magenta, (d) =>
            {
                double mt = Math.Cos((PI) + (PI2 * d * fm / (150 * fsm)));
                double y = (1 + m * mt) * Ac * Math.Sin(Math.PI + (2 * Math.PI * d * fc / (1600)));//10.1Hz
                return new ns_point(d, y).shift(0, 150);
            });
            return fx;
        }
        public static  fx_graph example1()
        {
            fx_graph fx = new fx_graph(100, 100, 300, 300);
            fx.SetBg(Brushes.Black);
            fx.DrawXyAxis().plotAxisX(10).plotAxisY(10);
            fx.plotFx(300, Brushes.Magenta, (d) =>
            {
                double y = 50 * Math.Sin(Math.PI + (2 * Math.PI * d * 10.1 / 600));//10.1Hz
                return new ns_point(d, y).shift(0, 150);
            }).plotFx(300, Brushes.Green, (d) =>
            {
                double y = 50 * Math.Cos(Math.PI + (2 * Math.PI * d * 10.1 / 600));//10.1Hz
                return new ns_point(d, y).shift(0, 150);
            }).plotFx(300, (d) =>
            {


                double y = 100 * Math.Cos(Math.PI + (2 * Math.PI * d * 10.1 / 600));//10.1Hz

                double y2 = (50 + y) * Math.Sin((Math.PI) + (2 * Math.PI * d * 100.1 / 6000));//10.1Hz
                return new ns_point(d, y2).shift(0, 150);
            });
            return fx;
        }
    }
}
