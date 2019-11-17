using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AutoMapper.Extensions.ExpressionMapping;
using VwM.BackgroundServices.Whois;
using VwM.Database.Extensions;
using VwM.ViewModels;

namespace VwM.Controllers
{
    [Authorize]
    public partial class ToolController : MyController<ToolController>
    {
        private const int WhoisMinUpdateDays = 7;


        [HttpGet]
        public IActionResult Whois()
        {
            var model = new WhoisViewModel
            {
                History = GetLastWhoisEntriesAsync()
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult StartWhois() => RedirectToAction("Whois");


        [HttpPost]
        public async Task<IActionResult> StartWhoisAsync(WhoisViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Whois", model);

            var entry = await _whois
                .Query(a => a.Hostname == model.Hostname)
                .Select(a => new { a.Updated, a.Result })
                .SingleOrDefaultAsync();

            if (entry != null && DateTime.UtcNow.Subtract(entry.Updated).TotalDays < WhoisMinUpdateDays)
            {
                model.Updated = entry.Updated;
                model.Result = entry.Result;
                model.Id = null;
            }
            else
            {
                var dtos = new List<WhoisDto> { new WhoisDto(model.Hostname) };
                model.Id = _whoisQueue.Add(dtos);
            }

            model.History = GetLastWhoisEntriesAsync();

            return View("Whois", model);
        }


        private List<WhoisViewModel.ResultModel> GetLastWhoisEntriesAsync()
        {
            return _whois.Query()
                .OrderByDescending(a => a.Updated)
                .Take(10)
                .UseAsDataSource(_mapper)
                .For<WhoisViewModel.ResultModel>()
                .ToList();
        }
    }
}
