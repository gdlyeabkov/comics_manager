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

namespace ComicsManager.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для CreateFramesDialog.xaml
    /// </summary>
    public partial class CreateFramesDialog : Window
    {
        public CreateFramesDialog()
        {
            InitializeComponent();
        }

        private void CreateFramesHandler(object sender, RoutedEventArgs e)
        {
            CreateFrames();
        }

        public void CreateFrames()
        {
            string countFramesPerXLabelContent = countFramesPerXLabel.Text;
            string countFramesPerYLabelContent = countFramesPerYLabel.Text;
            string marginLabelContent = marginLabel.Text;
            int parsedCountFramesPerXLabelContent = Int32.Parse(countFramesPerXLabelContent);
            int parsedCountFramesPerYLabelContent = Int32.Parse(countFramesPerYLabelContent);
            double parsedMarginLabelContent = Double.Parse(marginLabelContent);
            Dictionary<String, Object> data = new Dictionary<String, Object>();
            data.Add("countFramesPerX", parsedCountFramesPerXLabelContent);
            data.Add("countFramesPerY", parsedCountFramesPerYLabelContent);
            data.Add("margin", parsedMarginLabelContent);
            this.DataContext = data;
            this.Close();
        }

    }
}
