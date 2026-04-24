using EduWebAPI.Common;
using EduWebAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMSCommon;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace EduWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly Connect? _connect;
        private string msg = "";
        private readonly IWebHostEnvironment _webhost;
        Common.Common cmm =new Common.Common();
        FileInfodetails _fileInfodetails = new FileInfodetails();

        public BannerController(Connect connect, IWebHostEnvironment iwebhost)
        {
            _connect = connect;
            _webhost = iwebhost;
        }
        [HttpPost("AddBanner")]
        public IActionResult AddBannerData([FromForm]BannerModel? banner)
        {
            try
            {

                string file = "",filepath="";
                file = UploadImage(banner.File_Path);
                if (!_fileInfodetails.status)
                {
                    msg = _fileInfodetails.messge;
                    return Ok(msg);
                }
                filepath = _fileInfodetails.filepath;


                    SqlParameter[] _parambarner =
                    {
                        new SqlParameter("@mainmenuid",Convert.ToInt32(banner.Main_Menu_Id)),
                        new SqlParameter("@Banner_Name",banner.Banner_Name)
                       
                    };
                    string con = _connect.ConnectionString();
                    DataTable dtbaner = SqlHelper.ExecuteDataTable(con, "sp_duplicatebaner", _parambarner);
                    if (dtbaner != null && dtbaner.Rows.Count > 0)
                    {
                        msg = "Banner Already Exists";
                        return new JsonResult(msg);
                    }
                    SqlParameter[] _param2 =
                    {
                            new SqlParameter("@mode","insert"),
                            new SqlParameter("@Main_Menu_Id",Convert.ToInt32(banner.Main_Menu_Id)),
                            new SqlParameter("@Banner_Name",banner.Banner_Name),
                            new SqlParameter("@File_Path",filepath),
                            new SqlParameter("@Content",banner.Content),
                            new SqlParameter("@IsActive",banner.IsActive),
                            new SqlParameter("@Createdby",banner.Createdby),
                            new SqlParameter("@Createdon",banner.Createdon)
                        };
                    string result = (string)SqlHelper.ExecuteScalar(_connect.ConnectionString(), CommandType.StoredProcedure, "Sp_Banner", _param2);
                    if (!string.IsNullOrEmpty(result))
                    {
                        msg = $"{result} Added Successfully";
                    }
                
            }
            catch (Exception ex)
            {

                throw ex;
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
                

            string[] allowedextensions = { ".jpg",".png", ".svg" };
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
            
            string folder = Path.Combine(_webhost.WebRootPath, "BannerImages");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + extension;

            string filePath = Path.Combine(folder, fileName);
            if(!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                file.CopyTo(fs);
                _fileInfodetails.status = true;
                _fileInfodetails.filepath= $"/{filePath}";
            }
            else
            {
                _fileInfodetails.status = false;
                return _fileInfodetails.messge = "dose not exits filepath";
            }

             return _fileInfodetails.filepath;
        }

        [HttpPut("UpdateBannerData")]
        public IActionResult UpdateBannerData([FromForm] BannerModel? banner,Int32 banerid)
        {
            try
            {

                string file = "", filepath = "";
                file = UploadImage(banner.File_Path);
                if (!_fileInfodetails.status)
                {
                    msg = _fileInfodetails.messge;
                    return Ok(msg);
                }
                filepath = _fileInfodetails.filepath;
                               
                SqlParameter[] _param2 =
                {
                            new SqlParameter("@mode","update"),
                            new SqlParameter("@Main_Menu_Id",Convert.ToInt32(banner.Main_Menu_Id)),
                            new SqlParameter("@Banner_Name",banner.Banner_Name),
                            new SqlParameter("@File_Path",filepath),
                            new SqlParameter("@Content",banner.Content),
                            new SqlParameter("@IsActive",banner.IsActive),
                            new SqlParameter("@Updateby",banner.Updateby),
                            new SqlParameter("@UpdatedOn",banner.UpdatedOn),
                            new SqlParameter("@Banner_Id",banerid)
                        };
                string result = (string)SqlHelper.ExecuteScalar(_connect.ConnectionString(), CommandType.StoredProcedure, "Sp_Banner", _param2);
                if (!string.IsNullOrEmpty(result))
                {
                    msg = $"{result} Updated Successfully";
                }

            }
            catch (Exception ex)
            {

                //throw ex;
            }
            return new JsonResult(msg);
        }
        [HttpGet("GetAllBannerDetials")]
        public IActionResult GetAllBannerDetials()
        {
            BannerModel bnrmodel = new BannerModel();
            try
            {
                SqlParameter[] _parm = { 
                                     new SqlParameter("@mode","selectall")
                };
                DataTable dt1 = SqlHelper.ExecuteDataTable(_connect.ConnectionString(),CommandType.StoredProcedure, "Sp_Banner",_parm);

                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    foreach (DataRow drow in dt1.Rows)
                    {
                        bnrmodel.Banner_Name = Convert.ToString(drow["File_Path"]);
                        bnrmodel.File_Path = drow["File_Path"] as IFormFile;
                        bnrmodel.Content = drow["Content"].ToString().Trim();
                    }
                }
               
            }
            catch (Exception)
            {

               // throw;
            }
            return new JsonResult(bnrmodel);
        }
        [HttpGet("GetBannerDetialsbyid")]
        public IActionResult GetBannerDetialsbyid(Int32 bid)
        {
            BannerModel bnrmodel = new BannerModel();
            try
            {
                SqlParameter[] _parm = { new SqlParameter("@Banner_Id",bid),
                                     new SqlParameter("@mode","selectbyid")
                };
                DataTable dt1 = SqlHelper.ExecuteDataTable(_connect.ConnectionString(), CommandType.StoredProcedure, "Sp_Banner",_parm);

                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    foreach (DataRow drow in dt1.Rows)
                    {
                        bnrmodel.Banner_Name = Convert.ToString(drow["Banner_Name"]);
                        bnrmodel.File_Path = drow["File_Path"] as IFormFile;
                        bnrmodel.Content = drow["Content"].ToString().Trim();
                    }
                }

            }
            catch (Exception)
            {

                // throw;
            }
            return new JsonResult(bnrmodel);
        }

        [HttpDelete("DeleteBannerDetials")]
        public IActionResult DeleteBannerDetials(Int32 bid)
        {
            BannerModel bnrmodel = new BannerModel();
            try
            {
                SqlParameter[] _parmdel = { new SqlParameter("@Banner_Id",bid),
                                     new SqlParameter("@mode","delete")
             };
               
                int result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.StoredProcedure, "Sp_Banner", _parmdel);
                if (result > 0)
                {
                    msg = "Sub Menu Delete Successfully";
                }
                else
                {
                    msg = "Sub Menu Can not be Deleted";
                }

            }
            catch (Exception)
            {

                // throw;
            }
            return new JsonResult(bnrmodel);
        }
    }
}
