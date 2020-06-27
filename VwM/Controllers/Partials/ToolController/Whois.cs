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
        public async Task<IActionResult> WhoisAsync()
        {
            var model = new WhoisViewModel();

            if (_dbStatus.Connected)
                model.History = await GetLastWhoisEntriesAsync();

            return View(model);
        }


        [HttpGet]
        public IActionResult StartWhois() => RedirectToAction("Whois");


        [HttpPost]
        public async Task<IActionResult> StartWhoisAsync(WhoisViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Whois", model);

            if (_dbStatus.Connected)
            {
                var entry = await _whois
                        .Query(a => a.Hostname == model.Hostname)
                        .Select(a => new { a.Updated, a.Result })
                        .SingleOrDefaultAsync();

                if (entry != null
                    && DateTime.UtcNow.Subtract(entry.Updated).TotalDays < WhoisMinUpdateDays)
                {
                    model.Updated = entry.Updated;
                    model.Result = entry.Result;
                    model.Id = null;
                }

                model.History = await GetLastWhoisEntriesAsync();
            }

            if (!model.Updated.HasValue)
            {
                var dtos = new List<WhoisDto> { new WhoisDto(model.Hostname) };
                model.Id = _whoisQueue.Add(dtos);
            }

            return View("Whois", model);
        }


        private async Task<List<WhoisViewModel.ResultModel>> GetLastWhoisEntriesAsync(int take = 10)
        {
            var entries = await _whois.Query()
                .OrderByDescending(a => a.Updated)
                .Take(take)
                .ToListAsync();

            return entries
                .AsQueryable()
                .UseAsDataSource(_mapper)
                .For<WhoisViewModel.ResultModel>()
                .ToList();
        }
    }
}
