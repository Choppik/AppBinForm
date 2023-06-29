namespace AppBinForm.Model
{
    public class FileBytes
    {
        public string Shift;
        public string StrByte16;
        public string StrByte;

        public FileBytes() { }
        public FileBytes(string shift, string strByte16, string strByte = "")
        {
            Shift = shift;
            StrByte16 = strByte16;
            StrByte = strByte;
        }
    }
}