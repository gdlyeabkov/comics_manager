using System;
using System.Collections.Generic;
using System.IO;
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

using System.Speech.Synthesis;
using System.Windows.Controls.Primitives;

namespace ComicsManager
{
    
    public partial class MainWindow : Window
    {

        public Polygon polygon;
        public PointCollection defaultFrameBorders;
        public Point framesOrigin;
        public string activeTool = "Создать кадр";
        public Polyline sketch;
        public TextBox bubble;
        public Polygon screenToneFrame;
        public bool isFrameSelected = false;
        public Point lastDragerPoint = new Point(0, 0);
        public List<Rectangle> dragers;
        public const double markerThickness = 10.0;
        public SpeechSynthesizer debugger;
        public System.Windows.Shapes.Path path;
        public PolyBezierSegment segment;
        public Point effectOrigin;
        public List<UIElement> history;
        public int currentPageIndex = 0;
        public Image selectedLogo = null;

        public MainWindow()
        {
            InitializeComponent();

            Initialize();

        }

        public void Initialize()
        {
            debugger = new SpeechSynthesizer();
            PointCollection controllerPoints = currentFrameController.Points;
            defaultFrameBorders = new PointCollection();
            // defaultFrameBorders = controllerPoints;
            defaultFrameBorders.Add(controllerPoints[0]);
            defaultFrameBorders.Add(controllerPoints[1]);
            defaultFrameBorders.Add(controllerPoints[2]);
            defaultFrameBorders.Add(controllerPoints[3]);
            Point origin = controllerPoints[0];
            framesOrigin = origin;
            dragers = new List<Rectangle>()
            {
                new Rectangle(),
                new Rectangle(),
                new Rectangle(),
                new Rectangle()
            };
            history = new List<UIElement>();
        }

        private void TouchUpHandler(object sender, MouseButtonEventArgs e)
        {
            TouchUp();
        }

        public void TouchUp()
        {
            Point currentPosition = Mouse.GetPosition(manuscript);
            bool isGenerateFrameTool = activeTool == "Создать кадр";
            bool isApplyEffectTool = activeTool == "Добавить эффект";
            bool isBubbleTool = activeTool == "Добавить бабл";
            bool isCreateFramesTool = activeTool == "Заполнить страницу кадрами";
            bool isLogoTool = activeTool == "Управление логотипом";
            if (isGenerateFrameTool)
            {
                bool isPolygonExists = polygon != null;
                if (isPolygonExists)
                {
                    PointCollection polygonPoints = polygon.Points;
                    int countPolygonPoints = polygonPoints.Count;
                    bool isAddPoint = countPolygonPoints <= 3;
                    if (isAddPoint)
                    {
                        polygon.Points.Add(currentPosition);
                        countPolygonPoints = polygonPoints.Count;
                        bool isClosePolygon = countPolygonPoints >= 4;
                        if (isClosePolygon)
                        {
                            EnterToFrame(polygon);

                            polygon = null;

                        }
                    }
                }
                else
                {
                    polygon = new Polygon();
                    polygon.Points.Add(currentPosition);
                    polygon.Stroke = System.Windows.Media.Brushes.Black;
                    polygon.Fill = System.Windows.Media.Brushes.White;
                    manuscript.Children.Add(polygon);

                    polygon.MouseLeftButtonUp += SelectFrameHandler;

                }
            }
            else if (isApplyEffectTool)
            {
                // Canvas group = new Canvas();
                int effectItems = 15;
                for (int i = 0; i < effectItems; i++)
                {
                    Line effectItem = new Line();
                    manuscript.Children.Add(effectItem);
                    effectItem.X1 = effectOrigin.X;
                    effectItem.Y1 = effectOrigin.Y + 1;
                    effectItem.X2 = effectOrigin.X;
                    
                    double dX = effectOrigin.X - currentPosition.X;
                    double dY = effectOrigin.Y - currentPosition.Y;
                    double multi = dX * dX + dY * dY;
                    double rad = Math.Round(Math.Sqrt(multi), 3, MidpointRounding.AwayFromZero);

                    effectItem.Y2 = effectOrigin.Y + rad;
                    RotateTransform rotateTransform = new RotateTransform();
                    rotateTransform.Angle = 25 * i;
                    rotateTransform.CenterX = effectOrigin.X;
                    rotateTransform.CenterY = effectOrigin.Y;
                    effectItem.RenderTransform = rotateTransform;
                    effectItem.Stroke = System.Windows.Media.Brushes.Black;
                    // group.Children.Add(effectItem);
                }
                // manuscript.Children.Add(group);
            }
            else if (isBubbleTool)
            {
                bubble.Focus();
            }
            else if (isCreateFramesTool)
            {
                Dialogs.CreateFramesDialog dialog = new Dialogs.CreateFramesDialog();
                dialog.Closed += CreateFramesHandler;
                dialog.Show();
            }
            else if (isLogoTool)
            {
                bool isLogoExists = selectedLogo != null;
                if (isLogoExists)
                {
                    UnSelectLogo();
                }
                else
                {
                    ImportLogo(currentPosition);
                }
            }
            RefreshThumbnail();
        }

        public void UnSelectLogo ()
        {
            SelectLogo(null);
        }

        public void ImportLogo(Point origin)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Title = "Выберите лого";
            ofd.Filter = "Png documents (.png)|*.png";
            bool? res = ofd.ShowDialog();
            bool isOpened = res != false;
            if (isOpened)
            {
                string fullPath = ofd.FileName;
                PngBitmapDecoder imageDecoder = new PngBitmapDecoder(new Uri(fullPath), BitmapCreateOptions.None, BitmapCacheOption.Default);
                var frames = imageDecoder.Frames;
                BitmapSource source = frames[0];
                Image logo = new Image();
                double originX = origin.X - 50;
                double originY = origin.Y - 50;
                Canvas.SetLeft(logo, originX);
                Canvas.SetTop(logo, originY);
                logo.Width = 100;
                logo.Height = 100;
                logo.BeginInit();
                logo.Source = source;
                logo.EndInit();
                manuscript.Children.Add(logo);
                logo.MouseLeftButtonDown += SelectLogoHandler;
            }
        }

        public void SelectLogoHandler(object sender, EventArgs e)
        {
            Image currentLogo = ((Image)(sender));
            SelectLogo(currentLogo);
        }

        public void SelectLogo(Image currentLogo)
        {
            selectedLogo = currentLogo;
        }

        public void CreateFramesHandler(object sender, EventArgs e)
        {
            Dialogs.CreateFramesDialog dialog = ((Dialogs.CreateFramesDialog)(sender));
            object rawData = dialog.DataContext;
            bool isDataExists = rawData != null;
            if (isDataExists)
            {
                Dictionary<String, Object> data = ((Dictionary<String, Object>)(rawData));
                object rawCountFramesPerX = data["countFramesPerX"];
                object rawCountFramesPerY = data["countFramesPerY"];
                object rawMargin = data["margin"];
                int countFramesPerX = ((int)(rawCountFramesPerX));
                int countFramesPerY = ((int)(rawCountFramesPerY));
                double margin = ((double)(rawMargin));
                CreateFrames(countFramesPerX, countFramesPerY, margin);
            }
        }

        public void CreateFrames (int countFramesPerX, int countFramesPerY, double margin)
        {
            double totalX = margin;
            double totalY = margin;

            double manuscriptWidth = manuscript.ActualWidth;
            double manuscriptHeight = manuscript.ActualHeight;
            double doubleMargin = margin * 2;
            double frameWidthWithoutMargins = Math.Ceiling(manuscriptWidth / countFramesPerX);
            double frameWidth = frameWidthWithoutMargins - doubleMargin;
            double frameHeightWithoutMargins = Math.Ceiling(manuscriptHeight / countFramesPerY);
            double frameHeight = frameHeightWithoutMargins - doubleMargin;

            Canvas group = new Canvas();

            for (int i = 0; i < countFramesPerY; i++)
            {
                totalX = margin;
                bool isNotFirstRow = i >= 1;
                if (isNotFirstRow)
                {
                    totalY += frameHeight + margin;
                }
                for (int j = 0; j < countFramesPerX; j++)
                {
                    bool isNotFirstColumn = j >= 1;
                    if (isNotFirstColumn)
                    {
                        totalX += frameWidth + margin;
                    }
                    Polygon newFrame = new Polygon();
                    Point newFrameFirstPoint = new Point(totalX, totalY + margin);
                    newFrame.Points.Add(newFrameFirstPoint);
                    Point newFrameSecondPoint = new Point(totalX + frameWidth, totalY + margin);
                    newFrame.Points.Add(newFrameSecondPoint);
                    Point newFrameThirdPoint = new Point(totalX + frameWidth, totalY + frameHeight + margin);
                    newFrame.Points.Add(newFrameThirdPoint);
                    Point newFrameFourthPoint = new Point(totalX, totalY + frameHeight + margin);
                    newFrame.Points.Add(newFrameFourthPoint);
                    newFrame.Fill = System.Windows.Media.Brushes.White;
                    newFrame.Stroke = System.Windows.Media.Brushes.Black;
                    group.Children.Add(newFrame);
                    // manuscript.Children.Add(newFrame);
                    newFrame.MouseLeftButtonUp += SelectFrameHandler;
                }
            }
            manuscript.Children.Add(group);
        }

