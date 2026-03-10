using System.Net.WebSockets;
using System.Text;

namespace BlogRealtime.Api.WebSockets;

public class WebSocketsManager
{
    private readonly List<WebSocket> _sockets = new();

    public void AddSocket(WebSocket socket)
    {
        _sockets.Add(socket);
    }

    public async Task BroadcastAsync(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);

        foreach (var socket in _sockets.Where(s => s.State == WebSocketState.Open))
        {
            await socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }
    }
}
