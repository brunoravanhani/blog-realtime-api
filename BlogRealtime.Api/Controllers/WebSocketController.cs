using Microsoft.AspNetCore.Mvc;
using BlogRealtime.Api.WebSockets;
using System.Net.WebSockets;

namespace BlogRealtime.Api.Controllers
{
    [ApiController]
    [Route("ws")]
    public class WebSocketController : ControllerBase
    {
        private readonly WebSocketsManager _manager;

        public WebSocketController(WebSocketsManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                _manager.AddSocket(socket);

                var buffer = new byte[1024];

                while (socket.State == WebSocketState.Open)
                {
                    await socket.ReceiveAsync(buffer, CancellationToken.None);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
