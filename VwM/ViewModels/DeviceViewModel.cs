using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VwM.ViewModels
{
    public class DeviceViewModel
    {
        public string Id { get; set; }


        [MaxLength(512,
            ErrorMessageResourceType = typeof(Resources.ViewError),
            ErrorMessageResourceName = "MaxLengthString")]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [MaxLength(2000,
            ErrorMessageResourceType = typeof(Resources.ViewError),
            ErrorMessageResourceName = "MaxLengthString")]
        [Display(Name = "Description")]
        public string Description { get; set; }


        [Display(Name = "Hostname")]
        public IList<SelectListItem> Hostnames { get; set; } = new List<SelectListItem>();
        public IList<string> SelectedHostnames { get; set; }
    }
}
