using AppBinForm.Command.Base;
using AppBinForm.Servies;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System;

namespace AppBinForm.Command
{
    public class OpenBinFileCommand : AsyncBaseCommand
    {
        #region Переменные и свойтва
        private byte[] _file;
        private string _filePath;

        private readonly INavigationService _navigationService;
        #endregion

        public OpenBinFileCommand(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        public async override Task ExecuteAsync(object? parameter)
        {
            OpenFile();
            _file = ReadFile(_filePath);
            WriteBinFile(_file);

            _navigationService.Navigate();
        }

        private void OpenFile()
        {
            OpenFileDialog openFile = new();
            openFile.ShowDialog();
            _filePath = openFile.FileName;
        }

        private static byte[] ReadFile(string path)
        {
            using (FileStream fs = new(path, FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Close();
                return buffer;
            }
        }
        private static byte[] WriteBinFile(byte[] buffer)
        {
            using (FileStream fs = new("fileBin.bin", FileMode.OpenOrCreate))
            {
                using (BinaryWriter bw = new(fs, Encoding.Default))
                {
                    bw.Write(buffer);
                    bw.Close();
                }
                fs.Close();
            }
            return buffer;
        }
    }
}