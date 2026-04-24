//using Microsoft.AspNetCore.Http.HttpResults;

namespace EduWebAPI.Model
{
    public class SubMenuMasterModel
    {
       public Int32? Sub_Menu_Id {  get; set; }
        //public Int32? Main_Menu_Id { get; set; }
        public string? Sub_Menu_Name { get; set; } = string.Empty;

        public string?  Createdby { get; set; } = string.Empty;
        public DateTime Createdon { get; set; }=DateTime.Now;
        public MainMenu? mainmenu { get; set; } 
    }
    public class MainMenu
    {
        public int Main_Menu_Id { get;set;  }
        public string Menuname { get;set; }=string.Empty;
    }
}
