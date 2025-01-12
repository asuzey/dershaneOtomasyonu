using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

public class WebSocketClient
{
    private ClientWebSocket _webSocket;
    public WebSocketState State => _webSocket.State;

    public WebSocketClient()
    {
        _webSocket = new ClientWebSocket();
    }

    public async Task ConnectAsync(string uri)
    {
        if (_webSocket.State != WebSocketState.Open)
        {
            try
            {
                _webSocket = new ClientWebSocket();
                _webSocket.Options.SetBuffer(1024 * 1024, 1024 * 1024);
                await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                Console.WriteLine($"Connected to WebSocket server at {uri}");
            }
            catch (Exception ex)
            {
                throw new Exception("WebSocket connection failed.", ex);
            }
        }
        else
        {
            Console.WriteLine("WebSocket already connected.");
        }
    }

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
                throw new Exception("Error while sending message.", ex);
            }
        }
        else
        {
            throw new Exception("WebSocket is not connected.");
        }
    }

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
                    throw new Exception("WebSocket connection closed by the server.");
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Message received: " + message);
                return message;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while receiving message.", ex);
            }
        }
        else
        {
            throw new Exception("WebSocket is not connected.");
        }
    }

    public async Task DisconnectAsync()
    {
        if (_webSocket.State == WebSocketState.Open)
        {
            try
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                Console.WriteLine("Disconnected from WebSocket server.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error while disconnecting.", ex);
            }
        }
    }
}
