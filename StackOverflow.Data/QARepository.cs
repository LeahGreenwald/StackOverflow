using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackOverflow.Data
{
    public class QARepository
    {
        private readonly string _connectionString;
        public QARepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddUser (User user, string password)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            using var context = new QADbContext(_connectionString);
            context.Users.Add(user);
            context.SaveChanges();
        }
        public User GetByEmail(string email)
        {
            using var context = new QADbContext(_connectionString);
            return context.Users.FirstOrDefault(u => u.Email == email);
        }
        public User Login (string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            var validEmail = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (validEmail)
            {
                return user;
            }
            return null;
        }
        public List<Question> GetQuestions()
        {
            using var context = new QADbContext(_connectionString);
            return context.Questions.OrderByDescending(q => q.Date).Include(q => q.Likes).Include(q => q.Answers).Include(q => q.QuestionsTags).ThenInclude(qt => qt.Tag).ToList();
        }
        public Question GetQuestionForId (int id)
        {
            using var context = new QADbContext(_connectionString);
            return context.Questions.Include(q => q.Likes).ThenInclude(l => l.User).Include(q => q.Answers).Include(q => q.QuestionsTags).ThenInclude(qt => qt.Tag).FirstOrDefault(q => q.Id == id);
        }
    }
}
