using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace StackOverflow.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public List<Likes> Likes { get; set; }
        public List<Answer> Answers { get; set; }
        public List<Question> Questions { get; set; }
    }
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public User PostedBy { get; set; }
        public List<QuestionsTags> QuestionsTags { get; set; }
        public List<Likes> Likes { get; set; }
        public List<Answer> Answers { get; set; }
    }
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<QuestionsTags> QuestionsTags { get; set; }
    }
    public class QuestionsTags
    {
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
    public class Likes
    {
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }

    public class QADbContext : DbContext
    {
        private readonly string _connectionString;
        public QADbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<QuestionsTags> QuestionsTags { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Likes> Likes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionsTags>()
                .HasKey(qt => new { qt.QuestionId, qt.TagId });

            modelBuilder.Entity<QuestionsTags>()
                 .HasOne(qt => qt.Question)
                 .WithMany(q => q.QuestionsTags)
                 .HasForeignKey(q => q.QuestionId);

            modelBuilder.Entity<QuestionsTags>()
                .HasOne(qt => qt.Tag)
                .WithMany(t => t.QuestionsTags)
                .HasForeignKey(q => q.TagId);

            modelBuilder.Entity<Likes>()
                .HasKey(l => new { l.QuestionId, l.UserId });

            modelBuilder.Entity<Likes>()
                 .HasOne(l => l.Question)
                 .WithMany(q => q.Likes)
                 .HasForeignKey(q => q.QuestionId);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(u => u.UserId);
        }
    }
    public class QAContextFactory : IDesignTimeDbContextFactory<QADbContext>
    {
        public QADbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), $"..{Path.DirectorySeparatorChar}StackOverflow.Web"))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true).Build();

            return new QADbContext(config.GetConnectionString("ConStr"));
        }
    }
}
