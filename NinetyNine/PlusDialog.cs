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
	public partial class PlusDialog : Form
	{
		public PlusDialog()
		{
			InitializeComponent();
		}

		int num;
		int range;
		bool ans = false;

		public void SetData(int num, int range)
		{
			this.num = num;
			this.range = range;
		}

		public bool GetAns()
		{
			return ans;
		}

		private void PlusDialog_Load(object sender, EventArgs e)
		{
			label1.Text = "你要+" + num + "還是-" + num + "?";
			if (range > 99 - num)
			{
				button2.Enabled = false;
				button2.BackColor = Color.Red;
				ans = true;
				Close();
			}
			else if (range < num)
			{
				button1.Enabled = false;
				button1.BackColor = Color.Red;
				ans = false;
				Close();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			ans = true;
			Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			ans = false;
			Close();
		}
	}
}
