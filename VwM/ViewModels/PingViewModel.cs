using System.ComponentModel.DataAnnotations;

namespace VwM.ViewModels
{
    public class PingViewModel
    {
        [Display(Name = "SelectMode")]
        public string Mode { get; set; }


        [Display(Name = "Hostnames")]
        [DataType(DataType.MultilineText)]
        public string Hostnames { get; set; }
    }
}
