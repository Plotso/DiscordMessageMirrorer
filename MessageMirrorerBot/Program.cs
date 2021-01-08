namespace MessageMirrorerBot
{
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var messageMirrorer = new MessageMirrorer();
            await messageMirrorer.RunAsync();
        }
    }
}
