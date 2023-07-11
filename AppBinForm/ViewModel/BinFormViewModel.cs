using AppBinForm.ViewModel.Base;
using System.Windows.Input;
using AppBinForm.Command;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System;

namespace AppBinForm.ViewModel
{
    public class BinFormViewModel : BaseViewModel
    {
        #region Переменные и свойства
        private StringBuilder _sb;
        private FileStream _stream;

        private bool _isOpen = false;
        private bool _isScroll = false;
        private bool _isSearch = false;
        private bool _isAllFileRead = false;
        private string _filePath = "";
        private string _resultStr = "";
        private string _strSearch = "00000000";
        private long _currentPos = 0;
        private long _previousPos = 0;
        private long _buffer = 0;
        private int _leng = 79;
        private int _count = 0;
        private int _con = 0;
        private int _offset = 0;
        private int _preOffset = 0;
        private readonly int _maxLines = 1000;

        private readonly string pattern = @"[-]";
        private readonly string pattern2 = @"[\n\r\t\a\b\f\0\v\u0000-\u0033]";
        private readonly string pattern3 = @"[^A-Fa-f0-9]";
        private readonly Regex _rgx;
        private readonly Regex _rgx2;
        private readonly Regex _rgx3;

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                if (_isOpen == false)
                    Stream?.Close();
                OnPropertyChanged(nameof(IsOpen));
            }
        }
        public bool IsScroll
        {
            get
            {
                return _isScroll;
            }
            set
            {
                _isScroll = value;
                OnPropertyChanged(nameof(IsScroll));
            }
        }
        public bool IsSearch
        {
            get
            {
                return _isSearch;
            }
            set
            {
                _isSearch = value;
                OnPropertyChanged(nameof(IsSearch));
                if (_isSearch == true) ReadBinFile(_buffer, CurrentPos);
            }
        }
        public FileStream Stream
        {
            get
            {
                return _stream;
            }
            set
            {
                _stream = value;
                OnPropertyChanged(nameof(Stream));
                if (_stream != null)
                {
                    _count = 0;
                    _buffer = Stream.Length;
                }
            }
        }
        public StringBuilder Sb
        {
            get => _sb;
            set
            {
                _sb = value;
                OnPropertyChanged(nameof(Sb));
            }
        }
        public string ResultStr
        {
            get
            {
                return _resultStr;
            }
            set
            {
                _resultStr = value;
                OnPropertyChanged(nameof(ResultStr));
            }
        }
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }
        public string StrSearch
        {
            get
            {
                return _strSearch;
            }
            set
            {
                _strSearch = value;
                OnPropertyChanged(nameof(StrSearch));

            }
        }
        public long CurrentPos
        {
            get
            {
                return _currentPos;
            }
            set
            {
                _currentPos = value;
                OnPropertyChanged(nameof(CurrentPos));
            }
        }
        public int Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                OnPropertyChanged(nameof(Offset));
                if (_offset < _preOffset) IsScroll = true;
                else IsScroll = false;
                if (IsOpen == true)
                    ReadBinFile(_buffer, CurrentPos);
            }
        }
        public ICommand OpenBinFileCommand { get; }
        public ICommand SearchBinFileCommand { get; }
        #endregion

        public BinFormViewModel()
        {
            OpenBinFileCommand = new OpenBinFileCommand(this);
            Sb = new();
            _rgx = new(pattern);
            _rgx2 = new(pattern2);
            _rgx3 = new(pattern3);
            SearchBinFileCommand = new SearchBinFileCommand(this, _rgx3);
        }

        private void ReadBinFile(long buffer, long currentPos)
        {
            var newPos = currentPos - _previousPos;
            if (buffer == 0) return;
            else if (currentPos == 0)
            {
                Stream.Seek(currentPos, SeekOrigin.Begin); //Установка позиции
            }
            else if (IsSearch == true)
            {
                _con = 0;
                _count = 0;
                ResultStr = "";
                Stream.Seek(currentPos, SeekOrigin.Begin); //Установка позиции
            }
            else if (IsScroll == false)
            {
                ResultStr = ResultStr.Remove(0, _leng * (_maxLines / 100));
                _count -= _maxLines / 100;
                Stream.Seek(currentPos, SeekOrigin.Begin); //Установка позиции
                _con++;
            }
            else if (IsScroll == true)
            {
                ResultStr = ResultStr.Remove(_leng * (_maxLines - _maxLines / 100));
                _count -= _maxLines / 100;
                if (newPos < 160) newPos = 160;
                if (_con - 1 < 0) _con = 1;
                Stream.Seek(newPos * (_con - 1), SeekOrigin.Begin); //Установка позиции в начало
                _con--;
            }
            else return;

            var maxBytesRead = buffer; //Получаю количество байт
            var nBytesRead = 16; //Сколько байт нужно записать в 1 линию
            StringBuilder sb = new();

            _preOffset = Offset;
            _previousPos = currentPos;

            if (maxBytesRead > _maxLines * nBytesRead) maxBytesRead = _maxLines * nBytesRead; //Сколько можно за раз прочитать из файла байт

            while (maxBytesRead > 0)
            {
                var shift = Stream.Position.ToString("X8") + " : "; //Каждый 16 байт записывается смещение
                var buf = new byte[nBytesRead];
                var len = Stream.Read(buf, 0, buf.Length); //Заполнение массива байт и расчет длины массива
                var str16 = _rgx.Replace(BitConverter.ToString(buf), " ") + " | "; //Строка в байтовом представлении
                var str = _rgx2.Replace(Encoding.ASCII.GetString(buf), "."); //Строка в символьном представлении

                sb.AppendLine(shift + str16 + str);
                _count++;
                maxBytesRead -= len;
                CurrentPos = Stream.Position; //Запоминание последней позиции в потоке
                if (CurrentPos >= buffer) break;
                if (_count >= _maxLines) break;
            }

            if (_previousPos == 0) _con = 0;

            if (IsScroll != false && currentPos != 0)
            {
                CurrentPos = _previousPos - newPos;
                _previousPos -= newPos;
                ResultStr = ResultStr.Insert(0, sb.ToString());
            }
            else ResultStr = ResultStr.Insert(ResultStr.Length, sb.ToString());

            IsSearch = false;
        }
    }
}