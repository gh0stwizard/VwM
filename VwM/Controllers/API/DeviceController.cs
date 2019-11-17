using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using VwM.Database.Models;
using VwM.Database.Collections;
using VwM.Database.Filters;
using VwM.Models.API.DataTables.Extensions;

namespace VwM.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : MyApiController
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly DeviceCollection _devices;
        private readonly IMapper _mapper;


        public DeviceController(
            ILogger<DeviceController> logger,
            DeviceCollection deviceService,
            IMapper mapper)
        {
            _logger = logger;
            _devices = deviceService;
            _mapper = mapper;
        }


        [HttpGet]
        [Route("")]
        public Task<List<Device>> Get()
        {
            var ri = ParseQuery();
            var pagination = _mapper.Map<Pagination>(ri);

            return _devices.GetListAsync();
        }


        [HttpPost]
        [Route("")]
        public async Task<IActionResult> GetDataTables([FromBody]Models.API.DataTables.Request request)
        {
            try
            {
                var result = await request.GetResponseAsync(_devices, _mapper);
                return Ok(result);
            }
            catch (TimeoutException)
            {
                return StatusCode(504);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetDataTables Got Unhandeled Exception.");
                return StatusCode(500);
            }
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ids = id.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray();
            var result = await _devices.DeleteOneAsync(a => ids.Contains(a.Id));

            if (result.DeletedCount == 0)
                return NotFound();

            return Ok(new { Deleted = result.DeletedCount });
        }
    }
}
