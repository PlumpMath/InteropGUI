   M S B A M L   `   `   `  ���� E  AInteropGUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:InteropGUI
InteropGUI      InteropGUI.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     *localclr-namespace:InteropGUI       � Title$  
MainWindow��$	��600��$
��1000�� � 
ResizeMode$ CanMinimize=�.��� -    �Name$
 grid��.+��� -    ��Name$ image��$
��Left=�$	��571��$	��Top=�$	��784���� -    ��Name$ map_surface��$
��Left=�$	��569��$	��Top=�$	��784������  �� Color$ White�� �� Opacity$ 0Q��� ���	#FFF4F4F5$
��Left=�$	��569��$��	784,0,0,0q�
���   �$	��Top=�$	��210���� -    ��Name$ location_label��$��	Location:��$
��Left=�$��	784,0,0,0q�$	��Top=�$��25��$��96��$	��5,1q�$��Center=��� -   $ airspeed_label��$��	Airspeed:��$
��Left=�$��784,100,0,0q�$	��Top=�$��25��$��96��$	��5,1q�$��Center=��� -   $ altitude_label��$��	Altitude:��$
��Left=�$��
784,75,0,0q�$	��Top=�$��25��$��96��$	��5,1q�$��Center=��� -   $ longitude_label��$��
Longitude:��$
��Left=�$��
784,50,0,0q�$	��Top=�$��25��$��96��$	��5,1q�$��Center=��� -   $ latitude_label��$��	Latitude:��$
��Left=�$��
784,25,0,0q�$	��Top=�$��25��$��96��$	��5,1q�$��Center=��� -	    ��Name$ latitude_textbox��$
��Left=�$��25��$��
880,25,0,0q�	 �� TextWrapping$
	 Wrap=�$��TextBox��$	��Top=�$	��113��
 �� 
IsReadOnly
 . $	��5,1q�$��Center=��� -
   $ longitude_textbox��$
��Left=�$��25��$��
880,50,0,0q�$
	 Wrap=�$��TextBox��$	��Top=�$	��113��
 . $	��5,1q�$��Center=��� -   $ altitude_textbox��$
��Left=�$��25��$��
880,75,0,0q�$
	 Wrap=�$��TextBox��$	��Top=�$	��113��
 . $	��5,1q�$��Center=��� -   $ airspeed_textbox��$
��Left=�$��25��$��880,100,0,0q�$
	 Wrap=�$��TextBox��$	��Top=�$	��113��
 . $	��5,1q�$��Center=��� -   $ status_label��$��Status:��$
��Left=�$��784,125,0,0q�$	��Top=�$��25��$��96��$	��5,1q�$��Center=��� -   $ status_textbox��$
��Left=�$��25��$��880,125,0,0q�$
	 Wrap=�$	��Off��$	��Top=�$	��113��
 . $	��5,1q�$��Center=��� -   $ 
mode_label��$��Mode:��$
��Left=�$��784,150,0,0q�$	��Top=�$��25��$��96��$	��5,1q�$��Center=��� -   $ mode_textbox��$
��Left=�$��25��$��880,150,0,0q�$
	 Wrap=�$��Unknown��$	��Top=�$	��113��
 . $	��5,1q�$��Center=