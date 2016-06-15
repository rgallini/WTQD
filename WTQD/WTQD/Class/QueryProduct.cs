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
    public class QueryProducts: CodeActivity
    {
        #region "Parameter Definition"
        [RequiredArgument]
        [Input("Product Lookup Field1* (text field)")]
        [ReferenceTarget("")]
        public InArgument<String> ProdLookupField1 { get; set; }
        
        [RequiredArgument]
        [Input("Field1 Value* (string)")]
        [ReferenceTarget("")]
        public InArgument<String> ProdFieldValue1 { get; set; }
        
        [Input("Product Lookup Field2 (text field)")]
        [ReferenceTarget("")]
        public InArgument<String> ProdLookupField2 { get; set; }
        
        [Input("Field2 Value (string)")]
        [ReferenceTarget("")]
        public InArgument<String> ProdFieldValue2 { get; set; }

        [Output("FoundOne")]
        public OutArgument<bool> ProdFoundOne { get; set; }
        
        [Output("Output")]
        [ReferenceTarget("product")]
        public OutArgument<EntityReference> ProductOut { get; set; }

        [Output("FoundMultiple")]
        public OutArgument<bool> ProdFoundMultiple { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext executionContext)
        {

            #region "Load CRM Service from context"

            Common objCommon = new Common(executionContext);
            objCommon.tracingService.Trace("Load CRM Service from context --- OK");
            #endregion

            #region "Read Parameters"
            String _ProdEntityName = "product";
            String _ProdLookupField1 = this.ProdLookupField1.Get(executionContext);
            String _ProdLookupField2 = this.ProdLookupField2.Get(executionContext);
            String _ProdFieldValue1 = this.ProdFieldValue1.Get(executionContext);
            String _ProdFieldValue2 = this.ProdFieldValue2.Get(executionContext);

            objCommon.tracingService.Trace(String.Format("ProdEntityName: {0} - ProdLookupField1:{1} - ProdLookupField2:{2} - ProdFieldValue1:{3} ProdFieldValue2:{4}",
                _ProdEntityName, _ProdLookupField1, _ProdLookupField2, _ProdFieldValue1, _ProdFieldValue2));
            #endregion


            #region "QueryExpression Execution"
            QueryExpression qe = new QueryExpression();
            qe.EntityName = _ProdEntityName;
            qe.ColumnSet = new ColumnSet("productid");
            //prevent query from running forever with duplicate records
            qe.PageInfo = new PagingInfo();
            qe.PageInfo.Count = 2;
            qe.PageInfo.PageNumber = 1;
            
            FilterExpression filter = new FilterExpression(LogicalOperator.And);
            if (_ProdLookupField1 != null && _ProdFieldValue1 != "")
            {
                ConditionExpression condition1 = new ConditionExpression();
                condition1.AttributeName = _ProdLookupField1;
                condition1.Values.Add(_ProdFieldValue1);
                condition1.Operator = ConditionOperator.Equal;
                filter.Conditions.Add(condition1);
            }
            if (_ProdLookupField2 != null && _ProdFieldValue2 != "")
            {
                ConditionExpression condition2 = new ConditionExpression();
                condition2.AttributeName = _ProdLookupField2;
                condition2.Values.Add(_ProdFieldValue2);
                condition2.Operator = ConditionOperator.Equal;
                filter.Conditions.Add(condition2);
            }
            ConditionExpression condition3 = new ConditionExpression();
            condition3.AttributeName = "statecode";
            condition3.Values.Add(0); //active records 
            condition3.Operator = ConditionOperator.Equal;
            filter.Conditions.Add(condition3);

            qe.Criteria=filter;

            EntityCollection ProdResults= objCommon.service.RetrieveMultiple(qe);
            if (ProdResults.Entities.Count > 0)
            {
                ProdFoundOne.Set(executionContext, true);
                ProductOut.Set(executionContext, new EntityReference("product", ProdResults[0].Id));

            }
            if (ProdResults.Entities.Count > 1)
            {
                ProdFoundMultiple.Set(executionContext, true);
            }

            #endregion

        }
    }
}
