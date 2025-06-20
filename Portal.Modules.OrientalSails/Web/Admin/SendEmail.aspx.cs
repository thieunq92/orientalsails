using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.Web.Util;

namespace Portal.Modules.OrientalSails.Web.Admin
{
	public partial class SendEmailPage : SailsAdminBasePage
	{
		//private const string NO_EMAIL = "Unable to obtain email address";
		private const string APPROVED_SUBJECT = "Approved for your booking in {0:dd/MM/yyyy}";
		private const string REJECTED_SUBJECT = "Booking in {0:dd/MM/yyyy} rejected";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				Booking booking = Module.BookingGetById(Convert.ToInt32(Request.QueryString["BookingId"]));
				if (!string.IsNullOrEmpty(booking.Email))
				{
					lblEmailTo.Text = booking.Email;
				}
				else if (booking.Booker != null && !string.IsNullOrEmpty(booking.Booker.Email))
				{
					lblEmailTo.Text = booking.Booker.Email;
				}
				else if (booking.Agency != null && !string.IsNullOrEmpty(booking.Agency.Email))
				{
					lblEmailTo.Text = booking.Agency.Email;
				}
				else
				{
					lblEmailTo.Text = booking.CreatedBy.Email;
				}

				string[] data = new string[21];
				if (booking.Agency != null)
				{
					data[0] = booking.Agency.Name;
				}
				else
				{
					data[0] = SailsModule.NOAGENCY;
				}
				data[1] = booking.AgencyCode;
				data[2] = string.Format(BookingFormat, booking.Id);
				data[3] = booking.Trip.Name;
				data[4] = booking.StartDate.ToString("dd/MM/yyyy");
				data[16] = booking.EndDate.ToString("dd/MM/yyyy");
				data[5] = booking.Pax.ToString(CultureInfo.InvariantCulture);
				data[6] = booking.CustomerName;
				data[7] = booking.RoomName;
				if (booking.Transfer_BusType != null)
				{
					data[8] = booking.PickupAddress;
				}
				else
				{
					switch (booking.Cruise.Code)
					{
						case "OS1":
							data[17] = "Thời gian có mặt: 12h - 12h15(đến đúng giờ)</br>Địa chỉ : Quầy vé số 28<br/>Cảng hành khách quốc tế Hạ Long, số 9, Hạ Long, Bãi Cháy. Halong international cruise port -No 9 Halong road, Bai Chay ward, Halong city, Quang Ninh province<br/>Check -in time: 12 - 12:15 pm<br/>Liên hệ tại bến Ms Hằng 0965139022";
							data[18] = "Liên hệ tại bến Ms Hằng 0965139022<br/>Hotline +6công ty: 0243 926 4009";
							break;
						case "OS2":
							data[17] = "Thời gian có mặt: 12h - 12h15(đến đúng giờ)</br>Địa chỉ : Quầy vé số 28<br/>Cảng hành khách quốc tế Hạ Long, số 9, Hạ Long, Bãi Cháy. Halong international cruise port -No 9 Halong road, Bai Chay ward, Halong city, Quang Ninh province<br/>Check -in time: 12 - 12:15 pm<br/>Liên hệ tại bến Ms Hằng 0965139022";
							data[18] = "Liên hệ tại bến Ms Hằng 0965139022<br/>Hotline công ty: 0243 926 4009";
							break;
						case "OS3":
							data[17] = "Thời gian có mặt: 12h - 12h15(đến đúng giờ)</br>Địa chỉ : Quầy vé số 28<br/>Cảng hành khách quốc tế Hạ Long, số 9, Hạ Long, Bãi Cháy. Halong international cruise port -No 9 Halong road, Bai Chay ward, Halong city, Quang Ninh province<br/>Check -in time: 12 - 12:15 pm<br/>Liên hệ tại bến Ms Hằng 0965139022";
							data[18] = "Liên hệ tại bến Ms Hằng 0965139022<br/>Hotline công ty: 0243 926 4009";
							break;
						case "NCL1":
							data[17] = "Thời gian có mặt: 11:45</br>Địa chỉ : Lô 27 - Bến Cảng Tuần Châu. Block 27, Tuan Chau International Marina<br/>Lễ tân nhà chờ : Ms Lan 035 223 3222<br/>Quản lý : Mr Thơi 0988 775 128";
							data[18] = "Lễ tân nhà chờ : Ms Lan 035 223 3222<br/>Quản lý : Mr Thơi 0988 775 128";
							break;
						case "NCL2":
							data[17] = "Thời gian có mặt: 11:45</br>Địa chỉ : Lô 27 - Bến Cảng Tuần Châu. Block 27, Tuan Chau International Marina<br/>Lễ tân nhà chờ : Ms Lan 035 223 3222<br/>Quản lý : Mr Thơi 0988 775 128";
							data[18] = "Lễ tân nhà chờ : Ms Lan 035 223 3222<br/>Quản lý : Mr Thơi 0988 775 128";
							break;
						case "STL":
							data[17] = "Thời gian có mặt: 12:00 - 12:15</br>Địa chỉ : Quầy vé số 28<br/> Cảng Tàu Khách Quốc Tế Hạ Long, số 9 Hạ Long, phường Bãi Cháy, thành phố Hạ Long, Quảng Ninh. Halong international cruise port -No 9 Halong road, Bai Chay ward, Halong city, Quang Ninh province<br/>Check -in time: 12 - 12:15 pm<br/>Liên hệ: Mr.Xây quản lý 0943.567.408 / Ms Hằng 0965139022";
							data[18] = "Liên hệ: Mr.Xây quản lý 0943.567.408 / Ms Hằng 0965139022<br/>Hotline công ty: 0243 926 4009";
							break;
						case "VD":
							if (booking.Trip.TripCode == "4HCR")
							{
								data[17] = "V-Dream<br/>Thông tin đón bến Du thuyền V Dream:<br/>Thời gian có mặt: 07:15 - 07:30 tại Cảng Tàu Khách Quốc Tế Hạ Long, số 9 Hạ Long, phường Bãi Cháy, thành phố Hạ Long, Quảng Ninh.<br/>Check -in time: 07:15 - 07:30 at Halong international cruise port -No 9 Halong road, Bai Chay ward, Halong city, Quang Ninh province<br/>https://maps.app.goo.gl/8cg6PSLWAEKdg5SJ6?g_st=iz";
								data[18] = "Liên hệ: Ms. Dịu 0353516516<br/>Mr Nam quản lý: 0352642787";
							}
							if (booking.Trip.TripCode == "6HCR")
							{
								data[17] = "V-Dream<br/>Thông tin đón bến Du thuyền V Dream:<br/>Thời gian có mặt: 11:15 - 11:30 tại Cảng Tàu Khách Quốc Tế Hạ Long, số 9 Hạ Long, phường Bãi Cháy, thành phố Hạ Long, Quảng Ninh. <br/>Check -in time: 11:15 - 11:30am am at Halong international cruise port -No 9 Halong road, Bai Chay ward, Halong city, Quang Ninh province<br/>https://maps.app.goo.gl/8cg6PSLWAEKdg5SJ6?g_st=iz";
								data[18] = "Liên hệ: Ms. Dịu 0353516516<br/>Mr Nam quản lý: 0352642787";
							}
							if (booking.Trip.TripCode == "DNC")
							{
								data[17] = "V-Dream<br/>Thông tin đón bến Du thuyền V Dream:<br/>Thời gian có mặt: 18:15 - 18:30 tại Cảng Tàu Khách Quốc Tế Hạ Long, số 9 Hạ Long, phường Bãi Cháy, thành phố Hạ Long, Quảng Ninh. <br/>Check -in time: 18:15 - 18:30 am at Halong international cruise port -No 9 Halong road, Bai Chay ward, Halong city, Quang Ninh province<br/>https://maps.app.goo.gl/8cg6PSLWAEKdg5SJ6?g_st=iz";
								data[18] = "Liên hệ: Ms. Dịu 0353516516<br/>Mr Nam quản lý: 0352642787";
							}
							break;
						default:
							data[17] = "";
							data[8] = "";
							data[18] = "";
							break;
					}
					data[8] += "<br/>" + booking.PickupAddress;

				}
				data[9] = booking.SpecialRequest;
				data[19] = booking.SpecialRequestRoom;
				data[10] = booking.Total.ToString(CultureInfo.InvariantCulture);
				data[11] = UserIdentity.FullName;
				data[12] = UserIdentity.Email;
				data[13] = UserIdentity.Website;
				switch (booking.Cruise.Code)
				{
					case "OS1":
						data[14] = "Mr. Hiện 0934 600 099";
						data[15] = "Important note:     The bus pick-up time in Hanoi Old Quarter Area is 7:45-8:30 AM. To make the pick-up service smoothly for all passengers, please ask the guests to be ready with their passports at 7:45 AM at hotel lobby. Any late readiness beyond this time and miss for the bus, our company is not responsible.";
						break;
					case "OS2":
						data[14] = "Mr. Cuong 0943 827 399";
						data[15] = "Important note:     The bus pick-up time in Hanoi Old Quarter Area is 7:45-8:30 AM. To make the pick-up service smoothly for all passengers, please ask the guests to be ready with their passports at 7:45 AM at hotel lobby. Any late readiness beyond this time and miss for the bus, our company is not responsible.";
						break;
					case "OS3":
						data[14] = "Mr. Kiem 0913 024 975";
						data[15] = "Important note:     The bus pick-up time in Hanoi Old Quarter Area is 7:45-8:30 AM. To make the pick-up service smoothly for all passengers, please ask the guests to be ready with their passports at 7:45 AM at hotel lobby. Any late readiness beyond this time and miss for the bus, our company is not responsible.";
						break;
					case "NCL1":
						data[14] = "Ms. Phuong Anh 0989 201 640/ Ms. Ngoc 0982 920 486";
						data[15] = "Important note:     The bus pick-up time in Hanoi Old Quarter Area is 9:30-10:00 AM. To make the pick-up service smoothly for all passengers, please ask the guests to be ready with their passport at 9:25 AM at hotel lobby. Any late readiness beyond this time and miss for the bus, our company is not responsible.";
						break;
					case "NCL2":
						data[14] = "Ms. Phuong Anh 0989 201 640/ Ms. Ngoc 0982 920 486";
						data[15] = "Important note:     The bus pick-up time in Hanoi Old Quarter Area is 9:30-10:00 AM. To make the pick-up service smoothly for all passengers, please ask the guests to be ready with their passport at 9:25 AM at hotel lobby. Any late readiness beyond this time and miss for the bus, our company is not responsible.";
						break;
					case "STL":
						data[14] = "Mr. Dzung 0912 545 111";
						data[15] = "Important note:     The bus pick-up time in Hanoi Old Quarter Area is 7:45-8:30 AM. To make the pick-up service smoothly for all passengers, please ask the guests to be ready with their passports at 7:45 AM at hotel lobby. Any late readiness beyond this time and miss for the bus, our company is not responsible.";
						break;
					default:
						data[14] = "";
						data[15] = "";
						break;
				}

