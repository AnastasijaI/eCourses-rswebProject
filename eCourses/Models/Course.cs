using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eCourses.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "DateCreated is required")]
        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [ForeignKey("Instructor")]
        [Required(ErrorMessage = "InstructorId is required")]
        public int? InstructorId { get; set; }

        public Instructor? Instructor { get; set; }

        public string? Image { get; set; }

        public string? VideoUrl { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Language is required")]
        public string Language { get; set; }

        public ICollection<CourseCategory>? CourseCategories { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<UserCourse>? UserCourses { get; set; }
    }
}
