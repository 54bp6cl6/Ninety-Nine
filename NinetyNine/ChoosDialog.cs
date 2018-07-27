using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinetyNine
{
	public partial class ChoosDialog : Form
	{
		public ChoosDialog()
		{
			InitializeComponent();
		}

		bool[] pass_list;
		int ans;

		public void SetData(bool pass1,bool pass2,bool pass3)
		{
			pass_list = new bool[] { pass1, pass2, pass3 };
		}

		public int GetAns()
		{
			return ans;
		}

		private void ChoosDialog_Load(object sender, EventArgs e)
		{
			if (pass_list[0])
			{
				button1.Enabled = false;
				button1.BackColor = Color.Red;
			}
			if (pass_list[1])
			{
				button2.Enabled = false;
				button2.BackColor = Color.Red;
			}
			if (pass_list[2])
			{
				button3.Enabled = false;
				button3.BackColor = Color.Red;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			ans = 1;
			Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			ans = 2;
			Close();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			ans = 3;
			Close();
		}
	}
}
