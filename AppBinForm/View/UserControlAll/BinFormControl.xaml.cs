using System.Windows;
using System.Windows.Controls;

namespace AppBinForm.View.UserControlAll
{
    public partial class BinFormControl : UserControl
    {
        public BinFormControl()
        {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var g = 0;
            g++;
            Point p = e.GetPosition(this);
            MessageBox.Show("Координата x=" + p.X.ToString() + " y=" + p.Y.ToString());
        }
    }
}