				if (booking.Cruise.Code == "VD")
				{
					data[20] = "Thanh toán trước 3 ngày check in <br/>Chuyển khoản theo thông tin tài khoản sau:  <br/>TK CHUYỂN TIỀN TÀU VDREAM ( No VAT)  <br/> Tên tài khoản TRAN THI THANH NGA<br/> Số tài khoản 0011004229521<br/> Ngân hàng Vietcombank SGD Hà Nội <br/>";
				}
				else if (booking.Cruise.Code == "OS1" || booking.Cruise.Code == "OS2" || booking.Cruise.Code == "OS3")
				{
					data[20] = "Thanh toán trước 3 ngày check in <br/>Chuyển khoản theo thông tin tài khoản sau: <br/> TK CHUYỂN TIỀN TÀU ORIENTAL SAILS ( No VAT) <br/> Tên tài khoản Bùi Văn Chi<br/> Số tài khoản 0011004389709<br/> Ngân hàng Vietcombank SGD Hà Nội";
				}
				else if (booking.Cruise.Code == "NCL1" || booking.Cruise.Code == "NCL2")
				{
					data[20] = "Thanh toán trước 3 ngày check in <br/>Chuyển khoản theo thông tin tài khoản sau: <br/> TK CHUYỂN TIỀN TÀU CALYPSO ( No VAT) <br/> Tên tài khoản Bùi Văn Chi<br/> Số tài khoản 0011004389709<br/> Ngân hàng Vietcombank SGD Hà Nội";
				}
				else if (booking.Cruise.Code == "STL")
				{
					data[20] = "Thanh toán trước 3 ngày check in <br/>Chuyển khoản theo thông tin tài khoản sau: <br/> TK CHUYỂN TIỀN TÀU STARLIGHT ( No VAT) <br/> Tên tài khoản Bùi Văn Chi<br/> Số tài khoản 0011004389709<br/> Ngân hàng Vietcombank SGD Hà Nội";
				}

