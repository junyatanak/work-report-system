using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DailyWorkReport.Data;
using DailyWorkReport.Models;
using DailyWorkReport.Domain;
using DailyWorkReport.ViewModels.StandardWorkTime;
using Microsoft.AspNetCore.Authorization;
using DailyWorkReport.Constants;

namespace DailyWorkReport.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class StandardWorkTimesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StandardWorkTimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StandardWorkTimes
        public async Task<IActionResult> Index()
        {
            var vm = _context.StandardWorkTimes.Include(s => s.Process).Include(s => s.WorkClass).Include(s => s.WorkPattern)
            .Select(s => new StandardWorkTimeDisplayViewModel
            {
                Id = s.Id,
                WorkClassName = s.WorkClass.Name,
                ProcessName = s.Process.Name,
                WorkPatternName = s.WorkPattern.Name,
                StandardWorkTime = StandardWorkTimeConverter.ToPcsPerHour(s.StandardCycleSeconds)
            });
            return View(await vm.ToListAsync());
        }

        // GET: StandardWorkTimes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vm = await _context.StandardWorkTimes
                .Include(s => s.Process)
                .Include(s => s.WorkClass)
                .Include(s => s.WorkPattern)
                .Select(s => new StandardWorkTimeDisplayViewModel
                {
                    Id = s.Id,
                    WorkClassName = s.WorkClass.Name,
                    ProcessName = s.Process.Name,
                    WorkPatternName = s.WorkPattern.Name,
                    StandardWorkTime = StandardWorkTimeConverter.ToPcsPerHour(s.StandardCycleSeconds)
                })
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET: StandardWorkTimes/Create
        public IActionResult Create()
        {
            ViewData["ProcessId"] = new SelectList(_context.Processes, "Id", "Name");
            ViewData["WorkClassId"] = new SelectList(_context.WorkClasses, "Id", "Name");
            ViewData["WorkPatternId"] = new SelectList(_context.WorkPatterns, "Id", "Name");
            return View();
        }

        // POST: StandardWorkTimes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkClassId,ProcessId,WorkPatternId,StandardWorkTime")] StandardWorkTimeCreateViewModel vm)
        {
            if(await CombinationExistsAsync(vm.WorkClassId, vm.ProcessId, vm.WorkPatternId))
            {
                ModelState.AddModelError(string.Empty, "The combination of Work Class, Process, and Work Pattern must be unique.");
            }

            if (ModelState.IsValid)
            {
                var standardWorkTime = new StandardWorkTime
                {
                    WorkClassId = vm.WorkClassId,
                    ProcessId = vm.ProcessId,
                    WorkPatternId = vm.WorkPatternId,
                    StandardCycleSeconds = StandardWorkTimeConverter.ToStandardCycleSeconds(vm.StandardWorkTime)
                };
                _context.Add(standardWorkTime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProcessId"] = new SelectList(_context.Processes, "Id", "Name", vm.ProcessId);
            ViewData["WorkClassId"] = new SelectList(_context.WorkClasses, "Id", "Name", vm.WorkClassId);
            ViewData["WorkPatternId"] = new SelectList(_context.WorkPatterns, "Id", "Name", vm.WorkPatternId);
            return View(vm);
        }

        // GET: StandardWorkTimes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var standardWorkTime = await _context.StandardWorkTimes.FindAsync(id);
            if (standardWorkTime == null)
            {
                return NotFound();
            }
            var vm = new StandardWorkTimeEditViewModel{
                WorkClassId = standardWorkTime.WorkClassId,
                ProcessId = standardWorkTime.ProcessId,
                WorkPatternId = standardWorkTime.WorkPatternId,
                StandardWorkTime = StandardWorkTimeConverter.ToPcsPerHour(standardWorkTime.StandardCycleSeconds)
            };

            ViewData["ProcessId"] = new SelectList(_context.Processes, "Id", "Name", standardWorkTime.ProcessId);
            ViewData["WorkClassId"] = new SelectList(_context.WorkClasses, "Id", "Name", standardWorkTime.WorkClassId);
            ViewData["WorkPatternId"] = new SelectList(_context.WorkPatterns, "Id", "Name", standardWorkTime.WorkPatternId);
            return View(vm);
        }

        // POST: StandardWorkTimes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkClassId,ProcessId,WorkPatternId,StandardWorkTime")] StandardWorkTimeEditViewModel vm)
        {
            if(await CombinationExistsAsync(vm.WorkClassId, vm.ProcessId, vm.WorkPatternId, id))
            {
                ModelState.AddModelError(string.Empty, "The combination of Work Class, Process, and Work Pattern must be unique.");
            }

            var standardWorkTime = await _context.StandardWorkTimes.FindAsync(id);
            if (standardWorkTime == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    standardWorkTime.WorkClassId = vm.WorkClassId;
                    standardWorkTime.ProcessId = vm.ProcessId;
                    standardWorkTime.WorkPatternId = vm.WorkPatternId;
                    standardWorkTime.StandardCycleSeconds = StandardWorkTimeConverter.ToStandardCycleSeconds(vm.StandardWorkTime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StandardWorkTimeExists(standardWorkTime.Id))
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
            ViewData["ProcessId"] = new SelectList(_context.Processes, "Id", "Name", vm.ProcessId);
            ViewData["WorkClassId"] = new SelectList(_context.WorkClasses, "Id", "Name", vm.WorkClassId);
            ViewData["WorkPatternId"] = new SelectList(_context.WorkPatterns, "Id", "Name", vm.WorkPatternId);
            return View(vm);
        }

        // GET: StandardWorkTimes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vm = await _context.StandardWorkTimes
                .Include(s => s.Process)
                .Include(s => s.WorkClass)
                .Include(s => s.WorkPattern)
                .Select(s => new StandardWorkTimeDisplayViewModel
                {
                    Id = s.Id,
                    WorkClassName = s.WorkClass.Name,
                    ProcessName = s.Process.Name,
                    WorkPatternName = s.WorkPattern.Name,
                    StandardWorkTime = StandardWorkTimeConverter.ToPcsPerHour(s.StandardCycleSeconds)
                })
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // POST: StandardWorkTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var standardWorkTime = await _context.StandardWorkTimes.FindAsync(id);
            if (standardWorkTime != null)
            {
                _context.StandardWorkTimes.Remove(standardWorkTime);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StandardWorkTimeExists(int id)
        {
            return _context.StandardWorkTimes.Any(e => e.Id == id);
        }
        private async Task<bool> CombinationExistsAsync(
            int workClassId,
            int processId,
            int workPatternId,
            int? excludeId = null)
        {
            return await _context.StandardWorkTimes.AnyAsync(x =>
                x.WorkClassId == workClassId &&
                x.ProcessId == processId &&
                x.WorkPatternId == workPatternId &&
                (!excludeId.HasValue || x.Id != excludeId.Value));
        }
        
    }
}
