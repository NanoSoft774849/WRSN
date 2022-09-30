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
using ns.graph;
namespace TestRand
{
    /// <summary>
    /// Interaction logic for node_params.xaml
    /// </summary>
    /// 
    
    public partial class node_params : UserControl
    {
        private ns_node node;
        public node_params()
        {
            InitializeComponent();
            this.node = new ns_node();
        }
        public node_params(ns_node node)
        {
            InitializeComponent();
            this.node = node;
            this.node.getUiNode().PreviewMouseRightButtonDown += node_params_PreviewMouseRightButtonDown;
        }

        void node_params_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.node.AddAttribute("radius", radius.Text);
            this.node.AddAttribute("energy", energy.Text);

        }
    }
}
