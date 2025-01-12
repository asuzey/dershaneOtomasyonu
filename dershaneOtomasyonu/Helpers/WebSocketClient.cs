using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dershaneOtomasyonu.Helpers
{
    public class WebSocketClient
    {
        private ClientWebSocket _webSocket;

        public WebSocketClient()
        {
            _webSocket = new ClientWebSocket();
        }

        /// <summary>
        /// WebSocket sunucusuna bağlanır.
        /// </summary>
        public async Task ConnectAsync(string uri)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                _webSocket = new ClientWebSocket();
                _webSocket.Options.SetBuffer(1024 * 1024, 1024 * 1024);
                await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                Console.WriteLine($"Connected to WebSocket server at {uri}");
            }
            else
            {
                Console.WriteLine("WebSocket already connected.");
            }
        }

        /// <summary>
        /// WebSocket üzerinden bir mesaj gönderir.
        /// </summary>
        public async Task SendMessageAsync(object message)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var messageJson = JsonConvert.SerializeObject(message);
                    var messageBytes = Encoding.UTF8.GetBytes(messageJson);

                    await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine("Message sent: " + messageJson);
                }
                catch (Exception ex)
                {

                    throw;
                }
                
            }
            else
            {
                Console.WriteLine("WebSocket is not connected.");
            }
        }

        /// <summary>
        /// WebSocket üzerinden mesaj alır.
        /// </summary>
        public async Task<string> ReceiveMessageAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                var buffer = new byte[1024 * 1024];
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                        Console.WriteLine("WebSocket connection closed.");
                        return null;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine("Message received: " + message);
                    return message;
                }
                catch (Exception ex)
                {

                    throw;
                }


            }
            else
            {
                Console.WriteLine("WebSocket is not connected.");
                return null;
            }
        }

        /// <summary>
        /// WebSocket bağlantısını kapatır.
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                Console.WriteLine("Disconnected from WebSocket server.");
            }
        }
    }
}
