using System.Collections.Generic;
using LoginManagement;

namespace WebUI.Models
{
    public class SectionProfileModel
    {
        public static List<Section> Sections { get; set; }
        public static List<Profile> Profiles { get; set; }
        public static IList<Software> Softwares { get; set; }
    }
}