using AppBinForm.Servies;
using AppBinForm.Store;
using AppBinForm.View;
using AppBinForm.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace AppBinForm
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            #region Зависимости

            _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                services.AddSingleton<NavigationStore>();
                services.AddSingleton<BinFormViewModel>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton(s => CreateBinFormNavigationService(s));

                services.AddSingleton(s => new MainWindow()
                {
                    DataContext = s.GetRequiredService<MainWindowViewModel>()
                });
            }).Build();

            #endregion
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            INavigationService initialNavigationService = _host.Services.GetRequiredService<INavigationService>();
            initialNavigationService.Navigate();

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();
            base.OnExit(e);
        }
        private static INavigationService CreateBinFormNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<BinFormViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<BinFormViewModel>());
        }
    }
}