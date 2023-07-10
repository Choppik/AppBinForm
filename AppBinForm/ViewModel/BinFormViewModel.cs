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
        private StringBuilder _sb;
        private FileStream _stream;

        private bool _isOpen = false;
        private bool _isScroll = false;
        private bool _isSearch = false;
        private string _filePath = "";
        private string _shift = "";
        private string _str16 = "";
        private string _str = "";
        private string _resultStr = "";
        private string _strSearch = "00000000";
        private long _currentPos = 0;
        private long _previousPos = 0;
        private long _buffer = 0;
        private int _leng = 0;
        private int _count = 0;
        private int _con = 0;
        private int _offset = 0;
        private int _preOffset = 0;
        private int _maxLines = 1000;

        private readonly string pattern = @"[-]";
        private readonly string pattern2 = @"[\n\r\t\a\b\f\0\v\u0000-\u0033]";
        private readonly string pattern3 = @"[^A-Fa-f0-9]";
        private Regex _rgx;
        private Regex _rgx2;
        private Regex _rgx3;

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
                if (value != null)
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
                if (IsOpen == true && IsSearch == false)
                    ReadBinFile(_buffer, CurrentPos);
            }
        }
        public ICommand OpenBinFileCommand { get; }
        public ICommand SearchBinFileCommand { get; }

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
            if(IsSearch == true)
            {
                _count = 0;
                _con = 0;
                SearcheLine(buffer, currentPos);
                return;
            } else
            if (currentPos == 0)
            {
                ReadFileBottom(buffer, currentPos);
                _con = 0;
                return;
            } else
            if (currentPos < buffer && IsScroll == false)
            {
                ResultStr = ResultStr.Remove(0, _leng * (_maxLines / 10));
                _count -= _maxLines / 10;
                ReadFileBottom(buffer, currentPos);
                return;
            }
            else if (IsScroll == true)
            {
                ResultStr = ResultStr.Remove(_leng * (_maxLines - _maxLines / 10));
                _count -= _maxLines / 10;
                ReadFileTop(buffer, currentPos);
                return;
            }
        }

        private void ReadFileBottom(long buffer, long currentPos)
        {
            _preOffset = Offset;
            _previousPos = currentPos;
            _con++;
            var maxBytesRead = buffer; //Получаю количество байт
            var nBytesRead = 16; //Сколько байт нужно записать в 1 линию

            Stream.Seek(currentPos, SeekOrigin.Begin); //Установка позиции

            if (maxBytesRead > 65536 / 4) maxBytesRead = 65536 / 4; //Сколько нужно за раз прочитать из файла байт

            Sb = new();

            while (maxBytesRead > 0)
            {
                Shift = Stream.Position.ToString("X8") + " : "; //Каждый 16 байт записывается смещение

                var buf = new byte[nBytesRead];
                int len = Stream.Read(buf, 0, buf.Length); //Заполнение массива байт и расчет длины массива

                Str16 = _rgx.Replace(BitConverter.ToString(buf), " ") + " | "; //Строка в байтовом представлении
                Str = _rgx2.Replace(Encoding.ASCII.GetString(buf), "."); //Строка в символьном представлении
                Sb.Append(Shift);
                Sb.Append(Str16);
                Sb.Append(Str);
                Sb.Append('\n'); //Объединение строк в одну строку
                _count++;
                maxBytesRead -= len;
                if (CurrentPos >= buffer) break; //Не должно превышать максимум линий
                if (_count >= _maxLines) break; //Не должно превышать максимум линий
            }
            //_blocks.Add(value, Sb.ToString());
            CurrentPos = Stream.Position; //Запоминание последней позиции в потоке
            ResultStr = ResultStr.Insert(ResultStr.Length, Sb.ToString());
            _leng = 78;
        }
        private void ReadFileTop(long buffer, long currentPos)
        {
            _preOffset = Offset;
            var newPos = currentPos - _previousPos;

            var maxBytesRead = buffer; //Получаю количество байт
            var nBytesRead = 16; //Сколько байт нужно записать в 1 линию

            Stream.Seek(newPos * (_con - 1), SeekOrigin.Begin); //Установка позиции
            _con--;

            if (maxBytesRead > 65536 / 4) maxBytesRead = 65536 / 4; //Сколько нужно за раз прочитать из файла байт

            Sb = new();

            while (maxBytesRead > 0)
            {
                Shift = Stream.Position.ToString("X8") + " : "; //Каждый 16 байт записывается смещение

                var buf = new byte[nBytesRead];
                int len = Stream.Read(buf, 0, buf.Length); //Заполнение массива байт и расчет длины массива

                Str16 = _rgx.Replace(BitConverter.ToString(buf), " ") + " | "; //Строка в байтовом представлении
                Str = _rgx2.Replace(Encoding.ASCII.GetString(buf), "."); //Строка в символьном представлении
                Sb.Append(Shift);
                Sb.Append(Str16);
                Sb.Append(Str);
                Sb.Append('\n'); //Объединение строк в одну строку
                _count++;
                maxBytesRead -= len;
                if (_count >= _maxLines) break; //Не должно превышать максимум линий
            }
            CurrentPos = _previousPos;
            _previousPos -= newPos;
            ResultStr = ResultStr.Insert(0, Sb.ToString());
            _leng = 78;
        }
        private void SearcheLine(long buffer, long currentPos)
        {
            ResultStr = "";
            _preOffset = Offset;
            _previousPos = currentPos;
            _con++;
            var maxBytesRead = buffer; //Получаю количество байт
            var nBytesRead = 16; //Сколько байт нужно записать в 1 линию

            Stream.Seek(currentPos, SeekOrigin.Begin); //Установка позиции

            if (maxBytesRead > 65536 / 4) maxBytesRead = 65536 / 4; //Сколько нужно за раз прочитать из файла байт

            Sb = new();

            while (maxBytesRead > 0)
            {
                Shift = Stream.Position.ToString("X8") + " : "; //Каждый 16 байт записывается смещение

                var buf = new byte[nBytesRead];
                int len = Stream.Read(buf, 0, buf.Length); //Заполнение массива байт и расчет длины массива

                Str16 = _rgx.Replace(BitConverter.ToString(buf), " ") + " | "; //Строка в байтовом представлении
                Str = _rgx2.Replace(Encoding.ASCII.GetString(buf), "."); //Строка в символьном представлении
                Sb.Append(Shift);
                Sb.Append(Str16);
                Sb.Append(Str);
                Sb.Append('\n'); //Объединение строк в одну строку
                _count++;
                maxBytesRead -= len;
                if (CurrentPos >= buffer) break; //Не должно превышать максимум линий
                if (_count >= _maxLines) break; //Не должно превышать максимум линий
            }
            CurrentPos = Stream.Position; //Запоминание последней позиции в потоке
            ResultStr = ResultStr.Insert(ResultStr.Length, Sb.ToString());
            _leng = 78;
            IsSearch = false;
        }
    }
}