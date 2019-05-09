using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounselQuestionCheck.DAO
{
    public class QuestionQuery
    {
        public string Query { get; set; }
        public List<QuestionText> QuestionTextList;
    }
}
