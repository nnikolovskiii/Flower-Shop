using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Test1.Data;
using Test1.Models.Relations;

namespace Test1.Controllers
{
    public class ColorItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ColorItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ColorItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ColorItems.Include(c => c.FlowerItem);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ColorItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ColorItems == null)
            {
                return NotFound();
            }

            var colorItem = await _context.ColorItems
                .Include(c => c.FlowerItem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (colorItem == null)
            {
                return NotFound();
            }

            return View(colorItem);
        }

        // GET: ColorItems/Create
        public IActionResult Create()
        {
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name");
            return View();
        }

        // POST: ColorItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Color,FlowerItemId")] ColorItem colorItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(colorItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name", colorItem.FlowerItemId);
            return View(colorItem);
        }

        // GET: ColorItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ColorItems == null)
            {
                return NotFound();
            }

            var colorItem = await _context.ColorItems.FindAsync(id);
            if (colorItem == null)
            {
                return NotFound();
            }
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name", colorItem.FlowerItemId);
            return View(colorItem);
        }

        // POST: ColorItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Color,FlowerItemId")] ColorItem colorItem)
        {
            if (id != colorItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(colorItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColorItemExists(colorItem.Id))
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
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name", colorItem.FlowerItemId);
            return View(colorItem);
        }

        // GET: ColorItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ColorItems == null)
            {
                return NotFound();
            }

            var colorItem = await _context.ColorItems
                .Include(c => c.FlowerItem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (colorItem == null)
            {
                return NotFound();
            }

            return View(colorItem);
        }

        // POST: ColorItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ColorItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ColorItems'  is null.");
            }
            var colorItem = await _context.ColorItems.FindAsync(id);
            if (colorItem != null)
            {
                _context.ColorItems.Remove(colorItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ColorItemExists(int id)
        {
            return (_context.ColorItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
