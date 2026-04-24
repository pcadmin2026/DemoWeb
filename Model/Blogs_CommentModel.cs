namespace EduWebAPI.Model
{
    public class Blogs_CommentModel
    {
        public Int32 Comment_Id { get; set; }
        public Int32 Blog_Id { get; set; }
        public string? Username { get; set; } = string.Empty;
        public string? UserEmail { get; set; } = string.Empty;
        public string? Contact_number { get; set; } = string.Empty;
        public string? Comment_Text { get; set; } = string.Empty;
       
        public Boolean? isactive { get; set; } = false;
        public string? Createdby { get; set; } = string.Empty;
        public DateTime? Updatedon { get; set; } = DateTime.Now;
        public string? Updatedby { get; set; } = string.Empty;
        public DateTime? Deletedon { get; set; } = DateTime.Now;
        public string? Deletedby { get; set; } = string.Empty;
    }
}
