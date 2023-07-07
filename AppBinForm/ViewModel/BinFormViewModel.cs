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
        private string _filePath = "";
        private string _shift = "";
        private string _str16 = "";
        private string _str = "";
        private string _resultStr = "";
        private long _currentPos = 0;
        private int _leng = 0;
        private int _count = 0;
        private int _offset = 0;
        private int _changesOffset = 500;
        private int _maxLines = 1000;
        private long _buffer = 0;

        private readonly string pattern = @"[-]";
        private readonly string pattern2 = @"[\n\r\t\a\b\f\0\v\u0000-\u0033]";
        private Regex _rgx;
        private Regex _rgx2;

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
                    _changesOffset = 500;
                    _buffer = Stream.Length;
                    ReadBinFile(_buffer, CurrentPos);
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
                if (_offset > _changesOffset)
                    ReadBinFile(_buffer, CurrentPos);
            }
        }
        public ICommand OpenBinFileCommand { get; }
        public ICommand SaveBinFileCommand { get; }

        public BinFormViewModel()
        {
            OpenBinFileCommand = new OpenBinFileCommand(this);
            Sb = new();
            _rgx = new(pattern);
            _rgx2 = new(pattern2);
            //SaveBinFileCommand = new SaveBinFileCommand();
        }

        private void ReadBinFile(long buffer, long currentPos)
        {
            if (buffer == 0) return;
            if (Offset > _changesOffset)
            {
                Sb.Remove(0, _leng * 100);
                _count -= 100;
                _changesOffset = Offset;
                Offset -= Offset;
            }
            /*else if (Offset < _changesOffset && Offset != 0)
            {
                Sb.Remove(_leng / _minLeng, _leng / _minLeng);
                _changesOffset -= _changesOffset;
            }*/


            //var count = 0;
            var maxBytesRead = buffer; //Получаю количество байт
            var nBytesRead = 16; //Сколько байт нужно записать в 1 линию

            Stream.Seek(currentPos, SeekOrigin.Begin); //Установка позиции
            //var f = Stream.Seek(0, SeekOrigin.End); //Установка позиции

            if (maxBytesRead > 65536 / 4) maxBytesRead = 65536 / 4; //Сколько нужно за раз прочитать из файла байт

            while (maxBytesRead > 0)
            {
                Shift = Stream.Position.ToString("X8") + " : "; //Каждый 16 байт записывается смещение

                var buf = new byte[nBytesRead];
                int len = Stream.Read(buf, 0, buf.Length); //Заполнение массива байт и расчет длины массива

                Str16 = _rgx.Replace(BitConverter.ToString(buf), " ") + " | "; //Строка в байтовом представлении
                Str = _rgx2.Replace(Encoding.ASCII.GetString(buf), "."); //Строка в символьном представлении
                Sb.Append(_shift);
                Sb.Append(_str16);
                Sb.Append(_str);
                Sb.Append('\n'); //Объединение строк в одну строку
                _count++;
                _leng = Sb.Length / _count;//78
                maxBytesRead -= len;
                CurrentPos = Stream.Position; //Запоминание последней позиции в потоке
                if (_count >= _maxLines) break; //Не должно превышать максимум линий
            }
            _buffer -= maxBytesRead;
            ResultStr = Sb.ToString();
        }
    }
}