using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTQD;

namespace WTQD
{
    public class QueryContacts: CodeActivity
    {
        #region "Parameter Definition"
        [RequiredArgument]
        [Input("Contact Lookup Field1* (text field)")]
        [ReferenceTarget("")]
        public InArgument<String> CnctLookupField1 { get; set; }
        
        [RequiredArgument]
        [Input("Field1 Value* (string)")]
        [ReferenceTarget("")]
        public InArgument<String> CnctFieldValue1 { get; set; }
        
        [Input("Contact Lookup Field2 (text field)")]
        [ReferenceTarget("")]
        public InArgument<String> CnctLookupField2 { get; set; }
        
        [Input("Field2 Value (string)")]
        [ReferenceTarget("")]
        public InArgument<String> CnctFieldValue2 { get; set; }

        [Output("FoundOne")]
        public OutArgument<bool> CnctFoundOne { get; set; }
        
        [Output("Output")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> CnctOut { get; set; }

        [Output("FoundMultiple")]
        public OutArgument<bool> CnctFoundMultiple { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext executionContext)
        {

            #region "Load CRM Service from context"

            Common objCommon = new Common(executionContext);
            objCommon.tracingService.Trace("Load CRM Service from context --- OK");
            #endregion

            #region "Read Parameters"
            String _CnctEntityName = "contact";
            String _CnctLookupField1 = this.CnctLookupField1.Get(executionContext);
            String _CnctLookupField2 = this.CnctLookupField2.Get(executionContext);
            String _CnctFieldValue1 = this.CnctFieldValue1.Get(executionContext);
            String _CnctFieldValue2 = this.CnctFieldValue2.Get(executionContext);

            objCommon.tracingService.Trace(String.Format("CnctEntityName: {0} - CnctLookupField1:{1} - CnctLookupField2:{2} - CnctFieldValue1:{3} CnctFieldValue2:{4}",
                _CnctEntityName, _CnctLookupField1, _CnctLookupField2, _CnctFieldValue1, _CnctFieldValue2));
            #endregion


            #region "QueryExpression Execution"
            QueryExpression qe = new QueryExpression();
            qe.EntityName = _CnctEntityName;
            qe.ColumnSet = new ColumnSet("contactid");
            //prevent query from running forever with duplicate records
            qe.PageInfo = new PagingInfo();
            qe.PageInfo.Count = 2;
            qe.PageInfo.PageNumber = 1;
            
            FilterExpression filter = new FilterExpression(LogicalOperator.And);
            if (_CnctLookupField1 != null && _CnctFieldValue1 != "")
            {
                ConditionExpression condition1 = new ConditionExpression();
                condition1.AttributeName = _CnctLookupField1;
                condition1.Values.Add(_CnctFieldValue1);
                condition1.Operator = ConditionOperator.Equal;
                filter.Conditions.Add(condition1);
            }
            if (_CnctLookupField2 != null && _CnctFieldValue2 != "")
            {
                ConditionExpression condition2 = new ConditionExpression();
                condition2.AttributeName = _CnctLookupField2;
                condition2.Values.Add(_CnctFieldValue2);
                condition2.Operator = ConditionOperator.Equal;
                filter.Conditions.Add(condition2);
            }
            ConditionExpression condition3 = new ConditionExpression();
            condition3.AttributeName = "statecode";
            condition3.Values.Add(0); //active records 
            condition3.Operator = ConditionOperator.Equal;
            filter.Conditions.Add(condition3);

            qe.Criteria=filter;

            EntityCollection CnctResults= objCommon.service.RetrieveMultiple(qe);
            if (CnctResults.Entities.Count > 0)
            {
                CnctFoundOne.Set(executionContext, true);
                CnctOut.Set(executionContext, new EntityReference("contact", CnctResults[0].Id));

            }
            if (CnctResults.Entities.Count > 1)
            {
                CnctFoundMultiple.Set(executionContext, true);
            }

            #endregion

        }
    }
}
