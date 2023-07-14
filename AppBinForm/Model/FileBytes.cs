namespace AppBinForm.Model
{
    public class FileBytes
    {
        private readonly string? _shift;
        private readonly string? _strByte16;
        private readonly string? _str;

        public FileBytes() { }

        public FileBytes(string shift, string strByte16 = "..", string str = ".")
        {
            _shift = shift;
            _strByte16 = strByte16;
            _str = str;
        }
        public override string ToString()
        {
            return $"{_shift + " : " + _strByte16 + " | " + _str}";
        }
    }
}