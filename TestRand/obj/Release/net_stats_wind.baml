   M S B A M L   `   `   `  ����   TestRand    TestRand.net_stats_wind   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Network Statistics.�� � WindowState$ 	Maximized=�.���  � RowDefinitions �� $��40 ��� .+�`� $��0��$��0�� `� 
Background �	#22111412$��2��.�c�  c� Band$ 1�� c� 	BandIndex$ 1�����#111512.@��� -    � ToolTip$ To Latex[packets]��.���� $��icons\export.png��� $��1�����#abc.+��� .���� ���#112233��� .+�&   System.Windows.Controls.DataGrid  -   $��	mdatagrid��   AutoGenerateColumns .  ���� *Nodes+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn	   Header$	 ID��   DataGridTextCenter1  +System.Windows.Controls.DataGridBoundColumn
   ElementStyle#
 [     Width5  /System.Windows.Controls.DataGridLengthConverter$ *    Binding �� *ID+  $	 Location��#
 [  $ *  �� *
Location+  $	 	TxPackets��#
 [  $ *  �� *
Tx_Packets+  $	 	RxPackets��#
 [  $ *  �� *
Rx_Packets+  $	 
BatteryCap��#
 [  $ *  �� *BatteryCapacity+  $	 LifeTime��#
 [  $ *  �� *
LifeTime+  $
	 Type��#
 [  $ *  �� *
NodeType+  $	 Below_Th��#
 [  $ *  �� *IsDead+  $	 ChReq��#
 [  $ *  �� *Charge_Requests+  $	 	Threshold��#
 [  $ *  �� *	Threshold+  $
	 Port��#
 [  $ *  �� *port+