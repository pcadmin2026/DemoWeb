namespace EduWebAPI.Model
{
    public class MainMenuMasterModel
    {
        public Int32 MainmenuId { get; set; }
        public string? MainmenuName { get; set; }=string.Empty;
        public string? menushortcode { get; set; } = string.Empty;
        public Boolean? isactive { get; set; } = false;
        public string? Createdby { get; set; } = string.Empty;
        public DateTime? Createdon { get; set; }=DateTime.Now;
        //public string? Updatedby { get; set; }
        //public DateTime? Updatedon { get; set; }
        //public string? Deletedby { get; set; }
       // public DateTime? Deletedon { get; set; }
    }
}
