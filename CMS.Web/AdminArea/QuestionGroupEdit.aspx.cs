using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CMS.Web.AdminArea.DAL.Repository;
using CMS.Web.AdminArea.DAL.Domain;
using CMS.Core.Domain;
using CMS.Web.AdminArea.Enum;

namespace CMS.Web.AdminArea
{
    public partial class QuestionGroupEdit : Page
    {
        private QuestionGroupRepository _questionGroupRepository;
        private QuestionRepository _questionRepository;
        public QuestionGroupEdit()
        {
            _questionGroupRepository = new QuestionGroupRepository();
            _questionRepository = new QuestionRepository();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlLoaiFeedback.Items.Add(new ListItem("Tiếng Anh", ((int)LoaiFeedback.TiengAnh).ToString()));
                ddlLoaiFeedback.Items.Add(new ListItem("Tiếng Việt", ((int)LoaiFeedback.TiengViet).ToString()));
                if (Request.QueryString["groupid"] != null)
                {
                    var group = _questionGroupRepository.GetById(Convert.ToInt32(Request.QueryString["groupid"]));
                    txtSubject.Text = group.Name;
                    ddlLoaiFeedback.SelectedValue = group.LOAI_FEEDBACK.ToString();
                    txtSelection1.Text = group.Selection1;
                    txtSelection2.Text = group.Selection2;
                    txtSelection3.Text = group.Selection3;
                    txtSelection4.Text = group.Selection4;
                    txtSelection5.Text = group.Selection5;
                    txtPriority.Text = group.Priority.ToString();
                    chkIsInDayboatForm.Checked = group.IsInDayboatForm;
                    ddlGoodChoice.SelectedValue = group.GoodChoice.ToString();
                    rptSubCategory.DataSource = group.Questions;
                    rptSubCategory.DataBind();
                }
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            ((Control)sender).Parent.Visible = false;
        }

        protected void btnAddSubCategory_Click(object sender, EventArgs e)
        {
            IList<Question> data = RepeaterToList();
            data.Add(new Question());
            rptSubCategory.DataSource = data;
            rptSubCategory.DataBind();
        }

        protected IList<Question> RepeaterToList()
        {
            IList<Question> result = new List<Question>();
            foreach (RepeaterItem item in rptSubCategory.Items)
            {
                HiddenField hiddenId = (HiddenField)item.FindControl("hiddenId");
                TextBox txtName = (TextBox)item.FindControl("txtName");
                TextBox txtContent = (TextBox)item.FindControl("txtContent");

                Question question = new Question();
                int id = Convert.ToInt32(hiddenId.Value);
                if (id > 0)
                {
                    question = _questionRepository.GetById(id);
                }
                question.Name = txtName.Text;
                question.Content = txtContent.Text;
                question.Deleted = !item.Visible;
                result.Add(question);
            }

            return result;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _questionGroupRepository.BeginTransaction();
                _questionRepository.BeginTransaction();
                QuestionGroup group = _questionGroupRepository.GetById(Convert.ToInt32(Request.QueryString["groupid"]));
                if (group == null)
                {
                    group = new QuestionGroup();
                }

                group.Name = txtSubject.Text;
                group.Selection1 = txtSelection1.Text;
                group.Selection2 = txtSelection2.Text;
                group.Selection3 = txtSelection3.Text;
                group.Selection4 = txtSelection4.Text;
                group.Selection5 = txtSelection5.Text;
                group.Priority = Convert.ToInt32(txtPriority.Text);
                group.GoodChoice = Convert.ToInt32(ddlGoodChoice.SelectedValue);
                group.IsInDayboatForm = chkIsInDayboatForm.Checked;
                group.LOAI_FEEDBACK = int.Parse(ddlLoaiFeedback.SelectedValue);
                if (group.Id <= 0)
                {
                    group.CreatedBy = User.Identity as User;
                    group.CreatedDate = DateTime.Now;
                    _questionGroupRepository.SaveOrUpdate(group);
                }
                else
                {
                    group.ModifiedBy = User.Identity as User;
                    group.ModifiedDate = DateTime.Now;
                    _questionGroupRepository.SaveOrUpdate(group);
                }

                IList<Question> data = RepeaterToList();
                foreach (Question q in data)
                {
                    q.Group = group;
                    _questionRepository.SaveOrUpdate(q);
                }
                _questionGroupRepository.CommitTransaction();
                _questionRepository.CommitTransaction();
                Response.Redirect("questionview.aspx", false);
            }
            catch (Exception ex)
            {
                _questionGroupRepository.RollbackTransaction();
                _questionRepository.RollbackTransaction();
                throw ex;
            }
        }

        protected void rptSubCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is Question)
            {
                Question question = (Question)e.Item.DataItem;

                TextBox txtName = (TextBox)e.Item.FindControl("txtName");
                TextBox txtContent = (TextBox)e.Item.FindControl("txtContent");

                txtName.Text = question.Name;
                txtContent.Text = question.Content;
                e.Item.Visible = !question.Deleted;
            }
        }
    }
}
