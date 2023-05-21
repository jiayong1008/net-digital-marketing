using DigitalMarketing2.Data;
using DigitalMarketing2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace DigitalMarketing2.Controllers
{
    public class UserController : Controller
    {
        private UserManager<User> userManager;
        private IPasswordHasher<User> passwordHasher;
        private readonly ApplicationDbContext _context;
        
        public UserController(UserManager<User> usrMgr, IPasswordHasher<User> passwordHash, ApplicationDbContext context)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
            _context = context;
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
            User user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // If user is not admin and the user is attempting to see other user's details
            var currentUser = await userManager.GetUserAsync(User);
            if ((!await userManager.IsInRoleAsync(currentUser, "Admin")) && (currentUser.Id != user.Id))
                return NotFound();

            // Main task - extract all user quiz score details and summarize by module
            var studScores = await _context.StudentScore
                .Include(s => s.User)
                .Where(s => s.User.Id == id)
                .Include(s => s.QuizQuestion)
                    .ThenInclude(q => q.Lesson)
                        .ThenInclude(l => l.Module)
                .ToListAsync();

            var profileModuleSummaries = new List<ProfileModuleSummary>();
            var profileQuizScores = new List<ProfileQuizScore>();

            foreach (var studScore in studScores)
            {
                var lessonId = studScore.QuizQuestion.Lesson.LessonId;

                // Check if quizQuestionId exists in profileQuizScores
                var profileQuizScore = profileQuizScores.FirstOrDefault(p => p.LessonId == lessonId);

                if (profileQuizScore != null)
                {
                    // Update profileQuizScore
                    profileQuizScore.TotalQuestions += 1;

                    if (studScore.Status == ScoreStatus.correct)
                        profileQuizScore.CorrectQuestions += 1;

                    profileQuizScore.QuizScore = (((double)profileQuizScore.CorrectQuestions / profileQuizScore.TotalQuestions) * 100);
                    //profileQuizScore.TotalQuestions = profileQuizScore.TotalQuestions;
                }
                else
                {
                    // Create new profileQuizScore
                    profileQuizScore = new ProfileQuizScore
                    {
                        LessonId = studScore.QuizQuestion.Lesson.LessonId,
                        LessonName = studScore.QuizQuestion.Lesson.Name,
                        //QuizQuestionId = quizQuestionId,
                        CorrectQuestions = (studScore.Status == ScoreStatus.correct) ? 1 : 0,
                        TotalQuestions = 1,
                        QuizScore = (studScore.Status == ScoreStatus.correct) ? 100.00 : 0.00, // In percentage
                    };
                    profileQuizScores.Add(profileQuizScore);
                }

                // Module Summary
                var module = studScore.QuizQuestion.Lesson.Module;
                var moduleSummary = profileModuleSummaries.FirstOrDefault(mod => mod.ModuleId == module.ModuleId);

                if (moduleSummary != null)
                {
                    // Update module summary
                    moduleSummary.ProfileQuizScores = profileQuizScores;
                }
                else
                {
                    // Create new module summary
                    var profileModuleSummary = new ProfileModuleSummary
                    {
                        ModuleId = module.ModuleId,
                        ModuleName = module.Name,
                        ProfileQuizScores = profileQuizScores,
                    };
                    profileModuleSummaries.Add(profileModuleSummary);
                }
            }

            var profileUserModel = new ProfileUserModel
            {
                Id = currentUser.Id,
                Name = currentUser.UserName,
                Email = currentUser.Email,
                Gender = currentUser.Gender,
                profileModuleSummaries = profileModuleSummaries,
            };
            //var @module = await _context.Module
            //    .Include(mod => mod.Lessons)
            //    .FirstOrDefaultAsync(mod => mod.ModuleId == id);

            //if (@module == null) return NotFound();
            //return View(@module);

            return View(profileUserModel);
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
