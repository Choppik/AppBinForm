using AppBinForm.ViewModel.Base;
using System;

namespace AppBinForm.Store
{
    public class NavigationStore
    {
        public event Action? CurrentViewModelChanged;

        private BaseViewModels? _currentViewModel;
        public BaseViewModels? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel?.Dispose();
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }
        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}