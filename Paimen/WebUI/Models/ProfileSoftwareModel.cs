using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoginManagement;

namespace WebUI.Models
{
    public class ProfileSoftwareModel
    {
        public List<Profile> Profiles { get; set; }
        public List<Software> Softwares { get; set; }
    }
}