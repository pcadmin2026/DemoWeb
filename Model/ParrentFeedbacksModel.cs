//using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Xml.Linq;

namespace EduWebAPI.Model
{
    public class ParrentFeedbacksModel
    {
        public String? Feedback { get; set; }
        public String? Mobile_No { get; set; }
        public String? Email { get; set; }
        public String? Name { get; set; }
        public String? Subject { get; set; }
        public Boolean IsActive { get; set; }
        public String? IPAdrress { get;set; }
        public String? Createdby { get; set; }
        public String? Updatedby { get; set; }
    }
}