        private void GlobalHotKeyHandler(object sender, KeyEventArgs e)
        {
            Key currentKey = e.Key;
            Key escKey = Key.Escape;
            Key zKey = Key.Z;
            bool isEscKey = currentKey == escKey;
            bool isZKey = currentKey == zKey;
            bool isCtrlEnabled = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            bool isShiftEnabled = (Keyboard.Modifiers & ModifierKeys.Shift) > 0;
            bool isUndoCommand = isZKey && isCtrlEnabled;
            bool isRedoCommand = isZKey && isCtrlEnabled && isShiftEnabled;
            if (isEscKey)
            {
                ResetFrameBorders();
            }
            else if (isRedoCommand)
            {
                int countHistoryRecords = history.Count;
                bool isHaveRecords = countHistoryRecords >= 1;
                if (isHaveRecords)
                {
                    int lastRecordIndex = countHistoryRecords - 1;
                    UIElement element = history[lastRecordIndex];
                    try
                    {
                        int selectedToolIndex = toolBarControl.SelectedIndex;
                        bool isManuscriptTool = selectedToolIndex == 0;
                        if (isManuscriptTool)
                        {
                            manuscript.Children.Add(element);
                        }
                        else
                        {
                            storyBoard.Children.Add(element);
                        }
                        history.RemoveAt(lastRecordIndex);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else if (isUndoCommand)
            {
                bool isPolygonExists = polygon != null;
                if (isPolygonExists)
                {
                    PointCollection polygonPoints = polygon.Points;
                    int countPolygonPoints = polygonPoints.Count;
                    bool isPolygonClosed = countPolygonPoints >= 4;
                    if (isPolygonClosed)
                    {
                        Undo();
                    }
                    else
                    {
                        MessageBox.Show("Вы пытаетесь удалить кадр, не закрыв его", "Ошибка");
                    }
                }
                else
                {
                    Undo();
                }
            }
        }

        public void Undo ()
        {
            UIElementCollection canvasChildren = null;
            int selectedToolIndex = toolBarControl.SelectedIndex;
            bool isManuscriptTool = selectedToolIndex == 0;
            if (isManuscriptTool)
            {
                canvasChildren = manuscript.Children;
            }
            else
            {
                canvasChildren = storyBoard.Children;
            }
            int countManuscriptChildren = canvasChildren.Count;
            bool isHaveChilds = countManuscriptChildren >= 1;
            if (isHaveChilds)
            {
                int lastChildIndex = countManuscriptChildren - 1;
                history.Add(canvasChildren[lastChildIndex]);
                canvasChildren.RemoveAt(lastChildIndex);
                if (isFrameSelected)
                {
                    ResetFrameBorders();
                }
            }
        }

        public void ResetFrameBorders()
        {
            currentFrameOrigin.StartPoint = framesOrigin;
            currentFrameController.Points[0] = defaultFrameBorders[0];
            currentFrameController.Points[1] = defaultFrameBorders[1];
            currentFrameController.Points[2] = defaultFrameBorders[2];
            currentFrameController.Points[3] = defaultFrameBorders[3];
            isFrameSelected = false;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SelectToolHandler(object sender, RoutedEventArgs e)
        {
            bool isSimpleBtn = sender is Button;
            bool isCompoundBtn = sender is ToggleButton;
            object toolData = "";
            if (isSimpleBtn)
            {
                Button tool = ((Button)(sender));
                toolData = tool.DataContext;
            }
            else if (isCompoundBtn)
            {
                ToggleButton tool = ((ToggleButton)(sender));
                bool isToolChecked = ((bool)(tool.IsChecked));
                if (isToolChecked)
                {
                    toolData = tool.DataContext;
                }
            }
            string toolName = ((string)(toolData));
            SelectTool(toolName);
        }

        public void SelectTool(string tool)
        {
            activeTool = tool;
            HideBubbleDragers();
            foreach (UIElement toolBarHeaderItem in toolBarHeader.Children)
            {
                SelectActiveTool(toolBarHeaderItem, tool);
            }
            foreach (UIElement toolBarFooterItem in toolBarFooter.Children)
            {
                SelectActiveTool(toolBarFooterItem, tool);
            }
            foreach (UIElement storyBoardToolBarItem in storyBoardToolBar.Children)
            {
                SelectActiveTool(storyBoardToolBarItem, tool);
            }
            foreach (MenuItem toolMenuItem in toolMenu.Items)
            {
                toolMenuItem.IsChecked = false;
                object toolData = toolMenuItem.DataContext;
                string toolName = ((string)(toolData));
                bool isToolFound = tool == toolName;
                if (isToolFound)
                {
                    toolMenuItem.IsChecked = true;
                }
            }
        }

        public void SelectActiveTool (UIElement toolBarItem, string tool)
        {
            bool isSimpleBtn = toolBarItem is Button;
            bool isCompoundBtn = toolBarItem is ToggleButton;
            bool isBtn = isSimpleBtn || isCompoundBtn;
            if (isBtn)
            {
                if (isSimpleBtn)
                {
                    Button btn = toolBarItem as Button;
                    object toolData = btn.DataContext;
                    string toolName = ((string)(toolData));
                    bool isActiveTool = tool == toolName;
                    if (isActiveTool)
                    {
                        btn.Background = System.Windows.Media.Brushes.LightBlue;
                    }
                    else
                    {
                        btn.Background = System.Windows.Media.Brushes.LightGray;
                    }
                }
                else
                {
                    ToggleButton btn = toolBarItem as ToggleButton;
                    object toolData = btn.DataContext;
                    string toolName = ((string)(toolData));
                    bool isActiveTool = tool == toolName;
                    /*
                        Нельзя сбрасывать checked если этот инструмент не выбран это может привести к таким ошибкам как снятие флешбека или скринтонов
                        btn.IsChecked = isActiveTool;
                    */
                }
            }
        }

        public void HideBubbleDragers()
        {
            dragers[0].Fill = System.Windows.Media.Brushes.Transparent;
            dragers[1].Fill = System.Windows.Media.Brushes.Transparent;
            dragers[2].Fill = System.Windows.Media.Brushes.Transparent;
            dragers[3].Fill = System.Windows.Media.Brushes.Transparent;
            manuscript.Children.Remove(dragers[0]);
            manuscript.Children.Remove(dragers[1]);
            manuscript.Children.Remove(dragers[2]);
            manuscript.Children.Remove(dragers[3]);
        }

        private void ToggleFlashBackHandler(object sender, RoutedEventArgs e)
        {
            ToggleFlashBack();
        }

        public void ToggleFlashBack()
        {
            bool isFlashBack = ((bool)(flashBackTool.IsChecked));
            if (isFlashBack)
            {
                manuscript.Background = System.Windows.Media.Brushes.Black;
            }
            else
            {
                manuscript.Background = System.Windows.Media.Brushes.White;
            }
        }

        private void TouchDownHandler(object sender, MouseButtonEventArgs e)
        {
            // debugger.Speak("касание");
            Point currentPosition = Mouse.GetPosition(manuscript);
            double coordX = currentPosition.X;
            double coordY = currentPosition.Y;
            bool isApplyEffectTool = activeTool == "Добавить эффект";
            bool isPenTool = activeTool == "Рисование пером";
            bool isBubbleTool = activeTool == "Добавить бабл";
            bool isMarkerTool = activeTool == "Раскрасить маркером";
            bool isWhitewashTool = activeTool == "Замазать белилами";
            bool isPencilTool = activeTool == "Рисовать раскадровку";
            if (isApplyEffectTool)
            {
                effectOrigin = currentPosition;
            }
            else if (isPenTool)
            {
                sketch = new Polyline();
                PointCollection pointCollection = new PointCollection();
                sketch.Points = pointCollection;
                sketch.Stroke = System.Windows.Media.Brushes.Black;
                manuscript.Children.Add(sketch);
            }
            else if (isBubbleTool)
            {
                int selectedBubblesBoxItemIndex = bubblesBox.SelectedIndex;
                ItemCollection bubblesBoxItems = bubblesBox.Items;
                ComboBoxItem selectedBubblesBoxItem = ((ComboBoxItem)(bubblesBoxItems[selectedBubblesBoxItemIndex]));
                bubble = new TextBox();
                bubble.TextWrapping = TextWrapping.Wrap;
                bubble.BorderThickness = new Thickness(0);
                bubble.AcceptsReturn = true;
                bubble.TextAlignment = TextAlignment.Center;
                bubble.VerticalContentAlignment = VerticalAlignment.Center;
                bubble.VerticalAlignment = VerticalAlignment.Center;
                ImageBrush bubbleBrush = new ImageBrush();
                bubbleBrush.ImageSource = ((Image)(selectedBubblesBoxItem.Content)).Source;
                bubble.Background = bubbleBrush;
                bubble.Width = 150;
                bubble.Height = 150;
                bubble.Padding = new Thickness(35);
                manuscript.Children.Add(bubble);
                bubble.MouseLeftButtonUp += SelectBubbleHandler;
                bubble.LostKeyboardFocus += BubbleLostFocusHandler;
                bool isNotDirectionalBubble = selectedBubblesBoxItemIndex != 4;
                if (isNotDirectionalBubble)
                {
                    bubble.DataContext = ((int)(selectedBubblesBoxItemIndex));
                }
                else
                {
                    bubble.DataContext = new System.Windows.Shapes.Path();
                }
            }
            else if (isMarkerTool)
            {
                sketch = new Polyline();
                PointCollection pointCollection = new PointCollection();
                sketch.Points = pointCollection;
                Color? color = markerColorPicker.SelectedColor;
                bool isColorSelected = color != null;
                if (isColorSelected)
                {
                    Color selectedColor = ((Color)(color));
                    Color selectedColorBrush = color.Value;
                    sketch.Stroke = new SolidColorBrush(selectedColor);
                }
                else
                {
                    sketch.Stroke = System.Windows.Media.Brushes.Black;
                }
                sketch.StrokeThickness = markerThickness;
                manuscript.Children.Add(sketch);
            }
            else if (isWhitewashTool)
            {
                if (isFrameSelected)
                {
                    sketch = new Polyline();
                    PointCollection pointCollection = new PointCollection();
                    sketch.Points = pointCollection;
                    sketch.Stroke = System.Windows.Media.Brushes.White;
                    // sketch.StrokeThickness = 35.0;
                    sketch.StrokeThickness = 1.0;
                    manuscript.Children.Add(sketch);
                }
            }
            else if (isPencilTool)
            {
                MouseButtonState leftMouseBtn = Mouse.LeftButton;
                MouseButtonState mousePressed = MouseButtonState.Pressed;
                bool isMousePressed = mousePressed == leftMouseBtn;
                if (isMousePressed) {
                    sketch = new Polyline();
                    PointCollection pointCollection = new PointCollection();
                    sketch.Points = pointCollection;
                    sketch.Stroke = System.Windows.Media.Brushes.Black;
                    storyBoard.Children.Add(sketch);
                }
            }
        }

        private void BubbleLostFocusHandler(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox selectedBubble = ((TextBox)(sender));
            selectedBubble.IsReadOnly = true;
            selectedBubble.BorderBrush = System.Windows.Media.Brushes.Transparent;
            selectedBubble.BorderThickness = new Thickness(0.0);
        }

        private void TouchMoveHandler(object sender, MouseEventArgs e)
        {
            MouseButtonState leftMouseBtn = Mouse.LeftButton;
            MouseButtonState mousePressed = MouseButtonState.Pressed;
            bool isMousePressed = mousePressed == leftMouseBtn;
            if (isMousePressed)
            {
                Point currentPosition = Mouse.GetPosition(manuscript);
                double coordX = currentPosition.X;
                double coordY = currentPosition.Y;
                bool isPenTool = activeTool == "Рисование пером";
                bool isBubbleTool = activeTool == "Добавить бабл";
                bool isMarkerTool = activeTool == "Раскрасить маркером";
                bool isWhitewashTool = activeTool == "Замазать белилами";
                bool isPencilTool = activeTool == "Рисовать раскадровку";
                bool isSetBubbleDirectionTool = activeTool == "Задать направление бабла";
                bool isLogoTool = activeTool == "Управление логотипом";
                if (isPenTool)
                {
                    bool isSketchExists = sketch != null;
                    if (isSketchExists)
                    {
                        PointCollection pointCollection = sketch.Points;
                        pointCollection.Add(currentPosition);
                        sketch.Points = pointCollection;
                    }
                }
                else if (isBubbleTool)
                {
                    bool isBubbleExists = bubble != null;
                    if (isBubbleExists)
                    {
                        double startX = Canvas.GetLeft(bubble);
                        double startY = Canvas.GetTop(bubble);
                        Canvas.SetLeft(bubble, coordX);
                        Canvas.SetTop(bubble, coordY);
                        object bubbleData = bubble.DataContext;
                        bool isDirectionalBubble = bubbleData is System.Windows.Shapes.Path;
                        if (isDirectionalBubble)
                        {
                            Point startBubbleDirectionPoint = new Point(currentPosition.X - 10, currentPosition.Y);
                            Point endBubbleDirectionPoint = new Point(currentPosition.X + 10, currentPosition.Y);
                            System.Windows.Shapes.Path path = ((System.Windows.Shapes.Path)(bubble.DataContext));
                            PathGeometry pathGeometry = ((PathGeometry)(path.Data));
                            bool isPathGeometryExists = pathGeometry != null;
                            if (isPathGeometryExists) {
                                double deltaX = coordX - startX;
                                double deltaY = coordY - startY;
                                PathFigureCollection pathFigures = ((PathFigureCollection)(pathGeometry.Figures));
                                PathFigure pathFigure = ((PathFigure)(pathFigures[0]));
                                PathSegmentCollection pathSegments = ((PathSegmentCollection)(pathFigure.Segments));
                                PolyBezierSegment bubbleDirectionSegment = ((PolyBezierSegment)(pathSegments[0]));
                                Point bubbleDirectionSegmentFirstPoint = bubbleDirectionSegment.Points[0];
                                Point bubbleDirectionSegmentSecondPoint = bubbleDirectionSegment.Points[1];
                                Point bubbleDirectionSegmentThirdPoint = bubbleDirectionSegment.Points[2];
                                double bubbleDirectionSegmentFirstPointX = bubbleDirectionSegmentFirstPoint.X;
                                double bubbleDirectionSegmentFirstPointY = bubbleDirectionSegmentFirstPoint.Y;
                                double bubbleDirectionSegmentSecondPointX = bubbleDirectionSegmentSecondPoint.X;
                                double bubbleDirectionSegmentSecondPointY = bubbleDirectionSegmentSecondPoint.Y;
                                double bubbleDirectionSegmentThirdPointX = bubbleDirectionSegmentThirdPoint.X;
                                double bubbleDirectionSegmentThirdPointY = bubbleDirectionSegmentThirdPoint.Y;
                                double updatedBubbleDirectionSegmentFirstPointX = bubbleDirectionSegmentFirstPointX + deltaX;
                                double updatedBubbleDirectionSegmentFirstPointY = bubbleDirectionSegmentFirstPointY + deltaY;
                                double updatedBubbleDirectionSegmentSecondPointX = bubbleDirectionSegmentSecondPointX + deltaX;
                                double updatedBubbleDirectionSegmentSecondPointY = bubbleDirectionSegmentSecondPointY + deltaY;
                                double updatedBubbleDirectionSegmentThirdPointX = bubbleDirectionSegmentThirdPointX + deltaX;
                                double updatedBubbleDirectionSegmentThirdPointY = bubbleDirectionSegmentThirdPointY + deltaY;
                                Point updatedBubbleDirectionSegmentFirstPoint = new Point(updatedBubbleDirectionSegmentFirstPointX, updatedBubbleDirectionSegmentFirstPointY);
                                Point updatedBubbleDirectionSegmentSecondPoint = new Point(updatedBubbleDirectionSegmentSecondPointX, updatedBubbleDirectionSegmentSecondPointY);
                                Point updatedBubbleDirectionSegmentThirdPoint = new Point(updatedBubbleDirectionSegmentThirdPointX, updatedBubbleDirectionSegmentThirdPointY);
                                pathFigures[0].StartPoint = updatedBubbleDirectionSegmentFirstPoint;
                                bubbleDirectionSegment.Points[0] = updatedBubbleDirectionSegmentFirstPoint;
                                bubbleDirectionSegment.Points[1] = updatedBubbleDirectionSegmentSecondPoint;
                                bubbleDirectionSegment.Points[2] = updatedBubbleDirectionSegmentThirdPoint;

                            }
                        }
                    }
                }
                else if (isMarkerTool)
                {
                    PointCollection pointCollection = sketch.Points;
                    pointCollection.Add(currentPosition);
                    sketch.Points = pointCollection;
                }
                else if (isWhitewashTool)
                {
                    if (isFrameSelected)
                    {
                        PointCollection pointCollection = sketch.Points;
                        pointCollection.Add(currentPosition);
                        sketch.Points = pointCollection;
                    }
                }
                else if (isPencilTool)
                {
                    currentPosition = Mouse.GetPosition(storyBoard);
                    PointCollection pointCollection = sketch.Points;
                    pointCollection.Add(currentPosition);
                    sketch.Points = pointCollection;
                }
                else if (isSetBubbleDirectionTool)
                {
                    bool isBubbleExists = bubble != null;
                    if (isBubbleExists)
                    {
                        bool isSegmentExists = segment != null;
                        if (isSegmentExists)
                        {
                            bool isPointsExists = segment.Points.Count >= 3;
                            if (isPointsExists)
                            {
                                object bubbleData = bubble.DataContext;
                                bool isDirectionalBubble = bubbleData is System.Windows.Shapes.Path;
                                if (isDirectionalBubble)
                                {
                                    segment.Points[1] = currentPosition;
                                }
                            }
                        }
                    }
                }
                else if (isLogoTool)
                {
                    bool isLogoExists = selectedLogo != null;
                    if (isLogoExists)
                    {
                        Canvas.SetLeft(selectedLogo, coordX - selectedLogo.Width / 2);
                        Canvas.SetTop(selectedLogo, coordY - selectedLogo.Height / 2);
                    }
                }
                RefreshThumbnail();
            }

        }


        public void RefreshThumbnail()
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)manuscript.RenderSize.Width, (int)manuscript.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(manuscript);
            var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, ((int)(manuscript.ActualWidth)), ((int)(manuscript.ActualHeight))));
            BitmapEncoder imageEncoder = null;
            BitmapFrame frame = BitmapFrame.Create(rtb);
            imageEncoder = new PngBitmapEncoder();
            imageEncoder.Frames.Add(frame);
            BitmapSource source = frame.Thumbnail;
            Canvas currentPage = ((Canvas)(pages.Children[currentPageIndex]));
            Image thumbnail = ((Image)(currentPage.Children[0]));
            thumbnail.BeginInit();
            thumbnail.Source = crop;
            thumbnail.EndInit();
        }

