using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace VwM.Controllers
{
    public class MyController<T> : Controller
        where T : class
    {
        protected readonly ILogger<T> _logger;
        protected readonly IStringLocalizer<T> _lcz;


        public MyController(ILogger<T> logger, IStringLocalizer<T> localizer)
        {
            _logger = logger;
            _lcz = localizer;
        }


        internal string SanitizeReturnUrl(string returnUrl)
        {
            var req = HttpContext.Request;
            var baseUri = new Uri(string.Format("{0}://{1}", req.Scheme, req.Host));
            var uri = new Uri(baseUri, returnUrl);
            return uri.AbsoluteUri;
        }
    }
}
