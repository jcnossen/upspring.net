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
	public partial class ValueInput : Form
	{
		public string Value
		{
			set { txtValue.Text=value; }
			get { return txtValue.Text; }
		}

		public string InfoLabel
		{
			get { return label1.Text; }
			set { label1.Text=value; }
		}


		public ValueInput(string title, string labelText, string defaultValue)
		{
			InitializeComponent();
			Text = title;
			label1.Text = labelText;
			txtValue.Text = defaultValue;
		}
	}
}
