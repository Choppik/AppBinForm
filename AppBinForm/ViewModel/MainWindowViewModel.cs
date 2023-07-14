using AppBinForm.Store;
using AppBinForm.ViewModel.Base;

namespace AppBinForm.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
        public BaseViewModel? CurrentViewModel => _navigationStore.CurrentViewModel;

        #region Заголовок окна
        private string _title = "HexViewer";

        public string Title { get => _title; set => Set(ref _title, value); }
        #endregion

        public MainWindowViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChange;
        }

        public override void Dispose()
        {
            _navigationStore.CurrentViewModelChanged -= OnCurrentViewModelChange;
            base.Dispose();
        }

        private void OnCurrentViewModelChange()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
