using eCourses.Models;
using System.Collections.Generic;

namespace eCourses.viewModel
{
    public class CourseReviewViewModel
    {
        public Course Course { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}
