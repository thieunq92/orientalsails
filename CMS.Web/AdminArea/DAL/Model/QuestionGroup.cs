using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Web.AdminArea.DAL.Model
{
    public class QuestionGroup
    {
        public int QUESTION_GROUP_ID { get; set; }
        public string QUESTION_GROUP_NAME { get; set; }
        public bool ISINDAYBOATFORM { get; set; }
        public int QUESTION_ID { get; set; }
        public string QUESTION_NAME { get; set; }
        public int PRIORITY { get; set; }
    }
}