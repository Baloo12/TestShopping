using Prism.Commands;
using Prism.Events;
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
    public class LoginPageViewModel : BindableBase, INavigationAware
    {
        private IRegionManager _regionManager;
        private User _selecteduser;
        private ObservableCollection<User> _usersList = new ObservableCollection<User>();

        public DelegateCommand RegistrationOpenCommand { get; set; }
        public DelegateCommand LoginCommand { get; set; }

        public ObservableCollection<User> UsersList
        {
            get { return _usersList; }
            set { SetProperty(ref _usersList, value); }
        }
        public User SelectedUser
        {
            get { return _selecteduser; }
            set { SetProperty(ref _selecteduser, value); }
        }

        public LoginPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            RegistrationOpenCommand = new DelegateCommand(RegistrationOpen);
            LoginCommand = new DelegateCommand(Login);
        }

        #region Methods

        public void RegistrationOpen()
        {
            _regionManager.RequestNavigate("ContentRegion", "RegistrationPage");
        }

        private void Login()
        {
            if (SelectedUser == null)
                return;
            NavigationParameters navigationParams = new NavigationParameters();
            navigationParams.Add("User", SelectedUser);
            _regionManager.RequestNavigate("ContentRegion", "UserPage", navigationParams);
        }

        private void LoadUsers()
        {
            using (var db = new ShoppingContext())
            {
                var tempUsersCollection = new ObservableCollection<User>();
                try
                {
                    var query = from u in db.Users select u;

                    foreach (var user in query)
                        tempUsersCollection.Add(user);
                    
                }
                catch(Exception)
                {
                    MessageBox.Show("Failed to load db. Delete db! =)");
                    db.Database.Delete();
                }
                UsersList = tempUsersCollection;
            }

        }

        private void Initialize()
        {
            LoadUsers();
            SelectedUser = UsersList.FirstOrDefault();
        }

        #region INavagationAwareImpl
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Initialize();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            return;
        }
        #endregion

        #endregion
    }
}
