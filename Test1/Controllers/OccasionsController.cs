using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Test1.Data;
using Test1.Models;
using Test1.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Test1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OccasionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OccasionsController(ApplicationDbContext context)
        {
            _context = context;
        }

     

        // GET: Occasions
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            int itemsPerPage = pageSize;
            int currentPage = Math.Max(1, page); // Ensure the page number is at least 1

            var totalItems = await _context.FlowerItems.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            var paginatedData = _context.Occasion
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;

            return View(paginatedData);
        }

        // GET: Occasions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Occasion == null)
            {
                return NotFound();
            }

            var occasion = await _context.Occasion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (occasion == null)
            {
                return NotFound();
            }

            return View(occasion);
        }

        // GET: Occasions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Occasions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OccasionImageView model)
        {
            if (model.Occasion != null && model.ImageFile != null && model.ImageFile.Length > 0)
            {

                
                using (var memoryStream = new MemoryStream())
                {
                    model.ImageFile.CopyTo(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    model.Occasion.Image = imageBytes;
                }

                


                _context.Add(model.Occasion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Occasions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Occasion == null)
            {
                return NotFound();
            }

            var occasion = await _context.Occasion.FindAsync(id);
            if (occasion == null)
            {
                return NotFound();
            }
            return View(occasion);
        }

        // POST: Occasions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image")] Occasion occasion)
        {
            if (id != occasion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(occasion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OccasionExists(occasion.Id))
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
            return View(occasion);
        }

        // GET: Occasions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Occasion == null)
            {
                return NotFound();
            }

            var occasion = await _context.Occasion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (occasion == null)
            {
                return NotFound();
            }

            return View(occasion);
        }

        // POST: Occasions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Occasion == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Occasion'  is null.");
            }
            var occasion = await _context.Occasion.FindAsync(id);
            if (occasion != null)
            {
                _context.Occasion.Remove(occasion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OccasionExists(int id)
        {
          return (_context.Occasion?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
