using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UploadFilesApi.Models;
using static System.Net.Mime.MediaTypeNames;

namespace UploadFilesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly testdbContext _context;

        public UploadFileController(testdbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("UploadImage")]
        public IActionResult UploadImage(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyToAsync(memoryStream);
                        var imageData = memoryStream.ToArray();

                        var image = new Images
                        {
                            Name = file.FileName,
                            Data = imageData,
                            ContentType = file.ContentType
                        };

                        _context.Images.Add(image);
                        _context.SaveChanges();
                        return Ok(new { image.Id });
                    }
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("GetImage")]
        public IActionResult GetImage([FromBody] int id)
        {   
            var image = _context.Images.Find(id);

            if (image == null)
                return NotFound();

            return File(image.Data, image.ContentType);
        }
    }
}
