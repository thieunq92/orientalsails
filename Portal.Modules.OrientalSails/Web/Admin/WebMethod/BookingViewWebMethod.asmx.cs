using CMS.Core.Domain;
using Newtonsoft.Json;
using Portal.Modules.OrientalSails.BusinessLogic;
using Portal.Modules.OrientalSails.BusinessLogic.Share;
using Portal.Modules.OrientalSails.DataTransferObject;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Portal.Modules.OrientalSails.Web.Admin.WebMethod
{
    /// <summary>
    /// Summary description for BookingViewingWebMethod
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class BookingViewWebMethod : System.Web.Services.WebService
    {
        private BookingViewBLL bookingViewBLL;
        private PermissionBLL permissionBLL;
        private UserBLL userBLL;
        public BookingViewBLL BookingViewBLL
        {
            get
            {
                if (bookingViewBLL == null)
                {
                    bookingViewBLL = new BookingViewBLL();
                }
                return bookingViewBLL;
            }
        }
        public UserBLL UserBLL
        {
            get
            {
                if (userBLL == null)
                    userBLL = new UserBLL();
                return userBLL;
            }
        }
        public User CurrentUser
        {
            get
            {
                return UserBLL.UserGetCurrent();
            }
        }
        public PermissionBLL PermissionBLL
        {
            get
            {
                if (permissionBLL == null)
                    permissionBLL = new PermissionBLL();
                return permissionBLL;
            }
        }
        public bool AllowEditBooking
        {
            get
            {
                return true;
               // return PermissionBLL.UserCheckPermission(CurrentUser.Id, (int)PermissionEnum.AllowEditBooking);
            }
        }
        [WebMethod]
        public void CommissionSave(IList<CommissionDTO> listCommissionDTO, int bookingId)
        {
            if (!AllowEditBooking)
            {
                return;
            }
            var listNewCommissionId = new List<int>();
            foreach (var commissionDTO in listCommissionDTO)
            {
                Commission commission = null;
                var amount = 0.0;
                try
                {
                    amount = Double.Parse(commissionDTO.amount);
                }
                catch { }
                var commissionDTOBookingId = 0;
                try
                {
                    commissionDTOBookingId = Convert.ToInt32(commissionDTO.bookingId);
                }
                catch { }
                if (commissionDTO.id <= 0)
                {
                    commission = new Commission();
                }
                else if (commissionDTO.id > 0)
                {
                    commission = BookingViewBLL.CommissionGetById(commissionDTO.id);

                }
                commission.PayFor = commissionDTO.payFor;
                commission.Amount = amount;
                commission.PaymentVoucher = commissionDTO.paymentVoucher;
                commission.Transfer = commissionDTO.transfer;
                commission.Booking = BookingViewBLL.BookingGetById(commissionDTO.bookingId);
                BookingViewBLL.CommissionSaveOrUpdate(commission);
                listNewCommissionId.Add(commission.Id);
            }
            var listCommission = BookingViewBLL.CommissionGetAllByBookingId(bookingId);
            var listIdOfCommission = listCommission.Select(x => x.Id).ToList();
            var listIdOfCommissionDTO = listCommissionDTO.Where(x => x.id > 0).Select(x => x.id).ToList();
            var listCommissionIdNeedRemove = listIdOfCommission.Except(listIdOfCommissionDTO).Except(listNewCommissionId);
            foreach (var commissionIdNeedRemove in listCommissionIdNeedRemove)
            {
                var commissionNeedRemove = BookingViewBLL.CommissionGetById(commissionIdNeedRemove);
                BookingViewBLL.CommissionDelete(commissionNeedRemove);
            }
            Dispose();
        }
        [WebMethod]
        public string CommissionGetAllByBookingId(int bookingId)
        {
            var listCommission = BookingViewBLL.CommissionGetAllByBookingId(bookingId);
            var listCommissionDTO = new List<CommissionDTO>();
            foreach (var commission in listCommission)
            {
                var commissionDTO = new CommissionDTO()
                {
                    id = commission.Id,
                    payFor = commission.PayFor,
                    amount = commission.Amount.ToString("#,##0.##"),
                    bookingId = commission.Booking.Id,
                    paymentVoucher = commission.PaymentVoucher,
                    transfer = commission.Transfer,
                };
                listCommissionDTO.Add(commissionDTO);
            }
            Dispose();
            return JsonConvert.SerializeObject(listCommissionDTO);
        }
        [WebMethod]
        public void ServiceOutsideSave(IList<ServiceOutsideDTO> listServiceOutsideDTO, int bookingId)
        {
            if (!AllowEditBooking)
            {
                return;
            }
            var listNewServiceOutsideId = new List<int>();
            foreach (var serviceOutsideDTO in listServiceOutsideDTO)
            {
                ServiceOutside serviceOutside = null;
                var unitPrice = 0.0;
                try
                {
                    unitPrice = Double.Parse(serviceOutsideDTO.unitPrice);
                }
                catch { }
                var quantity = 0;
                try
                {
                    quantity = Convert.ToInt32(serviceOutsideDTO.quantity);
                }
                catch { }
                var totalPrice = 0.0;
                try
                {
                    totalPrice = Double.Parse(serviceOutsideDTO.totalPrice);
                }
                catch { }
                var serviceOutsideDTOBookingId = 0;
                try
                {
                    serviceOutsideDTOBookingId = Convert.ToInt32(serviceOutsideDTO.bookingId);
                }
                catch { }
                if (serviceOutsideDTO.id <= 0)
                {
                    serviceOutside = new ServiceOutside();
                }
                else if (serviceOutsideDTO.id > 0)
                {
                    serviceOutside = BookingViewBLL.ServiceOutsideGetById(serviceOutsideDTO.id);
                }
                serviceOutside.Service = serviceOutsideDTO.service;
                serviceOutside.UnitPrice = unitPrice;
                serviceOutside.Quantity = quantity;
                serviceOutside.TotalPrice = totalPrice;
                serviceOutside.Booking = BookingViewBLL.BookingGetById(serviceOutsideDTOBookingId);
                serviceOutside.VAT = serviceOutsideDTO.vat;
                BookingViewBLL.ServiceOutsideSaveOrUpdate(serviceOutside);
                listNewServiceOutsideId.Add(serviceOutside.Id);
                ServiceOutsideDetailSave(serviceOutsideDTO, serviceOutside);
            }
            var listServiceOutside = BookingViewBLL.ServiceOutsideGetAllByBookingId(bookingId);
            var listIdOfServiceOutside = listServiceOutside.Select(x => x.Id).ToList();
            var listIdOfServiceOutsideDTO = listServiceOutsideDTO.Where(x => x.id > 0).Select(x => x.id).ToList();
            var listServiceOutsideIdNeedRemove = listIdOfServiceOutside.Except(listIdOfServiceOutsideDTO).Except(listNewServiceOutsideId);
            foreach (var serviceOutsideIdNeedRemove in listServiceOutsideIdNeedRemove)
            {
                var serviceOutsideNeedRemove = BookingViewBLL.ServiceOutsideGetById(serviceOutsideIdNeedRemove);
                BookingViewBLL.ServiceOutsideDelete(serviceOutsideNeedRemove);
            }
            Dispose();
        }
        public void ServiceOutsideDetailSave(ServiceOutsideDTO serviceOutsideDTO, ServiceOutside serviceOutside)
        {
            if (!AllowEditBooking)
            {
                return;
            }
            var listNewServiceOutsideDetailId = new List<int>();
            foreach (var serviceOutsideDetailDTO in serviceOutsideDTO.listServiceOutsideDetailDTO)
            {
                ServiceOutsideDetail serviceOutsideDetail = null;
                var unitPrice = 0.0;
                try
                {
                    unitPrice = Double.Parse(serviceOutsideDetailDTO.unitPrice);
                }
                catch { }
                var quantity = 0;
                try
                {
                    quantity = Convert.ToInt32(serviceOutsideDetailDTO.quantity);
                }
                catch { }
                var totalPrice = 0.0;
                try
                {
                    totalPrice = Double.Parse(serviceOutsideDetailDTO.totalPrice);
                }
                catch { }
                if (serviceOutsideDetailDTO.id <= 0)
                {
                    serviceOutsideDetail = new ServiceOutsideDetail();
                }
                else if (serviceOutsideDetailDTO.id > 0)
                {
                    serviceOutsideDetail = BookingViewBLL.ServiceOutsideDetailGetById(serviceOutsideDetailDTO.id);
                }
                serviceOutsideDetail.Name = serviceOutsideDetailDTO.name;
                serviceOutsideDetail.UnitPrice = unitPrice;
                serviceOutsideDetail.Quantity = quantity;
                serviceOutsideDetail.TotalPrice = totalPrice;
                serviceOutsideDetail.ServiceOutside = serviceOutside;
                BookingViewBLL.ServiceOutsideDetailSaveOrUpdate(serviceOutsideDetail);
                listNewServiceOutsideDetailId.Add(serviceOutsideDetail.Id);
                var listServiceOutsideDetail = BookingViewBLL.ServiceOutsideDetailGetAllByServiceOutsideId(serviceOutside.Id);
                var listIdOfServiceOutsideDetail = listServiceOutsideDetail.Select(x => x.Id).ToList();
                var listIdOfServiceOutsideDetailDTO = serviceOutsideDTO.listServiceOutsideDetailDTO.Where(x => x.id > 0).Select(x => x.id).ToList();
                var listServiceOutsideDetailIdNeedRemove = listIdOfServiceOutsideDetail.Except(listIdOfServiceOutsideDetailDTO).Except(listNewServiceOutsideDetailId);
                foreach (var serviceOutsideDetailIdNeedRemove in listServiceOutsideDetailIdNeedRemove)
                {
                    var serviceOutsideDetailNeedRemove = BookingViewBLL.ServiceOutsideDetailGetById(serviceOutsideDetailIdNeedRemove);
                    BookingViewBLL.ServiceOutsideDetailDelete(serviceOutsideDetailNeedRemove);
                }
                Dispose();
            }
        }
        [WebMethod]
        public string ServiceOutsideGetAllByBookingId(int bookingId)
        {
            var listServiceOutside = BookingViewBLL.ServiceOutsideGetAllByBookingId(bookingId);
            var listServiceOutsideDTO = new List<ServiceOutsideDTO>();
            foreach (var serviceOutside in listServiceOutside)
            {
                var serviceOutsideDTO = new ServiceOutsideDTO()
                {
                    id = serviceOutside.Id,
                    service = serviceOutside.Service,
                    unitPrice = serviceOutside.UnitPrice.ToString("#,##0.##"),
                    quantity = serviceOutside.Quantity,
                    totalPrice = serviceOutside.TotalPrice.ToString("#,##0.##"),
                    bookingId  = serviceOutside.Booking.Id,
                    vat = serviceOutside.VAT,
                };
                listServiceOutsideDTO.Add(serviceOutsideDTO);
            }
            Dispose();
            return JsonConvert.SerializeObject(listServiceOutsideDTO);
        }
        [WebMethod]
        public string ServiceOutsideDetailGetAllByServiceOutsideId(int serviceOutsideId)
        {
            var listServiceOutsideDetail = BookingViewBLL.ServiceOutsideDetailGetAllByServiceOutsideId(serviceOutsideId);
            var listServiceOutsideDetailDTO = new List<ServiceOutsideDetailDTO>();
            foreach (var serviceOutsideDetail in listServiceOutsideDetail)
            {
                var serviceOutsideDetailDTO = new ServiceOutsideDetailDTO()
                {
                    id = serviceOutsideDetail.Id,
                    name = serviceOutsideDetail.Name,
                    unitPrice = serviceOutsideDetail.UnitPrice.ToString("#,##0.##"),
                    quantity = serviceOutsideDetail.Quantity,
                    totalPrice = serviceOutsideDetail.TotalPrice.ToString("#,##0.##"),
                };
                listServiceOutsideDetailDTO.Add(serviceOutsideDetailDTO);
            }
            Dispose();
            return JsonConvert.SerializeObject(listServiceOutsideDetailDTO);
        }
        public new void Dispose()
        {
            if (bookingViewBLL != null)
            {
                bookingViewBLL.Dispose();
                bookingViewBLL = null;
            }

            if(userBLL != null)
            {
                userBLL.Dispose();
                userBLL = null;
            }

            if(permissionBLL != null)
            {
                permissionBLL.Dispose();
                permissionBLL = null;
            }
        }
    }
}
