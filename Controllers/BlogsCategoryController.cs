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
    public class BlogsCategoryController : ControllerBase
    {
        private readonly Connect? _connect;
        string msg = "";
        string sql = "";
        public BlogsCategoryController(Connect connect)
        {
             _connect= connect;   
        }
        
        [HttpPost("AddBlogCategory")]
        public IActionResult AddBlogCategory([FromBody] BlogsCategoryModel blogcate)
        {
            try
            {
                sql = @"insert into Tbl_Blogs_Category(Category_name,IsActive,Createdby)
                        values(@Category_name,@IsActive,@Createdby);
                    select isnull(Category_name,'')Category_name from Tbl_Blogs_Category 
                    where Category_Id=SCOPE_IDENTITY();";
                SqlParameter[] _parm =
                {
                    new SqlParameter("@Category_name",blogcate.Category_name),
                    new SqlParameter("@IsActive",blogcate.IsActive),
                    new SqlParameter("@Createdby","admin")
                };
                string conn = _connect.ConnectionString();
                string blogcatee = (string)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, _parm);
                if (!string.IsNullOrEmpty(blogcatee))
                {
                    msg = $"{blogcatee} Added Successfully";
                }
                else
                {
                    msg = $"Not Response";
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(msg);
        }
        [HttpPut("UpdateBlogCategory")]
        public IActionResult UpdateBlogCategory([FromBody] BlogsCategoryModel blogcate,Int32 CategoryId)
        {
            try
            {
                sql = @"update Tbl_Blogs_Category set Category_name=@Category_name,IsActive=@IsActive
                    ,Updatedon=getdate(),Updatedby=@Updateby where Category_Id=@Category_Id
                    select isnull(Category_name,'')Category_name from Tbl_Blogs_Category 
                    where Category_Id=@Category_Id;";
                SqlParameter[] _parm =
                {
                    new SqlParameter("@Category_name",blogcate),
                    new SqlParameter("@IsActive",blogcate.IsActive),
                    new SqlParameter("@Updateby","admin"),
                     new SqlParameter("@Category_Id",CategoryId)
                };
                string conn = _connect.ConnectionString();
                string blogdtls = (string)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, _parm);
                if (!string.IsNullOrEmpty(blogdtls))
                {
                    msg = $"{blogdtls} Updated Successfully";
                }
                else
                {
                    msg = $"Not Response";
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(msg);
        }
        [HttpDelete("DeleteBlogCategory")]
        public IActionResult DeleteBlogCategory(Int32 CategoryId)
        {
            try
            {
                sql = @"delete from Tbl_Blogs_Category where Category_Id=@Category_Id;";
                SqlParameter[] _parm =
                {
                     new SqlParameter("@Category_Id",CategoryId)
                };
                string conn = _connect.ConnectionString();
                Int32 blogdtls = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, _parm);
                if (blogdtls>0)
                {
                    msg = $"Deleted Successfully";
                }
                else
                {
                    msg = $"Not Response";
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(msg);
        }
        [HttpGet("GetAllBlogCategory")]
        public IActionResult GetAllBlogCategory()
        {
            List<BlogsCategoryModel> lstblogcatee=new List<BlogsCategoryModel>();
            try
            {
                sql = @"select Category_Id,isnull(Category_name,'')Category_name,IsActive from Tbl_Blogs_Category";
               
                string conn = _connect.ConnectionString();
                DataTable dtblogdtls = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
                if (dtblogdtls!=null && dtblogdtls.Rows.Count > 0)
                {
                    foreach(DataRow drow in dtblogdtls.Rows)
                    {
                        lstblogcatee.Add(
                            new BlogsCategoryModel
                            {
                                Category_Id= Convert.ToInt32(drow["Category_Id"]),
                                Category_name = Convert.ToString(drow["Category_name"]),
                                IsActive= Convert.ToBoolean(drow["IsActive"])

                            }
                        );
                    }
                }
               
            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(lstblogcatee.Count > 0 ? lstblogcatee : "Record not found");
        }
        [HttpGet("GetBlogCategorybyId")]
        public IActionResult GetBlogCategorybyId( Int32 cateid)
        {
            List<BlogsCategoryModel> lstblogcatee = new List<BlogsCategoryModel>();
            try
            {
                sql = @"select Category_Id,isnull(Category_name,'')Category_name,IsActive from Tbl_Blogs_Category where Category_Id=@Category_Id";

                SqlParameter[] _parm =
                {
                     new SqlParameter("@Category_Id",cateid)
                };
                string conn = _connect.ConnectionString();
                DataTable dtblogdtls = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql,_parm);
                if (dtblogdtls != null && dtblogdtls.Rows.Count > 0)
                {
                    lstblogcatee = (from drow in dtblogdtls.AsEnumerable()
                                    select new BlogsCategoryModel
                                    {
                                        Category_Id = Convert.ToInt32(drow["Category_Id"]),
                                        Category_name = Convert.ToString(drow["Category_name"]),
                                        IsActive = Convert.ToBoolean(drow["IsActive"])

                                    }).ToList();
                    
                    
                }
                
            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(lstblogcatee.Count>0? lstblogcatee :"Record not found");
        }
    }
}
