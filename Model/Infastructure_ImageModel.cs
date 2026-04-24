namespace EduWebAPI.Model
{
    public class Infastructure_ImageModel
    {
        public Int32 Infastructure_image_id { get; set; }
        public Int32 Sub_Menu_Id { get; set; }
        public string? Image_Name { get; set; }
        public IFormFile? File_Path { get; set; }
        public string? Image_Url { get; set; }
        public String? Content { get; set; }
        public String? Createdby { get; set; }
        public String? Updateby { get; set; }
        public DateTime? Updatedon { get; set;}
        public String? Deletedby { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
