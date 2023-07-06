using AppBinForm.Command.Base;
using Microsoft.Win32;
using System.IO;
using AppBinForm.ViewModel;

namespace AppBinForm.Command
{
    public class OpenBinFileCommand : BaseCommand
    {
        private BinFormViewModel _binFormViewModel;

        public OpenBinFileCommand(BinFormViewModel binFormViewModel)
        {
            _binFormViewModel = binFormViewModel;
        }
        public override void Execute(object? parameter)
        {
            OpenFile();

/*            if (_binFormViewModel.IsOpen == true)
            {
                _binFormViewModel.Shift = "";
                _binFormViewModel.Str16 = "";
                _binFormViewModel.Str = "";
            }*/
        }

        private void OpenFile()
        {
            OpenFileDialog openFile = new();
            if (openFile.ShowDialog() == true)
            {
                _binFormViewModel.FilePath = openFile.FileName;
                _binFormViewModel.IsOpen = true;
                _binFormViewModel.Stream = new(_binFormViewModel.FilePath, FileMode.Open, FileAccess.Read);
            }
        }
    }
}