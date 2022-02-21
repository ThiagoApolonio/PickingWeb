using Microsoft.Web.WebSockets;

namespace Picking_Web
{
    public class TestWebSocketHandler : WebSocketHandler
    {
        private static WebSocketCollection clients = new WebSocketCollection();

        private string name;

        public override void OnOpen()
        {
            name = WebSocketContext.QueryString["name"];

            clients.Add(this);

            clients.Broadcast(name);
        }

        public override void OnMessage(string message)
        {
            clients.Broadcast(name);
        }

        public override void OnClose()
        {
            clients.Remove(this);

            //clients.Broadcast(string.Format("{0} has gone away.", name));

        }

    }
}