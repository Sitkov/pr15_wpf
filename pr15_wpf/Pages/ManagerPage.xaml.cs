using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using pr15_wpf.Models;
using pr15_wpf.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Логика взаимодействия для ManagerPage.xaml
    /// </summary>
    public partial class ManagerPage : Page
    {

        public string filterPriceFrom { get; set; } = null!;
        public string filterPriceTo { get; set; } = null!;

        public ElectroStoreContext db = DBService.Instance.Context;
        public ObservableCollection<Product> products { get; set; } = new();
        public ICollectionView productsView { get; set; }
        public string searchQuery { get; set; } = null!;
        public int selectedCategoryId = 0;
        public int selectedBrandId = 0;
        public ManagerPage()
        {
            InitializeComponent();
            DataContext = this;
            productsView = CollectionViewSource.GetDefaultView(products);
            productsView.Filter = FilterProducts;
            LoadData();
            LoadCategories();
            LoadBrands();
            selectedCategoryId = 0;
        }
        private void LoadPage (object sender, RoutedEventArgs e)
        {
            LoadData();
        }
        private void LoadCategories ()
        {
            CategoryFilter.Items.Clear();
            CategoryFilter.Items.Add(new ComboBoxItem { Content = "Все категории", Tag = 0 });
            var categories = db.Categories.ToList();
            foreach (var cat in categories)
            {
                CategoryFilter.Items.Add(new ComboBoxItem { Content = cat.Name, Tag = cat.Id });
            }
            CategoryFilter.SelectedIndex = 0;
        }

        private void LoadBrands ()
        {
            var brands = db.Brands.ToList();
            foreach (var brand in brands)
            {
                var item = new ComboBoxItem();
                item.Content = brand.Name;
                item.Tag = brand.Id;
                BrandFilter.Items.Add(item);
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            productsView.SortDescriptions.Clear();
            var cb = (ComboBox)sender;
            var selected = (ComboBoxItem)cb.SelectedItem;
            switch (selected.Tag)
            {
                case "PriceUp":
                    productsView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Ascending));
                    break;
                case "PriceDown":
                    productsView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Descending));
                    break;
                case "Name":
                    productsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    break;
                case "CountUp":
                    productsView.SortDescriptions.Add(new SortDescription("Stock", ListSortDirection.Ascending));
                    break;
                case "CountDown":
                    productsView.SortDescriptions.Add(new SortDescription("Stock", ListSortDirection.Descending));
                    break;
            }
            productsView.Refresh();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            productsView.Refresh();
        }

        public bool FilterProducts (object obj)
        {
            if (obj is not Product product)
                return false;

            if (!string.IsNullOrWhiteSpace(searchQuery) &&
                !product.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                return false;

            bool hasMin = decimal.TryParse(filterPriceFrom, out decimal minPrice);
            bool hasMax = decimal.TryParse(filterPriceTo, out decimal maxPrice);

            if (hasMin && hasMax && minPrice > maxPrice)
            {
                return true;
            }

            if (hasMin && product.Price < minPrice)
                return false;

            if (hasMax && product.Price > maxPrice)
                return false;

            if (selectedCategoryId > 0 && product.CategoryId != selectedCategoryId)
                return false;
            if (selectedBrandId > 0 && product.BrandId != selectedBrandId)
                return false;

            return true;
        }

        public void LoadData ()
        {
            products.Clear();
            var data = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Tags)
                .ToList();

            foreach (var product in data)
            {
                products.Add(product);
            }
        }


        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsList.SelectedItem as Product;
            if (selectedProduct != null)
            {
                var productTags = db.Set<Dictionary<string, object>>("ProductTag")
                                    .Where(pt => (int)pt["ProductId"] == selectedProduct.Id)
                                    .ToList();

                foreach (var pt in productTags)
                    db.Remove(pt);
                db.Products.Remove(selectedProduct);
                db.SaveChanges();
                products.Remove(selectedProduct);
            }
        }

        private void Change(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsList.SelectedItem as Product;
            NavigationService.Navigate(new AddChangeProduct(selectedProduct));
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddChangeProduct(null));
        }
        private void NumberValidationTextBox (object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, e.Text.Length - 1);
        }

        private void GoBrangs (object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangeBrand());
        }

        private void Button_Click (object sender, RoutedEventArgs e)
        {

        }

        private void GoCategory (object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangeCategory());
        }

        private void GoTags (object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangeTag());
        }
        private void ResetFilters_Click (object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            PriceFromTextBox.Text = "";
            PriceToTextBox.Text = "";

            searchQuery = "";
            filterPriceFrom = "";
            filterPriceTo = "";

            CategoryFilter.SelectedIndex = 0;
            BrandFilter.SelectedIndex = 0;
            selectedCategoryId = 0;
            selectedBrandId = 0;

            productsView.Refresh();
        }

        private void BrandFilter_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            var selected = BrandFilter.SelectedItem as ComboBoxItem;
            if (selected != null)
            {
                selectedBrandId = Convert.ToInt32(selected.Tag);
                productsView.Refresh();
            }
        }

        private void CategoryFilter_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            var selected = CategoryFilter.SelectedItem as ComboBoxItem;
            if (selected != null)
            {
                selectedCategoryId = Convert.ToInt32(selected.Tag);
                productsView.Refresh();
            }
        }

    }
}
