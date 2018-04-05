using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebAppP2P.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment env)
        {
            _env = env;
        }

        public ViewResult Index()
        {
            var ticksBundle = System.IO.File.GetLastWriteTime(_env.WebRootPath + "\\bundle.js").Ticks;
            var serverUrl = $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";
            var ws = Request.IsHttps ? "wss" : "ws";
            var serverWsUrl = $"{ws}://{Request.Host.Host}:{Request.Host.Port}";

            ViewBag.BundleHash = ticksBundle;
            ViewBag.ServerUrl = serverUrl;
            ViewBag.ServerWsUrl = serverWsUrl;

            return View();
        }
    }
}
