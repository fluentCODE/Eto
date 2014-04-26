using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Platform.Android.Forms.Controls
{
	public abstract class AndroidCommonControl<T, TWidget> : AndroidControl<T, TWidget>, ICommonControl
		where TWidget: CommonControl
		where T: av.View
	{
		public override av.View ContainerControl
		{
			get { return Control; }
		}

		public Font Font
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override Size Size
		{
			get { return new Size(Control.Width, Control.Height); }
			set
			{ 
				// TODO: need to change to desired size, not min size.. e.g. if control is in a container
				Control.SetMinimumWidth(value.Width);
				Control.SetMinimumHeight(value.Height);
			}
		}

		public override bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
	}
}