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

namespace ComicsManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
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
        public SpeechSynthesizer debugger;

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
                int effectItems = 15;
                for (int i = 0; i < effectItems; i++)
                {
                    Line effectItem = new Line();
                    manuscript.Children.Add(effectItem);
                    effectItem.X1 = 100;
                    effectItem.Y1 = 0;
                    effectItem.X2 = 100;
                    effectItem.Y2 = 100;
                    RotateTransform rotateTransform = new RotateTransform();
                    // rotateTransform.Angle = 360 / effectItems * i;
                    rotateTransform.Angle = 25 * i;
                    rotateTransform.CenterX = currentPosition.X;
                    rotateTransform.CenterY = currentPosition.Y;
                    effectItem.RenderTransform = rotateTransform;
                    effectItem.Stroke = System.Windows.Media.Brushes.Black;

                }
            }
            else if (isBubbleTool)
            {
                bubble.Focus();
            }
        }

        private void GlobalHotKeyHandler(object sender, KeyEventArgs e)
        {
            Key currentKey = e.Key;
            Key escKey = Key.Escape;
            bool isEscKey = currentKey == escKey;
            if (isEscKey)
            {
                ResetFrameBorders();
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
            Button tool = ((Button)(sender));
            object toolData = tool.DataContext;
            string toolName = ((string)(toolData));
            SelectTool(toolName);
        }

        public void SelectTool(string tool)
        {
            activeTool = tool;
            HideBubbleDragers();
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
            bool isPenTool = activeTool == "Рисование пером";
            bool isBubbleTool = activeTool == "Добавить бабл";
            bool isMarkerTool = activeTool == "Раскрасить маркером";
            bool isWhitewashTool = activeTool == "Замазать белилами";
            bool isPencilTool = activeTool == "Рисовать раскадровку";
            // bool isPencilTool = true;
            if (isPenTool)
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
                Transform bubbleBrushTransform = new ScaleTransform();
                bubbleBrush.Transform = bubbleBrushTransform;
                bubble.Background = bubbleBrush;
                bubble.Width = 150;
                bubble.Height = 150;
                bubble.Padding = new Thickness(35);
                manuscript.Children.Add(bubble);
                bubble.MouseLeftButtonUp += SelectBubbleHandler;
                bubble.LostKeyboardFocus += BubbleLostFocusHandler;
            }
            else if (isMarkerTool)
            {
                sketch = new Polyline();
                PointCollection pointCollection = new PointCollection();
                sketch.Points = pointCollection;
                sketch.Stroke = new SolidColorBrush(markerColorPicker.SelectedColor.Value);
                sketch.StrokeThickness = 10.0;
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
                    // debugger.Speak("Добавляю точку");
                }
            }

        }

        private void BubbleLostFocusHandler(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox selectedBubble = ((TextBox)(sender));
            selectedBubble.IsReadOnly = true;
            selectedBubble.BorderBrush = System.Windows.Media.Brushes.Transparent;
            selectedBubble.BorderThickness = new Thickness(0.0);
            /*dragers[0].Fill = System.Windows.Media.Brushes.Transparent;
            dragers[1].Fill = System.Windows.Media.Brushes.Transparent;
            dragers[2].Fill = System.Windows.Media.Brushes.Transparent;
            dragers[3].Fill = System.Windows.Media.Brushes.Transparent;*/

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
                // bool isPencilTool = true;
                if (isPenTool)
                {
                    PointCollection pointCollection = sketch.Points;
                    pointCollection.Add(currentPosition);
                    sketch.Points = pointCollection;
                }
                else if (isBubbleTool)
                {
                    Canvas.SetLeft(bubble, coordX);
                    Canvas.SetTop(bubble, coordY);
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
            }

        }

        public void ApplyScreenTones()
        {
            int selectedScreenTonesBoxItemIndex = screenTonesBox.SelectedIndex;
            ItemCollection screenTonesBoxItems = screenTonesBox.Items;
            ComboBoxItem selectedScreenTonesBoxItem = ((ComboBoxItem)(screenTonesBoxItems[selectedScreenTonesBoxItemIndex]));
            ImageSource imageSource = ((Image)(selectedScreenTonesBoxItem.Content)).Source;
            ImageBrush frameBrush = new ImageBrush(imageSource);
            screenToneFrame.Fill = frameBrush;
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
            screenToneFrame.Fill = System.Windows.Media.Brushes.White;
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
            }
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
        }

        public void LeftTopDragBubbleHandler(object sender, MouseEventArgs e)
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
            }
            lastDragerPoint = currentPosition;
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
            }
            lastDragerPoint = currentPosition;
        }

        private void ToggleModeHandler(object sender, SelectionChangedEventArgs e)
        {
            TabControl modeControl = ((TabControl)(sender));
            int modeControlIndex = modeControl.SelectedIndex;
            toolBarControl.SelectedIndex = modeControlIndex;
        }

        private void SaveChapterHandler(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Title = "Выберите папку вашего комикса";
            sfd.FileName = "Глава.chapter";
            sfd.DefaultExt = ".chapter";
            sfd.Filter = "Chaper documents (.chapter)|*.chapter";
            bool? res = sfd.ShowDialog();
            if (res != false)
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
                        /*double storyBoardItemPointX = storyBoardItemPoint.X;
                        double storyBoardItemPointY = storyBoardItemPoint.Y;*/
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

                string rawManuscriptPolylinesContent = "";
                int manuscriptItemIndex = -1;
                foreach (UIElement manuscriptItem in manuscript.Children)
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
                            rawManuscriptPolylinesContentItem += rawManuscriptItemPointData;
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
                foreach (UIElement manuscriptItem in manuscript.Children)
                {
                    bool isPolygon = manuscriptItem is Polygon;
                    if (isPolygon)
                    {
                        Polygon manuscriptPolygonItem = manuscriptItem as Polygon;
                        manuscriptItemIndex++;
                        string rawManuscriptPolygonesContentItem = "";
                        if (manuscriptItemIndex >= 1)
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
                            if (manuscriptItemPointIndex < lastPointIndex)
                            {
                                rawManuscriptPolygonesContentItem += "|";
                            }
                        }
                        rawManuscriptPolygonesContent += rawManuscriptPolygonesContentItem;
                    }
                }

                string chapterRawDataContent = storyBoardDescBoxContent + "\n" + rawStoryBoardVisualContent + "\n" + rawManuscriptPolylinesContent + "\n" + rawManuscriptPolygonesContent;
                File.WriteAllText(fullPath, chapterRawDataContent);
            }
        }

        private void OpenChapterHandler(object sender, RoutedEventArgs e)
        {
            OpenChapter();
        }

        public void OpenChapter()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Title = "Выберите файлы, которые нужно добавить";
            ofd.Multiselect = true;
            bool? res = ofd.ShowDialog();
            if (res != false)
            {
                string fullPath = ofd.FileName;
                string[] rawChapterData = File.ReadAllLines(fullPath);
                int rawChapterDataItemIndex = -1;
                foreach (string rawChapterDataItem in rawChapterData)
                {
                    rawChapterDataItemIndex++;
                    bool isStoryboardDescContent = rawChapterDataItemIndex == 0;
                    bool isStoryboardVisualContent = rawChapterDataItemIndex == 1;
                    bool isManuscriptPolylinesContent = rawChapterDataItemIndex == 2;
                    bool isManuscriptPolygonesContent = rawChapterDataItemIndex == 3;
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
                                manuscript.Children.Add(line);
                                // debugger.Speak("Добавляю точку");
                            }
                        }
                    }
                    else if (isManuscriptPolygonesContent)
                    {
                        string formatedRawChapterDataItem = rawChapterDataItem;
                        bool isErrorsDetected = formatedRawChapterDataItem.Contains("@@");
                        if (isErrorsDetected)
                        {
                            formatedRawChapterDataItem = rawChapterDataItem.Replace("@@", "@");
                        }
                        string[] rawLines = formatedRawChapterDataItem.Split(new Char[] { '@' });
                        int countRawLines = rawLines.Length;
                        // bool isRawDataDetected = countRawLines >= 2;
                        bool isRawDataDetected = countRawLines >= 1;
                        if (isRawDataDetected)
                        {
                            foreach (string rawLine in rawLines)
                            {
                                Polygon line = new Polygon();
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
                                int selectedScreenTonesBoxItemIndex = screenTonesBox.SelectedIndex;
                                ItemCollection screenTonesBoxItems = screenTonesBox.Items;
                                ComboBoxItem selectedScreenTonesBoxItem = ((ComboBoxItem)(screenTonesBoxItems[selectedScreenTonesBoxItemIndex]));
                                ImageSource imageSource = ((Image)(selectedScreenTonesBoxItem.Content)).Source;
                                ImageBrush frameBrush = new ImageBrush(imageSource);
                                line.Fill = frameBrush;
                                manuscript.Children.Add(line);
                                // debugger.Speak("Добавляю точку");
                            }
                        }
                    }
                }
            }
        }

    }
}