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
    public class BlogCommentController : ControllerBase
    {
        private readonly Connect? _connect;
        string msg = "";
        string sql = "";
        public BlogCommentController(Connect connect)
        {
                _connect=connect;
        }
        [HttpPost("AddBlogComment")]
        public IActionResult AddBlogComment([FromForm] Blogs_CommentModel blogcom)
        {
            try
            {
                sql = @"insert into Tbl_Blogs_Comment (Blog_Id,Username,UserEmail,Contact_number,Comment_Text,
                    IsActive,Createdby) values(@Blog_Id,@Username,@UserEmail,@Contact_number,@Comment_Text,
                    @IsActive,@Createdby);";
                SqlParameter[] _parm =
                {
                    new SqlParameter("@Blog_Id",blogcom.Blog_Id),
                    new SqlParameter("@Username",blogcom.Username),
                    new SqlParameter("@UserEmail",blogcom.UserEmail),
                    new SqlParameter("@Contact_number",blogcom.Contact_number),
                    new SqlParameter("@Comment_Text",blogcom.Comment_Text),
                    new SqlParameter("@IsActive",blogcom.isactive),
                     new SqlParameter("@Createdby",blogcom.Createdby)
                };
                string conn = _connect.ConnectionString();
                Int32 res = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, _parm);
                if (res>0)
                {
                    msg = $"Comment Added Successfully";
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
        [HttpPut("UpdateBlogComment")]
        public IActionResult UpdateBlogComment([FromForm] Blogs_CommentModel blogcom, Int32 Comment_Id)
        {
            try
            {
                sql = @"update Tbl_Blogs_Comment set Blog_Id=@Blog_Id,Username=@Username,UserEmail=@UserEmail
                    ,Contact_number=@Contact_number,Comment_Text=@Comment_Text,IsActive=@IsActive
                    ,Updatedon=@Updatedon,Updatedby=@Updatedby where Comment_Id=@Comment_Id 
                        and isnull(IsActive,'0')=1";
                SqlParameter[] _parm =
                {
                    new SqlParameter("@Blog_Id",blogcom.Blog_Id),
                    new SqlParameter("@Username",blogcom.Username),
                    new SqlParameter("@UserEmail",blogcom.Username),
                    new SqlParameter("@Contact_number",blogcom.Contact_number),
                    new SqlParameter("@Comment_Text",blogcom.Comment_Text),
                    
                     new SqlParameter("@IsActive",blogcom.isactive),
                    new SqlParameter("@Updateby",blogcom.Updatedby),
                    new SqlParameter("@Updatedon",DateTime.Now),
                     new SqlParameter("@Comment_Id",Comment_Id)
                };
                string conn = _connect.ConnectionString();
                Int32 res = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, _parm);
                if (res>0)
                {
                    msg = $"Updated Successfully";
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
        [HttpDelete("DeleteBlogComment")]
        public IActionResult DeleteBlogComment(Int32 Comment_Id)
        {
            try
            {
                sql = @"delete from Tbl_Blogs_Comment where Category_Id=@Comment_Id;";
                SqlParameter[] _parm =
                {
                     new SqlParameter("@Comment_Id",Comment_Id)
                };
                string conn = _connect.ConnectionString();
                Int32 blogdtls = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, _parm);
                if (blogdtls > 0)
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
        [HttpGet("GetAllBlogComment")]
        public IActionResult GetAllBlogComment()
        {
            List<Blogs_CommentModel> lstblogcom = new List<Blogs_CommentModel>();
            try
            {
                sql = @"select * from Tbl_Blogs_Comment where isnull(IsActive,'0')=1";

                string conn = _connect.ConnectionString();
                DataTable dtblogdtls = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
                if (dtblogdtls != null && dtblogdtls.Rows.Count > 0)
                {
                    foreach (DataRow drow in dtblogdtls.Rows)
                    {
                        lstblogcom.Add(
                            new Blogs_CommentModel
                            {
                                Comment_Id = Convert.ToInt32(drow["Comment_Id"]),
                                Blog_Id = Convert.ToInt32(drow["Blog_Id"]),
                                Username = Convert.ToString(drow["Username"]),                                
                                UserEmail= Convert.ToString(drow["UserEmail"]),
                                Contact_number= Convert.ToString(drow["Contact_number"]),
                                Comment_Text= Convert.ToString(drow["Comment_Text"]),
                                isactive= Convert.ToBoolean(drow["IsActive"]),
                                Createdby= Convert.ToString(drow["Createdby"])

                            }
                        );
                    }
                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(lstblogcom.Count > 0 ? lstblogcom : "Record not found");
        }
        [HttpGet("GetBlogCommentbyId")]
        public IActionResult GetBlogCommentbyId(Int32 commentid)
        {
            List<Blogs_CommentModel> lstblogcom = new List<Blogs_CommentModel>();
            try
            {
                sql = @"select * from Tbl_Blogs_Comment where Comment_Id=@Comment_Id and isnull(IsActive,'0')=1";

                SqlParameter[] _parm =
                {
                     new SqlParameter("@Comment_Id",commentid)
                };
                string conn = _connect.ConnectionString();
                DataTable dtblogcomdtls = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, _parm);
                if (dtblogcomdtls != null && dtblogcomdtls.Rows.Count > 0)
                {
                    lstblogcom = (from drow in dtblogcomdtls.AsEnumerable()
                                    select new Blogs_CommentModel
                                    {
                                        Comment_Id = Convert.ToInt32(drow["Comment_Id"]),
                                        Blog_Id = Convert.ToInt32(drow["Blog_Id"]),
                                        Username = Convert.ToString(drow["Username"]),
                                        UserEmail = Convert.ToString(drow["UserEmail"]),
                                        Contact_number = Convert.ToString(drow["Contact_number"]),
                                        Comment_Text = Convert.ToString(drow["Comment_Text"]),
                                        isactive = Convert.ToBoolean(drow["IsActive"]),
                                        Createdby = Convert.ToString(drow["Createdby"])

                                    }).ToList();


                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(lstblogcom.Count > 0 ? lstblogcom : "Record not found");
        }
    }
}
