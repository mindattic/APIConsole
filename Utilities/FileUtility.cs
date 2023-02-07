using System.Diagnostics;

namespace USPSAddressValidator.Utilities
{
    public class FileUtility
    {
        private string fileName;
        private string textEditor;

        public FileUtility(string fileName, string textEditor = "notepad.exe")
        {
            this.fileName = fileName;
            this.textEditor = textEditor;
        }

        public void Save(string s)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            File.Create(fileName).Close();
            File.WriteAllText(fileName, s);
        }

        public void Open()
        {
            Process.Start(textEditor, fileName);
        }

    }
}
