using IEXTrading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IEXTrading.Models
{
    public class FinancialReport
    {
        public int Id { get; set; }
        public string reportDate { get; set; }
        public Int64 grossProfit { get; set; }
        public Int64 costOfRevenue { get; set; }
        public Int64 operatingRevenue { get; set; }
        public Int64 totalRevenue { get; set; }
        public Int64 operatingIncome { get; set; }
        public Int64 netIncome { get; set; }
        public Int64 researchAndDevelopment { get; set; }
        public Int64 operatingExpense { get; set; }
        public Int64 currentAssets { get; set; }
        public Int64 totalAssets { get; set; }
        public Int64 totalLiabilities { get; set; }
        public Int64 currentCash { get; set; }
        public Int64 currentDebt { get; set; }
        public Int64 totalCash { get; set; }
        public Int64 totalDebt { get; set; }
        public Int64 shareholderEquity { get; set; }
        public Int64 cashChange { get; set; }
        public Int64 cashFlow { get; set; }
        public string operatingGainsLosses { get; set; }
        public string symbol { get; set; }

    }




    public class FinanceInfo

    {
        public string symbol { get; set; }
        public List<FinancialReport> financials { get; set; }



    }
}
