using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountInfo.Dynamics_CRM;
using AccountInfo.Models;
using Microsoft.Xrm.Sdk;

namespace AccountInfo.Controllers
{
    public class HomeController : Controller
    {
        private IOrganizationService _service { get; set; }
        public HomeController(IOrganizationService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            List<CrmAccountInfoViewModel> crmAccountsInfoCollection = new List<CrmAccountInfoViewModel>();
            var connectionString = ConfigurationManager.ConnectionStrings["CrmConnection"].ConnectionString;
            var crmConnector = new CrmConnector();

            using (var serviceProxy = crmConnector.GetCrmService(connectionString))
            {
                var service = (IOrganizationService)serviceProxy;
                var activeAccounts = crmConnector.RetrieveActiveAccounts(service);


                foreach (var account in activeAccounts.Entities)
                {

            

                    crmAccountsInfoCollection.Add
                        (
                            new CrmAccountInfoViewModel
                            {
                                AccountName = account["name"] as string,
                                AccountId = account.Id,
                                AccountAddress = account.Attributes.Contains("address1_city") ? account["address1_city"] as string : "n/a",
                                NoContactData = true
                            }

                        );
                }
               
            }
        
            return View(crmAccountsInfoCollection);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}