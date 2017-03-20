using System;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!"); // yay
        }
        
        public static async Task Connect()
	{
		var cws = new ClientWebSocket();
		await cws.ConnectAsync(new Uri("wss://api.bitfinex.com/ws/2"), CancellationToken.None);

		string ping = @"{
			event:ping
		}";

		var buffer = enc.GetBytes(ping);
		cws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None).Wait();

		byte[] bufty = new byte[receiveChunkSize];
		var result = await cws.ReceiveAsync(new ArraySegment<byte>(bufty), CancellationToken.None);

		Console.WriteLine(result.MessageType);
		Console.WriteLine(result.Count);
		Console.WriteLine(enc.GetString(bufty));
	}
    }
}
