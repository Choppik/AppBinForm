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
                _binFormViewModel.CurrentPos = 0;
                _binFormViewModel.Stream = new(_binFormViewModel.FilePath, FileMode.Open, FileAccess.Read);
                _binFormViewModel.Offset = 0;
            }
        }
    }
}