				data[20] += " <br/> Tài khoản bao gồm VAT ( vui lòng + thêm 8% VAT ) <br/> Công ty Cổ Phần Những Cánh Buồm Phương Đông <br/> Số tài khoản: 0011004109002 <br/> Tại Sở Giao Dịch Ngân Hàng Ngoại Thương Hà Nội - Vietcombank";

				StatusType status = (StatusType)Convert.ToInt32(Request.QueryString["status"]);
				if (Request.QueryString["status"] == null)
				{
					status = booking.Status;
				}
				switch (status)
				{
					case StatusType.Approved:
						StreamReader appReader = new StreamReader(Server.MapPath("/Modules/Sails/Admin/EmailTemplate/Approved.txt"));
						string appFormat = appReader.ReadToEnd();
						txtSubject.Text = string.Format(APPROVED_SUBJECT, booking.StartDate);
						fckContent.Value = string.Format(appFormat, data);
						break;
					//case StatusType.Rejected:
					//    StreamReader rejReader = new StreamReader(Server.MapPath("/Modules/Sails/Admin/EmailTemplate/rejected.txt"));
					//    string rejFormat = rejReader.ReadToEnd();
					//    txtSubject.Text = string.Format(REJECTED_SUBJECT, booking.StartDate);
					//    fckContent.Value =
					//        string.Format(rejFormat, data);
					//    break;
					case StatusType.Cancelled:
						StreamReader canReader = new StreamReader(Server.MapPath("/Modules/Sails/Admin/EmailTemplate/cancelled.txt"));
						string canFormat = canReader.ReadToEnd();
						txtSubject.Text = string.Format(REJECTED_SUBJECT, booking.StartDate);
						data[10] = booking.CancelPay.ToString();
						fckContent.Value =
							string.Format(canFormat, data);
						break;
					default:
						break;
				}
			}
		}

		protected void btnSend_Click(object sender, EventArgs e)
		{
			// Đăng nhập            
			SmtpClient smtpClient = new SmtpClient("mail.orientalsails.com");
			smtpClient.Credentials = new NetworkCredential("sales.c2@orientalsails.com", "os#@!123");
			//smtpClient.EnableSsl = true;

			// Địa chỉ email người gửi
			//MailAddress fromAddress = new MailAddress(UserIdentity.Email);
			MailAddress fromAddress = new MailAddress("sales@orientalsails.com");

			MailMessage message = new MailMessage();
			message.From = fromAddress;
			message.To.Add(lblEmailTo.Text);
			message.Subject = txtSubject.Text;
			message.IsBodyHtml = true;
			message.BodyEncoding = Encoding.UTF8;
			message.Body = fckContent.Value;
			message.CC.Add(UserIdentity.Email);

			smtpClient.Send(message);
			ClientScript.RegisterClientScriptBlock(typeof(SendEmail), "closure", "window.close()", true);
		}
	}
}
