namespace EduWebAPI.Common
{
    public struct FileInfodetails
    {
        public bool status;
        public string messge { get; set; }
        public string filepath {  get; set; }
    }
   
    public class Common
    {
      public bool IsValidImage(byte[] header,IFormFile file)
       {
            try
            {
                // JPEG Magic Number
                if (header[0] == 0xFF && header[1] == 0xD8)
                {
                    return true;
                }

                // PNG Magic Number
                if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
                {
                    return true;
                }
                // SVG check (text file)
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string content = reader.ReadToEnd().ToLower();

                    if (content.Contains("<svg") && !content.Contains("<script"))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }

       }

    }
}
