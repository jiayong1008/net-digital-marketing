using DigitalMarketing2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalMarketing2.Controllers
{
    public class UserController : Controller
    {
        private UserManager<User> userManager;
        private IPasswordHasher<User> passwordHasher;

        
        public UserController(UserManager<User> usrMgr, IPasswordHasher<User> passwordHash)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        [Authorize(Roles = "Admin,Registered")]
        // GET: User/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();

            //var @module = await _context.Module
            //    .Include(mod => mod.Lessons)
            //    .FirstOrDefaultAsync(mod => mod.ModuleId == id);

            //if (@module == null) return NotFound();
            //return View(@module);

            // Current logged in user
            var currentUser = await userManager.GetUserAsync(User);
            User user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // If user is not admin and the user is attempting to see other user's details
            if ((!await userManager.IsInRoleAsync(currentUser, "Admin")) && (currentUser.Id != user.Id))
                return NotFound();

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public ViewResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RegisterUserModel user)
        {
            if (ModelState.IsValid)
            {
                User appUser = new User
                {
                    UserName = user.Name,
                    Email = user.Email,
                    Gender = user.Gender,
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        [Authorize(Roles = "Admin,Registered")]
        public async Task<IActionResult> Update(string id)
        {
            if (id == null) return NotFound();
            User user = await userManager.FindByIdAsync(id);
            User currentUser = await userManager.GetUserAsync(User);

            // If user is not admin and attempting to update other's profile
            var isAdmin = await userManager.IsInRoleAsync(currentUser, "Admin");
            if ( !isAdmin && (user.Id != currentUser.Id) )
                return NotFound();

            if (user == null)
            {
                if (isAdmin) return RedirectToAction("Index");
                else return NotFound();
            }

            UpdateUserModel userModel = new UpdateUserModel
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Gender = user.Gender,
            };

            return View(userModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Registered")]
        public async Task<IActionResult> Update([Bind("Id,Name,Email,Gender,Password,PasswordConfirmaiton")] UpdateUserModel userModel)
        {
            User user = await userManager.FindByIdAsync(userModel.Id);

            if (user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                return View(userModel);
            }

            // Authorization check - If user doesnt have access to update somebody else's profile
            User currentUser = await userManager.GetUserAsync(User);
            if ((!await userManager.IsInRoleAsync(currentUser, "Admin")) && (currentUser.Id != user.Id))
                return NotFound();

            if (!ModelState.IsValid) 
                return View(userModel);

            user.UserName = userModel.Name;
            user.Email = userModel.Email;
            user.Gender = userModel.Gender;

            if (!string.IsNullOrEmpty(userModel.Password)) 
                user.PasswordHash = passwordHasher.HashPassword(user, userModel.Password);

            IdentityResult result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
                if (User.IsInRole("Admin")) return RedirectToAction("Index");
                else return RedirectToAction("Details", new { id = currentUser.Id });
            else
                Errors(result);

            return View(userModel);
        }

        [Authorize(Roles = "Admin")]
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            User user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", userManager.Users);
        }
    }
}
