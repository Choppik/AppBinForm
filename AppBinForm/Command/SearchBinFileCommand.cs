using AppBinForm.Command.Base;
using AppBinForm.ViewModel;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;

namespace AppBinForm.Command
{
    public class SearchBinFileCommand : BaseCommand
    {
        private BinFormViewModel _binFormViewModel;

        public SearchBinFileCommand(BinFormViewModel binFormViewModel)
        {
            _binFormViewModel = binFormViewModel;
        }
        public override void Execute(object? parameter)
        {
            var res = Encoding.ASCII.GetBytes(_binFormViewModel.StrSearch);
            /*_binFormViewModel.IsOpen = false;
            _binFormViewModel.Sb.Clear();
            _binFormViewModel.FilePath = openFile.FileName;
            _binFormViewModel.IsOpen = true;
            _binFormViewModel.CurrentPos = 0;
            _binFormViewModel.Stream = new(_binFormViewModel.FilePath, FileMode.Open, FileAccess.Read);
            _binFormViewModel.Offset = 0;*/
        }
    }
}