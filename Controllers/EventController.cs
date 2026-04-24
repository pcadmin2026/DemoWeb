using EduWebAPI.Common;
using EduWebAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMSCommon;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EduWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly Connect? _connect;
        string msg = "";
        string sql = "";
        private readonly IWebHostEnvironment? _webhost;
        Common.Common cmm = new Common.Common();
        FileInfodetails _fileInfodetails = new FileInfodetails();
        public EventController(Connect connect, IWebHostEnvironment webhost)
        {
            _connect=connect;
            _webhost=webhost;
        }

        [HttpPost("AddEvent")]
        public IActionResult AddEvent([FromForm] EventModel model)
        {
            try
            {

                string file = "", filepath = "";
                file = UploadImage(model.File);
                if (!_fileInfodetails.status)
                {
                    msg = _fileInfodetails.messge;
                    return Ok(msg);
                }
                filepath = _fileInfodetails.filepath;
                // Convert file to binary
                byte[] fileBytes = null;
                var ms = new MemoryStream();
                model.File.CopyTo(ms);
                fileBytes = ms.ToArray();
          
               
                  sql = @"INSERT INTO Tbl_Event_Details
                                    (Main_menu_Id,Sub_menu_id,Event_titles,Event_Descriptions,
                                     File_path,File_Content,Start_date,End_date,
                                     Event_status,IsActive,Createdby)
                                    VALUES
                                    (@Main_menu_Id,@Sub_menu_id,@Event_titles,@Event_Descriptions,
                                     @File_path,@File_Content,@Start_date,@End_date,
                                     @Event_status,@IsActive,@Createdby)";
                SqlParameter[] _param ={
                                            new SqlParameter("@Main_menu_Id",model.MainmenuId),
                                            new SqlParameter("@Sub_menu_id",model.Submenuid),
                                             new SqlParameter("@Event_titles",model.Eventtitles),
                                            new SqlParameter("@Event_Descriptions",model.EventDescriptions),
                                             new SqlParameter("@File_path",filepath),
                                            new SqlParameter("@File_Content",fileBytes),
                                             new SqlParameter("@Start_date",model.StartDate),
                                            new SqlParameter("@End_date",model.End_date),
                                             new SqlParameter("@Event_status",model.Event_status),
                                            new SqlParameter("@IsActive",model.IsActive),
                                             new SqlParameter("@Createdby",model.Createdby)
                };
                Int32 result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param);
                if (result > 0)
                {
                    msg = $"Event Inserted Successfully";
                }    
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return new JsonResult(msg);
        }
        private String UploadImage(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                _fileInfodetails.status = false;
                return _fileInfodetails.messge = "Invalid File";
            }


            string[] allowedextensions = { ".jpg", ".png", ".svg" };
            string extension = Path.GetExtension(file.FileName).ToLower();

            string[] allowedMimeTypes = { "image/jpeg", "image/png", "image/svg+xml" };

            if (!allowedextensions.Contains(extension))
            {
                _fileInfodetails.status = false;
                return _fileInfodetails.messge = "Only JPG and PNG images allowed";
            }
            if (!allowedMimeTypes.Contains(file.ContentType))
            {
                _fileInfodetails.status = false;
                return _fileInfodetails.messge = "Invalid MIME type";
            }
            var stream = file.OpenReadStream();

            byte[] header = new byte[8];
            stream.Read(header, 0, header.Length);

            if (!cmm.IsValidImage(header, file))
            {
                _fileInfodetails.status = false;
                return _fileInfodetails.messge = "File content is not a valid image";
            }

            if (file.Length > 2 * 1024 * 1024)
            {
                _fileInfodetails.status = false;
                return _fileInfodetails.messge = "Image size must be less than 2MB";
            }

            string folder = Path.Combine(_webhost.WebRootPath, "Event");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + extension;
            string foldername = new DirectoryInfo(folder).Name;
            string filePath = Path.Combine(folder, fileName);
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                file.CopyTo(fs);
                _fileInfodetails.status = true;
                _fileInfodetails.filepath = $"../{foldername}/{fileName}";
            }
            else
            {
                _fileInfodetails.status = false;
                return _fileInfodetails.messge = "dose not exits filepath";
            }

            return _fileInfodetails.filepath;
        }
        [HttpPut("UpdateEvent")]
        public IActionResult UpdateEvent([FromForm] EventModel model,Int32 eventid)
        {
            try
            {

                string file = "", filepath = "";
                file = UploadImage(model.File);
                if (!_fileInfodetails.status)
                {
                    msg = _fileInfodetails.messge;
                    return Ok(msg);
                }
                filepath = _fileInfodetails.filepath;
                // Convert file to binary
                byte[] fileBytes = null;
                var ms = new MemoryStream();
                model.File.CopyTo(ms);
                fileBytes = ms.ToArray();


                sql = @"update Tbl_Event_Details set Main_menu_Id=@Main_menu_Id,Sub_menu_id=@Sub_menu_id,
                        Event_titles=@Event_titles,Event_Descriptions=@Event_Descriptions,File_path=@File_path,
                        File_Content=@File_Content,Start_date=@Start_date,End_date=@End_date,Event_status=@Event_status,
                        IsActive=@IsActive,Updatedon=GETDATE(),Updatedby=@Createdby
                        where Event_id=@Event_id and IsActive=1";
                SqlParameter[] _param ={
                                            new SqlParameter("@Main_menu_Id",model.MainmenuId),
                                            new SqlParameter("@Sub_menu_id",model.Submenuid),
                                             new SqlParameter("@Event_titles",model.Eventtitles),
                                            new SqlParameter("@Event_Descriptions",model.EventDescriptions),
                                             new SqlParameter("@File_path",filepath),
                                            new SqlParameter("@File_Content",fileBytes),
                                             new SqlParameter("@Start_date",model.StartDate),
                                            new SqlParameter("@End_date",model.End_date),
                                             new SqlParameter("@Event_status",model.Event_status),
                                            new SqlParameter("@IsActive",model.IsActive),
                                             new SqlParameter("@Createdby",model.Createdby),
                                              new SqlParameter("@Event_id",eventid)
                };
                Int32 result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param);
                if (result > 0)
                {
                    msg = $"Event Updated Successfully";
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return new JsonResult(msg);
        }
        [HttpDelete("DeleteEvents")]
        public IActionResult DeleteEvents(Int32 eventid)
        {
            ClassImageModel climodel = new ClassImageModel();
            try
            {
                SqlParameter[] _parmdel = {
                                new SqlParameter("@GImg_Id",eventid)
                    };
                sql = @"delete from Tbl_Event_Details where Event_id=@Event_id and IsActive=1";
                int result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _parmdel);
                if (result > 0)
                {
                    msg = "Data Delete Successfully";
                }
                else
                {
                    msg = "Data Can not be Deleted";
                }

            }
            catch (Exception)
            {

                // throw;
            }
            return new JsonResult(msg);
        }
        [HttpGet("GetAllEvents")]
        public IActionResult GetAllEvents()
        {
            List<EventModel> lstevent = new List<EventModel>();
            try
            {
                sql = @"select isnull(Main_menu_Id,0)Main_menu_Id,isnull(Sub_menu_id,0)Sub_menu_id
                        ,isnull(Event_titles,'')Event_titles,isnull(Event_Descriptions,'')Event_Descriptions
                        ,isnull(File_path,'')File_path,isnull(File_Content,'')File_Content
                        ,isnull(Start_date,'')Start_date,isnull(End_date,'')End_date
                        ,isnull(Event_status,'')Event_status,isnull(IsActive,'0')IsActive
                        ,convert(varchar(10),Createdon,103)Createdon,isnull(Createdby,'')Createdby
                        from Tbl_Event_Details where  isnull(IsActive,'0')=1";

                string conn = _connect.ConnectionString();
                DataTable dtevent = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
                if (dtevent != null && dtevent.Rows.Count > 0)
                {
                    foreach (DataRow drow in dtevent.Rows)
                    {
                        lstevent.Add(
                            new EventModel
                            {
                                MainmenuId = Convert.ToInt32(drow["Main_menu_Id"]),
                                Submenuid = Convert.ToInt32(drow["Sub_menu_id"]),
                                Eventtitles = Convert.ToString(drow["Event_titles"]),
                                EventDescriptions = Convert.ToString(drow["Event_Descriptions"]),
                                filepath = Convert.ToString(drow["File_path"]),
                                StartDate = Convert.ToString(drow["Start_date"]),
                                End_date = Convert.ToString(drow["End_date"]),
                                Event_status = Convert.ToString(drow["Event_status"])
                            }
                        );
                    }
                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(lstevent.Count > 0 ? lstevent : "Record not found");
        }
        [HttpGet("GetEventbyId")]
        public IActionResult GetEventbyId(int enventid)
        {
            List<EventModel> lstevent = new List<EventModel>();
            try
            {
                sql = @"select isnull(Main_menu_Id,0)Main_menu_Id,isnull(Sub_menu_id,0)Sub_menu_id
                        ,isnull(Event_titles,'')Event_titles,isnull(Event_Descriptions,'')Event_Descriptions
                        ,isnull(File_path,'')File_path,isnull(File_Content,'')File_Content
                        ,isnull(Start_date,'')Start_date,isnull(End_date,'')End_date
                        ,isnull(Event_status,'')Event_status,isnull(IsActive,'0')IsActive
                        ,convert(varchar(10),Createdon,103)Createdon,isnull(Createdby,'')Createdby
                        from Tbl_Event_Details where  Event_id=@Event_id and isnull(IsActive,'0')=1";

                string conn = _connect.ConnectionString();
                SqlParameter[] _p =
                {
                    new SqlParameter("@Event_id",enventid)
                };
                DataTable dtevent = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, _p);
                if (dtevent != null && dtevent.Rows.Count > 0)
                {
                    foreach (DataRow drow in dtevent.Rows)
                    {
                        lstevent.Add(
                            new EventModel
                            {
                                MainmenuId = Convert.ToInt32(drow["Main_menu_Id"]),
                                Submenuid = Convert.ToInt32(drow["Sub_menu_id"]),
                                Eventtitles = Convert.ToString(drow["Event_titles"]),
                                EventDescriptions = Convert.ToString(drow["Event_Descriptions"]),
                                filepath = Convert.ToString(drow["File_path"]),
                                StartDate = Convert.ToString(drow["Start_date"]),
                                End_date = Convert.ToString(drow["End_date"]),
                                Event_status = Convert.ToString(drow["Event_status"])
                            }
                        );
                    }
                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(lstevent.Count > 0 ? lstevent : "Record not found");
        }
    }
}
