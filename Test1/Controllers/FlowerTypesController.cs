using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test1.Data;
using Test1.Models;
using Test1.Models.ViewModels;

namespace Test1.Controllers
{
    public class FlowerTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlowerTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetFlowerTypeSuggestions(string term)
        {
            var suggestions = _context.FlowerType.Where(ft => ft.Name.Contains(term))
                                                 .Select(ft => ft.Name)
                                                 .ToList();
            return Json(suggestions);
        }

        // GET: FlowerTypes
        public async Task<IActionResult> Index()
        {
            return _context.FlowerType != null ?
                        View(await _context.FlowerType.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.FlowerType'  is null.");
        }

        // GET: FlowerTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FlowerType == null)
            {
                return NotFound();
            }

            var flowerType = await _context.FlowerType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flowerType == null)
            {
                return NotFound();
            }

            return View(flowerType);
        }

        // GET: FlowerTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FlowerTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlowerType, ImageFile")] FlowerTypeImageView model)
        {
            if (model.FlowerType != null && model.ImageFile != null && model.ImageFile.Length > 0)
            {


                using (var memoryStream = new MemoryStream())
                {
                    model.ImageFile.CopyTo(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    model.FlowerType.Image = imageBytes;
                }




                _context.Add(model.FlowerType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: FlowerTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FlowerType == null)
            {
                return NotFound();
            }

            var flowerType = await _context.FlowerType.FindAsync(id);
            FlowerType fl = new FlowerType();
            fl.Name = flowerType.Name;
            fl.Description = flowerType.Description;
            fl.Image = flowerType.Image;
            if (flowerType == null)
            {
                return NotFound();
            }
            return View(fl);
        }

        // POST: FlowerTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlowerType flowerType)
        {
            if (id != flowerType.Id)
            {
                return NotFound();
            }

            if (flowerType != null)
            {
                try
                {
                    FlowerType? tmp = _context.FlowerType.Find(id);
                    tmp.Description = flowerType.Description;
                    if (flowerType.Image != null)
                    {
                        tmp.Image = flowerType.Image;
                    }
                    tmp.Name = flowerType.Name;
                    _context.Update(tmp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlowerTypeExists(flowerType.Id))
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
            return View(flowerType);
        }

        // GET: FlowerTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FlowerType == null)
            {
                return NotFound();
            }

            var flowerType = await _context.FlowerType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flowerType == null)
            {
                return NotFound();
            }

            return View(flowerType);
        }

        // POST: FlowerTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FlowerType == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FlowerType'  is null.");
            }
            var flowerType = await _context.FlowerType.FindAsync(id);
            if (flowerType != null)
            {
                _context.FlowerType.Remove(flowerType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlowerTypeExists(int id)
        {
            return (_context.FlowerType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
