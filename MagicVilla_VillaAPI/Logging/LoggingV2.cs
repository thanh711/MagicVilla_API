namespace MagicVilla_VillaAPI.Logging
{
    public class LoggingV2 : ILogging
    {
        public void Log(string message, string type)
        {
            if (type == "error")
            {
                Console.BackgroundColor=ConsoleColor.Red;
                Console.WriteLine("error: " + message);
                Console.BackgroundColor = ConsoleColor.Black;

            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
