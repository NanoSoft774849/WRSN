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
    public  class user_node : UserControl
    {
        private Path path;
        private Grid grid;
        private Label label;
        private Canvas mc;
        private Point fx_location;
        private bool isMoving = false;
        public user_node()
        {
            this.grid = new Grid();
            this.Width = 100;
            this.Height = 100;
            this.path = new Path();
            this.label = new Label();
        }
       

        public user_node(Canvas mcan , double x, double y)
        {
            this.grid = new Grid();
            this.grid.Name = "mgrid";
            //this.Width = 10;
            //this.Height = 10;
            this.path = new Path();
            this.label = new Label();
            this.mc = mcan;
            this.SetLocation(new Point(x, y));
            
            string data = "M 0 0.88004999999998 A 14 14 0 1 1 0.00399999933334 0.88004800000014 Z ";
            this.path.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.path.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            this.path.Data = Geometry.Parse(data);
            this.path.Fill = Brushes.Green;

            this.grid.Children.Add(this.path);
            this.AddChild(grid);

          
            this.mc.Children.Add(this);
            this.init_events();
            this.fx_location = new Point(x, y);
            //this.mc.Children.Add(this);


            
        }
        private void init_events()
        {
            this.grid.MouseDown += node_MouseDown;
           this.grid.MouseMove += node_MouseMove;
          //  this.grid.MouseUp += node_MouseUp;
           //this.grid.MouseEnter += node_MouseMove;
            this.grid.MouseLeave += node_MouseLeave;
        }

        public user_node SetData(string data)
        {
            this.path.Data = Geometry.Parse(data);
            return this;
        }
     

        private bool isLeft(Point old, Point new_p)
        {
            double x = new_p.X - old.X;
            return x >= 0;
        }
        private bool isRight(Point old, Point new_p)
        {
            return (new_p.X - old.X) <= 0;
        }
      
        public Point Location
        {
            get
            {
                return this.fx_location;
            }
            set
            {
                Point p = value;

               
                value = p;
                this.Margin = new Thickness(value.X, value.Y, 0, 0);
                this.fx_location = value;

                    
            }
        }
        public void SetLocation(Point p)
        {
            //this.Margin = new Thickness(p.X, p.Y, 0, 0);
            this.Location = p;
        }
        private void node_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //this.SetLocation(e.GetPosition(this.mc));
                this.isMoving = true;
            }
        }

        private void node_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isMoving)
            {
               
                this.SetLocation(e.GetPosition(null));
               // MessageBox.Show("Move");
            }
        }

        private void node_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.isMoving = false;
          
        }

        private void node_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isMoving = false;
        }
       
       

    }
}
