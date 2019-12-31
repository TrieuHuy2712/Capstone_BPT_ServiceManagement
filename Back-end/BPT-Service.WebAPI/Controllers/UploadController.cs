using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("UploadImage")]
    public class UploadController : ControllerBase
    {
        private const string BaseUrl = '$"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}"';
        #region  Constructor
        public UploadController()
        {

        }
        #endregion
        [HttpPost]
        [Route("saveImage/{type}")]
        public async Task<IActionResult> SaveImage([FromForm(Name = "uploadedFile")] IFormFile postedFile, long userId, string type)
        {
            try
            {
                if (postedFile != null && postedFile.Length > 0)
                {

                    int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB

                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                    var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();
                    if (!AllowedFileExtensions.Contains(extension))
                    {

                        var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                        return new ObjectResult(message);
                    }
                    else if (postedFile.Length > MaxContentLength)
                    {

                        var message = string.Format("Please Upload a file upto 1 mb.");

                        return new ObjectResult(message);
                    }
                    else
                    {
                        string directory = string.Empty;
                        if (type == "avatar")
                        {
                            directory = "/UploadedFiles/Avatars/";
                        }
                        else if (type == "product")
                        {
                            directory = "/UploadedFiles/Products/";
                        }
                        else if (type == "news")
                        {
                            directory = "/UploadedFiles/News/";
                        }
                        else if (type == "banner")
                        {
                            directory = "/UploadedFiles/Banners/";
                        }
                        else
                        {
                            directory = "/UploadedFiles/";
                        }
                        if (!Directory.Exists((directory)))
                        {
                            Directory.CreateDirectory(BaseUrl + "/" + directory);
                        }

                        string path = Path.Combine(BaseUrl + "/" + directory, postedFile.FileName);
                        //Userimage myfolder name where i want to save my image
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(fileStream);
                        }
                    }
                    var message1 = string.Format("Image Updated Successfully.");
                    return new OkObjectResult(message1);
                }
                var res = string.Format("Please Upload a image.");
                return new OkObjectResult(res);

            }
            catch (Exception ex)
            {
                var message1 = string.Format("Image Updated UnSuccessfully.");
                return new OkObjectResult(message1);

            }
        }
    }
}