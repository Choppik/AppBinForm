using AppBinForm.Store;
using AppBinForm.ViewModel.Base;
using System;

namespace AppBinForm.Servies
{
    public class ParameterNavigationService<TParameter, TViewModel> : IParamNavigationService<TParameter>
        where TViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TParameter, TViewModel> _createViewModel;

        public ParameterNavigationService(NavigationStore navigationStore, Func<TParameter, TViewModel> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public void Navigate(TParameter parameter)
        {
            _navigationStore.CurrentViewModel = _createViewModel(parameter);
        }
    }
}