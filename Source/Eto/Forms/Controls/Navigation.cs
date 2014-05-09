using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	[Handler(typeof(Navigation.IHandler))]
	public class Navigation : Container
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public override IEnumerable<Control> Controls
		{
			get
			{
				yield break;
			}
		}

		public static bool IsSupported
		{
			get { return Platform.Instance.Supports<IHandler>(); }
		}

		public event EventHandler<EventArgs> ItemShown;
		
		public virtual void OnItemShown (EventArgs e)
		{
			if (ItemShown != null)
				ItemShown (this, e);
		}

		public Navigation()
		{
		}

		[Obsolete("Use default constructor instead")]
		public Navigation (Generator generator)
			: base(generator, typeof(IHandler))
		{
		}
		
		public Navigation (Control content, string title = null)
			: this()
		{
			Push (content, title);
		}
		
		public Navigation (NavigationItem item)
			: this()
		{
			Push (item);
		}
		
		public void Push (Control content, string title = null)
		{
			Push (new NavigationItem { Content = content, Text = title });
		}
		
		public void Push (INavigationItem item)
		{
			SetParent(item.Content, () => Handler.Push(item));
		}
		
		public virtual void Pop ()
		{
			Handler.Pop ();
		}

		public override void Remove(Control child)
		{
			//throw new NotImplementedException();
		}

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : Container.ICallback
		{
			void OnItemShown(Navigation widget, EventArgs e);
		}

		protected class Callback : Container.Callback, ICallback
		{
			public void OnItemShown(Navigation widget, EventArgs e)
			{
				widget.OnItemShown(e);
			}
		}

		public interface IHandler : Container.IHandler
		{
			void Push (INavigationItem item);

			void Pop ();
		}
	}
}

