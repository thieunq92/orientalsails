using System;
using System.Collections;
using System.Collections.Generic;
using CMS.Core.Domain;
using Portal.Modules.OrientalSails.Web.Util;

namespace Portal.Modules.OrientalSails.Domain
{
    public class Expense
    {
        protected IList services;
        private Agency guide;
        private double cost;
        private string name;
        private string phone;
        private List<ExpenseHistory> listPendingExpenseHistory;
        public virtual List<ExpenseHistory> ListPendingExpenseHistory
        {
            get
            {
                if (listPendingExpenseHistory == null)
                {
                    listPendingExpenseHistory = new List<ExpenseHistory>();
                }
                return listPendingExpenseHistory;
            }
            set
            {
                listPendingExpenseHistory = value;
            }
        }
        public virtual int Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual Cruise Cruise { get; set; }
        public virtual SailExpensePayment Payment { get; set; }

        public virtual IList Services
        {
            get
            {
                if (services == null)
                {
                    services = new ArrayList();
                }
                return services;
            }
            set
            {
                services = value;
            }
        }

        public virtual bool LockIncome { get; set; }
        public virtual bool LockOutcome { get; set; }
        public virtual DateTime? LockDate { get; set; }
        public virtual User LockBy { get; set; }
        public virtual string LockStatus { get; set; }
        public virtual IList<ExpenseHistory> ListExpenseHistory { get; set; }
        private int numberOfGroup;
        public virtual int NumberOfGroup
        {
            get
            {
                if (numberOfGroup <= 0)
                {
                    numberOfGroup = 1;
                }
                return numberOfGroup;
            }
            set { numberOfGroup = value; }
        }
        public virtual string Type { get; set; }
        public virtual Agency Guide
        {
            get
            {
                return guide;
            }
            set
            {
                var oldGuideId = guide != null ? guide.Id : -1;
                var newGuideId = value != null ? value.Id : -1;
                SetProperty<int>("GuideId", ref oldGuideId, newGuideId);
                guide = value;
            }
        }
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == null) name = "";
                SetProperty<string>("Name", ref name, value);
            }
        }
        public virtual double Cost
        {
            get
            {
                return cost;
            }
            set
            {
                SetProperty<double>("Cost", ref cost, value);
            }
        }
        public virtual User Operator { get; set; }
        public virtual string Phone
        {
            get
            {
                return phone;
            }
            set
            {
                if (phone == null) phone = "";
                SetProperty<string>("Phone", ref phone, value);
            }
        }
        public virtual User CurrentUser { get; set; }
        public virtual Dictionary<CostType, double> Calculate(IList costTypes, GetCurrentCostTable costTable, GetCurrentDailyCostTable dailyTable, GetCurrentCruiseExpenseTable getCruiseTable, Cruise activecruise, IList bookings, SailsModule module, bool partnership)
        {
            Cruise = activecruise;

            if (Cruise != null)
            {
                Dictionary<CostType, ExpenseService> serviceMap = new Dictionary<CostType, ExpenseService>();
                Dictionary<CostType, double> serviceTotal = new Dictionary<CostType, double>();
                foreach (CostType type in costTypes)
                {
                    serviceMap.Add(type, null);
                    serviceTotal.Add(type, 0);
                }

                foreach (ExpenseService service in Services)
                {
                    if (!serviceMap.ContainsKey(service.Type))
                    {
                        continue;
                    }
                    serviceMap[service.Type] = service;
                    if (service.Type.IsDailyInput)
                    {
                        serviceTotal[service.Type] += service.Cost;
                    }
                }
                int adultHaiPhong = 0;
                int childHaiPhong = 0;

                foreach (Booking booking in bookings)
                {
                    Dictionary<CostType, double> bookingCost = booking.Cost(costTable(Date, booking.Trip, booking.TripOption), costTypes);
                    foreach (CostType type in costTypes)
                    {
                        serviceTotal[type] += bookingCost[type];
                    }

                    adultHaiPhong += booking.Adult;
                    childHaiPhong += booking.Child;
                }

                bool _isRun = bookings.Count > 0;

                if (_isRun)
                {
                    DailyCostTable table = dailyTable(Date);
                    if (table != null)
                    {
                        foreach (DailyCost cost in dailyTable(Date).Costs)
                        {
                            if (serviceTotal.ContainsKey(cost.Type))
                            {
                                serviceTotal[cost.Type] += cost.Cost;
                            }
                        }
                    }
                }

                CruiseExpenseTable cruiseTable = getCruiseTable(Date, Cruise);
                CalculateCruiseExpense(costTypes, serviceTotal, adultHaiPhong, childHaiPhong, cruiseTable);

                foreach (CostType type in costTypes)
                {
                    if (type.IsDailyInput)
                    {
                        continue;
                    }
                    if (serviceMap[type] != null)
                    {
                        if (serviceMap[type].Cost != serviceTotal[type])
                        {
                            serviceMap[type].Cost = serviceTotal[type];
                            module.SaveOrUpdate(serviceMap[type]);
                        }
                    }
                    else
                    {
                        if (type.DefaultAgency == null && partnership)
                        {
                            throw new Exception("You must config default agency for " + type.Name);
                        }
                        ExpenseService service = new ExpenseService();
                        service.Expense = this;
                        service.Cost = serviceTotal[type];
                        service.Name = string.Format("{0:dd/MM/yyyy}- {1}", Date, type.Name);
                        service.Paid = 0;
                        service.Supplier = type.DefaultAgency;
                        if (service.Supplier != null)
                        {
                            service.Phone = type.DefaultAgency.Phone;
                        }
                        service.Type = type;
                        module.SaveOrUpdate(service);
                    }
                }
                return serviceTotal;
            }

            Dictionary<CostType, double> total = new Dictionary<CostType, double>();
            IList cruises = module.CruiseGetAll();

            foreach (CostType type in costTypes)
            {
                total.Add(type, 0);
            }

            foreach (Cruise cruise in cruises)
            {
                Expense expense = module.ExpenseGetByDate(cruise, Date);

                IList filtered = new ArrayList();
                foreach (Booking booking in bookings)
                {
                    if (booking.Cruise != null && booking.Cruise.Id == cruise.Id)
                    {
                        filtered.Add(booking);
                    }
                }

                Dictionary<CostType, double> expenses = expense.Calculate(costTypes, costTable, dailyTable,
                                                                          getCruiseTable, cruise, filtered, module,
                                                                          partnership);
                foreach (CostType type in costTypes)
                {
                    total[type] += expenses[type];
                }
            }
            return total;
        }

        public virtual void CalculateCruiseExpense(IList costTypes, IDictionary<CostType, double> serviceTotal, int adultHaiPhong, int childHaiPhong, CruiseExpenseTable cruiseTable)
        {
            CostType cruise = null;
            foreach (CostType type in costTypes)
            {
                if (type.Id == SailsModule.HAIPHONG)
                {
                    cruise = type;
                    break;
                }
            }
            if (cruise != null)
            {
                double haiphong = adultHaiPhong + childHaiPhong / 2;

                if (haiphong > 0)
                {
                    int index = -1;
                    for (int ii = 0; ii < cruiseTable.Expenses.Count; ii++)
                    {
                        CruiseExpense cExpense = (CruiseExpense)cruiseTable.Expenses[ii];
                        if (cExpense.CustomerFrom <= haiphong && cExpense.CustomerTo >= haiphong)
                        {
                            index = ii;
                            break;
                        }
                    }

                    if (index < 0)
                    {
                        throw new Exception("Hai phong cruise price is not valid, can not find price for " +
                                            haiphong +
                                            " persons");
                    }
                    serviceTotal[cruise] += ((CruiseExpense)cruiseTable.Expenses[index]).Price;
                }
            }
        }

        public virtual Dictionary<CostType, double> GetPayable(IList costTypes)
        {
            Dictionary<CostType, double> serviceTotal = new Dictionary<CostType, double>();
            foreach (CostType type in costTypes)
            {
                serviceTotal.Add(type, 0);
            }

            foreach (ExpenseService service in Services)
            {
                serviceTotal[service.Type] += service.Cost - service.Paid;
            }

            return serviceTotal;
        }
        protected void SetProperty<T>(string name, ref T oldValue, T newValue) where T : System.IEquatable<T>
        {
            if (oldValue == null || !oldValue.Equals(newValue))
            {
                var expenseHistory = new ExpenseHistory()
                {
                    ColumnName = name,
                    OldValue = oldValue.ToString(),
                    NewValue = newValue.ToString(),
                    CreatedDate = DateTime.Now,
                    Expense = this,
                    CreatedBy = CurrentUser,
                };
                ListPendingExpenseHistory.Add(expenseHistory);
                oldValue = newValue;
            }
        }
        protected void SetProperty<T>(string name, ref Nullable<T> oldValue, Nullable<T> newValue) where T : struct, System.IEquatable<T>
        {
            if (oldValue.HasValue != newValue.HasValue || (newValue.HasValue && !oldValue.Value.Equals(newValue.Value)))
            {
                var expenseHistory = new ExpenseHistory()
                {
                    ColumnName = name,
                    OldValue = oldValue.ToString(),
                    NewValue = newValue.ToString(),
                    CreatedDate = DateTime.Now,
                    Expense = this,
                    CreatedBy = CurrentUser,
                };
                ListPendingExpenseHistory.Add(expenseHistory);
                oldValue = newValue;
            }
        }
        public virtual BusByDate BusByDate { get; set; }

    }

    public delegate CostingTable GetCurrentCostTable(DateTime date, SailsTrip trip, TripOption option);
    public delegate DailyCostTable GetCurrentDailyCostTable(DateTime date);
    public delegate CruiseExpenseTable GetCurrentCruiseExpenseTable(DateTime date, Cruise cruise);
}