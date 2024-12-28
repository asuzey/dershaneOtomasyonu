using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace dershaneOtomasyonu
{
    public class WebSocketManager
    {
        private ClientWebSocket _clientWebSocket;

        public event Action<string> OnMessageReceived;
        public event Action<string> OnConnectionStatusChanged;

        public WebSocketManager()
        {
            _clientWebSocket = new ClientWebSocket();
        }

        public async Task ConnectAsync(string uri)
        {
            try
            {
                await _clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                OnConnectionStatusChanged?.Invoke("WebSocket bağlantısı kuruldu.");

                _ = ListenForMessages(); // Mesajları dinlemeye başla
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged?.Invoke($"Bağlantı hatası: {ex.Message}");
            }
        }

        public async Task DisconnectAsync()
        {
            if (_clientWebSocket.State == WebSocketState.Open)
            {
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Kapatılıyor", CancellationToken.None);
                OnConnectionStatusChanged?.Invoke("WebSocket bağlantısı kapatıldı.");
            }
        }

        public async Task SendMessageAsync(object message)
        {
            if (_clientWebSocket.State != WebSocketState.Open)
            {
                OnConnectionStatusChanged?.Invoke("WebSocket bağlantısı kapalı.");
                return;
            }

            var messageJson = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);

            try
            {
                await _clientWebSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged?.Invoke($"Mesaj gönderme hatası: {ex.Message}");
            }
        }

        private async Task ListenForMessages()
        {
            var buffer = new byte[1024];

            while (_clientWebSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await DisconnectAsync();
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived?.Invoke(message);
                }
                catch (Exception ex)
                {
                    OnConnectionStatusChanged?.Invoke($"Mesaj alma hatası: {ex.Message}");
                    break;
                }
            }
        }
    }
}
