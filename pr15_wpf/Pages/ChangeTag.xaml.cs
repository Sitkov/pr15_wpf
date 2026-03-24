using pr15_wpf.Models;
using pr15_wpf.Service;
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
    /// Логика взаимодействия для ChangeTag.xaml
    /// </summary>
    public partial class ChangeTag :Page
    {
        private ElectroStoreContext db = DBService.Instance.Context;

        public ChangeTag ()
        {
            InitializeComponent();
            LoadTags();
        }

        private void LoadTags ()
        {
            TagsList.ItemsSource = db.Tags.ToList();
        }

        private void AddBtn_Click (object sender, RoutedEventArgs e)
        {
            var exists = db.Tags.FirstOrDefault(b => b.Name == BrandNameTxt.Text);
            if (exists != null)
            {
                MessageBox.Show("Такой teg уж есть!");
                return;
            }
            if (string.IsNullOrWhiteSpace(BrandNameTxt.Text))
                return;

            var newBrand = new Tag { Name = BrandNameTxt.Text };
            db.Tags.Add(newBrand);
            db.SaveChanges();

            BrandNameTxt.Clear();
            LoadTags();
        }

        private void EditBtn_Click (object sender, RoutedEventArgs e)
        {
            if (TagsList.SelectedItem is Tag selectedBrand)
            {
                BrandNameTxt.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                db.SaveChanges();
                LoadTags();
                MessageBox.Show("TEG обновлен");
            }
        }

        private void DeleteBtn_Click (object sender, RoutedEventArgs e)
        {
            if (TagsList.SelectedItem is Tag selectedBrand)
            {
                var result = MessageBox.Show($"Удалить TEG {selectedBrand.Name}?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.Tags.Remove(selectedBrand);
                        db.SaveChanges();
                        LoadTags();
                    } catch (Exception ex)
                    {
                        MessageBox.Show("Бренд нелья удалить, тк существуют продукты с этим тегом");
                    }
                }
            }
        }

        private void Back (object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LoadPage (object sender, RoutedEventArgs e)
        {
            LoadTags();
        }
    }
}
