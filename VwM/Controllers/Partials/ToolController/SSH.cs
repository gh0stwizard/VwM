using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using VwM.ViewModels;

namespace VwM.Controllers
{
    [Authorize]
    public partial class ToolController : MyController<ToolController>
    {
        [HttpGet]
        public IActionResult SSH() => View(new SshViewModel());
    }
}
