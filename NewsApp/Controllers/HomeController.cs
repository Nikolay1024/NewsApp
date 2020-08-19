using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;
using NewsApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewsApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: News
        public async Task<IActionResult> Index()
        {
            return View(await _context.News.ToListAsync());
        }

        // GET: News/Details/Id
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var article = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null) return NotFound();

            return View(article);
        }

        // GET: News/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Photo,Title,Review,Text")] Article article,
            IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                article.Id = Guid.NewGuid().ToString();
                article.Photo = await UploadFile(uploadedFile);
                article.Date = DateTime.Now;

                _context.News.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: News/Edit/Id
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var article = await _context.News.FindAsync(id);
            if (article == null) return NotFound();

            return View(article);
        }

        // POST: News/Edit/Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Photo,Title,Review,Text")] Article article,
            IFormFile uploadedFile)
        {
            if (id != article.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    article.Photo = await UploadFile(uploadedFile);
                    article.Date = DateTime.Now;

                    _context.News.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
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
            return View(article);
        }

        // GET: News/Delete/Id
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var article = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null) return NotFound();

            return View(article);
        }

        // POST: News/Delete/Id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var article = await _context.News.FindAsync(id);
            _context.News.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(string id)
        {
            return _context.News.Any(e => e.Id == id);
        }

        private async Task<string> UploadFile(IFormFile uploadedFile)
        {
            string path = "/files/" + uploadedFile.FileName;
            using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }
            return path;
        }
    }
}