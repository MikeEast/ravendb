﻿namespace Raven.Studio.Commands
{
	using System.ComponentModel.Composition;
	using Caliburn.Micro;
	using Features.Database;
	using Features.Statistics;

	public class OpenErrorsScreen
	{
		readonly DatabaseExplorer database;
		readonly ErrorsViewModel errors;

		[ImportingConstructor]
		public OpenErrorsScreen(DatabaseExplorer database, ErrorsViewModel errors)
		{
			this.database = database;
			this.errors = errors;
		}

		public void Execute()
		{
			if (!database.IsActive)
			{
				((IConductor) database.Parent).ActivateItem(database);
			}

			database.Show(errors);
		}
	}
}