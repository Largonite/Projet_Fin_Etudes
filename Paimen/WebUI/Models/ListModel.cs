using System.Collections.Generic;
using LoginManagement;

namespace WebUI.Models
{
    public class ListModel
    {
        public List<Section> Sections { get; set; }
        public List<Profile> Profiles { get; set; }
        public List<Software> Softwares { get; set; }
        public bool IsSelected { get; set; }
        public string YearSection { get; set; }
    }
}