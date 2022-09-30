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

namespace TestRand
{
    /// <summary>
    /// Interaction logic for fx_node.xaml
    /// </summary>
    public partial class fx_node : UserControl
    {
        private Canvas mc;
        private Point fx_location;
        public fx_node()
        {
            InitializeComponent();
            this.fx_location = new Point(0, 0);
        }

        public fx_node(Canvas mcan , double x, double y)
        {
            InitializeComponent();
            this.mc = mcan;
            this.SetLocation(new Point(x, y));
            this.mc.Children.Add(this);
        }
        public fx_node(Canvas mcan , double x, double y , Path pat, Label txt)
        {

            InitializeComponent();
            this.mc = mcan;
            this.SetLocation(new Point(x, y));
            this.node_name = txt;
            this.path = pat;
            this.mc.Children.Add(this);
        }
        public bool isMoving = false;

        public fx_node SetData(string data)
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
                p.X = p.X - 10;
                p.Y = p.Y - 10;
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
                this.SetLocation(e.GetPosition(this.mc));
                this.isMoving = true;
            }
        }

        private void node_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
               
                this.SetLocation(e.GetPosition(this.mc));
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
