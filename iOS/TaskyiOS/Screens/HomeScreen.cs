using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using Tasky.Core;
using Tasky.ApplicationLayer;
using Microsoft.WindowsAzure.MobileServices;

namespace Tasky.Screens {

	/// <summary>
	/// A UITableViewController that uses MonoTouch.Dialog - displays the list of Tasks
	/// </summary>
	public class HomeScreen : DialogViewController {
		// 
		List<Task> tasks;
		MobileServiceUser user;

		
		// MonoTouch.Dialog individual TaskDetails view (uses /AL/TaskDialog.cs wrapper class)
		BindingContext context;
		TaskDialog taskDialog;
		Task currentTask;
		DialogViewController detailsScreen;

		public HomeScreen () : base (UITableViewStyle.Plain, null)
		{
			Initialize ();
		}
		
		protected void Initialize()
		{
			NavigationItem.SetRightBarButtonItem (new UIBarButtonItem (UIBarButtonSystemItem.Add), false);
			NavigationItem.RightBarButtonItem.Clicked += (sender, e) => { ShowTaskDetails(new Task()); };
			NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem("Login",UIBarButtonItemStyle.Bordered,LoginHandler), false);
		}

		public void LoginHandler(object sender, EventArgs e) 
		{
			TaskManager.MobileService.LoginAsync(this, MobileServiceAuthenticationProvider.Twitter).ContinueWith(t => 
			                                                                                                     {
				BeginInvokeOnMainThread (() => 
				                         {
					user = t.Result;
					var alert = new UIAlertView("Welcome!",  
					                            "You are now logged in and your user id is " 
					                            + user.UserId, null, "OK");     
					TaskManager.MobileService.CurrentUser = user;
					alert.Show ();
				});
			});		
		}
		
		protected void ShowTaskDetails(Task task)
		{
			currentTask = task;
			taskDialog = new TaskDialog (task);
			context = new BindingContext (this, taskDialog, "Task Details");
			detailsScreen = new DialogViewController (context.Root, true);
			ActivateController(detailsScreen);
		}
		public void SaveTask()
		{
			context.Fetch (); // re-populates with updated values
			currentTask.Name = taskDialog.Name;
			currentTask.Notes = taskDialog.Notes;
			TaskManager.SaveTask(currentTask);
			NavigationController.PopViewControllerAnimated (true);
			context.Dispose (); // per documentation
		}
		public void DeleteTask ()
		{
			if (currentTask.id >= 0)
				TaskManager.DeleteTask (currentTask);
			NavigationController.PopViewControllerAnimated (true);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			// reload/refresh
			PopulateTable();
		}
		
		protected void PopulateTable()
		{
			tasks = TaskManager.GetTasks().ToList ();
			Root = new RootElement("Tasky") {
				new Section() {
					from t in tasks
					select (Element) new StringElement((t.Name==""?"<new task>":t.Name), t.Notes)
				}
			}; 
		}
		public override void Selected (MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var task = tasks[indexPath.Row];
			ShowTaskDetails(task);
		}
		public override Source CreateSizingSource (bool unevenRows)
		{
			return new EditingSource (this);
		}
		public void DeleteTaskRow(int rowId)
		{
			TaskManager.DeleteTask(tasks[rowId]);
		}
	}
}