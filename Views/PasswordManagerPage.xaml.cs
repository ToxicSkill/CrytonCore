using System.Windows;
using System.Windows.Controls;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for PasswordManagerPage.xaml
    /// </summary>
    public partial class PasswordManagerPage : Page
    {
        char count = (char)65;
        int i = 0;
        int j = 0;

        public PasswordManagerPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button MyControl1 = new();
            MyControl1.Content = count.ToString();
            MyControl1.Name = "Button" + count.ToString();

            Grid.SetColumn(MyControl1, j);
            Grid.SetRow(MyControl1, i);
            grid.Children.Add(MyControl1);
            j++;
            if (j == 11)
            {
                j = 0;
                i++;
            }
            count++;

        }
    }
}
