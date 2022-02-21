namespace CmdTools.Core.CmdMenuAndPages
{
    public interface IIOWrapper
    {
        void WritePageHeader(string header);
        void WriteLine(string message);
        void WriteException(Exception ex);

        bool GetConfirmation(string text, bool defaultValue = true);

        int ReadInt(string prompt, int defaultValue, int min, int max);
        string ReadString(string prompt);      
    }
}
