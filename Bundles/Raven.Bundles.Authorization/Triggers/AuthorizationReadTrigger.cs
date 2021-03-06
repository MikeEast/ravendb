//-----------------------------------------------------------------------
// <copyright file="AuthorizationReadTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;
using Raven.Abstractions.Data;
using Raven.Database;
using Raven.Database.Plugins;
using Raven.Http;
using Raven.Json.Linq;

namespace Raven.Bundles.Authorization.Triggers
{
	public class AuthorizationReadTrigger : AbstractReadTrigger
	{
		public AuthorizationDecisions AuthorizationDecisions { get; set; }

		public override void Initialize()
		{
			AuthorizationDecisions = new AuthorizationDecisions(Database);	
		}

		public override ReadVetoResult AllowRead(string key, RavenJObject metadata, ReadOperation readOperation,
		                                         TransactionInformation transactionInformation)
		{
			using (Database.DisableAllTriggersForCurrentThread())
			{
				var user = CurrentOperationContext.Headers.Value[Constants.RavenAuthorizationUser];
				var operation = CurrentOperationContext.Headers.Value[Constants.RavenAuthorizationOperation];
				if (string.IsNullOrEmpty(operation) || string.IsNullOrEmpty(user))
					return ReadVetoResult.Allowed;

				var sw = new StringWriter();
				var isAllowed = AuthorizationDecisions.IsAllowed(user, operation, key, metadata, sw.WriteLine);
				if (isAllowed)
					return ReadVetoResult.Allowed;
				return readOperation == ReadOperation.Query ?
					ReadVetoResult.Ignore :
					ReadVetoResult.Deny(sw.GetStringBuilder().ToString());
			}
		}
	}
}
