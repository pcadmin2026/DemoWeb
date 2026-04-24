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
    public class ParrentFeedbacksController : ControllerBase
    {
        private readonly Connect? _connect;
        string msg = "";
        string sql = "";

        public ParrentFeedbacksController(Connect connect)
        {
           _connect= connect;     
        }
        [HttpPost("AddParrentFeedbacks")]
        public IActionResult ParrentFeedbacks([FromForm] ParrentFeedbacksModel pfm)
        {
            try
            {

                pfm.IPAdrress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
                sql = @"insert into Tbl_Parrent_Feedbacks(Feedback,Mobile_No,Email,Name,Subject,IP_Address,
                        IsActive,Createdby) values(
                        @Feedback,@Mobile_No,@Email,@Name,@Subject,@IP_Address,
                        @IsActive,@Createdby
                        )";
                SqlParameter[] _param ={
                                            new SqlParameter("@Feedback",pfm.Feedback),
                                            new SqlParameter("@Mobile_No",pfm.Mobile_No),
                                             new SqlParameter("@Email",pfm.Email),
                                            new SqlParameter("@Name",pfm.Name),
                                             new SqlParameter("@Subject",pfm.Subject),
                                            new SqlParameter("@IP_Address",pfm.IPAdrress),
                                             new SqlParameter("@IsActive",pfm.IsActive),
                                            new SqlParameter("@Createdby",pfm.Createdby)
                                            
                };
                Int32 result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param);
                if (result > 0)
                {
                    msg = $"Inserted Successfully";
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return new JsonResult(msg);
        }

        [HttpPut("UpdateParrentFeedbacks")]
        public IActionResult UpdateParrentFeedbacks([FromForm] ParrentFeedbacksModel pfm,Int32 feedbackid)
        {
            try
            {

                pfm.IPAdrress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
                sql = @"update Tbl_Parrent_Feedbacks set Feedback=@Feedback,Mobile_No=@Mobile_No,Email=@Email,
                            Name=@Name,Subject=@Subject,IP_Address=@IP_Address,IsActive=@IsActive,Updatedon=getdate(),
                            Updatedby=@Updatedby where Feedback_id=@Feedback_id and IsActive=1";
                SqlParameter[] _param ={
                                            new SqlParameter("@Feedback",pfm.Feedback),
                                            new SqlParameter("@Mobile_No",pfm.Mobile_No),
                                             new SqlParameter("@Email",pfm.Email),
                                            new SqlParameter("@Name",pfm.Name),
                                             new SqlParameter("@Subject",pfm.Subject),
                                            new SqlParameter("@IP_Address",pfm.IPAdrress),
                                             new SqlParameter("@IsActive",pfm.IsActive),
                                            new SqlParameter("@Updatedby",pfm.Updatedby),
                                             new SqlParameter("@Feedback_id",feedbackid)
                };
                Int32 result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param);
                if (result > 0)
                {
                    msg = $"Data Updated Successfully";
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return new JsonResult(msg);
        }

        [HttpDelete("DeleteParrentFeedbacks")]
        public IActionResult DeleteParrentFeedbacks(Int32 Feedbackid)
        {
            try
            {
                sql = @"delete from Tbl_Parrent_Feedbacks where Feedback_id=@Feedback_id and IsActive=1";
                SqlParameter _param = new SqlParameter("@Feedback_id", Feedbackid);
                Int32 result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param);
                if (result > 0)
                {
                    msg = $"Data Deleted Successfully";
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return new JsonResult(msg);
        }
        [HttpGet("GetAllParrentFeedbacks")]
        public IActionResult GetAllParrentFeedbacks()
        {
            List<ParrentFeedbacksModel> lstpmodel = new List<ParrentFeedbacksModel>();
            try
            {
                sql = @"select Feedback_id,isnull(Feedback,'')Feedback,isnull(Mobile_No,'')Mobile_No,
                    isnull(Email,'')Email,isnull(Name,'')Name,isnull(Subject,'')Subject,isnull(IsActive,'0')IsActive
                    from Tbl_Parrent_Feedbacks
                    where IsActive=1";            
                DataTable dtfeedback = SqlHelper.ExecuteDataTable(_connect.ConnectionString(), CommandType.Text, sql);
                if (dtfeedback!=null && dtfeedback.Rows.Count>0)
                {
                   foreach(DataRow drow in dtfeedback.Rows)
                    {
                        lstpmodel.Add(new()
                        {
                            Feedback = drow["Feedback"].ToString(),
                            Mobile_No = Convert.ToString(drow["Mobile_No"]),
                            Email = Convert.ToString(drow["Mobile_No"]),
                            Name = Convert.ToString(drow["Name"]),
                            Subject = Convert.ToString(drow["Subject"]),
                            IsActive = Convert.ToBoolean(drow["IsActive"]),
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return new JsonResult(lstpmodel);
        }
        [HttpGet("GetParrentFeedbacksbyId")]
        public IActionResult GetParrentFeedbacksbyId(Int32 feedbackid)
        {
            List<ParrentFeedbacksModel> lstpmodel = new List<ParrentFeedbacksModel>();
            try
            {
                sql = @"select Feedback_id,isnull(Feedback,'')Feedback,isnull(Mobile_No,'')Mobile_No,
                    isnull(Email,'')Email,isnull(Name,'')Name,isnull(Subject,'')Subject,isnull(IsActive,'0')IsActive
                    from Tbl_Parrent_Feedbacks
                    where Feedback_id=@Feedback_id and IsActive=1";
                SqlParameter parm = new SqlParameter("@Feedback_id", feedbackid);
                DataTable dtfeedback = SqlHelper.ExecuteDataTable(_connect.ConnectionString(), CommandType.Text, sql,parm);
                if (dtfeedback != null && dtfeedback.Rows.Count > 0)
                {
                    lstpmodel=(from drow in dtfeedback.AsEnumerable()
                               select new ParrentFeedbacksModel
                               {
                                   Feedback = drow["Feedback"].ToString(),
                                   Mobile_No = Convert.ToString(drow["Mobile_No"]),
                                   Email = Convert.ToString(drow["Mobile_No"]),
                                   Name = Convert.ToString(drow["Name"]),
                                   Subject = Convert.ToString(drow["Subject"]),
                                   IsActive = Convert.ToBoolean(drow["IsActive"]),
                               }

                        ).ToList();                                          
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return new JsonResult(lstpmodel);
        }
    }
}
