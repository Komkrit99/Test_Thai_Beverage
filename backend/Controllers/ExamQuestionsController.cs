using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamQuestionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExamQuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamQuestionDto>>> GetExamQuestions()
        {
            var questions = await _context.ExamQuestions
                .OrderBy(q => q.QuestionNumber)
                .ToListAsync();

            var questionDtos = questions.Select(q => new ExamQuestionDto
            {
                Id = q.Id,
                QuestionNumber = q.QuestionNumber,
                QuestionText = q.QuestionText,
                Option1 = q.Option1,
                Option2 = q.Option2,
                Option3 = q.Option3,
                Option4 = q.Option4,
                CorrectAnswer = q.CorrectAnswer,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt
            }).ToList();

            return Ok(questionDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExamQuestionDto>> GetExamQuestion(int id)
        {
            var question = await _context.ExamQuestions.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            var questionDto = new ExamQuestionDto
            {
                Id = question.Id,
                QuestionNumber = question.QuestionNumber,
                QuestionText = question.QuestionText,
                Option1 = question.Option1,
                Option2 = question.Option2,
                Option3 = question.Option3,
                Option4 = question.Option4,
                CorrectAnswer = question.CorrectAnswer,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt
            };

            return Ok(questionDto);
        }

        [HttpPost]
        public async Task<ActionResult<ExamQuestionDto>> PostExamQuestion(CreateExamQuestionDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingQuestion = await _context.ExamQuestions
                .FirstOrDefaultAsync(q => q.QuestionText.ToLower().Trim() == createDto.QuestionText.ToLower().Trim());
            
            if (existingQuestion != null)
            {
                return BadRequest(new { error = "A question with this text already exists in the database." });
            }

            var options = new[] {
                createDto.Option1?.Trim().ToLower(),
                createDto.Option2?.Trim().ToLower(),
                createDto.Option3?.Trim().ToLower(),
                createDto.Option4?.Trim().ToLower()
            };

            var duplicateOptions = options
                .Where(option => !string.IsNullOrEmpty(option))
                .GroupBy(option => option)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicateOptions.Any())
            {
                return BadRequest(new { error = "All answer options must be different. Duplicate options found." });
            }

            var maxQuestionNumber = await _context.ExamQuestions
                .MaxAsync(q => (int?)q.QuestionNumber) ?? 0;

            var question = new ExamQuestion
            {
                QuestionNumber = maxQuestionNumber + 1,
                QuestionText = createDto.QuestionText,
                Option1 = createDto.Option1,
                Option2 = createDto.Option2,
                Option3 = createDto.Option3,
                Option4 = createDto.Option4,
                CorrectAnswer = createDto.CorrectAnswer,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ExamQuestions.Add(question);
            await _context.SaveChangesAsync();

            var questionDto = new ExamQuestionDto
            {
                Id = question.Id,
                QuestionNumber = question.QuestionNumber,
                QuestionText = question.QuestionText,
                Option1 = question.Option1,
                Option2 = question.Option2,
                Option3 = question.Option3,
                Option4 = question.Option4,
                CorrectAnswer = question.CorrectAnswer,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt
            };

            return CreatedAtAction(nameof(GetExamQuestion), new { id = question.Id }, questionDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamQuestion(int id, UpdateExamQuestionDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = await _context.ExamQuestions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            var existingQuestion = await _context.ExamQuestions
                .FirstOrDefaultAsync(q => q.Id != id && q.QuestionText.ToLower().Trim() == updateDto.QuestionText.ToLower().Trim());
            
            if (existingQuestion != null)
            {
                return BadRequest(new { error = "A question with this text already exists in the database." });
            }

            var options = new[] {
                updateDto.Option1?.Trim().ToLower(),
                updateDto.Option2?.Trim().ToLower(),
                updateDto.Option3?.Trim().ToLower(),
                updateDto.Option4?.Trim().ToLower()
            };

            var duplicateOptions = options
                .Where(option => !string.IsNullOrEmpty(option))
                .GroupBy(option => option)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicateOptions.Any())
            {
                return BadRequest(new { error = "All answer options must be different. Duplicate options found." });
            }

            question.QuestionText = updateDto.QuestionText;
            question.Option1 = updateDto.Option1;
            question.Option2 = updateDto.Option2;
            question.Option3 = updateDto.Option3;
            question.Option4 = updateDto.Option4;
            question.CorrectAnswer = updateDto.CorrectAnswer;
            question.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamQuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamQuestion(int id)
        {
            var question = await _context.ExamQuestions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            var questionNumber = question.QuestionNumber;

            _context.ExamQuestions.Remove(question);

            var subsequentQuestions = await _context.ExamQuestions
                .Where(q => q.QuestionNumber > questionNumber)
                .OrderBy(q => q.QuestionNumber)
                .ToListAsync();

            foreach (var q in subsequentQuestions)
            {
                q.QuestionNumber--;
                q.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamQuestionExists(int id)
        {
            return _context.ExamQuestions.Any(e => e.Id == id);
        }
    }
}