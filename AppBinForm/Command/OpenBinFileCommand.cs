using AppBinForm.Command.Base;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using AppBinForm.ViewModel;
using AppBinForm.Model;
using System.Text;
using System.Collections.Generic;
using System;
using System.Windows.Media.TextFormatting;

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
            using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(stream, Encoding.ASCII, false);
            var con = 0;
            long buffer = stream.Length;

            int nCols = 16;
            int nBytesToRead = 0;

            StringBuilder shift = new(), strByte = new(), str = new();
            shift.Capacity = 4 * nCols;
            strByte.Capacity = 4 * nCols;
            str.Capacity = 4 * nCols;
            while (buffer > 0)
            {
                long nBytesRead = buffer;

                if (nBytesRead > 65536)
                    nBytesRead = 65536;

                //long nLines = (nBytesRead / nCols) + 1;


                /*                for (int j = 0; j < nBytesRead; j++)
                                {*/
                if (stream.Position % 16 == 0)
                {
                    var str16f = string.Format("{0,1:X}" + "\n", stream.Position);
                    if (str16f.Length == 1) str16f = str16f.Insert(0, "0000000"); else str16f = str16f.Insert(0, "000000");
                    shift.Append(str16f);
                }

                var nextByte = reader.ReadBytes(16);

                nBytesToRead++;
                foreach (byte b in nextByte)
                {
                    if (b < 0 || nBytesToRead > 65536)
                        break;

                    var nextChar = (char)b;
                    strByte.Append(string.Format("{0,1:X}" + " ", (int)b));
                    str.Append(nextChar);
                }
                //}


                con++;

                buffer -= 16;
            }
            _binFormViewModel.Shift = shift.ToString();
            _binFormViewModel.Str16 = strByte.ToString();
            _binFormViewModel.Str = str.ToString();
            reader.Dispose();
            stream.Close();
        }
    }
}