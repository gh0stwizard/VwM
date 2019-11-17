using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using VwM.Controllers;

namespace VwM.ViewModels
{
    public class ToolListViewModel
    {
        [Display(Name = "ProtocolType")]
        public Lazy<BaseType[]> ProtocolTypes { get; private set; }

        [Display(Name = "OsType")]
        public Lazy<BaseType[]> OsTypes { get; private set; }


        public ToolListViewModel(IStringLocalizer<ToolListViewModel> _lcz)
        {
            ProtocolTypes = new Lazy<BaseType[]>(() =>
            {
                return Tools
                    .SelectMany(a => a.ProtocolTypes)
                    .Select(a => a)
                    .Distinct()
                    .OrderBy(a => a.Name)
                    .ToArray();
            });

            OsTypes = new Lazy<BaseType[]>(() =>
            {
                return Tools
                    .SelectMany(a => a.OsTypes)
                    .Select(a => a)
                    .Where(a => a.TypeName != "all")
                    .Distinct()
                    .OrderBy(a => a.Name)
                    .ToArray();
            });


            var icmpTool = new Tool
            {
                Name = _lcz["Ping"],
                Action = "Ping",
                OsTypes = new BaseTypeList()
                {
                    new BaseType { Name = _lcz["All"], TypeName = "all" }
                },
                ProtocolTypes = new BaseTypeList()
                {
                    new BaseType { Name = "ICMP", TypeName = "icmp" }
                }
            };

            var whoisTool = new Tool
            {
                Name = _lcz["Whois"],
                Action = "Whois",
                OsTypes = new BaseTypeList()
                {
                    new BaseType { Name = _lcz["All"], TypeName = "all" }
                },
                ProtocolTypes = new BaseTypeList()
                {
                    new BaseType { Name = "WHOIS", TypeName = "whois" }
                }
            };

            var sshTool = new Tool
            {
                Name = _lcz["SSH"],
                Action = "SSH",
                OsTypes = new BaseTypeList()
                {
                    new BaseType { Name = _lcz["OsUnix"], TypeName = "unix" }
                },
                ProtocolTypes = new BaseTypeList()
                {
                    new BaseType { Name = "SSH", TypeName = "ssh" }
                }
            };

            Tools = new List<Tool>()
            {
                icmpTool,
                whoisTool,
                sshTool
            };
        }


        public IList<Tool> Tools { get; set; }


        public class Tool
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Action { get; set; }

            [Display(Name = "ProtocolType")]
            public BaseTypeList ProtocolTypes { get; set; }


            [Display(Name = "OsType")]
            public BaseTypeList OsTypes { get; set; }
        }


        public class BaseType
        {
            public string Name { get; set; }
            public string TypeName { get; set; }


            public override bool Equals(object obj)
            {
                if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    var me = (BaseType)obj;
                    return this.TypeName.Equals(me.TypeName);
                }
            }


            public override int GetHashCode()
            {
                return this.TypeName.GetHashCode();
            }
        }


        public class BaseTypeList : List<BaseType>
        {
            public override string ToString() =>
                string.Join(", ", this.Select(a => a.Name).ToList());
        }
    }


    public static class ToolListViewModelExtensions
    {
        public static string GetAllTypesJson<T>(this T tool)
            where T: ToolListViewModel.Tool
        {
            var allNames = new List<string>() { "all" };
            allNames.AddRange(tool.ProtocolTypes.Select(a => a.TypeName).ToList());
            allNames.AddRange(tool.OsTypes.Select(a => a.TypeName).ToList());
            return JsonConvert.SerializeObject(allNames);
        }
    }
}
