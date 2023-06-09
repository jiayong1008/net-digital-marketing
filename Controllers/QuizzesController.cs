﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalMarketing2.Data;
using DigitalMarketing2.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
        public async Task<IActionResult> Index(
            int? LessonId, List<QuizQuestionViewModel>? quizQuestionViewModels)
        {
            if (LessonId == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var lesson = await _context.Lesson.FindAsync(LessonId);
            List<QuizQuestion> quizQuestions = null;
            
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                quizQuestions = await _context.QuizQuestion
                    .Where(q => q.Lesson.LessonId == LessonId)
                    .Include(q => q.Lesson)
                    .Include(q => q.QuestionOptions)
                    .Include(q => q.Answer)
                    .OrderBy(q => q.QuizOrder)
                    .ToListAsync();

                var viewQuizIndexModel = new ViewQuizIndexModel
                {
                    LessonId = (int)LessonId,
                    LessonName = lesson.Name,
                    QuizQuestions = quizQuestions,
                };

                return View("ViewQuiz", viewQuizIndexModel);
            }
            else // Registered User
            {
                quizQuestions = await _context.QuizQuestion
                    .Where(q => q.Lesson.LessonId == LessonId)
                    .Include(q => q.QuestionOptions)
                    .OrderBy(q => q.QuizOrder)
                    .ToListAsync();

                if (quizQuestions.Count == 0) // No quiz for that question
                    return RedirectToAction("Details", "Lessons", new { id = LessonId });

                quizQuestionViewModels = new List<QuizQuestionViewModel>();
                foreach (QuizQuestion quizQuestion in quizQuestions)
                {
                    var quizQuestionViewModel = new QuizQuestionViewModel
                    {
                        QuizQuestionId = quizQuestion.QuizQuestionId,
                        Question = quizQuestion.Question,
                        LessonId = (int) LessonId,
                        Options = quizQuestion.QuestionOptions.Select(
                            qo => new QuestionOptionViewModel
                            {
                                QuestionOptionId = qo.QuestionOptionId,
                                Option = qo.Option
                            }).ToList(),
                        AnswerId = (int) quizQuestion.AnswerId,
                    };
                    quizQuestionViewModels.Add(quizQuestionViewModel);
                }

                var quizIndexModel = new AttemptQuizIndexModel
                {
                    LessonId = (int)LessonId,
                    LessonName = lesson.Name,
                    QuizQuestionViewModels = quizQuestionViewModels
                };
                return View("AttemptQuiz", quizIndexModel);
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
        public async Task<IActionResult> Create(
            [Bind("LessonId,Question,QuizOrder,QuestionOptions,AnswerId")] QuizFormModel quizFormModel)
        {
            // Display error if user removed all question options
            if (quizFormModel.QuestionOptions == null)
            {
                ModelState.AddModelError(string.Empty, 
                    "Please enter at least two options and select one as the correct answer.");
                return View(quizFormModel);
            }

            // Temporarily remove QuestionOptions[{i}].QuizQuestion's validations (Will manually add later on)
            for (int i = 0; i < quizFormModel.QuestionOptions.Count; i++)
                ModelState.Remove($"QuestionOptions[{i}].QuizQuestion");

            if (!ModelState.IsValid) return View(quizFormModel);
            
            // Check if there are at least two question options
            if (quizFormModel.QuestionOptions.Count < 2 || quizFormModel.AnswerId == null)
            {
                ModelState.AddModelError(string.Empty, 
                    "Please enter at least two options and select one as the correct answer.");
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
        public async Task<IActionResult> Edit(int id, 
            [Bind("QuizQuestionId,LessonId,Question,QuizOrder,QuestionOptions,AnswerId")] QuizFormModel quizFormModel)
        {
            if (id != quizFormModel.QuizQuestionId) return NotFound();

            var quizQuestion = await _context.QuizQuestion.Include(ques => ques.Answer)
                                .FirstOrDefaultAsync(ques => ques.QuizQuestionId == id);
            if (quizQuestion == null) return NotFound();

            // Display error if user removed all question options
            if (quizFormModel.QuestionOptions == null)
            {
                //quizFormModel.QuestionOptions = quizQuestion.QuestionOptions.ToList()
                ModelState.AddModelError(string.Empty, 
                    "Please enter at least two options and select one as the correct answer.");
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
                return NotFound();

            var quizQuestion = await _context.QuizQuestion
                .Include(q => q.Answer)
                .Include(q => q.Lesson)
                .FirstOrDefaultAsync(m => m.QuizQuestionId == id);

            if (quizQuestion == null)
                return NotFound();

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

            var quizQuestion = await _context.QuizQuestion
                .Include(q => q.Lesson)
                .FirstOrDefaultAsync(q => q.QuizQuestionId == id);
            if (quizQuestion == null) return NotFound();

            var lessonId = quizQuestion.Lesson.LessonId;

            // Remove quiz question
            _context.QuizQuestion.Remove(quizQuestion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { LessonId = lessonId });
        }

        // POST: Quizzes/SubmitQuiz
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Registered")]
        public async Task<IActionResult> SubmitQuiz(
            [Bind("LessonId,LessonName,QuizQuestionViewModels")] 
            AttemptQuizIndexModel model)
        {
            //var lessonId = quizAttemptModels.First().LessonId;
            var lessonId = model.LessonId;
            var numQuesModels = model.QuizQuestionViewModels.Count;
            ModelState.Remove("LessonName");

            if (!ModelState.IsValid)
            {
                var quizModel = new AttemptQuizIndexModel
                {
                    LessonId = lessonId,
                    LessonName = model.LessonName,
                    QuizQuestionViewModels = model.QuizQuestionViewModels
                };

                // Populate Options for each quizQuestionViewModel
                foreach (var quizQuestion in quizModel.QuizQuestionViewModels)
                {
                    quizQuestion.Options = await _context.QuestionOption
                        .Where(qo => qo.QuizQuestionId == quizQuestion.QuizQuestionId)
                        .Select(qo => new QuestionOptionViewModel {
                            QuestionOptionId = qo.QuestionOptionId,
                            Option = qo.Option
                        })
                        .ToListAsync();
                }
                return View("AttemptQuiz", quizModel);
                // return RedirectToAction(nameof(Index), new { LessonId = lessonId, quizQuestionViewModels = model.QuizQuestionViewModels });
            }

            // Retrieve the current user
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(userId);

            // Loop through each quiz question and create a student score instance for it
            foreach (var question in model.QuizQuestionViewModels)
            {
                // Retrieve the selected answer from the form
                int attemptedAnswerId = question.AttemptedAnswerId;

                // Determine if the selected answer is correct
                bool isCorrect = (attemptedAnswerId == question.AnswerId);

                // Attempt to find existing student score for current student and question
                var studentScore = await _context.StudentScore
                    .Where(score => score.User == user && score.QuizQuestionId == question.QuizQuestionId)
                    .SingleOrDefaultAsync();

                if (studentScore == null)
                {
                    // Create a new student score instance
                    studentScore = new StudentScore
                    {
                        User = user,
                        QuizQuestion = await _context.QuizQuestion.FindAsync(question.QuizQuestionId),
                        Answer = await _context.QuestionOption.FindAsync(attemptedAnswerId),
                        Status = (isCorrect ? ScoreStatus.correct : ScoreStatus.incorrect)
                    };
                    _context.Add(studentScore);
                }
                else
                {
                    // Update the existing student score instance
                    studentScore.Answer = await _context.QuestionOption.FindAsync(question.AttemptedAnswerId);
                    studentScore.Status = (isCorrect ? ScoreStatus.correct : ScoreStatus.incorrect);
                    _context.Update(studentScore);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            // Redirect the user to the student score index page for this lesson
            //return RedirectToAction("Results", "StudentScore", new { user, LessonId });
            return RedirectToAction("Details", "User", new { id = userId });
        }
        
    private bool QuizQuestionExists(int id)
    {
        return (_context.QuizQuestion?.Any(e => e.QuizQuestionId == id)).GetValueOrDefault();
    }
    }
}
