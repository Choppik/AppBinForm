using AppBinForm.Command.Base;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using AppBinForm.ViewModel;
using AppBinForm.Model;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Primitives;

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
            _binFormViewModel.Shift = "";
            _binFormViewModel.Str16 = "";
            _binFormViewModel.Str = "";

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
            int nCols = 16;

            using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(stream, Encoding.UTF8);

            var con = 0;
            var buffer = stream.Length;


            /*while (true)
            {*/
            var nBytesRead = buffer;

            if (nBytesRead > 65536)
                nBytesRead = 65536;

            var nLines = (int)(nBytesRead / nCols) + 1;

            string[] lines = new string[nLines];
            string[] lines2 = new string[nLines];
            string[] lines3 = new string[nLines];

            int nBytesToRead = 0;

            for (int i = 0; i < nLines; i++)
            {
                StringBuilder shift = new(), strByte = new(), str = new();
                shift.Capacity = 4 * nCols;
                strByte.Capacity = 4 * nCols;
                str.Capacity = 4 * nCols;

                for (int j = 0; j < 16; j++)
                {
                    if (stream.Position % 16 == 0)
                    {
                        var str16f = string.Format("{0,1:X}", stream.Position);
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
                        shift.Append(str16f);
                    }

                    var nextByte = reader.ReadByte();
                    nBytesToRead++;

                    if (nextByte < 0 || nBytesToRead > 65536)
                        break;

                    var nextChar = (char)nextByte;

                    var strf = string.Format("{0,1:X}" + " ", (int)nextChar);
                    if (strf.Length == 2) strf = strf.Insert(0, "0");
                    strByte.Append(strf);
                    str.Append(nextChar);

                    con++;

                }
                lines[i] = shift.ToString();
                lines2[i] = strByte.ToString();
                lines3[i] = str.ToString();

                //buffer -= nBytesRead;
            }
            reader.Close();
            stream.Close();

            string pattern = @"[\r\n\t\v\f\a\e\0]";
            Regex rgx = new(pattern);

            string text = "";
            string text2 = "";
            string text3 = "";

            foreach (string l in lines)
                text += l + "\n";
            foreach (string l in lines2)
                text2 += l + "\n";
            foreach (string l in lines3)
                text3 += rgx.Replace(l, ".") + "\n";

            _binFormViewModel.Shift = text;
            _binFormViewModel.Str16 = text2;
            _binFormViewModel.Str = text3;
        }
    }
}