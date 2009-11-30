namespace upspring
{
	partial class MaterialView
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
			this.textureBox = new System.Windows.Forms.PictureBox();
			this.labelName = new System.Windows.Forms.Label();
			this.labelSize = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.textureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// textureBox
			// 
			this.textureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textureBox.Location = new System.Drawing.Point(3, 24);
			this.textureBox.Name = "textureBox";
			this.textureBox.Size = new System.Drawing.Size(225, 163);
			this.textureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.textureBox.TabIndex = 0;
			this.textureBox.TabStop = false;
			this.textureBox.Click += new System.EventHandler(this.textureBox_Click);
			// 
			// labelName
			// 
			this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.labelName.BackColor = System.Drawing.Color.Gainsboro;
			this.labelName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelName.Location = new System.Drawing.Point(3, 1);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(146, 20);
			this.labelName.TabIndex = 1;
			this.labelName.Click += new System.EventHandler(this.labelName_Click);
			// 
			// labelSize
			// 
			this.labelSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSize.BackColor = System.Drawing.Color.Gainsboro;
			this.labelSize.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelSize.Location = new System.Drawing.Point(155, 1);
			this.labelSize.Name = "labelSize";
			this.labelSize.Size = new System.Drawing.Size(73, 20);
			this.labelSize.TabIndex = 2;
			this.labelSize.Text = "1024 x 1024";
			this.labelSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelSize.Click += new System.EventHandler(this.labelSize_Click);
			// 
			// MaterialView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelSize);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.textureBox);
			this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Name = "MaterialView";
			this.Size = new System.Drawing.Size(231, 190);
			((System.ComponentModel.ISupportInitialize)(this.textureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox textureBox;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label labelSize;
	}
}
