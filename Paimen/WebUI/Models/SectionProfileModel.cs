using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoginManagement;

namespace WebUI.Models
{
    public class SectionProfileModel
    {
        public List<Section> Sections { get; set; }
        public List<Profile> Profiles { get; set; }
    }
}