using System;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using CMS.Core;
using CMS.Core.Domain;
using CMS.Core.Search;
using CMS.Core.Util;
using CMS.Web.Components;
using CMS.Web.Util;

namespace CMS.Web.UI
{
    /// <summary>
    /// Summary description for ModuleAdminBasePage.
    /// </summary>
    public class KitModuleAdminBasePage : KitGenericBasePage
    {
        #region -- PRIVATE MEMBERS --

        private CultureInfo _currentUICulture;
        private readonly bool _localizable;
        protected readonly ModuleLoader _moduleLoader;
        private ResourceManager _resMan;
        protected IDictionary _systemSettings;
        protected IDictionary _moduleSettings;

        /// <summary>
        /// Cờ trạng thái redirect của page
        /// </summary>
        private bool _isRedirected;

        protected ModuleBase _module;
        private Node _node;
        private Section _section;

        #endregion

        #region -- CONSTRUCTORS --

        /// <summary>
        /// Default constructor calls base constructor with parameters for templatecontrol, 
        /// templatepath and stylesheet.
        /// </summary>
        public KitModuleAdminBasePage()
        {
            _node = null;
            _section = null;

            _moduleLoader = Container.Resolve<ModuleLoader>();
            //Mặc định là chưa redirect
            _isRedirected = false;

            #region Localize

            // Base name of the resources consists of Namespace.Resources.Strings
            string baseName = GetType().BaseType.Namespace + ".Resources";
            _resMan = new ResourceManager(baseName, GetType().BaseType.Assembly);
            try
            {
                _resMan.GetString("Test");
                _localizable = true;
            }
            catch (MissingManifestResourceException)
            {
                _localizable = false;
            }
            _currentUICulture = Thread.CurrentThread.CurrentUICulture;

            #endregion
        }

        public void LoadRes(Type type)
        {
            _resMan = ResourceManager.CreateFileBasedResourceManager("Resources", "D:\\", type);
        }

        #endregion

        #region -- PROPERTIES --

        public User UserIdentity
        {
            get { return User.Identity as User; }
        }

        protected virtual bool CanByPassCanEditCheck()
        {
            return false;
        }

        public IDictionary SystemSettings
        {
            get
            {
                try
                {
                    if (_systemSettings == null)
                    {
                        _systemSettings = new Hashtable();
                        IList list = CoreRepository.GetSystemSettings();
                        foreach (SystemSetting setting in list)
                        {
                            _systemSettings.Add(setting.Key, setting.Value);
                        }
                    }
                }
                catch
                {
                    _systemSettings = new Hashtable();
                }
                return _systemSettings;
            }
        }

        public IDictionary ModuleSettings
        {
            get
            {
                try
                {
                    if (_moduleSettings == null)
                    {
                        _moduleSettings = new Hashtable();
                        IList list = CoreRepository.GetModuleSettings(Section.ModuleType);
                        foreach (SystemSetting setting in list)
                        {
                            _moduleSettings.Add(setting.Key, setting.Value);
                        }
                    }
                }
                catch
                {
                    _moduleSettings = new Hashtable();
                }
                return _moduleSettings;
            }
        }

        #endregion

        #region -- METHODS --

        #region -- Localize --

        /// <summary>
        /// Get a localized text string for a given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual string GetText(string key)
        {
            return _resMan.GetString(key, _currentUICulture);
        }

        /// <summary>
        /// Try to localize the controls. 
        /// The resource keys should consist of the name of the User Control class
        /// with the ID of the control added. For example "Articles.lblTitle".
        /// </summary>
        protected virtual void LocalizeControls()
        {
            foreach (Control control in Controls)
            {
                LocalizeControls(control);
            }
        }

