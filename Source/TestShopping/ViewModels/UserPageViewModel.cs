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

            LoadCurrentList();
            RefreshHistory();
        }

        public ActivePage CurrentPage
        {
            get { return _currentPage; }
            set { SetProperty(ref _currentPage, value); }
        }

        public ObservableCollection<Product> CurrentList { get; set; }

        public ObservableCollection<ShoppingList> HistoryLists { get; set; }

        public ObservableCollection<Product> OldListProducts { get; set; }

        public string ProductTitle { get; set; }

        public Product SelectedProduct { get; set; }

        public ShoppingList SelectedHistoryList { get; set; }

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
            CurrentList = new ObservableCollection<Product>();
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
                CurrentList = new ObservableCollection<Product>(db.Products.Where(x => x.ShoppingListId == _currentShoppingList.ShoppingListId));
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
        }

        private bool CanAddProduct()
        {
            return string.IsNullOrWhiteSpace(ProductTitle);
        }

        private void DeleteProduct()
        {
            CurrentList.Remove(SelectedProduct);
            using (var db = new ShoppingContext())
            {
                db.Products.Remove(SelectedProduct);
                db.SaveChanges();
            }
        }

        private bool CanDeleteProduct()
        {
            return SelectedProduct != null;
        }

        private void ChangeProductState()
        {
            SelectedProduct.Done = !SelectedProduct.Done;
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
            }
            RefreshHistory();
        }

        private void RefreshHistory()
        {
            using (var db =new ShoppingContext())
            {
                HistoryLists = new ObservableCollection<ShoppingList>(db.ShoppingLists.Where(x => x.UserId == _currentUser.UserId).OrderByDescending(x => x.CreationDate).Skip(1));
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
        }
        #endregion
    }

    public enum ActivePage
    {
        CurrentListPage,
        HistoryPage
    }
}
