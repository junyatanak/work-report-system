using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DailyWorkReport.Data;
using DailyWorkReport.Models;

namespace DailyWorkReport.Controllers
{
    public class WorkClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkClasses
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkClasses.ToListAsync());
        }

        // GET: WorkClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workClass = await _context.WorkClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workClass == null)
            {
                return NotFound();
            }

            return View(workClass);
        }

        // GET: WorkClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] WorkClass workClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workClass);
        }

        // GET: WorkClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workClass = await _context.WorkClasses.FindAsync(id);
            if (workClass == null)
            {
                return NotFound();
            }
            return View(workClass);
        }

        // POST: WorkClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] WorkClass workClass)
        {
            if (id != workClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkClassExists(workClass.Id))
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
            return View(workClass);
        }

        // GET: WorkClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workClass = await _context.WorkClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workClass == null)
            {
                return NotFound();
            }

            return View(workClass);
        }

        // POST: WorkClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workClass = await _context.WorkClasses.FindAsync(id);
            if (workClass != null)
            {
                _context.WorkClasses.Remove(workClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkClassExists(int id)
        {
            return _context.WorkClasses.Any(e => e.Id == id);
        }
    }
}
