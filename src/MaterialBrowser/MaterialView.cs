using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Tao.DevIl;

namespace upspring
{
	using CEImage = ce.engine.Image;

	public partial class MaterialView : UserControl
	{
		bool selected;

		string textureFile;
		public string TextureFile { get { return textureFile; } }

		static Color StandardBackColor = Color.LightGray;
		static Color SelectedBackColor = Color.DarkBlue;

		public bool Selected
		{
			get { return selected; }
			set {
				if (selected != value) {
					selected = value;
					BackColor = selected ? SelectedBackColor : StandardBackColor;
				}
			}
		}

		public event EventHandler ViewSelected;

		public MaterialView()
		{
			InitializeComponent();

			BackColor = StandardBackColor;
		}

		public void SetSize(int s)
		{
			int marginX = Width - textureBox.Width;
			int marginY = Height - textureBox.Height;

			Size = new Size(marginX + s, marginY + s);
		}

		public static MaterialView FromVFSFile (string file)
		{
			byte[] data = Program.FileSystem.ReadAllBytes(file);
			CEImage img = new CEImage(data);

			MaterialView mv = new MaterialView();
			mv.textureBox.Image = img.ToBitmap();
			mv.labelName.Text = Path.GetFileName( file );
			mv.labelSize.Text = string.Format("{0} x {1}", img.Width, img.Height);

			mv.textureFile = file;

			return mv;
		}

		private void textureBox_Click(object sender, EventArgs e)
		{
			HandleSelect();
		}

		private void HandleSelect()
		{
			Selected = true;

			if (ViewSelected != null)
				ViewSelected(this, null);
		}

		private void labelSize_Click(object sender, EventArgs e)
		{
			HandleSelect();
		}

		private void labelName_Click(object sender, EventArgs e)
		{
			HandleSelect();
		}
	}
}
