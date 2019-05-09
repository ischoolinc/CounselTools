using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounselQuestionCheck.DAO
{
    public class SubjectInfo
    {
        public string Subject { get; set; }
        public List<QuestionGroup> QuestionGroupList;
    }
}
