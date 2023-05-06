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
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.ContentModel;
using System.Security.Claims;
using System.Security.Policy;

namespace DigitalMarketing2.Controllers
{
    public class QuizzesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<User> _userManager;

        public QuizzesController(ApplicationDbContext context, UserManager<User> userMgr)
        {
            _context = context;
            _userManager = userMgr;
        }

        // GET: Quizzes
        [Authorize(Roles = "Admin,Registered")]
        public async Task<IActionResult> Index(int? LessonId)
        {
            if (LessonId == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var quizQuestions = await _context.QuizQuestion
                .Where(q => q.Lesson.LessonId == LessonId)
                .Include(q => q.Lesson)
                .Include(q => q.QuestionOptions)
                .Include(q => q.Answer)
                .OrderBy(q => q.QuizOrder)
                .ToListAsync();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return View("ViewQuiz", quizQuestions);
            }
            else // Registered User
            {
                var quizAttemptModels = new List<QuizAttemptModel>();

                foreach (QuizQuestion quizQuestion in quizQuestions)
                {
                    var quizAttemptModel = new QuizAttemptModel
                    {
                        QuizQuestionId = quizQuestion.QuizQuestionId,
                        Question = quizQuestion.Question,
                        Lesson = quizQuestion.Lesson,
                        QuestionOptions = quizQuestion.QuestionOptions,
                        Answer = quizQuestion.Answer,
                    };
                    quizAttemptModels.Add(quizAttemptModel);
                }
                return View("AttemptQuiz", quizAttemptModels);
            }
        }

        // GET: Quizzes/Details/5
        [Authorize(Roles = "Admin,Registered")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.QuizQuestion == null)
                return NotFound();

            var quizQuestion = await _context.QuizQuestion
                .Include(quizQuestion => quizQuestion.Lesson)
                .Include(quizQuestion => quizQuestion.QuestionOptions)
                .Include(quizQuestion => quizQuestion.Answer)
                .FirstOrDefaultAsync(quizQuestion => quizQuestion.QuizQuestionId == id);
            if (quizQuestion == null) return NotFound();

