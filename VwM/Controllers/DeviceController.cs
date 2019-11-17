using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoMapper;
using VwM.Database.Models;
using VwM.Database.Collections;
using VwM.Database.Filters;
using VwM.ViewModels;

namespace VwM.Controllers
{
    [Authorize]
    public class DeviceController : MyController<DeviceController>
    {
        private readonly DeviceCollection _devices;
        private readonly IMapper _mapper;


        public DeviceController(
            ILogger<DeviceController> logger,
            IStringLocalizer<DeviceController> localizer,
            DeviceCollection devices,
            IMapper mapper) : base(logger, localizer)
        {
            _devices = devices;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult List() => View();


        [HttpGet]
        public IActionResult New() => View("Form", new DeviceViewModel());


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var entry = await _devices.GetFirstOrDefaultAsync(a => a.Id == id);

            if (entry == null)
                return NotFound();

            var model = _mapper.Map<DeviceViewModel>(entry);

            return View("Form", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(DeviceViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            try
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                {
                    // Insert
                    var entry = _mapper.Map<Device>(model);
                    await _devices.InsertOneAsync(entry);
                    model.Id = entry.Id;
                }
                else
                {
                    // Update
                    var entry = await _devices.GetFirstOrDefaultAsync(a => a.Id == model.Id);

                    if (entry == null)
                        return NotFound();

                    _mapper.Map(model, entry);
                    var result = await _devices.ReplaceOneAsync(a => a.Id == model.Id, entry);

                    if (result.MatchedCount != 1)
                        return NotFound();
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View("Form", model);
            }

            return RedirectToAction("List");
        }
    }
}
