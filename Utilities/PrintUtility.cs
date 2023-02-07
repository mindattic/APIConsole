using USPSAddressValidator.Extensions;

namespace USPSAddressValidator.Utilities
{
    public class PrintUtility
    {

        public void Text(string s = "")
        {
            Console.Out.WriteAsync(s);
        }

        public void Line(string s = "", int newLines = 1)
        {
            Console.Out.WriteAsync($"{s}{Environment.NewLine.Repeat(newLines)}");
        }

    }
}
