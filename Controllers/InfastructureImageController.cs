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
    public class InfastructureImageController : ControllerBase
    {
        private readonly Connect? _connect;
        private string msg = "";
        private readonly IWebHostEnvironment _webhost;
        Common.Common cmm = new Common.Common();
        FileInfodetails _fileInfodetails = new FileInfodetails();
        string sql = "";
        public InfastructureImageController(Connect connect, IWebHostEnvironment iwebhost)
        {
            _connect = connect;
            _webhost = iwebhost;
        }
        [HttpPost("AddInfaImage")]
        public IActionResult AddInfaImage([FromForm] Infastructure_ImageModel? infaimage)
        {
            try
            {

                string file = "", filepath = "";
                file = UploadImage(infaimage.File_Path);
                if (!_fileInfodetails.status)
                {
                    msg = _fileInfodetails.messge;
                    return Ok(msg);
                }
                filepath = _fileInfodetails.filepath;


                SqlParameter[] _parambarner =
                {
                        new SqlParameter("@submenuid",Convert.ToInt32(infaimage.Sub_Menu_Id)),
                        new SqlParameter("@img_Name",infaimage.Image_Name)

                };
                string con = _connect.ConnectionString();
                DataTable dtbaner = SqlHelper.ExecuteDataTable(con, "sp_duplicate_InfaImage", _parambarner);
                if (dtbaner != null && dtbaner.Rows.Count > 0)
                {
                    msg = "Class Image Already Exists";
                    return new JsonResult(msg);
                }
                SqlParameter[] _param2 =
                {

                            new SqlParameter("@Sub_Menu_Id",Convert.ToInt32(infaimage.Sub_Menu_Id)),
                            new SqlParameter("@Image_Name",infaimage.Image_Name),
                            new SqlParameter("@File_Path",filepath),
                            new SqlParameter("@Content",infaimage.Content),
                            new SqlParameter("@Createdby",infaimage.Createdby)

                 };
                string qry = @"insert into Tbl_Infastructure_Image_details(Sub_Menu_Id,Image_Name,File_Path,Content,Createdby)
                                values(@Sub_Menu_Id,@Image_Name,@File_Path,@Content,@Createdby);
                    select ISNULL(Image_Name,'')Image_Name from Tbl_Infastructure_Image_details where Infastructure_image_id=SCOPE_IDENTITY();";
                string result = (string)SqlHelper.ExecuteScalar(_connect.ConnectionString(), CommandType.Text, qry, _param2);
                if (!string.IsNullOrEmpty(result))
                {
                    msg = $"{result} Added Successfully";
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

            string folder = Path.Combine(_webhost.WebRootPath, "ClassImage");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + extension;

            string filePath = Path.Combine(folder, fileName);
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                file.CopyTo(fs);
                _fileInfodetails.status = true;
                _fileInfodetails.filepath = $"../ClassImage/{fileName}";
            }
            else
            {
                _fileInfodetails.status = false;
                return _fileInfodetails.messge = "dose not exits filepath";
            }

            return _fileInfodetails.filepath;
        }
        [HttpPut("UpdateInfaImages")]
        public IActionResult UpdateInfaImages([FromForm] Infastructure_ImageModel? infaimage, Int32 infaimgid)
        {
            try
            {

                string file = "", filepath = "";
                file = UploadImage(infaimage.File_Path);
                if (!_fileInfodetails.status)
                {
                    msg = _fileInfodetails.messge;
                    return Ok(msg);
                }
                filepath = _fileInfodetails.filepath;

                SqlParameter[] _param2 =
                {
                            new SqlParameter("@Sub_Menu_Id",Convert.ToInt32(infaimage.Sub_Menu_Id)),
                            new SqlParameter("@Image_Name",infaimage.Image_Name),
                            new SqlParameter("@File_Path",filepath),
                            new SqlParameter("@Content",infaimage.Content),
                            new SqlParameter("@Updateby",infaimage.Updateby),
                            new SqlParameter("@UpdatedOn",infaimage.Updatedon),
                            new SqlParameter("@Infastructure_image_id",infaimgid)
                        };
                sql = @"update Tbl_Infastructure_Image_details set Sub_Menu_Id=@Sub_Menu_Id,Image_Name=@Image_Name
                        ,File_Path=@File_Path,Content=@Content,IsActive=@IsActive
                        ,Updateby=@Updateby,UpdatedOn=@UpdatedOn
                        where Infastructure_image_id=@Infastructure_image_id and IsActive=1;
                        ";
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
        [HttpGet("GetAllInfaImagesDetials")]
        public IActionResult GetAllInfaImagesDetials()
        {
            Infastructure_ImageModel infaimodel = new Infastructure_ImageModel();
            try
            {
                sql = @"select Infastructure_image_id,ISNULL(Image_Name,'')Image_Name
                        ,ISNULL(File_Path,'')File_Path,Content from Tbl_Infastructure_Image_details
                         where isnull(IsActive,'0')=1";
                DataTable dt1 = SqlHelper.ExecuteDataTable(_connect.ConnectionString(), CommandType.Text, sql);

                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    foreach (DataRow drow in dt1.Rows)
                    {

                        infaimodel.Image_Name = Convert.ToString(drow["Image_Name"]);
                        infaimodel.Image_Url = Convert.ToString(drow["File_Path"]);
                        infaimodel.Content = drow["Content"].ToString().Trim();
                    }
                }

            }
            catch (Exception)
            {

                // throw;
            }
            return new JsonResult(infaimodel);
        }
        [HttpGet("GetAllInfaImagesDetialsbyid")]
        public IActionResult GetAllInfaImagesDetialsbyid(Int32 infaid)
        {
            Infastructure_ImageModel climodel = new Infastructure_ImageModel();
            try
            {
                SqlParameter[] _parm = { new SqlParameter("@Infastructure_image_id",infaid)
            };
                sql = @" select Infastructure_image_id,ISNULL(Image_Name,'')Image_Name,ISNULL(File_Path,'')File_Path,Content 
                            from Tbl_Infastructure_Image_details
                         where isnull(IsActive,'0')=1 and Infastructure_image_id=@Infastructure_image_id";
                DataTable dt1 = SqlHelper.ExecuteDataTable(_connect.ConnectionString(), CommandType.Text, sql, _parm);

                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    foreach (DataRow drow in dt1.Rows)
                    {
                        climodel.Image_Name = Convert.ToString(drow["Image_Name"]);
                        climodel.Image_Url = Convert.ToString(drow["File_Path"]);
                        climodel.Content = drow["Content"].ToString().Trim();
                    }
                }

            }
            catch (Exception)
            {

                // throw;
            }
            return new JsonResult(climodel);
        }
        [HttpDelete("DeleteInfaImagesDetials")]
        public IActionResult DeleteInfaImagesDetials(Int32 infaid)
        {
            ClassImageModel climodel = new ClassImageModel();
            try
            {
                SqlParameter[] _parmdel = { new SqlParameter("@Infastructure_image_id",infaid)

            };
                sql = @"delete from Tbl_Infastructure_Image_details where Infastructure_image_id = @Infastructure_image_id 
                        and isnull(IsActive,'0')= 1";
                int result = SqlHelper.ExecuteNonQuery(_connect.ConnectionString(), CommandType.Text, sql, _parmdel);
                if (result > 0)
                {
                    msg = "Image Delete Successfully Delete Successfully";
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
    }
}
