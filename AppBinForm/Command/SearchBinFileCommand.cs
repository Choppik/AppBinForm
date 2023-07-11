using AppBinForm.Command.Base;
using AppBinForm.ViewModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace AppBinForm.Command
{
    public class SearchBinFileCommand : BaseCommand
    {
        private readonly BinFormViewModel _binFormViewModel;
        private readonly Regex _regex;

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
            long.TryParse(_binFormViewModel.StrSearch, System.Globalization.NumberStyles.HexNumber, null, out long i);
            while (i%16 != 0)
            {
                i--;
            }
            if (i > _binFormViewModel.Stream.Length)
            {
                MessageBox.Show("Позиция превышает размер файла.");
                return;
            }
            _binFormViewModel.CurrentPos = i;
            _binFormViewModel.IsSearch = true;
        }
    }
}