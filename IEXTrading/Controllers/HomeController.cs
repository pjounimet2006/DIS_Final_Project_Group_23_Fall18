using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IEXTrading.Infrastructure.IEXTradingHandler;
using IEXTrading.Models;
using IEXTrading.Models.ViewModel;
using IEXTrading.DataAccess;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;


namespace MVCTemplate.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {

           return View();
        }
        public IActionResult AboutUs()
        {

            return View();
        }
        /*Method that Calls for FinancialReports View and lets user
         /* Get Financial Reports into a DB for a given Symbol*/
        public IActionResult FinancialReports(string symbol)
        {
            List<Company> companies = dbContext.Companies.ToList();

            return View(companies);
            
        }
        /*Method that Applies Top 5 Stocks from Data in DB in table FinancialReports
         *. It firsts applies logic getting Current Ratio and orders it in descending
         * Order. I will later store in DB if it was previously not stored. */

        public IActionResult Stats()
        {
            List<Stat> StatList = new List<Stat> { };

            List<Stat> StatListDB = new List<Stat> { };

            foreach (FinancialReport fin in dbContext.FinancialReports)

            {

                    Stat temp = new Stat();

                    if (!StatList.Exists(o => o.symbol.Equals(fin.symbol)))
                    {

                        if (fin.currentDebt != 0)
                        {
                            temp.current_ratio = fin.currentAssets / fin.currentDebt;
                            temp.symbol = fin.symbol;
                            
                            StatList.Add(temp);
                        }
                    }
                
                
              
               
            }


            foreach (Stat s in StatList.GetRange(0, 5)
                           .OrderByDescending(o => o.current_ratio))
            {
                if (dbContext.Stats.Where(c => c.symbol.Equals(s.symbol)).Count() == 0)


                {
                    dbContext.Stats.Add(s);

                }
            }
            dbContext.SaveChanges();
            ViewBag.dbSuccessChart = 1;

            return View(StatList.GetRange(0,5)
                                .OrderByDescending(o=> o.current_ratio));
           


        }

        /****
         * The Symbols action calls the GetSymbols method that returns a list of Companies.
         * This list of Companies is passed to the Symbols View.
        ****/
        public IActionResult Symbols()
        {
            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;
            IEXHandler webHandler = new IEXHandler();
            List<Company> companies = webHandler.GetSymbolsTotal();

            
            //Save comapnies in TempData
            //TempData["Companies"] = JsonConvert.SerializeObject(companies);
            String companiesData = JsonConvert.SerializeObject(companies);
            


            return View(companies);
        }

        /****
         * The Chart action calls the GetChart method that returns 1 year's equities for the passed symbol.
         * A ViewModel CompaniesEquities containing the list of companies, prices, volumes, avg price and volume.
         * This ViewModel is passed to the Chart view.
        ****/
        public IActionResult Chart(string symbol)
        {
            //Set ViewBag variable first
            ViewBag.dbSuccessChart = 0;
            List<Equity> equities = new List<Equity>();
            if (symbol != null)
            {
                IEXHandler webHandler = new IEXHandler();
                equities = webHandler.GetChart(symbol);
                equities = equities.OrderBy(c => c.date).ToList(); //Make sure the data is in ascending order of date.
            }

            CompaniesEquities companiesEquities = getCompaniesEquitiesModel(equities);

            return View(companiesEquities);
        }

        /****
         * The Refresh action calls the ClearTables method to delete records from a or all tables.
         * Count of current records for each table is passed to the Refresh View.
        ****/
        public IActionResult Refresh(string tableToDel)
        {
            ClearTables(tableToDel);
            Dictionary<string, int> tableCount = new Dictionary<string, int>();
            tableCount.Add("Companies", dbContext.Companies.Count());
            tableCount.Add("Charts", dbContext.Equities.Count());
            tableCount.Add("Financial Reports", dbContext.FinancialReports.Count());
            tableCount.Add("Stats", dbContext.Stats.Count());
            return View(tableCount);
        }

        /****
         * Saves the Symbols in database.
        ****/
        public IActionResult PopulateSymbols()
        { 
            List<Company> companies = JsonConvert.DeserializeObject<List<Company>>(TempData["Companies"].ToString());
            foreach (Company company in companies)
            {
                //Database will give PK constraint violation error when trying to insert record with existing PK.
                //So add company only if it doesnt exist, check existence using symbol (PK)
                if (dbContext.Companies.Where(c => c.symbol.Equals(company.symbol)).Count() == 0)
                {
                    dbContext.Companies.Add(company);
                }
            }
            dbContext.SaveChanges();
            ViewBag.dbSuccessComp = 1;
            return View("Symbols", companies);
        }

      
