namespace EduWebAPI.Model
{
    public class BlogDetailsModel
    {
        public Int32 BlogID { get; set; }
        public String? Title { get; set; }
        public Int32 SubmenuId { get; set; }
        public String? Description { get; set; }

        public String? Content { get; set; }
        public Int32 CategoryID { get; set; }
        public String? AuthorName { get; set; }
        public String? ThumbnailURL { get; set; }
        public bool? IsPublished { get; set; }

        public Int32 ViewCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
