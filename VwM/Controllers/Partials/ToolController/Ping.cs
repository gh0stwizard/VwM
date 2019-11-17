using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using VwM.ViewModels;
using VwM.BackgroundServices.Ping;

namespace VwM.Controllers
{
    [Authorize]
    public partial class ToolController : MyController<ToolController>
    {
        private const int MaxPingHosts = 2000;


        [HttpGet]
        public IActionResult Ping() => View(new PingViewModel());


        [HttpGet]
        public IActionResult StartPing() => RedirectToAction("Ping");


        [HttpPost]
        public async Task<IActionResult> StartPing(PingViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Ping", model);

            List<PingDto> requestDtos;
            var resultModel = new PingResultViewModel();

            if (model.Mode == "DB")
            {
                // unsupported by mongodb at 13.11.2019
                //requestDtos = await _devices.Query()
                //    .SelectMany(a => a.Hostnames.Select(b => new PingDto(a.Name, b)))
                //    .ToListAsync();
                try
                {
                    if (!_dbStatus.Connected)
                        throw new TimeoutException();

                    var devices = await _devices.Query().Select(a => new { a.Name, a.Hostnames }).ToListAsync();
                    requestDtos = new List<PingDto>();
                    foreach (var device in devices)
                        foreach (var hostname in device.Hostnames)
                            requestDtos.Add(new PingDto(device.Name, hostname));
                }
                catch (Exception e)
                {
                    if (e is TimeoutException ||
                        e is EndOfStreamException)
                    {
                        ModelState.AddModelError("", _lcz["DbTimeout"]);
                        return View("Ping", model);
                    }
                    else
                    {
                        _logger.LogError(e, "StartPing Got Unhandeled Exception.");
                        var err = string.Format(_lcz["DbErrorFmt"], e.Message);
                        ModelState.AddModelError("", err);
                    }

                    return View("Ping", model);
                }
            }
            else if (string.IsNullOrEmpty(model.Hostnames))
            {
                ModelState.AddModelError("", _lcz["ErrorEmptyHosts"]);
                return View("Ping", model);
            }
            else
            {
                requestDtos = model.Hostnames
                    .Split(new string[] { "\n", "\r\n", ",", " " }, StringSplitOptions.RemoveEmptyEntries)
                    .Distinct()
                    .Select(a => new PingDto(a))
                    .ToList();

                if (requestDtos.Count > MaxPingHosts)
                {
                    ModelState.AddModelError("", string.Format(_lcz["ErrorTooManyHosts"], MaxPingHosts));
                    return View("Ping", model);
                }
            }

            resultModel.Id = _pingQueue.Add(requestDtos);
            resultModel.Total = requestDtos.Count;

            return View("PingResult", resultModel);
        }
    }
}
