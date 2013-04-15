using System;
using Tasky.Core;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;

namespace Tasky.Core
{
	public class TaskManager
	{
		public static MobileServiceClient MobileService = new MobileServiceClient(
			"***azureUrl***","***apiKey***"
		);

		private static IMobileServiceTable<Task> table = MobileService.GetTable<Task>();

		public TaskManager ()
		{
		}

		public static Task GetTask (int taskID)
		{
			var asyncTask = table.LookupAsync(taskID);
			asyncTask.Wait();
			return asyncTask.Result;
		}

		public static void SaveTask (Task task)
		{		
			var asyncTask = table.InsertAsync(task);
			asyncTask.Wait();
		}

		public static void DeleteTask (Task task)
		{		
			var asyncTask = table.DeleteAsync(task);
			asyncTask.Wait();
		}

		public static IList<Task> GetTasks ()
		{		
			var asyncTask = table.ToListAsync();
			asyncTask.Wait();
			return asyncTask.Result;
		}
	}
}

