using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StackOverflow.Data;
using StackOverflow.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackOverflow.Web.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly string _connectionString;
        public QuestionsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult ViewQuestion(int id)
        {
            var connectionString = _connectionString;
            var repo = new QARepository(connectionString);
            var question = repo.GetQuestionForId(id);
            var vm = new QuestionViewModel
            {
                Question = question
            };
            return View(vm);
        }
        [Authorize]
        public IActionResult AddAnswer(Answer answer)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            var connectionString = _connectionString;
            var repo = new QARepository(connectionString);
            var email = User.Identity.Name;
            answer.Date = DateTime.Now;
            answer.UserId = repo.GetByEmail(email).Id;
            repo.AddAnswer(answer);
            return Redirect($"/questions/viewquestion?id={answer.QuestionId}");
        }
        [Authorize]
        public IActionResult AskAQuestion()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult Add(string title, string text, List<string> tags)
        {
            var connectionString = _connectionString;
            var repo = new QARepository(connectionString);
            var currentUser = repo.GetByEmail(User.Identity.Name);
            Question question = new Question
            {
                Title = title,
                Text = text,
                Date = DateTime.Now,
                UserId = currentUser.Id
            };
            repo.AddQuestion(question, tags);
            return Redirect("/");
        }
    }
}
