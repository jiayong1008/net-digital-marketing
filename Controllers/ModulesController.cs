using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DigitalMarketing2.Models;
using DigitalMarketing2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DigitalMarketing2.Controllers
{
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<User> _userManager;

        public ModulesController(ApplicationDbContext context, UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {
              return _context.Module != null ? 
                          View(await _context.Module.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Module' is null.");
        }

        // GET: Modules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Module == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            var @module = await _context.Module
                .Include(mod => mod.Lessons)
                .Include(mod => mod.Discussions)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(mod => mod.ModuleId == id);

            if (@module == null) return NotFound();

            var moduleDetailModel = new ModuleDetailModel
            {
                Module = @module,
                DiscussionForm = new DiscussionFormModel { UserId = (user == null) ? null : user.Id },
            };

            return View(moduleDetailModel);
        }

        [Authorize(Roles = "Admin")]
        // GET: Modules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleId,Name,Description,ModuleOrder")] Module @module)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@module);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@module);
        }

        // GET: Modules/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Module == null)
            {
                return NotFound();
            }

            var @module = await _context.Module.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }
            return View(@module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ModuleId,Name,Description,ModuleOrder")] Module @module)
        {
            if (id != @module.ModuleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.ModuleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@module);
        }

        // GET: Modules/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Module == null)
            {
                return NotFound();
            }

            var @module = await _context.Module
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Module == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Module'  is null.");
            }
            var @module = await _context.Module.FindAsync(id);
            if (@module != null)
            {
                _context.Module.Remove(@module);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Modules/CreateDiscussion
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Registered")]
        public async Task<IActionResult> CreateDiscussion([Bind("Module,DiscussionForm")] ModuleDetailModel model)
        {
            ModelState.Remove("Module.Name");
            ModelState.Remove("Module.Description");
            ModelState.Remove("Module.ModuleOrder");

            for (int i = 0, n = model.Module.Lessons.Count; i < n; i++)
            {
                ModelState.Remove($"Module.Lessons[{i}].Module");
            }

            var module = model.Module;
            if (!ModelState.IsValid || module == null)
            {
                if (model.Module.Lessons == null) model.Module.Lessons = new List<Lesson>();
                if (model.Module.Discussions == null) model.Module.Discussions = new List<Discussion>();
                return View("Details", model);
            }

            // Create new discussion
            var discussion = new Discussion
            {
                Comment = model.DiscussionForm.Comment,
                User = await _userManager.FindByIdAsync(model.DiscussionForm.UserId),
                ModuleId = model.Module.ModuleId,
            };
            _context.Add(discussion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = module.ModuleId });
        }

        // GET: Modules/DeleteDiscussion/5
        [Authorize(Roles = "Admin,Registered")]
        public async Task<IActionResult> DeleteDiscussion(int id)
        {
            if (id == null || _context.Discussion == null) 
                return NotFound();

            var discussion = await _context.Discussion
                .Include(d => d.Module)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DiscussionId == id);
            if (discussion == null) return NotFound();

            // Check for unauthorized user
            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && user.Id != discussion.UserId)
            {
                return NotFound();
            }

            return View("../Discussion/Delete", discussion);
        }

        // POST: Modules/DeleteDiscussion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Registered")]
        public async Task<IActionResult> DeleteDiscussion(int? id)
        {
            if (id == null) return NotFound();

            var discussion = await _context.Discussion
                .Include(d => d.Module)
                .FirstOrDefaultAsync(d => d.DiscussionId == id);
            if (discussion == null) return NotFound();

            // Check for unauthorized user
            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && user.Id != discussion.UserId)
                return NotFound();

            var moduleId = discussion.Module.ModuleId;
            _context.Discussion.Remove(discussion);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = moduleId });
        }

        private bool ModuleExists(int id)
        {
          return (_context.Module?.Any(e => e.ModuleId == id)).GetValueOrDefault();
        }
    }
}
