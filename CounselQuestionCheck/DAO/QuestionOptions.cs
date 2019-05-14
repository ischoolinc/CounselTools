using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounselQuestionCheck.DAO
{
    public class QuestionOptions
    {
        public int AnswerID { get; set; }
        public string OptionCode { get; set; }
        public string OptionText { get; set; }
        public string AnswerValue { get; set; }
        public string AnswerMatrix { get; set; }
        public bool AnswerChecked { get; set; }
        public bool AnswerComplete { get; set; }


        public bool isAnswerIDPass = false;
        public bool isOptionCodePass = false;
        public bool isOptionTextPass = false;
        public bool isAnswerValuePass = false;
        public bool isAnswerMatrixPass = false;
        public bool isAnswerCheckedPass = false;
        public bool isAnswerCompletePass = false;
    }
}
