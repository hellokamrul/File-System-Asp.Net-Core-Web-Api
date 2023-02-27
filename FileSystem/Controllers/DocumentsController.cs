using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileSystem.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Cors;

namespace FileSystem.Controllers
{
    [EnableCors]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly MultipleFileSystemContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentsController(MultipleFileSystemContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> GetDocuments()
        {
            return await _context.Documents.ToListAsync();
        }
        [HttpGet]
        [Route("User/{uid}")]
        public List<Document> GetDocumentById(int uid)
        {
            var documents = _context.Documents.Where(x => x.Uid == uid).ToList();
            return documents;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            return document;
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocument(int id, Document document)
        {
            if (id != document.Id)
            {
                return BadRequest();
            }

            _context.Entry(document).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

       
        [HttpPost]
        public async Task<ActionResult<Document>> PostDocument(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocument", new { id = document.Id }, document);
        }

        [HttpGet]
        [Route("getfilenames")]
        public IActionResult GetFileNames()
        {
            Console.WriteLine(_webHostEnvironment.WebRootPath);
            var files = Directory.GetFiles("D:\\ASP.Net Core\\New folder (4)\\FileSystem\\FileSystem\\App_Data");

            var fileNames = new List<string>();

            foreach (var file in files)
            {
                fileNames.Add(Path.GetFileName(file));
            }

            return Ok(fileNames);
        }

        [HttpPost]
        [Route("upload")]
        public async Task<ActionResult> Upload(List<IFormFile> files)
        {
           
            long size = files.Sum(f => f.Length);
            var rootPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Documents");

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            foreach (var file in files)
            {
                var filePath = Path.Combine(rootPath, file.FileName);
                using (var steam = new FileStream(filePath, FileMode.Create))
                {
                    var document = new Document
                    {
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        Length = file.Length,
                        Uid = 1
                    };
                    await file.CopyToAsync(steam);

                    _context.Documents.Add(document);
                    await _context.SaveChangesAsync();

                }

            }

            return Ok(new { count = files.Count, size });

        }

        [HttpPost]
        [Route("uploadfile")]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                var httpRequest = HttpContext.Request;
                if (httpRequest.Form.Files.Count > 0)
                {
                    var file = httpRequest.Form.Files[0];
                    var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest("No file uploaded.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while uploading the file.");
            }
        }



        [HttpPost]
        [Route("download/{id}")]
        public async Task<ActionResult> Download(int id)
        {
            var provider = new FileExtensionContentTypeProvider();
            var document = await _context.Documents.FindAsync(id);

            if (document == null)
                return NotFound();

            var file = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Documents", document.FileName);

            string contentType;
            if (!provider.TryGetContentType(file, out contentType))
            {
                contentType = "application/octet-stream";
            }


            byte[] fileBytes;
            if (System.IO.File.Exists(file))
            {
                fileBytes = System.IO.File.ReadAllBytes(file);
            }
            else
            {
                return NotFound();
            }
            return File(fileBytes, contentType, document.FileName);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
    }
}
