using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using System.Runtime.InteropServices;

namespace Eto.WinForms.Forms
{
	public class FormHandler : WindowHandler<swf.Form, Form, Form.ICallback>, Form.IHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        public class EtoForm : swf.Form
		{
            bool hideFromAltTab;

            public EtoForm()
            {
            }

            public new sd.Rectangle MaximizedBounds
            {
                get
                {
                    return base.MaximizedBounds;
                }

                set
                {
                    base.MaximizedBounds = value;
                }
            }

			public bool HideFromAltTab
			{
				get { return hideFromAltTab; }
				set
				{
					if (hideFromAltTab != value)
					{
						hideFromAltTab = value;
						if (IsHandleCreated)
						{
							var style = Win32.GetWindowLong(Handle, Win32.GWL.EXSTYLE);
							if (hideFromAltTab)
								style |= (uint)Win32.WS_EX.TOOLWINDOW;
							else
								style &= (uint)~Win32.WS_EX.TOOLWINDOW;

							Win32.SetWindowLong(Handle, Win32.GWL.EXSTYLE, style);
						}
					}
				}
			}

			public bool ShouldShowWithoutActivation { get; set; }

            public int HeadingSize { get; set; }

			protected override bool ShowWithoutActivation
			{
				get { return ShouldShowWithoutActivation; }
			}

			protected override swf.CreateParams CreateParams
			{
				get
				{
					var createParams = base.CreateParams;
                    
					if (hideFromAltTab)
						createParams.ExStyle |= (int)Win32.WS_EX.TOOLWINDOW;

                    if (createParamsHack)
                        createParams.Style &= ~(int)(Win32.WS.BORDER | Win32.WS.CAPTION | Win32.WS.DLGFRAME | Win32.WS.THICKFRAME);

                    return createParams;
				}
			}

            protected override void SetClientSizeCore(int x, int y)
            {
                createParamsHack = true;
                base.SetClientSizeCore(x, y);
                createParamsHack = false;
            }

            const int WM_NCCALCSIZE = 0x0083;
            const int WM_SYSCOMMAND = 0x0112;
            const int WM_MOUSEMOVE = 0x0200;
            const int WM_SIZE = 0x0005;
            const int WM_NCHITTEST = 0x0084;
            const int WM_GETMINMAXINFO = 0x0024;
            const int WM_WINDOWPOSCHANGED = 0x0047;
            // --- might not use
            const int WM_PAINT = 0;
            const int WM_ERASEBKGND = 0;

            // hit test items
            const int HTBORDER = 18;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTLEFT = 10;
            const int HTTOPRIGHT = 14;
            const int HTRIGHT = 11;
            const int HTCAPTION = 2;
            const int HTCLIENT = 1;
            const int HTCLOSE = 20;
            const int HTERROR = -2;
            const int HTGROWBOX = 4;
            const int HTSIZE = 4;
            const int HTHELP = 21;
            const int HTMENU = 5;
            const int HTMINBUTTON = 8;
            const int HTMAXBUTTON = 9;
            const int HTREDUCE = 8;
            const int HTZOOM = 9;
            const int HTNOWHERE = 0;
            const int HTSYSMENU = 3;
            const int HTTRANSPARENT = -1;
            const int HTVSCROLL = 7;

            private bool createParamsHack = false;
            
