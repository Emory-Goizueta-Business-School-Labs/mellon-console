using System;
namespace MellonConsole
{
	public class ToDo
	{
		public int Id { get; set; }
		public string Text { get; set; } = "";
		public bool IsDone { get; set; } = false;

		public ToDo()
		{
		}
	}
}

