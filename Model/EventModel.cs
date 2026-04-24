namespace EduWebAPI.Model
{
    public class EventModel
    {
        public Int32 MainmenuId { get; set; }
        public Int32 Submenuid { get; set; }
        public String? Eventtitles { get; set; }
        public String? EventDescriptions { get; set; }

        public IFormFile? File { get; set; }   
        public String? filepath { get;set;  }
        public String? StartDate { get; set; }
        public String? End_date { get; set; }
        public String? Event_status { get; set; }
        public bool IsActive { get; set; }
        public String? Createdby { get; set; }
    }
}
