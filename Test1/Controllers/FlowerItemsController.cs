using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Test1.Data;
using Test1.Models;
using Test1.Models.Enums;
using Test1.Models.Relations;
using Test1.Models.ViewModels;

namespace Test1.Controllers
{
    public class FlowerItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlowerItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FlowerItems
        public async Task<IActionResult> Index(Color[] SelectedColors
            , Size[] SelectedSizes,
            int[] Occasions, int[] FlowerTypes,
            int page = 1, int pageSize = 9)
        {

            int itemsPerPage = pageSize;
            int currentPage = Math.Max(1, page); // Ensure the page number is at least 1

            var totalItems = await _context.FlowerItems.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            List<FlowerItem> flowerItems = _context.FlowerItems.ToList();

            if (SelectedColors.Length != 0)
            {

                List<int> list = _context.ColorItems.Where(co =>
                SelectedColors.Contains(co.Color))
                    .Select(c => c.FlowerItemId).ToList();
                flowerItems = flowerItems.Where(fl => list.Contains(fl.Id)).ToList();
            }

            if (SelectedSizes.Length != 0)
            {
                List<int> list = _context.SizeItems.Where(co =>
                SelectedSizes.Contains(co.Size))
                    .Select(c => c.FlowerItemId).ToList();
                flowerItems = flowerItems.Where(fl => list.Contains(fl.Id)).ToList();
            }

            if (FlowerTypes.Length != 0)
            {

                flowerItems = flowerItems.Where(fl =>
                {
                    foreach (FlowerItemFlowerType f in fl.FlowerTypes)
                    {
                        if (FlowerTypes.Contains(f.FlowerType.Id))
                            return true;
                    }
                    return false;
                }).ToList();

            }

            if (Occasions.Length != 0)
            {

                flowerItems = flowerItems.Where(fl =>
                {
                    foreach (FlowerItemOccasion f in fl.Occasions)
                    {
                        if (Occasions.Contains(f.Occasion.Id))
                            return true;
                    }
                    return false;
                }).ToList();
            }

            var paginatedData = flowerItems
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage);




            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            IEnumerable<Color>? colors = Enum.GetValues(typeof(Color)) as IEnumerable<Color>;
            ViewBag.Colors = colors;
            IEnumerable<Size>? sizes = Enum.GetValues(typeof(Size)) as IEnumerable<Size>;
            ViewBag.Sizes = sizes;
            ViewBag.SelectedColors = SelectedColors.ToList();
            ViewBag.SelectedSizes = SelectedSizes.ToList();
            ViewBag.SelectedFlowerTypes = FlowerTypes.ToList();
            ViewBag.SelectedOccasions = Occasions.ToList();
            List<Occasion> occasions = _context.Occasion.ToList();

            ViewBag.Occasions = occasions;

            List<FlowerType> flowerTypes = _context.FlowerType.ToList();

            ViewBag.FlowerTypes = flowerTypes;


