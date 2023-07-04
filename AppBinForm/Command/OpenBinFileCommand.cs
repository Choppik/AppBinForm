using AppBinForm.Command.Base;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using AppBinForm.ViewModel;
using AppBinForm.Model;
using System.Text.RegularExpressions;
using System;
using System.Windows;
using System.Text;

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
            _filePath = "";
            OpenFile();

            if (_filePath != "")
            {
                _binFormViewModel.Shift = "";
                _binFormViewModel.Str16 = "";
                _binFormViewModel.Str = "";
                await ReadBinFile(_filePath);
            }
        }

        private void OpenFile()
        {
            OpenFileDialog openFile = new();
            if (openFile.ShowDialog() == true)
                _filePath = openFile.FileName;
        }

        private async Task ReadBinFile(string path)
        {
            string pattern = @"[-]";
            string pattern2 = @"[\n\r\t\a\b\f\0\v]";
            Regex rgx = new(pattern);
            Regex rgx2 = new(pattern2);

            using FileStream stream = new(path, FileMode.Open, FileAccess.Read);

            try
            {
            var con = 0;
            var buffer = stream.Length;

                var nBytesRead = buffer;

                if (nBytesRead > 65536)
                    nBytesRead = 65536;

                var buf = new byte[nBytesRead];

                var t = 0;
                /*while (t <= nBytesRead)
                {
                    if (t % 16 ==0)
                    {
                        var str16f = string.Format("{0,1:X}", t);
                        switch (str16f.Length)
                        {
                            case 0:
                            case 1:
                                str16f = str16f.Insert(0, "0000000");
                                break;
                            case 2:
                                str16f = str16f.Insert(0, "000000");
                                break;
                            case 3:
                                str16f = str16f.Insert(0, "00000");
                                break;
                            case 4:
                                str16f = str16f.Insert(0, "0000");
                                break;
                            case 5:
                                str16f = str16f.Insert(0, "000");
                                break;
                            case 6:
                                str16f = str16f.Insert(0, "00");
                                break;
                            case 7:
                                str16f = str16f.Insert(0, "0");
                                break;
                        }
                        _binFormViewModel.Shift += str16f + "\n";
                    }
                        t += 1;
                }*/

                await stream.ReadAsync(buf);
                _binFormViewModel.Str16 += rgx.Replace(BitConverter.ToString(buf), " ");
                _binFormViewModel.Str += rgx2.Replace(Encoding.ASCII.GetString(buf), ".");

                con++;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                _binFormViewModel.Pos = 15;
                stream.Close();
            }
        }
    }
}