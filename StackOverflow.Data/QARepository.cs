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
        public void AddUser(User user, string password)
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
        public User Login(string email, string password)
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
            List<Question> questions = context.Questions.OrderByDescending(q => q.Date)
                .Include(q => q.Likes)
                .Include(q => q.Answers)
                .Include(q => q.QuestionsTags).ThenInclude(qt => qt.Tag)
                .ToList();
            foreach (Question question in questions)
            {
                question.User = GetById(question.Id);
            }
            return questions;
        }
        public User GetById(int id)
        {
            using var context = new QADbContext(_connectionString);
            return context.Users.Include(u => u.Likes).FirstOrDefault(u => u.Id == id);
        }
        public Question GetQuestionForId(int id)
        {
            using var context = new QADbContext(_connectionString);
            var question = context.Questions
                .Include(q => q.Likes).ThenInclude(l => l.User)
                .Include(q => q.Answers)
                .Include(q => q.QuestionsTags).ThenInclude(qt => qt.Tag)
                .FirstOrDefault(q => q.Id == id);
            question.User = GetById(question.UserId);
            foreach (Answer answer in question.Answers)
            {
                answer.User = GetById(answer.UserId);
            }
            return question;
        }
        public List<Tag> GetTags()
        {
            using var context = new QADbContext(_connectionString);
            return context.Tags.ToList();
        }
        public void AddAnswer(Answer answer)
        {
            using var context = new QADbContext(_connectionString);
            context.Answers.Add(answer);
            context.SaveChanges();
        }
        public void AddQuestion(Question question, List<string> tagTexts)
        {
            using var context = new QADbContext(_connectionString);
            context.Questions.Add(question);
            context.SaveChanges();
            var tags = GetTags();
            foreach (string tagText in tagTexts)
            {
                var tag = tags.FirstOrDefault(t => t.Name == tagText);
                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = tagText
                    };
                    context.Tags.Add(tag);
                }
                context.QuestionsTags.Add(new QuestionsTags
                {
                    QuestionId = question.Id,
                    Question = question,
                    Tag = tag,
                    TagId = tag.Id
                });
            }
            context.SaveChanges();
        }
        public void UpdateLikes(int id, User user)
        {
            using var context = new QADbContext(_connectionString);
            var like = new Likes
            {
                QuestionId = id,
                UserId = user.Id
            };
            context.Likes.Add(like);
            context.SaveChanges();
        }
        public int GetLikes (int id)
        {
            using var context = new QADbContext(_connectionString);
            var question = GetQuestionForId(id);
            return question.Likes.Count;
        }
    }
}
