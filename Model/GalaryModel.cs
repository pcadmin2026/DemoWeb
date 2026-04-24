namespace EduWebAPI.Model
{
    public class GalaryModel
    {
        public Int32 GImgId { get; set; }
        public Int32 Mainmenu_Id { get; set; }
        public Int32 Submenu_id { get; set; }
        public IFormFile? Imagepath { get; set; }
        public String? Filepath { get; set; }
        public String? Imagecontent { get; set; }
        public String? ImageName { get; set; }
        public Boolean IsActive { get; set; }
        public String? Createdby { get; set; }
    }
}
