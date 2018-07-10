using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestShopping.Views;

namespace TestShopping.PrismStuff
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.TryResolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterTypeForNavigation<LoginPage>("LoginPage");
            Container.RegisterTypeForNavigation<RegistrationPage>("RegistrationPage");
            Container.RegisterTypeForNavigation<UserPage>("UserPage");
        }
    }
}
