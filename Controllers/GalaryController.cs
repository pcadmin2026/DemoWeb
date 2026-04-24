using EduWebAPI.Common;
using EduWebAPI.Model;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMSCommon;
using System.Data;
using System.Data.SqlClient;

namespace EduWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalaryController : ControllerBase
    {
        private readonly Connect? _connect;
        string msg = "";
        string sql = "";
        Common.Common cmm = new Common.Common();
        FileInfodetails _fileInfodetails = new FileInfodetails();
        private readonly IWebHostEnvironment _webhost;
        public GalaryController(Connect connect, IWebHostEnvironment webhost)
        {
            _connect = connect;
            _webhost = webhost;
        }
        [HttpPost("AddGalleryImage")]
        public IActionResult AddGalleryImage([FromForm] GalaryModel? galleryimg)
        {
            try
            {

                string file = "", filepath = "";
                file = UploadImage(galleryimg.Imagepath);
                if (!_fileInfodetails.status)
                {
                    msg = _fileInfodetails.messge;
                    return Ok(msg);
                }
                filepath = _fileInfodetails.filepath;
                
                SqlParameter[] _param2 =
                {

                            new SqlParameter("@Main_menu_Id",Convert.ToInt32(galleryimg.Mainmenu_Id)),
                            new SqlParameter("@Sub_menu_id",galleryimg.Submenu_id),
                            new SqlParameter("@Image_path",filepath),
                            new SqlParameter("@Image_content",galleryimg.Imagecontent),
                            new SqlParameter("@Image_Name",galleryimg.ImageName),
                            new SqlParameter("@IsActive",galleryimg.IsActive),
                            new SqlParameter("@Createdby",galleryimg.Createdby)
                        };
                string qry = @"insert into Tbl_Galary_Images(Main_menu_Id,Sub_menu_id,Image_path,Image_content,Image_Name,
                        IsActive,Createdby) values(@Main_menu_Id,@Sub_menu_id,@Image_path,@Image_content,@Image_Name,
                        @IsActive,@Createdby)";
               Int32 result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, qry, _param2);
                if (result>0)
                {
                    msg = $"Image Added Successfully";
                }

            }
            catch (Exception ex)
            {

                //throw ex;
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

            string folder = Path.Combine(_webhost.WebRootPath, "GalleryImage");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + extension;
            string foldername=new DirectoryInfo(folder).Name;
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
        [HttpPut("UpdateGalleryImages")]
        public IActionResult UpdateGalleryImages([FromForm] GalaryModel? galleryimage, Int32 galleryid)
        {
            try
            {

                string file = "", filepath = "";
                file = UploadImage(galleryimage.Imagepath);
                if (!_fileInfodetails.status)
                {
                    msg = _fileInfodetails.messge;
                    return Ok(msg);
                }
                filepath = _fileInfodetails.filepath;

                SqlParameter[] _param2 =
                {
                            new SqlParameter("@Main_menu_Id",Convert.ToInt32(galleryimage.Mainmenu_Id)),
                            new SqlParameter("@Sub_menu_id",galleryimage.Submenu_id),
                            new SqlParameter("@Image_path",galleryimage.Imagepath),
                            new SqlParameter("@Image_content",galleryimage.Imagecontent),
                            new SqlParameter("@Image_Name",galleryimage.ImageName),
                            new SqlParameter("@IsActive",galleryimage.IsActive),
                           
                            new SqlParameter("@Updatedby","admin"),
                            new SqlParameter("@GImg_Id",galleryid)
                        };
                sql = @"update Tbl_Galary_Images set Main_menu_Id=@Main_menu_Id,Sub_menu_id=@Sub_menu_id
                        ,Image_path=@Image_path,Image_content=@Image_content,Image_Name=@Image_Name,IsActive=@IsActive
                        ,UpdatedOn=GETDATE(),Updatedby=@Updatedby
                        where GImg_Id=@GImg_Id and IsActive=1;";
                Int32 result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _param2);
                if (result > 0)
                {
                    msg = $"Updated Successfully";
                }

            }
            catch (Exception ex)
            {

                //throw ex;
            }
            return new JsonResult(msg);
        }
        [HttpDelete("DeletegalleryDetials")]
        public IActionResult DeletegalleryDetials(Int32 galbid)
        {
            ClassImageModel climodel = new ClassImageModel();
            try
            {
                SqlParameter[] _parmdel = { new SqlParameter("@GImg_Id",galbid)

            };
                sql = @"delete from Tbl_Galary_Images where 
                        GImg_Id = @GImg_Id and isnull(IsActive,'0')= 1";
                int result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _parmdel);
                if (result > 0)
                {
                    msg = " Image Delete Successfully Delete Successfully";
                }
                else
                {
                    msg = "Image Can not be Deleted";
                }

            }
            catch (Exception)
            {

                // throw;
            }
            return new JsonResult(msg);
        }
        [HttpGet("GetAllGalleryimage")]
        public IActionResult GetAllGalleryimage()
        {
            List<GalaryModel> lstgllery = new List<GalaryModel>();
            try
            {
                sql = @"select GImg_Id,Main_menu_Id,Sub_menu_id,isnull(Image_path,'')Image_path
                ,isnull(Image_content,'')Image_content,isnull(Image_Name,'')Image_Name
                ,isnull(IsActive,'0')IsActive,convert(varchar(10),Createdon,103)Createdon,
                isnull(Createdby,'')Createdby from Tbl_Galary_Images where IsActive=1";

                string conn = _connect.ConnectionString();
                DataTable dtgallery = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
                if (dtgallery != null && dtgallery.Rows.Count > 0)
                {
                    foreach (DataRow drow in dtgallery.Rows)
                    {
                        lstgllery.Add(
                            new GalaryModel
                            {
                                Mainmenu_Id = Convert.ToInt32(drow["Main_menu_Id"]),
                                Submenu_id = Convert.ToInt32(drow["Sub_menu_id"]),
                                Filepath = Convert.ToString(drow["Image_path"]),
                                Imagecontent = Convert.ToString(drow["Image_content"]),
                                ImageName    = Convert.ToString(drow["Image_Name"]),
                                IsActive = Convert.ToBoolean(drow["IsActive"])                           
                            }
                        );
                    }
                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(lstgllery.Count > 0 ? lstgllery : "Record not found");
        }

        [HttpGet("GetGalleryimagebyid")]
        public IActionResult GetGalleryimagebyid(Int32 gimgid)
        {
            List<GalaryModel> lstgllery = new List<GalaryModel>();
            try
            {
                sql = @"select GImg_Id,Main_menu_Id,Sub_menu_id,isnull(Image_path,'')Image_path
                ,isnull(Image_content,'')Image_content,isnull(Image_Name,'')Image_Name
                ,isnull(IsActive,'0')IsActive,convert(varchar(10),Createdon,103)Createdon,
                isnull(Createdby,'')Createdby from Tbl_Galary_Images where IsActive=1 and GImg_Id=@GImg_Id ";
                SqlParameter parm = new SqlParameter("@GImg_Id", gimgid);
                string conn = _connect.ConnectionString();
                DataTable dtgallery = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parm);
                if (dtgallery != null && dtgallery.Rows.Count > 0)
                {
                    lstgllery = (from drow in dtgallery.AsEnumerable()
                                 select new GalaryModel
                                 {
                                     Mainmenu_Id = Convert.ToInt32(drow["Main_menu_Id"]),
                                     Submenu_id = Convert.ToInt32(drow["Sub_menu_id"]),
                                     Filepath = Convert.ToString(drow["Image_path"]),
                                     Imagecontent = Convert.ToString(drow["Image_content"]),
                                     ImageName = Convert.ToString(drow["Image_Name"]),
                                     IsActive = Convert.ToBoolean(drow["IsActive"])
                                 }).ToList();
                }

            }
            catch (Exception ex)
            {

                //throw;
            }
            return new JsonResult(lstgllery.Count > 0 ? lstgllery : "Record not found");
        }
    }
}
