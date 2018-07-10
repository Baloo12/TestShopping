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
        private ActivePage _currentPage = ActivePage.CurrentListPage;

        private IRegionManager _regionManager;

        public UserPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            CurrentList = (ObservableCollection<Product>)_currentUser.ShoppingLists.OrderByDescending(x => x.CreationDate).First()

            LogoutCommand = new DelegateCommand(Logout);
            OpenCurrentListCommand = new DelegateCommand(OpenCurrentList);
            OpenHistoryCommand = new DelegateCommand(OpenHistory);
        }

        public ActivePage CurrentPage
        {
            get { return _currentPage; }
            set { SetProperty(ref _currentPage, value); }
        }

        public ObservableCollection<Product> CurrentList { get; set; }

        #region Commands

        public DelegateCommand LogoutCommand { get; set; }
        public DelegateCommand OpenCurrentListCommand { get; set; }
        public DelegateCommand OpenHistoryCommand { get; set; }

        #endregion

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
