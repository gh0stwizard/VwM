using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using VwM.Database.Collections;
using VwM.ViewModels;
using VwM.BackgroundServices;
using VwM.Database.Server;
using AutoMapper;

namespace VwM.Controllers
{
    [Authorize]
    public partial class ToolController : MyController<ToolController>
    {
        private readonly PingRequestQueue _pingQueue;
        private readonly WhoisRequestQueue _whoisQueue;
        private readonly DeviceCollection _devices;
        private readonly WhoisCollection _whois;
        private readonly ToolListViewModel _listModel;
        private readonly DatabaseStatus _dbStatus;
        private readonly IMapper _mapper;


        public ToolController(
            ILogger<ToolController> logger,
            IStringLocalizer<ToolController> localizer,
            DeviceCollection devices,
            WhoisCollection whoisCollection,
            PingRequestQueue pingQueue,
            WhoisRequestQueue whoisQueue,
            ToolListViewModel listModel,
            DatabaseStatus databaseStatus,
            IMapper mapper) : base(logger, localizer)
        {
            _devices = devices;
            _whois = whoisCollection;
            _pingQueue = pingQueue;
            _whoisQueue = whoisQueue;
            _listModel = listModel;
            _dbStatus = databaseStatus;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult List() => View(_listModel);
    }
}
