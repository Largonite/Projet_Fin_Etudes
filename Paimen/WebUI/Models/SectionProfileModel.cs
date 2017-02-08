using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoginManagement;

namespace WebUI.Models
{
    public class SectionProfileModel
    {
        public static List<Section> Sections { get; set; }
        public static List<Profile> Profiles { get; set; }
        public static List<Software> Softwares { get; set; }
        public bool IsSelected { get; set; }
        public string YearSection { get; set; }
    }
}