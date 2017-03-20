using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;

namespace HelloWorld
{
    class Program
    {
		static UTF8Encoding enc = new UTF8Encoding();
		private const int sendChunkSize = 1024;
		private const int receiveChunkSize = 1024;
		private static object consoleLock = new object();

		static string ping = @"{
""event"":""ping""
}";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!"); // yay

			Go().Wait();

			Console.WriteLine("All done!"); // yay
			//Console.ReadLine();

        }

		public static async Task Go()
		{
			var cws = new ClientWebSocket();

			await cws.ConnectAsync(new Uri("wss://api.bitfinex.com/ws/2"), CancellationToken.None);
			Console.WriteLine("Connected");

			var msg = await ReceiveAsync(cws);
			Console.WriteLine(msg);
			Console.WriteLine();
			Console.WriteLine();

			//Console.WriteLine("Sending ping: " + ping);

			var msg2 = await SendReceiveAsync(cws, enc.GetBytes(Subscribe()));
			Console.WriteLine(msg2);
			Console.WriteLine();
			Console.WriteLine();

			await CloseAsync(cws);

		}

		//        public static async Task Connect()
		//		{
		//			var cws = new ClientWebSocket();

		//			Console.WriteLine("Connecting");
		//			await cws.ConnectAsync(new Uri("wss://api.bitfinex.com/ws/2"), CancellationToken.None);
		//			Console.WriteLine("Connected");

		//			string ping = @"{
		//""event"":""ping""
		//}";

		//			Console.WriteLine(ping);


		//			byte[] bufty = new byte[receiveChunkSize];
		//			var result = await cws.ReceiveAsync(new ArraySegment<byte>(bufty), CancellationToken.None);

		//			Console.WriteLine(enc.GetString(bufty));
		//			Console.WriteLine();
		//			Console.WriteLine();

		//			var payload = enc.GetBytes(ping);
		//			await cws.SendAsync(new ArraySegment<byte>(payload), WebSocketMessageType.Text, true, CancellationToken.None);

		//			bufty = new byte[receiveChunkSize];
		//			result = await cws.ReceiveAsync(new ArraySegment<byte>(bufty), CancellationToken.None);

		//			Console.WriteLine();
		//			Console.WriteLine();

		//			Console.WriteLine(enc.GetString(bufty));


		//			if (cws.State != WebSocketState.Closed)
		//			{
		//				await cws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
		//			}
		//			cws.Dispose();

		//			Console.WriteLine();
		//			Console.WriteLine(cws.State);
		//			Console.WriteLine(cws.CloseStatus.ToString());

		//		}

		static string Subscribe()
		{
			string s = @"{
""event"": ""subscribe"",
""channel"": ""ticker"",
""symbol"": ""BTCUSD""
		}";

			return s;
		}

		public static async Task<string> ReceiveAsync(ClientWebSocket cws)
		{
			try
			{
				byte[] receiveBuffer = new byte[receiveChunkSize];

				WebSocketReceiveResult receiveResult =
					await cws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

				return enc.GetString(receiveBuffer);
			}
			catch (WebSocketException exweb)
			{
				Console.WriteLine(exweb.Message);
				CloseAsync(cws).Wait();
			}

			return null;
		}

		public static async Task<string> SendReceiveAsync(ClientWebSocket cws, byte[] payload)
		{
			try
			{
				// Send request operation
				await cws.SendAsync(new ArraySegment<byte>(payload), WebSocketMessageType.Text, true, CancellationToken.None);

				byte[] receiveBuffer = new byte[receiveChunkSize];

				WebSocketReceiveResult receiveResult =
					await cws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

				return enc.GetString(receiveBuffer);
			}
			catch (WebSocketException exweb)
			{
				Console.WriteLine(exweb.Message);
				CloseAsync(cws).Wait();
			}

			return null;
		}

		public static async Task CloseAsync(ClientWebSocket cws)
		{
			try
			{
				if (cws!= null)
				{
					if (cws.State != WebSocketState.Closed)
					{
						await cws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
					}
					cws.Dispose();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				cws = null;
			}
		}
	





    }
}
