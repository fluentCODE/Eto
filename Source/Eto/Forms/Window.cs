using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public enum WindowState
	{
		Normal,
		Maximized,
		Minimized
	}

	public enum WindowStyle
	{
		Default,
		None
	}

	public abstract class Window : Panel
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		#region Events

		public const string ClosedEvent = "Window.Closed";

		public event EventHandler<EventArgs> Closed
		{
			add { Properties.AddHandlerEvent(ClosedEvent, value); }
			remove { Properties.RemoveEvent(ClosedEvent, value); }
		}

		protected virtual void OnClosed(EventArgs e)
		{
			OnUnLoad(EventArgs.Empty);
			Properties.TriggerEvent(ClosedEvent, this, e);
		}

		public const string ClosingEvent = "Window.Closing";

		public event EventHandler<CancelEventArgs> Closing
		{
			add { Properties.AddHandlerEvent(ClosingEvent, value); }
			remove { Properties.RemoveEvent(ClosingEvent, value); }
		}

		protected virtual void OnClosing(CancelEventArgs e)
		{
			Properties.TriggerEvent(ClosingEvent, this, e);
		}

		public const string LocationChangedEvent = "Window.LocationChanged";

		public event EventHandler<EventArgs> LocationChanged
		{
			add { Properties.AddHandlerEvent(LocationChangedEvent, value); }
			remove { Properties.RemoveEvent(LocationChangedEvent, value); }
		}

		protected virtual void OnLocationChanged(EventArgs e)
		{
			Properties.TriggerEvent(LocationChangedEvent, this, e);
		}

		public const string WindowStateChangedEvent = "Window.WindowStateChanged";

		public event EventHandler<EventArgs> WindowStateChanged
		{
			add { Properties.AddHandlerEvent(WindowStateChangedEvent, value); }
			remove { Properties.RemoveEvent(WindowStateChangedEvent, value); }
		}

		protected virtual void OnWindowStateChanged(EventArgs e)
		{
			Properties.TriggerEvent(WindowStateChangedEvent, this, e);
		}


		#endregion

		static Window()
		{
			EventLookup.Register<Window>(c => c.OnClosed(null), ClosedEvent);
			EventLookup.Register<Window>(c => c.OnClosing(null), ClosingEvent);
			EventLookup.Register<Window>(c => c.OnLocationChanged(null), LocationChangedEvent);
			EventLookup.Register<Window>(c => c.OnWindowStateChanged(null), WindowStateChangedEvent);
		}

		protected Window()
		{
		}

		protected Window(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Window"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the handler instance</param>
		/// <param name="type">Type of interface to create for the handler, must implement <see cref="IHandler"/></param>
		/// <param name="initialize"><c>true</c> to initialize the handler, false if the subclass will initialize</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Window(Generator generator, Type type, bool initialize = true)
			: base(generator, type, false)
		{
			if (initialize)
				Initialize();
			HandleEvent(ClosedEvent);
		}

		/// <summary>
		/// Gets or sets the title of the window
		/// </summary>
		/// <remarks>
		/// The title of the window is displayed to the user usually at the top of the window, but in cases where
		/// you show a window in a mobile environment, this may be the title shown in a navigation controller.
		/// </remarks>
		/// <value>The title of the window</value>
		public string Title
		{
			get { return Handler.Title; }
			set { Handler.Title = value; }
		}

		/// <summary>
		/// Gets or sets the location of the window
		/// </summary>
		/// <remarks>
		/// Note that in multi-monitor setups, the origin of the location is at the upper-left of <see cref="Eto.Forms.Screen.PrimaryScreen"/>
		/// </remarks>
		public new Point Location
		{
			get { return Handler.Location; }
			set { Handler.Location = value; }
		}

		/// <summary>
		/// Gets or sets the size and location of the window
		/// </summary>
		/// <value>The bounding rectangle of the window</value>
		public new Rectangle Bounds
		{
			get { return new Rectangle(Handler.Location, Handler.Size); }
			set
			{
				Handler.Location = value.Location;
				Handler.Size = value.Size;
			}
		}

		/// <summary>
		/// Gets or sets the tool bar for the window.
		/// </summary>
		/// <remarks>
		/// Note that each window can only have a single tool bar
		/// </remarks>
		/// <value>The tool bar for the window</value>
		public ToolBar ToolBar
		{
			get { return Handler.ToolBar; }
			set
			{ 
				var toolbar = Handler.ToolBar;
				if (toolbar != null)
					toolbar.OnUnLoad(EventArgs.Empty);
				Handler.ToolBar = value;
				if (value != null)
					value.OnLoad(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the opacity of the window
		/// </summary>
		/// <value>The window opacity.</value>
		public double Opacity
		{
			get { return Handler.Opacity; }
			set { Handler.Opacity = value; }
		}

		/// <summary>
		/// Closes the window
		/// </summary>
		/// <remarks>
		/// Note that once a window is closed, it cannot be shown again in most platforms.
		/// </remarks>
		public virtual void Close()
		{
			Handler.Close();
		}

		/// <summary>
		/// Gets the screen this window is mostly contained in. Typically defined by the screen center of the window is visible.
		/// </summary>
		/// <value>The window's current screen.</value>
		public Screen Screen
		{
			get { return Handler.Screen; }
		}

		public virtual MenuBar Menu
		{
			get { return Handler.Menu; }
			set
			{
				var menu = Handler.Menu;
				if (menu != null)
					menu.OnUnLoad(EventArgs.Empty);
				if (value != null && value.AutoTrim)
				{
					value.Items.Trim();
				}
				Handler.Menu = value;
				if (value != null)
					value.OnLoad(EventArgs.Empty);
			}
		}

		public Icon Icon
		{
			get { return Handler.Icon; }
			set { Handler.Icon = value; }
		}

		public bool Resizable
		{
			get { return Handler.Resizable; }
			set { Handler.Resizable = value; }
		}

		public bool Maximizable
		{
			get { return Handler.Maximizable; }
			set { Handler.Maximizable = value; }
		}

		public bool Minimizable
		{
			get { return Handler.Minimizable; }
			set { Handler.Minimizable = value; }
		}

		public bool ShowInTaskbar
		{
			get { return Handler.ShowInTaskbar; }
			set { Handler.ShowInTaskbar = value; }
		}

		public bool Topmost
		{
			get { return Handler.Topmost; }
			set { Handler.Topmost = value; }
		}

		public WindowState WindowState
		{
			get { return Handler.WindowState; }
			set { Handler.WindowState = value; }
		}

		public Rectangle? RestoreBounds
		{
			get { return Handler.RestoreBounds; }
		}

		public void Minimize()
		{
			Handler.WindowState = WindowState.Minimized;
		}

		public void Maximize()
		{
			Handler.WindowState = WindowState.Maximized;
		}

		public WindowStyle WindowStyle
		{
			get { return Handler.WindowStyle; }
			set { Handler.WindowStyle = value; }
		}

		public void BringToFront()
		{
			Handler.BringToFront();
		}

		public void SendToBack()
		{
			Handler.SendToBack();
		}

		#region Callback

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : Panel.ICallback
		{
			void OnClosed(Window widget, EventArgs e);
			void OnClosing(Window widget, CancelEventArgs e);
			void OnLocationChanged(Window widget, EventArgs e);
			void OnWindowStateChanged(Window widget, EventArgs e);
		}

		protected class Callback : Panel.Callback, ICallback
		{
			public void OnClosed(Window widget, EventArgs e)
			{
				widget.OnClosed(e);
			}
			public void OnClosing(Window widget, CancelEventArgs e)
			{
				widget.OnClosing(e);
			}
			public void OnLocationChanged(Window widget, EventArgs e)
			{
				widget.OnLocationChanged(e);
			}
			public void OnWindowStateChanged(Window widget, EventArgs e)
			{
				widget.OnWindowStateChanged(e);
			}
		}

		#endregion

		#region Handler

		public interface IHandler : Panel.IHandler
		{
			ToolBar ToolBar { get; set; }

			void Close();

			new Point Location { get; set; }

			double Opacity { get; set; }

			string Title { get; set; }

			Screen Screen { get; }

			MenuBar Menu { get; set; }

			Icon Icon { get; set; }

			bool Resizable { get; set; }

			bool Maximizable { get; set; }

			bool Minimizable { get; set; }

			bool ShowInTaskbar { get; set; }

			bool Topmost { get; set; }

			WindowState WindowState { get; set; }

			Rectangle? RestoreBounds { get; }

			WindowStyle WindowStyle { get; set; }

			void BringToFront();

			void SendToBack();
		}

		#endregion
	}
}
