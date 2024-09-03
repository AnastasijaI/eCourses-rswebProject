using eCourses.Areas.Identity.Data;
using eCourses.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eCourses.Models
{
    public static class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<eCoursesUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            eCoursesUser user = await UserManager.FindByEmailAsync("admin@course.com");
            if (user == null)
            {
                var User = new eCoursesUser();
                User.Email = "admin@course.com";
                User.UserName = "admin@course.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin 
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new eCoursesContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<eCoursesContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
                
                if (context.Course.Any())
                {
                    return;   
                }

               
                var instructors = new Instructor[]
                {
                    new Instructor
                    {
                        FullName = "John Doe",
                        BirthDate = new DateTime(1980, 5, 14),
                        Nationality = "American",
                        Gender = "Male",
                        ImageUrl = "https://th.bing.com/th/id/R.7c07f64bdb676d6a44a66a75f010dd1f?rik=ZF4TorZblJhbmw&pid=ImgRaw&r=0"
                    },
                    new Instructor
                    {
                        FullName = "Jane Smith",
                        BirthDate = new DateTime(1985, 7, 21),
                        Nationality = "British",
                        Gender = "Female",
                        ImageUrl = "https://images.gr-assets.com/authors/1553237143p8/6088388.jpg"
                    },
                    new Instructor
                    {
                        FullName = "Emily Johnson",
                        BirthDate = new DateTime(1990, 11, 30),
                        Nationality = "Canadian",
                        Gender = "Female",
                        ImageUrl = "https://th.bing.com/th/id/OIP.-GSgKMf717r3zukyHj1uoAHaHZ?rs=1&pid=ImgDetMain"
                    }
                };

                foreach (var instructor in instructors)
                {
                    context.Instructor.Add(instructor);
                }
                context.SaveChanges();

                // Add categories
                var categories = new Category[]
                {
                    new Category { CategoryName = "Programming Languages" },
                    new Category { CategoryName = "Web Development" },
                    new Category { CategoryName = "Data Science" }
                };

                foreach (var category in categories)
                {
                    context.Category.Add(category);
                }
                context.SaveChanges();

                // Add courses
                var courses = new Course[]
                {
                    new Course
                    {
                        Title = "Introduction to C#",
                        DateCreated = DateTime.Now,
                        Description = "Learn the basics of C# programming.",
                        InstructorId = instructors.Single(i => i.FullName == "John Doe").Id,
                        Image = "https://th.bing.com/th/id/OIP.lTQdUH-eRa6-CS53nBgvXgHaEo?rs=1&pid=ImgDetMain",
                        VideoUrl = "http://example.com/intro_to_csharp.mp4",
                        Price = 99.99M,
                        Language = "English"
                    },
                    new Course
                    {
                        Title = "Advanced JavaScript",
                        DateCreated = DateTime.Now,
                        Description = "Deep dive into advanced JavaScript concepts.",
                        InstructorId = instructors.Single(i => i.FullName == "Jane Smith").Id,
                        Image = "https://th.bing.com/th/id/R.978af0ee72293b99fe022f7e4ceeb874?rik=rsPkHmvAchQy4Q&pid=ImgRaw&r=0",
                        VideoUrl = "http://example.com/advanced_js.mp4",
                        Price = 149.99M,
                        Language = "English"
                    },
                    new Course
                    {
                        Title = "Data Science with Python",
                        DateCreated = DateTime.Now,
                        Description = "Comprehensive course on data science using Python.",
                        InstructorId = instructors.Single(i => i.FullName == "Emily Johnson").Id,
                        Image = "https://th.bing.com/th/id/R.245e4b0323f3cf082209efc08ca1fb22?rik=sn%2bgjDtJCmIHqA&pid=ImgRaw&r=0",
                        VideoUrl = "http://example.com/data_science_python.mp4",
                        Price = 199.99M,
                        Language = "English"
                    }
                };

                foreach (var course in courses)
                {
                    context.Course.Add(course);
                }
                context.SaveChanges();

                // Add course-category relationships
                var courseCategories = new CourseCategory[]
                {
                    new CourseCategory { CourseId = courses.Single(c => c.Title == "Introduction to C#").Id, CategoryId = categories.Single(c => c.CategoryName == "Programming Languages").Id },
                    new CourseCategory { CourseId = courses.Single(c => c.Title == "Advanced JavaScript").Id, CategoryId = categories.Single(c => c.CategoryName == "Web Development").Id },
                    new CourseCategory { CourseId = courses.Single(c => c.Title == "Data Science with Python").Id, CategoryId = categories.Single(c => c.CategoryName == "Data Science").Id }
                };

                foreach (var courseCategory in courseCategories)
                {
                    context.CourseCategory.Add(courseCategory);
                }
                context.SaveChanges();
            }
        }
    }
}
