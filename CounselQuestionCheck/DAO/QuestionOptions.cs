﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounselQuestionCheck.DAO
{
    public class QuestionOptions
    {
        public string AnswerID { get; set; }
        public string OptionCode { get; set; }
        public string OptionText { get; set; }
        public string AnswerValue { get; set; }
        public string AnswerMatrix { get; set; }
        public string AnswerChecked { get; set; }
        public string AnswerComplete { get; set; }

    }
}
