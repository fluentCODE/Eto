using System;

namespace Eto.Forms
{
	public abstract class Cell : Widget
	{
		protected Cell()
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Cell (Generator g, Type type, bool initialize)
			: base(g, type, initialize)
		{
		}

		public interface IHandler
		{
		}
	}
}

