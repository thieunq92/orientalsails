using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using model = CMS.Web.AdminArea.DAL.Model;
using domain = CMS.Web.AdminArea.DAL.Domain;
using NHibernate.Transform;

namespace CMS.Web.AdminArea.DAL.Repository
{
    public class QuestionGroupRepository : RepositoryBase<domain.QuestionGroup>
    {
        public List<model.QuestionGroup> LayDanhSachQuestionGroup(int loaiFeedback)
        {
            var query = _session.CreateSQLQuery(
                        @"SELECT 
                          QG.ID QUESTION_GROUP_ID,
                          QG.NAME QUESTION_GROUP_NAME,
                          QG.ISINDAYBOATFORM ISINDAYBOATFORM,
                          QG.PRIORITY PRIORITY,  
                          Q.ID QUESTION_ID,
                          Q.NAME QUESTION_NAME
                        FROM 
                          SV_QUESTIONGROUP QG 
                          LEFT JOIN SV_QUESTION Q ON Q.GroupId = QG.Id 
                        WHERE 
                          (Q.Deleted IS NULL OR Q.Deleted = 0)
                          AND QG.Deleted = 0
                          AND QG.LOAI_FEEDBACK = :P_LOAI_FEEDBACK
                        ");
            return query
                .SetParameter("P_LOAI_FEEDBACK", loaiFeedback)
                .SetResultTransformer(Transformers.AliasToBean<model.QuestionGroup>())
                .List<model.QuestionGroup>().ToList();
        }
    }
}