            if (User.IsInRole("Admin"))
            {
                return View(paginatedData);
            }
            else
            {
                return View("UserIndex", paginatedData);
            }
        }

        // GET: FlowerItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FlowerItems == null)
            {
                return NotFound();
            }

            var flowerItem = await _context.FlowerItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flowerItem == null)
            {
                return NotFound();
            }

            List<SizeItem> sizeItems = _context.SizeItems.Where(si => si.FlowerItemId == id).ToList();

            ViewBag.SizeItems = sizeItems;

            if (User.IsInRole("Admin"))
            {
                return View(flowerItem);
            }
            else
            {
                FlowerItemView flowerItemView = new FlowerItemView();
                flowerItemView.flowerItem = flowerItem;
                return View("UserDetails", flowerItemView);
            }
        }

        // GET: FlowerItems/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int id)
        {
            FlowerItemImageView model = new FlowerItemImageView();
            IEnumerable<Size>? sizes = Enum.GetValues(typeof(Size)) as IEnumerable<Size>;
            model.Sizes = sizes;
            IEnumerable<Color>? colors = Enum.GetValues(typeof(Color)) as IEnumerable<Color>;
            model.Colors = colors;
            List<FlowerType> flowerTypes = _context.FlowerType.ToList();
            ViewBag.FlowerTypes = flowerTypes;
            List<Occasion> occasion = _context.Occasion.ToList();
            ViewBag.Occasions = occasion;

            model.SelectedSizeItems = new List<SizeItem>();
            if (id != null)
            {
                FlowerItem? flowerItem = _context.FlowerItems.Find(id);
                model.FlowerItem = flowerItem;

                List<SizeItem> sizeItems = _context.SizeItems.Where(si => si.FlowerItemId == id)
                    .ToList();
                List<FlowerType> selectedFlowerTypes = _context.FlowerItemFlowerTypes.Where(fi => fi.FlowerItemId == id).
                    Select(si => si.FlowerType).ToList();
                List<Occasion> selectedOccasions = _context.FlowerItemOccasions
                    .Where(o => o.FlowerItemId == id).Select(o => o.Occasion).ToList();
                model.SelectedSizeItems = sizeItems;
                model.SelectedFlowerTypes = selectedFlowerTypes;
                model.SelectedOccasions = selectedOccasions;
                List<Color> colorItems = _context.ColorItems.Where(si => si.FlowerItemId == id)
                    .Select(co => co.Color).ToList();
                model.selectedColors = colorItems;
            }


            return View(model);
        }

        // POST: FlowerItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, FlowerItemImageView model,
            string CountsJson,
            int[] SelectedFlowerTypes,
            int[] SelectedOccasions)
        {
            if (model.FlowerItem != null)
            {

                bool changed = false;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.ImageFile.CopyTo(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();

                        model.FlowerItem.Image = imageBytes;
                        changed = true;
                    }
                }

                FlowerItem item;

                if (id != 0)
                {
                    item = _context.FlowerItems.Find(id);

                    if (changed)
                    {
                        item.Image = model.FlowerItem.Image;

                    }
                    model.FlowerItem.Id = -1;
                    item.Id = id;
                    item.Description = model.FlowerItem.Description;
                    item.Name = model.FlowerItem.Name;

                }
                else
                {
                    item = model.FlowerItem;
                }






                //Colors

                if (model.SelectedColors != null)
                {
                    IEnumerable<Color> selectedColorEnums = model.SelectedColors
                .Select(color => (Color)Enum.Parse(typeof(Color), color));

                    // Now you can process and save the selectedColorEnums collection
                    foreach (var colorEnum in selectedColorEnums)
                    {
                        bool hasItem = item.ColorItems.Any(m => m.Color.Equals(colorEnum));
                        if (hasItem)
                        {

                        }
                        else
                        {
                            ColorItem colorItem = new ColorItem();
                            colorItem.FlowerItem = item;
                            colorItem.Color = colorEnum;

                            item.ColorItems.Add(colorItem);
                        }
                    }

                    IEnumerable<Color>? colors = Enum.GetValues(typeof(Color)) as IEnumerable<Color>;
                    List<Color> colorsList = colors.Where(c => !selectedColorEnums.Contains(c)).ToList();
                    foreach (var colorEnum in colorsList)
                    {
                        ColorItem cItem = item.ColorItems.FirstOrDefault(m => m.Color.Equals(colorEnum));
                        if (cItem != null)
                        {
                            item.ColorItems.Remove(cItem);
                        }

                    }
                }

                //Size Item

                if (!string.IsNullOrEmpty(model.PricesJson))
                {
                    var pricesList = JsonConvert.DeserializeObject<List<double>>(model.PricesJson);
                    var countList = JsonConvert.DeserializeObject<List<int>>(CountsJson);
                    IEnumerable<Size> selectedSizeEnums = model.SelectedSizes
                .Select(size => (Size)Enum.Parse(typeof(Size), size));


                    for (int i = 0; i < pricesList.Count; i++)
                    {
                        Size size = (Test1.Models.Enums.Size)
                        Enum.Parse(typeof(Test1.Models.Enums.Size), model.SelectedSizes[i]);
                        SizeItem sItem = item.SizeItems.FirstOrDefault(s => s.Size.Equals(size));
                        if (sItem != null)
                        {
                            sItem.Price = pricesList[i];
                            sItem.Count = countList[i];
                        }
                        else
                        {
                            SizeItem sizeItem = new SizeItem();
                            sizeItem.Price = pricesList[i];

                            sizeItem.Count = countList[i];
                            sizeItem.Size = size;
                            sizeItem.FlowerItem = item;


                            item.SizeItems.Add(sizeItem);
                        }
                    }


                    IEnumerable<Size>? sizesL = Enum.GetValues(typeof(Size)) as IEnumerable<Size>;
                    List<Size> sizeList = sizesL.Where(c => !selectedSizeEnums.Contains(c)).ToList();
                    foreach (var sizeEnum in sizeList)
                    {
                        SizeItem cItem = item.SizeItems.FirstOrDefault(m => m.Size.Equals(sizeEnum));
                        if (cItem != null)
                        {
                            item.SizeItems.Remove(cItem);
                        }

                    }

                }

                //Flower Types

                if (SelectedFlowerTypes != null)
                {

                    foreach (int typeId in SelectedFlowerTypes)
                    {
                        bool hasItem = item.FlowerTypes.Any(ft => ft.FlowerType.Id == typeId);

                        if (hasItem)
                        {

                        }
                        else
                        {
                            FlowerType? flowerType = _context.FlowerType.Find(typeId);
                            FlowerItemFlowerType fl = new FlowerItemFlowerType();


                            fl.FlowerType = flowerType;


                            fl.FlowerItem = item;

                            item.FlowerTypes.Add(fl);
                        }

                    }


                    List<FlowerType> types = _context.FlowerType.Where(ft => !SelectedFlowerTypes.Contains(ft.Id)).ToList();
                    foreach (FlowerType type in types)
                    {
                        FlowerItemFlowerType cItem = item.FlowerTypes.FirstOrDefault(m => m.FlowerType.Id == type.Id);
                        if (cItem != null)
                        {
                            item.FlowerTypes.Remove(cItem);
                        }

                    }
                }

                //Occasions

                if (SelectedOccasions != null)
                {

                    foreach (int typeId in SelectedOccasions)
                    {
                        bool hasItem = item.Occasions.Any(ft => ft.Occasion.Id == typeId);

                        if (hasItem)
                        {

                        }
                        else
                        {
                            Occasion? occasion = _context.Occasion.Find(typeId);
                            FlowerItemOccasion fl = new FlowerItemOccasion();

                            fl.Occasion = occasion;
                            fl.FlowerItem = item;

                            item.Occasions.Add(fl);
                        }

                    }


                    List<Occasion> types = _context.Occasion.Where(ft => !SelectedOccasions.Contains(ft.Id)).ToList();
                    foreach (Occasion type in types)
                    {
                        FlowerItemOccasion cItem = item.Occasions.FirstOrDefault(m => m.Occasion.Id == type.Id);
                        if (cItem != null)
                        {
                            item.Occasions.Remove(cItem);
                        }

                    }


                }

                if (id != 0)
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Add(item);
                    await _context.SaveChangesAsync();
                }



                return RedirectToAction(nameof(Index));
            }
            IEnumerable<Size>? sizes = Enum.GetValues(typeof(Size)) as IEnumerable<Size>;
            model.Sizes = sizes;
            return View(model);
        }

        // GET: FlowerItems/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FlowerItems == null)
            {
                return NotFound();
            }

            var flowerItem = await _context.FlowerItems.FindAsync(id);
            if (flowerItem == null)
            {
                return NotFound();
            }
            return View("Create", flowerItem);
        }

        public async Task<IActionResult> GetShoppingCart()
        {
            string userEmail = User.FindFirstValue(ClaimTypes.Email);

            List<Models.Order> orders = _context.Orders.Where(o => o.UserEmail == userEmail
            && o.IsProcessed == false).ToList();

            Models.Order order = orders[0];

            List<SizeItem?> list = _context.FlowerItemOrder
                .Where(fio => fio.OrderId == order.Id)
                .Select(fio => fio.FlowerItem)
                .ToList();
            double Price = _context.FlowerItemOrder.Where(fio => fio.OrderId == order.Id).Select(fio => fio.Count * fio.FlowerItem.Price).Sum();

            ViewBag.Price = Price;
            ViewBag.OrderId = order.Id;

            Dictionary<SizeItem, FlowerItem> dict = new Dictionary<SizeItem, FlowerItem>();

            foreach (SizeItem item in list)
            {
                FlowerItem flowerItem = _context.FlowerItems
                     .FirstOrDefault(f => f.Id == item.FlowerItemId);

                dict.Add(item, flowerItem);
            }

            List<FlowerItemOrder> flowerItemOrders = _context.FlowerItemOrder.Where(f => f.OrderId == order.Id)
                .ToList();

            Dictionary<SizeItem, double> dict1 = new Dictionary<SizeItem, double>();

            foreach (SizeItem item in list)
            {
                FlowerItemOrder o = flowerItemOrders.FirstOrDefault(f => f.FlowerItemId == item.Id);

                dict1.Add(item, o.Count);
            }

            ViewBag.map = dict;
            ViewBag.map1 = dict1;

            List<Size> enumerable = Enum.GetValues(typeof(Size)).Cast<Size>().ToList();

            ViewBag.sizes = enumerable;

            return PartialView("_OverlayPartial", list);
        }

        //OverlayPartial


        // POST: FlowerItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Image,Size")] FlowerItem flowerItem)
        {
            if (id != flowerItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flowerItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlowerItemExists(flowerItem.Id))
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
            return View(flowerItem);
        }

        // GET: FlowerItems/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FlowerItems == null)
            {
                return NotFound();
            }

            var flowerItem = await _context.FlowerItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flowerItem == null)
            {
                return NotFound();
            }

            return View(flowerItem);
        }

        // POST: FlowerItems/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FlowerItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FlowerItems'  is null.");
            }
            var flowerItem = await _context.FlowerItems.FindAsync(id);
            if (flowerItem != null)
            {
                _context.FlowerItems.Remove(flowerItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlowerItemExists(int id)
        {
            return (_context.FlowerItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
