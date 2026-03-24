using Microsoft.EntityFrameworkCore;
using pr15_wpf.Models;
using pr15_wpf.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для AddChangeProduct.xaml
    /// </summary>
    public partial class AddChangeProduct : Page
    {
        public ObservableCollection<Product> products { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<Brand> Brands { get; set; } = new();
        public Product SelectedProduct { get; set; } = new();

        private ElectroStoreContext db = DBService.Instance.Context;
        public AddChangeProduct (Product product)
        {
            InitializeComponent();

            if (product == null)
                SelectedProduct = new Product();
            else
                SelectedProduct = db.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Tags) 
                    .FirstOrDefault(p => p.Id == product.Id) ?? new Product();

            Categories = new ObservableCollection<Category>(db.Categories.ToList());
            Brands = new ObservableCollection<Brand>(db.Brands.ToList());

            DataContext = this;
            LoadTags();

            foreach (var tag in SelectedProduct.Tags)
            {
                var item = lbTags.Items.Cast<Tag>().FirstOrDefault(t => t.Id == tag.Id);
                if (item != null)
                    lbTags.SelectedItems.Add(item);
            }
        }

        private void LoadTags ()
        {
            var tags = db.Tags.ToList();
            lbTags.ItemsSource = tags;
            lbTags.DisplayMemberPath = "Name";
            lbTags.SelectedValuePath = "Id";
        }
        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SelectedProduct.Name) ||
                                          SelectedProduct.Price <= 0 ||
                                          SelectedProduct.Stock < 0 ||
                                          SelectedProduct.Category == null ||
                                          SelectedProduct.Brand == null)
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return; 
            }
            SelectedProduct.Tags.Clear(); 
            foreach (var item in lbTags.SelectedItems)
            {
                var tag = item as Tag;
                if (tag != null)
                {
                    var dbTag = db.Tags.Find(tag.Id);
                    SelectedProduct.Tags.Add(dbTag);
                }
            }

            if (SelectedProduct.Id == 0)
            {
                db.Products.Add(SelectedProduct);
            }

            try
            {
                db.SaveChanges();
                MessageBox.Show("Продукт сохранён!");
                NavigationService.GoBack();
            } catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
