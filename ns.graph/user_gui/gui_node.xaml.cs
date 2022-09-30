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

namespace ns.graph.user_gui
{
    /// <summary>
    /// Interaction logic for gui_node.xaml
    /// </summary>
    public partial class gui_node : UserControl
    {
        private Point _location;
        private double _CommRange;
        private double _node_radius;
        public string id;
        public ns_point location;
        public double radius;
        public Brush color;
        public ns_attributes attrs;
        public gui_node()
        {
            InitializeComponent();
            this._location = new Point(10, 10);
            this._node_radius = 10;
            this._CommRange = 50;
            this.node.Stroke = new ns_point(10, 10).ColorFromPoint();
            this.Margin = new Thickness(10, 10, 0, 0);
        }
        public gui_node (string name ,  double x, double y)
        {
            InitializeComponent();
            this.location = new ns_point(x, y);
            this._node_radius = 10;
            this._CommRange = 50;
            this.node.Stroke = new ns_point(x, y).ColorFromPoint();
            this.node_name.Content = name;
            //this.node_name.Margin = new Thickness(-10, -10, 0, 0);
            this.Margin = new Thickness(x-15, y-15, 0, 0);
        }
        public Point Location
        {
            get
            {
                return this.location.toPoint();
            }
            set
            {
                ns_point p = new ns_point(value).shift(-10, -10);
                this.location = p;//.toPoint();
                this.Margin = new Thickness(p.x, p.y, 0, 0);
            }
        }

        private bool isMoving = false;
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                isMoving = true;
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.isMoving = false;
            this.node_CR.Visibility = System.Windows.Visibility.Hidden;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if(isMoving)
            {
                Point p = e.GetPosition(null);
                this.Location = p;
                this.node_CR.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isMoving = false;
            this.node_CR.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
