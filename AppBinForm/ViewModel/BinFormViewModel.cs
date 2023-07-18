using AppBinForm.ViewModel.Base;
using System.Windows.Input;
using AppBinForm.Command;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System;
using AppBinForm.Model;

namespace AppBinForm.ViewModel
{
    public class BinFormViewModel : BaseViewModel
    {
        #region Переменные и свойства
        private FileStream _stream;

        private bool _isOpen = false;
        private bool _isScroll = false;
        private bool _isSearch = false;
        private bool _isChecked = true;
        private string _filePath = "";
        private string _resultStr = "";
        private string _strSearch = "00000000";
        private double _offset = 0.00;
        private double _maxOffset = 0.00;
        private double _maxShProc = 0.00;
        private double _minShProc = 0.00;
        private int _count = 1;
        private long _currentPos = 0;
        private long _previousPosition = 0;
        private long _buffer = 0;
        private string _size = "0";
        private readonly long _sizeFile = 4000000000;
        private readonly int _nBytesRead = 16; //Сколько байт нужно записать в 1 линию
        private readonly int _maxLines = 500;
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
                if (!_isOpen)
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
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
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
                if (_isSearch)
                {
                    ReadBinFile(_buffer, CurrentPosition);
                }
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
                    _buffer = Stream.Length;
                    Offset = 0.00;
                    ReadBinFile(_buffer, CurrentPosition);
                }
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
        public string Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
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
        public long CurrentPosition
        {
            get
            {
                return _currentPos;
            }
            set
            {
                _currentPos = value;
                OnPropertyChanged(nameof(CurrentPosition));
            }
        }
        public double Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                OnPropertyChanged(nameof(Offset));
                if (IsChecked)
                {
                    if (_offset == 0 && !(CurrentPosition - _previousPosition <= _previousPosition))
                    {
                        IsScroll = true;
                        _maxShProc = _offset;
                        ReadBinFile(_buffer, CurrentPosition);
                    }
                    else if (_offset == MaxOffset && CurrentPosition != _buffer && CurrentPosition != 0)
                    {
                        IsScroll = false;
                        _maxShProc = _offset;
                        ReadBinFile(_buffer, CurrentPosition);
                    }
                }
                else
                {
                    if (_offset != 0 && _offset > _minShProc && _maxShProc <= _offset)
                    {
                        if (_offset / _minShProc >= _count + 1)
                        {
                            IsScroll = false;
                            _count = (int)(_offset / _minShProc);
                            _maxShProc = _offset;
                            ReadBinFileInPercent(_buffer, CurrentPosition, _count);
                        }
                    }
                    else if (_offset != MaxOffset && _offset < _maxShProc)
                    {
                        if (_offset / _minShProc <= _count - 1)
                        {
                            IsScroll = true;
                            _count = (int)(_offset / _minShProc);
                            _maxShProc = _offset;
                            ReadBinFileInPercent(_buffer, CurrentPosition, _count);
                        }
                    }
                }
            }
        }
        public double MaxOffset
        {
            get
            {
                return _maxOffset;
            }
            set
            {
                _maxOffset = value;
                OnPropertyChanged(nameof(MaxOffset));
                if (MaxOffset != 0)
                {
                    _minShProc = MaxOffset / 100;
                }
            }
        }
        public ICommand OpenBinFileCommand { get; }
        public ICommand SearchBinFileCommand { get; }
        #endregion

        public BinFormViewModel()
        {
            OpenBinFileCommand = new OpenBinFileCommand(this);
            _rgx = new(pattern);
            _rgx2 = new(pattern2);
            _rgx3 = new(pattern3);
            SearchBinFileCommand = new SearchBinFileCommand(this, _rgx3);
        }

        private void ReadBinFile(long buffer, long currentPosition)
        {
            if (buffer == 0) return; //Если размер файла 0, то выйти
            else if (currentPosition == 0) Stream.Seek(currentPosition, SeekOrigin.Begin); //Установка начальной позиции
            else if (IsSearch) Stream.Seek(currentPosition, SeekOrigin.Begin); //Установка конкретной позиции
            else if (!IsScroll) Stream.Seek(currentPosition - _previousPosition, SeekOrigin.Begin); //Установка позиции при скроллинге вниз
            else if (IsScroll)
            {
                var newPosition = currentPosition - _previousPosition * 2;
                while (newPosition % 16 != 0)
                {
                    newPosition--;
                }
                if (newPosition != 0 && newPosition < _previousPosition) Stream.Seek(0, SeekOrigin.Begin); //Установка позиции при скроллинге вверх
                else Stream.Seek(newPosition - _previousPosition, SeekOrigin.Begin); //Установка позиции при скроллинге вверх
            }
            else return;

            StringBuilder sb = ReadPartFile(buffer);

            if (currentPosition == 0)
            {
                _previousPosition = CurrentPosition / 2;
                if (Offset != 0) Offset = 0;
            }
            else if (CurrentPosition != buffer && Offset != 0 && !IsSearch) Offset = MaxOffset / 2; //Проверки, когда нужно сместить ползунок в середину
            else if (currentPosition != 0 && Offset == 0 && MaxOffset != 0) Offset = MaxOffset / 2;
            else if (IsSearch && CurrentPosition != buffer) Offset = 1;

            ResultStr = ResultStr.Insert(ResultStr.Length, sb.ToString());
            IsSearch = false;
        }

        private void ReadBinFileInPercent(long buffer, long currentPosition, int count)
        {
            if (buffer < _maxLines * _nBytesRead) return;
            if (buffer == 0) return; //Если размер файла 0, то выйти

            var buf = buffer / 100;
            while (buf % 16 != 0)
            {
                buf--;
            }

            var endFile = buffer - _nBytesRead * _maxLines;
            while (endFile % 16 != 0)
            {
                endFile++;
            }

            var curPos = buf * count;

            if (currentPosition == 0) Stream.Seek(currentPosition, SeekOrigin.Begin); //Установка начальной позиции
            //else if (buffer == currentPosition && !IsScroll) return;
            else if (count == 100 && !IsScroll) Stream.Seek(endFile, SeekOrigin.Begin);
            else if (!IsScroll) Stream.Seek(curPos, SeekOrigin.Begin); //Установка позиции при скроллинге вниз
            else if ((count == 0 && IsScroll) || curPos - buf < 0) Stream.Seek(0, SeekOrigin.Begin);
            else if (IsScroll) Stream.Seek(curPos - buf, SeekOrigin.Begin); //Установка позиции при скроллинге вверх
            else return;

            StringBuilder sb = ReadPartFile(buffer);
            ResultStr = ResultStr.Insert(ResultStr.Length, sb.ToString());
            IsSearch = false;
        }
        private StringBuilder ReadPartFile(long buffer)
        {
            var maxBytesRead = buffer; //Получаю количество байт для чтения
            var shift = "";
            var x8Shift = "X8";
            var x10Shift = "X10";
            StringBuilder sb = new();

            ResultStr = ResultStr.Remove(0);

            if (maxBytesRead > _maxLines * _nBytesRead) maxBytesRead = _maxLines * _nBytesRead; //Сколько можно за раз прочитать из файла байт

            while (maxBytesRead > 0)
            {
                switch (buffer > _sizeFile)  //Каждый 16 байт записывается смещение (формат меняется взависимости от размера файла)
                {
                    case true:
                        shift = Stream.Position.ToString(x10Shift);
                        break;
                    case false:
                        shift = Stream.Position.ToString(x8Shift);
                        break;
                }
                var buf = new byte[_nBytesRead];
                var lengthBuf = Stream.Read(buf, 0, buf.Length); //Заполнение массива байт и расчет длины массива
                var str16 = _rgx.Replace(BitConverter.ToString(buf), " "); //Строка в байтовом представлении
                var str = _rgx2.Replace(Encoding.ASCII.GetString(buf), "."); //Строка в символьном представлении
                FileBytes fb = new(shift, str16, str);
                sb.AppendLine(fb.ToString());
                maxBytesRead -= lengthBuf;
                CurrentPosition = Stream.Position; //Запоминание последней позиции в потоке
                if (CurrentPosition >= buffer) break;
            }
            return sb;
        }
    }
}