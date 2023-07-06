using AppBinForm.ViewModel.Base;
using System.Windows.Input;
using AppBinForm.Command;
using AppBinForm.Model;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.Timers;

namespace AppBinForm.ViewModel
{
    public class BinFormViewModel : BaseViewModel
    {
        private StringBuilder _resultStr;

        private bool _isOpen = false;
        private string _filePath = "";
        private string _shift = "";
        private string _str16 = "";
        private string _str = "";
        private long _currentPos = 0;
        private int _offset = 0;

        private readonly string pattern = @"[-]";
        private readonly string pattern2 = @"[\n\r\t\a\b\f\0\v]";
        private Regex rgx;
        private Regex rgx2;
        private FileStream _stream;

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
        public string ResultStr => _resultStr.ToString();

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
                    ReadBinFile(_stream.Length, _currentPos);
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
        public int Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                if (_offset % 32 == 0)
                    ReadBinFile(32, _currentPos);
                OnPropertyChanged(nameof(Offset));
            }
        }
        public ICommand OpenBinFileCommand { get; }
        public ICommand SaveBinFileCommand { get; }

        public BinFormViewModel()
        {
            OpenBinFileCommand = new OpenBinFileCommand(this);
            _resultStr = new();
            rgx = new(pattern);
            rgx2 = new(pattern2);
            //SaveBinFileCommand = new SaveBinFileCommand(binFormViewModelNavigationService);
        }

        private void ReadBinFile(long buffer, long currentPos)
        {
            var maxBytesRead = buffer;
            var nBytesRead = 16;

            if (maxBytesRead > 65536) maxBytesRead = 65536;

            while (maxBytesRead > 0)
            {
                var buf = new byte[nBytesRead];

                int len = _stream.Read(buf, (int)currentPos, buf.Length);
                _shift = _stream.Position.ToString("X8") + " : ";
                _str16 = rgx.Replace(BitConverter.ToString(buf), " ") + " | ";
                /*foreach (byte b in buf)
                {
                    //_str16 += b.ToString("X2") + " ";
                    if (b >= 33 || b <= 126)
                        _str += b.ToString() + " ";
                    else _str += ". ";
                }*/
                _str = rgx2.Replace(Encoding.ASCII.GetString(buf), ".");
                _resultStr.Append(_shift);
                _resultStr.Append(_str16);
                _resultStr.Append(_str);
                _resultStr.Append("\n");
                maxBytesRead -= len;
            }
            _currentPos = _stream.Position;
        }
    }
}