using AppBinForm.ViewModel.Base;
using System.Windows.Input;
using AppBinForm.Command;
using AppBinForm.Model;
using System.Collections.ObjectModel;
using System;

namespace AppBinForm.ViewModel
{
    public class BinFormViewModel : BaseViewModel
    {
        private FileBytes _fileBytes;
        private ObservableCollection<FileBytes> _file;

        private bool _isLoading;
        private bool _isChecked;
        private string _str16;
        private string _str;
        private string _shift;
        private Int32 _pos;

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

        public ObservableCollection<FileBytes> File => _file;
        public string Shift
        {
            get
            {
                return _shift;
            }
            set
            {
                _shift = value;
                OnPropertyChanged(nameof(Shift));
            }
        }
        public Int32 Pos
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
                OnPropertyChanged(nameof(Pos));
            }
        }
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
        public string Str16
        {
            get
            {
                return _str16;
            }
            set
            {
                _str16 = value;
                OnPropertyChanged(nameof(Str16));
            }
        }
        public string Str
        {
            get
            {
                return _str;
            }
            set
            {
                _str = value;
                OnPropertyChanged(nameof(Str));
            }
        }
        public ICommand OpenBinFileCommand { get; }
        public ICommand SaveBinFileCommand { get; }

        public BinFormViewModel()
        {
            OpenBinFileCommand = new OpenBinFileCommand(this);
            //_fileBytes = new FileBytes();
            _file = new ObservableCollection<FileBytes>();
            _fileBytes = new FileBytes();
            //SaveBinFileCommand = new SaveBinFileCommand(binFormViewModelNavigationService);
        }
    }
}