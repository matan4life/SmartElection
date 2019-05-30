using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionFrontend.Models
{
    public class AlternativeViewModel
    {
        public string Alternative { get; set; }

        public List<SelectListItem> Alternatives { get; set; } = new List<SelectListItem>();
    }
}
