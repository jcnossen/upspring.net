namespace upspring
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.toolCubeBrush = new System.Windows.Forms.ToolStripButton();
			this.toolConeBrush = new System.Windows.Forms.ToolStripButton();
			this.toolCilinderBrush = new System.Windows.Forms.ToolStripButton();
			this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuNew = new System.Windows.Forms.MenuItem();
			this.menuOpen = new System.Windows.Forms.MenuItem();
			this.menuSave = new System.Windows.Forms.MenuItem();
			this.menuSaveAs = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.toolCamera = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabModel = new System.Windows.Forms.TabPage();
			this.treeView = new System.Windows.Forms.TreeView();
			this.tabObjTree = new System.Windows.Forms.TabPage();
			this.tabTextures = new System.Windows.Forms.TabPage();
			this.materialBrowser = new upspring.MaterialBrowser();
			this.glView1 = new UpspringSharp.GLView();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.toolStripContainer.ContentPanel.SuspendLayout();
			this.toolStripContainer.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabModel.SuspendLayout();
			this.tabTextures.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolCubeBrush
			// 
			this.toolCubeBrush.Name = "toolCubeBrush";
			this.toolCubeBrush.Size = new System.Drawing.Size(23, 23);
			// 
			// toolConeBrush
			// 
			this.toolConeBrush.Name = "toolConeBrush";
			this.toolConeBrush.Size = new System.Drawing.Size(23, 23);
			// 
			// toolCilinderBrush
			// 
			this.toolCilinderBrush.Name = "toolCilinderBrush";
			this.toolCilinderBrush.Size = new System.Drawing.Size(23, 23);
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuNew,
            this.menuOpen,
            this.menuSave,
            this.menuSaveAs});
			this.menuItem1.Text = "File";
			// 
			// menuNew
			// 
			this.menuNew.Index = 0;
			this.menuNew.Text = "New";
			// 
			// menuOpen
			// 
			this.menuOpen.Index = 1;
			this.menuOpen.Text = "Open";
			// 
			// menuSave
			// 
			this.menuSave.Index = 2;
			this.menuSave.Text = "Save";
			// 
			// menuSaveAs
			// 
			this.menuSaveAs.Index = 3;
			this.menuSaveAs.Text = "Save As";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem3});
			this.menuItem2.Text = "Settings";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
			this.menuItem3.Text = "Material Directories";
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.toolStripContainer);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.tabControl);
			this.splitContainer.Size = new System.Drawing.Size(814, 301);
			this.splitContainer.SplitterDistance = 587;
			this.splitContainer.TabIndex = 1;
			// 
			// toolStripContainer
			// 
			// 
			// toolStripContainer.ContentPanel
			// 
			this.toolStripContainer.ContentPanel.Controls.Add(this.glView1);
			this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(587, 262);
			this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer.Name = "toolStripContainer";
			this.toolStripContainer.Size = new System.Drawing.Size(587, 301);
			this.toolStripContainer.TabIndex = 2;
			this.toolStripContainer.Text = "toolStripContainer1";
			// 
			// toolStripContainer.TopToolStripPanel
			// 
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip);
			// 
			// toolStrip
			// 
			this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip.GripMargin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolCamera,
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripButton6,
            this.toolStripButton7,
            this.toolStripButton8});
			this.toolStrip.Location = new System.Drawing.Point(3, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
			this.toolStrip.Size = new System.Drawing.Size(335, 39);
			this.toolStrip.TabIndex = 0;
			// 
			// toolCamera
			// 
			this.toolCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolCamera.Image = global::UpspringSharp.Properties.Resources.camera;
			this.toolCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolCamera.Name = "toolCamera";
			this.toolCamera.Size = new System.Drawing.Size(36, 36);
			this.toolCamera.Text = "toolStripButton1";
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = global::UpspringSharp.Properties.Resources.move;
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(36, 36);
			this.toolStripButton1.Text = "toolStripButton1";
			// 
			// toolStripButton2
			// 
			this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton2.Image = global::UpspringSharp.Properties.Resources.rotate;
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new System.Drawing.Size(36, 36);
			this.toolStripButton2.Text = "toolStripButton2";
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton3.Image = global::UpspringSharp.Properties.Resources.scale;
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(36, 36);
			this.toolStripButton3.Text = "toolStripButton3";
			// 
			// toolStripButton4
			// 
			this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton4.Image = global::UpspringSharp.Properties.Resources.originmove;
			this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton4.Name = "toolStripButton4";
			this.toolStripButton4.Size = new System.Drawing.Size(36, 36);
			this.toolStripButton4.Text = "toolStripButton4";
			// 
			// toolStripButton5
			// 
			this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton5.Image = global::UpspringSharp.Properties.Resources.color;
			this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton5.Name = "toolStripButton5";
			this.toolStripButton5.Size = new System.Drawing.Size(36, 36);
			this.toolStripButton5.Text = "toolStripButton5";
			// 
			// toolStripButton6
			// 
			this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton6.Image = global::UpspringSharp.Properties.Resources.polyflip1;
			this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton6.Name = "toolStripButton6";
			this.toolStripButton6.Size = new System.Drawing.Size(36, 36);
			this.toolStripButton6.Text = "toolStripButton6";
			// 
			// toolStripButton7
			// 
			this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton7.Image = global::UpspringSharp.Properties.Resources.texture;
			this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton7.Name = "toolStripButton7";
			this.toolStripButton7.Size = new System.Drawing.Size(36, 36);
			this.toolStripButton7.Text = "toolStripButton7";
			// 
			// toolStripButton8
			// 
			this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton8.Image = global::UpspringSharp.Properties.Resources.rotatetex;
			this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton8.Name = "toolStripButton8";
			this.toolStripButton8.Size = new System.Drawing.Size(36, 36);
			this.toolStripButton8.Text = "toolStripButton8";
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabModel);
			this.tabControl.Controls.Add(this.tabObjTree);
			this.tabControl.Controls.Add(this.tabTextures);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(223, 301);
			this.tabControl.TabIndex = 2;
			// 
			// tabModel
			// 
			this.tabModel.Controls.Add(this.treeView);
			this.tabModel.Location = new System.Drawing.Point(4, 22);
			this.tabModel.Name = "tabModel";
			this.tabModel.Padding = new System.Windows.Forms.Padding(3);
			this.tabModel.Size = new System.Drawing.Size(215, 275);
			this.tabModel.TabIndex = 0;
			this.tabModel.Text = "Model";
			this.tabModel.UseVisualStyleBackColor = true;
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.Location = new System.Drawing.Point(3, 3);
			this.treeView.Name = "treeView";
			this.treeView.PathSeparator = "/";
			this.treeView.Size = new System.Drawing.Size(209, 269);
			this.treeView.TabIndex = 0;
			// 
			// tabObjTree
			// 
			this.tabObjTree.Location = new System.Drawing.Point(4, 22);
			this.tabObjTree.Name = "tabObjTree";
			this.tabObjTree.Padding = new System.Windows.Forms.Padding(3);
			this.tabObjTree.Size = new System.Drawing.Size(215, 275);
			this.tabObjTree.TabIndex = 2;
			this.tabObjTree.Text = "Tree";
			this.tabObjTree.UseVisualStyleBackColor = true;
			// 
			// tabTextures
			// 
			this.tabTextures.Controls.Add(this.materialBrowser);
			this.tabTextures.Location = new System.Drawing.Point(4, 22);
			this.tabTextures.Name = "tabTextures";
			this.tabTextures.Padding = new System.Windows.Forms.Padding(3);
			this.tabTextures.Size = new System.Drawing.Size(215, 275);
			this.tabTextures.TabIndex = 1;
			this.tabTextures.Text = "Materials";
			this.tabTextures.UseVisualStyleBackColor = true;
			// 
			// materialBrowser
			// 
			this.materialBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.materialBrowser.Location = new System.Drawing.Point(3, 3);
			this.materialBrowser.Name = "materialBrowser";
			this.materialBrowser.Size = new System.Drawing.Size(209, 269);
			this.materialBrowser.TabIndex = 0;
			// 
			// glView1
			// 
			this.glView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.glView1.Location = new System.Drawing.Point(0, 0);
			this.glView1.Name = "glView1";
			this.glView1.Size = new System.Drawing.Size(587, 262);
			this.glView1.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(814, 301);
			this.Controls.Add(this.splitContainer);
			this.KeyPreview = true;
			this.Menu = this.mainMenu;
			this.Name = "MainForm";
			this.Text = "UpspringSharp";
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.toolStripContainer.ContentPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.PerformLayout();
			this.toolStripContainer.ResumeLayout(false);
			this.toolStripContainer.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.tabModel.ResumeLayout(false);
			this.tabTextures.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.ToolStripContainer toolStripContainer;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton toolCubeBrush;
		private System.Windows.Forms.ToolStripButton toolConeBrush;
		private System.Windows.Forms.ToolStripButton toolCilinderBrush;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuSaveAs;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuNew;
		private System.Windows.Forms.MenuItem menuOpen;
		private System.Windows.Forms.MenuItem menuSave;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabModel;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.TabPage tabObjTree;
		private System.Windows.Forms.TabPage tabTextures;
		private MaterialBrowser materialBrowser;
		private System.Windows.Forms.ToolStripButton toolCamera;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripButton toolStripButton4;
		private System.Windows.Forms.ToolStripButton toolStripButton5;
		private System.Windows.Forms.ToolStripButton toolStripButton6;
		private System.Windows.Forms.ToolStripButton toolStripButton7;
		private System.Windows.Forms.ToolStripButton toolStripButton8;
		private UpspringSharp.GLView glView1;
	}
}

