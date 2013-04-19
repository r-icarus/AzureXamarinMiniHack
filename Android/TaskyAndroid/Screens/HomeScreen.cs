using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Tasky.Core;
using Microsoft.WindowsAzure.MobileServices;

namespace TaskyAndroid.Screens {
	/// <summary>
	/// Main ListView screen displays a list of tasks, plus an [Add] button
	/// </summary>
	[Activity (Label = "Tasky", MainLauncher = true, Icon="@drawable/icon")]			
	public class HomeScreen : Activity {
		protected Adapters.TaskListAdapter taskList;
		protected IList<Task> tasks;
		protected Button addTaskButton = null;
		protected Button loginButton = null;
		protected ListView taskListView = null;

		MobileServiceUser user = null;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			// set our layout to be the home screen
			SetContentView(Resource.Layout.HomeScreen);

			//Find our controls
			taskListView 	= FindViewById<ListView> (Resource.Id.lstTasks);
			addTaskButton 	= FindViewById<Button> (Resource.Id.btnAddTask);
			loginButton		= FindViewById<Button> (Resource.Id.btnLogin);	

			// wire up add task button handler
			if(addTaskButton != null) {
				addTaskButton.Click += (sender, e) => {
					StartActivity(typeof(TaskDetailsScreen));
				};
			}

			// wire up add task button handler
			if(loginButton != null) {
				loginButton.Click += (sender, e) => {
					TaskManager.MobileService.LoginAsync(this, MobileServiceAuthenticationProvider.Twitter).ContinueWith(t =>
					                                                                                                     {
						RunOnUiThread (() =>
						                         {
							user = t.Result;
							var toast = Toast.MakeText(this.ApplicationContext,
							                           "You are now logged in and your user id is " + user.UserId,
							                           ToastLength.Short);							                           							
							TaskManager.MobileService.CurrentUser = user;
							toast.Show();
						});
					});					
				};
			}
			
			// wire up task click handler
			if(taskListView != null) {
				taskListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					var taskDetails = new Intent (this, typeof (TaskDetailsScreen));
					taskDetails.PutExtra ("TaskID", tasks[e.Position].id);
					StartActivity (taskDetails);
				};
			}
		}
		
		protected override void OnResume ()
		{
			base.OnResume ();

			tasks = TaskManager.GetTasks();
			
			// create our adapter
			taskList = new Adapters.TaskListAdapter(this, tasks);

			//Hook up our adapter to our ListView
			taskListView.Adapter = taskList;
		}
	}
}