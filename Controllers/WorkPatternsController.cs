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
    public class WorkPatternsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkPatternsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkPatterns
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkPatterns.ToListAsync());
        }

        // GET: WorkPatterns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPattern = await _context.WorkPatterns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workPattern == null)
            {
                return NotFound();
            }

            return View(workPattern);
        }

        // GET: WorkPatterns/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkPatterns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] WorkPattern workPattern)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workPattern);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workPattern);
        }

        // GET: WorkPatterns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPattern = await _context.WorkPatterns.FindAsync(id);
            if (workPattern == null)
            {
                return NotFound();
            }
            return View(workPattern);
        }

        // POST: WorkPatterns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] WorkPattern workPattern)
        {
            if (id != workPattern.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workPattern);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkPatternExists(workPattern.Id))
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
            return View(workPattern);
        }

        // GET: WorkPatterns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPattern = await _context.WorkPatterns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workPattern == null)
            {
                return NotFound();
            }

            return View(workPattern);
        }

        // POST: WorkPatterns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workPattern = await _context.WorkPatterns.FindAsync(id);
            if (workPattern != null)
            {
                _context.WorkPatterns.Remove(workPattern);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkPatternExists(int id)
        {
            return _context.WorkPatterns.Any(e => e.Id == id);
        }
    }
}
