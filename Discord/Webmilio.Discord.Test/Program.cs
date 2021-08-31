using System;
using System.IO;
using System.IO.Compression;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Webmilio.Commons.Extensions;

namespace Webmilio.Discord.Test
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var config = await PrivateConfiguration.Load($"{Environment.CurrentDirectory}/private.json");

            Console.WriteLine("Bot Token: {0}", config.BotToken[..8]
                .Insert(8, new string('*', config.BotToken.Length - 8)));

            CancellationTokenSource cts = new();
            var ct = cts.Token;
            Uri gateway = new("wss://gateway.discord.gg/?v=9&encoding=json");

            using var ws = new ClientWebSocket();
            await ws.ConnectAsync(gateway, ct);

            var payload = new byte[1024 * 4];
            await ws.ReceiveAsync(payload = new byte[payload.Length], ct);

            int lr = -1;

            payload.Do(b =>
            {
                if (b != 0)
                {
                    lr = b;
                    return;
                }

                if (lr != -1)
                {
                    
                }
            });
            payload.Do(b => Console.Write("{0:x2} ", b));

            using (DeflateStream ds = new(new MemoryStream(payload), CompressionMode.Decompress))
            {
                using StreamReader sr = new(ds);

                var content = await sr.ReadToEndAsync();
                Console.WriteLine(content);
            }


            System.Diagnostics.Debugger.Break();

#if !DEBUG
            Console.ReadKey();
#endif
        }
    }
}
