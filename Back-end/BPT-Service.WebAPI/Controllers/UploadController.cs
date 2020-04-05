using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Authorize]
    [Route("UploadImage")]
    public class UploadController : ControllerBase
    {
        //private const string BaseUrl = "$'{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}'";
        private const string BaseUrl = "http://localhost:5000";

        private readonly IWebHostEnvironment _env;

        #region Constructor

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        #endregion Constructor

        [HttpPost]
        [Route("saveImage/{type}")]
        public async Task<IActionResult> SaveImage([FromForm(Name = "postedFile")] IFormFile postedFile, long userId, string type)
        {
            try
            {
                string returnPath = "";
                if (postedFile != null && postedFile.Length > 0)
                {
                    int MaxContentLength = 1024 * 1024 * 5; //Size = 5 MB

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
                        else if (type == "location")
                        {
                            directory = "/UploadedFiles/Location/";
                        }
                        else if (type == "category")
                        {
                            directory = "/UploadedFiles/Categories/";
                        }
                        else
                        {
                            directory = "/UploadedFiles/";
                        }
                        if (!Directory.Exists(_env.WebRootPath + directory))
                        {
                            Directory.CreateDirectory(_env.WebRootPath + directory);
                        }
                        var nameImage = type + System.DateTime.Now.ToString("MM_DD_YYYY_h_mm_ss_fffff_tt") + ext;
                        string path = Path.Combine(_env.WebRootPath + directory, nameImage);
                        //Userimage myfolder name where i want to save my image
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(fileStream);
                        }
                        returnPath = BaseUrl + directory + "/" + nameImage;
                    }
                    var message1 = string.Format("Image Updated Successfully.");
                    return new JsonResult(returnPath);
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