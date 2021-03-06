using System;
using CMS.Core.Domain;

namespace Portal.Modules.OrientalSails.Domain
{
    //Lớp model cho bảng os_AgencyHistory
    //AgencyHistory để chứa thông tin về lịch sử các sales in charge  
    public class AgencyHistory
    {
        public virtual int Id { get; set; }

        public virtual Agency Agency { get; set; }//Agency mà sales được nhận

        public virtual User Sale { get; set; }//Sales in charge 

        public virtual DateTime ApplyFrom { get; set; }//Ngày sales nhận agency
    }
}
