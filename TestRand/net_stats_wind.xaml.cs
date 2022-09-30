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
using System.Collections.ObjectModel;
using ns.graph;
using System.IO;
namespace TestRand
{
    /// <summary>
    /// Interaction logic for net_stats_wind.xaml
    /// </summary>
    public partial class net_stats_wind : Window
    {
        private List<ns_nodeInfo> fxNodes_List;
        private MainViewModel mViewModel = new MainViewModel();
        private MainWindow _main;
        private Dictionary<string, ns_node> MyNodes;
        public net_stats_wind()
        {
            InitializeComponent();

            this.fxNodes_List = new List<ns_nodeInfo>();
            this.mViewModel.Nodes = new ObservableCollection<ns_nodeInfo>();
            this.DataContext = mViewModel;
        }
        public net_stats_wind(MainWindow win,Dictionary<string,ns_node> nodes)
        {
            InitializeComponent();

            this.fxNodes_List = new List<ns_nodeInfo>();
            this.mViewModel.Nodes = new ObservableCollection<ns_nodeInfo>();
            this.DataContext = mViewModel;

            this._main = win;
            this.MyNodes = nodes;
            RenderInfo(nodes);
        }
        private void export_packets_as_latex(object sender , EventArgs args)
        {
            string fn = "net_packets.dat";
            string fwd_packets = "net_fwd_packets.dat";
            string charge_rq_packets = "net_ch_packets.dat";
            RenderInfo(this.MyNodes);
            
            if(this.mViewModel.Nodes.Count>0)
            {
                File.WriteAllText(fn, "");// clear 
                File.WriteAllText(fwd_packets, "");// clear 
                File.WriteAllText(charge_rq_packets, "");// clear 
                string fmt = "x\ty\t\n";
                string fwd_fmt = fmt;
                string ch_fmt = fmt;
               
                int i = 1;
                foreach (var item in this.mViewModel.Nodes)
                {
                    if (item.NodeType == "SN")
                    {
                        double packets = item.Tx_Packets + item.Rx_Packets;
                        ns_point p = new ns_point(i, packets);
                        ns_node thenode = item.getTheNode();
                        fmt += p.toLatexDataFilePoint();
                        p = new ns_point(i, thenode.PowerConfig.charge_request_packets_tx);
                        ch_fmt += p.toLatexDataFilePoint();
                        
                        p = new ns_point(i, thenode.PowerConfig.forward_packets);
                        fwd_fmt += p.toLatexDataFilePoint();
                        
                        i++;
                    }
                }
                File.WriteAllText(fn, fmt);
                File.WriteAllText(fwd_packets, fwd_fmt);
                File.WriteAllText(charge_rq_packets, ch_fmt);
            }

        }
        private void RenderInfo(Dictionary<string, ns_node> nodes)
        {
            this.mViewModel.Nodes.Clear();
            foreach(var item in nodes)
            {
                ns_node node = item.Value;
                ns_nodeInfo info = new ns_nodeInfo(node);
                this.mViewModel.Nodes.Add(info);
            }
        }
        public void ShowWind()
        {
            this.ShowDialog();
            this.Owner = this._main;
        }
        protected override void OnClosed(EventArgs e)
        {
            this.fxNodes_List.Clear();
            this.mViewModel.Nodes.Clear();
            base.OnClosed(e);
        }
    }
}
