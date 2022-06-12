using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CMS.Core.Domain;

namespace CMS.Web.AdminArea.DAL.Domain
{
    public class AnswerSheet
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Address { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime? Date { get; set; }
        public virtual bool IsSent { get; set; }
        public virtual string Guide { get; set; }
        public virtual string Driver { get; set; }
        public virtual string RoomNumber { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual int CruiseId { get; set; }
        public virtual int ROOM_ID { get; set; }
        public virtual int LOAI_FEEDBACK { get; set; }
        public virtual string SO_DIEN_THOAI { get; set; }
    }
}
