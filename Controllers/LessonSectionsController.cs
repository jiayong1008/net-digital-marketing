using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DigitalMarketing2.Data;
using DigitalMarketing2.Models;
using Microsoft.AspNetCore.Authorization;

namespace DigitalMarketing2.Controllers
{
    public class LessonSectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LessonSectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LessonSections
        public async Task<IActionResult> Index()
        {
            return _context.LessonSection != null ?
                View(await _context.LessonSection.Include(ls => ls.Lesson).ToListAsync()) :
                Problem("Entity set 'ApplicationDbContext.LessonSection'  is null.");

            //var lessonSections = await _context.LessonSection
            //    .Include(ls => ls.Lesson)
            //    .Where(ls => ls is TextLessonSection || ls is ImageLessonSection)
            //    .Select(ls => new LessonSectionViewModel
            //    {
            //        LessonSectionId = ls.LessonSectionId,
            //        LessonId = ls.Lesson.LessonId,
            //        LessonName = ls.Lesson.Name,
            //        LessonSectionOrder = ls.Lesson.LessonOrder,
            //        Text = ls is TextLessonSection ? ((TextLessonSection)ls).Text : "N/A",
            //        Image = ls is ImageLessonSection ? ((ImageLessonSection)ls).Image : null,
            //    }).ToListAsync();

            //return View(lessonSections);
        }

        // GET: LessonSections/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LessonSection == null)
                return NotFound();

            var lessonSection = await _context.LessonSection
                .Include(ls => ls.Lesson)
                .FirstOrDefaultAsync(m => m.LessonSectionId == id);
            if (lessonSection == null)
                return NotFound();

            // Convert byte[] ImageData to IFormFile
            IFormFile imageFile = await ConvertImageDataToFormFile(lessonSection.ImageData);

            // Generating specific LessonSectionFormModel based on its Text / Image property
            var lessonSectionForm = new LessonSectionFormModel
            {
                LessonSectionId = lessonSection.LessonSectionId,
                LessonSectionOrder = lessonSection.LessonSectionOrder,
                Text = lessonSection.Text,
                Image = imageFile ?? null,
                LessonId = lessonSection.Lesson.LessonId
            };

