using AppBinForm.Servies;
using AppBinForm.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System;
using AppBinForm.Command;
using System.IO;
using System.Text;

namespace AppBinForm.ViewModel
{
    public class BinFormViewModel : BaseViewModels
    {

       // private ObservableCollection<Employee> _staff;

       // private Employee _employee;

        private string _userName;
        private string _password;
        private string _replayPassword;
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
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set //=> Set(ref _password, value);
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string ReplayPassword
        {
            get
            {
                return _replayPassword;
            }
            set
            {
                _replayPassword = value;
                OnPropertyChanged(nameof(ReplayPassword));
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
        //public IEnumerable<Employee> Staff => _staff;
        /*public Employee Employee
        {
            get
            {
                return _employee;
            }
            set
            {
                _employee = value;
                OnPropertyChanged(nameof(Employee));
            }
        }*/

        public ICommand OpenBinFileCommand { get; }
        public ICommand SaveBinFileCommand { get; }

        public BinFormViewModel(INavigationService binFormViewModelNavigationService)
        {
            OpenBinFileCommand = new OpenBinFileCommand(binFormViewModelNavigationService);

            ReadBinFile();
            //SaveBinFileCommand = new SaveBinFileCommand(binFormViewModelNavigationService);
        }
        private static void ReadBinFile()
        {
            using (FileStream fs = new("fileBin.bin", FileMode.Open))
            {
                using (BinaryReader bw = new(fs, Encoding.Default))
                {
                    bw.Read();
                    bw.ReadBytes(4);
                    bw.Close();
                }
                fs.Close();
            }
        }
    }
}