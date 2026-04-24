using EduWebAPI.Common;
using EduWebAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMSCommon;
using System.Data;
using System.Data.SqlClient;

namespace EduWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubMenuMasterController : ControllerBase
    {
        private string msg = "";
        private Connect? _connect;
        public SubMenuMasterController(Connect connect)
        {
            _connect = connect;
        }
        [HttpGet("GetMMenu")]
        public JsonResult GetMMenu()
        {
            List<MainMenu> lstmenu = new List<MainMenu>();
            try
            {
                string sqlmenu = @"select Menu_Id,isnull(Menu_Name,'')Menu_Name,isnull(Menu_short_Code,'')Menu_short_Code from Tbl_MainMenuMaster
                                where IsActive=1";
                DataTable dt1=SqlHelper.ExecuteDataTable(_connect.ConnectionString(),CommandType.Text,sqlmenu);

                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    foreach (DataRow drow in dt1.Rows)
                    {
                        lstmenu.Add(new MainMenu
                        {
                            Main_Menu_Id = Convert.ToInt32(drow["Menu_Id"]),
                            Menuname = Convert.ToString(drow["Menu_Name"]).Trim()
                        });

                    }
                }
                    return new JsonResult(lstmenu.Count>0 ? lstmenu : "--NA--");
            }
            catch (Exception)
            {

                throw;
            }
          
        }
       [HttpPost("AddsubmenuMenu")]
        public IActionResult AddsubmenuMenu([FromBody] SubMenuMasterModel subMenu)
        {
           try
            {
                if (ModelState.IsValid)
                {
                    SqlParameter[] _param1 =
                    {
                        new SqlParameter("@submenuname",subMenu.Sub_Menu_Name),
                        new SqlParameter("@mainmenuid",subMenu.mainmenu?.Main_Menu_Id)
                    };
                    string con = _connect.ConnectionString();
                    DataTable dtsubmenuduplicate = SqlHelper.ExecuteDataTable(con,"IsDuplicatesubmenu", _param1);
                    if (dtsubmenuduplicate != null && dtsubmenuduplicate.Rows.Count > 0)
                    {
                            msg = "SubMenu Already Exists";
                            return new JsonResult(msg);                      
                    }
                         SqlParameter[] _param2 =
                         {
                            new SqlParameter("@mode","insert"),
                            new SqlParameter("@main_menu_id",subMenu.mainmenu?.Main_Menu_Id),
                            new SqlParameter("@submenuname",subMenu.Sub_Menu_Name),
                            new SqlParameter("@IsActice",true),
                            new SqlParameter("@createdby","Admin"),
                            new SqlParameter("@createdon",DateTime.Now)
                        };
                        string result = (string)SqlHelper.ExecuteScalar(_connect.ConnectionString(),CommandType.Text, "Sp_Submenu",_param2);
                        if (!string.IsNullOrEmpty(result))
                        {
                            msg = $"Sub Menu {result} Added Successfully";
                        }                  
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return new JsonResult(msg);
        }
        
        [HttpGet("GetAllMainMenu")]
        public JsonResult GetSubMenuAll()
        {
            List<SubMenuMasterModel> data = new List<SubMenuMasterModel>();
            try
            {
                DataTable dtsubmenu = SubmenuDetails();
                if (dtsubmenu != null && dtsubmenu.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtsubmenu.Rows)
                    {
                        data.Add(new SubMenuMasterModel
                        {
                            Sub_Menu_Id = Convert.ToInt32(dr["Sub_Menu_Id"]),
                            mainmenu=new MainMenu
                            {
                                Menuname = Convert.ToString(dr["Menu_Name"])
                            },
                           Sub_Menu_Name = Convert.ToString(dr["submenuname"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                //return ex;
            }
            return new JsonResult(data.Count <= 0 ? "Records not found" : data);
        }
        [HttpGet("GetMainMenuwisesbyId")]
        public JsonResult GetMainMenuwisesbyId(int mainid)
        {
            List<SubMenuMasterModel> dataitem = new List<SubMenuMasterModel>();
            try
            {
                DataTable dtsubmenu = SubmenuDetails(mainid);
                if (dtsubmenu != null && dtsubmenu.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtsubmenu.Rows)
                    {
                        dataitem.Add(new SubMenuMasterModel
                        {
                            Sub_Menu_Id = Convert.ToInt32(dr["Sub_Menu_Id"]),
                            mainmenu = new MainMenu
                            {
                                Menuname = Convert.ToString(dr["Menu_Name"])
                            },
                            Sub_Menu_Name = Convert.ToString(dr["submenuname"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                //return ex;
            }
            return new JsonResult(dataitem.Count <= 0 ? "Records not found" : dataitem);
        }
        [NonAction]
        public DataTable SubmenuDetails(int mainuid=0)
        {
            try
            {
                DataTable dtsubmenudetails = new DataTable();
                string sql = "";
                sql = @"select sm.Sub_Menu_Id,isnull(mm.Menu_Name,'')Menu_Name,sm.Main_Menu_Id,isnull(sm.Sub_Menu_Name,'')submenuname from Sub_MenuMaster sm 
                            inner join Tbl_MainMenuMaster mm on sm.Main_Menu_Id=mm.Menu_Id
                            where sm.IsActive=1";
                if (mainuid > 0)
                {
                    sql += " and mm.Menu_Id=@mainuid";
                    SqlParameter[] _param = { new SqlParameter("@mainuid", mainuid) };
                    dtsubmenudetails = SqlHelper.ExecuteDataTable(_connect.ConnectionString(), CommandType.Text, sql, _param);
                }
                else
                {
                    dtsubmenudetails = SqlHelper.ExecuteDataTable(_connect.ConnectionString(), CommandType.Text, sql);
                }
                return dtsubmenudetails;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpPut("putSubMenubyId")]
        public IActionResult UpdateSubMenu(int submenuid, string mainmenuid, string submenuname,string username)
        {
            try
            {
                if (submenuid > 0)
                {
                    string sql = @"update Sub_MenuMaster set Main_Menu_Id=@Main_Menu_Id,Sub_Menu_Name=@Sub_Menu_Name
                                   ,Updateby=@Updateby,UpdatedOn=@UpdatedOn where Sub_Menu_Id=@Sub_Menu_Id and IsActive='1'";
                    SqlParameter[] _param1 =
                     {
                        new SqlParameter("@Main_Menu_Id",mainmenuid),
                        new SqlParameter("@Sub_Menu_Name",submenuname),
                        new SqlParameter("@Updateby",username),
                        new SqlParameter("@UpdatedOn",DateTime.Now),
                        new SqlParameter("@Sub_Menu_Id",submenuid) };
                    int result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param1);
                    if (result > 0)
                    {
                        msg = "Sub Menu Update Successfully";
                    }
                    else
                    {
                        msg = "Sub Menu Update Can not be Updated";
                    }
                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return Ok(msg);
        }
        [HttpDelete("DeleteMainMenubyId")]
        public IActionResult DeleteMainMenu(int submenuid)
        {
            try
            {
                if (submenuid > 0)
                {
                    string sql = @"update Sub_MenuMaster set IsActive=0,Deletedby=@Deletedby,Deletedon=@Deletedon 
                                    where Sub_Menu_Id=@subMenu_Id and IsActive='1'";
                    SqlParameter[] _param1 =
                     {
                         new SqlParameter("@Deletedby","admin"),
                        new SqlParameter("@Deletedon",DateTime.Now),
                      new SqlParameter("@subMenu_Id",submenuid) };
                    int result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param1);
                    if (result > 0)
                    {
                        msg = "Sub Menu Delete Successfully";
                    }
                    else
                    {
                        msg = "Sub Menu Can not be Deleted";
                    }

                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return Ok(msg);
        }

    }
}
