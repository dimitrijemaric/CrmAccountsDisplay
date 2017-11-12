using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountInfo.Dynamics_CRM;
using AccountInfo.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Ninject;

namespace AccountInfo.Controllers
{
    public class HomeController : Controller
    {
        public CrmManager _crmManager { get; set; }
        private IOrganizationService _service { get; set; }

        public HomeController()
        {
                
        }    

        public HomeController(IOrganizationService service)
        {
           

            _service = service;
            _crmManager = new CrmManager(_service);
        }

        public ActionResult Accounts()
        {
           

            List<CrmAccountInfoViewModel> crmAccountsInfoCollection = new List<CrmAccountInfoViewModel>();

            if (_service == null)
            {
                // log the issue somwehere

                ViewBag.ConnectionError = "yes";

                return View(crmAccountsInfoCollection);
            }


            var accountColumns = new ColumnSet(new string[] {"name", "address1_composite", "accountid", "primarycontactid"});
            var activeAccounts = _crmManager.RetrieveActiveRecordsForEntity("account", accountColumns);

            if (activeAccounts.Entities.Count == 0)
            {
                return View(crmAccountsInfoCollection);
            }

            foreach (var account in activeAccounts.Entities)
            {
                ColumnSet contactColumns = new ColumnSet("fullname", "emailaddress1", "address1_composite","mobilephone","address1_telephone1", "contactid");

                var allContacts = _crmManager.GetAllChildRecords(account.Id, account.LogicalName,  "contact","parentcustomerid", contactColumns).Entities;
                var primaryContact = _crmManager.GetParentRecord("contact", account.Id, account.LogicalName,null, "primarycontactid", contactColumns);

                if (primaryContact != null && allContacts.ToList().Find(a => a.Id.Equals(primaryContact.Id)) == null)
                {
                    allContacts.Add(primaryContact);
                }



                var accountInfo = new CrmAccountInfoViewModel
                {
                    AccountName = account["name"] as string,
                    AccountId = account.Id,
                    AccountAddress = account.Attributes.Contains("address1_composite") ? account["address1_composite"] as string : "n/a",
                    NoContactData = IsAllContactDataMissing(allContacts),
                    NoChildContacts = allContacts.Count == 0,
                    ContactsCount = allContacts.Count
                };

                crmAccountsInfoCollection.Add(accountInfo);
             
            }
               
            
        
            return View(crmAccountsInfoCollection);
        }

        private static bool IsAllContactDataMissing(DataCollection<Entity> allContacts)
        {
            return allContacts.Where(c => (!c.Attributes.Contains("address1_composite") || c["address1_composite"] == null) && (!c.Attributes.Contains("emailaddress1") || c["emailaddress1"] == null) && (!c.Attributes.Contains("address1_telephone1") || c["address1_telephone1"] == null) && (!c.Attributes.Contains("mobilephone") || c["mobilephone"] == null)).ToList().Count == allContacts.Count;
        }


        public ActionResult Contacts(string accountId, string accountName)
        {

            ColumnSet contactColumns = new ColumnSet("fullname", "emailaddress1", "address1_composite", "contactid");
            var allChildContacts = _crmManager.GetAllChildRecords(new Guid(accountId), "account", "contact", "parentcustomerid",contactColumns).Entities;
            var primaryContact = _crmManager.GetParentRecord("contact",new Guid(accountId), "account",null, "primarycontactid", contactColumns);

            List <CrmContactInfoViewModel> crmContactsInfoCollection = new List<CrmContactInfoViewModel>();

            if (primaryContact != null)
            {
                crmContactsInfoCollection.Add(
                    new CrmContactInfoViewModel
                    {
                        ContactName = primaryContact["fullname"] as string,
                        Email = primaryContact.Attributes.Contains("emailaddress1") ? primaryContact["emailaddress1"] as string : "n/a",
                        Location = primaryContact.Attributes.Contains("address1_composite") ? primaryContact["address1_composite"] as string : "n/a",
                        ContactId = (Guid)primaryContact["contactid"],
                        isPrimary = true
                    }
                );
            }


            foreach (var contact in allChildContacts)
            {
                if (crmContactsInfoCollection.Find(a=>a.ContactId.Equals(contact.Id)) == null)
                {
                    crmContactsInfoCollection.Add(
                        new CrmContactInfoViewModel
                        {
                            ContactName = contact["fullname"] as string,
                            Email = contact.Attributes.Contains("emailaddress1") ? contact["emailaddress1"] as string : "n/a",
                            Location = contact.Attributes.Contains("address1_composite") ? contact["address1_composite"] as string : "n/a",
                            ContactId = (Guid) contact["contactid"],
                            isPrimary = false
                        }
                    );
                }
            }


            ViewBag.ParentAccountName = accountName;
            return View(crmContactsInfoCollection);
        }
    }
}