   M S B A M L   `   `   `  ????a  ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  ?? MergedDictionaries ?? Source&    System.Windows.Controls.DataGrid ?? 
TargetType ?? Property ?? Value ?? 
TargetType ?? Padding J? SnapsToDevicePixels ??Name	 ? ColumnDefinitions
 ? RowDefinitions    SelectAllCommand ?? 
ResourceId ?? TypeInTargetAssembly7  1System.Windows.Controls.DataGridHeadersVisibility
   All ?? ConverterParameter!    HeadersVisibilityConverter ?? 	Converter ?? RelativeSource ?? AncestorTypeG  ASystem.Windows.Controls.Primitives.DataGridColumnHeadersPresenter  Name   Column ??Name ?? CanContentScroll ??Name ?? ScrollableHeight ? Maximum ?? Orientation* ?? #ComputedVerticalScrollBarVisibility ?? ViewportHeight ?? ViewportSize ? Value  ?? Mode! ?? ScrollableWidth," ?? %ComputedHorizontalScrollBarVisibility# ?? ViewportWidth$ ?? Triggers% ]? 
Conditions& ?? Property' ?? Value   ColumnHeaderGripperStyle=  7System.Windows.Controls.Primitives.DataGridColumnHeader3   -Microsoft.Windows.Themes.DataGridHeaderBorder(   CanUserSort)   IsClickable*   	IsPressed+ J? IsMouseOver,   	IsHovered-   SortDirection.   SortDirection/   SeparatorBrush0   SeparatorBrush1   SeparatorVisibility2   SeparatorVisibility3 n?Name)  #System.Windows.Controls.DataGridRow4 ??Name?  9System.Windows.Controls.Primitives.SelectiveScrollingGrid?  9System.Windows.Controls.Primitives.DataGridCellsPresenter5   
ItemsPanelA  ;System.Windows.Controls.Primitives.DataGridDetailsPresenter6   DetailsVisibility$7   SelectiveScrollingOrientation;	  5System.Windows.Controls.SelectiveScrollingOrientation8 	  Vertical#9    RowDetailsScrollingConverter:
  4System.Windows.Controls.Primitives.DataGridRowHeader
:   Row; P? Property< P? Value*  $System.Windows.Controls.DataGridCell  DataGridTextCenter?? 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      UThemesJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5        5      ?? 5   
   $G Apack://application:,,,/ClientsApp;component/Styles/BaseStyle.xaml??6   6j   %  '         &	  ?	    '      7   7   '      '  ?    &	 ?    ?? 5	      "   6   .?5
   
   ??  ? ?? ?#fff6'   ?? 5   
    ? ?? ?#3336'   ?? 5   
    ? ?? ?#ddd6(   ?? 5   
    ? ??$ 0q?6,   ?? 5   
    ?@  	RowHeight$ 38Q?6&   ?? 5   
    ?@  ColumnHeaderHeight$ 38Q?6/   ?? 5   
    ?@  GridLinesVisibility$ 
Horizontal=?60   ?? 5   
     ?@  HorizontalGridLinesBrush ?#ddd65   ?? 5   
    ?@  HeadersVisibility$ Column=?6.   ?? 5   
    ?@??AlternationCount$ 2??6-   ?? 5   
     ?@  AlternatingRowBackground ?#f9f9f965   ?? 5   
     ?@  RowDetailsVisibilityMode$ VisibleWhenSelected=?65   ?? 5   
    ? ?? . 6:   ?? 5   
    ?@??PanningMode$
 Both=?65   ?? 5   
    ?@??IsFlicksEnabled .  63   ?? 5   
    ? ?? 5      ?? 5      "   6"   .R?5      ?? #??z??6   #??z??6I   #??z??6}   # z??6?    . ._?5      ?? $ DG_ScrollViewer??6'   |?.  ??5      ?? 5   "   " ??62   .R?5    &   ? 	 5!   *   ?? 5"   .   $
??Auto ?6?   ?? 5#   .   $??* ?6?   ?? 5$   .   $
??Auto ?6?   
 5&   *   ?? 5'   .   $
??Auto ?6<   ?? 5(   .   $??* ?6<   ?? 5)   .   $
??Auto ?6<   .+?5+   *   ?? #??Z 61   |?.  ??6r   C? *?? $" DataGridSelectAllButtonStyle??"   +y?6?   ?? *HeadersVisibility+# Z # Z  ?? "   ??6?  ?? *CellsPanelHorizontalOffset+ ?? "     5,   *   $! PART_ColumnHeadersPresenter??6I   $??1??6n   y?6~   ?? *HeadersVisibility+# Z # Z  ?? "   ?? 5-   *   $! PART_ScrollContentPresenter??6A   # z??6f   $??2??6?   $??1??6?   ?? 5.   *   $ PART_VerticalScrollBar??64   $??2??6T   # z 6d   $ Vertical=?6?   $??1??6?   #y?z 6?   # z 6W   6?   ?? *VerticalOffset+$  OneWay=? ?? *TemplatedParent+6W  ? 5/   *   $??1??6/   $??2??6?   	 50   .   ?? 51   2   ??6C   ?? **(NonFrozenColumnsViewportHorizontalOffset+ ?? "   ?? 52   2   $??* ?6C   .+?54   .   ?? $ PART_HorizontalScrollBar??68   $??1??6Z   # z! 6j   $ 
Horizontal=?6?   #y?z" 6?   # z# 6U   6?   ?? *HorizontalOffset+$  OneWay=? ?? *TemplatedParent+6U  55   +   56   '   57   #   .??59      ?? # z 6-   5:      5;      5<      5>      $ 5?   
   ]? 5@      % 5A      ?? 5B      & ?@??
IsGrouping' . 66   ?? 5C      "& ?@%?IsVirtualizingWhenGrouping' .  6X   ./?5E      ??  ? ?? .  6B   5F      5H      ?? 5I      " n?6-   .?5J   
   ??  ? ??$ 8??6"   ?? 5K   
    ? ??
 ???? 6'   ?? 5L   
    ?@?Cursor$ SizeWE??6#   ?? 5M   
    ? ?? 5N      ?? 5O      " n?6"   .R?5P      ?? #??z??6   # z??6G   5Q      5S      5T      ?? 5U      "  6   .?5V   
   ??  ? ?? ?#fff6'   ?? 5W   
    ? ?? ?#0006'   ?? 5X   
    ? ??$
 Bold"?6'   ?? 5Y   
    ? ?? ?#ddd6(   ?? 5Z   
    ? ??$ 0,0,0,2q?6,   ?? 5[   
    ? ??$	 5,0q?6$   ?? 5\   
    ? ??$ Center=?65   ?? 5]   
    ? ?? 5^      ?? 5_      "  6"   .R?5`      ? .+?5a        #??z??66   #??z??6b   #??z??6?   #) z( 6?   #* z??6?   #, z+ 6  # z??6>  #. z- 6b  #0 z/ 6?  #2 z1 6?  ._?5b      ?? #??z??6/   ??. # z 6?   #??z??6?   5c      n? 5d      $3 PART_LeftHeaderGripper??6    $
