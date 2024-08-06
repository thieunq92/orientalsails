using NHibernate.Hql.Ast.ANTLR.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Modules.OrientalSails.DataTransferObject
{
	public class BookingDTO
	{
		public int Id { get; set; }

		public string BookingId { get; set; }

		public string StartDate { get; set; }

		public string TripName { get; set; }

		public string CruiseName { get; set; }

		public int NoOfRoom { get; set; }

		public int NoOfPax { get; set; }

		public string Room { get; set; }

		public string Pax { get; set; }

		public string TACode { get; set; }

		public string SpecialRequest { get; set; }

		public string CutoffDate { get; set; }

		public string Status { get; set; }

		public string TripCode { get; set; }

		public string ModifiedBy { get; set; }

		public string ModifiedDate { get; set; }

		public string CustomerName { get; set; }

		public string AgencyName { get; set; }

		public int AgencyId { get; set; }

		public int Adult { get; set; }

		public int Child { get; set; }

		public int Baby { get; set; }

		public string PickupAddress { get; set; }

		public string SpecialRequestRoom { get; set; }

		public string SalesInChargeName { get; set; }

		public string SalesInChargePhone { get; set; }

		public bool HasInvoice { get; set; }

		public string AgencyNotes { get; set; }

		public bool IsWarningBooking { get; set; }

		public bool IsPendingBooking { get; set; }

		public bool Inspection { get; set; }

		public bool HaveBirthdayBooking { get; set; }
		public double Total { get; set; }
		public int TripId { get; set; }
	}
}