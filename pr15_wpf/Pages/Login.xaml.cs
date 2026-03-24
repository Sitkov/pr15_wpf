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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pr15_wpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login :Page
    {
        public Login ()
        {
            InitializeComponent();
        }

        private void GoManager (object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PINKOD.Password))
            {
                MessageBox.Show("Введите пароль!");
            } else if (PINKOD.Password != "1234")
            {
                MessageBox.Show("Неверный пароль!");
            } else
            {
                NavigationService.Navigate(new MainPage(1));
            }
        }

        private void GoGuest (object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage(0));
        }
    }
}
