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
            var vm = new QuestionViewModel
            {
                Question = repo.GetQuestionForId(id)
            };
            return View(vm);
        }
    }
}
