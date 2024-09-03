using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eCourses.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; }

        public ICollection<CourseCategory>? CourseCategories { get; set; }
    }
}
