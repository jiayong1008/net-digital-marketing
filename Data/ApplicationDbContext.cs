using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalMarketing2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DigitalMarketing2.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DigitalMarketing2.Models.Module> Module { get; set; } = default!;

        public DbSet<DigitalMarketing2.Models.Lesson> Lesson { get; set; } = default!;

        public DbSet<DigitalMarketing2.Models.LessonSection> LessonSection { get; set; } = default!;
        public DbSet<DigitalMarketing2.Models.QuizQuestion> QuizQuestion { get; set; } = default!;
        public DbSet<DigitalMarketing2.Models.QuestionOption> QuestionOption { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<QuizQuestion>()
                .HasMany(q => q.QuestionOptions)
                .WithOne(qo => qo.QuizQuestion)
                .HasForeignKey(qo => qo.QuizQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(q => q.Answer)
                .WithOne()
                .HasForeignKey<QuizQuestion>(q => q.AnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentScore>()
                .HasOne(score => score.QuizQuestion)
                .WithMany()
                .HasForeignKey(score => score.StudentQuizQuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
