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
    public class QueryAccounts: CodeActivity
    {
        #region "Parameter Definition"
        [RequiredArgument]
        [Input("Account Lookup Field1* (text field)")]
        [ReferenceTarget("")]
        public InArgument<String> AcctLookupField1 { get; set; }
        
        [RequiredArgument]
        [Input("Field1 Value* (string)")]
        [ReferenceTarget("")]
        public InArgument<String> AcctFieldValue1 { get; set; }
        
        [Input("Account Lookup Field2 (text field)")]
        [ReferenceTarget("")]
        public InArgument<String> AcctLookupField2 { get; set; }
        
        [Input("Field2 Value (string)")]
        [ReferenceTarget("")]
        public InArgument<String> AcctFieldValue2 { get; set; }

        [Output("FoundOne")]
        public OutArgument<bool> AcctFoundOne { get; set; }
        
        [Output("Output")]
        [ReferenceTarget("account")]
        public OutArgument<EntityReference> AcctOut { get; set; }

        [Output("FoundMultiple")]
        public OutArgument<bool> AcctFoundMultiple { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext executionContext)
        {

            #region "Load CRM Service from context"

            Common objCommon = new Common(executionContext);
            objCommon.tracingService.Trace("Load CRM Service from context --- OK");
            #endregion

            #region "Read Parameters"
            String _AcctEntityName = "account";
            String _AcctLookupField1 = this.AcctLookupField1.Get(executionContext);
            String _AcctLookupField2 = this.AcctLookupField2.Get(executionContext);
            String _AcctFieldValue1 = this.AcctFieldValue1.Get(executionContext);
            String _AcctFieldValue2 = this.AcctFieldValue2.Get(executionContext);

            objCommon.tracingService.Trace(String.Format("AcctEntityName: {0} - AcctLookupField1:{1} - AcctLookupField2:{2} - AcctFieldValue1:{3} AcctFieldValue2:{4}",
                _AcctEntityName, _AcctLookupField1, _AcctLookupField2, _AcctFieldValue1, _AcctFieldValue2));
            #endregion


            #region "QueryExpression Execution"
            QueryExpression qe = new QueryExpression();
            qe.EntityName = _AcctEntityName;
            qe.ColumnSet = new ColumnSet("accountid");
            //prevent query from running forever with duplicate records
            qe.PageInfo = new PagingInfo();
            qe.PageInfo.Count = 2;
            qe.PageInfo.PageNumber = 1;
            
            FilterExpression filter = new FilterExpression(LogicalOperator.And);
            if (_AcctLookupField1 != null && _AcctFieldValue1 != "")
            {
                ConditionExpression condition1 = new ConditionExpression();
                condition1.AttributeName = _AcctLookupField1;
                condition1.Values.Add(_AcctFieldValue1);
                condition1.Operator = ConditionOperator.Equal;
                filter.Conditions.Add(condition1);
            }
            if (_AcctLookupField2 != null && _AcctFieldValue2 != "")
            {
                ConditionExpression condition2 = new ConditionExpression();
                condition2.AttributeName = _AcctLookupField2;
                condition2.Values.Add(_AcctFieldValue2);
                condition2.Operator = ConditionOperator.Equal;
                filter.Conditions.Add(condition2);
            }
            ConditionExpression condition3 = new ConditionExpression();
            condition3.AttributeName = "statecode";
            condition3.Values.Add(0); //active records 
            condition3.Operator = ConditionOperator.Equal;
            filter.Conditions.Add(condition3);

            qe.Criteria=filter;

            EntityCollection AcctResults= objCommon.service.RetrieveMultiple(qe);
            if (AcctResults.Entities.Count > 0)
            {
                AcctFoundOne.Set(executionContext, true);
                AcctOut.Set(executionContext, new EntityReference("account", AcctResults[0].Id));

            }
            if (AcctResults.Entities.Count > 1)
            {
                AcctFoundMultiple.Set(executionContext, true);
            }

            #endregion

        }
    }
}
