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
    public class UploadController : Controller
    {
        //private const string BaseUrl = "$'{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}'";
        //private const string BaseUrl = "http://localhost:5000";
        //private const string BaseUrl = "https://bpt-servicewebapi20200509090056.azurewebsites.net";
        //private const string BaseUrl = $"{Request.Scheme}://{this.Request.Host}";

        private readonly IWebHostEnvironment _env;
        private static IHttpContextAccessor _httpContextAccessor;
        public static HttpContext Current => _httpContextAccessor.HttpContext;
        public static string BaseUrl => $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";

        #region Constructor

        public UploadController(IWebHostEnvironment env, IHttpContextAccessor contextAccessor)
        {
            _env = env;
            _httpContextAccessor = contextAccessor;
        }

        #endregion Constructor

        [HttpPost]
        [Route("saveImage/{type}")]
        public async Task<IActionResult> SaveImage([FromForm(Name = "postedFile")] IFormFile postedFile, int userId, string type)
        {
            try
            {
                string returnPath = "";
                if (postedFile != null && postedFile.Length > 0)
                {
                    int MaxContentLength = 1024 * 1024 * 5; //Size = 5 MB

                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png", ".jpeg" };
                    var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();
                    if (!AllowedFileExtensions.Contains(extension))
                    {
                        var message = string.Format("Please Upload image of type .jpg,.gif,.png., .jpeg");

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
                        else if (type == "service")
                        {
                            directory = "/UploadedFiles/Services/";
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
                        var nameImage = type + System.DateTime.Now.ToString("MM_dd_yyyy_h_mm_ss_fffff_tt") + ext;
                        string path = Path.Combine(_env.WebRootPath + directory, nameImage);
                        //Userimage myfolder name where i want to save my image
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(fileStream);
                        }
                        returnPath = BaseUrl + directory + nameImage;
                    }
                    var message1 = string.Format("Image Updated Successfully.");
                    return new JsonResult(returnPath);
                }
                var res = string.Format("Please Upload a image.");
                return new OkObjectResult(res);
            }
            catch (Exception)
            {
                var message1 = string.Format("Image Updated UnSuccessfully.");
                return new OkObjectResult(message1);
            }
        }
    }
}