using AppBinForm.Command.Base;
using AppBinForm.ViewModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace AppBinForm.Command
{
    public class SearchBinFileCommand : BaseCommand
    {
        private BinFormViewModel _binFormViewModel;
        private Regex _regex;

        public SearchBinFileCommand(BinFormViewModel binFormViewModel, Regex regex)
        {
            _binFormViewModel = binFormViewModel;
            _regex = regex;
        }
        public override void Execute(object? parameter)
        {
            if (_regex.IsMatch(_binFormViewModel.StrSearch) == true)
            {
                MessageBox.Show("Некорректный ввод.");
                return;
            }
            var search = _regex.Replace(_binFormViewModel.StrSearch, "");
            long.TryParse(search, System.Globalization.NumberStyles.HexNumber, null, out long i);
            while (i%16 != 0)
            {
                i--;
            }
            _binFormViewModel.CurrentPos = i;
            _binFormViewModel.IsSearch = true;
        }
    }
}