using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eCourses.Data;
using eCourses.Models;
using eCourses.viewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace eCourses.Controllers
{
    public class CoursesController : Controller
    {
        private readonly eCoursesContext _context;
        private readonly UserCoursesController _userCoursesController;

        public CoursesController(eCoursesContext context, UserCoursesController userCoursesController)
        {
            _context = context;
            _userCoursesController = userCoursesController;
        }
        public IActionResult CoursesByInstructors(int instructorId)
        {
            var courses = _context.Course
                .Where(c => c.InstructorId == instructorId)
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .ToList();

            if (courses.Count == 0)
            {
                return NotFound();
            }

            ViewData["InstructorName"] = courses.FirstOrDefault()?.Instructor?.FullName;
            return View(courses);
        }


        // GET: Courses
        public async Task<IActionResult> Index(string searchString1, string searchString2, string searchString3)
        {
            var courses = from c in _context.Course.Include(c => c.Instructor)
                          select c;

            if (!String.IsNullOrEmpty(searchString1))
            {
                courses = courses.Where(c => c.Title.Contains(searchString1));
            }

            if (!String.IsNullOrEmpty(searchString2))
            {
                courses = courses.Where(c => c.Instructor.FullName.Contains(searchString2));
            }

            if (!String.IsNullOrEmpty(searchString3))
            {
                courses = courses.Where(c => c.Language.Contains(searchString3));
            }

            return View(await courses.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            var viewModel = new CourseReviewViewModel
            {
                Course = course,
                Reviews = course.Reviews
            };

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool hasUserBoughtCourse = userId != null &&
                await _context.UserCourse.AnyAsync(uc => uc.CourseId == id && uc.AppUser == userId);

            ViewData["HasUserBoughtCourse"] = hasUserBoughtCourse;

            return View(course);
        }
        
        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["InstructorId"] = new SelectList(_context.Set<Instructor>(), "Id", "FullName");
            return View();
        }
        
        // POST: Courses/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course, IFormFile image, IFormFile video)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    course.Image = "/images/" + imageFileName;
                }

                if (video != null && video.Length > 0)
                {
                    var videoFileName = Guid.NewGuid().ToString() + Path.GetExtension(video.FileName);
                    var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/videos", videoFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(videoPath));
                    using (var stream = new FileStream(videoPath, FileMode.Create))
                    {
                        await video.CopyToAsync(stream);
                    }
                    course.VideoUrl = "/videos/" + videoFileName;
                }

                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["InstructorId"] = new SelectList(_context.Set<Instructor>(), "Id", "FullName", course.InstructorId);
            return View(course);
        }


        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["InstructorId"] = new SelectList(_context.Set<Instructor>(), "Id", "FullName", course.InstructorId);
            return View(course);
        }
        // POST: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course, IFormFile image, IFormFile video)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image != null && image.Length > 0)
                    {
                        var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageFileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        course.Image = "/images/" + imageFileName;
                    }

                    if (video != null && video.Length > 0)
                    {
                        var videoFileName = Guid.NewGuid().ToString() + Path.GetExtension(video.FileName);
                        var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/videos", videoFileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(videoPath));
                        using (var stream = new FileStream(videoPath, FileMode.Create))
                        {
                            await video.CopyToAsync(stream);
                        }
                        course.VideoUrl = "/videos/" + videoFileName;
                    }

                    _context.Update(course); 
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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
            ViewData["InstructorId"] = new SelectList(_context.Set<Instructor>(), "Id", "FullName", course.InstructorId);
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            if (course != null)
            {
                _context.Course.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Buy(int courseId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            bool result = await _userCoursesController.AddCourseToUser(userId, courseId);

            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "UserCourses");
        }
    }
}
