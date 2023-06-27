using System;

namespace AppBinForm.Model
{
    public class FileHeader
    {
        public UInt32 shift = 0;
        public UInt32 lenght = 0;
        public string name = "";

        public FileHeader(UInt32 shift, string name)
        {
            this.shift = shift;
            this.name = name;
        }
    }
}