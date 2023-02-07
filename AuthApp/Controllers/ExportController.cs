using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using HtmlToOpenXml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ExportController(IWebHostEnvironment webHostEnvironment)
        {
            _hostingEnvironment = webHostEnvironment;
        }
        [HttpPost]
        public ActionResult Post(dynamic htmlString)
        {
            try
            {
                //var contentRootPath = _hostingEnvironment.ContentRootPath;
                //var htmlString = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "Hubs", "Template", "Email", "BookingGuestMail.html"));
                MemoryStream stream = new MemoryStream();
                using WordprocessingDocument package = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);

                MainDocumentPart mainPart = package.MainDocumentPart;
                if (mainPart == null)
                {
                    mainPart = package.AddMainDocumentPart();
                    new DocumentFormat.OpenXml.Wordprocessing.Document(new DocumentFormat.OpenXml.Wordprocessing.Body()).Save(mainPart);
                }

                HtmlConverter converter = new HtmlConverter(mainPart);
                converter.ParseHtml(htmlString);
                mainPart.Document.Save();
                package.Close();
                stream.Position = 0;
                stream = new MemoryStream(stream.ToArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "test.docx");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