        /// <summary>
        /// Try to localize the controls. 
        /// The resource keys should be the same as the ID of the control that should be translated.
        /// You can override this behavior by adding a prefix with the UserControl name and a 
        /// semicolon, for example: "Articles:lblTitle".
        /// </summary>
        /// <param name="control"></param>
        protected virtual void LocalizeControls(Control control)
        {
            foreach (Control childControl in control.Controls)
            {
                // First try to find a string for this specific user control.
                // Use the name of BaseType because we need the code-behind class.
                string resourceKey = GetType().BaseType.Name + ":" + childControl.ID;
                string localizedText = GetText(resourceKey);

                if (localizedText == null)
                {
                    // No translation found with the user control prefix. Try to find a translation that
                    // only has the control ID as key
                    if (childControl.ID != null)
                    {
                        localizedText = GetText(childControl.ID);
                    }
                }

                if (!String.IsNullOrEmpty(localizedText))
                {
                    if ((childControl is Label) && !(childControl is BaseValidator))
                    {
                        Label label = (Label)childControl;
                        label.Text = localizedText;
                    }
                    if (childControl is Button)
                    {
                        Button button = (Button)childControl;
                        button.Text = localizedText;
                    }
                    if (childControl is LinkButton)
                    {
                        LinkButton linkButton = (LinkButton)childControl;
                        linkButton.Text = localizedText;
                    }
                    if (childControl is HyperLink)
                    {
                        HyperLink hyperLink = (HyperLink)childControl;
                        hyperLink.Text = localizedText;
                    }
                    if (childControl is RadioButton)
                    {
                        RadioButton radioButton = (RadioButton)childControl;
                        radioButton.Text = localizedText;
                    }
                    else if (childControl is BaseValidator)
                    {
                        BaseValidator validator = (BaseValidator)childControl;
                        validator.ErrorMessage = localizedText;
                    }
                    else if (childControl is CheckBox)
                    {
                        CheckBox checkBox = (CheckBox)childControl;
                        checkBox.Text = localizedText;
                    }
                    else if (childControl is ConfirmButtonExtender)
                    {
                        ConfirmButtonExtender confirmExtender = (ConfirmButtonExtender)childControl;
                        confirmExtender.ConfirmText = localizedText;
                    }
                }
                // Recursive translate childcontrols
                LocalizeControls(childControl);
            }
        }

        /// <summary>
        /// Recursively databind controls that might have localized texts.
        /// </summary>
        protected virtual void BindResources()
        {
            BindResources(this);
        }

        private void BindResources(Control control)
        {
            foreach (Control childControl in control.Controls)
            {
                if (childControl is Label || childControl is Button || childControl is BaseValidator)
                {
                    childControl.DataBind();
                }
                BindResources(childControl);
            }
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            if (!IsPostBack && _localizable)
            {
                LocalizeControls();
            }
        }

        #endregion

        #region -- PROTECTED AND PUBLIC PROPERTIES --

        /// <summary>
        /// Property Node (Node)
        /// </summary>
        public Node Node
        {
            get { return _node; }
        }

        /// <summary>
        /// Property Section (Section)
        /// </summary>
        public Section Section
        {
            get { return _section; }
        }

        /// <summary>
        /// Property Module (ModuleBase)
        /// </summary>
        public ModuleBase Module
        {
            get { return _module; }
        }

        #endregion

        #region -- OVERRIDE --

        /// <summary>
        /// In the OnInit method the Node and Section of every ModuleAdminPage is set. 
        /// An exception is thrown when one of them cannot be set.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            if ((!Page.Request.ContentType.Contains("json") && !Page.Request.ContentType.Contains("multipart/form-data")) ||
                IsPostBack)
            {
                try
                {
                    //if (!string.isnullorempty(context.request.querystring["nodeid"]))
                    //{
                    int nodeId = 1;
                    try
                    {
                        nodeId = Int32.Parse(Context.Request.QueryString["NodeId"]);
                    }
                    catch (Exception) { }

                    _node = (Node)CoreRepository.GetObjectById(typeof(Node), nodeId);

                    int sectionId = 15;
                    try
                    {
                        sectionId = Int32.Parse(Context.Request.QueryString["SectionId"]);
                    }
                    catch (Exception) { }

                    _section = (Section)CoreRepository.GetObjectById(typeof(Section), sectionId);
                    _module = _moduleLoader.GetModuleFromSection(_section);

                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(_node.Culture);
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(_node.Culture);
                    _currentUICulture = Thread.CurrentThread.CurrentUICulture;
                    //}
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "Unable to initialize the Module Admin page because a Node or a Section could not be created.",
                        ex);
                }
                // Check permissions
                if (!Context.User.Identity.IsAuthenticated)
                {
                    Response.Redirect(string.Format("/Login.aspx?ReturnUrl={0}", Request.RawUrl.Replace("&", "%26")));
                    throw new AccessForbiddenException("You are not logged in.");
                }
                User user = Context.User.Identity as User;
                if (user == null)
                {
                    return;
                }
                if (!CanByPassCanEditCheck() && !user.CanEdit(_section))
                {
                    Response.Redirect("/Error.aspx");
                    throw new ActionForbiddenException("You are not allowed to edit the section.");
                }

