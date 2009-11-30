namespace upspring
{
	partial class MaterialBrowser
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.comboDirectory = new System.Windows.Forms.ComboBox();
			this.panelMaterials = new System.Windows.Forms.FlowLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.comboSizes = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// comboDirectory
			// 
			this.comboDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.comboDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDirectory.FormattingEnabled = true;
			this.comboDirectory.Location = new System.Drawing.Point(3, 3);
			this.comboDirectory.Name = "comboDirectory";
			this.comboDirectory.Size = new System.Drawing.Size(243, 21);
			this.comboDirectory.TabIndex = 0;
			this.comboDirectory.SelectedIndexChanged += new System.EventHandler(this.comboDirectory_SelectedIndexChanged);
			// 
			// panelMaterials
			// 
			this.panelMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.panelMaterials.AutoScroll = true;
			this.panelMaterials.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelMaterials.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panelMaterials.Location = new System.Drawing.Point(3, 52);
			this.panelMaterials.Name = "panelMaterials";
			this.panelMaterials.Size = new System.Drawing.Size(243, 167);
			this.panelMaterials.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Display size:";
			// 
			// comboSizes
			// 
			this.comboSizes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.comboSizes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSizes.FormattingEnabled = true;
			this.comboSizes.Items.AddRange(new object[] {
            "32 x 32",
            "64 x 64",
            "128 x 128",
            "256 x 256",
            "512 x 512"});
			this.comboSizes.Location = new System.Drawing.Point(74, 25);
			this.comboSizes.Name = "comboSizes";
			this.comboSizes.Size = new System.Drawing.Size(172, 21);
			this.comboSizes.TabIndex = 3;
			this.comboSizes.SelectedIndexChanged += new System.EventHandler(this.comboSizes_SelectedIndexChanged);
			// 
			// MaterialBrowser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.comboSizes);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panelMaterials);
			this.Controls.Add(this.comboDirectory);
			this.Name = "MaterialBrowser";
			this.Size = new System.Drawing.Size(249, 222);
			this.Load += new System.EventHandler(this.MaterialBrowser_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboDirectory;
		private System.Windows.Forms.FlowLayoutPanel panelMaterials;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboSizes;
	}
}
