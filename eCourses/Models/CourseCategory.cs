using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eCourses.Models
{
    public class CourseCategory
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Course")]
        [Required(ErrorMessage = "CourseId is required")]
        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        [ForeignKey("Category")]
        [Required(ErrorMessage = "CategoryId is required")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
