using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers
{
    [Authorize]
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET: TrainingSessions
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            return View(await _context.TrainingSessions
                .Where(ts => ts.UserId == userId)
                .ToListAsync());
        }

        // GET: TrainingSessions/Details/5
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

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (trainingSession == null)
            {
                return NotFound();
            }

            return View(trainingSession);
        }

        // GET: TrainingSessions/Create
        public IActionResult Create()
        {
            var now = DateTime.Now;
            // Round down to nearest minute for better UX
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            
            return View(new TrainingSessionCreateDto
            {
                StartedAt = now,
                EndedAt = now.AddHours(1) // Default to 1 hour later
            });
        }

        // POST: TrainingSessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingSessionCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return NotFound();
                }

                var trainingSession = new TrainingSession
                {
                    UserId = userId,
                    StartedAt = dto.StartedAt,
                    EndedAt = dto.EndedAt
                };

                _context.Add(trainingSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: TrainingSessions/Edit/5
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

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (trainingSession == null)
            {
                return NotFound();
            }
            return View(trainingSession);
        }

        // POST: TrainingSessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartedAt,EndedAt")] TrainingSession trainingSession)
        {
            if (id != trainingSession.Id)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            // Verify that the session belongs to the current user
            var existingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (existingSession == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingSession.StartedAt = trainingSession.StartedAt;
                    existingSession.EndedAt = trainingSession.EndedAt;
                    _context.Update(existingSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingSessionExists(trainingSession.Id))
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
            return View(trainingSession);
        }

        // GET: TrainingSessions/Delete/5
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

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (trainingSession == null)
            {
                return NotFound();
            }

            return View(trainingSession);
        }

        // POST: TrainingSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            var trainingSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (trainingSession != null)
            {
                _context.TrainingSessions.Remove(trainingSession);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainingSessionExists(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return false;
            }
            return _context.TrainingSessions.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}

