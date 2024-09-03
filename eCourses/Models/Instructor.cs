using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace eCourses.Models
{
    public class Instructor
    {
        [Key]
        public int Id { get; set; }

        public string? ImageUrl { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "BirthDate is required")]
        public DateTime? BirthDate { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Nationality is required")]
        public string? Nationality { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }

        public ICollection<Course>? Courses { get; set; }
    }
}
