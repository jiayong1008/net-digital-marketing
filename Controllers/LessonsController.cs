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
    public class LessonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LessonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lessons
        public async Task<IActionResult> Index()
        {
              return _context.Lesson != null ? 
                          View(await _context.Lesson.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Lesson'  is null.");
        }

        // GET: Lessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Lesson == null) { return NotFound(); }

            var lesson = await _context.Lesson
                .FirstOrDefaultAsync(m => m.LessonId == id);
            if (lesson == null) { return NotFound(); }

            ViewBag.ModuleList = _context.Module.ToList();
            var lessonForm = new LessonCreateFormModel
            {
                LessonId = lesson.LessonId,
                Name = lesson.Name,
                Duration = lesson.Duration,
                LessonOrder = lesson.LessonOrder,
                ModuleId = lesson.Module.ModuleId
            };

            return View(lessonForm);
        }

        // GET: Lessons/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.ModuleList = _context.Module.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Duration,LessonOrder,ModuleId")] LessonCreateFormModel lessonForm)
        {
            if (ModelState.IsValid)
            {
                var module = await _context.Module.FindAsync(lessonForm.ModuleId);
                if (module == null) { return NotFound(); }

                var lesson = new Lesson
                {
                    Name = lessonForm.Name,
                    Duration = lessonForm.Duration,
                    LessonOrder = lessonForm.LessonOrder,
                    Module = module
                };
                _context.Add(lesson);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.ModuleList = _context.Module.ToList();
            return View(lessonForm);
        }

        // GET: Lessons/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Lesson == null) { return NotFound(); }
            var lesson = await _context.Lesson.Include(lesson => lesson.Module).FirstOrDefaultAsync(lesson => lesson.LessonId == id);
            if (lesson == null) { return NotFound(); }

            var lessonForm = new LessonCreateFormModel
            {
                LessonId = lesson.LessonId,
                Name = lesson.Name,
                Duration = lesson.Duration,
                LessonOrder = lesson.LessonOrder,
                ModuleId = lesson.Module.ModuleId
            };

            ViewBag.ModuleList = _context.Module.ToList();
            return View(lessonForm);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("LessonId,Name,Duration,LessonOrder, ModuleId")] LessonCreateFormModel lessonForm)
        {
            if (id != lessonForm.LessonId) { return NotFound(); }

            if (ModelState.IsValid)
            {
                var module = await _context.Module.FindAsync(lessonForm.ModuleId);
                if (module == null) { return NotFound(); }

                var lesson = new Lesson
                {
                    LessonId = lessonForm.LessonId,
                    Name = lessonForm.Name,
                    Duration = lessonForm.Duration,
                    LessonOrder = lessonForm.LessonOrder,
                    Module = module
                };

                _context.Update(lesson);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.ModuleList = _context.Module.ToList();
            return View(lessonForm);
        }

        // GET: Lessons/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Lesson == null) return NotFound(); 

            var lesson = await _context.Lesson
                .Include(lesson => lesson.Module)
                .FirstOrDefaultAsync(m => m.LessonId == id);
            if (lesson == null) return NotFound();

            var lessonForm = new LessonCreateFormModel
            {
                LessonId = lesson.LessonId,
                Name = lesson.Name,
                Duration = lesson.Duration,
                LessonOrder = lesson.LessonOrder,
                ModuleId = lesson.Module.ModuleId
            };

            ViewBag.ModuleList = _context.Module.ToList();
            return View(lessonForm);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Lesson == null)
                return Problem("Entity set 'ApplicationDbContext.Lesson'  is null.");

            var lesson = await _context.Lesson.FindAsync(id);
            if (lesson != null)
                _context.Lesson.Remove(lesson);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LessonExists(int id)
        {
          return (_context.Lesson?.Any(e => e.LessonId == id)).GetValueOrDefault();
        }
    }
}
