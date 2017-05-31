namespace SymbolFilterServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var arguments = new ArgumentsParser().Parse(args);
            var server = new SymbolFilterServer(arguments);
            server.Run();
        }
    }
}