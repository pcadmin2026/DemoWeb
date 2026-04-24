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
    public class BlogDetailsController : ControllerBase
    {
        private readonly Connect? _connect;
         string msg = "";
        string sql = "";
        public BlogDetailsController(Connect connect)
        {
            _connect = connect;      
        }
        [HttpPost("AddBlogDetails")]
        public IActionResult AddBlogDetails([FromForm]BlogDetailsModel blogdtls)
        {
            try
            {
                sql = @"insert into Tbl_Blogs_details (Title,SubmenuId,Description,Content,CategoryID,AuthorName,
                        ThumbnailURL,IsPublished,ViewCount,CreatedDate)
                        values(@Title,@SubmenuId,@Description,@Content,@CategoryID,@AuthorName,
                        @ThumbnailURL,@IsPublished,@ViewCount,@CreatedDate);
                        select isnull(Title,'')Title from Tbl_Blogs_details where BlogID=SCOPE_IDENTITY(); ";
                SqlParameter[] _param = {
                                        new SqlParameter("@Title",blogdtls.Title),
                                        new SqlParameter("@SubmenuId",blogdtls.SubmenuId),
                                        new SqlParameter("@Description",blogdtls.Description),
                                        new SqlParameter("@Content",blogdtls.Content),
                                        new SqlParameter("@CategoryID",blogdtls.CategoryID),
                                        new SqlParameter("@AuthorName",blogdtls.AuthorName),
                                        new SqlParameter("@ThumbnailURL",blogdtls.ThumbnailURL),
                                        new SqlParameter("@IsPublished",blogdtls.IsPublished),
                                        new SqlParameter("@ViewCount",blogdtls.ViewCount),
                                        new SqlParameter("@CreatedDate",DateTime.Now)
                };
                String conn = _connect.ConnectionString();
                string blogtitles = (string)SqlHelper.ExecuteScalar(conn,CommandType.Text, sql, _param);
                if (!string.IsNullOrWhiteSpace(blogtitles))
                {
                    msg = $"{blogtitles} Added Successfully";
                }
                else
                {
                    msg= $"Data Added Faild";
                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(msg);
        }
        [HttpPut("UpdateBlogDetails")]
        public IActionResult UpdateBlogDetails([FromForm] BlogDetailsModel blogdtls)
        {
            try
            {
                sql = @"update Tbl_Blogs_details  set Title=@Title,SubmenuId=@SubmenuId,Description=@Description,Content=@Content
                        ,CategoryID=@CategoryID,AuthorName=@AuthorName,ThumbnailURL=@ThumbnailURL,IsPublished=@IsPublished
                        ,ViewCount=@ViewCount,UpdatedDate=@UpdatedDate where BlogID=@BlogID;
                        select isnull(Title,'')Title from Tbl_Blogs_details where BlogID=@BlogID; ";
                SqlParameter[] _param = {
                                        new SqlParameter("@Title",blogdtls.Title),
                                        new SqlParameter("@SubmenuId",blogdtls.SubmenuId),
                                        new SqlParameter("@Description",blogdtls.Description),
                                        new SqlParameter("@Content",blogdtls.Content),
                                        new SqlParameter("@CategoryID",blogdtls.CategoryID),
                                        new SqlParameter("@AuthorName",blogdtls.AuthorName),
                                        new SqlParameter("@ThumbnailURL",blogdtls.ThumbnailURL),
                                        new SqlParameter("@IsPublished",blogdtls.IsPublished),
                                        new SqlParameter("@ViewCount",blogdtls.ViewCount),
                                        new SqlParameter("@UpdatedDate",DateTime.Now),
                                        new SqlParameter("@UpdatedDate",blogdtls.BlogID)
                };
                String conn = _connect.ConnectionString();
                string blogtitles = (string)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, _param);
                if (!string.IsNullOrWhiteSpace(blogtitles))
                {
                    msg = $"{blogtitles} Updated Successfully";
                }
                else
                {
                    msg = $"Data Update Faild";
                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(msg);
        }
        [HttpDelete("DeleteBlogDetails")]
        public IActionResult DeleteBlogDetails(Int32 blogid)
        {
            try
            {
                SqlParameter[] _parm = { new SqlParameter("@BlogID", blogid) }; 
                sql = @"delete from Tbl_Blogs_details  where BlogID=@BlogID";
                String conn = _connect.ConnectionString();
                int result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, _parm);
                if (result>0)
                {
                    msg = $"Data Deleted Successfully";
                }
                else
                {
                    msg = $"Error Please try again";
                }
             }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(msg);
        }
        [HttpGet("GetAllBlogDetails")]
        public IActionResult GetAllBlogDetails()
        {
            List<BlogDetailsModel> lstblogdtls = new List<BlogDetailsModel>();
            try
            {          
                sql = @"select isnull(Title,'')Title,SubmenuId,isnull(Description,'')Description,isnull(Content,'')Content,CategoryID
                        ,isnull(AuthorName,'')AuthorName,
                        isnull(ThumbnailURL,'')ThumbnailURL,IsPublished,ViewCount,convert(varchar(20),CreatedDate,103)CreatedDate
                        ,convert(varchar(20),UpdatedDate,103)UpdatedDate from Tbl_Blogs_details";
                String conn = _connect.ConnectionString();
                DataTable dt = SqlHelper.ExecuteDataTable(conn,CommandType.Text,sql);
                if (dt!=null && dt.Rows.Count > 0)
                {
                   foreach (DataRow drow in dt.Rows)
                    {
                        lstblogdtls.Add(new BlogDetailsModel
                        {
                            Title = Convert.ToString(drow["Title"]),
                            SubmenuId = Convert.ToInt32(drow["SubmenuId"]),
                            Description= Convert.ToString(drow["Description"]),
                            Content=Convert.ToString(drow["Description"]),
                            CategoryID= Convert.ToInt32(drow["CategoryID"]),
                            AuthorName=Convert.ToString(drow["Author"]),
                            ThumbnailURL = Convert.ToString(drow["ThumbnailURL"]),
                            IsPublished = Convert.ToBoolean(drow["IsPublished"]),
                            ViewCount = Convert.ToInt32(drow["ViewCount"]),
                            CreatedDate = Convert.ToDateTime(drow["CreatedDate"]),
                            UpdatedDate = Convert.ToDateTime(drow["UpdatedDate"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(lstblogdtls.Count>0?lstblogdtls :"");
        }
        [HttpGet("GetBlogDetailsbyId")]
        public IActionResult GetBlogDetailsbyId(Int32 blogdtlsid)
        {
            List<BlogDetailsModel> lstblogdtls = new List<BlogDetailsModel>();
            try
            {
                SqlParameter[] _param= { new SqlParameter("@BlogId", blogdtlsid) };
                sql = @"select isnull(Title,'')Title,SubmenuId,isnull(Description,'')Description,isnull(Content,'')Content,CategoryID
                    ,isnull(AuthorName,'')AuthorName,
                    isnull(ThumbnailURL,'')ThumbnailURL,IsPublished,ViewCount,convert(varchar(20)
                    ,CreatedDate,103)CreatedDate
                    ,convert(varchar(20),UpdatedDate,103)UpdatedDate from Tbl_Blogs_details where BlogID=@BlogId";
                String conn = _connect.ConnectionString();
                DataTable dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, _param);
                if (dt != null && dt.Rows.Count > 0)
                {
                      DataRow drow=dt.NewRow();
                        lstblogdtls.Add(new BlogDetailsModel
                        {
                            Title = Convert.ToString(drow["Title"]),
                            SubmenuId = Convert.ToInt32(drow["SubmenuId"]),
                            Description = Convert.ToString(drow["Description"]),
                            Content = Convert.ToString(drow["Description"]),
                            CategoryID = Convert.ToInt32(drow["CategoryID"]),
                            AuthorName = Convert.ToString(drow["Author"]),
                            ThumbnailURL = Convert.ToString(drow["ThumbnailURL"]),
                            IsPublished = Convert.ToBoolean(drow["IsPublished"]),
                            ViewCount = Convert.ToInt32(drow["ViewCount"]),
                            CreatedDate = Convert.ToDateTime(drow["CreatedDate"]),
                            UpdatedDate = Convert.ToDateTime(drow["UpdatedDate"])
                        });
                    }
                
            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(lstblogdtls.Count > 0 ? lstblogdtls : "");
        }
    }
}
