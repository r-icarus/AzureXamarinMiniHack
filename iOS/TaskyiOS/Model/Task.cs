using System;
using System.Runtime.Serialization;

namespace Tasky.Core
{
	public class Task
	{
		public int id {get; set;}

		[DataMember (Name="name")]
		public string Name {get; set;}

		[DataMember (Name="notes")]
		public string Notes {get; set;}
	}
}

