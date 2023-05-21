using DigitalMarketing2.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalMarketing2.Controllers
{
    public class DiscussionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscussionController(ApplicationDbContext context) 
        {
            _context = context;
        }

        // GET: DiscussionController
        public async Task<ActionResult> Index(int? ModuleId)
        {
            if (ModuleId == null) return NotFound();
            var module = await _context.Module.FindAsync(ModuleId);
            if (module == null) return NotFound();

            var discussions = await _context.Discussion
                .Where(d => d.ModuleId == ModuleId)
                .ToListAsync();

            return View(discussions);
        }

        // GET: DiscussionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DiscussionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DiscussionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DiscussionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DiscussionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DiscussionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DiscussionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
