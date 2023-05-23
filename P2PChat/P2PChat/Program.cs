namespace P2PChat
{
    public static class Program
    {
        public static void Main(string[] _)
        {
            var server = new Server();
            
            Console.WriteLine("Server is listening...");

            while (true)
            {
                if ("q" != Console.ReadLine()) continue;
                server.Dispose();
                break;
            }
        }
    }
}
