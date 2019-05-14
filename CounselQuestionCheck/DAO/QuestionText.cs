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
        public bool Require { get; set; }
        public string RequireLink { get; set; }
        public string Text { get; set; }

        public bool isQuestionCodePass = false;
        public bool isTypePass = false;
        public bool isRequirePass = false;
        public bool isRequireLinkPass = false;
        public bool isTextPass = false;

        public List<QuestionOptions> QuestionOptionsList;
    }
}
