using AppBinForm.Command.Base;
using Microsoft.Win32;
using System.IO;
using AppBinForm.ViewModel;

namespace AppBinForm.Command
{
    public class OpenBinFileCommand : BaseCommand
    {
        private readonly BinFormViewModel _binFormViewModel;

        public OpenBinFileCommand(BinFormViewModel binFormViewModel)
        {
            _binFormViewModel = binFormViewModel;
        }
        public override void Execute(object? parameter)
        {
            OpenFileDialog openFile = new();
            if (openFile.ShowDialog() == true)
            {
                _binFormViewModel.IsOpen = false;
                _binFormViewModel.FilePath = openFile.FileName;
                _binFormViewModel.IsOpen = true;
                _binFormViewModel.CurrentPosition = 0;
                _binFormViewModel.ResultStr = "";
                _binFormViewModel.Stream = new(_binFormViewModel.FilePath, FileMode.Open, FileAccess.Read);
            }
        }
    }
}