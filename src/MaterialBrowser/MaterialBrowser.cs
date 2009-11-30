using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ce.util.vfs;

namespace upspring
{
	public partial class MaterialBrowser : UserControl
	{
		public event Action<string> TextureSelected;
		void OnTextureSelected(string t) {
			if (TextureSelected != null)
				TextureSelected(t);
		}

		class TextureCategory
		{
			public FileSystem.Directory directory;

			public TextureCategory (FileSystem.Directory dir) {
				directory=dir;
			}
			public override string ToString() { return directory.name; }
		}

		public MaterialBrowser()
		{
			InitializeComponent();
		}

		private void MaterialBrowser_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;

			comboSizes.SelectedIndex = 2;

			var dir = Program.FileSystem.GetDirectory("textures");
			if (dir == null)
				return;

			comboDirectory.Items.Clear();
			comboDirectory.Items.Add(new TextureCategory(dir));

			foreach (var subdir in dir.subdirs) {
				comboDirectory.Items.Add(new TextureCategory(subdir));
			}

			comboDirectory.SelectedIndex = 0;
		}

		private void comboDirectory_SelectedIndexChanged(object sender, EventArgs e)
		{
			ShowCategory((TextureCategory)comboDirectory.SelectedItem);
		}

		private void ShowCategory(TextureCategory cat)
		{
			// Clear events to allow GC to collect the view
			foreach (Control c in panelMaterials.Controls) 
				((MaterialView)c).ViewSelected -= view_ViewSelected;

			panelMaterials.Controls.Clear();

			foreach (var file in cat.directory.files) {
				MaterialView view = MaterialView.FromVFSFile(file.Name);
				if (view != null) {
					view.ViewSelected += new EventHandler(view_ViewSelected);

					view.SetSize(SelectedDisplaySize);
					panelMaterials.Controls.Add(view);
				}
			}
		}

		int SelectedDisplaySize { get { return 32 << comboSizes.SelectedIndex; } }

		void view_ViewSelected(object sender, EventArgs e)
		{
			MaterialView selmv = (MaterialView)sender;

 			foreach (Control c in panelMaterials.Controls) {
				MaterialView mv = (MaterialView)c;
				if (mv != sender)
					mv.Selected = false;
			}

			OnTextureSelected(selmv.TextureFile);
		}

		private void comboSizes_SelectedIndexChanged(object sender, EventArgs e)
		{
			panelMaterials.SuspendLayout();
			foreach (Control c in panelMaterials.Controls) {
				MaterialView mv = (MaterialView)c;
				mv.SetSize(SelectedDisplaySize);
			}
			panelMaterials.ResumeLayout();
		}
	}
}
