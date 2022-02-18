namespace CmdTools.Core.CmdMenuAndPages
{
    public interface IIOWrapper
    {
        void WritePageHeader(string header);
        void WriteLine(string message);
        void WriteException(Exception ex);


        void DisplayPrompt(string format, params object[] args);
        //int ReadInt(string prompt, int min, int max);
        int ReadInt(string prompt, int defaultValue, int min, int max);
        int ReadInt(int min, int max);
        int? ReadNullableInt(int min, int max);
        int ReadInt();
        //int? ReadNullableInt();
        string ReadString(string prompt);
        //TEnum ReadEnum<TEnum>(string prompt) where TEnum : struct, IConvertible, IComparable, IFormattable;
    }
}
