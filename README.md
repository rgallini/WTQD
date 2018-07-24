Install the managed solution, or upload the .dll with the Plugin Registration Utility from the SDK.

In your workflow, add the custom activity for the entity you want to look up.
In the details for the activity, enter at least one lookup field and value.
For Lookup Field 1, enter the field name (not the schema name or display name) i.e. "emailaddress1" (field name) not "EmailAddress1" (schema name) or "Email Address 1" (display name). If you are not sure, look it up in the entity fields.
For Field1 Value, enter the value for that field (can be a text field from the entity or a result from another workflow step)

If you enter a second field, it is used as an "And" not "Or". (If you want "Or" use a second step in your workflow)

In future actions, you can use one of three outputs:
1. FoundOne (bool) will be True if the query found only one match. I use this as a condition in the workflow.
2. (entity)Out will be the record found
3. FoundMultiple (bool) will be True if the query found two matches. I use this to determine if I might have duplicates and want to alert someone to clean up the data.
