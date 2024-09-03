using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eCourses.Data;
using eCourses.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace eCourses.Controllers
{
    public class UserCoursesController : Controller
    {
        private readonly eCoursesContext _context;

        public UserCoursesController(eCoursesContext context)
        {
            _context = context;
        }

        public async Task<bool> AddCourseToUser(string userId, int courseId)
        {
            var course = await _context.Course.FindAsync(courseId);
            if (course == null)
            {
                return false; 
            }

            var existingUserCourse = await _context.UserCourse
                .FirstOrDefaultAsync(uc => uc.AppUser == userId && uc.CourseId == courseId);

            if (existingUserCourse != null)
            {
                return false; 
            }

            var userCourse = new UserCourse
            {
                AppUser = userId,
                CourseId = courseId
            };

            _context.UserCourse.Add(userCourse);
            await _context.SaveChangesAsync();
            return true;
        }

        // GET: UserCourses
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userName = _context.Users.FirstOrDefault(u => u.Id == userId)?.UserName;

            var userCourses = await _context.UserCourse
                .Include(uc => uc.Course)
                .Where(uc => uc.AppUser == userId)
                .ToListAsync();

            ViewBag.UserName = userName;
            return View(userCourses);
        }

        // GET: UserCourses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCourse = await _context.UserCourse
                .Include(u => u.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userCourse == null)
            {
                return NotFound();
            }

            return View(userCourse);
        }

        // GET: UserCourses/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Description");
            return View();
        }

        // POST: UserCourses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppUser,CourseId")] UserCourse userCourse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Description", userCourse.CourseId);
            return View(userCourse);
        }

        // GET: UserCourses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCourse = await _context.UserCourse.FindAsync(id);
            if (userCourse == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Description", userCourse.CourseId);
            return View(userCourse);
        }

        // POST: UserCourses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUser,CourseId")] UserCourse userCourse)
        {
            if (id != userCourse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userCourse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserCourseExists(userCourse.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Description", userCourse.CourseId);
            return View(userCourse);
        }

        // GET: UserCourses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCourse = await _context.UserCourse
                .Include(u => u.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userCourse == null)
            {
                return NotFound();
            }

            return View(userCourse);
        }

        // POST: UserCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userCourse = await _context.UserCourse.FindAsync(id);
            if (userCourse != null)
            {
                _context.UserCourse.Remove(userCourse);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveCourseFromUser(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var userCourse = await _context.UserCourse
                .FirstOrDefaultAsync(uc => uc.CourseId == courseId && uc.AppUser == userId);

            if (userCourse == null)
            {
                return NotFound();
            }

            _context.UserCourse.Remove(userCourse);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Buy(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var course = await _context.Course.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var existingUserCourse = await _context.UserCourse
                .FirstOrDefaultAsync(uc => uc.CourseId == courseId && uc.AppUser == userId);

            if (existingUserCourse != null)
            {
                return RedirectToAction("Index", "UserCourses");
            }

            var userCourse = new UserCourse
            {
                AppUser = userId,
                CourseId = courseId
            };

            _context.UserCourse.Add(userCourse);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "UserCourses");
        }

        private bool UserCourseExists(int id)
        {
            return _context.UserCourse.Any(e => e.Id == id);
        }
    }
}

