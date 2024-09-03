using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eCourses.Models;
using eCourses.Areas.Identity.Data;

namespace eCourses.Data
{
    public class eCoursesContext : IdentityDbContext<eCoursesUser>
    {
        public eCoursesContext (DbContextOptions<eCoursesContext> options)
            : base(options)
        {
        }

        public DbSet<eCourses.Models.Course> Course { get; set; } = default!;
        public DbSet<eCourses.Models.Instructor> Instructor { get; set; } = default!;
        public DbSet<eCourses.Models.Category> Category { get; set; } = default!;
        public DbSet<eCourses.Models.CourseCategory> CourseCategory { get; set; } = default!;
        public DbSet<eCourses.Models.Review> Review { get; set; } = default!;
        public DbSet<eCourses.Models.UserCourse> UserCourse { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