            ViewBag.LessonSelectList = await _context.Lesson.ToListAsync();
            return View(lessonSectionForm);
        }

        // GET: LessonSections/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int? LessonId) 
        {
            if (LessonId == null) return NotFound();

            //ViewBag.LessonSelectList = _context.Lesson.ToList();

            var lessonSectionForm = new LessonSectionFormModel
            {
                LessonId = (int) LessonId
            };
            return View(lessonSectionForm); 
        }

        // POST: LessonSections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("LessonId, LessonSectionOrder, Text, Image")] LessonSectionFormModel lessonSectionForm)
        {
            ViewBag.LessonSelectList = await _context.Lesson.ToListAsync();
            if (!ModelState.IsValid) return View(lessonSectionForm);

            // Check whether ONLY ONE of Text or Image is provided
            if (string.IsNullOrEmpty(lessonSectionForm.Text) && lessonSectionForm.Image == null)
            {
                ModelState.AddModelError(string.Empty, "Please provide either Text or Image.");
                return View(lessonSectionForm);
            }

            if (!string.IsNullOrEmpty(lessonSectionForm.Text) && lessonSectionForm.Image != null)
            {
                ModelState.AddModelError(string.Empty, "Please provide either Text or Image, not both.");
                return View(lessonSectionForm);
            }

            var lesson = await _context.Lesson.FindAsync(lessonSectionForm.LessonId);
            if (lesson == null) return NotFound();

            // Convert the IFormFile to a byte array
            byte[] imageData = null;
            if (lessonSectionForm.Image != null)
            {
                using (var stream = new MemoryStream())
                {
                    await lessonSectionForm.Image.CopyToAsync(stream);
                    imageData = stream.ToArray();
                }
            }

            // Create new Lesson Section
            var lessonSection = new LessonSection
            {
                LessonSectionOrder = lessonSectionForm.LessonSectionOrder,
                Text = lessonSectionForm.Text,
                ImageData = imageData,
                ImageType = lessonSectionForm.Image?.ContentType,
                Lesson = lesson
            };

            _context.Add(lessonSection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), "Lessons", new { id = lesson.LessonId });
        }

        // GET: LessonSections/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            // Check if lesson section exists
            if (id == null || _context.LessonSection == null)
                return NotFound();

            var lessonSection = await _context.LessonSection
                .Include(ls => ls.Lesson)
                .FirstOrDefaultAsync(ls => ls.LessonSectionId == id);
            if (lessonSection == null) return NotFound();

            // Convert byte[] ImageData to IFormFile
            IFormFile imageFile = await ConvertImageDataToFormFile(lessonSection.ImageData);

            // Generating specific LessonSectionFormModel based on its Text / Image property
            var lessonSectionForm = new LessonSectionFormModel
            {
                LessonSectionId = lessonSection.LessonSectionId,
                LessonSectionOrder = lessonSection.LessonSectionOrder,
                Text = lessonSection.Text,
                Image = imageFile ?? null,
                LessonId = lessonSection.Lesson.LessonId
            };

            ViewBag.LessonSelectList = await _context.Lesson.ToListAsync();
            return View(lessonSectionForm);
        }

        // POST: LessonSections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("LessonSectionId,LessonSectionOrder, Text, Image, LessonId, RemoveImage")] LessonSectionFormModel lessonSectionForm)
        {
            // Basic Edit Validation Check
            if (id != lessonSectionForm.LessonSectionId)
                return NotFound();

            var lessonSection = await _context.LessonSection.FindAsync(id);
            if (lessonSection == null) return NotFound();

            ViewBag.LessonSelectList = await _context.Lesson.ToListAsync();
            if (!ModelState.IsValid) return View(lessonSectionForm);

            // Check if there is Image data (Existing or replaced or manually added)
            var hasImageData = false;
            if (lessonSectionForm.Image != null || (lessonSection.ImageData != null && !lessonSectionForm.RemoveImage)) 
                hasImageData = true;

            // Check whether ONLY ONE of Text or Image is provided
            if (!string.IsNullOrEmpty(lessonSectionForm.Text) && hasImageData) // Both provided
            {
                // Convert byte[] ImageData to IFormFile
                lessonSectionForm.Image = await ConvertImageDataToFormFile(lessonSection.ImageData);
                ModelState.AddModelError(string.Empty, "Please provide either Text or Image, not both.");

                return View(lessonSectionForm);
            }
            else if (string.IsNullOrEmpty(lessonSectionForm.Text) && !hasImageData) // Both not provided
            {
                // Convert byte[] ImageData to IFormFile
                lessonSectionForm.Image = await ConvertImageDataToFormFile(lessonSection.ImageData);
                ModelState.AddModelError(string.Empty, "Please provide either Text or Image.");

                return View(lessonSectionForm);
            }

            // All validations passed, proceed with update procedures
            var lesson = await _context.Lesson.FindAsync(lessonSectionForm.LessonId);
            if (lesson == null) return NotFound();

            lessonSection.LessonSectionOrder = lessonSectionForm.LessonSectionOrder;
            lessonSection.Text = lessonSectionForm.Text;
            lessonSection.Lesson = lesson;

            // If remove image is requested, remove the image
            if (lessonSection.ImageData != null && lessonSectionForm.RemoveImage)
            {
                lessonSection.ImageData = null;
                lessonSection.ImageType = null;
            }

            // If replace image is requested, replace the image (Convert IFormFile to byte[] ImageData)
            if (lessonSectionForm.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await lessonSectionForm.Image.CopyToAsync(memoryStream);
                    lessonSection.ImageData = memoryStream.ToArray();
                }
                lessonSection.ImageType = lessonSectionForm.Image?.ContentType;
            }

            try
            {
                _context.Update(lessonSection);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonSectionExists(lessonSectionForm.LessonSectionId))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Details), "Lessons", new { Id = lesson.LessonId });
        }

        // GET: LessonSections/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Lesson == null) return NotFound();

            var lessonSection = await _context.LessonSection
                .Include(ls => ls.Lesson)
                .FirstOrDefaultAsync(m => m.LessonSectionId == id);
            if (lessonSection == null) return NotFound();

            // Convert byte[] ImageData to IFormFile
            IFormFile imageFile = await ConvertImageDataToFormFile(lessonSection.ImageData);

            // Generating specific LessonSectionFormModel based on its Text / Image property
            var lessonSectionForm = new LessonSectionFormModel
            {
                LessonSectionId = lessonSection.LessonSectionId,
                LessonSectionOrder = lessonSection.LessonSectionOrder,
                Text = lessonSection.Text,
                Image = imageFile ?? null,
                LessonId = lessonSection.Lesson.LessonId
            };

            ViewBag.LessonSelectList = await _context.Lesson.ToListAsync();
            return View(lessonSectionForm);
        }

        // POST: LessonSections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LessonSection == null)
                return Problem("Entity set 'ApplicationDbContext.LessonSection'  is null.");
            
            var lessonSection = await _context.LessonSection
                .Include(ls => ls.Lesson)
                .FirstOrDefaultAsync(ls => ls.LessonSectionId == id);
            if (lessonSection ==  null) return NotFound();

            var lessonId = lessonSection.LessonId;
            _context.LessonSection.Remove(lessonSection);
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Lessons", new { id = lessonId });
        }

        private bool LessonSectionExists(int id)
        {
          return (_context.LessonSection?.Any(e => e.LessonSectionId == id)).GetValueOrDefault();
        }


        // =====================      HELPER FUNCTIONS        ==========================
        // Retrieve Image and load in front end
        public IActionResult GetImage(int id)
        {
            var lessonSection = _context.LessonSection.FirstOrDefault(ls => ls.LessonSectionId == id);
            if (lessonSection != null && lessonSection.IsImage && lessonSection.ImageData != null)
            {
                return new FileContentResult(lessonSection.ImageData, lessonSection.ImageType);
            }
            else
            {
                return NotFound();
            }
        }

        // Convert byte[] ImageData to IFormFile
        private async Task<IFormFile> ConvertImageDataToFormFile(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            var ms = new MemoryStream(imageData);
            return new FormFile(ms, 0, ms.Length, "Image", "lessonSection.jpg");
        }

    }
}
