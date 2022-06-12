using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using model = CMS.Web.AdminArea.DAL.Model;
using CMS.Web.AdminArea.DAL.Repository;
using System.Linq;
using System.Collections.Generic;
using MoreLinq;
using CMS.Web.AdminArea.Enum;
using CMS.Core.Domain;

namespace CMS.Web.AdminArea
{
    public partial class QuestionView : Page
    {
        QuestionGroupRepository questionGroupRepository;
        List<model.QuestionGroup> questionGroupModels;
        public QuestionView()
        {
            questionGroupRepository = new QuestionGroupRepository();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlLoaiFeedback.Items.Add(new ListItem("Tiếng Anh", ((int)LoaiFeedback.TiengAnh).ToString()));
                ddlLoaiFeedback.Items.Add(new ListItem("Tiếng Việt", ((int)LoaiFeedback.TiengViet).ToString()));
            }

            questionGroupModels = questionGroupRepository.LayDanhSachQuestionGroup(int.Parse(ddlLoaiFeedback.SelectedValue));
            var questionGroups = questionGroupModels.DistinctBy(x => x.QUESTION_GROUP_ID).Where(x => x.ISINDAYBOATFORM == false).OrderBy(x => x.PRIORITY);
            rptGroups.DataSource = questionGroups;
            rptGroups.DataBind();
            var questionGroupsTrongDayboatForm = questionGroupModels.DistinctBy(x => x.QUESTION_GROUP_ID).Where(x => x.ISINDAYBOATFORM == true).OrderBy(x => x.PRIORITY);
            rptDayboatGroup.DataSource = questionGroupsTrongDayboatForm;
            rptDayboatGroup.DataBind();
        }

        protected void rptGroups_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is model.QuestionGroup)
            {
                var group = (model.QuestionGroup)e.Item.DataItem;
                var rptQuestions = (Repeater)e.Item.FindControl("rptQuestions");
                var hplEdit = (HyperLink)e.Item.FindControl("hplEdit");
                rptQuestions.DataSource = questionGroupModels.Where(x => x.QUESTION_GROUP_ID == group.QUESTION_GROUP_ID);
                rptQuestions.DataBind();
                hplEdit.NavigateUrl = "questiongroupedit.aspx?groupid=" + group.QUESTION_GROUP_ID;
            }
        }

        protected void lbtDelete_Click(object sender, EventArgs e)
        {
            try
            {
                questionGroupRepository.BeginTransaction();
                var ctrl = (Control)sender;
                var hiddenId = (HiddenField)ctrl.Parent.FindControl("hiddenId");
                var group = questionGroupRepository.GetById(Convert.ToInt32(hiddenId.Value));
                group.Deleted = true;
                group.ModifiedBy = User.Identity as User;
                group.ModifiedDate = DateTime.Now;
                questionGroupRepository.Update(group);
                rptGroups.DataSource = questionGroupRepository.LayDanhSachQuestionGroup(int.Parse(ddlLoaiFeedback.SelectedValue));
                rptGroups.DataBind();
                questionGroupRepository.CommitTransaction();
            }
            catch (Exception ex)
            {
                questionGroupRepository.RollbackTransaction();
                throw ex;
            }
        }
    }
}
