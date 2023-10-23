namespace Proxy.Forwarder.PoC.Client.Controllers;

using Microsoft.AspNetCore.Mvc;

public sealed class HomeController : Controller
{
    [Route(""), HttpGet, ApiExplorerSettings(IgnoreApi = true)]
    public RedirectResult Index()
    {
        return Redirect("swagger/index.html");
    }
}
