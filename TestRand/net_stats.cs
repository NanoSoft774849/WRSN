using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ns.graph;
namespace TestRand
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<ns_nodeInfo> xNodesInfo;
        //private ObservableCollection<DeviceInfo> xNodeDetails;

        public ObservableCollection<ns_nodeInfo> Nodes
        {
            get { return xNodesInfo; }
            set
            {
                xNodesInfo = value;
                RaisePropertyChanged("NodesAdded");
            }
        }

    }
        public class ViewModelBase : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public void RaisePropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class ns_nodeInfo
    {
        public string ID { get; set; }
        public int port { get; set; }
        public int Tx_Packets { get; set; }
        public int Rx_Packets { get; set; }
        public double BatteryCapacity { get; set; }
        public string LifeTime { get; set; }
        public bool IsDead { get; set; }
        public string NodeType { get; set; }
        public string Location { get; set; }
        public double Comm_Range { get; set; }
        public double Threshold { get; set; }
        public double Charge_Requests { get; set; }

        private ns_node thenode;
        public ns_nodeInfo(ns_node node)
        {
            this.ID = node.tag;
            this.port = node.getServerPort();
            this.Rx_Packets = node.getRxPackets();
            this.Tx_Packets = node.getTxPackets();
            this.BatteryCapacity = node.PowerConfig.BatteryCapacity;
            this.LifeTime = node.getLifeTimeString();
          
            this.Location = string.Format("({0},{1})", node.location.x, node.location.y);
            this.Threshold = node.PowerConfig.energy_threshold;
            this.Charge_Requests = node.PowerConfig.charge_request_packets_tx;
            if(node.NodeType == Node_TypeEnum.Mobile_Charger)
            {
                this.Charge_Requests = node.PowerConfig.charge_request_packets_rx;
            }
            this.IsDead = this.BatteryCapacity <= this.Threshold;
           // this.Ch = node.PowerConfig.charge_request_packets_tx;

            set_node_type(node);
            this.thenode = node;
        }
        public ns_node getTheNode()
        {
            return this.thenode;
        }
        private void set_node_type(ns_node node)
        {
            if (node.NodeType == Node_TypeEnum.BaseStation)
            {
                this.NodeType = "BS";
                return;
            }
            if (node.NodeType == Node_TypeEnum.Sensor_node)
            {
                this.NodeType = "SN";
                return;
            }
            if (node.NodeType == Node_TypeEnum.Mobile_Charger)
            {
                this.NodeType = "MC";
                return;
            }
            this.NodeType = "undefined";
        }
    }

