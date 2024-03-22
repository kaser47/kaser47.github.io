using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecentlyAddedShows.Service.Data;
using RecentlyAddedShows.Service.Data.Entities;
using Serilog.Core;

namespace RecentlyAddedShows.Web.Controllers
{
    public class FavouritesController : Controller
    {
        private readonly ILogger<FavouritesController> _logger;
        private readonly Context _context;
        private readonly Service.Classes.RecentlyAddedShows _recentlyAddedShows;

        public FavouritesController(Context context, ILogger<FavouritesController> logger)
        {
            _logger = logger;
            _context = context;
            _recentlyAddedShows = new Service.Classes.RecentlyAddedShows();
        }

        // GET: Favourites
        public async Task<IActionResult> Index()
        {
            _logger.LogWarning($"Favourite/Index was called");
            _recentlyAddedShows.RefreshFavourites();
            return View(await _context.Favourites.ToListAsync());
        }

        // GET: Favourites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            _logger.LogWarning($"Favourite/Details was called");
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourites
                .FirstOrDefaultAsync(m => m.id == id);
            if (favourite == null)
            {
                return NotFound();
            }
            return View(favourite);
        }

        // GET: Favourites/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Favourites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Title")] Favourite favourite)
        {
            _logger.LogWarning($"Favourite/Create was called");
            favourite.Created = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                _context.Add(favourite);
                await _context.SaveChangesAsync();
                _logger.LogWarning($"{this.GetType().Name}/Create Result: {favourite.Title}");
                return RedirectToAction(nameof(Index));
            }

            return View(favourite);
        }

        // GET: Favourites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            _logger.LogWarning($"Favourite/EditGet was called");
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourites.FindAsync(id);
            if (favourite == null)
            {
                return NotFound();
            }
            _logger.LogWarning($"{this.GetType().Name}/{MethodBase.GetCurrentMethod().Name} Result: {favourite.Title}");
            return View(favourite);
        }

        // POST: Favourites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Title,Created")] Favourite favourite)
        {
            _logger.LogWarning($"Favourite/Edit was called");
            if (id != favourite.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favourite);
                    _logger.LogWarning($"{this.GetType().Name}/Edit Result: {favourite.Title}");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavouriteExists(favourite.id))
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
            return View(favourite);
        }

        // GET: Favourites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogWarning($"Favourite/Delete Confirmation was called");
            if (id == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourites
                .FirstOrDefaultAsync(m => m.id == id);
            if (favourite == null)
            {
                return NotFound();
            }
            return View(favourite);
        }

        // POST: Favourites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogWarning($"Favourite/Delete was called");
            var favourite = await _context.Favourites.FindAsync(id);
            _context.Favourites.Remove(favourite);
            _logger.LogWarning($"{this.GetType().Name}/Delete Result: {favourite.Title}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavouriteExists(int id)
        {
            return _context.Favourites.Any(e => e.id == id);
        }
    }
}