??Left=?6@   8??  6[   n? 5e      $3 PART_RightHeaderGripper??6    $??Right=?6A   8?? 6]   5f      5g      5i      5j      ?? 5k      "  6   .?5l   
   ??  ? ??# ? ??6'   ?? 5m   
    ?@J?SnapsToDevicePixels . 60   ?? 5n   
    ?@=?ErrorTemplate 65   Z? ?? 5o   
    ?@ ValidationErrorTemplate 5p      ?? 5q      .R?5r      ?? 
???  ??$??2,0,0,0q?61   $??!??6B   $??Center=?6K   5s      5u      ?? 5v   
    ? ?? 5w      ?? 5x      "  6"   .R?5y      ?? $4 
DGR_Border??6   #??z??61   #??z??6]   #??z??6?    . ._?5z        	 5{      ?? 5|   "   $
??Auto ?63   ?? 5}   "   $??* ?63   
 5      ?? 5?   "   $??* ?60   ?? 5?   "   $
??Auto ?60   .+?5?        $??1??65   #??z5 6E   # z 6o     5?      $??1??67   $??1??6G   #y?z6 6k  7 6T   ?? *AreRowDetailsFrozen+# Z8 # Z9  ?? "   6k  
  5?      $??2??60   $7 Vertical=?6A   y?6?   ?? *HeadersVisibility+# Z: # Z  ?? "   5?      5?      5?      5?      $ 5?   
   P? 5?      ; ?@ 	IsNewItem< . . ?5?      ??  ? ?? 6+   ?? *NewItemMargin+ ?? "   5?      P? 5?      ; ?@??AlternationIndex$< 0??6?   . ?5?      ??  ? ?? 6/   ?? *AlternatingRowBackground+ ?? "   5?      P? 5?      ; ?@J?IsMouseOver< . . ?5?      ??  ? ?? ?#f5f5f56/   5?      5?      ?? 5?      "  6   .?5?   
   ??  ? ??
 ???? 6'   ?? 5?   
    ? ??
 ???? 6(   ?? 5?   
    ? ??$ 0q?6,   ?? 5?   
    ? ?? 5?      ?? 5?      "  6"   .R?5?      ?? #??z??6   #??z??6I   #??z??6}    . ._?5?      ?? $	??5,0q?6+   # z 68   5?      5?      5?      $ 5?   
   P? 5?      ; ?@ 
IsSelected< . . ?5?      ??  ? ?? ?#90F6706/   ?? 5?       ? ?? ?#3336/   5?      P? 5?      ; ?@J?IsKeyboardFocusWithin< . . ?5?      ??  ? ?? ?#0006/   5?      ]? 5?      % 5?      ?? 5?      & ?@ 
IsSelected' . 66   ?? 5?      & ?@??IsSelectionActive' .  6F   ./?5?      ??  ? ?? ?#fcf8e36/   5?      P? 5?      ; ? {?< .  . ?5?      ??  ? ?? ?#eee6/   5?      5?      ?? 5?      " ??6'   .?5?   
   ??  ? ??$ Center=?6.   5?      5?      