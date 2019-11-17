using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VwM.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetLogin(this ClaimsPrincipal user) =>
            user.Claims.SingleOrDefault(a => a.Type == ClaimTypes.WindowsAccountName)?.Value;
    }
}
