using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecentlyAddedShows.Service.Classes;
using RecentlyAddedShows.Service.Data;
using RecentlyAddedShows.Service.Data.Entities;

namespace RecentlyAddedShows.Web.Controllers
{
    public class ExcludedFavouritesController : Controller
    {
        ILogger<ExcludedFavouritesController> _logger;
        private readonly Context _context;
        private readonly Service.Classes.RecentlyAddedShows _recentlyAddedShows;

        public ExcludedFavouritesController(Context context, ILogger<ExcludedFavouritesController> logger)
        {
            _logger = logger;
            _context = context;
            _recentlyAddedShows = new Service.Classes.RecentlyAddedShows();
        }

        // GET: ExcludedFavourites
        public async Task<IActionResult> Index()
        {
            _logger.LogWarning($"ExcludedFavourite/Index was called");
            _recentlyAddedShows.RefreshFavourites();
            return View(await _context.ExcludedFavourites.ToListAsync());
        }

        // GET: ExcludedFavourites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            _logger.LogWarning($"ExcludedFavourite/Details was called");
            if (id == null)
            {
                return NotFound();
            }

            var excludedFavourite = await _context.ExcludedFavourites
                .FirstOrDefaultAsync(m => m.id == id);
            if (excludedFavourite == null)
            {
                return NotFound();
            }

            return View(excludedFavourite);
        }

        // GET: ExcludedFavourites/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExcludedFavourites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Title,Created")] ExcludedFavourite excludedFavourite)
        {
            _logger.LogWarning($"ExcludedFavourite/Create was called");
            excludedFavourite.Created = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                _context.Add(excludedFavourite);
                await _context.SaveChangesAsync();
                _logger.LogWarning($"{this.GetType().Name}/Create Result: {excludedFavourite.Title}");
                return RedirectToAction(nameof(Index));
            }
            return View(excludedFavourite);
        }

        // GET: ExcludedFavourites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            _logger.LogWarning($"ExcludedFavourite/EditGet was called");
            if (id == null)
            {
                return NotFound();
            }

            var excludedFavourite = await _context.ExcludedFavourites.FindAsync(id);
            if (excludedFavourite == null)
            {
                return NotFound();
            }
            return View(excludedFavourite);
        }

        // POST: ExcludedFavourites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Title,Created")] ExcludedFavourite excludedFavourite)
        {
            _logger.LogWarning($"ExcludedFavourite/Edit was called");
            if (id != excludedFavourite.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(excludedFavourite);
                    await _context.SaveChangesAsync();
                    _logger.LogWarning($"{this.GetType().Name}/Edit Result: {excludedFavourite.Title}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExcludedFavouriteExists(excludedFavourite.id))
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
            return View(excludedFavourite);
        }

        // GET: ExcludedFavourites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogWarning($"ExcludedFavourite/DeleteConfirmation was called");
            if (id == null)
            {
                return NotFound();
            }

            var excludedFavourite = await _context.ExcludedFavourites
                .FirstOrDefaultAsync(m => m.id == id);
            if (excludedFavourite == null)
            {
                return NotFound();
            }

            return View(excludedFavourite);
        }

        // POST: ExcludedFavourites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogWarning($"ExcludedFavourite/Delete was called");
            var excludedFavourite = await _context.ExcludedFavourites.FindAsync(id);
            if (excludedFavourite != null)
            {
                _context.ExcludedFavourites.Remove(excludedFavourite);
                _logger.LogWarning($"{this.GetType().Name}/Delete Result: {excludedFavourite.Title}");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExcludedFavouriteExists(int id)
        {
            return _context.ExcludedFavourites.Any(e => e.id == id);
        }
    }
}
