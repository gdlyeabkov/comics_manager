   M S B A M L   `   `   `  ���� H  DComicsManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:ComicsManagerComicsManager      ComicsManager.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:ComicsManager     X TXceed.Wpf.Toolkit, Version=4.3.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f47toolkit)http://schemas.xceed.com/wpf/xaml/toolkit  _ [MaterialDesignThemes.Wpf, Version=4.4.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  -     � Title$   Office Ware Comics Manager��$	��450��$	��800�� � WindowState$ 	Maximized=� J� InputBindings �� 
 �� Key$ Z�� �� 	Modifiers$
 Ctrlf� �� Command$  ��.��]� .+�o� $	��Top=�.@�m� $��Файл��.@�m� -   $��Открыть главу��m� -   $#��Сохранить главу��m� -   $��Закрыть главу��m� -    m�Name$ toolMenu��$��Инструменты��.@�m� -   $��Создать кадр�� � DataContext$ Создать кадр�� m� IsCheckable . 	 m� 	IsChecked	 . m� -   $��Эффекты��$# Добавить эффект�� . 	 .  m� -   $��Перо��$# Рисование пером�� . 	 .  m� -	   $��Бабл��$ Добавить бабл�� . 	 .  m� -
   $��Флэшбек��$+ %Переключить флэшбек�� . 	 .  m� -   $��Маркер��$+ %Раскрасить маркером�� . 	 .  m� -   $��Белила��$' !Замазать белилами�� . 	 .  m� -   $��Скринтоны��$+ %Применить скринтоны�� . 	 .  m� -   $��Выбрать кадр��$, &Задать активный кадр�� . 	 .  m� -   $)��#Редактировать бабл��$> 8Редактировать содержмое бабла�� . 	 .  m� -   $!��Направить бабл��$4 .Задать направление бабла�� . 	 .  m� -   $��Создать кадры��$8 2Заполнить страницу кадрами�� . 	 .  m� -   $��Лого��$- 'Управление логотипом�� . 	 .  m� -   $��Карандаш��$- 'Рисовать раскадровку�� . 	 .  m� -   $ modeMenu��$��Режимы��.@�m� -   $��Манускрипт��$ 0�� . 	 . m� -   $��Раскадровка��$ 1�� . 	 .  m� -   $ storyBoardModeMenu��$��Раскадровка��.@�m� -   $��Сценарий��$ 0�� . 	 . m� -   $��Визуально��$ 1�� . 	 .  �� -   
 ��Name$
 toolBarControl��$	��Top=�$��75��
������ $��0q�.@��� $y�	Collapsed=�.���� .+��� -    ��Name$ toolBarHeader�� �� Orientation$ 
Horizontal=�$��5q�.+��� -   $��Создать кадр��$��25��$	��5 0q�$ Создать кадр���� -    ��Name$ fxTool��$��Эффекты��$��25��$	��5 0q�$# Добавить эффект���� $	��275��$��25��$	��5 0q� �� SelectedIndex$ 0��.@��� ���� -   
�������$��10000�� � Cursor$	 Pen��.+��� 	 �� X1$	 100��	 �� Y1$ 0��	 �� X2$	 100��	 �� Y2$	 100��
���   �z���  �� Angle$ 0Q� �� CenterX$	 100Q� �� CenterY$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $ 25Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $ 50Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $ 75Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $	 100Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $	 125Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $	 150Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $	 175Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $	 200Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $	 225Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $	 250Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $	 275Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z��� $	 300Q�$	 100Q�$	 100Q��� $	 100��$ 0��$	 100��$	 100��
���   �z�V� .~��� $	 325Q�$	 100Q�$	 100Q��� -   $ penTool��$��Перо��$��25��$	��5 0q�$# Рисование пером���� -    $��Бабл��$��25��$	��5 0q�$ Добавить бабл���� -!    ��Name$ 
bubblesBox��$	��275��$��25��$	��5 0q�$ 0��.@��� ���� $	��100��$	��100������  �� 	UriSource$ Assets\bubble_1_2.png?��� ���� $	��100��$	��100������ $ Assets\bubble_2.png?��� ���� $	��100��$	��100������ $ Assets\bubble_3.png?��� ���� $	��100��$	��100������ $ Assets\bubble_4.png?��� ���� $	��100��$	��100������ $ Assets\bubble_5.png?�d� -"    d�Name$ flashBackTool��$��Флэшбек��$��25��$	��5 0q� d� 	IsChecked$ FalseY�$+ %Переключить флэшбек���� -#   $��Маркер��$��25��$	��5 0q�$+ %Раскрасить маркером��#  Xceed.Wpf.Toolkit.ColorPicker  -$     Name$ markerColorPicker��$��40��$��25���� -%   $��Белила��$��25��$	��5 0q�$' !Замазать белилами��d� -&   $ screenTonesTool��$��Скринтоны��$��25��$	��5 0q�$+ %Применить скринтоны���� -'   $ screenTonesBox��$	��275��$��25��$	��5 0q�$ 0��.@��� ���� $	��100��$	��100������ $ Assets\screentone_1.jpg?��� ���� $	��100��$	��100������ $ Assets\screentone_2.jpg?��� ���� $	��100��$	��100������ $ Assets\screentone_3.jpg?��� ���� $	��100��$	��100������ $ Assets\screentone_4.jpg?��� -(   $��Выбрать кадр��$, &Задать активный кадр��$��25��$	��5 0q��� -)   $ toolBarFooter��$ 
Horizontal=�$��5q�.+��� -*   $)��#Редактировать бабл��$> 8Редактировать содержмое бабла��$��25��$	��5 0q��� -+   $!��Направить бабл��$4 .Задать направление бабла��$��25��$	��5 0q��� -,   $��Создать кадры��$8 2Заполнить страницу кадрами��$��25��$	��5 0q��� --   $��Лого��$- 'Управление логотипом��$��25��$	��5 0q��� $y�	Collapsed=�.���� -.   $ storyBoardToolBar��$
��Left=�.+��� -/   $ 
pencilTool��$��Карандаш��$- 'Рисовать раскадровку��$��25��$	��5 0q�$��75���� $ 
Horizontal=�$��Bottom=�$��75��
�������.+�'  !MaterialDesignThemes.Wpf.PackIcon  -0   $
��25 0q�$��Center=�   Kind$	 Add=�  -1     Name$ pageBackBtn��$
��25 0q�$��Center=�$ 	ArrowLeft=�
��������� $��Hidden=�.���� -2   $ pages��$
��1335��$ 
Horizontal=�.+��� -3   $��40��$��65��$��5q�
����·�.+��� $��40��$��65������ $X Rhttps://www.google.com/images/branding/googlelogo/2x/googlelogo_color_160x56dp.png?�  -4   $ pageNextBtn��$
��25 0q�$��Center=�$ 
ArrowRight=�
��������� -5   $
 modeControl��.@��� $��Манускрипт��.���� -6   $
 manuscriptPages��$��0q�
������ .@��� $y�	Collapsed=�.���� -7    ��Name$ 
background��.���� -8    ��Name$ 
manuscript��
�������$	 Pen��}�.   J� Clip  B� ��D� E� -9   currentFrameOrigin����@� � -:   currentFrameController��! � Points! 8� " 8� X$" 0Q�# 8� Y$# 0Q�8� $" 10000Q�$# 0Q�8� $" 10000Q�$# 10000Q�8� $" 0Q�$# 10000Q��� $��Раскадровка��.���� -;   $
 storyBoardModeControl��$
��0 15q�.@��� $��Сценарий��.���� -<   $ ��Name$$ storyBoardDescBox��% �� AcceptsReturn% . �� $��Визуально��.���� .���� -=   $ 
storyBoard��$��10000��
������ $	 Pen��