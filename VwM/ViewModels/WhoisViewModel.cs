using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VwM.ViewModels
{
    public class WhoisViewModel
    {
        [Required(
            ErrorMessageResourceType = typeof(Resources.ViewError),
            ErrorMessageResourceName = "RequiredString")]
        [MaxLength(254,
            ErrorMessageResourceType = typeof(Resources.ViewError),
            ErrorMessageResourceName = "MaxLengthString")]
        [Display(Name = "Hostname")]
        public string Hostname { get; set; }

        // from db
        public DateTime? Updated { get; set; }
        public string Result { get; set; }


        public Guid? Id { get; set; }
        public IList<ResultModel> History { get; set; }
        public bool HasHistory { get { return History != null && History.Count >= 1; } }


        public class ResultModel
        {
            public DateTime Created { get; set; }
            public DateTime Updated { get; set; }
            public string Hostname { get; set; }
            public string Result { get; set; }
        }
    }
}
