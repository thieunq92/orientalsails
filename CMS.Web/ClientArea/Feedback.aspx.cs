using CMS.Web.AdminArea.DAL.Domain;
using CMS.Web.AdminArea.DAL.Repository;
using CMS.Web.AdminArea.Enum;
using CMS.Web.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CMS.Web.ClientArea
{
    public partial class Feedback : System.Web.UI.Page
    {
        private QuestionGroupRepository questionGroupRepository;
        private QuestionRepository questionRepository;
        private CruiseRepository cruiseRepository;
        private RoomRepository roomRepository;
        private AnswerGroupRepository answerGroupRepository;
        private AnswerOptionRepository answerOptionRepository;
        private AnswerSheetRepository answerSheetRepository;
        private Question currentQuestion;
        private List<string> currentOptions;
        private bool hasQuestion = true;
        public string errorMessage = "";

        public Feedback()
        {
            questionGroupRepository = new QuestionGroupRepository();
            questionRepository = new QuestionRepository();
            cruiseRepository = new CruiseRepository();
            roomRepository = new RoomRepository();
            answerGroupRepository = new AnswerGroupRepository();
            answerOptionRepository = new AnswerOptionRepository();
            answerSheetRepository = new AnswerSheetRepository();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void InitializeCulture()
        {
            try
            {
                if (Session["lang"] == null)
                {
                    Session["lang"] = "en-GB";
                }

                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["lang"].ToString());
                base.InitializeCulture();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void rptGroups_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is QuestionGroup)
            {
                QuestionGroup group = (QuestionGroup)e.Item.DataItem;
                ValueBinder.BindLiteral(e.Item, "litGroupName", group.Name);
                currentOptions = group.Selections.ToList();
                Repeater rptQuestions = (Repeater)e.Item.FindControl("rptQuestions");
                rptQuestions.DataSource = group.Questions;
                rptQuestions.DataBind();
                Repeater rptOptions = (Repeater)e.Item.FindControl("rptOptions");
                rptOptions.DataSource = currentOptions;
                rptOptions.DataBind();
            }
        }

        protected void rptQuestion_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is Question)
            {
                Question question = (Question)e.Item.DataItem;
                ValueBinder.BindLiteral(e.Item, "litQuestion", question.Content);
                currentQuestion = question;
                Repeater rptOptions = (Repeater)e.Item.FindControl("rptOptions");
                rptOptions.DataSource = currentOptions;
                rptOptions.DataBind();
            }
        }

        protected void rptOptions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RadioButton radOption = (RadioButton)e.Item.FindControl("radOption");
            radOption.GroupName = currentQuestion.Id.ToString();
            string script = "SetUniqueRadioButton('$" + currentQuestion.Id + "',this)";
            radOption.Attributes.Add("onclick", script);
        }

        protected void ddlCruises_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var cruiseId = Int32.Parse(ddlCruises.SelectedValue);
                if (cruiseId <= 0)
                {
                    return;
                }

                ddlRoom.DataTextField = "Name";
                ddlRoom.DataValueField = "Id";
                ddlRoom.Items.Clear();
                ddlRoom.Items.Add(new ListItem("-- " + Resources.Resource.Feedback_Select + "--", ""));
                ddlRoom.DataSource = roomRepository.GetAll().Where(x => x.Cruise.Id == cruiseId && x.Deleted == false).OrderBy(x=>x.Name);
                ddlRoom.DataBind();
            }
            catch (Exception ex)
            {
                //Ghi log
                return;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (Context.Session.IsNewSession)
                {
                    var message = "";
                    if(hidLang.Value == ((int)LoaiFeedback.TiengViet).ToString())
                    {
                        message = "Phiên giao dịch hết hạn. Ấn ok để tải lại trang";
                    }
                    else
                    {
                        message = "Session has timed out. Press ok to reload page";
                    }

                    var script = @"
                            $(function(){
                                    alert('"+ message +@"');
                                    window.location.reload(false); 
                                }
                            )";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "showContent", script, true);
                    throw new Exception();
                }

                answerSheetRepository.BeginTransaction();
                answerGroupRepository.BeginTransaction();
                answerOptionRepository.BeginTransaction();
                var answerSheet = new AnswerSheet();
                if (string.IsNullOrEmpty(txtStartDate.Text))
                {
                    ShowError(Resources.Resource.Feedback_PleaseChooseStartDate);
                    throw new Exception();
                }

                DateTime startDate = DateTime.MinValue;
                try
                {
                    startDate = Convert.ToDateTime(txtStartDate.Text, CultureInfo.CurrentCulture.DateTimeFormat);
                }
                catch (Exception ex)
                {
                    ShowError(Resources.Resource.Feedback_PleaseCheckStartDateFormatAgain);
                    throw new Exception();
                }

                var cruiseId = Int32.Parse(ddlCruises.SelectedValue);
                if (cruiseId <= 0)
                {
                    ShowError(Resources.Resource.Feedback_PleaseChooseCruise);
                    throw new Exception();
                }

                var roomId = Int32.Parse(ddlRoom.SelectedValue);
                if (roomId <= 0)
                {
                    ShowError(Resources.Resource.Feedback_PleaseChooseRoom);
                    throw new Exception();
                }

                answerSheet.Date = startDate;
                answerSheet.CruiseId = cruiseId;
                answerSheet.ROOM_ID = roomId;
                if(Session["lang"]!= null && Session["lang"].ToString() == "vi")
                {
                    answerSheet.LOAI_FEEDBACK = (int)LoaiFeedback.TiengViet;
                }
                else
                {
                    answerSheet.LOAI_FEEDBACK = (int)LoaiFeedback.TiengAnh;
                }

                var isPhoneNumberNumeric = Double.TryParse(txtPN.Text, out _);
                if(isPhoneNumberNumeric == false) {
                    ShowError(Resources.Resource.Feedback_PhoneNumberAreOnlyNumbers);
                    throw new Exception();
                }

                if(txtPN.Text.Length > 11)
                {
                    ShowError(Resources.Resource.Feedback_PhoneNumberExceed11Characters);
                    throw new Exception();
                }
                answerSheet.SO_DIEN_THOAI = txtPN.Text;
                answerSheet.Name = txtName.Text;
                foreach (RepeaterItem groupItem in rptGroups.Items)
                {
                    var hiddenId = (HiddenField)groupItem.FindControl("hiddenId");
                    var txtComment = (TextBox)groupItem.FindControl("txtComment");
                    var rptQuestions = (Repeater)groupItem.FindControl("rptQuestions");
                    var grp = questionGroupRepository.GetById(Convert.ToInt32(hiddenId.Value));
                    var group = new AnswerGroup();
                    group.AnswerSheetId = answerSheet.Id;
                    group.GroupId = grp.Id;
                    group.Comment = txtComment.Text;
                    answerGroupRepository.SaveOrUpdate(group);
                    foreach (RepeaterItem questionItem in rptQuestions.Items)
                    {
                        var hiddenQId = (HiddenField)questionItem.FindControl("hiddenId");
                        var question = questionRepository.GetById(Convert.ToInt32(hiddenQId.Value));
                        var rptOptions = (Repeater)questionItem.FindControl("rptOptions");
                        var option = new AnswerOption();
                        option.QuestionId = question.Id;
                        option.AnswerSheetId = answerSheet.Id;
                        for (int ii = 0; ii < rptOptions.Items.Count; ii++)
                        {
                            var radOption = (RadioButton)rptOptions.Items[ii].FindControl("radOption");
                            if (radOption.Checked)
                            {
                                option.Option = ii + 1;
                            }
                        }

                        answerOptionRepository.SaveOrUpdate(option);
                    }
                }

                answerSheetRepository.SaveOrUpdate(answerSheet);
                pnlThank.Visible = true;
                pnlFeedback.Visible = false;
                answerOptionRepository.CommitTransaction();
                answerGroupRepository.CommitTransaction();
                answerSheetRepository.CommitTransaction();
            }
            catch (Exception ex)
            {
                answerSheetRepository.RollbackTransaction();
                answerGroupRepository.RollbackTransaction();
                answerOptionRepository.RollbackTransaction();
                //Ghi log
                return;
            }
        }

        protected void btnTiengViet_Click(object sender, EventArgs e)
        {
            Session["lang"] = "vi";
            InitializeCulture();
            hidLang.Value = ((int)LoaiFeedback.TiengViet).ToString();
            txtStartDate.Attributes.Add("placeholder", Resources.Resource.Feedback_ChooseStartDate);
            btnSubmit.Text = Resources.Resource.Feedback_Send;
            ddlCruises.DataTextField = "Name";
            ddlCruises.DataValueField = "Id";
            ddlCruises.Items.Add(new ListItem("-- " + Resources.Resource.Feedback_Select + " --", ""));
            ddlCruises.DataSource = cruiseRepository.GetAll().Where(x => x.Deleted == false);
            ddlCruises.DataBind();
            rptGroups.DataSource = questionGroupRepository.GetAll()
                .Where(x => x.Deleted == false && x.IsInDayboatForm == false && x.LOAI_FEEDBACK == (int)LoaiFeedback.TiengViet)
                .OrderByDescending(x => x.Priority);
            rptGroups.DataBind();
            pnlMain.Visible = true;
            var script = @"
                            $(function(){
                                $('#content').hide();
                                $('#content').fadeIn(500);
                                $('#lang').fadeOut(500);
                                }
                            )";
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "showContent", script, true);
        }

        protected void btnTiengAnh_Click(object sender, EventArgs e)
        {
            Session["lang"] = "en-GB";
            InitializeCulture();
            hidLang.Value= ((int)LoaiFeedback.TiengAnh).ToString();
            txtStartDate.Attributes.Add("placeholder", Resources.Resource.Feedback_ChooseStartDate);
            btnSubmit.Text = Resources.Resource.Feedback_Send;
            ddlCruises.DataTextField = "Name";
            ddlCruises.DataValueField = "Id";
            ddlCruises.Items.Add(new ListItem("-- " + Resources.Resource.Feedback_Select + " --", ""));
            ddlCruises.DataSource = cruiseRepository.GetAll();
            ddlCruises.DataBind();
            rptGroups.DataSource = questionGroupRepository.GetAll()
               .Where(x => x.Deleted == false && x.IsInDayboatForm == false && x.LOAI_FEEDBACK == (int)LoaiFeedback.TiengAnh)
               .OrderByDescending(x => x.Priority);
            rptGroups.DataBind();
            pnlMain.Visible = true;
            var script = @"
                            $(function(){
                                $('#content').hide();
                                $('#content').fadeIn(500);
                                $('#lang').fadeOut(500);
                                }
                            )";
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "showContent", script, true);

        }

        public void ShowError(string message)
        {
            var script = @"
                            $(function () {
                                $('#globalErrorMessage #message').html('" + message + @"');
                                $('#globalErrorMessage').show();
                                setTimeout(function () {
                                    $('#globalErrorMessage').fadeOut(500);
                                }, 3000)
                            });
                            ";
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "showError", script, true);
        }

        public void DungXuLy()
        {
            var script = @"
                            $(function(){
                                $('#content').show();
                                }
                            )";
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "showContent", script, true);
            return;
        }
    }
}
