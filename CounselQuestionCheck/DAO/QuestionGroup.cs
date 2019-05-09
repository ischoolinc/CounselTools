using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounselQuestionCheck.DAO
{
    public class QuestionGroup
    {
        public string Group { get; set; }
        public List<QuestionQuery> QuestionQueryList;
    }
}
