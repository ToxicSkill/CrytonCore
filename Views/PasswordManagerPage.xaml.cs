using System.Windows.Controls;
using CrytonCore.Model;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for PasswordManagerPage.xaml
    /// </summary>
    public partial class PasswordManagerPage : Page
    {
        public PasswordManagerPage()
        {
            InitializeComponent();
            Weather weather = new();
            var curr = weather.GetCurrentWeather();
            var t = 0;
        }
        }
}
