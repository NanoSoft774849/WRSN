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
    /// Interaction logic for Wind_Node_Config.xaml
    /// </summary>
    public partial class Wind_Node_Config : Window
    {
        private ns_node node;
        private StackPanel panel;
        public Wind_Node_Config()
        {
            InitializeComponent();
            this.node = new ns_node();
        }
        public Wind_Node_Config(ns_node node , MainWindow main_wind)
        {
            InitializeComponent();

            this.node = node;
            this.Owner = main_wind;
            var loc = node.location.ToUI();
            this.Title = string.Format("Node Configuration:{0}", node.tag);
            this.panel = node.PowerConfig.getUiPanel();
            Border bb = new Border();
            bb.Margin = new Thickness(5);
            bb.BorderBrush = node.location.ColorFromPoint();
            ScrollViewer sv = new ScrollViewer();
           
            
            panel.Name = node.tag;
           
            this.panel.Children.Add(loc);
            this.fx_content.Children.Clear();// clear first
            //bb.Child = panel;
            sv.Content = this.panel;
            bb.Child = sv;
            this.fx_content.Children.Add(bb);
            
            this.SaveForm.Background = node.location.ColorFromPoint();
            
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.fx_content.Children.Clear();
        }

        private void SaveForm_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.fx_content.Children.Remove(this.panel);
        }
    }
}
