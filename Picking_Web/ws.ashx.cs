using Microsoft.Web.WebSockets;
using System.Web;

namespace Picking_Web
{
    /// <summary>
    /// Summary description for ws
    /// </summary>
    public class ws : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest) { }
                //context.AcceptWebSocketRequest(new TestWebSocketHandler());

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}