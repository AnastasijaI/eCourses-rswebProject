using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eCourses.Data;
using eCourses.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eCourses.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly eCoursesContext _context;

        public ReviewsController(eCoursesContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var reviews = _context.Review.Include(r => r.Course);
            return View(await reviews.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Review
                .Include(r => r.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (review == null) return NotFound();

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create(int courseId)
        {
            var review = new Review { CourseId = courseId };
            return View(review);
        }

        // POST: Reviews/Create
        // GET: Reviews/Create
        [HttpGet]
        [HttpPost]
        public IActionResult Create(int courseId, Review review = null)
        {
            if (Request.Method == "GET")
            {
                var model = new Review
                {
                    AppUser = User.Identity.Name,
                    CourseId = courseId
                };
                return View(model);
            }

            if (ModelState.IsValid)
            {
                review.AppUser = User.Identity.Name;
                review.CourseId = courseId;
                _context.Add(review);
                _context.SaveChanges();
                return RedirectToAction("Details", "Courses", new { id = review.CourseId });
            }

            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null) return NotFound();

            var review = await _context.Review
                .Include(r => r.Course) 
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,AppUser,Comment,Likes")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var trackedReview = _context.Review.Local.FirstOrDefault(r => r.Id == id);

                    if (trackedReview != null)
                    {
                        _context.Entry(trackedReview).CurrentValues.SetValues(review);
                    }
                    else
                    {
                        _context.Attach(review).State = EntityState.Modified;
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", "Courses", new { id = review.CourseId });
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Description", review.CourseId);
            return View(review);
        }
        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Review
                .Include(r => r.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (review == null) return NotFound();

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Review.FindAsync(id);
            if (review != null)
            {
                _context.Review.Remove(review);
                await _context.SaveChangesAsync();
            }

            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Details", "Courses", new { id = review.CourseId });
        }

        private bool ReviewExists(int id)
        {
            return _context.Review.Any(e => e.Id == id);
        }
    }
}

