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
    /// Interaction logic for graph_indicator.xaml
    /// </summary>
    public partial class graph_indicator : UserControl
    {
        private bool isMoving = false;
        private Point _location;
        private double canvas_size_per = 0.80;
        private double x_axis_size_per = 0.10;
        private double y_axis_size_per = 0.10;
        private Rect _canvas_Rect;
        private ScaleTransform st;
        public graph_indicator()
        {
            InitializeComponent();
            this.Height = 300;
            this.Width = 300;
            _location = new Point(0, 0);
            this._canvas_Rect = new Rect();
            this.st = new ScaleTransform();
        }
        public graph_indicator(double w, double h, double x, double y)
        {
            InitializeComponent();
            this.Width = w;
            this.Height = h;
            this.Margin = new Thickness(x, y, 0, 0);
            this._location = new Point(x, y);
            this._canvas_Rect = new Rect(x, y, w * canvas_size_per, h * canvas_size_per);
            this.st = new ScaleTransform();
           // this.graph.RenderTransform = this.st;
            this.RenderTransform = st;
        }
        public Rect CanvasRect
        {
            get
            {
                return this._canvas_Rect;
            }
            set
            {
                this._canvas_Rect = value;
            }
        }

        public Point Location
        {
            get
            {
                return this._location;
            }
            set
            {
                this._location = new ns_point(value).shift(-10, -10).toPoint();
                value = this._location;
                this.Margin = new Thickness(value.X, value.Y, 0, 0);
            }
        }
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isMoving = true;
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isMoving)
            {
                Point p = e.GetPosition(null);
                this.Location = p;
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isMoving = false;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.isMoving = false;
        }

        private void graph_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                this.st.ScaleX *= 1.2;
                this.st.ScaleY *= 1.2;
            }
            else
            {
                this.st.ScaleX /= 1.2;
                this.st.ScaleY /= 1.2;
            }
        }
    }
}
