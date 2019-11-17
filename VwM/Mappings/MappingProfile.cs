using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper;
using VwM.Controllers.API;
using VwM.ViewModels;
using VwM.Database.Filters;

namespace VwM.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RequestInfo, Pagination>();

            CreateMap<Models.API.DataTables.Request, Pagination>()
                .ForMember(dest => dest.Take, opt => { opt.SetMappingOrder(1); opt.MapFrom(src => src.Length); })
                .ForMember(dest => dest.Skip, opt => { opt.SetMappingOrder(2); opt.MapFrom(src => src.Start); });


            CreateMap<DeviceViewModel, Database.Models.Device>()
                .ForMember(dest => dest.Hostnames, opt => opt.MapFrom(src =>
                    src.SelectedHostnames.Select(a => a)));

            CreateMap<Database.Models.Device, DeviceViewModel>()
                .ForMember(dest => dest.Hostnames, opt => opt.MapFrom(src =>
                    src.Hostnames.Select(a => new SelectListItem { Value = a, Text = a, Selected = true })))
                .ForMember(dest => dest.SelectedHostnames, opt => opt.MapFrom(src =>
                    src.Hostnames.Select(a => a)));

            CreateMap<Database.Models.Whois, WhoisViewModel.ResultModel>();
        }
    }
}
