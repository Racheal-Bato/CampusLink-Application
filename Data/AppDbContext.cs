﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CampusLink_Application.Models;
using CampusLink.Models;


namespace CampusLink_Application.Data
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
       
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lecturer> Lecturers{ get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<CourseLecturer> CourseLecturers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many for Course and Lecturer
            modelBuilder.Entity<CourseLecturer>()
                .HasKey(cl => new { cl.CourseId, cl.LecturerId });

            modelBuilder.Entity<CourseLecturer>()
                .HasOne(cl => cl.Course)
                .WithMany(c => c.CourseLecturers)
                .HasForeignKey(cl => cl.CourseId);


            modelBuilder.Entity<CourseLecturer>()
                .HasOne(cl => cl.Lecturer)
                .WithMany(l => l.CourseLecturers)
                .HasForeignKey(cl => cl.LecturerId);
            

            // One-to-one ApplicationUser <-> Student
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Student)
                .WithOne(s => s.ApplicationUser)
                .HasForeignKey<ApplicationUser>(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-one ApplicationUser <-> Lecturer
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Lecturer)
                .WithOne(l => l.ApplicationUser)
                .HasForeignKey<ApplicationUser>(a => a.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

        }




    }

}
