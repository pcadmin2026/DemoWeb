//using Microsoft.AspNetCore.Http.HttpResults;

namespace EduWebAPI.Model
{
    public class BannerModel
    {
        //public int? Bannerid { get; set; }
        public int? Main_Menu_Id { get; set; }
        public string? Banner_Name { get; set; } = string.Empty;
        public IFormFile? File_Path { get; set; }
        public string? Content { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Createdby { get; set; } = string.Empty;
        public DateTime Createdon { get; set; }
        public string? Updateby { get; set; } = string.Empty;
        public DateTime UpdatedOn { get; set; }
        public string? Deletedby { get; set; } = string.Empty;
        public DateTime Deletedon { get; set; }
    }
}