                // Optional indexing event handlers
                if (_module is ISearchable
                    && Boolean.Parse(Config.GetConfiguration()["InstantIndexing"]))
                {
                    ISearchable searchableModule = (ISearchable)_module;
                    searchableModule.ContentCreated += searchableModule_ContentCreated;
                    searchableModule.ContentUpdated += searchableModule_ContentUpdated;
                    searchableModule.ContentDeleted += searchableModule_ContentDeleted;
                }

                // Set FCKEditor context (used by some module admin pages)
                // It would be nicer if we could do this in the Global.asax, but there the 
                // ultra-convenient ~/Path (ResolveUrl) isn't available :).
                string userFilesPath = Config.GetConfiguration()["FCKeditor:UserFilesPath"];
                if (userFilesPath != null && HttpContext.Current.Application["FCKeditor:UserFilesPath"] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application["FCKeditor:UserFilesPath"] = ResolveUrl(userFilesPath);
                    HttpContext.Current.Application.UnLock();
                }
            }

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            #region -- Ajax enabled --

            if (Page.Request.ContentType.Contains("json"))
            {
                return;
            }

            #endregion

            #region Set Section, Node, Back button hyperlink

            if (Page is KitModuleAdminBasePage && Master != null)
            {
                KitModuleAdminBasePage adminPage = (KitModuleAdminBasePage)Page;

                if (Module != null && adminPage.Node != null)
                {
                    HyperLink hplBack = Master.FindControl("hplBack") as HyperLink;
                    if (hplBack != null)
                    {
                        hplBack.NavigateUrl = UrlHelper.GetUrlFromNode(adminPage.Node);
                    }

                    Label lblNode = Master.FindControl("lblNode") as Label;
                    if (lblNode != null)
                    {
                        lblNode.Text = adminPage.Node.Title;
                    }

                    Label lblSection = Master.FindControl("lblSection") as Label;
                    if (lblSection != null)
                    {
                        lblSection.Text = adminPage.Section.Title;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Override lại hàm RaisePostBackEvent, chỉ gọi đến các hàm sự kiện khi trang chưa bị
        /// redirect, nếu đã redirect, bỏ qua để tiết kiệm bộ nhớ
        /// </summary>
        /// <param name="sourceControl"></param>
        /// <param name="eventArgument"></param>
        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            if (_isRedirected == false)
                base.RaisePostBackEvent(sourceControl, eventArgument);
        }

        /// <summary>
        /// Override lại hàm Render, chỉ render sang html và gửi cho client khi trang chưa bị redirect
        /// Nếu đã redirect, bỏ qua để tiết kiệm bộ nhớ
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (_isRedirected == false)
                base.Render(writer);
        }

        #endregion

        #region -- PRIVATE METHODS --

        /// <summary>
        /// Redirect đến trang khác, sử dụng phương pháp tiết kiệm tài nguyên hơn Response.Redirect
        /// </summary>
        /// <param name="url">Đường dẫn cần redirect tới</param>
        public void PageRedirect(string url)
        {
            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            _isRedirected = true;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetBaseQueryString()
        {
            return String.Format("?NodeId={0}&SectionId={1}", Node.Id, Section.Id);
        }

        #region -- Searchable --

        private void IndexContent(SearchContent searchContent, IndexAction action)
        {
            // Index
            string indexDir = Context.Server.MapPath(Config.GetConfiguration()["SearchIndexDir"]);
            IndexBuilder ib = new IndexBuilder(indexDir, false);
            switch (action)
            {
                case IndexAction.Create:
                    ib.AddContent(searchContent);
                    break;
                case IndexAction.Update:
                    ib.UpdateContent(searchContent);
                    break;
                case IndexAction.Delete:
                    ib.DeleteContent(searchContent);
                    break;
            }
            ib.Close();
        }

        private void searchableModule_ContentCreated(object sender, IndexEventArgs e)
        {
            IndexContent(e.SearchContent, IndexAction.Create);
        }

        private void searchableModule_ContentUpdated(object sender, IndexEventArgs e)
        {
            IndexContent(e.SearchContent, IndexAction.Update);
        }

        private void searchableModule_ContentDeleted(object sender, IndexEventArgs e)
        {
            IndexContent(e.SearchContent, IndexAction.Delete);
        }

        #endregion

        private enum IndexAction
        {
            Create,
            Update,
            Delete
        }

        #endregion
    }
}