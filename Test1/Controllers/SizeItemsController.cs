using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Test1.Data;
using Test1.Models;
using Test1.Models.Relations;

namespace Test1.Controllers
{
    public class SizeItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SizeItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SizeItems
        [HttpPost]
        public async Task<IActionResult> Show(int id)
        {
            Order order = _context.Orders.Find(id);
            return View("Index", order.FlowerItems.Select(f => f.FlowerItem).ToList());
        }

        // GET: SizeItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SizeItems == null)
            {
                return NotFound();
            }

            var sizeItem = await _context.SizeItems
                .Include(s => s.FlowerItem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sizeItem == null)
            {
                return NotFound();
            }

            return View(sizeItem);
        }

        // GET: SizeItems/Create
        public IActionResult Create()
        {
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name");

            return View();
        }

        // POST: SizeItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317 598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int SelectedSizeItemId, int Count)
        {
            SizeItem? sizeItem = await _context.SizeItems.FindAsync(SelectedSizeItemId);

            string userEmail = User.FindFirstValue(ClaimTypes.Email);

            List<Models.Order> orders = _context.Orders.Where(o => o.UserEmail == userEmail
            && o.IsProcessed == false).ToList();

            Models.Order order = orders[0];

            FlowerItemOrder flowerItemOrder = new FlowerItemOrder();
            flowerItemOrder.FlowerItemId = SelectedSizeItemId;
            flowerItemOrder.OrderId = order.Id;
            flowerItemOrder.Count = Count;

            FlowerItemOrder? tmp = _context.FlowerItemOrder
                .FirstOrDefault(a => a.OrderId == order.Id
                && a.FlowerItemId == SelectedSizeItemId);
            if (tmp != null)
            {
                tmp.Count = Count + tmp.Count;
                _context.Update(tmp);
                await _context.SaveChangesAsync();
            }
            else
            {
                flowerItemOrder.Count = Count;
                _context.Add(flowerItemOrder);
                await _context.SaveChangesAsync();
            }



            return RedirectToAction("GetShoppingCart", "FlowerItems");
        }

        // GET: SizeItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SizeItems == null)
            {
                return NotFound();
            }

            var sizeItem = await _context.SizeItems.FindAsync(id);
            if (sizeItem == null)
            {
                return NotFound();
            }
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name", sizeItem.FlowerItemId);
            return View(sizeItem);
        }

        // POST: SizeItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SizeItem sizeItem)
        {
            if (id != sizeItem.Id)
            {
                return NotFound();
            }

            if (sizeItem != null)
            {
                try
                {
                    _context.Update(sizeItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SizeItemExists(sizeItem.Id))
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
            ViewData["FlowerItemId"] = new SelectList(_context.FlowerItems, "Id", "Name", sizeItem.FlowerItemId);
            return View(sizeItem);
        }

        // GET: SizeItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SizeItems == null)
            {
                return NotFound();
            }

            var sizeItem = await _context.SizeItems
                .Include(s => s.FlowerItem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sizeItem == null)
            {
                return NotFound();
            }

            return View(sizeItem);
        }

        // POST: SizeItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SizeItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SizeItems'  is null.");
            }
            var sizeItem = await _context.SizeItems.FindAsync(id);
            if (sizeItem != null)
            {
                _context.SizeItems.Remove(sizeItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SizeItemExists(int id)
        {
            return (_context.SizeItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
