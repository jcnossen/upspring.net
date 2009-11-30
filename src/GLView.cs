using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tao.Platform.Windows;
using Tao.OpenGl;
using Tao.DevIl;

namespace UpspringSharp
{
	public partial class GLView : UserControl
	{
		public GLView()
		{
			InitializeComponent();
		}

		private void GLView_Load(object sender, EventArgs e)
		{
			glControl.InitializeContexts();

			if (DesignMode)
				return;

			MakeCurrent();
			Ilut.ilutInit();

//			glFont = GLFont.LoadFont("Times", 10.0f, System.Drawing.FontStyle.Regular);

		}

		public new void MakeCurrent() {
			glControl.MakeCurrent();
		}
	}
}
