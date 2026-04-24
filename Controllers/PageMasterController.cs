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
    public class PageMasterController : ControllerBase
    {
        private readonly Connect? _connect;
        string msg = "";
        string sql = "";
        private readonly IWebHostEnvironment? _webhost;
        Common.Common cmm = new Common.Common();
        FileInfodetails _fileInfodetails = new FileInfodetails();
        public PageMasterController(Connect? connect, IWebHostEnvironment? webhost)
        {
            _connect=connect;
            _webhost=webhost;
        }
        [HttpPost("AddPageMaster")]
        public IActionResult AddPageMaster([FromForm] PageMasterModel pagemodel)
        {
            try
            {

                string file = "", filepath = "";
                file = UploadImage(pagemodel.Filepathdetails);
                if (!_fileInfodetails.status)
                {
                    msg = _fileInfodetails.messge;
                    return Ok(msg);
                }
                filepath = _fileInfodetails.filepath;
                


                sql = @"insert into Page_Master(Main_menu_Id,Sub_menu_Id,Page_Name,Content,File_path,Access_by,
                        Is_Visiable,IsActive,Createdon,Createdby) values(@Main_menu_Id,@Sub_menu_Id
                            ,@Page_Name,@Content,@File_path,@Access_by,
                            @Is_Visiable,@IsActive,GETDATE(),@Createdby)";
                SqlParameter[] _param ={
                                            new SqlParameter("@Main_menu_Id",pagemodel.Main_menu_Id),
                                            new SqlParameter("@Sub_menu_Id",pagemodel.Sub_menu_Id),
                                             new SqlParameter("@Page_Name",pagemodel.Page_Name),
                                            new SqlParameter("@Content",pagemodel.Content),
                                             new SqlParameter("@File_path",filepath),
                                             new SqlParameter("@Is_Visiable",pagemodel.Is_Visiable),
                                            new SqlParameter("@IsActive",pagemodel.IsActive),                                         
                                             new SqlParameter("@Createdby",pagemodel.Createdby)
                };
                Int32 result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param);
                if (result > 0)
                {
                    msg = $"Page Inserted Successfully";
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
        [HttpPost("UpdatePageMaster")]
        public IActionResult UpdatePageMaster([FromForm] PageMasterModel pagemodel,Int32 pageid)
        {
            try
            {

                string file = "", filepath = "";
                file = UploadImage(pagemodel.Filepathdetails);
                if (!_fileInfodetails.status)
                {
                    msg = _fileInfodetails.messge;
                    return Ok(msg);
                }
                filepath = _fileInfodetails.filepath;



                sql = @"update Page_Master set Main_menu_Id=@Main_menu_Id,Sub_menu_Id=@Sub_menu_Id
                    ,Page_Name=@Page_Name,Content=@Content,File_path=@File_path,IsActive=@IsActive,
                    Updatedon=GETDATE(),Updatedby=@Updatedby where Page_Id=@Page_Id and IsActive=1";
                SqlParameter[] _param ={
                                            new SqlParameter("@Main_menu_Id",pagemodel.Main_menu_Id),
                                            new SqlParameter("@Sub_menu_Id",pagemodel.Sub_menu_Id),
                                             new SqlParameter("@Page_Name",pagemodel.Page_Name),
                                            new SqlParameter("@Content",pagemodel.Content),
                                             new SqlParameter("@File_path",filepath),                                          
                                            new SqlParameter("@IsActive",pagemodel.IsActive),
                                             new SqlParameter("@Updatedby",pagemodel.Updatedby),
                                              new SqlParameter("@Page_Id",pageid)
                };
                Int32 result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param);
                if (result > 0)
                {
                    msg = $"Page Updated Successfully";
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return new JsonResult(msg);
        }
    }
}
