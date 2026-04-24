//using Microsoft.AspNetCore.Http.HttpResults;

namespace EduWebAPI.Model
{
    public class PageMasterModel
    {
       public Int32 Main_menu_Id { get; set; }
        public Int32 Sub_menu_Id { get; set; }
        public String? Page_Name { get; set; }
        public String?  Content { get; set; }
        public IFormFile? Filepathdetails { get; set; }
        public String? File_path { get; set; }
        public String?  Access_by { get; set; }
        public Boolean  Is_Visiable { get; set; }
        public Boolean IsActive { get; set; }
        public String?  Createdby { get; set; }
        public String? Updatedby { get; set; }
    }
}
