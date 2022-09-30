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
using System.Windows.Shapes;
using ns.graph;
namespace TestRand
{
    /// <summary>
    /// Interaction logic for Wind_net_Stat.xaml
    /// </summary>
    public partial class Wind_net_Stat : Window
    {
        
        public Wind_net_Stat()
        {
            InitializeComponent();
        }
        public Wind_net_Stat(MainWindow wind)
        {
            InitializeComponent();
            
           // PlotFigures();
           
        }
        public void ShowWin(MainWindow wind)
        {
            if(this.IsVisible || this.IsActive)
            {
                this.Close();
            }
            
            this.ShowDialog();
            this.Owner = wind;
            this.figures.Children.Clear();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.figures.Children.Clear();
        }
        public void PlotFigures()
        {
            fx_graph gf = new fx_graph(400, 200, 100, 100);
            gf.plotAxisX(40);
            gf.plotAxisY(20);
            gf.plotFx(300, (x) =>
                {
                    return new ns_point(x, 50*Math.Sin(10.1*x * 2 * Math.PI / 600)).shifty(100);
                });
            this.figures.Children.Add(gf);
        }
        public Canvas getCanvas()
        {
            return this.figures;
        }
    }
}
