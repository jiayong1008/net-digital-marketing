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
        public DbSet<DigitalMarketing2.Models.StudentScore> StudentScore { get; set; } = default!;
        public DbSet<DigitalMarketing2.Models.Discussion> Discussion { get; set; } = default!;

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
                .WithMany(q => q.StudentScores)
                .HasForeignKey(score => score.QuizQuestionId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // SEED DATA
            // Module
            // modelBuilder.Entity<Module>().HasData(
            //     new Module()
            //     {
            //         Name = "Take a business online",
            //         Description = "It's never been easier, cheaper or more beneficial for your business to get online. Don’t be intimidated by the breadth of opportunities in digital.",
            //         ModuleOrder = 1,
            //     },
            //     new Module()
            //     {
            //         Name = "Make it easy for people to find a business on the web",
            //         Description = "Start our free course to discover just some of the ways businesses can reach and connect with more customers online. Plus, learn how to improve your search engine performance (SEO), and use online advertising (SEM) to boost sales and awareness.",
            //         ModuleOrder = 2,
            //     },
            //     new Module()
            //     {
            //         Name = "Get yourself noticed on social media",
            //         Description = "Everyone is on social media, so it makes sense that your business should be on social media too. To take advantage of popular social media networks, understand why you need to be there, join the right social media sites, and increase your presence by engaging with them.",
            //         ModuleOrder = 3,
            //     },
            //     new Module()
            //     {
            //         Name = "Expand a business to other countries",
            //         Description = "Learn how to take your business global and start selling to customers abroad.",
            //         ModuleOrder = 4,
            //     },
            //     new Module()
            //     {
            //         Name = "Promote a business with online advertising",
            //         Description = "Discover the online tools you can use to promote a business online, create your marketing strategy, and attract the right customers.",
            //         ModuleOrder = 5,
            //     }
            // );

            // modelBuilder.Entity<Lesson>().HasData(
            //     new Lesson {

            //     }
            // );



            // modelBuilder.Entity<StudentScore>()
            //     .HasOne(score => score.QuizQuestion)
            //     .WithMany()
            //     .HasForeignKey(score => score.QuizQuestionId)
            //     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
