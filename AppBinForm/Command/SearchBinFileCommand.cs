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
            if (!_binFormViewModel.IsOpen)
            {
                MessageBox.Show("Файл не выбран.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_regex.IsMatch(_binFormViewModel.StrSearch) == true)
            {
                MessageBox.Show("Некорректный ввод.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            long.TryParse(_binFormViewModel.StrSearch, System.Globalization.NumberStyles.HexNumber, null, out long i);
            while (i%16 != 0)
            {
                i--;
            }
            if (i > _binFormViewModel.Stream.Length)
            {
                MessageBox.Show("Введенная позиция превышает размер файла.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_binFormViewModel.MaxOffset == 0) return;
            _binFormViewModel.CurrentPosition = i;
            _binFormViewModel.IsSearch = true;
        }
    }
}