﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "23344B07CE2775316E4A80B7DDC945902AF4BA84C760FEBAD8A23EBF09CAA483"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using ComicsManager;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Converters;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Mag.Converters;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace ComicsManager {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 42 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl toolBarControl;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel toolBarHeader;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button fxTool;
        
        #line default
        #line hidden
        
        
        #line 317 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox bubblesBox;
        
        #line default
        #line hidden
        
        
        #line 396 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton flashBackTool;
        
        #line default
        #line hidden
        
        
        #line 414 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.ColorPicker markerColorPicker;
        
        #line default
        #line hidden
        
        
        #line 426 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton screenTonesTool;
        
        #line default
        #line hidden
        
        
        #line 436 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox screenTonesBox;
        
        #line default
        #line hidden
        
        
        #line 508 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel toolBarFooter;
        
        #line default
        #line hidden
        
        
        #line 570 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel pages;
        
        #line default
        #line hidden
        
        
        #line 680 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer background;
        
        #line default
        #line hidden
        
        
        #line 683 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas manuscript;
        
        #line default
        #line hidden
        
        
        #line 696 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.PathFigure currentFrameOrigin;
        
        #line default
        #line hidden
        
        
        #line 701 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.PolyLineSegment currentFrameController;
        
        #line default
        #line hidden
        
        
        #line 742 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox storyBoardDescBox;
        
        #line default
        #line hidden
        
        
        #line 753 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas storyBoard;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ComicsManager;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 14 "..\..\MainWindow.xaml"
            ((ComicsManager.MainWindow)(target)).KeyUp += new System.Windows.Input.KeyEventHandler(this.GlobalHotKeyHandler);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 33 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenChapterHandler);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 37 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.SaveChapterHandler);
            
            #line default
            #line hidden
            return;
            case 4:
            this.toolBarControl = ((System.Windows.Controls.TabControl)(target));
            return;
            case 5:
            this.toolBarHeader = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            
            #line 62 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 7:
            this.fxTool = ((System.Windows.Controls.Button)(target));
            
            #line 70 "..\..\MainWindow.xaml"
            this.fxTool.Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 84 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Canvas)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.TouchUpHandler);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 307 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 314 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 11:
            this.bubblesBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 12:
            this.flashBackTool = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            
            #line 402 "..\..\MainWindow.xaml"
            this.flashBackTool.Checked += new System.Windows.RoutedEventHandler(this.ToggleFlashBackHandler);
            
            #line default
            #line hidden
            
            #line 403 "..\..\MainWindow.xaml"
            this.flashBackTool.Unchecked += new System.Windows.RoutedEventHandler(this.ToggleFlashBackHandler);
            
            #line default
            #line hidden
            
            #line 404 "..\..\MainWindow.xaml"
            this.flashBackTool.Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 411 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 14:
            this.markerColorPicker = ((Xceed.Wpf.Toolkit.ColorPicker)(target));
            return;
            case 15:
            
            #line 423 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 16:
            this.screenTonesTool = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            
            #line 431 "..\..\MainWindow.xaml"
            this.screenTonesTool.Checked += new System.Windows.RoutedEventHandler(this.ApplyScreenTonesHandler);
            
            #line default
            #line hidden
            
            #line 432 "..\..\MainWindow.xaml"
            this.screenTonesTool.Unchecked += new System.Windows.RoutedEventHandler(this.ResetScreenTonesHandler);
            
            #line default
            #line hidden
            
            #line 433 "..\..\MainWindow.xaml"
            this.screenTonesTool.Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 17:
            this.screenTonesBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 18:
            
            #line 504 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 19:
            this.toolBarFooter = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 20:
            
            #line 518 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 21:
            
            #line 525 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 22:
            
            #line 532 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 23:
            
            #line 549 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SelectToolHandler);
            
            #line default
            #line hidden
            return;
            case 24:
            
            #line 564 "..\..\MainWindow.xaml"
            ((MaterialDesignThemes.Wpf.PackIcon)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.SelectPreviousPageHandler);
            
            #line default
            #line hidden
            return;
            case 25:
            this.pages = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 26:
            
            #line 579 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Canvas)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.SelectPageHandler);
            
            #line default
            #line hidden
            return;
            case 27:
            
            #line 597 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Canvas)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.SelectPageHandler);
            
            #line default
            #line hidden
            return;
            case 28:
            
            #line 615 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Canvas)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.SelectPageHandler);
            
            #line default
            #line hidden
            return;
            case 29:
            
            #line 633 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Canvas)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.SelectPageHandler);
            
            #line default
            #line hidden
            return;
            case 30:
            
            #line 651 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Canvas)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.SelectPageHandler);
            
            #line default
            #line hidden
            return;
            case 31:
            
            #line 670 "..\..\MainWindow.xaml"
            ((MaterialDesignThemes.Wpf.PackIcon)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.SelectNextPageHandler);
            
            #line default
            #line hidden
            return;
            case 32:
            
            #line 674 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.TabControl)(target)).SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ToggleModeHandler);
            
            #line default
            #line hidden
            return;
            case 33:
            this.background = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 34:
            this.manuscript = ((System.Windows.Controls.Canvas)(target));
            
            #line 687 "..\..\MainWindow.xaml"
            this.manuscript.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.TouchDownHandler);
            
            #line default
            #line hidden
            
            #line 688 "..\..\MainWindow.xaml"
            this.manuscript.MouseMove += new System.Windows.Input.MouseEventHandler(this.TouchMoveHandler);
            
            #line default
            #line hidden
            
            #line 689 "..\..\MainWindow.xaml"
            this.manuscript.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.TouchUpHandler);
            
            #line default
            #line hidden
            return;
            case 35:
            this.currentFrameOrigin = ((System.Windows.Media.PathFigure)(target));
            return;
            case 36:
            this.currentFrameController = ((System.Windows.Media.PolyLineSegment)(target));
            return;
            case 37:
            this.storyBoardDescBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 38:
            this.storyBoard = ((System.Windows.Controls.Canvas)(target));
            
            #line 755 "..\..\MainWindow.xaml"
            this.storyBoard.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.TouchDownHandler);
            
            #line default
            #line hidden
            
            #line 756 "..\..\MainWindow.xaml"
            this.storyBoard.MouseMove += new System.Windows.Input.MouseEventHandler(this.TouchMoveHandler);
            
            #line default
            #line hidden
            
            #line 757 "..\..\MainWindow.xaml"
            this.storyBoard.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.TouchUpHandler);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

