namespace EduWebAPI.Model
{
    public class ClassImageModel
    {
        public int Class_image_id { get; set; }
        public int Sub_Menu_Id { get; set; }
        public string? Image_Name { get; set; }
        public IFormFile? File_Path { get; set; }
        public string? Img_path { get; set; }
        public string? Content { get; set; }
        public bool IsActive { get; set; }
        public string? Createdby { get; set; }
      
        public string? Updateby { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? Deletedby { get; set; }
        public DateTime? Deletedon { get; set; }
    }
}
