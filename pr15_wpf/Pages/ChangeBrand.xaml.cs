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
    /// Логика взаимодействия для ChangeBrand.xaml
    /// </summary>
    public partial class ChangeBrand :Page
    {
        private ElectroStoreContext db = DBService.Instance.Context;

        public ChangeBrand ()
        {
            InitializeComponent();
            LoadBrands();
        }

        private void LoadBrands ()
        {
            BrandsList.ItemsSource = db.Brands.ToList();
        }

        private void AddBtn_Click (object sender, RoutedEventArgs e)
        {
            var exists = db.Brands.FirstOrDefault(b => b.Name == BrandNameTxt.Text);
            if (exists != null)
            {
                MessageBox.Show("Такой бренд уж есть!");
                return;
            }
            if (string.IsNullOrWhiteSpace(BrandNameTxt.Text))
                return;

            var newBrand = new Brand { Name = BrandNameTxt.Text };
            db.Brands.Add(newBrand);
            db.SaveChanges();

            BrandNameTxt.Clear();
            LoadBrands();
        }

        private void EditBtn_Click (object sender, RoutedEventArgs e)
        {
            if (BrandsList.SelectedItem is Brand selectedBrand)
            {
                BrandNameTxt.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                db.SaveChanges();
                LoadBrands();
                MessageBox.Show("Бренд обновлен");
            }
        }

        private void DeleteBtn_Click (object sender, RoutedEventArgs e)
        {
            if (BrandsList.SelectedItem is Brand selectedBrand)
            {
                var result = MessageBox.Show($"Удалить бренд {selectedBrand.Name}?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.Brands.Remove(selectedBrand);
                        db.SaveChanges();
                        LoadBrands();
                    } catch (Exception ex) 
                    { 
                        MessageBox.Show("Бренд нелья удалить, тк существуют продукты этого бренда");
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
            LoadBrands();
        }
    }
}
