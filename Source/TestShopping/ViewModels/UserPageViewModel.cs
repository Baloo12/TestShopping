using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestShopping.Models;

namespace TestShopping.ViewModels
{
    public class UserPageViewModel : BindableBase, INavigationAware
    {
        private User _currentUser;
        private ShoppingList _currentShoppingList;
        private ActivePage _currentPage = ActivePage.CurrentListPage;

        private ObservableCollection<BindableProduct> _currentList;
        private ObservableCollection<ShoppingList> _historyLists;
        private ObservableCollection<Product> _oldListProducts;
        private string _productTitle;
        private BindableProduct _selectedProduct;
        private ShoppingList _selectedHistoryList;

        private IRegionManager _regionManager;

        public UserPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            LogoutCommand = new DelegateCommand(Logout);
            OpenCurrentListCommand = new DelegateCommand(OpenCurrentList);
            OpenHistoryCommand = new DelegateCommand(OpenHistory);
            AddProductCommand = new DelegateCommand(AddProduct, CanAddProduct);
            DeleteProductCommand = new DelegateCommand(DeleteProduct, CanDeleteProduct);
            ChangeProductStateCommand = new DelegateCommand(ChangeProductState, CanChangeProductState);
            CreateNewListCommand = new DelegateCommand(CreateNewList, CanCreateNewList);

        }

        public ActivePage CurrentPage
        {
            get { return _currentPage; }
            set { SetProperty(ref _currentPage, value); }
        }

        public ObservableCollection<BindableProduct> CurrentList
        {
            get { return _currentList; }
            set { SetProperty(ref _currentList, value); }
        }

        public ObservableCollection<ShoppingList> HistoryLists
        {
            get { return _historyLists; }
            set { SetProperty(ref _historyLists, value); }
        }

        public ObservableCollection<Product> OldListProducts
        {
            get { return _oldListProducts; }
            set { SetProperty(ref _oldListProducts, value); }
        }

        public string ProductTitle
        {
            get { return _productTitle; }
            set
            {
                SetProperty(ref _productTitle, value);
                AddProductCommand.RaiseCanExecuteChanged();
            }
        }

        public BindableProduct SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                SetProperty(ref _selectedProduct, value);
                DeleteProductCommand.RaiseCanExecuteChanged();
                CreateNewListCommand.RaiseCanExecuteChanged();
                ChangeProductStateCommand.RaiseCanExecuteChanged();
            }
        }

        public ShoppingList SelectedHistoryList
        {
            get { return _selectedHistoryList; }
            set
            {
                SetProperty(ref _selectedHistoryList, value);
                RefreshSelectedOldList();
            }
        }

        #region Commands

        public DelegateCommand LogoutCommand { get; set; }
        public DelegateCommand OpenCurrentListCommand { get; set; }
        public DelegateCommand OpenHistoryCommand { get; set; }
        public DelegateCommand AddProductCommand { get; set; }
        public DelegateCommand DeleteProductCommand { get; set; }
        public DelegateCommand ChangeProductStateCommand { get; set; }
        public DelegateCommand CreateNewListCommand { get; set; }

        #endregion

        private void LoadCurrentList()
        {
            CurrentList = new ObservableCollection<BindableProduct>();
            using (var db = new ShoppingContext())
            {
                _currentShoppingList = db.ShoppingLists.Where(x => x.UserId == _currentUser.UserId).OrderByDescending(x => x.CreationDate).FirstOrDefault();
                if (_currentShoppingList == null)
                {
                    _currentShoppingList = new ShoppingList(_currentUser.UserId);
                    db.ShoppingLists.Add(_currentShoppingList);
                    db.SaveChanges();
                    return;
                }
                var products = db.Products.Where(x => x.ShoppingListId == _currentShoppingList.ShoppingListId);
                CurrentList = new ObservableCollection<BindableProduct>();
                foreach (var product in products)
                    CurrentList.Add(new BindableProduct(product));
            }
        }

        private void OpenCurrentList()
        {
            CurrentPage = ActivePage.CurrentListPage;
        }

        private void OpenHistory()
        {
            CurrentPage = ActivePage.HistoryPage;
        }

        private void Logout()
        {
            _regionManager.RequestNavigate("ContentRegion", "LoginPage");
        }

        private void AddProduct()
        {
            var newProduct = new Product(ProductTitle, _currentShoppingList.ShoppingListId);
            using (var db = new ShoppingContext())
            {
                db.Products.Add(newProduct);
                db.SaveChanges();
            }
            CurrentList.Add(new BindableProduct(newProduct));
            ProductTitle = "";
        }

        private bool CanAddProduct()
        {
            return !string.IsNullOrWhiteSpace(ProductTitle);
        }

        private void DeleteProduct()
        {
            using (var db = new ShoppingContext())
            {
                var productToRemove = db.Products.SingleOrDefault(x => x.ProductId == SelectedProduct.OriginalProduct.ProductId);
                db.Products.Remove(productToRemove);
                db.SaveChanges();
            }
            CurrentList.Remove(SelectedProduct);
        }

        private bool CanDeleteProduct()
        {
            return SelectedProduct != null;
        }

        private void ChangeProductState()
        {
            SelectedProduct.Done = !SelectedProduct.Done;
            using (var db = new ShoppingContext())
            {
                var productToChange = db.Products.SingleOrDefault(x => x.ProductId == SelectedProduct.OriginalProduct.ProductId);
                if (productToChange != null)
                {
                    productToChange.Done = SelectedProduct.Done;
                    db.SaveChanges();
                }
            }
        }

        private bool CanChangeProductState()
        {
            return SelectedProduct != null;
        }

        private void CreateNewList()
        {
            using (var db = new ShoppingContext())
            {
                var newList = new ShoppingList(_currentUser.UserId);
                db.ShoppingLists.Add(newList);
                db.SaveChanges();
                _currentShoppingList = newList;
                CurrentList = new ObservableCollection<BindableProduct>();
            }
            RefreshHistory();
        }

        private bool CanCreateNewList()
        {
            return CurrentList != null && CurrentList.Count > 0;
        }

        private void RefreshHistory()
        {
            using (var db = new ShoppingContext())
            {
                HistoryLists = new ObservableCollection<ShoppingList>(db.ShoppingLists.Where(x => x.UserId == _currentUser.UserId).OrderByDescending(x => x.CreationDate).Skip(1));
            }
        }

        private void RefreshSelectedOldList()
        {
            if (SelectedHistoryList == null)
                return;
            using (var db = new ShoppingContext())
            {
                OldListProducts = new ObservableCollection<Product>(db.Products.Where(x => x.ShoppingListId == SelectedHistoryList.ShoppingListId));
            }
        }

        #region INavigationAwareImpl
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            return;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            User user = (User)navigationContext.Parameters["User"];
            _currentUser = user;
            LoadCurrentList();
            RefreshHistory();
        }
        #endregion
    }

    public enum ActivePage
    {
        CurrentListPage,
        HistoryPage
    }
}
