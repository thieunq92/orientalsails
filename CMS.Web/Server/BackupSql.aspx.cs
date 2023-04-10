using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CMS.Web.Server
{
    public partial class BackupSql : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Microsoft.SqlServer.Management.Smo.Server myServer = new Microsoft.SqlServer.Management.Smo.Server(@"203.113.174.12\MSSQLSERVER2019,1437");
            myServer.ConnectionContext.LoginSecure = false;
            myServer.ConnectionContext.Login = "orientalsails";
            myServer.ConnectionContext.Password = "4y7W75^kt";
            myServer.ConnectionContext.Connect();

            Backup bkpDBFull = new Backup();
            bkpDBFull.Action = BackupActionType.Database;
            bkpDBFull.Database = "orientalsails";
            bkpDBFull.Devices.AddDevice(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "orientalsails.bak"), DeviceType.File);
            bkpDBFull.BackupSetName = "orientalsails";
            bkpDBFull.BackupSetDescription = "moos database - Full Backup";
            bkpDBFull.ExpirationDate = DateTime.Today.AddDays(360);
            bkpDBFull.Initialize = false;
            bkpDBFull.SqlBackup(myServer);
        }
    }
}