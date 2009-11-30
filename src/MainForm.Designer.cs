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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
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
			this.toolMove = new System.Windows.Forms.ToolStripButton();
			this.toolScale = new System.Windows.Forms.ToolStripButton();
			this.toolRotate = new System.Windows.Forms.ToolStripButton();
			this.toolOriginMove = new System.Windows.Forms.ToolStripButton();
			this.toolColor = new System.Windows.Forms.ToolStripButton();
			this.toolPolyFlip = new System.Windows.Forms.ToolStripButton();
			this.toolRotateTex = new System.Windows.Forms.ToolStripButton();
			this.toolTexture = new System.Windows.Forms.ToolStripButton();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabModel = new System.Windows.Forms.TabPage();
			this.treeView = new System.Windows.Forms.TreeView();
			this.tabObjTree = new System.Windows.Forms.TabPage();
			this.tabTextures = new System.Windows.Forms.TabPage();
			this.materialBrowser = new upspring.MaterialBrowser();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
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
			this.splitContainer.Size = new System.Drawing.Size(814, 427);
			this.splitContainer.SplitterDistance = 587;
			this.splitContainer.TabIndex = 1;
			// 
			// toolStripContainer
			// 
			// 
			// toolStripContainer.ContentPanel
			// 
			this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(587, 388);
			this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer.Name = "toolStripContainer";
			this.toolStripContainer.Size = new System.Drawing.Size(587, 427);
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
            this.toolMove,
            this.toolScale,
            this.toolRotate,
            this.toolOriginMove,
            this.toolColor,
            this.toolPolyFlip,
            this.toolRotateTex,
            this.toolTexture});
			this.toolStrip.Location = new System.Drawing.Point(3, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
			this.toolStrip.Size = new System.Drawing.Size(335, 39);
			this.toolStrip.TabIndex = 0;
			// 
			// toolCamera
			// 
			this.toolCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolCamera.Image = ((System.Drawing.Image)(resources.GetObject("toolCamera.Image")));
			this.toolCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolCamera.Name = "toolCamera";
			this.toolCamera.Size = new System.Drawing.Size(36, 36);
			this.toolCamera.Text = "toolStripButton1";
			// 
			// toolMove
			// 
			this.toolMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolMove.Image = ((System.Drawing.Image)(resources.GetObject("toolMove.Image")));
			this.toolMove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolMove.Name = "toolMove";
			this.toolMove.Size = new System.Drawing.Size(36, 36);
			this.toolMove.Text = "toolStripButton3";
			// 
			// toolScale
			// 
			this.toolScale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolScale.Image = ((System.Drawing.Image)(resources.GetObject("toolScale.Image")));
			this.toolScale.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolScale.Name = "toolScale";
			this.toolScale.Size = new System.Drawing.Size(36, 36);
			this.toolScale.Text = "toolStripButton8";
			// 
			// toolRotate
			// 
			this.toolRotate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolRotate.Image = ((System.Drawing.Image)(resources.GetObject("toolRotate.Image")));
			this.toolRotate.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolRotate.Name = "toolRotate";
			this.toolRotate.Size = new System.Drawing.Size(36, 36);
			this.toolRotate.Text = "toolStripButton6";
			// 
			// toolOriginMove
			// 
			this.toolOriginMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolOriginMove.Image = ((System.Drawing.Image)(resources.GetObject("toolOriginMove.Image")));
			this.toolOriginMove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolOriginMove.Name = "toolOriginMove";
			this.toolOriginMove.Size = new System.Drawing.Size(36, 36);
			this.toolOriginMove.Text = "toolStripButton4";
			// 
			// toolColor
			// 
			this.toolColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolColor.Image = ((System.Drawing.Image)(resources.GetObject("toolColor.Image")));
			this.toolColor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolColor.Name = "toolColor";
			this.toolColor.Size = new System.Drawing.Size(36, 36);
			this.toolColor.Text = "toolStripButton2";
			// 
			// toolPolyFlip
			// 
			this.toolPolyFlip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolPolyFlip.Image = ((System.Drawing.Image)(resources.GetObject("toolPolyFlip.Image")));
			this.toolPolyFlip.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolPolyFlip.Name = "toolPolyFlip";
			this.toolPolyFlip.Size = new System.Drawing.Size(36, 36);
			this.toolPolyFlip.Text = "toolStripButton5";
			// 
			// toolRotateTex
			// 
			this.toolRotateTex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolRotateTex.Image = ((System.Drawing.Image)(resources.GetObject("toolRotateTex.Image")));
			this.toolRotateTex.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolRotateTex.Name = "toolRotateTex";
			this.toolRotateTex.Size = new System.Drawing.Size(36, 36);
			this.toolRotateTex.Text = "toolStripButton7";
			// 
			// toolTexture
			// 
			this.toolTexture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolTexture.Image = ((System.Drawing.Image)(resources.GetObject("toolTexture.Image")));
			this.toolTexture.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolTexture.Name = "toolTexture";
			this.toolTexture.Size = new System.Drawing.Size(36, 36);
			this.toolTexture.Text = "toolStripButton9";
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
			this.tabControl.Size = new System.Drawing.Size(223, 427);
			this.tabControl.TabIndex = 2;
			// 
			// tabModel
			// 
			this.tabModel.Controls.Add(this.treeView);
			this.tabModel.Location = new System.Drawing.Point(4, 22);
			this.tabModel.Name = "tabModel";
			this.tabModel.Padding = new System.Windows.Forms.Padding(3);
			this.tabModel.Size = new System.Drawing.Size(215, 401);
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
			this.treeView.Size = new System.Drawing.Size(209, 395);
			this.treeView.TabIndex = 0;
			// 
			// tabObjTree
			// 
			this.tabObjTree.Location = new System.Drawing.Point(4, 22);
			this.tabObjTree.Name = "tabObjTree";
			this.tabObjTree.Padding = new System.Windows.Forms.Padding(3);
			this.tabObjTree.Size = new System.Drawing.Size(215, 401);
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
			this.tabTextures.Size = new System.Drawing.Size(215, 401);
			this.tabTextures.TabIndex = 1;
			this.tabTextures.Text = "Materials";
			this.tabTextures.UseVisualStyleBackColor = true;
			// 
			// materialBrowser
			// 
			this.materialBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.materialBrowser.Location = new System.Drawing.Point(3, 3);
			this.materialBrowser.Name = "materialBrowser";
			this.materialBrowser.Size = new System.Drawing.Size(209, 395);
			this.materialBrowser.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(814, 427);
			this.Controls.Add(this.splitContainer);
			this.KeyPreview = true;
			this.Menu = this.mainMenu;
			this.Name = "MainForm";
			this.Text = "UpspringSharp";
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
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
		private System.Windows.Forms.ToolStripButton toolCamera;
		private System.Windows.Forms.ToolStripButton toolMove;
		private System.Windows.Forms.ToolStripButton toolScale;
		private System.Windows.Forms.ToolStripButton toolRotate;
		private System.Windows.Forms.ToolStripButton toolOriginMove;
		private System.Windows.Forms.ToolStripButton toolColor;
		private System.Windows.Forms.ToolStripButton toolPolyFlip;
		private System.Windows.Forms.ToolStripButton toolRotateTex;
		private System.Windows.Forms.ToolStripButton toolTexture;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabModel;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.TabPage tabObjTree;
		private System.Windows.Forms.TabPage tabTextures;
		private MaterialBrowser materialBrowser;
	}
}

