using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Test1.Data;
using Test1.Models;
using Test1.Models.Relations;

namespace Test1.Controllers
{

    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ShowDetails(int id)
        {
            Order? order = _context.Orders.Find(id);
            return View(order);
        }


        // GET: Orders
        public async Task<IActionResult> Index()
        {

            if (User.IsInRole("Admin"))
            {

                return View(_context.Orders.Where(o => o.IsProcessed == true).ToList());

            }
            else
            {
                string userEmail = User.FindFirstValue(ClaimTypes.Email);



                return View(_context.Orders.Where(o => o.UserEmail == userEmail && o.IsProcessed == true).ToList());
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details()
        {
            /*if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
*/
            return PartialView("_Details");
        }

        [HttpPost]
        public async Task<IActionResult> Details(Order order, int id)
        {
            string userEmail = User.FindFirstValue(ClaimTypes.Email);

            List<Models.Order> orders = _context.Orders.Where(o => o.UserEmail == userEmail
            && o.IsProcessed == false).ToList();

            Models.Order entry = orders[0];

            List<FlowerItemOrder> fio = _context.FlowerItemOrder.Where(f => f.OrderId == entry.Id).ToList();
            foreach (FlowerItemOrder elem in fio)
            {
                int count = elem.Count;
                if (elem.FlowerItem.Count - count < 0)
                {
                    entry.FlowerItems.Remove(elem);
                }
                else
                {
                    elem.FlowerItem.Count -= count;
                }
                _context.Update(elem);
                await _context.SaveChangesAsync();
            }
            if (id != 1)
            {

                entry.IsDelivery = true;
                entry.Street = order.Street;
                entry.City = order.City;
                entry.Message = order.Message;
                entry.IsGift = order.IsGift;
            }
            else
            {
                entry.IsDelivery = false;
                entry.Street = "Our Store";
                entry.City = "Skopje";
                entry.IsGift = false;

            }

            entry.IsProcessed = true;
            _context.Update(entry);



            Models.Order tmp = new Models.Order();
            tmp.UserEmail = userEmail;
            tmp.IsProcessed = false;

            _context.Add(tmp);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "FlowerItems");
        }


        [HttpPost]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            string userEmail = User.FindFirstValue(ClaimTypes.Email);

            List<Models.Order> orders = _context.Orders.Where(o => o.UserEmail == userEmail
            && o.IsProcessed == false).ToList();

            Models.Order order = orders[0];

            ;

            order.FlowerItems = order.FlowerItems.Where(f => f.FlowerItem.Id != itemId)
                .ToList();

            _context.Update(order);
            await _context.SaveChangesAsync();


            return RedirectToAction("ShowDetails", "Orders", new { id = order.Id });

        }

        // GET: Orders/Create

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Street,City,IsGift,IsDelivery,Message")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Show(int? id)
        {
            Order order = _context.Orders.Find(id);
            return RedirectToAction("Index", "SizeItems", order.FlowerItems.Select(f => f.FlowerItem).ToList());
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Street,City,IsGift,IsDelivery,Message")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
