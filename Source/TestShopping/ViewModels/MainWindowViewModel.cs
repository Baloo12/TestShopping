using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestShopping.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private bool _startupNavigationDone = false;
        private readonly IRegionManager _regionManager;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            ExitCommand = new DelegateCommand(CloseWindow);
            _regionManager.Regions.CollectionChanged += (o, e) => StartupNavigate();
        }

        public DelegateCommand ExitCommand { get; set; }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate("ContentRegion", uri);
        }

        private void StartupNavigate()
        {
            if (_startupNavigationDone)
                return;

            if (_regionManager == null)
                return;

            if (_regionManager.Regions.Count(x => x != null) == 1)
                if (_regionManager.Regions.FirstOrDefault(x => x.Name == "ContentRegion") != null)
                {
                    Navigate("LoginPage");
                    _startupNavigationDone = true;
                }

        }

        private void CloseWindow()
        {
            Application.Current.MainWindow.Close();
        }
    }
}
