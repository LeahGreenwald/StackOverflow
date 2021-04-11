using StackOverflow.Data;
using System;
using System.Collections.Generic;

namespace StackOverflow.Web.Models
{
    public class HomeViewModel
    {
        public List<Question> Questions { get; set; }
    }
    public class QuestionViewModel
    {
        public Question Question { get; set; }
    }
}
