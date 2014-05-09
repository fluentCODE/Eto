using System;

namespace Eto.Threading
{
	[Handler(typeof(Thread.IHandler))]
	public class Thread : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		readonly Action action;

		public static Thread CurrentThread
		{
			get
			{
				var thread = new Thread();
				thread.Handler.CreateCurrent();
				thread.Initialize();
				return thread;
			}
		}

		public static Thread MainThread
		{
			get
			{
				var thread = new Thread();
				thread.Handler.CreateMain();
				thread.Initialize();
				return thread;
			}
		}

		Thread()
		{
		}

		public Thread(Action action)
		{
			this.action = action;
			Handler.Create();
			Initialize();
		}

		protected virtual void OnExecuted()
		{
			if (action != null)
				action();
		}
		
		public void Start()
		{
			Handler.Start();
		}
		
		public void Abort()
		{
			Handler.Abort();
		}
		
		public bool IsAlive
		{
			get { return Handler.IsAlive; }
		}
		
		public bool IsMain
		{
			get { return Handler.IsMain; }
		}
		
		public static bool IsMainThread
		{
			get { return CurrentThread.IsMain; }
		}

		#pragma warning disable 612,618

		[Obsolete("Use constructor without generator instead")]
		public Thread(Action action, Generator generator = null)
			: base(generator, typeof(IHandler), false)
		{
			this.action = action;
			Handler.Create();
			Initialize();
		}

		#pragma warning restore 612,618

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : Widget.ICallback
		{
			void OnExecuted(Thread widget);
		}

		protected class Callback : ICallback
		{
			public void OnExecuted(Thread widget)
			{
				widget.OnExecuted();
			}
		}

		[AutoInitialize(false)]
		public interface IHandler : Widget.IHandler
		{
			void Create();

			void CreateCurrent();

			void CreateMain();

			void Start();

			void Abort();

			bool IsAlive { get; }

			bool IsMain { get; }
		}
	}
}
