using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EduWebAPI.Model;
using EduWebAPI.Common;
using RMSCommon;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
//using Microsoft.AspNetCore.Http.HttpResults;

namespace EduWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainMenuController : ControllerBase
    {
        private string msg = "";
        private Connect? _connect;
        public MainMenuController(Connect connect)
        {
            _connect=connect;
        }
        [HttpPost("AddMainMenu")]
        public IActionResult AddMainMenu([FromBody]MainMenuMasterModel menumaster)
        {
            bool isexists=false;
            try
            {               
                if (menumaster != null)
                {
                    SqlParameter[] _param1 =
                    {
                        new SqlParameter("@mode","check"),
                        new SqlParameter("@menuname",menumaster.MainmenuName)
                                              
                    };
                    string con = _connect.ConnectionString();
                    DataTable dtduplicate = SqlHelper.ExecuteDataTable(con, "SP_dulicateMainMenucheck", _param1);
                    if (dtduplicate != null && dtduplicate.Rows.Count > 0)
                    {
                        string mainmenu = Convert.ToString(dtduplicate.Rows[0]["Menu_Name"])!=""? Convert.ToString(dtduplicate.Rows[0]["Menu_Name"]):"";
                        if (mainmenu == menumaster.MainmenuName)
                        {
                            msg = "MainMenu Already Exists";
                           isexists = true;

                        }
                        string mscode = Convert.ToString(dtduplicate.Rows[0]["Menu_short_Code"]) != "" ? Convert.ToString(dtduplicate.Rows[0]["Menu_short_Code"]) : "";
                        if (mscode == menumaster.menushortcode)
                        {
                            msg = "Menu Short Code Already Exists";
                            isexists = true;
                        }
                    }
                   if(!isexists)
                    {
                        SqlParameter[] _param2 =
                   {
                        new SqlParameter("@mode","create"),
                        new SqlParameter("@menuname",menumaster.MainmenuName),
                        new SqlParameter("@menushortcode",menumaster.menushortcode),
                        new SqlParameter("@isActive",true),
                        new SqlParameter("@createdby","Admin"),
                        new SqlParameter("@createdon",DateTime.Now)
                    };
                        Int32 result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(),CommandType.StoredProcedure, "SP_AddMainMenumaster", _param2);
                        if (result > 0)
                        {
                            msg = "Main Menu Inserted Successfully";
                        }
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
        public JsonResult GetMainMenuAll()
        {
            List<MainMenuMasterModel> data = new List<MainMenuMasterModel>();
            try
            {
               DataTable dtmainment = Mainmenufetch();
                if (dtmainment != null && dtmainment.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtmainment.Rows)
                    {
                        data.Add(new MainMenuMasterModel
                        {
                            MainmenuId = Convert.ToInt32(dr["Menu_Id"]),
                            MainmenuName = Convert.ToString(dr["MenuName"]),
                            menushortcode = Convert.ToString(dr["MenushortCode"]),
                        });
                    }
                }
             }
            catch (Exception ex)
            {
                //return ex;
            }
            return new JsonResult(data.Count<=0? "Records not found":data);
        }

        [HttpGet("GetMainMenubyId")]
        public IActionResult GetMainmenubyid(int id)
        {
            DataTable dt1 = new DataTable();
            try
            {
                if (id > 0) 
                {                   
                    IEnumerable <MainMenuMasterModel>  menudetails = (from menu in Mainmenufetch().AsEnumerable()
                                  where menu.Field<int>("Menu_Id") == id
                                  select new MainMenuMasterModel{ 
                                   MainmenuId=menu.Field<int>("Menu_Id"),
                                   MainmenuName = menu.Field<string>("MenuName"),
                                   menushortcode = menu.Field<string>("MenushortCode")
                                  }).ToList();
                    if (menudetails.Count() <= 0)
                    {
                        return NotFound("Records Not found");
                    }
                   return Ok(menudetails);
                }    
              
            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(dt1);
        }
        [HttpPut("putMainMenubyId")]
        public IActionResult UpdateMainMenu(int id,string mainmenuname,string menushortname)
        {
            try
            {
                if (id > 0)
                {
                    string sql = @"update Tbl_MainMenuMaster set Menu_Name=@Menu_Name,Menu_short_Code=@Menu_short_Code
                                   ,Updateby=@Updateby,UpdatedOn=@UpdatedOn where Menu_Id=@Menu_Id and IsActive='1'";
                    SqlParameter[] _param1 =
                     {
                        new SqlParameter("@Menu_Name",mainmenuname),
                        new SqlParameter("@Menu_short_Code",menushortname),
                        new SqlParameter("@Updateby","admin"),
                        new SqlParameter("@UpdatedOn",DateTime.Now),
                        new SqlParameter("@Menu_Id",id) };
                    int result=SqlHelper.ExecuteNonQuery(_connect.ConnectionString(),CommandType.Text,sql, _param1);
                    if (result > 0)
                    {
                        msg = "Main Menu Update Successfully";
                    }
                    else {
                        msg = "Main Menu Update Can not be Updated";
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
        public IActionResult DeleteMainMenu(int id)
        {
            try
            {
                if (id > 0)
                {
                    string sql = @"update Tbl_MainMenuMaster set IsActive=0,Deletedby=@Deletedby,Deletedon=@Deletedon 
                                    where Menu_Id=@Menu_Id and IsActive='1'";
                    SqlParameter[] _param1 =
                     {
                         new SqlParameter("@Deletedby","admin"),
                        new SqlParameter("@Deletedon",DateTime.Now),
                      new SqlParameter("@Menu_Id",id) };
                    int result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param1);
                    if (result > 0)
                    {
                        msg = "Main Menu Delete Successfully";
                    }
                    else
                    {
                        msg = "Main Menu Can not be Deleted";
                    }

                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return Ok(msg);
        }
        [NonAction]
        public DataTable Mainmenufetch()
        {
            DataTable dtmainmenu = new DataTable();
            try
            { 
                string qry = @"select Menu_Id,ISNULL(Menu_Name,'')MenuName,ISNULL(Menu_short_Code,'')MenushortCode 
                                from Tbl_MainMenuMaster where IsActive=1";               
                dtmainmenu= SqlHelper.ExecuteDataTable(_connect.ConnectionString(), CommandType.Text, qry);
            }
            catch (Exception)
            {
                throw;
            }
            return dtmainmenu;
        }
        [NonAction]
        public DataTable CreateDatatable(string datatablename,Dictionary<string,Type> dtcol,string dtrow)
        {
            DataTable dt = null;
            try
            {
                 dt = new DataTable(datatablename);
                foreach (var dc in dtcol)
                {
                  dt.Columns.Add(dc.Key,dc.Value);               
                }
                foreach (object row in dtrow)
                {
                    dt.Rows.Add(dtrow);
                }   
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }
    }
}
