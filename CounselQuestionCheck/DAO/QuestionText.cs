using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounselQuestionCheck.DAO
{
    public class QuestionText
    {
        public string QuestionCode { get; set; }
        public string Type { get; set; }
        public string Require { get; set; }
        public string RequireLink { get; set; }
        public string Text { get; set; }

        public List<QuestionOptions> QuestionOptionsList;
    }
}
