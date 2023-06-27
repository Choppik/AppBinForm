using AppBinForm.Command.Base;
using AppBinForm.Servies;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System;
using AppBinForm.Model;
using System.Collections.Generic;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Controls;

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
            _file = await ReadFile(_filePath);
            await WriteBinFile(_file);

            _navigationService.Navigate();
        }

        private void OpenFile()
        {
            OpenFileDialog openFile = new();
            openFile.ShowDialog();
            _filePath = openFile.FileName;
        }

        private async static Task<byte[]> ReadFile(string path)
        {
            using (FileStream fs = new(path, FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[fs.Length];
                await fs.ReadAsync(buffer, 0, buffer.Length);
                fs.Close();
                return buffer;
            }
        }
        private static async Task WriteBinFile(byte[] bytes)
        {
            using (FileStream stream = new("fileBin.dat", FileMode.Create, FileAccess.ReadWrite))
            {
                await stream.WriteAsync(bytes);
                stream.Close();
            }
        }
    }
}