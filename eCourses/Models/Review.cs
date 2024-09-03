using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eCourses.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Course")]
        [Required(ErrorMessage = "CourseId is required")]
        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        [Required(ErrorMessage = "AppUser is required")]
        public string AppUser { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        [Required(ErrorMessage = "Comment is required")]
        public string Comment { get; set; }

        public int? Likes { get; set; } 
    }
}
