using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace AccountInfo.Dynamics_CRM
{
    public  class CrmConnector
    {

        internal OrganizationServiceProxy GetCrmService(string crmConnectionString)
        {

           
                //System.Configuration.;

            var crmUrl = crmConnectionString.Split(';')[0];
            var crmUsername = crmConnectionString.Split(';')[1];
            var crmPassword = crmConnectionString.Split(';')[2];

            Uri oUri = new Uri(crmUrl);
       
            ClientCredentials clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = crmUsername;
            clientCredentials.UserName.Password = crmPassword;
            clientCredentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;


 
            var _serviceProxy = new OrganizationServiceProxy(
                oUri,
                null,
                clientCredentials,
                null);
         

            _serviceProxy.EnableProxyTypes();

            return _serviceProxy;
        

        }

        internal EntityCollection RetrieveActiveAccounts(IOrganizationService service)
        {
            var criteria = new FilterExpression();
            criteria.AddCondition("statuscode", ConditionOperator.Equal,
               new object[] { 1 } );
            return service.RetrieveMultiple(new QueryExpression
            {
                Criteria = criteria,
                ColumnSet = new ColumnSet(new string[] {"name", "address1_city", "accountid"}),
                EntityName = "account",
                Distinct = true,

                PageInfo =
                {
                    Count = int.MaxValue,
                    ReturnTotalRecordCount = true
                }
            });
             
        }
    }
}
