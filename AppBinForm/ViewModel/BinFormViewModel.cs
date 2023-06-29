using AppBinForm.ViewModel.Base;
using System.Windows.Input;
using AppBinForm.Command;
using AppBinForm.Model;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace AppBinForm.ViewModel
{
    public class BinFormViewModel : BaseViewModel
    {
        private FileBytes _fileBytes;
        private ObservableCollection<FileBytes> _file;

        private bool _isLoading;
        private bool _isChecked;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public IEnumerable<FileBytes> File => _file;
        public FileBytes FileBytes
        {
            get
            {
                return _fileBytes;
            }
            set
            {
                _fileBytes = value;
                OnPropertyChanged(nameof(FileBytes));
            }
        }
        public ICommand OpenBinFileCommand { get; }
        public ICommand SaveBinFileCommand { get; }

        public BinFormViewModel()
        {
            OpenBinFileCommand = new OpenBinFileCommand(this);
            _file = new ObservableCollection<FileBytes>();
            //SaveBinFileCommand = new SaveBinFileCommand(binFormViewModelNavigationService);
        }
    }
}