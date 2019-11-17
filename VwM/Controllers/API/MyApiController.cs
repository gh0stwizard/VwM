using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using VwM.Extensions;

namespace VwM.Controllers.API
{
    [Authorize]
    public class MyApiController : ControllerBase
    {
        public RequestInfo ParseQuery()
        {
            var ri = new RequestInfo();
            var query = Request.Query;
            StringValues values;
            int integer;


            if (query.TryGetValue("q", out values))
                ri.Search = values.FirstOrDefault();

            if (query.TryGetValue("limit", out values) &&
                int.TryParse(values.FirstOrDefault(), out integer))
                ri.Take = integer;

            if (query.TryGetValue("page", out values) &&
                int.TryParse(values.FirstOrDefault(), out integer))
                ri.Page = integer;

            if (query.TryGetValue("select2", out values))
                ri.UseSelect2 = values.FirstOrDefault().ParseBool(false);

            return ri;
        }
    }
}
