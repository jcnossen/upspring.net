using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace upspring
{
	public partial class PropertyEditorForm : Form
	{
		public PropertyEditorForm(string title, string info, object properties)
		{
			InitializeComponent();

			propertyGrid.SelectedObject = properties;

			labelInfo.Text = info;
			Text = title;
		}
	}
}
