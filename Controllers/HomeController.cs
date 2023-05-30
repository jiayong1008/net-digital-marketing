using DigitalMarketing2.Data;
using DigitalMarketing2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DigitalMarketing2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<User> userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userMgr, ApplicationDbContext context)
        {
            _logger = logger;
            userManager = userMgr;
            _context = context;
        }


        //[Authorize]
        //public IActionResult Index()
        //{
        //    return View();
        //}

        // [Authorize]
        public async Task<IActionResult> Index()
        {
            User user = await userManager.GetUserAsync(HttpContext.User);
            if (user == null || !await userManager.IsInRoleAsync(user, "Admin"))
                return View();

            // ========= Render admin index view (AKA Dashboard) ===========
            // Get total active student count (users who attempted at least 1 quiz)
            int activeStudentCount = await _context.StudentScore
                .Include(s => s.User)
                .Select(s => s.User.Id)
                .Distinct()
                .CountAsync();

            // Total completed quizzes
            int completedQuizCount = await _context.StudentScore
                .Include(s => s.QuizQuestion)
                .Select(s => s.QuizQuestionId)
                .Distinct()
                .CountAsync();

            // Total discussion card (to know engagement in each module)
            int totalDiscussionCount = await _context.Discussion.CountAsync();

            // Calculate Module popularity data (More discussion + more quiz completion = popular module)
            var moduleData = await _context.Module
                .Include(m => m.Discussions)
                .Include(m => m.Lessons)
                    .ThenInclude(l => l.QuizQuestions)
                        .ThenInclude(q => q.StudentScores)
                .ToListAsync();

            var modulePopularity = moduleData.Select(m => new ModulePopularity
            {
                ModuleId = m.ModuleId,
                ModuleName = m.Name,
                DiscussionCount = m.Discussions.Count,
                Lessons = m.Lessons.Select(l => new LessonData
                {
                    QuizQuestions = l.QuizQuestions.Select(q => new QuizQuestionData
                    {
                        StudentScoresCount = q.StudentScores.Count
                    }).ToList()
                }).ToList()
            }).ToList();

            foreach (var module in modulePopularity)
            {
                int quizCompletionCount = 0;
                foreach (var lesson in module.Lessons)
                {
                    foreach (var quizQuestion in lesson.QuizQuestions)
                    {
                        quizCompletionCount += quizQuestion.StudentScoresCount;
                    }
                }
                module.QuizCompletionCount = quizCompletionCount;
            }

            // Convert module popularity data to pie chart format
            var modulePopularityChart = modulePopularity.Select(m => new {
                x = m.ModuleName,
                y = m.DiscussionCount + m.QuizCompletionCount
            }).ToList();

            // Store the extracted data in ViewBag
            ViewBag.ActiveStudentCount = activeStudentCount;
            ViewBag.CompletedQuizCount = completedQuizCount;
            ViewBag.TotalDiscussionCount = totalDiscussionCount;
            ViewBag.ModulePopularityChart = modulePopularityChart;

            return View("AdminIndex");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}