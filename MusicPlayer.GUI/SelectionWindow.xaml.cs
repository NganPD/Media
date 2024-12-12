using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MusicPlayer.GUI
{
    public partial class SelectionWindow : Window
    {
        public List<string> SelectedItems { get; private set; } = new();

        public SelectionWindow(List<string> items)
        {
            InitializeComponent();
            ItemsListBox.ItemsSource = items;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedItems = ItemsListBox.SelectedItems.Cast<string>().ToList();
            DialogResult = true; // Đóng cửa sổ với trạng thái OK
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Đóng cửa sổ mà không chọn gì
            Close();
        }
    }
}
