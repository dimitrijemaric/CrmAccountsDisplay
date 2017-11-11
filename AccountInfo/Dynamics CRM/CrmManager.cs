using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Provider;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Ninject;

namespace AccountInfo.Dynamics_CRM
{
    public class CrmManager
    {
        public IOrganizationService _service { get; set; }

       
        public CrmManager(IOrganizationService service)
        {
            _service = service;
        }



        internal EntityCollection RetrieveActiveRecordsForEntity(string entityLogicalName, ColumnSet columns)
        {
          
                var criteria = new FilterExpression();
                criteria.AddCondition("statuscode", ConditionOperator.Equal,
                    new object[] {1});

                var qe = CreateQE(criteria, columns, entityLogicalName);

                return _service.RetrieveMultiple(qe);
           
        }

        private QueryExpression CreateQE(FilterExpression criteria, ColumnSet columns, string entityName)
        {
            return new QueryExpression
            {
                Criteria = criteria,
                ColumnSet = columns,
                EntityName = entityName,
                Distinct = true,

                PageInfo =
                {
                    Count = int.MaxValue,
                    ReturnTotalRecordCount = true
                }

            };


        }

        internal Entity GetParentRecord(string parentEntityLogicalName, Guid childRecordId,  string childEntityLogicalName, Entity childRecord, string childEntityLookupField, ColumnSet columns)
        {
            if (childRecord == null)
            {
                 childRecord = _service.Retrieve(childEntityLogicalName, childRecordId, new ColumnSet(childEntityLookupField));
            }
            if (childRecord != null && childRecord.Attributes.Contains(childEntityLookupField) && childRecord[childEntityLookupField] != null)
            {
                return _service.Retrieve(parentEntityLogicalName, ((EntityReference) childRecord[childEntityLookupField]).Id, columns);
            }
            else return null;
        }

        internal EntityCollection GetAllChildRecords(Guid parentEntityId, string entityLogicalName, string childEntityName, string childEntityLookupField, ColumnSet columns)
        {
            
          
                var criteria = new FilterExpression();
                criteria.AddCondition(childEntityLookupField, ConditionOperator.Equal,
                    parentEntityId);

                var qe = CreateQE(criteria, columns, childEntityName);

                return _service.RetrieveMultiple(qe);

        


        }
    }
}
