   M S B A M L   `   `   `  ����  TestRand    TestRand.net_stats_wind   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Network Statistics.��5   	    � WindowState$ 	Maximized=�6%   .��5      �  � RowDefinitions 5   
   �� 5      $��40 �6   6*   �� 5      6   .+�5
   
   `� $��0��6   $��0��6&    `� 
Background �	#22111412$��2��6J   .�5      c�  c� Band$ 1��6    c� 	BandIndex$ 1��6   ���#111512.@�5      �� -   5         � ToolTip$ To Latex[packets]��5      .��5      �� $��icons\export.png��6   5      5      5      � 5   
   $��1��6   ���#abc.+�5      �� .��5      �� ���#112233��5      � 5      .+�5      &   System.Windows.Controls.DataGrid  -   5        $��	mdatagrid��5   '      AutoGenerateColumns .  ��68   �� *Nodes+   Columns 5   "   0  *System.Windows.Controls.DataGridTextColumn  5   &   ,  &System.Windows.Controls.DataGridColumn	   Header$	 ID��6P      DataGridTextCenter1  +System.Windows.Controls.DataGridBoundColumn
   ElementStyle#
 [  6\      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6�      Binding 69   �� *ID+6�     5   &   $	 Location��6V   #
 [  6h   $ * 6�    69   �� *
Location+6�     5   &   $	 	TxPackets��6X   #
 [  6k   $ * 6�    69   �� *
Tx_Packets+6�     5   &   $	 	RxPackets��6X   #
 [  6k   $ * 6�    69   �� *
Rx_Packets+6�     5   &   $	 
BatteryCap��6]   #
 [  6q   $ * 6�    69   �� *BatteryCapacity+6�     5   &   $	 LifeTime��6V   #
 [  6h   $ * 6�    69   �� *
LifeTime+6�     5   &   $
	 Type��6V   #
 [  6d   $ * 6�    69   �� *
NodeType+6�     5   &   $	 Below_Th��6T   #
 [  6f   $ * 6�    69   �� *IsDead+6�     5    &   $	 ChReq��6]   #
 [  6l   $ * 6�    69   �� *Charge_Requests+6�     5!   &   $	 	Threshold��6W   #
 [  6j   $ * 6�    69   �� *	Threshold+6�     5"   &   $
	 Port��6R   #
 [  6`   $ * 6�    69   �� *port+6�   5$      5%      5'      5(      5)      5+      5-      