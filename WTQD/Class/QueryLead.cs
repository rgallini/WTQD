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
    public class QueryLeads: CodeActivity
    {
        #region "Parameter Definition"
        [RequiredArgument]
        [Input("Contact Lookup Field1* (text field)")]
        [ReferenceTarget("")]
        public InArgument<String> LeadLookupField1 { get; set; }
        
        [RequiredArgument]
        [Input("Field1 Value* (string)")]
        [ReferenceTarget("")]
        public InArgument<String> LeadFieldValue1 { get; set; }
        
        [Input("Contact Lookup Field2 (text field)")]
        [ReferenceTarget("")]
        public InArgument<String> LeadLookupField2 { get; set; }
        
        [Input("Field2 Value (string)")]
        [ReferenceTarget("")]
        public InArgument<String> LeadFieldValue2 { get; set; }

        [Output("FoundOne")]
        public OutArgument<bool> LeadFoundOne { get; set; }
        
        [Output("Output")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> LeadOut { get; set; }

        [Output("FoundMultiple")]
        public OutArgument<bool> LeadFoundMultiple { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext executionContext)
        {

            #region "Load CRM Service from context"

            Common objCommon = new Common(executionContext);
            objCommon.tracingService.Trace("Load CRM Service from context --- OK");
            #endregion

            #region "Read Parameters"
            String _LeadEntityName = "lead";
            String _LeadLookupField1 = this.LeadLookupField1.Get(executionContext);
            String _LeadLookupField2 = this.LeadLookupField2.Get(executionContext);
            String _LeadFieldValue1 = this.LeadFieldValue1.Get(executionContext);
            String _LeadFieldValue2 = this.LeadFieldValue2.Get(executionContext);

            objCommon.tracingService.Trace(String.Format("LeadEntityName: {0} - LeadLookupField1:{1} - LeadLookupField2:{2} - LeadFieldValue1:{3} LeadFieldValue2:{4}",
                _LeadEntityName, _LeadLookupField1, _LeadLookupField2, _LeadFieldValue1, _LeadFieldValue2));
            #endregion


            #region "QueryExpression Execution"
            QueryExpression qe = new QueryExpression();
            qe.EntityName = _LeadEntityName;
            qe.ColumnSet = new ColumnSet("leadid");
            //prevent query from running forever with duplicate records
            qe.PageInfo = new PagingInfo();
            qe.PageInfo.Count = 2;
            qe.PageInfo.PageNumber = 1;
            
            FilterExpression filter = new FilterExpression(LogicalOperator.And);
            if (_LeadLookupField1 != null && _LeadFieldValue1 != "")
            {
                ConditionExpression condition1 = new ConditionExpression();
                condition1.AttributeName = _LeadLookupField1;
                condition1.Values.Add(_LeadFieldValue1);
                condition1.Operator = ConditionOperator.Equal;
                filter.Conditions.Add(condition1);
            }
            if (_LeadLookupField2 != null && _LeadFieldValue2 != "")
            {
                ConditionExpression condition2 = new ConditionExpression();
                condition2.AttributeName = _LeadLookupField2;
                condition2.Values.Add(_LeadFieldValue2);
                condition2.Operator = ConditionOperator.Equal;
                filter.Conditions.Add(condition2);
            }
            ConditionExpression condition3 = new ConditionExpression();
            condition3.AttributeName = "statecode";
            condition3.Values.Add(0); //active records 
            condition3.Operator = ConditionOperator.Equal;
            filter.Conditions.Add(condition3);

            qe.Criteria=filter;

            EntityCollection LeadResults= objCommon.service.RetrieveMultiple(qe);
            if (LeadResults.Entities.Count > 0)
            {
                LeadFoundOne.Set(executionContext, true);
                LeadOut.Set(executionContext, new EntityReference("lead", LeadResults[0].Id));

            }
            if (LeadResults.Entities.Count > 1)
            {
                LeadFoundMultiple.Set(executionContext, true);
            }

            #endregion

        }
    }
}
