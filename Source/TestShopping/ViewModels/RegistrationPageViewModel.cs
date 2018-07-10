using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestShopping.Models;

namespace TestShopping.ViewModels
{
    public class RegistrationPageViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private List<string> _userNames = new List<string>();

        public DelegateCommand RegisterAndLoginCommand { get; set; }
        public DelegateCommand ReturnCommand { get; set; }

        public string UserName { get; set; }

        public RegistrationPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            ReturnCommand = new DelegateCommand(Return);
            RegisterAndLoginCommand = new DelegateCommand(RegisterAndLogin);

            LoadUserNames();
        }

        private void Return()
        {
            
            _regionManager.RequestNavigate("ContentRegion", "LoginPage");
        }

        private void RegisterAndLogin()
        {
            if (_userNames.Contains(UserName))
            {
                MessageBox.Show("User already exists");
                return;
            }
            if (string.IsNullOrWhiteSpace(UserName))
            {
                MessageBox.Show("Enter vaild name");
                return;
            }
            var newUser = new User(UserName);
            using (var db = new ShoppingContext())
            {
                db.Users.Add(newUser);
                db.SaveChanges();
            }
            if (newUser == null)
                return;
            NavigationParameters navigationParams = new NavigationParameters();
            navigationParams.Add("User", newUser);
            _regionManager.RequestNavigate("ContentRegion", "UserPage", navigationParams);
        }

        private void LoadUserNames()
        {
            using (var db = new ShoppingContext())
            {
                _userNames.Clear();
                var query = from u in db.Users select u.Name;
                foreach (var name in query)
                    _userNames.Add(name);
            }
        }

    }
}
