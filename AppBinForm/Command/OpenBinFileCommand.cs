using AppBinForm.Command.Base;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using AppBinForm.ViewModel;
using System.Windows.Shapes;
using AppBinForm.Model;
using System.Text;
using System.Windows.Media.TextFormatting;
using System.Linq;
using System;

namespace AppBinForm.Command
{
    public class OpenBinFileCommand : AsyncBaseCommand
    {
        #region Переменные и свойтва
        private string _filePath;

        private BinFormViewModel _binFormViewModel;
        #endregion

        public OpenBinFileCommand(BinFormViewModel binFormViewModel)
        {
            _binFormViewModel = binFormViewModel;
        }
        public async override Task ExecuteAsync(object? parameter)
        {
            OpenFile();
            _binFormViewModel.FileBytes = new FileBytes();
            await ReadBinFile(_filePath);
        }

        private void OpenFile()
        {
            OpenFileDialog openFile = new();
            openFile.ShowDialog();
            _filePath = openFile.FileName;
        }

        private async Task ReadBinFile(string path)
        {
            using (FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite))
            {
                int nCols = 16; //число символов
                long nBytesRead = stream.Length; //длинна файла
                if (nBytesRead > 65536 / 4)
                    nBytesRead = 65536 / 4;
                long nLines = (nBytesRead / nCols) + 1; //количество строк
                string[] lines = new string[nLines];
                string[] lines2 = new string[nLines];
                string[] lines3 = new string[nLines];
                int nBytesToRead = 0;

                for (int i = 0; i < nLines; i++)
                {
                    StringBuilder bit16 = new(), shift = new(), str = new()
                    {
                        Capacity = 4 * nCols
                    };

                    for (int j = 0; j < 16; j++)
                    {
                        var nextByte = stream.ReadByte();
                        var str16form = string.Format("{0,1:X}", stream.Position);

                        while (str16form.Length < 8) str16form = str16form.Insert(0, "0");
                        shift.Append(str16form + " ");

                        nBytesToRead++;

                        if (nextByte < 0 || nBytesToRead > stream.Length)
                            break;

                        char nextChar = (char)nextByte;
                        bit16.Append(string.Format("{0,1:X}" + " ", (int)nextChar));
                        str.Append(nextChar);
                    }
                    lines[i] = bit16.ToString();
                    lines2[i] = str.ToString();
                    lines3[i] = shift.ToString();
                }
                stream.Close();
                string text = "";
                foreach (string l in lines)
                    text += l;
                _binFormViewModel.FileBytes.Shift = text;
            }
        }
    }
}