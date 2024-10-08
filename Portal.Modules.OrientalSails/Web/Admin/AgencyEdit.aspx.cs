using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CMS.Core.Domain;
using CMS.ServerControls.FileUpload;
using CMS.Web.Admin.Controls;
using CMS.Web.Util;
using NHibernate.Criterion;
using log4net;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.BusinessLogic;
using System.Linq;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class AgencyEdit : SailsAdminBase
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(AgencyEdit));
        private AgencyEditBLL agencyEditBLL;
        public AgencyEditBLL AgencyEditBLL
        {
            get
            {
                if (agencyEditBLL == null)
                {
                    agencyEditBLL = new AgencyEditBLL();
                }
                return agencyEditBLL;
            }
        }
        private Agency _agency;
        private DateTime _current;
        private IList _roles;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (Request.QueryString["AgencyId"] != null)
                    {
                        if (!Module.PermissionCheck(Permission.ACTION_EDITAGENCY, UserIdentity))
                        {

                            buttonSave.Visible = false;

                        }
                    }

                    if (Request.QueryString["AgencyId"] == null)
                    {
                        buttonSave.Visible = true;
                    }
                    ddlAgentLevel.DataSource = Module.GetAgentLevel();
                    ddlAgentLevel.DataTextField = "Name";
                    ddlAgentLevel.DataValueField = "Id";
                    ddlAgentLevel.DataBind();

                    ddlAgencyRoles.DataSource = AgencyEditBLL.RoleGetAll();
                    ddlAgencyRoles.DataTextField = "Name";
                    ddlAgencyRoles.DataValueField = "Id";
                    ddlAgencyRoles.DataBind();

                    Role role = AgencyEditBLL.RoleGetById(21);
                    ddlSales.DataSource = role.Users.Cast<User>().Where(x => x.IsActive == true);
                    ddlSales.DataTextField = "FullName";
                    ddlSales.DataValueField = "Id";
                    ddlSales.DataBind();

                    ddlPaymentPeriod.DataSource = Enum.GetNames(typeof(PaymentPeriod));
                    ddlPaymentPeriod.DataBind();

                    ddlLocations.DataSource = AgencyEditBLL.AgencyLocationGetAll();
                    ddlLocations.DataTextField = "Name";
                    ddlLocations.DataValueField = "Id";
                    ddlLocations.DataBind();
                    LoadInfo();

                    if (Request.QueryString["agencyid"] != null)
                    {
                        // Sale in charge không phải user hiện tại và không có quyền edit sale in charge
                        if (_agency.Sale != UserIdentity && !Module.PermissionCheck(Permission.EDIT_SALE_IN_CHARGE, UserIdentity))
                        {
                            ddlSales.Enabled = false;
                            txtSaleStart.Enabled = false;
                        }
                    }
                    else
                    {

                    }

                }
            }
            catch (Exception ex)
            {
            }
        }

        protected void buttonSave_Click(object sender, EventArgs e)
        {
            bool isAddnew = false;
            if (Request.QueryString["AgencyId"] != null)
            {
                _agency = Module.AgencyGetById(Convert.ToInt32(Request.QueryString["AgencyId"]));
            }
            else
            {
                _agency = new Agency();
                isAddnew = true;
            }
            _agency.Name = textBoxName.Text;
            _agency.Phone = txtPhone.Text;
            _agency.Address = txtAddress.Text;
            _agency.TradingName = txtTradingName.Text;
            _agency.Representative = txtRepresentative.Text;
            _agency.RepresentativePosition = txtRepresentativePosition.Text;
            _agency.Contact = txtContact.Text;
            _agency.ContactAddress = txtContactAddress.Text;
            _agency.ContactEmail = txtContactEmail.Text;
            _agency.ContactPosition = txtContactPosition.Text;
            _agency.Website = txtWebsite.Text;
            _agency.ContractAddress = txtContractAddress.Text;
            _agency.ContractPhone = txtContractPhone.Text;
            _agency.ContractTaxCode = txtContractTaxCode.Text;
            if (ddlAgencyRoles.SelectedIndex != 0)
                _agency.Role = Module.RoleGetById(Convert.ToInt32(ddlAgencyRoles.SelectedValue));
            else
                _agency.Role = null;

            if (_agency.Role != null && _agency.Role.Id == 20)
            {
                _agency.AgencyType = "Guide";
            }
            if (!string.IsNullOrWhiteSpace(ddlAgentLevel.SelectedValue))
                _agency.QAgentlevel = Module.GetById<QAgentLevel>(Convert.ToInt32(ddlAgentLevel.SelectedValue));
            _agency.Email = txtEmail.Text;
            _agency.TaxCode = txtTaxCode.Text;
            _agency.Description = txtDescription.Text;
            //_agency.ContractStatus = Convert.ToInt32(ddlContractStatus.SelectedValue);

            _agency.PaymentPeriod = (PaymentPeriod)Enum.Parse(typeof(PaymentPeriod), ddlPaymentPeriod.SelectedValue);


            User oldsale = _agency.Sale;
            DateTime? oldStart = _agency.SaleStart;

            if (!string.IsNullOrEmpty(txtSaleStart.Text))
            {
                _agency.SaleStart = DateTime.ParseExact(txtSaleStart.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                _agency.SaleStart = null;
            }
            if (ddlSales.Items.Count > 0 && ddlSales.SelectedIndex > 0)
            {
                _agency.Sale = Module.UserGetById(Convert.ToInt32(ddlSales.SelectedValue));
            }
            else
            {
                _agency.Sale = null;
            }


            if (_agency.Id <= 0)
            {
                _agency.CreatedBy = UserIdentity;
                _agency.CreatedDate = DateTime.Now;
            }
            else
            {
                _agency.ModifiedBy = UserIdentity;
                _agency.ModifiedDate = DateTime.Now;
            }

            if (ddlLocations.SelectedIndex > 0)
            {
                _agency.Location = Module.GetObject<AgencyLocation>(Convert.ToInt32(ddlLocations.SelectedValue));
            }
            else
            {
                _agency.Location = null;
            }

            Module.SaveOrUpdate(_agency);

            //foreach (RepeaterItem item in rptCruises.Items)
            //{
            //    var hiddenCruiseId = item.FindControl("hiddenCruiseId") as HiddenField;
            //    var ddlRoles = item.FindControl("ddlRoles") as DropDownList;
            //    if (hiddenCruiseId != null && ddlRoles != null)
            //    {
            //        Cruise cruise = Module.CruiseGetById(Convert.ToInt32(hiddenCruiseId.Value));
            //        CruiseRole role = Module.GetCruiseRole(cruise, _agency);
            //        role.Cruise = cruise;
            //        role.Agency = _agency;
            //        role.Role = Module.RoleGetById(Convert.ToInt32(ddlRoles.SelectedValue));
            //        Module.SaveOrUpdate(role);
            //    }
            //}

            // Nếu chưa có lịch sử thì lưu lại
            if (_agency.History.Count == 0 && oldsale != null)
            {
                AgencyHistory history = new AgencyHistory();
                history.Agency = _agency;
                history.Sale = oldsale;
                if (oldStart.HasValue)
                {
                    history.ApplyFrom = oldStart.Value;
                }
                else
                {
                    history.ApplyFrom = new DateTime(2000, 1, 1);
                }
                Module.SaveOrUpdate(history);
            }

            if ((oldsale != _agency.Sale || oldStart != _agency.SaleStart) && _agency.SaleStart != null)
            {
                // Khi có sự thay đổi về sale và ngày áp dụng thì lưu lại lịch sử

                // Nhưng nếu đã có ngày áp dụng này rồi thì lưu lại theo sale mới
                AgencyHistory history = null;
                foreach (AgencyHistory oldhis in _agency.History)
                {
                    if (oldhis.ApplyFrom == _agency.SaleStart.Value)
                    {
                        history = oldhis;
                        break;
                    }
                }
                if (history == null)
                    history = new AgencyHistory();
                history.Agency = _agency;
                history.Sale = _agency.Sale;
                history.ApplyFrom = _agency.SaleStart.Value;
                Module.SaveOrUpdate(history);
            }

            if (isAddnew)
            {
                PageRedirect(string.Format("AgencyEdit.aspx?NodeId={0}&SectionId={1}&AgencyId={2}", Node.Id, Section.Id, _agency.Id));
            }
            else
            {
                PageRedirect(string.Format("AgencyList.aspx?NodeId={0}&SectionId={1}", Node.Id, Section.Id));
            }
        }

        public void LoadInfo()
        {
            if (Request.QueryString["AgencyId"] != null)
            {
                _agency = Module.AgencyGetById(Convert.ToInt32(Request.QueryString["AgencyId"]));

                if (_agency.Sale != null && _agency.Sale.Id == UserIdentity.Id)
                {
                    buttonSave.Visible = true;
                }
                if (_agency.QAgentlevel != null) ddlAgentLevel.SelectedValue = _agency.QAgentlevel.Id.ToString();

                textBoxName.Text = _agency.Name;
                txtPhone.Text = _agency.Phone;
                txtAddress.Text = _agency.Address;

                txtEmail.Text = _agency.Email;
                txtTaxCode.Text = _agency.TaxCode;
                txtDescription.Text = _agency.Description;
                //ddlContractStatus.SelectedValue = _agency.ContractStatus.ToString();

                txtRepresentative.Text = _agency.Representative;
                txtTradingName.Text = _agency.TradingName;
                txtRepresentativePosition.Text = _agency.RepresentativePosition;
                txtContact.Text = _agency.Contact;
                txtContactAddress.Text = _agency.ContactAddress;
                txtContactPosition.Text = _agency.ContactPosition;
                txtContactEmail.Text = _agency.ContactEmail;
                txtWebsite.Text = _agency.Website;
                txtContractTaxCode.Text = _agency.ContractTaxCode;
                txtContractAddress.Text = _agency.ContractAddress;
                txtContractPhone.Text = _agency.ContractPhone;

                if (_agency.CreatedBy != null && _agency.CreatedDate.HasValue)
                {
                    litCreated.Text = string.Format("Created by {0} at {1:dd/MM/yyyy HH:MM}", _agency.CreatedBy.FullName, _agency.CreatedDate.Value);
                }
                if (_agency.ModifiedBy != null && _agency.ModifiedDate.HasValue)
                {
                    litModified.Text = string.Format("and last edited by {0} at {1:dd/MM/yyyy HH:MM}", _agency.ModifiedBy.FullName, _agency.ModifiedDate.Value);
                }
                ddlPaymentPeriod.SelectedValue = _agency.PaymentPeriod.ToString();

                if (_agency.SaleStart.HasValue)
                {
                    txtSaleStart.Text = _agency.SaleStart.Value.ToString("dd/MM/yyyy");
                }

                if (ddlSales.Items.Count > 0)
                {
                    if (_agency.Sale != null)
                    {
                        ddlSales.SelectedValue = _agency.Sale.Id.ToString();
                    }
                    else
                    {
                        ddlSales.SelectedIndex = 0;
                    }
                }

                if (_agency.Role != null)
                    ddlAgencyRoles.SelectedValue = _agency.Role.Id.ToString();
                else
                    ddlAgencyRoles.SelectedIndex = 0;

                foreach (AgencyHistory history in _agency.History)
                {
                    if (history.ApplyFrom > _current && history.ApplyFrom < DateTime.Today)
                    {
                        _current = history.ApplyFrom;
                    }
                }

                rptHistory.DataSource = _agency.History;
                rptHistory.DataBind();

                if (_agency.Location != null)
                {
                    ddlLocations.SelectedValue = _agency.Location.Id.ToString();
                }
            }
            else
            {

            }
        }

        protected void rptHistory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is AgencyHistory)
            {
                AgencyHistory history = (AgencyHistory)e.Item.DataItem;

                Literal litSale = e.Item.FindControl("litSale") as Literal;
                if (litSale != null)
                {
                    if (history.Sale != null)
                    {
                        litSale.Text = history.Sale.FullName;
                    }
                    else
                    {
                        litSale.Text = "Unbound Sales";
                    }
                }

                Literal litSaleStart = e.Item.FindControl("litSaleStart") as Literal;
                if (litSaleStart != null)
                {
                    litSaleStart.Text = history.ApplyFrom.ToString("dd/MM/yyyy");
                }

                if (history.ApplyFrom == _current)
                {
                    HtmlTableRow trLine = e.Item.FindControl("trLine") as HtmlTableRow;
                    if (trLine != null)
                    {
                        trLine.Attributes.Add("style", "font-weight: bold; color: red;");
                    }
                }
            }
        }
    }
}
