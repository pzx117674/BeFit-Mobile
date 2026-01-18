using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers
{
    [Authorize]
    public class TrainingEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET: TrainingEntries
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            var applicationDbContext = _context.TrainingEntries
                .Include(t => t.ExerciseType)
                .Include(t => t.TrainingSession)
                .Where(te => te.UserId == userId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TrainingEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            var trainingEntry = await _context.TrainingEntries
                .Include(t => t.ExerciseType)
                .Include(t => t.TrainingSession)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (trainingEntry == null)
            {
                return NotFound();
            }

            return View(trainingEntry);
        }

        // GET: TrainingEntries/Create
        public IActionResult Create()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name");
            ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions
                .Where(ts => ts.UserId == userId)
                .Select(ts => new { 
                    Id = ts.Id, 
                    Display = $"{ts.StartedAt:yyyy-MM-dd HH:mm} - {ts.EndedAt:yyyy-MM-dd HH:mm}" 
                }), "Id", "Display");
            return View(new TrainingEntryCreateDto());
        }

        // POST: TrainingEntries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingEntryCreateDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            // Verify that the selected training session belongs to the current user
            var selectedSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(ts => ts.Id == dto.TrainingSessionId && ts.UserId == userId);
            if (selectedSession == null)
            {
                ModelState.AddModelError("TrainingSessionId", "Wybrana sesja treningowa nie należy do Ciebie.");
            }

            if (ModelState.IsValid)
            {
                var trainingEntry = new TrainingEntry
                {
                    UserId = userId,
                    TrainingSessionId = dto.TrainingSessionId,
                    ExerciseTypeId = dto.ExerciseTypeId,
                    Weight = dto.Weight,
                    Sets = dto.Sets,
                    Repetitions = dto.Repetitions
                };

                _context.Add(trainingEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", dto.ExerciseTypeId);
            ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions
                .Where(ts => ts.UserId == userId)
                .Select(ts => new { 
                    Id = ts.Id, 
                    Display = $"{ts.StartedAt:yyyy-MM-dd HH:mm} - {ts.EndedAt:yyyy-MM-dd HH:mm}" 
                }), "Id", "Display", dto.TrainingSessionId);
            return View(dto);
        }

        // GET: TrainingEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            var trainingEntry = await _context.TrainingEntries
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (trainingEntry == null)
            {
                return NotFound();
            }
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", trainingEntry.ExerciseTypeId);
            ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions
                .Where(ts => ts.UserId == userId)
                .Select(ts => new { 
                    Id = ts.Id, 
                    Display = $"{ts.StartedAt:yyyy-MM-dd HH:mm} - {ts.EndedAt:yyyy-MM-dd HH:mm}" 
                }), "Id", "Display", trainingEntry.TrainingSessionId);
            return View(trainingEntry);
        }

        // POST: TrainingEntries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TrainingSessionId,ExerciseTypeId,Weight,Sets,Repetitions")] TrainingEntry trainingEntry)
        {
            if (id != trainingEntry.Id)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            // Verify that the entry belongs to the current user
            var existingEntry = await _context.TrainingEntries
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (existingEntry == null)
            {
                return NotFound();
            }

            // Verify that the selected training session belongs to the current user
            var selectedSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(ts => ts.Id == trainingEntry.TrainingSessionId && ts.UserId == userId);
            if (selectedSession == null)
            {
                ModelState.AddModelError("TrainingSessionId", "Wybrana sesja treningowa nie należy do Ciebie.");
                ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", trainingEntry.ExerciseTypeId);
                ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions
                    .Where(ts => ts.UserId == userId)
                    .Select(ts => new { 
                        Id = ts.Id, 
                        Display = $"{ts.StartedAt:yyyy-MM-dd HH:mm} - {ts.EndedAt:yyyy-MM-dd HH:mm}" 
                    }), "Id", "Display", trainingEntry.TrainingSessionId);
                return View(trainingEntry);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingEntry.TrainingSessionId = trainingEntry.TrainingSessionId;
                    existingEntry.ExerciseTypeId = trainingEntry.ExerciseTypeId;
                    existingEntry.Weight = trainingEntry.Weight;
                    existingEntry.Sets = trainingEntry.Sets;
                    existingEntry.Repetitions = trainingEntry.Repetitions;
                    _context.Update(existingEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingEntryExists(trainingEntry.Id))
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
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", trainingEntry.ExerciseTypeId);
            ViewData["TrainingSessionId"] = new SelectList(_context.TrainingSessions
                .Where(ts => ts.UserId == userId)
                .Select(ts => new { 
                    Id = ts.Id, 
                    Display = $"{ts.StartedAt:yyyy-MM-dd HH:mm} - {ts.EndedAt:yyyy-MM-dd HH:mm}" 
                }), "Id", "Display", trainingEntry.TrainingSessionId);
            return View(trainingEntry);
        }

        // GET: TrainingEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            var trainingEntry = await _context.TrainingEntries
                .Include(t => t.ExerciseType)
                .Include(t => t.TrainingSession)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (trainingEntry == null)
            {
                return NotFound();
            }

            return View(trainingEntry);
        }

        // POST: TrainingEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            var trainingEntry = await _context.TrainingEntries
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (trainingEntry != null)
            {
                _context.TrainingEntries.Remove(trainingEntry);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainingEntryExists(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return false;
            }
            return _context.TrainingEntries.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}