/* Script to Get all Symbols into Database */
            
        public IActionResult PopulateSymbolsDB()
        {
            ViewBag.dbSucessComp = 0;
            IEXHandler webHandler = new IEXHandler();
            List<Company> companies = webHandler.GetSymbolsTotal();

            //List<Company> companies = JsonConvert.DeserializeObject<List<Company>>(TempData["Companies"].ToString());
            foreach (Company company in companies)
            {
                //Database will give PK constraint violation error when trying to insert record with existing PK.
                //So add company only if it doesnt exist, check existence using symbol (PK)
                if (dbContext.Companies.Where(c => c.symbol.Equals(company.symbol)).Count() == 0)
                {
                    dbContext.Companies.Add(company);
                }
            }
            dbContext.SaveChanges();
            ViewBag.dbSuccessComp = 1;
            return View("Symbols", companies);

        }

        public IActionResult SaveFinacialReportsDB(string symbol)
        {
            IEXHandler webHandler = new IEXHandler();
            FinanceInfo tempFinfo = new FinanceInfo();
            tempFinfo= webHandler.GetFinancials(symbol);
            if (tempFinfo.financials != null)
            {
                foreach (FinancialReport fr in tempFinfo.financials)
                {

                    fr.symbol = tempFinfo.symbol;

                    dbContext.FinancialReports.Add(fr);

                }
                dbContext.SaveChanges();
                ViewBag.dbSuccessComp = 1;
            }
            return View("Index");
        }


        //PopulateFinacialReportsDB
        /****
         * Saves the equities in database.
        ****/
        public IActionResult SaveCharts(string symbol)
        {
            IEXHandler webHandler = new IEXHandler();
            List<Equity> equities = webHandler.GetChart(symbol);
            //List<Equity> equities = JsonConvert.DeserializeObject<List<Equity>>(TempData["Equities"].ToString());
            foreach (Equity equity in equities)
            {
                if (dbContext.Equities.Where(c => c.date.Equals(equity.date)).Count() == 0)
                {
                    dbContext.Equities.Add(equity);
                }
            }

            dbContext.SaveChanges();
            ViewBag.dbSuccessChart = 1;

            CompaniesEquities companiesEquities = getCompaniesEquitiesModel(equities);

            return View("Chart", companiesEquities);
        }

        /****
         * Deletes the records from tables.
        ****/
        public void ClearTables(string tableToDel)
        {
            if ("all".Equals(tableToDel))
            {
                //First remove equities and then the companies
                dbContext.Equities.RemoveRange(dbContext.Equities);
                dbContext.Companies.RemoveRange(dbContext.Companies);
                dbContext.Stats.RemoveRange(dbContext.Stats);
                dbContext.FinancialReports.RemoveRange(dbContext.FinancialReports);
            }
            else if ("Companies".Equals(tableToDel))
            {
                //Remove only those that don't have Equity stored in the Equitites table
                dbContext.Companies.RemoveRange(dbContext.Companies
                                                         .Where(c => c.Equities.Count == 0)
                                                                      );
            }
            else if ("Charts".Equals(tableToDel))
            {
                dbContext.Equities.RemoveRange(dbContext.Equities);
            }
            else if ("Stats".Equals(tableToDel))
            {
                dbContext.Stats.RemoveRange(dbContext.Stats);
            }
            else if ("FinancialReports".Equals(tableToDel))
            {
                dbContext.FinancialReports.RemoveRange(dbContext.FinancialReports);
            }
            dbContext.SaveChanges();
        }

        /****
         * Returns the ViewModel CompaniesEquities based on the data provided.
         ****/
        public CompaniesEquities getCompaniesEquitiesModel(List<Equity> equities)
        {
            List<Company> companies = dbContext.Companies.ToList();

            if (equities.Count == 0)
            {
                return new CompaniesEquities(companies, null, "", "", "", 0, 0);
            }

            Equity current = equities.Last();
            string dates = string.Join(",", equities.Select(e => e.date));
            string prices = string.Join(",", equities.Select(e => e.high));
            string volumes = string.Join(",", equities.Select(e => e.volume / 1000000)); //Divide vol by million
            float avgprice = equities.Average(e => e.high);
            double avgvol = equities.Average(e => e.volume) / 1000000; //Divide volume by million
            return new CompaniesEquities(companies, equities.Last(), dates, prices, volumes, avgprice, avgvol);
        }

    }
}