            return View(quizQuestion);
        }

        // GET: Quizzes/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int? LessonId)
        {
            if (LessonId == null) return NotFound();

            //ViewBag.LessonSelectList = _context.Lesson.ToList();
            var quizFormModel = new QuizFormModel
            {
                LessonId = (int) LessonId
            };
            return View(quizFormModel);
        }

        // POST: Quizzes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("LessonId,Question,QuizOrder,QuestionOptions,AnswerId")] QuizFormModel quizFormModel)
        {
            // Display error if user removed all question options
            if (quizFormModel.QuestionOptions == null)
            {
                ModelState.AddModelError(string.Empty, "Please enter at least two options and select one as the correct answer.");
                return View(quizFormModel);
            }

            // Temporarily remove QuestionOptions[{i}].QuizQuestion's validations (Will manually add later on)
            for (int i = 0; i < quizFormModel.QuestionOptions.Count; i++)
                ModelState.Remove($"QuestionOptions[{i}].QuizQuestion");

            if (!ModelState.IsValid) return View(quizFormModel);
            
            // Check if there are at least two question options
            if (quizFormModel.QuestionOptions.Count < 2 || quizFormModel.AnswerId == null)
            {
                ModelState.AddModelError(string.Empty, "Please enter at least two options and select one as the correct answer.");
                return View(quizFormModel);
            }

            var lesson = await _context.Lesson.FindAsync(quizFormModel.LessonId);
            if (lesson == null) return NotFound();

            // Create new QuizQuestion
            var quizQuestion = new QuizQuestion
            {
                Lesson = lesson,
                Question = quizFormModel.Question,
                QuizOrder = quizFormModel.QuizOrder,
            };

            _context.Add(quizQuestion);
            await _context.SaveChangesAsync();

            // Create new quiz options and set answer for the question
            var index = 0;
            foreach (var option in quizFormModel.QuestionOptions)
            {
                var quizOption = new QuestionOption
                {
                    QuizQuestionId = quizQuestion.QuizQuestionId,
                    Option = option.Option
                };

                _context.Add(quizOption);
                quizQuestion.QuestionOptions.Add(quizOption);

                if (index == quizFormModel.AnswerId)
                    quizQuestion.Answer = quizOption;

                await _context.SaveChangesAsync();
                index++;
            }

            return RedirectToAction(nameof(Index), new { LessonId = lesson.LessonId });
        }

        private Task PopulateSelectListsAsync(QuizFormModel quizFormModel)
        {
            throw new NotImplementedException();
        }

        // GET: Quizzes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            //ViewBag.LessonSelectList = _context.Lesson.ToList();

            if (id == null || _context.QuizQuestion == null)
                return NotFound();

            var quizQuestion = await _context.QuizQuestion
                .Include(quizQuestion => quizQuestion.Lesson)
                .Include(quizQuestion => quizQuestion.QuestionOptions)
                .Include(quizQuestion => quizQuestion.Answer)
                .FirstOrDefaultAsync(quizQuestion => quizQuestion.QuizQuestionId == id);

            if (quizQuestion == null)
                return NotFound();

            var quizFormModel = new QuizFormModel
            {
                QuizQuestionId = quizQuestion.QuizQuestionId,
                LessonId = quizQuestion.Lesson.LessonId,
                Question = quizQuestion.Question,
                QuizOrder = quizQuestion.QuizOrder,
                QuestionOptions = quizQuestion.QuestionOptions.ToList(),
                Answer = quizQuestion.Answer,
            };

            return View(quizFormModel);
        }

        // POST: Quizzes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("QuizQuestionId,LessonId,Question,QuizOrder,QuestionOptions,AnswerId")] QuizFormModel quizFormModel)
        {
            if (id != quizFormModel.QuizQuestionId) return NotFound();

            var quizQuestion = await _context.QuizQuestion.Include(ques => ques.Answer)
                                .FirstOrDefaultAsync(ques => ques.QuizQuestionId == id);
            if (quizQuestion == null) return NotFound();

            // Display error if user removed all question options
            if (quizFormModel.QuestionOptions == null)
            {
                //quizFormModel.QuestionOptions = quizQuestion.QuestionOptions.ToList()
                ModelState.AddModelError(string.Empty, "Please enter at least two options and select one as the correct answer.");
                return View(quizFormModel);
            }

            for (int i = 0; i < quizFormModel.QuestionOptions.Count; i++)
                ModelState.Remove($"QuestionOptions[{i}].QuizQuestion");

            // Temporarily remove QuestionOptions[{i}].QuizQuestion's validations (Will manually add later on)
            if (!ModelState.IsValid)
                return View(quizFormModel);

            // Check if there are at least two question options
            if (quizFormModel.QuestionOptions.Count < 2 || quizFormModel.AnswerId == null)
            {
                ModelState.AddModelError(string.Empty, "Please enter at least two options and select one as the correct answer.");
                return View(quizFormModel);
            }

            var lesson = await _context.Lesson.FindAsync(quizFormModel.LessonId);
            if (lesson == null) return NotFound();

            // Validations passed, proceed with update procedures
            quizQuestion.Question = quizFormModel.Question;
            quizQuestion.QuizOrder = quizFormModel.QuizOrder;
            quizQuestion.Lesson = lesson;
            quizQuestion.AnswerId = null;

            // Remove all question options and answer first
            if (quizQuestion.Answer != null)
            {
                _context.QuestionOption.Remove(quizQuestion.Answer);
                await _context.SaveChangesAsync();
            }
            var qOptions = _context.QuestionOption.Where(qo => qo.QuizQuestionId == id);
            _context.QuestionOption.RemoveRange(qOptions);
            await _context.SaveChangesAsync();
            quizQuestion.QuestionOptions = new HashSet<QuestionOption>();

            // Update New Question Options
            // Create new quiz options and set answer for the question
            var index = 0;
            foreach (var option in quizFormModel.QuestionOptions)
            {
                var quizOption = new QuestionOption
                {
                    QuizQuestionId = quizQuestion.QuizQuestionId,
                    Option = option.Option
                };

                _context.Add(quizOption);
                quizQuestion.QuestionOptions.Add(quizOption);

                if (index == quizFormModel.AnswerId)
                    quizQuestion.Answer = quizOption;

                await _context.SaveChangesAsync();
                index++;
            }

            try
            {
                _context.Update(quizQuestion);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuizQuestionExists(quizQuestion.QuizQuestionId))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index), new { LessonId = lesson.LessonId });
        }

        // GET: Quizzes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.QuizQuestion == null)
            {
                return NotFound();
            }

            var quizQuestion = await _context.QuizQuestion
                .Include(q => q.Answer)
                .FirstOrDefaultAsync(m => m.QuizQuestionId == id);
            if (quizQuestion == null)
            {
                return NotFound();
            }

            return View(quizQuestion);
        }

        // POST: Quizzes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.QuizQuestion == null)
            {
                return Problem("Entity set 'ApplicationDbContext.QuizQuestion'  is null.");
            }
            var quizQuestion = await _context.QuizQuestion.FindAsync(id);
            if (quizQuestion != null)
            {
                _context.QuizQuestion.Remove(quizQuestion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Quizzes/SubmitQuiz
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Registered")]
        public async Task<IActionResult> SubmitQuiz(int LessonId, IEnumerable<QuizQuestion> quizQuestions)
        {
            // Retrieve the current user
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(userId);

            // Loop through each quiz question and create a student score instance for it
            foreach (var question in quizQuestions)
            {
                // Retrieve the selected answer from the form
                int selectedAnswerId = int.Parse(Request.Form[question.QuizQuestionId.ToString()]);

                // Determine if the selected answer is correct
                bool isCorrect = (selectedAnswerId == question.AnswerId);

                // Create a new student score instance
                var studentScore = new StudentScore
                {
                    User = user,
                    QuizQuestion = question,
                    Answer = await _context.QuestionOption.FindAsync(selectedAnswerId),
                    Status = (isCorrect ? ScoreStatus.correct : ScoreStatus.incorrect)
                };

                // Save the student score to the database
                _context.Add(studentScore);
                await _context.SaveChangesAsync();
            }

            // Redirect the user to the student score index page for this lesson
            return RedirectToAction("Results", "StudentScore", new { user, LessonId });
        }
        
    private bool QuizQuestionExists(int id)
    {
        return (_context.QuizQuestion?.Any(e => e.QuizQuestionId == id)).GetValueOrDefault();
    }
    }
}