        public void RefreshThumbnails()
        {
            foreach (TabItem manuscriptPage in manuscriptPages.Items)
            {
                int manuscriptIndex = manuscriptPages.Items.IndexOf(manuscriptPage);
                int manuscriptWidth = ((int)(((Canvas)((ScrollViewer)(manuscriptPage.Content)).Content)).RenderSize.Width);
                int manuscriptHeight = ((int)(((Canvas)((ScrollViewer)(manuscriptPage.Content)).Content)).RenderSize.Height);
                if (manuscriptWidth <= 0)
                {
                    manuscriptWidth = 1000;
                }
                if (manuscriptHeight <= 0)
                {
                    manuscriptHeight = 1000;
                }
                RenderTargetBitmap rtb = new RenderTargetBitmap(manuscriptWidth, manuscriptHeight, 96d, 96d, System.Windows.Media.PixelFormats.Default);
                rtb.Render(((Canvas)((ScrollViewer)(manuscriptPage.Content)).Content));
                var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, ((int)(((Canvas)((ScrollViewer)(manuscriptPage.Content)).Content).ActualWidth)), ((int)(((Canvas)((ScrollViewer)(manuscriptPage.Content)).Content).ActualHeight))));
                BitmapEncoder imageEncoder = null;
                BitmapFrame frame = BitmapFrame.Create(rtb);
                imageEncoder = new PngBitmapEncoder();
                imageEncoder.Frames.Add(frame);
                BitmapSource source = frame.Thumbnail;
                Canvas currentPage = ((Canvas)(pages.Children[manuscriptIndex]));
                Image thumbnail = ((Image)(currentPage.Children[0]));
                thumbnail.BeginInit();
                thumbnail.Source = crop;
                thumbnail.EndInit();
            }
        }

        public void ApplyScreenTones()
        {
            if (isFrameSelected)
            {
                int selectedScreenTonesBoxItemIndex = screenTonesBox.SelectedIndex;
                ItemCollection screenTonesBoxItems = screenTonesBox.Items;
                ComboBoxItem selectedScreenTonesBoxItem = ((ComboBoxItem)(screenTonesBoxItems[selectedScreenTonesBoxItemIndex]));
                ImageSource imageSource = ((Image)(selectedScreenTonesBoxItem.Content)).Source;
                ImageBrush frameBrush = new ImageBrush(imageSource);
                screenToneFrame.Fill = frameBrush;
            }
            else
            {
                MessageBox.Show("Вы пытаетесь наложить скринтон, не выбрав кадр", "Ошибка");
            }
        }

        private void ApplyScreenTonesHandler(object sender, RoutedEventArgs e)
        {
            ApplyScreenTones();
        }

        public void ResetScreenTonesHandler(object sender, RoutedEventArgs e)
        {
            ResetScreenTones();
        }

        public void ResetScreenTones()
        {
            if (isFrameSelected)
            {
                screenToneFrame.Fill = System.Windows.Media.Brushes.White;
            }
            else
            {
                MessageBox.Show("Вы пытаетесь наложить скринтон, не выбрав кадр", "Ошибка");
            }
        }

        private void SelectFrameHandler(object sender, RoutedEventArgs e)
        {
            Polygon selectedFrame = ((Polygon)(sender));
            SelectFrame(selectedFrame);
        }

        public void SelectFrame(Polygon selectedFrame)
        {
            bool isSelectFrameTool = activeTool == "Задать активный кадр";
            if (isSelectFrameTool)
            {
                EnterToFrame(selectedFrame);
                UpdateScreenTonesTool(selectedFrame);
            }
        }

        public void UpdateScreenTonesTool(Polygon selectedFrame)
        {
            Brush frameBrush = selectedFrame.Fill;
            bool isScreenTonesApplied = frameBrush is ImageBrush;
            screenTonesTool.IsChecked = isScreenTonesApplied;
        }

        public void EnterToFrame(Polygon selectedFrame)
        {
            PointCollection framePoints = new PointCollection();
            PointCollection currentFramePoints = selectedFrame.Points;
            Point firstPoint = currentFramePoints[0];
            Point secondPoint = currentFramePoints[1];
            Point thirdPoint = currentFramePoints[2];
            Point fourthPoint = currentFramePoints[3];
            framePoints.Add(firstPoint);
            framePoints.Add(secondPoint);
            framePoints.Add(thirdPoint);
            framePoints.Add(fourthPoint);
            currentFrameOrigin.StartPoint = firstPoint;
            currentFrameController.Points.Clear();
            currentFrameController.Points = framePoints;
            screenToneFrame = selectedFrame;
            isFrameSelected = true;
        }

        public void SelectBubbleHandler(object sender, MouseEventArgs e)
        {
            TextBox selectedBubble = ((TextBox)(sender));
            SelectBubble(selectedBubble);
        }

        public void SelectBubble(TextBox selectedBubble)
        {
            bool isEditBubbleTool = activeTool == "Редактировать содержмое бабла";
            bool isSetBubbleDirectionTool = activeTool == "Задать направление бабла";
            if (isEditBubbleTool)
            {
                selectedBubble.IsReadOnly = false;
                selectedBubble.Focus();
                selectedBubble.BorderBrush = System.Windows.Media.Brushes.LightGray;
                selectedBubble.BorderThickness = new Thickness(1.0);

                HideBubbleDragers();

                dragers[0] = new Rectangle();
                dragers[0].Width = 5;
                dragers[0].Height = 5;
                dragers[0].Fill = System.Windows.Media.Brushes.Red;
                Canvas.SetLeft(dragers[0], Canvas.GetLeft(selectedBubble));
                Canvas.SetTop(dragers[0], Canvas.GetTop(selectedBubble));
                manuscript.Children.Add(dragers[0]);
                dragers[0].DataContext = selectedBubble;
                dragers[0].MouseMove += LeftTopDragBubbleHandler;
                dragers[1] = new Rectangle();
                dragers[1].Width = 5;
                dragers[1].Height = 5;
                dragers[1].Fill = System.Windows.Media.Brushes.Red;
                Canvas.SetLeft(dragers[1], Canvas.GetLeft(selectedBubble) + selectedBubble.Width - dragers[1].Width);
                Canvas.SetTop(dragers[1], Canvas.GetTop(selectedBubble));
                manuscript.Children.Add(dragers[1]);
                dragers[1].DataContext = selectedBubble;
                dragers[1].MouseMove += RightTopDragBubbleHandler;
                dragers[2] = new Rectangle();
                dragers[2].Width = 5;
                dragers[2].Height = 5;
                dragers[2].Fill = System.Windows.Media.Brushes.Red;
                Canvas.SetLeft(dragers[2], Canvas.GetLeft(selectedBubble));
                Canvas.SetTop(dragers[2], Canvas.GetTop(selectedBubble) + selectedBubble.Height - dragers[2].Height);
                manuscript.Children.Add(dragers[2]);
                dragers[2].DataContext = selectedBubble;
                dragers[2].MouseMove += LeftBottomDragBubbleHandler;
                dragers[3] = new Rectangle();
                dragers[3].Width = 5;
                dragers[3].Height = 5;
                dragers[3].Fill = System.Windows.Media.Brushes.Red;
                Canvas.SetLeft(dragers[3], Canvas.GetLeft(selectedBubble) + selectedBubble.Width - dragers[3].Width);
                Canvas.SetTop(dragers[3], Canvas.GetTop(selectedBubble) + selectedBubble.Height - dragers[3].Height);
                manuscript.Children.Add(dragers[3]);
                dragers[3].DataContext = selectedBubble;
                dragers[3].MouseMove += RightBottomDragBubbleHandler;

                bubble = selectedBubble;

            }
            else if (isSetBubbleDirectionTool)
            {
                Point currentPosition = Mouse.GetPosition(manuscript);
                double coordX = currentPosition.X;
                double coordY = currentPosition.Y;
                object bubbleData = bubble.DataContext;
                bool isDirectionalBubble = bubbleData is System.Windows.Shapes.Path;
                if (isDirectionalBubble)
                {
                    manuscript.Children.Remove(((System.Windows.Shapes.Path)(bubbleData)));
                    path = new System.Windows.Shapes.Path();
                    path.Stroke = System.Windows.Media.Brushes.Black;
                    path.Fill = System.Windows.Media.Brushes.White;
                    path.StrokeThickness = 1;
                    PathGeometry pathGeometry = new PathGeometry();
                    PathFigureCollection pathFigureCollection = new PathFigureCollection();
                    PathFigure pathFigure = new PathFigure();
                    PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();
                    pathFigure.Segments = pathSegmentCollection;
                    Point bubbleOrigin = new Point(Canvas.GetLeft(bubble) + bubble.Width / 2, Canvas.GetTop(bubble) + bubble.Height / 2);
                    Point startBubbleDirectionPoint = new Point(currentPosition.X - 10, currentPosition.Y);
                    Point endBubbleDirectionPoint = new Point(currentPosition.X + 10, currentPosition.Y);
                    double bubbleHeight = bubble.Height;
                    double bubbleHeightQuarter = bubbleHeight / 4;
                    double bubbleTop = Canvas.GetTop(bubble);
                    bool isLTThreeQuarters = coordY < bubbleTop + bubbleHeight - bubbleHeightQuarter;
                    bool isGtQuarterHeight = coordY > bubbleTop + bubbleHeightQuarter;
                    bool isSwitchDirection = isLTThreeQuarters && isGtQuarterHeight;
                    if (isSwitchDirection)
                    {
                        startBubbleDirectionPoint = new Point(currentPosition.X, currentPosition.Y - 10);
                        endBubbleDirectionPoint = new Point(currentPosition.X, currentPosition.Y + 10);
                    }
                    pathFigure.StartPoint = startBubbleDirectionPoint;
                    pathFigureCollection.Add(pathFigure);
                    pathGeometry.Figures = pathFigureCollection;
                    path.Data = pathGeometry;
                    PolyBezierSegment mockSegment = new PolyBezierSegment();
                    mockSegment.IsStroked = false;
                    mockSegment.Points.Add(startBubbleDirectionPoint);
                    mockSegment.Points.Add(endBubbleDirectionPoint);
                    segment = new PolyBezierSegment();
                    segment.Points.Add(startBubbleDirectionPoint);
                    segment.Points.Add(currentPosition);
                    segment.Points.Add(endBubbleDirectionPoint);
                    segment.IsSmoothJoin = true;
                    pathSegmentCollection.Add(segment);
                    pathSegmentCollection.Add(mockSegment);
                    manuscript.Children.Add(path);
                    bubble.DataContext = ((System.Windows.Shapes.Path)(path));
                }
            }
        }

        public void LeftTopDragBubbleHandler(object sender, MouseEventArgs e)
        {
            MouseButtonState mouseLeftBtnState = e.LeftButton;
            MouseButtonState mouseBtnPressed = MouseButtonState.Pressed;
            bool isMouseLeftBtnPreesed = mouseBtnPressed == mouseLeftBtnState;
            Point currentPosition = e.GetPosition(manuscript);
            if (isMouseLeftBtnPreesed)
            {

                double startX = Canvas.GetLeft(bubble);
                double startY = Canvas.GetTop(bubble);

                Rectangle drager = ((Rectangle)(sender));
                double dragerWidth = drager.Width;
                double dragerHeight = drager.Height;
                double halfDragerWidth = drager.Width / 2;
                double halfDragerHeight = drager.Height / 2;
                double coordX = currentPosition.X;
                double coordY = currentPosition.Y;
                double deltaX = coordX - lastDragerPoint.X;
                double deltaY = coordY - lastDragerPoint.Y;
                TextBox selectedBubble = ((TextBox)(drager.DataContext));
                Canvas.SetLeft(selectedBubble, coordX - halfDragerWidth);
                Canvas.SetTop(selectedBubble, coordY - halfDragerHeight);
                Canvas.SetLeft(dragers[0], coordX - halfDragerWidth);
                Canvas.SetTop(dragers[0], coordY - halfDragerHeight);
                try
                {
                    selectedBubble.Width -= deltaX;
                    selectedBubble.Height -= deltaY;
                }
                catch (Exception)
                {

                }
                double selectedBubbleWidth = selectedBubble.Width;
                double selectedBubbleHeight = selectedBubble.Height;
                Canvas.SetLeft(dragers[1], coordX + selectedBubbleWidth - dragerWidth);
                Canvas.SetTop(dragers[1], coordY - halfDragerHeight);
                Canvas.SetLeft(dragers[2], coordX - halfDragerWidth);
                Canvas.SetTop(dragers[2], coordY + selectedBubbleHeight - halfDragerHeight);
                Canvas.SetLeft(dragers[3], coordX + selectedBubbleWidth - dragerWidth);
                Canvas.SetTop(dragers[3], coordY + selectedBubbleHeight - halfDragerHeight);

                MoveBubbleDirection(currentPosition, coordX, coordY, startX, startY, deltaX, deltaY);

            }
            lastDragerPoint = currentPosition;
        }

        public void MoveBubbleDirection(Point currentPosition, double coordX, double coordY, double startX, double startY, double deltaX, double deltaY)
        {   object bubbleData = bubble.DataContext;
            bool isDirectionalBubble = bubbleData is System.Windows.Shapes.Path;
            if (isDirectionalBubble)
            {

                // сбрасываю направление бабла при его искажении, если этого не сделать будет искажаться само направление бабла
                manuscript.Children.Remove((UIElement)(bubbleData));

                Point startBubbleDirectionPoint = new Point(currentPosition.X - 10, currentPosition.Y);
                Point endBubbleDirectionPoint = new Point(currentPosition.X + 10, currentPosition.Y);
                System.Windows.Shapes.Path path = ((System.Windows.Shapes.Path)(bubble.DataContext));
                PathGeometry pathGeometry = ((PathGeometry)(path.Data));
                bool isPathGeometryExists = pathGeometry != null;
                if (isPathGeometryExists)
                {
                    // double deltaX = coordX - startX;
                    // double deltaY = coordY - startY;
                    PathFigureCollection pathFigures = ((PathFigureCollection)(pathGeometry.Figures));
                    PathFigure pathFigure = ((PathFigure)(pathFigures[0]));
                    PathSegmentCollection pathSegments = ((PathSegmentCollection)(pathFigure.Segments));
                    PolyBezierSegment bubbleDirectionSegment = ((PolyBezierSegment)(pathSegments[0]));
                    Point bubbleDirectionSegmentFirstPoint = bubbleDirectionSegment.Points[0];
                    Point bubbleDirectionSegmentSecondPoint = bubbleDirectionSegment.Points[1];
                    Point bubbleDirectionSegmentThirdPoint = bubbleDirectionSegment.Points[2];
                    double bubbleDirectionSegmentFirstPointX = bubbleDirectionSegmentFirstPoint.X;
                    double bubbleDirectionSegmentFirstPointY = bubbleDirectionSegmentFirstPoint.Y;
                    double bubbleDirectionSegmentSecondPointX = bubbleDirectionSegmentSecondPoint.X;
                    double bubbleDirectionSegmentSecondPointY = bubbleDirectionSegmentSecondPoint.Y;
                    double bubbleDirectionSegmentThirdPointX = bubbleDirectionSegmentThirdPoint.X;
                    double bubbleDirectionSegmentThirdPointY = bubbleDirectionSegmentThirdPoint.Y;
                    double updatedBubbleDirectionSegmentFirstPointX = bubbleDirectionSegmentFirstPointX + deltaX;
                    double updatedBubbleDirectionSegmentFirstPointY = bubbleDirectionSegmentFirstPointY + deltaY;
                    double updatedBubbleDirectionSegmentSecondPointX = bubbleDirectionSegmentSecondPointX + deltaX;
                    double updatedBubbleDirectionSegmentSecondPointY = bubbleDirectionSegmentSecondPointY + deltaY;
                    double updatedBubbleDirectionSegmentThirdPointX = bubbleDirectionSegmentThirdPointX + deltaX;
                    double updatedBubbleDirectionSegmentThirdPointY = bubbleDirectionSegmentThirdPointY + deltaY;
                    Point updatedBubbleDirectionSegmentFirstPoint = new Point(updatedBubbleDirectionSegmentFirstPointX, updatedBubbleDirectionSegmentFirstPointY);
                    Point updatedBubbleDirectionSegmentSecondPoint = new Point(updatedBubbleDirectionSegmentSecondPointX, updatedBubbleDirectionSegmentSecondPointY);
                    Point updatedBubbleDirectionSegmentThirdPoint = new Point(updatedBubbleDirectionSegmentThirdPointX, updatedBubbleDirectionSegmentThirdPointY);
                    pathFigures[0].StartPoint = updatedBubbleDirectionSegmentFirstPoint;
                    bubbleDirectionSegment.Points[0] = updatedBubbleDirectionSegmentFirstPoint;
                    bubbleDirectionSegment.Points[1] = updatedBubbleDirectionSegmentSecondPoint;
                    bubbleDirectionSegment.Points[2] = updatedBubbleDirectionSegmentThirdPoint;
                }
            }
        }

        public void RightTopDragBubbleHandler(object sender, MouseEventArgs e)
        {
            MouseButtonState mouseLeftBtnState = e.LeftButton;
            MouseButtonState mouseBtnPressed = MouseButtonState.Pressed;
            bool isMouseLeftBtnPreesed = mouseBtnPressed == mouseLeftBtnState;
            Point currentPosition = e.GetPosition(manuscript);
            if (isMouseLeftBtnPreesed)
            {
                Rectangle drager = ((Rectangle)(sender));
                double dragerWidth = drager.Width;
                double dragerHeight = drager.Height;
                double halfDragerWidth = drager.Width / 2;
                double halfDragerHeight = drager.Height / 2;
                double coordX = currentPosition.X;
                double coordY = currentPosition.Y;
                double deltaX = coordX - lastDragerPoint.X;
                double deltaY = coordY - lastDragerPoint.Y;
                TextBox selectedBubble = ((TextBox)(drager.DataContext));
                double selectedBubbleWidth = selectedBubble.Width;
                // Canvas.SetLeft(selectedBubble, coordX - selectedBubbleWidth - halfDragerWidth);
                Canvas.SetLeft(selectedBubble, coordX - selectedBubbleWidth - halfDragerWidth);
                Canvas.SetTop(selectedBubble, coordY - halfDragerHeight);
                Canvas.SetLeft(dragers[1], coordX - halfDragerWidth);
                Canvas.SetTop(dragers[1], coordY - halfDragerHeight);
                try
                {
                    selectedBubble.Width -= deltaX;
                    selectedBubble.Height -= deltaY;
                }
                catch (Exception)
                {

                }
                selectedBubbleWidth = selectedBubble.Width;
                double selectedBubbleHeight = selectedBubble.Height;
                Canvas.SetLeft(dragers[0], coordX - selectedBubbleWidth - dragerWidth);
                Canvas.SetTop(dragers[0], coordY - halfDragerHeight);
                Canvas.SetLeft(dragers[2], coordX - halfDragerWidth);
                Canvas.SetTop(dragers[2], coordY + selectedBubbleHeight - halfDragerHeight);
                Canvas.SetLeft(dragers[3], coordX - selectedBubbleWidth - dragerWidth);
                Canvas.SetTop(dragers[3], coordY + selectedBubbleHeight - halfDragerHeight);
            
                MoveBubbleDirection(currentPosition, coordX, coordY, 0, 0, deltaX, deltaY);

            }
            lastDragerPoint = currentPosition;
        }

        public void LeftBottomDragBubbleHandler(object sender, MouseEventArgs e)
        {
            MouseButtonState mouseLeftBtnState = e.LeftButton;
            MouseButtonState mouseBtnPressed = MouseButtonState.Pressed;
            bool isMouseLeftBtnPreesed = mouseBtnPressed == mouseLeftBtnState;
            Point currentPosition = e.GetPosition(manuscript);
            if (isMouseLeftBtnPreesed)
            {
                Rectangle drager = ((Rectangle)(sender));
                double dragerWidth = drager.Width;
                double dragerHeight = drager.Height;
                double halfDragerWidth = drager.Width / 2;
                double halfDragerHeight = drager.Height / 2;
                double coordX = currentPosition.X;
                double coordY = currentPosition.Y;
                double deltaX = coordX - lastDragerPoint.X;
                double deltaY = coordY - lastDragerPoint.Y;
                TextBox selectedBubble = ((TextBox)(drager.DataContext));
                double selectedBubbleHeight = selectedBubble.Height;
                Canvas.SetLeft(selectedBubble, coordX - halfDragerWidth);
                Canvas.SetTop(selectedBubble, coordY - selectedBubbleHeight - halfDragerHeight);
                Canvas.SetLeft(dragers[2], coordX - halfDragerWidth);
                Canvas.SetTop(dragers[2], coordY - halfDragerHeight);
                try
                {
                    selectedBubble.Width -= deltaX;
                    selectedBubble.Height -= deltaY;
                }
                catch (Exception)
                {

                }
                double selectedBubbleWidth = selectedBubble.Width;
                selectedBubbleHeight = selectedBubble.Height;
                Canvas.SetLeft(dragers[0], coordX + selectedBubbleWidth - dragerWidth);
                Canvas.SetTop(dragers[0], coordY - selectedBubbleHeight - halfDragerHeight);
                Canvas.SetLeft(dragers[1], coordX - halfDragerWidth);
                Canvas.SetTop(dragers[1], coordY - selectedBubbleHeight - halfDragerHeight);
                Canvas.SetLeft(dragers[3], coordX + selectedBubbleWidth - dragerWidth);
                Canvas.SetTop(dragers[3], coordY - halfDragerHeight);

                MoveBubbleDirection(currentPosition, coordX, coordY, 0, 0, deltaX, deltaY);

            }
            lastDragerPoint = currentPosition;
        }

        public void RightBottomDragBubbleHandler(object sender, MouseEventArgs e)
        {
            MouseButtonState mouseLeftBtnState = e.LeftButton;
            MouseButtonState mouseBtnPressed = MouseButtonState.Pressed;
            bool isMouseLeftBtnPreesed = mouseBtnPressed == mouseLeftBtnState;
            Point currentPosition = e.GetPosition(manuscript);
            if (isMouseLeftBtnPreesed)
            {
                Rectangle drager = ((Rectangle)(sender));
                double dragerWidth = drager.Width;
                double dragerHeight = drager.Height;
                double halfDragerWidth = drager.Width / 2;
                double halfDragerHeight = drager.Height / 2;
                double coordX = currentPosition.X;
                double coordY = currentPosition.Y;
                double deltaX = coordX - lastDragerPoint.X;
                double deltaY = coordY - lastDragerPoint.Y;
                TextBox selectedBubble = ((TextBox)(drager.DataContext));
                double selectedBubbleWidth = selectedBubble.Width;
                double selectedBubbleHeight = selectedBubble.Height;
                Canvas.SetLeft(selectedBubble, coordX - selectedBubbleWidth - halfDragerWidth);
                Canvas.SetTop(selectedBubble, coordY - selectedBubbleHeight - halfDragerHeight);
                Canvas.SetLeft(dragers[3], coordX - halfDragerWidth);
                Canvas.SetTop(dragers[3], coordY - halfDragerHeight);
                try
                {
                    selectedBubble.Width -= deltaX;
                    selectedBubble.Height -= deltaY;
                }
                catch (Exception)
                {

                }
                selectedBubbleWidth = selectedBubble.Width;
                selectedBubbleHeight = selectedBubble.Height;
                Canvas.SetLeft(dragers[0], coordX - selectedBubbleWidth - dragerWidth);
                Canvas.SetTop(dragers[0], coordY - selectedBubbleHeight - halfDragerHeight);
                Canvas.SetLeft(dragers[1], coordX - halfDragerWidth);
                Canvas.SetTop(dragers[1], coordY - selectedBubbleHeight - halfDragerHeight);
                Canvas.SetLeft(dragers[2], coordX - selectedBubbleWidth - dragerWidth);
                Canvas.SetTop(dragers[2], coordY - halfDragerHeight);

                MoveBubbleDirection(currentPosition, coordX, coordY, 0, 0, deltaX, deltaY);

            }
            lastDragerPoint = currentPosition;
        }

        private void ToggleModeHandler(object sender, SelectionChangedEventArgs e)
        {
            TabControl modeControl = ((TabControl)(sender));
            int modeControlIndex = modeControl.SelectedIndex;
            ToggleMode(modeControlIndex);
        }

        public void ToggleMode (int modeControlIndex)
        {
            toolBarControl.SelectedIndex = modeControlIndex;
            bool isManuscript = modeControlIndex == 0;
            RoutedEventArgs eventArgs = new RoutedEventArgs();
            eventArgs.RoutedEvent = Button.ClickEvent;
            if (isManuscript)
            {
                penTool.RaiseEvent(eventArgs);
            }
            else
            {
                pencilTool.RaiseEvent(eventArgs);
            }
            foreach (MenuItem modeControlItem in modeMenu.Items)
            {
                modeControlItem.IsChecked = false;
                object modeData = modeControlItem.DataContext;
                string rawModeIndex = ((string)(modeData));
                int modeIndex = Int32.Parse(rawModeIndex);
                bool isModeFound = modeIndex == modeControlIndex;
                if (isModeFound)
                {
                    modeControlItem.IsChecked = true;
                }
            }
        }

        private void SaveChapterHandler(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Title = "Выберите папку вашего комикса";
            sfd.FileName = "Глава.chapter";
            sfd.DefaultExt = ".chapter";
            sfd.Filter = "Chaper documents (.chapter)|*.chapter";
            bool? res = sfd.ShowDialog();
            bool isSaved = res != false;
            if (isSaved)
            {
                string fullPath = sfd.FileName;
                string storyBoardDescBoxContent = storyBoardDescBox.Text;
                string rawStoryBoardVisualContent = "";
                int storyBoardItemIndex = -1;
                foreach (Polyline storyBoardItem in storyBoard.Children)
                {
                    storyBoardItemIndex++;
                    string rawStoryBoardVisualContentItem = "";
                    if (storyBoardItemIndex >= 1)
                    {
                        rawStoryBoardVisualContentItem += "@";
                    }
                    PointCollection storyBoardItemPoints = storyBoardItem.Points;
                    int countPoints = storyBoardItemPoints.Count;
                    int lastPointIndex = countPoints - 1;
                    int storyBoardItemPointIndex = -1;
                    foreach (Point storyBoardItemPoint in storyBoardItemPoints)
                    {
                        storyBoardItemPointIndex++;
                        int storyBoardItemPointX = ((int)(storyBoardItemPoint.X));
                        int storyBoardItemPointY = ((int)(storyBoardItemPoint.Y));
                        string rawStoryBoardItemPointX = storyBoardItemPointX.ToString();
                        string rawStoryBoardItemPointY = storyBoardItemPointY.ToString();
                        string rawStoryBoardItemPointData = rawStoryBoardItemPointX + ":" + rawStoryBoardItemPointY;
                        rawStoryBoardVisualContentItem += rawStoryBoardItemPointData;
                        if (storyBoardItemPointIndex < lastPointIndex)
                        {
                            rawStoryBoardVisualContentItem += "|";
                        }
                    }
                    rawStoryBoardVisualContent += rawStoryBoardVisualContentItem;
                }

                string rawManuscriptsContent = "";
                foreach (TabItem manuscriptPage in manuscriptPages.Items)
                {
                    ScrollViewer manuscriptScroll = ((ScrollViewer)(manuscriptPage.Content));
                    Canvas someManuscript = ((Canvas)(manuscriptScroll.Content));
                    
                    string rawManuscriptPolylinesContent = "";
                    int manuscriptItemIndex = -1;
                    foreach (UIElement manuscriptItem in someManuscript.Children)
                    {
                        bool isPolyline = manuscriptItem is Polyline;
                        if (isPolyline)
                        {
                            Polyline manuscriptPolylineItem = manuscriptItem as Polyline;
                            manuscriptItemIndex++;
                            string rawManuscriptPolylinesContentItem = "";
                            if (manuscriptItemIndex >= 1)
                            {
                                rawManuscriptPolylinesContentItem += "@";
                            }
                            PointCollection manuscriptItemPoints = manuscriptPolylineItem.Points;
                            int countPoints = manuscriptItemPoints.Count;
                            int lastPointIndex = countPoints - 1;
                            int manuscriptItemPointIndex = -1;
                            foreach (Point manuscriptItemPoint in manuscriptItemPoints)
                            {
                                manuscriptItemPointIndex++;
                                int manuscriptItemPointX = ((int)(manuscriptItemPoint.X));
                                int manuscriptItemPointY = ((int)(manuscriptItemPoint.Y));
                                string rawManuscriptItemPointX = manuscriptItemPointX.ToString();
                                string rawManuscriptItemPointY = manuscriptItemPointY.ToString();
                                string rawManuscriptItemPointData = rawManuscriptItemPointX + ":" + rawManuscriptItemPointY;
                                string rawManuscriptItemBrushData = manuscriptPolylineItem.Stroke.ToString();
                                string rawManuscriptItemData = rawManuscriptItemPointData + ":" + rawManuscriptItemBrushData;
                                rawManuscriptPolylinesContentItem += rawManuscriptItemData;
                                if (manuscriptItemPointIndex < lastPointIndex)
                                {
                                    rawManuscriptPolylinesContentItem += "|";
                                }
                            }
                            rawManuscriptPolylinesContent += rawManuscriptPolylinesContentItem;
                        }
                    }

                    string rawManuscriptPolygonesContent = "";
                    manuscriptItemIndex = -1;
                    foreach (UIElement manuscriptItem in someManuscript.Children)
                    {
                        bool isPolygon = manuscriptItem is Polygon;
                        if (isPolygon)
                        {
                            Polygon manuscriptPolygonItem = manuscriptItem as Polygon;
                            manuscriptItemIndex++;
                            string rawManuscriptPolygonesContentItem = "";
                            bool isNotFirstManuscriptIndex = manuscriptItemIndex >= 1;
                            if (isNotFirstManuscriptIndex)
                            {
                                rawManuscriptPolygonesContentItem += "@";
                            }
                            PointCollection manuscriptItemPoints = manuscriptPolygonItem.Points;
                            int countPoints = manuscriptItemPoints.Count;
                            int lastPointIndex = countPoints - 1;
                            int manuscriptItemPointIndex = -1;
                            foreach (Point manuscriptItemPoint in manuscriptItemPoints)
                            {
                                manuscriptItemPointIndex++;
                                int manuscriptItemPointX = ((int)(manuscriptItemPoint.X));
                                int manuscriptItemPointY = ((int)(manuscriptItemPoint.Y));
                                string rawManuscriptItemPointX = manuscriptItemPointX.ToString();
                                string rawManuscriptItemPointY = manuscriptItemPointY.ToString();
                                string rawManuscriptItemPointData = rawManuscriptItemPointX + ":" + rawManuscriptItemPointY;
                                rawManuscriptPolygonesContentItem += rawManuscriptItemPointData;
                                bool isLTLastManuscriptItemPointIndex = manuscriptItemPointIndex < lastPointIndex;
                                if (isLTLastManuscriptItemPointIndex)
                                {
                                    rawManuscriptPolygonesContentItem += "|";
                                }
                                else
                                {
                                    string rawManuscriptItemBrush = "|";
                                    Brush manuscriptPolygonItemFill = manuscriptPolygonItem.Fill;
                                    if (manuscriptPolygonItemFill is ImageBrush)
                                    {
                                        ImageBrush manuscriptPolygonItemScreenTone = manuscriptPolygonItemFill as ImageBrush;
                                        Uri manuscriptPolygonItemScreenToneSource = ((BitmapImage)(manuscriptPolygonItemScreenTone.ImageSource)).UriSource;
                                        rawManuscriptItemBrush += manuscriptPolygonItemScreenToneSource.ToString();
                                    }
                                    rawManuscriptPolygonesContentItem += rawManuscriptItemBrush;
                                }
                            }
                            rawManuscriptPolygonesContent += rawManuscriptPolygonesContentItem;
                        }
                    }

                    foreach (UIElement manuscriptItem in someManuscript.Children)
                    {
                        bool isCanvas = manuscriptItem is Canvas;
                        if (isCanvas)
                        {
                            Canvas manuscriptCanvasItem = manuscriptItem as Canvas;

                            manuscriptItemIndex = -1;
                            foreach (UIElement manuscriptElement in manuscriptCanvasItem.Children)
                            {
                                bool isPolygon = manuscriptElement is Polygon;
                                if (isPolygon)
                                {
                                    Polygon manuscriptPolygonItem = manuscriptElement as Polygon;
                                    manuscriptItemIndex++;
                                    string rawManuscriptPolygonesContentItem = "";
                                    bool isNotFirstManuscriptIndex = manuscriptItemIndex >= 1;
                                    if (isNotFirstManuscriptIndex)
                                    {
                                        rawManuscriptPolygonesContentItem += "@";
                                    }
                                    PointCollection manuscriptItemPoints = manuscriptPolygonItem.Points;
                                    int countPoints = manuscriptItemPoints.Count;
                                    int lastPointIndex = countPoints - 1;
                                    int manuscriptItemPointIndex = -1;
                                    foreach (Point manuscriptItemPoint in manuscriptItemPoints)
                                    {
                                        manuscriptItemPointIndex++;
                                        int manuscriptItemPointX = ((int)(manuscriptItemPoint.X));
                                        int manuscriptItemPointY = ((int)(manuscriptItemPoint.Y));
                                        string rawManuscriptItemPointX = manuscriptItemPointX.ToString();
                                        string rawManuscriptItemPointY = manuscriptItemPointY.ToString();
                                        string rawManuscriptItemPointData = rawManuscriptItemPointX + ":" + rawManuscriptItemPointY;
                                        rawManuscriptPolygonesContentItem += rawManuscriptItemPointData;
                                        bool isLTLastManuscriptItemPointIndex = manuscriptItemPointIndex < lastPointIndex;
                                        if (isLTLastManuscriptItemPointIndex)
                                        {
                                            rawManuscriptPolygonesContentItem += "|";
                                        }
                                        else
                                        {
                                            string rawManuscriptItemBrush = "|";
                                            Brush manuscriptPolygonItemFill = manuscriptPolygonItem.Fill;
                                            if (manuscriptPolygonItemFill is ImageBrush)
                                            {
                                                ImageBrush manuscriptPolygonItemScreenTone = manuscriptPolygonItemFill as ImageBrush;
                                                Uri manuscriptPolygonItemScreenToneSource = ((BitmapImage)(manuscriptPolygonItemScreenTone.ImageSource)).UriSource;
                                                rawManuscriptItemBrush += manuscriptPolygonItemScreenToneSource.ToString();
                                            }
                                            rawManuscriptPolygonesContentItem += rawManuscriptItemBrush;
                                        }
                                    }
                                    rawManuscriptPolygonesContent += rawManuscriptPolygonesContentItem;
                                }
                            }

                        }
                    }

                    bool isFlashBack = false;
                    isFlashBack = ((bool)(flashBackTool.IsChecked));
                    string rawManuscriptFlashBackContent = isFlashBack.ToString();
                    string rawManuscriptBubblesContent = "";
                    foreach (UIElement manuscriptItem in someManuscript.Children)
                    {
                        bool isTextBox = manuscriptItem is TextBox;
                        if (isTextBox)
                        {
                            TextBox manuscriptTextBoxItem = manuscriptItem as TextBox;
                            manuscriptItemIndex++;
                            string rawManuscriptTextboxesContentItem = "";
                            bool isSecondOrGtManuscriptItemIndex = manuscriptItemIndex >= 1;
                            if (isSecondOrGtManuscriptItemIndex)
                            {
                                rawManuscriptTextboxesContentItem += "@";
                            }
                            int manuscriptItemPointX = ((int)(Canvas.GetLeft(manuscriptTextBoxItem)));
                            int manuscriptItemPointY = ((int)(Canvas.GetTop(manuscriptTextBoxItem)));
                            string rawManuscriptItemPointX = manuscriptItemPointX.ToString();
                            string rawManuscriptItemPointY = manuscriptItemPointY.ToString();
                            string rawManuscriptItemPointData = rawManuscriptItemPointX + "|" + rawManuscriptItemPointY;
                            rawManuscriptTextboxesContentItem += rawManuscriptItemPointData;
                            string rawManuscriptItemTextData = manuscriptTextBoxItem.Text.Replace(System.Environment.NewLine, "~");
                            string rawManuscriptItemWidth = ((int)(manuscriptTextBoxItem.Width)).ToString();
                            string rawManuscriptItemHeight = ((int)(manuscriptTextBoxItem.Height)).ToString();
                            string rawManuscriptItemSizeData = rawManuscriptItemWidth + "|" + rawManuscriptItemHeight;
                            rawManuscriptTextboxesContentItem += "|";
                            rawManuscriptTextboxesContentItem += rawManuscriptItemSizeData;
                            rawManuscriptTextboxesContentItem += "|";
                            rawManuscriptTextboxesContentItem += rawManuscriptItemTextData;
                            string rawManuscriptItemBrush = "|";
                            ImageBrush manuscriptTextBoxItemFill = ((ImageBrush)(manuscriptTextBoxItem.Background));
                            Uri manuscriptTextBoxItemBubbleSource = ((BitmapImage)(manuscriptTextBoxItemFill.ImageSource)).UriSource;
                            rawManuscriptItemBrush += manuscriptTextBoxItemBubbleSource.ToString();
                            rawManuscriptTextboxesContentItem += rawManuscriptItemBrush;
                            object rawManuscriptItemDirectionData = manuscriptTextBoxItem.DataContext;
                            string rawManuscriptItemDirection = "|";
                            bool isDirectionalBubble = rawManuscriptItemDirectionData is System.Windows.Shapes.Path;
                            if (isDirectionalBubble)
                            {
                                System.Windows.Shapes.Path path = ((System.Windows.Shapes.Path)(manuscriptTextBoxItem.DataContext));
                                PathGeometry pathGeometry = ((PathGeometry)(path.Data));
                                bool isPathGeometryExists = pathGeometry != null;
                                if (isPathGeometryExists)
                                {
                                    PathFigureCollection pathFigures = ((PathFigureCollection)(pathGeometry.Figures));
                                    PathFigure pathFigure = ((PathFigure)(pathFigures[0]));
                                    PathSegmentCollection pathSegments = ((PathSegmentCollection)(pathFigure.Segments));
                                    PolyBezierSegment bubbleDirectionSegment = ((PolyBezierSegment)(pathSegments[0]));
                                    Point bubbleDirectionSegmentFirstPoint = bubbleDirectionSegment.Points[0];
                                    Point bubbleDirectionSegmentSecondPoint = bubbleDirectionSegment.Points[1];
                                    Point bubbleDirectionSegmentThirdPoint = bubbleDirectionSegment.Points[2];
                                    int bubbleDirectionSegmentFirstPointX = ((int)(bubbleDirectionSegmentFirstPoint.X));
                                    int bubbleDirectionSegmentFirstPointY = ((int)(bubbleDirectionSegmentFirstPoint.Y));
                                    int bubbleDirectionSegmentSecondPointX = ((int)(bubbleDirectionSegmentSecondPoint.X));
                                    int bubbleDirectionSegmentSecondPointY = ((int)(bubbleDirectionSegmentSecondPoint.Y));
                                    int bubbleDirectionSegmentThirdPointX = ((int)(bubbleDirectionSegmentThirdPoint.X));
                                    int bubbleDirectionSegmentThirdPointY = ((int)(bubbleDirectionSegmentThirdPoint.Y));
                                    rawManuscriptItemDirection += bubbleDirectionSegmentFirstPointX.ToString() + ":" + bubbleDirectionSegmentFirstPointY.ToString() + "&" + bubbleDirectionSegmentSecondPointX.ToString() + ":" + bubbleDirectionSegmentSecondPointY.ToString() + "&" + bubbleDirectionSegmentThirdPointX.ToString() + ":" + bubbleDirectionSegmentThirdPointY.ToString();
                                }
                            }
                            rawManuscriptTextboxesContentItem += rawManuscriptItemDirection;
                            rawManuscriptBubblesContent += rawManuscriptTextboxesContentItem;
                        }
                    }
                    string rawManuscriptLogosContent = "";
                    manuscriptItemIndex = -1;
                    foreach (UIElement manuscriptItem in someManuscript.Children)
                    {
                        bool isImage = manuscriptItem is Image;
                        if (isImage)
                        {
                            manuscriptItemIndex++;
                            Image manuscriptImageItem = manuscriptItem as Image;
                            
                            ImageSource logo = manuscriptImageItem.Source;
                            bool isHaveLogo = logo != null;
                            if (isHaveLogo)
                            {
                                string rawManuscriptImagesContentItem = "";
                                bool isSecondOrGtManuscriptItemIndex = manuscriptItemIndex >= 1;
                                if (isSecondOrGtManuscriptItemIndex)
                                {
                                    rawManuscriptImagesContentItem += "@";
                                }
                                string rawManuscriptImagesContentItemPointData = "";
                                double logoCoordX = Canvas.GetLeft(manuscriptItem);
                                double logoCoordY = Canvas.GetTop(manuscriptItem);
                                int parsedLogoCoordX = ((int)(logoCoordX));
                                int parsedLogoCoordY = ((int)(logoCoordY));
                                string rawLogoCoordX = parsedLogoCoordX.ToString();
                                string rawLogoCoordY = parsedLogoCoordY.ToString();
                                rawManuscriptImagesContentItemPointData += rawLogoCoordX;
                                rawManuscriptImagesContentItemPointData += "|";
                                rawManuscriptImagesContentItemPointData += rawLogoCoordY;
                                rawManuscriptImagesContentItem += rawManuscriptImagesContentItemPointData;
                                rawManuscriptImagesContentItem += "|";

                                string logoSource = logo.ToString();

                                rawManuscriptImagesContentItem += logoSource;
                                rawManuscriptLogosContent += rawManuscriptImagesContentItem;
                            }
                        }
                    }
                    string rawManuscriptContent = rawManuscriptPolygonesContent + "\n" + rawManuscriptPolylinesContent + "\n" + rawManuscriptFlashBackContent + "\n" + rawManuscriptBubblesContent + "\n" + rawManuscriptLogosContent;
                    rawManuscriptsContent += rawManuscriptContent + "\n";
                }
                string chapterRawDataContent = storyBoardDescBoxContent + "\n" + rawStoryBoardVisualContent + "\n" + rawManuscriptsContent;
                File.WriteAllText(fullPath, chapterRawDataContent);
            }
        }

        private void OpenChapterHandler(object sender, RoutedEventArgs e)
        {
            CloseChapter();
            OpenChapter();
            polygon = null;
        }

        public void OpenChapter()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Title = "Выберите главу";
            ofd.Multiselect = true;
            ofd.Filter = "Chapter documents (.chapter)|*.chapter";
            bool? res = ofd.ShowDialog();
            bool isOpened = res != false;
            if (isOpened)
            {
                string fullPath = ofd.FileName;
                string[] rawChapterData = File.ReadAllLines(fullPath);
                int rawChapterDataItemIndex = -1;
                List<int> manuscriptPolygonesContents = new List<int>();
                int manuscriptPolygonesContentsCursor = 2;
                List<int> manuscriptPolylinesContents = new List<int>();
                int manuscriptPolylinesContentsCursor = 3;
                List<int> manuscriptFlashBackContents = new List<int>();
                int manuscriptFlashBackContentsCursor = 4;
                List<int> manuscriptBubblesContents = new List<int>();
                int manuscriptBubblesContentsCursor = 5;
                List<int> manuscriptLogosContents = new List<int>();
                int manuscriptLogosContentsCursor = 6;
                int rawChapterDataLength = rawChapterData.Length;
                for (int i = 0; i < rawChapterDataLength; i++)
                {
                    bool isManuscriptPolygonesContentsCursor = i == manuscriptPolygonesContentsCursor;
                    bool isManuscriptPolylinesContentsCursor = i == manuscriptPolylinesContentsCursor;
                    bool isManuscriptFlashBacksContentsCursor = i == manuscriptFlashBackContentsCursor;
                    bool isManuscriptBubblesContentsCursor = i == manuscriptBubblesContentsCursor;
                    bool isManuscriptLogosContentsCursor = i == manuscriptLogosContentsCursor;
                    if (isManuscriptPolygonesContentsCursor)
                    {
                        manuscriptPolygonesContents.Add(i);
                        manuscriptPolygonesContentsCursor += 5;
                    }
                    else if (isManuscriptPolylinesContentsCursor)
                    {
                        manuscriptPolylinesContents.Add(i);
                        manuscriptPolylinesContentsCursor += 5;
                    }
                    else if (isManuscriptFlashBacksContentsCursor)
                    {
                        manuscriptFlashBackContents.Add(i);
                        manuscriptFlashBackContentsCursor += 5;
                    }
                    else if (isManuscriptBubblesContentsCursor)
                    {
                        manuscriptBubblesContents.Add(i);
                        manuscriptBubblesContentsCursor += 5;
                    }
                    else if (isManuscriptLogosContentsCursor)
                    {
                        manuscriptLogosContents.Add(i);
                        manuscriptLogosContentsCursor += 5;
                    }
                }

                /*
                 * Неправильное удаление
                ItemCollection manuscriptPagesItems = manuscriptPages.Items;
                int countManuscriptPages = manuscriptPagesItems.Count;
                int countRemovedPages = countManuscriptPages - 1;
                for (int i = 1; i < countRemovedPages; i++)
                {
                    manuscriptPages.Items.RemoveAt(i);
                    pages.Children.RemoveAt(i);
                }
                */

                foreach (string rawChapterDataItem in rawChapterData)
                {
                    rawChapterDataItemIndex++;
                    bool isStoryboardDescContent = rawChapterDataItemIndex == 0;
                    bool isStoryboardVisualContent = rawChapterDataItemIndex == 1;
                    bool isManuscriptPolygonesContent = manuscriptPolygonesContents.Contains(rawChapterDataItemIndex);
                    bool isManuscriptPolylinesContent = manuscriptPolylinesContents.Contains(rawChapterDataItemIndex);
                    bool isManuscriptFlashBackContent = manuscriptFlashBackContents.Contains(rawChapterDataItemIndex);
                    bool isManuscriptBubblesContent = manuscriptBubblesContents.Contains(rawChapterDataItemIndex);
                    bool isManuscriptLogosContent = manuscriptLogosContents.Contains(rawChapterDataItemIndex);
                    if (isStoryboardDescContent)
                    {
                        storyBoardDescBox.Text = rawChapterDataItem;
                    }
                    else if (isStoryboardVisualContent)
                    {
                        string formatedRawChapterDataItem = rawChapterDataItem;
                        bool isErrorsDetected = formatedRawChapterDataItem.Contains("@@");
                        if (isErrorsDetected)
                        {
                            formatedRawChapterDataItem = rawChapterDataItem.Replace("@@", "@");
                        }
                        string[] rawLines = formatedRawChapterDataItem.Split(new Char[] { '@' });
                        int countRawLines = rawLines.Length;
                        bool isRawDataDetected = formatedRawChapterDataItem.Length >= 2;
                        if (isRawDataDetected)
                        {
                            foreach (string rawLine in rawLines)
                            {
                                Polyline line = new Polyline();
                                string[] rawLinePoints = rawLine.Split(new Char[] { '|' });
                                foreach (string rawLinePoint in rawLinePoints)
                                {
                                    string[] rawLinePointCoords = rawLinePoint.Split(new Char[] { ':' });
                                    string rawLinePointXCoord = rawLinePointCoords[0];
                                    string rawLinePointYCoord = rawLinePointCoords[1];
                                    int linePointXCoordDouble = 0;
                                    int linePointYCoordDouble = 0;
                                    bool isGetXCoord = Int32.TryParse(rawLinePointXCoord, out linePointXCoordDouble);
                                    bool isGetYCoord = Int32.TryParse(rawLinePointYCoord, out linePointYCoordDouble);
                                    Point linePoint = new Point(linePointXCoordDouble, linePointYCoordDouble);
                                    if (isGetXCoord && isGetYCoord)
                                    {
                                        line.Points.Add(linePoint);
                                    }
                                }
                                line.Stroke = System.Windows.Media.Brushes.Black;
                                storyBoard.Children.Add(line);
                            }
                        }
                    }
                    else if (isManuscriptPolygonesContent)
                    {
                        bool isOtherPages = rawChapterDataItemIndex >= 7;
                        if (isOtherPages)
                        {
                            AddPage();
                        }

                        string formatedRawChapterDataItem = rawChapterDataItem;
                        bool isErrorsDetected = formatedRawChapterDataItem.Contains("@@");
                        if (isErrorsDetected)
                        {
                            formatedRawChapterDataItem = rawChapterDataItem.Replace("@@", "@");
                        }
                        string[] rawLines = formatedRawChapterDataItem.Split(new Char[] { '@' });
                        int countRawLines = rawLines.Length;
                        bool isRawDataDetected = countRawLines >= 1;
                        if (isRawDataDetected)
                        {
                            foreach (string rawLine in rawLines)
                            {
                                string[] rawLinePoints = rawLine.Split(new Char[] { '|' });
                                int countFramePoints = rawLinePoints.Length;
                                bool isFrameValid = countFramePoints >= 3;
                                if (isFrameValid)
                                {
                                    int rawLinePointIndex = -1;
                                    Polygon line = new Polygon();
                                    foreach (string rawLinePoint in rawLinePoints)
                                    {
                                        rawLinePointIndex++;
                                        if (rawLinePointIndex < rawLinePoints.Length - 1)
                                        {
                                            string[] rawLinePointCoords = rawLinePoint.Split(new Char[] { ':' });
                                            string rawLinePointXCoord = rawLinePointCoords[0];
                                            string rawLinePointYCoord = rawLinePointCoords[1];
                                            string rawLineSource = rawLinePointCoords[1];
                                            int linePointXCoordDouble = 0;
                                            int linePointYCoordDouble = 0;
                                            bool isGetXCoord = Int32.TryParse(rawLinePointXCoord, out linePointXCoordDouble);
                                            bool isGetYCoord = Int32.TryParse(rawLinePointYCoord, out linePointYCoordDouble);
                                            Point linePoint = new Point(linePointXCoordDouble, linePointYCoordDouble);
                                            if (isGetXCoord && isGetYCoord)
                                            {
                                                line.Points.Add(linePoint);
                                            }
                                        }
                                    }
                                    line.Stroke = System.Windows.Media.Brushes.Black;
                                    int selectedScreenTonesBoxItemIndex = screenTonesBox.SelectedIndex;
                                    ItemCollection screenTonesBoxItems = screenTonesBox.Items;
                                    ComboBoxItem selectedScreenTonesBoxItem = ((ComboBoxItem)(screenTonesBoxItems[selectedScreenTonesBoxItemIndex]));
                                    string rawSource = rawLinePoints[rawLinePoints.Length - 1];
                                    if (rawSource.Length >= 1)
                                    {
                                        int index = -1;
                                        foreach (ComboBoxItem screenTonesBoxItem in screenTonesBox.Items)
                                        {
                                            index++;
                                            Image currentImage = ((Image)(screenTonesBoxItem.Content));
                                            BitmapImage currentImageSource = ((BitmapImage)(currentImage.Source));
                                            Uri currentImageSourceUri = currentImageSource.UriSource;
                                            string rawCurrentImageSourceUri = currentImageSourceUri.ToString();
                                            bool isSourcesMatches = rawCurrentImageSourceUri == rawSource;
                                            if (isSourcesMatches) {
                                                selectedScreenTonesBoxItemIndex = index;
                                                break;
                                            }
                                        }
                                        bool isSourceDetected = selectedScreenTonesBoxItemIndex >= 0;
                                        if (isSourceDetected)
                                        {
                                            var selectedItem = screenTonesBoxItems[selectedScreenTonesBoxItemIndex];
                                            selectedScreenTonesBoxItem = ((ComboBoxItem)(selectedItem));
                                            var selectedScreenTonesBoxItemContent = selectedScreenTonesBoxItem.Content;
                                            Image selectedScreenTonesBoxItemImgContent = ((Image)(selectedScreenTonesBoxItemContent));
                                            ImageSource imageSource = selectedScreenTonesBoxItemImgContent.Source;
                                            ImageBrush frameBrush = new ImageBrush(imageSource);
                                            line.Fill = frameBrush;
                                        }
                                        else
                                        {
                                            Brush frameBrush = System.Windows.Media.Brushes.White;
                                            line.Fill = frameBrush;
                                        }
                                    
                                    }
                                    else
                                    {
                                        line.Fill = System.Windows.Media.Brushes.White;
                                    }
                                    int manuscriptIndex = 0;
                                    manuscriptIndex = manuscriptPolygonesContents.IndexOf(rawChapterDataItemIndex);
                                    ((Canvas)(((ScrollViewer)(((TabItem)(manuscriptPages.Items[manuscriptIndex])).Content)).Content)).Children.Add(line);
                                    line.MouseLeftButtonUp += SelectFrameHandler;
                                }
                            }
                        }
                    }
                    else if (isManuscriptPolylinesContent)
                    {
                        string formatedRawChapterDataItem = rawChapterDataItem;
                        bool isErrorsDetected = formatedRawChapterDataItem.Contains("@@");
                        if (isErrorsDetected)
                        {
                            formatedRawChapterDataItem = rawChapterDataItem.Replace("@@", "@");
                        }
                        string[] rawLines = formatedRawChapterDataItem.Split(new Char[] { '@' });
                        int countRawLines = rawLines.Length;
                        // bool isRawDataDetected = countRawLines >= 1;
                        bool isRawDataDetected = true;
                        if (isRawDataDetected)
                        {
                            foreach (string rawLine in rawLines)
                            {
                                if (rawLine.Length >= 2)
                                {
                                    Polyline line = new Polyline();
                                    string[] rawLinePoints = rawLine.Split(new Char[] { '|' });
                                    string rawLineBrush = "Black";
                                    foreach (string rawLinePoint in rawLinePoints)
                                    {
                                        string[] rawLinePointCoords = rawLinePoint.Split(new Char[] { ':' });
                                        string rawLinePointXCoord = rawLinePointCoords[0];
                                        string rawLinePointYCoord = rawLinePointCoords[1];
                                        rawLineBrush = rawLinePointCoords[2];
                                        int linePointXCoordDouble = 0;
                                        int linePointYCoordDouble = 0;
                                        bool isGetXCoord = Int32.TryParse(rawLinePointXCoord, out linePointXCoordDouble);
                                        bool isGetYCoord = Int32.TryParse(rawLinePointYCoord, out linePointYCoordDouble);
                                        Point linePoint = new Point(linePointXCoordDouble, linePointYCoordDouble);
                                        if (isGetXCoord && isGetYCoord)
                                        {
                                            line.Points.Add(linePoint);
                                        }
                                    }
                                    line.Stroke = System.Windows.Media.Brushes.Black;
                                    BrushConverter brushConverter = new BrushConverter();
                                    Brush brush = ((Brush)(brushConverter.ConvertFrom(rawLineBrush)));
                                    line.Stroke = brush;
                                    bool isNotWhiteBrush = rawLineBrush != "#FFFFFFFF";
                                    bool isNotBlackBrush = rawLineBrush != "#FF000000";
                                    bool isMarker = isNotWhiteBrush && isNotBlackBrush;
                                    if (isMarker)
                                    {
                                        line.StrokeThickness = markerThickness;
                                    }
                                    
                                    int manuscriptIndex = 0;
                                    manuscriptIndex = manuscriptPolylinesContents.IndexOf(rawChapterDataItemIndex);
                                    ((Canvas)(((ScrollViewer)(((TabItem)(manuscriptPages.Items[manuscriptIndex])).Content)).Content)).Children.Add(line);
                                
                                }
                            }
                        }
                    }
                    else if (isManuscriptFlashBackContent)
                    {
                        bool isFlashBack = false;
                        isFlashBack = Boolean.Parse(rawChapterDataItem);
                        if (isFlashBack)
                        {
                            
                            int manuscriptIndex = 0;
                            manuscriptIndex = manuscriptFlashBackContents.IndexOf(rawChapterDataItemIndex);
                            ((Canvas)(((ScrollViewer)(((TabItem)(manuscriptPages.Items[manuscriptIndex])).Content)).Content)).Background = System.Windows.Media.Brushes.Black;
                            
                            flashBackTool.IsChecked = true;
                        }

                    }
                    else if (isManuscriptBubblesContent)
                    {
                        string formatedRawChapterDataItem = rawChapterDataItem;
                        bool isErrorsDetected = formatedRawChapterDataItem.Contains("@@");
                        if (isErrorsDetected)
                        {
                            formatedRawChapterDataItem = rawChapterDataItem.Replace("@@", "@");
                        }
                        string[] rawBubbles = formatedRawChapterDataItem.Split(new Char[] { '@' });
                        int countRawBubbles = rawBubbles.Length;
                        bool isRawDataDetected = countRawBubbles >= 1;
                        if (isRawDataDetected)
                        {
                            foreach (string rawBubble in rawBubbles)
                            {
                                TextBox bubble = new TextBox();
                                bubble.TextWrapping = TextWrapping.Wrap;
                                bubble.BorderThickness = new Thickness(0);
                                bubble.AcceptsReturn = true;
                                bubble.TextAlignment = TextAlignment.Center;
                                bubble.VerticalContentAlignment = VerticalAlignment.Center;
                                bubble.VerticalAlignment = VerticalAlignment.Center;
                                ImageBrush bubbleBrush = new ImageBrush();
                                bubble.Background = bubbleBrush;
                                string[] rawBubbleData = rawBubble.Split(new Char[] { '|' });
                                if (rawBubbleData.Length >= 2)
                                {
                                    string rawBubbleDataXCoord = rawBubbleData[0];
                                    string rawBubbleDataYCoord = rawBubbleData[1];
                                    string rawBubbleDataWidth = rawBubbleData[2];
                                    string rawBubbleDataHeight = rawBubbleData[3];
                                    string rawBubbleDataText = rawBubbleData[4].Replace("~", System.Environment.NewLine);
                                    string rawSource = rawBubbleData[5];
                                    string rawDirection = rawBubbleData[6];
                                    int bubbleXCoordDouble = 0;
                                    int bubbleYCoordDouble = 0;
                                    int bubbleWidth = 0;
                                    int bubbleHeight = 0;
                                    bool isGetXCoord = Int32.TryParse(rawBubbleDataXCoord, out bubbleXCoordDouble);
                                    bool isGetYCoord = Int32.TryParse(rawBubbleDataYCoord, out bubbleYCoordDouble);
                                    bool isGetWidth = Int32.TryParse(rawBubbleDataWidth, out bubbleWidth);
                                    bool isGetHeight = Int32.TryParse(rawBubbleDataHeight, out bubbleHeight);
                                    bubble.Text = rawBubbleDataText;
                                    manuscript.Children.Add(bubble);
                                    if (isGetXCoord && isGetYCoord && isGetWidth && isGetHeight)
                                    {
                                        Canvas.SetLeft(bubble, bubbleXCoordDouble);
                                        Canvas.SetTop(bubble, bubbleYCoordDouble);
                                        bubble.Width = bubbleWidth;
                                        bubble.Height = bubbleHeight;
                                        bubble.Padding = new Thickness(35);
                                    }
                                    else
                                    {
                                        Canvas.SetLeft(bubble, 0);
                                        Canvas.SetTop(bubble, 0);
                                        bubble.Width = 150;
                                        bubble.Height = 150;
                                    }
                                    ItemCollection bubblesBoxItems = bubblesBox.Items;
                                    int selectedScreenTonesBoxItemIndex = 0;
                                    int index = -1;
                                    foreach (ComboBoxItem bubblesBoxItem in bubblesBoxItems)
                                    {
                                        index++;
                                        Image currentImage = ((Image)(bubblesBoxItem.Content));
                                        BitmapImage currentImageSource = ((BitmapImage)(currentImage.Source));
                                        Uri currentImageSourceUri = currentImageSource.UriSource;
                                        string rawCurrentImageSourceUri = currentImageSourceUri.ToString();
                                        bool isSourcesMatches = rawCurrentImageSourceUri == rawSource;
                                        if (isSourcesMatches)
                                        {
                                            selectedScreenTonesBoxItemIndex = index;
                                            break;
                                        }
                                    }
                                    var selectedItem = bubblesBoxItems[selectedScreenTonesBoxItemIndex];
                                    ComboBoxItem selectedBubblesBoxItem = ((ComboBoxItem)(selectedItem));
                                    var selectedBubblesBoxItemContent = selectedBubblesBoxItem.Content;
                                    Image selectedBubblesBoxItemImgContent = ((Image)(selectedBubblesBoxItemContent));
                                    ImageSource imageSource = selectedBubblesBoxItemImgContent.Source;
                                    ImageBrush frameBrush = new ImageBrush(imageSource);
                                    bubble.Background = frameBrush;

                                    bubble.MouseLeftButtonUp += SelectBubbleHandler;
                                    bubble.LostKeyboardFocus += BubbleLostFocusHandler;
                                    bool isPointsExists = rawDirection.Length >= 2;
                                    if (isPointsExists)
                                    {
                                        if (isPointsExists)
                                        {
                                            System.Windows.Shapes.Path directionPath = new System.Windows.Shapes.Path();
                                            PathGeometry pathGeometry = new PathGeometry();
                                            PathFigureCollection pathFigureCollection = new PathFigureCollection();
                                            PathFigure pathFigure = new PathFigure();
                                            PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();
                                            pathFigure.Segments = pathSegmentCollection;
                                            
                                            string[] rawPoints = rawDirection.Split(new Char[] { '&' });
                                            string[] firstRawPointCoords = rawPoints[0].Split(new Char[] { ':' });
                                            string firstRawPointXCoord = firstRawPointCoords[0];
                                            string firstRawPointYCoord = firstRawPointCoords[1];
                                            string[] secondRawPointCoords = rawPoints[1].Split(new Char[] { ':' });
                                            string secondRawPointXCoord = secondRawPointCoords[0];
                                            string secondRawPointYCoord = secondRawPointCoords[1];
                                            string[] thirdRawPointCoords = rawPoints[2].Split(new Char[] { ':' });
                                            string thirdRawPointXCoord = thirdRawPointCoords[0];
                                            string thirdRawPointYCoord = thirdRawPointCoords[1];

                                            int firstPointXCoord = Int32.Parse(firstRawPointXCoord);
                                            int firstPointYCoord = Int32.Parse(firstRawPointYCoord);
                                            int secondPointXCoord = Int32.Parse(secondRawPointXCoord);
                                            int secondPointYCoord = Int32.Parse(secondRawPointYCoord);
                                            int thirdPointXCoord = Int32.Parse(thirdRawPointXCoord);
                                            int thirdPointYCoord = Int32.Parse(thirdRawPointYCoord);
                                            Point startBubbleDirectionPoint = new Point(firstPointXCoord, firstPointYCoord);
                                            Point currentPosition = new Point(secondPointXCoord, secondPointYCoord);
                                            Point endBubbleDirectionPoint = new Point(thirdPointXCoord, thirdPointYCoord);

                                            pathFigure.StartPoint = startBubbleDirectionPoint;
                                            pathFigureCollection.Add(pathFigure);
                                            pathGeometry.Figures = pathFigureCollection;
                                            directionPath.Data = pathGeometry;
                                            PolyBezierSegment mockSegment = new PolyBezierSegment();
                                            mockSegment.IsStroked = false;
                                            mockSegment.Points.Add(startBubbleDirectionPoint);
                                            mockSegment.Points.Add(endBubbleDirectionPoint);
                                            segment = new PolyBezierSegment();
                                            segment.Points.Add(startBubbleDirectionPoint);
                                            segment.Points.Add(currentPosition);
                                            segment.Points.Add(endBubbleDirectionPoint);

                                            segment.IsSmoothJoin = true;
                                            pathSegmentCollection.Add(segment);
                                            pathSegmentCollection.Add(mockSegment);
                                            
                                            int manuscriptIndex = 0;
                                            manuscriptIndex = manuscriptBubblesContents.IndexOf(rawChapterDataItemIndex);
                                            ((Canvas)(((ScrollViewer)(((TabItem)(manuscriptPages.Items[manuscriptIndex])).Content)).Content)).Children.Add(directionPath);
                                            
                                            directionPath.Stroke = System.Windows.Media.Brushes.Black;
                                            directionPath.Fill = System.Windows.Media.Brushes.White;
                                            bubble.DataContext = ((System.Windows.Shapes.Path)(directionPath));

                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (isManuscriptLogosContent)
                    {
                        string formatedRawChapterDataItem = rawChapterDataItem;
                        bool isErrorsDetected = formatedRawChapterDataItem.Contains("@@");
                        if (isErrorsDetected)
                        {
                            formatedRawChapterDataItem = rawChapterDataItem.Replace("@@", "@");
                        }
                        string[] rawLogos = formatedRawChapterDataItem.Split(new Char[] { '@' });
                        int countRawLogos = rawLogos.Length;
                        bool isRawDataDetected = countRawLogos >= 1;
                        if (isRawDataDetected)
                        {
                            foreach (string rawLogo in rawLogos)
                            {
                                Image logo = new Image();
                                string[] rawLogoData = rawLogo.Split(new Char[] { '|' });
                                bool isDataDetected = rawLogoData.Length >= 2;
                                if (isDataDetected)
                                {
                                    string rawLogoDataXCoord = rawLogoData[0];
                                    string rawLogoDataYCoord = rawLogoData[1];
                                    string rawLogoSource = rawLogoData[2];
                                    int logoXCoordDouble = 0;
                                    int logoYCoordDouble = 0;
                                    bool isGetXCoord = Int32.TryParse(rawLogoDataXCoord, out logoXCoordDouble);
                                    bool isGetYCoord = Int32.TryParse(rawLogoDataYCoord, out logoYCoordDouble);
                                    bool isCoordsDetected = isGetXCoord && isGetYCoord;
                                    if (isCoordsDetected)
                                    {
                                        Canvas.SetLeft(logo, logoXCoordDouble);
                                        Canvas.SetTop(logo, logoYCoordDouble);
                                    }
                                    else
                                    {
                                        Canvas.SetLeft(logo, 0);
                                        Canvas.SetTop(logo, 0);
                                    }
                                    logo.BeginInit();
                                    logo.Source = new BitmapImage(new Uri(rawLogoSource));
                                    logo.EndInit();
                                }
                                logo.Width = 100;
                                logo.Height = 100;
                                manuscript.Children.Add(logo);
                                logo.MouseLeftButtonDown += SelectLogoHandler;
                            }
                        }
                    }
                }
            }
            RefreshThumbnails();
        }

        public RoutedCommand UndoHandler ()
        {
            return new RoutedCommand();
        }

        private void SelectPageHandler(object sender, MouseButtonEventArgs e)
        {
            Canvas page = ((Canvas)(sender));
            SelectPage(page);
        }

        public void SelectPage(Canvas page)
        {
            int pageIndex = pages.Children.IndexOf(page);
            UnselectPages();
            page.Background = System.Windows.Media.Brushes.LightSkyBlue;
            currentPageIndex = pageIndex;
            manuscriptPages.SelectedIndex = currentPageIndex;
            TabItem tab = ((TabItem)(manuscriptPages.Items[currentPageIndex]));
            ScrollViewer manuscriptScroll = ((ScrollViewer)(tab.Content));
            manuscript = ((Canvas)(manuscriptScroll.Content));
            pageBackBtn.Foreground = System.Windows.Media.Brushes.Black;
            pageNextBtn.Foreground = System.Windows.Media.Brushes.Black;
            bool isFirstPage = currentPageIndex == 0;
            int countPages = manuscriptPages.Items.Count;
            int lastPageIndex = countPages - 1;
            bool isLastPage = currentPageIndex == lastPageIndex;
            if (isFirstPage)
            {
                pageBackBtn.Foreground = System.Windows.Media.Brushes.LightGray;
            }
            else if (isLastPage)
            {
                pageNextBtn.Foreground = System.Windows.Media.Brushes.LightGray;
            }
        }

        public void UnselectPages()
        {
            foreach (Canvas currentPage in pages.Children)
            {
                currentPage.Background = System.Windows.Media.Brushes.White;
            }
        }

        private void SelectPreviousPageHandler(object sender, MouseButtonEventArgs e)
        {
            bool isCanSwitchPage = currentPageIndex >= 1;
            if (isCanSwitchPage)
            {
                currentPageIndex--;
                Canvas currentPage = ((Canvas)(pages.Children[currentPageIndex]));
                SelectPage(currentPage);
            }
        }

        private void SelectNextPageHandler(object sender, MouseButtonEventArgs e)
        {
            int countPages = pages.Children.Count;
            int lastPageIndex = countPages - 1;
            bool isCanSwitchPage = currentPageIndex < lastPageIndex;
            if (isCanSwitchPage)
            {
                currentPageIndex++;
                Canvas currentPage = ((Canvas)(pages.Children[currentPageIndex]));
                SelectPage(currentPage);
            }
        }

        private void AddPageHandler(object sender, MouseButtonEventArgs e)
        {
            AddPage();
        }

        public void AddPage()
        {
            UnselectPages();
            Canvas page = new Canvas();
            page.Width = 40;
            page.Height = 65;
            page.Background = System.Windows.Media.Brushes.LightSkyBlue;
            page.Margin = new Thickness(5);
            pages.Children.Add(page);
            page.MouseLeftButtonDown += SelectPageHandler;
            Image pagePreview = new Image();
            pagePreview.Width = 40;
            pagePreview.Height = 65;
            page.Children.Add(pagePreview);
            int countPages = pages.Children.Count;
            int lastPageIndex = countPages - 1;
            currentPageIndex = lastPageIndex;
            TabItem tab = new TabItem();
            tab.Visibility = Visibility.Collapsed;
            manuscriptPages.Items.Add(tab);
            manuscriptPages.SelectedIndex = currentPageIndex;
            manuscript = new Canvas();
            ScrollViewer manuscriptScroll = new ScrollViewer();
            tab.Content = manuscriptScroll;
            manuscriptScroll.Content = manuscript;
            manuscript.Background = System.Windows.Media.Brushes.White;
            manuscript.Cursor = Cursors.Pen;
            manuscript.ClipToBounds = true;
            manuscript.MouseLeftButtonDown += TouchDownHandler;
            manuscript.MouseMove += TouchMoveHandler;
            manuscript.MouseLeftButtonUp += TouchUpHandler;
            ContextMenu pageContextMenu = new ContextMenu();
            MenuItem pageContextMenuItem = new MenuItem();
            pageContextMenuItem.Header = "Удалить";
            pageContextMenuItem.DataContext = ((int)(currentPageIndex));
            pageContextMenuItem.Click += RemovePageHandler;
            pageContextMenu.Items.Add(pageContextMenuItem);
            page.ContextMenu = pageContextMenu;
            pageBackBtn.Foreground = System.Windows.Media.Brushes.Black;
            pageNextBtn.Foreground = System.Windows.Media.Brushes.LightGray;
        }

        public void RemovePageHandler(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = ((MenuItem)(sender));
            object pageData = menuItem.DataContext;
            int index = ((int)(pageData));
            RemovePage(index);
        }

        public void RemovePage(int index)
        {
            manuscriptPages.Items.RemoveAt(index);
            pages.Children.RemoveAt(index);
            foreach (Canvas page in pages.Children)
            {
                int pageIndex = pages.Children.IndexOf(page);
                bool  isOtherPages = pageIndex >= 1;
                if (isOtherPages)
                {
                    ContextMenu pageContextMenu = page.ContextMenu;
                    MenuItem removePageContextMenuItem = ((MenuItem)(pageContextMenu.Items[0]));
                    removePageContextMenuItem.DataContext = ((int)(pageIndex));
                }
            }
            Canvas firstManuscript = ((Canvas)(pages.Children[0]));
            SelectPage(firstManuscript);
        }

        private void CloseChapterHandler(object sender, RoutedEventArgs e)
        {
            CloseChapter();
        }

        public void CloseChapter()
        {

            history.Clear();

            ItemCollection manuscriptPagesItems = manuscriptPages.Items;
            int countManuscriptPages = manuscriptPagesItems.Count;
            bool isCanClose = countManuscriptPages >= 2;
            if (isCanClose)
            {
                int countRemovedPages = countManuscriptPages - 1;
                for (int i = countRemovedPages; i > 0; i--)
                {
                    manuscriptPages.Items.RemoveAt(i);
                    pages.Children.RemoveAt(i);
                }
            }
            Canvas firstManuscript = ((Canvas)(pages.Children[0]));
            SelectPage(firstManuscript);
            manuscript.Children.Clear();
        }

        private void SelectToolFromMenuHandler(object sender, RoutedEventArgs e)
        {
            MenuItem tool = ((MenuItem)(sender));
            object toolData = tool.DataContext;
            string toolName = ((string)(toolData));
            SelectTool(toolName);
        }

        private void ToggleModeFromMenuHandler(object sender, RoutedEventArgs e)
        {
            MenuItem mode = ((MenuItem)(sender));
            object modeData = mode.DataContext;
            string rawModeIndex = ((string)(modeData));
            int modeIndex = Int32.Parse(rawModeIndex);
            modeControl.SelectedIndex = modeIndex;
        }

        private void ToggleStoryBoardModeFromMenuHandler(object sender, RoutedEventArgs e)
        {
            MenuItem mode = ((MenuItem)(sender));
            object modeData = mode.DataContext;
            string rawModeIndex = ((string)(modeData));
            int modeIndex = Int32.Parse(rawModeIndex);
            storyBoardModeControl.SelectedIndex = modeIndex;
        }

        private void ToggleStoryBoardModeHandler(object sender, SelectionChangedEventArgs e)
        {
            TabControl modeControl = ((TabControl)(sender));
            int modeControlIndex = modeControl.SelectedIndex;
            ToggleStoryBoardMode(modeControlIndex);
        }

        public void ToggleStoryBoardMode(int modeControlIndex)
        {
            toolBarControl.SelectedIndex = modeControlIndex;
            RoutedEventArgs eventArgs = new RoutedEventArgs();
            eventArgs.RoutedEvent = Button.ClickEvent;
            foreach (MenuItem modeControlItem in storyBoardModeMenu.Items)
            {
                modeControlItem.IsChecked = false;
                object modeData = modeControlItem.DataContext;
                string rawModeIndex = ((string)(modeData));
                int modeIndex = Int32.Parse(rawModeIndex);
                bool isModeFound = modeIndex == modeControlIndex;
                if (isModeFound)
                {
                    modeControlItem.IsChecked = true;
                }
            }
        }

    }
}