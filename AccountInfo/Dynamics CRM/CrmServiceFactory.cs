using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using AccountInfo.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Ninject;

namespace AccountInfo.Dynamics_CRM
{
    public sealed class CrmServiceFactory : IOrganizationServiceFactory, IDisposable
    {
        private readonly ConcurrentBag<OrganizationServiceProxy> serviceProxyBag  = new ConcurrentBag<OrganizationServiceProxy>();



        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            try
            {

                var crmConnectionString = ConfigurationManager.ConnectionStrings["CrmConnection"].ConnectionString;
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
                serviceProxyBag.Add(_serviceProxy);
                return _serviceProxy;
            }
            catch
            {

                return null;
            }
        }

  

      

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var proxy in serviceProxyBag)
                    {
                        proxy?.Dispose();                    
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CrmServiceFactory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
