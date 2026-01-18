using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers
{
    [Authorize]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET: Stats
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound();
            }

            var fourWeeksAgo = DateTime.Now.AddDays(-28);

            // Get statistics using a direct join with TrainingSessions to filter by date
            var statistics = await _context.TrainingEntries
                .Include(te => te.ExerciseType)
                .Join(
                    _context.TrainingSessions.Where(ts => ts.UserId == userId && ts.StartedAt >= fourWeeksAgo),
                    te => te.TrainingSessionId,
                    ts => ts.Id,
                    (te, ts) => te
                )
                .Where(te => te.UserId == userId)
                .GroupBy(te => te.ExerciseType)
                .Select(g => new ExerciseStatistics
                {
                    ExerciseTypeName = g.Key != null ? g.Key.Name : "Nieznany",
                    TimesPerformed = g.Count(),
                    TotalRepetitions = g.Sum(te => te.Sets * te.Repetitions),
                    AverageWeight = g.Average(te => te.Weight),
                    MaxWeight = g.Max(te => te.Weight)
                })
                .OrderBy(s => s.ExerciseTypeName)
                .ToListAsync();

            return View(statistics);
        }
    }
}

