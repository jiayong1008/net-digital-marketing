using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DigitalMarketing2.Data;
using DigitalMarketing2.Models;

namespace DigitalMarketing2.Controllers
{
    public class QuizzesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizzesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Quizzes
        public async Task<IActionResult> Index()
        {
            var quizQuestions = await _context.QuizQuestion
                .Include(q => q.Lesson)
                //.Include(q => q.QuestionOptions)
                .Include(q => q.Answer)
                .ToListAsync();

            return View(quizQuestions);
        }

        // GET: Quizzes/Details/5
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

            //var quizFormModel = new QuizFormModel
            //{
            //    QuizQuestionId = quizQuestion.QuizQuestionId,
            //    LessonId = quizQuestion.Lesson.LessonId,
            //    Question = quizQuestion.Question,
            //    QuizOrder = quizQuestion.QuizOrder,
            //    QuestionOptions = quizQuestion.QuestionOptions.ToList(),
            //    Answer = quizQuestion.Answer,
            //};

            return View(quizQuestion);
        }

        // GET: Quizzes/Create
        public IActionResult Create()
        {
            ViewBag.LessonSelectList = _context.Lesson.ToList();
            return View();
        }

        // POST: Quizzes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LessonId,Question,QuizOrder,QuestionOptions,AnswerId")] QuizFormModel quizFormModel)
        {
            ViewBag.LessonSelectList = _context.Lesson.ToList();

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

            return RedirectToAction(nameof(Index));
        }

        private Task PopulateSelectListsAsync(QuizFormModel quizFormModel)
        {
            throw new NotImplementedException();
        }

        // GET: Quizzes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.LessonSelectList = _context.Lesson.ToList();

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
        public async Task<IActionResult> Edit(int id, [Bind("QuizQuestionId,LessonId,Question,QuizOrder,QuestionOptions,AnswerId")] QuizFormModel quizFormModel)
        {
            if (id != quizFormModel.QuizQuestionId) return NotFound();

            var quizQuestion = await _context.QuizQuestion.FindAsync(id);
            if (quizQuestion == null) return NotFound();

            ViewBag.LessonSelectList = _context.Lesson.ToList();

            for (int i = 0; i < quizFormModel.QuestionOptions.Count; i++)
                ModelState.Remove($"QuestionOptions[{i}].QuizQuestion");
            
            if (!ModelState.IsValid)
                return View(quizFormModel);

            var lesson = await _context.Lesson.FindAsync(quizFormModel.LessonId);
            if (lesson == null) return NotFound();

            // Validations passed, proceed with update procedures
            quizQuestion.Question = quizFormModel.Question;
            quizQuestion.QuizOrder = quizFormModel.QuizOrder;
            quizQuestion.Lesson = lesson;
            quizQuestion.QuestionOptions = new HashSet<QuestionOption>(); // Empty it first (will be updated below)

            // Update Question Options
            foreach (var option in quizFormModel.QuestionOptions)
            {
                var qOption = _context.QuestionOption.Where(quesOption => quesOption.Option.Equals(option.Option)).First();

                // If updated question option does not exist yet
                if (qOption == null)
                {
                    // Create new question option
                    qOption = new QuestionOption
                    {
                        Option = option.Option,
                        QuizQuestionId = quizFormModel.QuizQuestionId,
                    };
                    _context.Add(qOption);
                    await _context.SaveChangesAsync();
                }
                
                // Add the option to the list of question options and update answer
                quizQuestion.QuestionOptions.Add(qOption);
                if (quizFormModel.AnswerId == qOption.QuestionOptionId)
                    quizQuestion.Answer = qOption;
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
            return RedirectToAction(nameof(Index));
        }

        // GET: Quizzes/Delete/5
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

        private bool QuizQuestionExists(int id)
        {
          return (_context.QuizQuestion?.Any(e => e.QuizQuestionId == id)).GetValueOrDefault();
        }
    }
}
