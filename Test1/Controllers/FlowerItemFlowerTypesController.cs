using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Test1.Data;
using Test1.Models.Relations;

namespace Test1.Controllers
{
    public class FlowerItemFlowerTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlowerItemFlowerTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FlowerItemFlowerTypes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FlowerItemFlowerTypes.Include(f => f.FlowerItem).Include(f => f.FlowerType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FlowerItemFlowerTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FlowerItemFlowerTypes == null)
            {
                return NotFound();
            }

            var flowerItemFlowerType = await _context.FlowerItemFlowerTypes
                .Include(f => f.FlowerItem)
                .Include(f => f.FlowerType)
                .FirstOrDefaultAsync(m => m.FlowerItemId == id);
            if (flowerItemFlowerType == null)
            {
                return NotFound();
            }

            return View(flowerItemFlowerType);
        }

        // GET: FlowerItemFlowerTypes/Create
        public IActionResult Create()
        {
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name");
            ViewData["FlowerTypeId"] = new SelectList(_context.FlowerType, "Id", "Name");
            return View();
        }

        // POST: FlowerItemFlowerTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlowerItemId,FlowerTypeId")] FlowerItemFlowerType flowerItemFlowerType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flowerItemFlowerType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name", flowerItemFlowerType.FlowerItemId);
            ViewData["FlowerTypeId"] = new SelectList(_context.FlowerType, "Id", "Name", flowerItemFlowerType.FlowerTypeId);
            return View(flowerItemFlowerType);
        }

        // GET: FlowerItemFlowerTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FlowerItemFlowerTypes == null)
            {
                return NotFound();
            }

            var flowerItemFlowerType = await _context.FlowerItemFlowerTypes.FindAsync(id);
            if (flowerItemFlowerType == null)
            {
                return NotFound();
            }
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name", flowerItemFlowerType.FlowerItemId);
            ViewData["FlowerTypeId"] = new SelectList(_context.FlowerType, "Id", "Name", flowerItemFlowerType.FlowerTypeId);
            return View(flowerItemFlowerType);
        }

        // POST: FlowerItemFlowerTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FlowerItemId,FlowerTypeId")] FlowerItemFlowerType flowerItemFlowerType)
        {
            if (id != flowerItemFlowerType.FlowerItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flowerItemFlowerType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlowerItemFlowerTypeExists(flowerItemFlowerType.FlowerItemId))
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
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name", flowerItemFlowerType.FlowerItemId);
            ViewData["FlowerTypeId"] = new SelectList(_context.FlowerType, "Id", "Name", flowerItemFlowerType.FlowerTypeId);
            return View(flowerItemFlowerType);
        }

        // GET: FlowerItemFlowerTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FlowerItemFlowerTypes == null)
            {
                return NotFound();
            }

            var flowerItemFlowerType = await _context.FlowerItemFlowerTypes
                .Include(f => f.FlowerItem)
                .Include(f => f.FlowerType)
                .FirstOrDefaultAsync(m => m.FlowerItemId == id);
            if (flowerItemFlowerType == null)
            {
                return NotFound();
            }

            return View(flowerItemFlowerType);
        }

        // POST: FlowerItemFlowerTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FlowerItemFlowerTypes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FlowerItemFlowerTypes'  is null.");
            }
            var flowerItemFlowerType = await _context.FlowerItemFlowerTypes.FindAsync(id);
            if (flowerItemFlowerType != null)
            {
                _context.FlowerItemFlowerTypes.Remove(flowerItemFlowerType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlowerItemFlowerTypeExists(int id)
        {
          return (_context.FlowerItemFlowerTypes?.Any(e => e.FlowerItemId == id)).GetValueOrDefault();
        }
    }
}