            protected override void WndProc(ref swf.Message m)
            {
                switch (m.Msg)
                {
                    case WM_WINDOWPOSCHANGED:
                        createParamsHack = true;
                        base.WndProc(ref m);
                        createParamsHack = false;
                        return;
                    case WM_NCCALCSIZE:
                        if (FormBorderStyle != swf.FormBorderStyle.None)
                        {
                            if (m.WParam.Equals(IntPtr.Zero))
                            {
                                Win32.RECT rc = (Win32.RECT)m.GetLParam(typeof(Win32.RECT));
                                Rectangle r = rc.ToRectangle();
                                r.Height--;
                                Marshal.StructureToPtr(new Win32.RECT(r), m.LParam, true);
                            }
                            else
                            {
                                Win32.NCCALCSIZE_PARAMS csp = (Win32.NCCALCSIZE_PARAMS)m.GetLParam(typeof(Win32.NCCALCSIZE_PARAMS));
                                Rectangle r = csp.rgrc0.ToRectangle();
                                r.Height--;
                                csp.rgrc0 = new Win32.RECT(r);
                                Marshal.StructureToPtr(csp, m.LParam, true);
                            }
                        }
                        m.Result = IntPtr.Zero;
                        return;
                    case WM_NCHITTEST:
                        var borderWidth = 8;
                        var point = new sd.Point(m.LParam.ToInt32());
                        var result = HTCLIENT;

                        if (WindowState == System.Windows.Forms.FormWindowState.Maximized)
                        {
                            if (HeadingSize != 0 && point.Y >= Location.Y && point.Y <= Location.Y + HeadingSize)
                                result = HTCAPTION;

                            m.Result = (IntPtr)result;
                            return;
                        }

                        if (point.Y >= Location.Y && point.Y <= Location.Y + borderWidth)
                        {
                            if (point.X >= Location.X && point.X <= Location.X + borderWidth)
                            {
                                result = HTTOPLEFT;
                            }
                            else if (point.X < Location.X + Size.Width && point.X >= Location.X + Size.Width - borderWidth)
                            {
                                result = HTTOPRIGHT;
                            }
                            else
                            {
                                result = HTTOP;
                            }
                        }
                        else if (HeadingSize != 0 && point.Y >= Location.Y && point.Y <= Location.Y + HeadingSize)
                        {
                            result = HTCAPTION;

                            if (point.X >= Location.X && point.X <= Location.X + borderWidth && point.Y <= Location.Y + borderWidth)
                            {
                                result = HTTOPLEFT;
                            }
                            else if (point.X < Location.X + Size.Width && point.X >= Location.X + Size.Width - borderWidth && point.Y <= Location.Y + borderWidth)
                            {
                                result = HTTOPRIGHT;
                            }
                            else if (point.X >= Location.X && point.X <= Location.X + borderWidth)
                            {
                                result = HTLEFT;
                            }
                            else if (point.X <= Location.X + Size.Width && point.X >= Location.X + Size.Width - borderWidth)
                            {
                                result = HTRIGHT;
                            }
                        }
                        else if (point.Y <= Location.Y + Size.Height && point.Y >= Location.Y + Size.Height - borderWidth)
                        {
                            if (point.X >= Location.X && point.X <= Location.X + borderWidth)
                            {
                                result = HTBOTTOMLEFT;
                            }
                            else if (point.X < Location.X + Size.Width && point.X >= Location.X + Size.Width - borderWidth)
                            {
                                result = HTBOTTOMRIGHT;
                            }
                            else
                            {
                                result = HTBOTTOM;
                            }
                        }
                        else
                        {
                            if (point.X >= Location.X && point.X <= Location.X + borderWidth)
                            {
                                result = HTLEFT;
                            }
                            else if (point.X <= Location.X + Size.Width && point.X >= Location.X + Size.Width - borderWidth)
                            {
                                result = HTRIGHT;
                            }
                        }
                        
                        m.Result = (IntPtr)result;
                        return;
                }

                base.WndProc(ref m);
            }
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        public FormHandler(swf.Form form)
		{
			Control = form;
		}

		public FormHandler()
		{
			Control = new EtoForm
			{
				StartPosition = swf.FormStartPosition.CenterParent,
				AutoSize = true,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty
			};
		}

        private MARGINS margins = new MARGINS
        {
            Left = 1,
            Right = 1,
            Top = 1,
            Bottom = 1
        };

		protected override void Initialize()
		{
			base.Initialize();
			Resizable = true;
            DwmExtendFrameIntoClientArea(this.Control.Handle, ref margins);
        }

		public void Show()
		{
			Control.Show();
		}

		public override bool ShowInTaskbar
		{
			get { return base.ShowInTaskbar; }
			set
			{
				base.ShowInTaskbar = value;
				var myForm = Control as EtoForm;
				if (myForm != null)
					myForm.HideFromAltTab = !value;
			}
		}

        public int HeadingSize
        {
            get { return ((EtoForm)Control).HeadingSize; }
            set { ((EtoForm)Control).HeadingSize = value; }
        }

		public Color TransparencyKey
		{
			get { return Control.TransparencyKey.ToEto(); }
			set { Control.TransparencyKey = value.ToSD(); }
		}


		public bool KeyPreview
		{
			get { return Control.KeyPreview; }
			set { Control.KeyPreview = value; }
		}
    }
}
