using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Core.Domain;
using NHibernate.Criterion;
using NHibernate.Transform;
using Portal.Modules.OrientalSails.Domain;

namespace Portal.Modules.OrientalSails
{
    public partial class SailsModule
    {
        public IList<Facilitie> GetAllFacilities()
        {
            var queryOver = _commonDao.OpenSession().QueryOver<Facilitie>();
            return queryOver.List<Facilitie>();
        }
        public IList<FacilitieMap> GetAllFacilitiesByType(string type, string id)
        {
            var queryOver = _commonDao.OpenSession().QueryOver<FacilitieMap>();
            queryOver.Where(x => x.FacilitieType == type.ToUpper() && x.ObjectId == id);
            return queryOver.List<FacilitieMap>();
        }
        public IList<DocumentCategory> GetAllDocumentByType(string type, string id)
        {
            var queryOver = _commonDao.OpenSession().QueryOver<DocumentCategory>();
            queryOver.Where(x => x.DocumentType == type.ToUpper() && x.ObjectId == id);
            return queryOver.List<DocumentCategory>();
        }
        public IList<ImageGallery> GetAllImageGalleryByType(string type, string id)
        {
            var queryOver = _commonDao.OpenSession().QueryOver<ImageGallery>();
            queryOver.Where(x => x.ImageType == type.ToUpper() && x.ObjectId == id);
            return queryOver.List<ImageGallery>();
        }
        public IList<Reviews> GetAllReviewByType(string type, string id)
        {
            var queryOver = _commonDao.OpenSession().QueryOver<Reviews>();
            queryOver.Where(x => x.ReviewType == type.ToUpper() && x.ObjectId == id);
            return queryOver.List<Reviews>();
        }
        public IList<SailsTrip> TripGetByDateNotLock(DateTime date, User user)
        {
            var cruiseRoleQuery = _commonDao.OpenSession().QueryOver<IvRoleCruise>();
            cruiseRoleQuery = cruiseRoleQuery.Where(x => x.User.Id == user.Id);
            var cruiseRoles = cruiseRoleQuery.List().ToList();
            var cruiseTripQuery = _commonDao.OpenSession().QueryOver<CruiseTrip>();
            var cruiseTrips = cruiseTripQuery.List().ToList();
            cruiseTrips = cruiseTrips.Where(x => cruiseRoles.Select(y => y.Cruise).Contains(x.Cruise)).ToList();
            var trips = cruiseTrips.Select(x => x.Trip);

            var query = _commonDao.OpenSession().QueryOver<SailsTrip>();
            query = query.Where(x => x.Deleted == false);
            query = query.Where(x => x.IsLock == false || (x.IsLock && x.LockType == "From" && x.LockFromDate > date)
                                     || (x.IsLock && x.LockType == "FromTo" &&
                                         (x.LockFromDate > date || x.LockToDate < date)));
            var tripsByCruiseRole = query.List().ToList().Where(x => trips.Select(y => y.Id).Contains(x.Id));
            return tripsByCruiseRole.ToList();
        }
        public IList<TripConfigPrice> GetTripPriceConfig(int pageSize, int pageIndex, out int total)
        {
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            var queryOver = _commonDao.OpenSession().QueryOver<TripConfigPrice>();
            //queryOver.Where(p => p.FromDate > DateTime.Now.AddDays(-1));
            queryOver.Where(p => p.Enable && p.Trip != null && p.Trip.Id > 0);
            queryOver = queryOver.OrderBy(x => x.FromDate).Desc;
            total = queryOver.RowCount();

            if (pageSize > 0)
            {
                return queryOver.Skip(pageSize * pageIndex).Take(pageSize).List<TripConfigPrice>();
            }
            else
            {
                return queryOver.List<TripConfigPrice>();
            }
        }
        public IList<CruiseConfigPrice> GetCruiseConfigPrice(int tripPriceId, Cruise cruise)
        {
            var queryOver = _commonDao.OpenSession().QueryOver<CruiseConfigPrice>();
            queryOver.Where(p => p.TripConfigPriceId == tripPriceId && p.CruiseId == cruise.Id);
            return queryOver.List<CruiseConfigPrice>();
        }
        public IList<TripConfigPrice> GetAgencyTripPriceConfig(int pageSize, int pageIndex, out int total)
        {
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            var queryOver = _commonDao.OpenSession().QueryOver<TripConfigPrice>();
            //queryOver.Where(p => p.FromDate > DateTime.Now.AddDays(-1));
            queryOver.Where(p => p.Enable && p.AgentLevel != null && p.AgentLevel.Id > 0);
            queryOver = queryOver.OrderBy(x => x.FromDate).Desc;
            total = queryOver.RowCount();

            if (pageSize > 0)
            {
                return queryOver.Skip(pageSize * pageIndex).Take(pageSize).List<TripConfigPrice>();
            }
            else
            {
                return queryOver.List<TripConfigPrice>();
            }
        }
        public TripConfigPrice GetConfigPriceByCampaign(Campaign campaign)
        {
            var queryOver = _commonDao.OpenSession().QueryOver<TripConfigPrice>();
            queryOver.Where(p => p.Campaign.Id == campaign.Id);
            return queryOver.SingleOrDefault();
        }
    }
}