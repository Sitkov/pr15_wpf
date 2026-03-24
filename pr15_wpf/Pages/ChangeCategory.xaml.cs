using pr15_wpf.Models;
using pr15_wpf.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для ChangeCategory.xaml
    /// </summary>
    public partial class ChangeCategory :Page
    {
        private ElectroStoreContext db = DBService.Instance.Context;

        public ChangeCategory ()
        {
            InitializeComponent();
            LoadTags();
        }

        private void LoadTags ()
        {
            CategoryList.ItemsSource = db.Categories.ToList();
        }

        private void AddBtn_Click (object sender, RoutedEventArgs e)
        {
            var exists = db.Categories.FirstOrDefault(b => b.Name == BrandNameTxt.Text);
            if (exists != null)
            {
                MessageBox.Show("Такая категория уже есть!");
                return;
            }
            if (string.IsNullOrWhiteSpace(BrandNameTxt.Text))
                return;

            var newBrand = new Category { Name = BrandNameTxt.Text };
            db.Categories.Add(newBrand);
            db.SaveChanges();

            BrandNameTxt.Clear();
            LoadTags();
        }

        private void EditBtn_Click (object sender, RoutedEventArgs e)
        {
            if (CategoryList.SelectedItem is Category selectedBrand)
            {
                BrandNameTxt.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                db.SaveChanges();
                LoadTags();
                MessageBox.Show("Категория обновлена");
            }
        }

        private void DeleteBtn_Click (object sender, RoutedEventArgs e)
        {
            if (CategoryList.SelectedItem is Category selectedBrand)
            {
                var result = MessageBox.Show($"Удалить категорию {selectedBrand.Name}?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.Categories.Remove(selectedBrand);
                        db.SaveChanges();
                        LoadTags();
                    } catch (Exception ex)
                    {
                        MessageBox.Show("Бренд нелья удалить, тк существуют продукты с этой категорией");
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
