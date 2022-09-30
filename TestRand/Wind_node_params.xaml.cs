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
    /// Interaction logic for Wind_node_params.xaml
    /// </summary>
    public partial class Wind_node_params : Window
    {
        private ns_node node;
        //private MainWindow _owner;
        public Wind_node_params()
        {
            InitializeComponent();
            this.node = new ns_node();
            
        }
        public Wind_node_params(ns_node node  , MainWindow wind)
        {
            InitializeComponent();
            this.node = node;
            this.Title = string.Format("Node Configuration:{0}", node.tag);
           // this.node.getUiNode().PreviewMouseRightButtonDown += Wind_node_params_PreviewMouseRightButtonDown;
            this.Owner = wind;
            Save.Background = node.getUiNode().Fill;

            var color = node.getUiNode().Fill;
            SolidColorBrush cc = color as SolidColorBrush;
            string cv = cc.Color.ToString();
            _color.Text = cv;
            this.node_name.Text = node.tag;
            SerializeBasicParams();
            this.node.GetAttrs().Foreach((a) =>
                {
                    createAttrs_ui(a.key, a.value);
                });
            this.stp.Children.Add(node.location.ToUI());
          //  _color.IsEnabled = false;
           // this.Background = node.getUiNode().Fill;
        }

       private void createAttrs_ui(string key , object value)
        {
            StackPanel xsp = new StackPanel();
            Border sp = new Border();
            sp.BorderBrush = this.node.getUiNode().Fill;
            sp.BorderThickness = new Thickness(1);
            sp.Margin = new Thickness(1);
            xsp.Orientation = Orientation.Horizontal;
            Label attr_name = new Label();
            attr_name.Background = this.node.getUiNode().Fill;
            attr_name.Content = key;
            Label _val = new Label();
            _val.Content = value;
            _val.Foreground = this.node.getUiNode().Fill;
            xsp.Children.Add(attr_name);
            xsp.Children.Add(_val);
            sp.Child = xsp;
            attrs.Children.Add(sp);
        }

       private void SetBasicParams()
       {
           if (!string.IsNullOrEmpty(radius.Text))
               this.node.AddAttribute("radius", radius.Text);
           if (!string.IsNullOrEmpty(energy.Text))
               this.node.AddAttribute("energy", energy.Text);
           if (!string.IsNullOrEmpty(node_cr.Text))
               this.node.AddAttribute("com_range", node_cr.Text);
       }
       private void SerializeBasicParams()
       {
           this.node_cr.Text = get_attr("com_range");
           this.energy.Text = get_attr("energy");
           this.radius.Text = get_attr("radius");
       }
        private string get_attr(string key)
       {
           if (this.node.HasAttribute(key)) return this.node.getAttr_value(key).ToString();
           return "";
       }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SetBasicParams();

            string attr_name = _attr_name.Text;
            string attr_value = __attr__value.Text;

            if(!string.IsNullOrEmpty(attr_name) && !string.IsNullOrEmpty(attr_value))
            {
                this.node.AddAttribute(attr_name, attr_value);
            }

            this.Owner.Dispatcher.Invoke(() =>
                {

                    string color = _color.Text;
                    string name = node_name.Text;
                    if (!string.IsNullOrEmpty(color))
                    {
                        Path p = this.node.getUiNode();
                        Color cc = (Color)ColorConverter.ConvertFromString(color);//as Color;
                        p.Fill = new SolidColorBrush(cc);
                    }
                    if(!string.IsNullOrEmpty(name))
                    {
                        Label ml = node.getNodeText();
                        if(ml!=null)
                        {
                            ml.Content = name;
                        }
                    }
                });
           

            this.Close();

        }
    }
}
