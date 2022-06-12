using System;

namespace Portal.Modules.OrientalSails.Domain
{
    //Lớp model cho bảng os_AgencyIssue
    public class AgencyIssue
    {    
        public virtual int Id { get; set; }
        public virtual AgencyContract AgencyContract { get; set; }
        public virtual QQuotation QQuotation { get; set; }
        public virtual QCruiseGroup GroupCruise { get; set; }
        public virtual string AgentLevelCode { get; set; }
    }
}
