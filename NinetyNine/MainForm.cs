using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CardClass;
using System.Threading;

namespace NinetyNine
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		List<Card> all;              //待抽的卡組
		Player plr;
		Player[] all_player;         //所有玩家的陣列
		Button[] CardButton;         //玩家手上5張牌對應的5個按鈕
		Label[] name;                //顯示三個電腦名字的Label陣列，當電腦出局時要把它變成紅色
		Label[] say;                 //電腦出牌時出現的Label陣列
		int index = 0;               //現在是誰的局
		int way = 1;                 //輪轉方向，1代表順時針，-1代表逆時針

		//將撲克牌輸出成固定的格式
		public static string PrintCard(Card c)
		{
			string s;
			if (c.suit == Card.Suits.spade) s = "♠";
			else if (c.suit == Card.Suits.heart) s = "♥";
			else if (c.suit == Card.Suits.diamond) s = "♦";
			else s = "♣";
			return s + "\n" + c.rank;
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			Form.CheckForIllegalCrossThreadCalls = false;    //此行允許跨執行敘控制元件
			all = Dealer.GetDeckOfCard();
			Dealer.Shuffle(all);
			plr = new Player("你");
			all_player = new Player[] { plr, new Player("魯迅"), new Player("愛迪生"), new Player("貝多芬") };
			Dealer.DrawCard(all, plr.hand, 5);
			Dealer.DrawCard(all, all_player[1].hand, 5);
			Dealer.DrawCard(all, all_player[2].hand, 5);
			Dealer.DrawCard(all, all_player[3].hand, 5);
			CardButton = new Button[] { card0_BT, card1_BT, card2_BT, card3_BT, card4_BT };
			name = new Label[] { cpt1_name_LB, cpt1_name_LB, cpt2_name_LB, cpt3_name_LB };
			say = new Label[] { cpt1_card_LB, cpt1_card_LB, cpt2_card_LB, cpt3_card_LB };
			foreach (Label LB in say)
			{
				LB.Visible = false;
			}
			ButtonUpdate();
		}

		//根據不同按鈕被按下，帶入不同的引數
		#region 按鈕觸發
		private void card0_BT_Click(object sender, EventArgs e)
		{
			ButtonOnclick(0);
		}
		private void card1_BT_Click(object sender, EventArgs e)
		{
			ButtonOnclick(1);
		}
		private void card2_BT_Click(object sender, EventArgs e)
		{
			ButtonOnclick(2);
		}
		private void card3_BT_Click(object sender, EventArgs e)
		{
			ButtonOnclick(3);
		}
		private void card4_BT_Click(object sender, EventArgs e)
		{
			ButtonOnclick(4);
		}
		#endregion

		private void ButtonOnclick(int i)
		{
			PlrPlayCard(plr.hand[i]);
			GetCard(plr.hand, i);
			ButtonUpdate();
			MoveOn();
			ButtonLuck(true);
		}

		//控制玩家能否出牌
		private void ButtonLuck(bool luck)
		{
			if (luck)
			{
				foreach (Button BT in CardButton)
				{
					BT.Enabled = false;
				}
			}
			else
			{
				foreach (Button BT in CardButton)
				{
					BT.Enabled = true;
					ButtonUpdate();
				}
			}
			if (luck) plr_LB.Visible = false;
			else plr_LB.Visible = true;
		}

		//重新整理玩家手牌的顯示
		private void ButtonUpdate()
		{
			for (int i = 0; i < 5; i++)
			{
				CardButton[i].Text = PrintCard(plr.hand[i]);
			}
			//將不能出的牌改成紅色
			for (int i = 0; i < 5; i++)
			{
				if ((!(plr.hand[i].rank == "A" && plr.hand[i].suit == Card.Suits.spade)) && plr.hand[i].rank != "4" && plr.hand[i].rank != "5" && plr.hand[i].rank != "10" && plr.hand[i].rank != "J" && plr.hand[i].rank != "Q" && plr.hand[i].rank != "K")
				{
					if (int.Parse(point_LB.Text) + plr.hand[i].ToInt() > 99)
					{
						CardButton[i].Enabled = false;
						CardButton[i].BackColor = Color.Red;
					}
					else CardButton[i].BackColor = SystemColors.Control;
				}
				else CardButton[i].BackColor = SystemColors.Control;
			}
		}

		private void ChangWay()
		{
			if (way > 0)
			{
				way = -1;
				way_LB.Text = "方向：逆時針";
			}
			else
			{
				way = 1;
				way_LB.Text = "方向：順時針";
			}
		}

		//處理玩家打出一張牌後的效果
		private void PlrPlayCard(Card c)
		{
			if (c.rank == "A" && c.suit == Card.Suits.spade)
			{
				record_TB.Text += "你打出了" + PrintCard(c) + "，把牌局歸零!!\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
				point_LB.Text = "0";
			}
			else if (c.rank == "4")
			{
				record_TB.Text += "你打出了" + PrintCard(c) + "，迴轉了順序\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
				ChangWay();
			}
			else if (c.rank == "5")
			{
				int alive = 0;
				int surviver = -1;
				for (int i = 1; i < 4; i++)
				{
					if (!all_player[i].pass)
					{
						alive++;
						surviver = i;
					}
				}
				if (alive > 1)
				{
					ChoosDialog CD = new ChoosDialog();
					CD.SetData(all_player[1].pass, all_player[2].pass, all_player[3].pass);
					CD.ShowDialog();
					record_TB.Text += "你打出了" + PrintCard(c) + "，指定" + all_player[CD.GetAns()].name + "出牌\r\n";
					record_TB.SelectionStart = record_TB.Text.Length;
					record_TB.ScrollToCaret();
					index = CD.GetAns() - way;
				}
				else
				{
					record_TB.Text += "你打出了" + PrintCard(c) + "，指定" + all_player[surviver].name + "出牌\r\n";
					record_TB.SelectionStart = record_TB.Text.Length;
					record_TB.ScrollToCaret();
					index = surviver - way;
				}
			}
			else if (c.rank == "10")
			{
				if (int.Parse(point_LB.Text) < 10)
				{
					record_TB.Text += "你打出了" + PrintCard(c) + "，+10\r\n";
					record_TB.SelectionStart = record_TB.Text.Length;
					record_TB.ScrollToCaret();
					point_LB.Text = (int.Parse(point_LB.Text) + 10).ToString();
				}
				else if (int.Parse(point_LB.Text) > 89)
				{
					record_TB.Text += "你打出了" + PrintCard(c) + "，-10\r\n";
					record_TB.SelectionStart = record_TB.Text.Length;
					record_TB.ScrollToCaret();
					point_LB.Text = (int.Parse(point_LB.Text) - 10).ToString();
				}
				else
				{
					PlusDialog PD = new PlusDialog();
					PD.SetData(10, 99 - int.Parse(point_LB.Text));
					PD.ShowDialog();
					if (PD.GetAns())
					{
						record_TB.Text += "你打出了" + PrintCard(c) + "，+10\r\n";
						record_TB.SelectionStart = record_TB.Text.Length;
						record_TB.ScrollToCaret();
						point_LB.Text = (int.Parse(point_LB.Text) + 10).ToString();
					}
					else
					{
						record_TB.Text += "你打出了" + PrintCard(c) + "，-10\r\n";
						record_TB.SelectionStart = record_TB.Text.Length;
						record_TB.ScrollToCaret();
						point_LB.Text = (int.Parse(point_LB.Text) - 10).ToString();
					}
				}
			}
			else if (c.rank == "J")
			{
				record_TB.Text += "你打出了" + PrintCard(c) + "，這局跳過\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
			}
			else if (c.rank == "Q")
			{
				if (int.Parse(point_LB.Text) < 20)
				{
					record_TB.Text += "你打出了" + PrintCard(c) + "，+20\r\n";
					record_TB.SelectionStart = record_TB.Text.Length;
					record_TB.ScrollToCaret();
					point_LB.Text = (int.Parse(point_LB.Text) + 20).ToString();
				}
				else if (int.Parse(point_LB.Text) > 79)
				{
					record_TB.Text += "你打出了" + PrintCard(c) + "，-20\r\n";
					record_TB.SelectionStart = record_TB.Text.Length;
					record_TB.ScrollToCaret();
					point_LB.Text = (int.Parse(point_LB.Text) - 20).ToString();
				}
				else
				{
					PlusDialog PD = new PlusDialog();
					PD.SetData(20, 99 - int.Parse(point_LB.Text));
					PD.ShowDialog();
					if (PD.GetAns())
					{
						record_TB.Text += "你打出了" + PrintCard(c) + "，+20\r\n";
						record_TB.SelectionStart = record_TB.Text.Length;
						record_TB.ScrollToCaret();
						point_LB.Text = (int.Parse(point_LB.Text) + 20).ToString();
					}
					else
					{
						record_TB.Text += "你打出了" + PrintCard(c) + "，-20\r\n";
						record_TB.SelectionStart = record_TB.Text.Length;
						record_TB.ScrollToCaret();
						point_LB.Text = (int.Parse(point_LB.Text) - 20).ToString();
					}
				}
			}
			else if (c.rank == "K")
			{
				record_TB.Text += "你打出了" + PrintCard(c) + "，99!\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
				point_LB.Text = "99";
			}
			else
			{
				record_TB.Text += "你打出了" + PrintCard(c) + "\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
				point_LB.Text = (int.Parse(point_LB.Text) + c.ToInt()).ToString();
			}
		}

		//打出一張牌後抽一張
		private void GetCard(List<Card> hand, int i)
		{
			//把打出的牌丟回牌堆底
			all.Add(hand[i]);
			hand.RemoveAt(i);
			//抽一張
			Dealer.DrawCard(all, hand, 1);
			Dealer.Shuffle(all);
		}

		//電腦決定要打哪一張牌
		private void CptPlay()
		{
			/*將手上的牌分成普通與特殊兩種
			 * 如果可以出普通牌，就出情況許可下最大的牌
			 * 若不能出普通牌，則隨機出一張特殊牌
			 * 若沒有特殊牌，就輸了*/
			List<Card> nc = new List<Card>();
			List<Card> sc = new List<Card>();
			foreach (Card c in all_player[index].hand)
			{
				if (c.rank == "A" && c.suit == Card.Suits.spade) sc.Add(c);
				else if (c.rank == "4" || c.rank == "5" || c.rank == "10" || c.rank == "J" || c.rank == "Q" || c.rank == "K") sc.Add(c);
				else nc.Add(c);
			}
			int range = 99 - int.Parse(point_LB.Text);
			Card ans = new Card(Card.Suits.club, "0");
			bool choosen = false;
			if (nc.Count > 0)
			{
				foreach (Card c in nc)
				{
					if (c.ToInt() <= range && c.ToInt() > ans.ToInt())
					{
						ans = c;
						choosen = true;
					}
				}
			}
			if (!choosen && sc.Count > 0)
			{
				if (sc.Count > 1) Dealer.Shuffle(sc);
				ans = sc[0];
				choosen = true;
			}

			if (!choosen)
			{
				name[index].BackColor = Color.Red;
				record_TB.Text += all_player[index].name + "爆了!!";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
				say[index].Text = "爆了";
				all_player[index].pass = true;
				say[index].Visible = true;
			}
			else
			{
				int index_ans = -1;
				for (int i = 0; i < 5; i++)
				{
					if (all_player[index].hand[i].suit == ans.suit && all_player[index].hand[i].rank == ans.rank)
					{
						index_ans = i;
						break;
					}
				}
				say[index].Text = PrintCard(ans);
				say[index].Visible = true;
				int old_index = index;
				CptPlayCard(ans);
				GetCard(all_player[old_index].hand, index_ans);
			}
		}

		//電腦打出一張牌後的效果
		private void CptPlayCard(Card c)
		{
			if (c.rank == "A" && c.suit == Card.Suits.spade)
			{
				record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，把牌局歸零!!\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
				point_LB.Text = "0";
			}
			else if (c.rank == "4")
			{
				record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，迴轉了順序\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
				ChangWay();
			}
			else if (c.rank == "5")
			{
				List<int> surviver = new List<int>();
				for (int i = 0; i < 4; i++)
				{
					if (!all_player[i].pass) surviver.Add(i);
				}
				Random ran = new Random();
				int num = index;
				while (num == index)
				{
					num = surviver[ran.Next(surviver.Count())];
				}
				record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，指定" + all_player[num].name + "出牌\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				index = num - way;
				record_TB.ScrollToCaret();
			}
			else if (c.rank == "10")
			{
				if (int.Parse(point_LB.Text) < 10)
				{
					record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，+10\r\n";
					record_TB.SelectionStart = record_TB.Text.Length;
					record_TB.ScrollToCaret();
					point_LB.Text = (int.Parse(point_LB.Text) + 10).ToString();
				}
				else if (int.Parse(point_LB.Text) > 89)
				{
					record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，-10\r\n";
					record_TB.SelectionStart = record_TB.Text.Length;
					record_TB.ScrollToCaret();
					point_LB.Text = (int.Parse(point_LB.Text) - 10).ToString();
				}
				else
				{
					Random ran = new Random();
					if (ran.Next(2) == 0)
					{
						record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，-10\r\n";
						record_TB.SelectionStart = record_TB.Text.Length;
						record_TB.ScrollToCaret();
						point_LB.Text = (int.Parse(point_LB.Text) - 10).ToString();
					}
					else
					{
						record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，+10\r\n";
						record_TB.SelectionStart = record_TB.Text.Length;
						record_TB.ScrollToCaret();
						point_LB.Text = (int.Parse(point_LB.Text) + 10).ToString();
					}
				}
			}
			else if (c.rank == "J")
			{
				record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，這局跳過\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
			}
			else if (c.rank == "Q")
			{
				if (int.Parse(point_LB.Text) < 20)
				{
					record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，+20\r\n";
					record_TB.SelectionStart = record_TB.Text.Length;
					record_TB.ScrollToCaret();
					point_LB.Text = (int.Parse(point_LB.Text) + 20).ToString();
				}
				else if (int.Parse(point_LB.Text) > 79)
				{
					record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，-20\r\n";
					record_TB.SelectionStart = record_TB.Text.Length;
					record_TB.ScrollToCaret();
					point_LB.Text = (int.Parse(point_LB.Text) - 20).ToString();
				}
				else
				{
					Random ran = new Random();
					if (ran.Next(2) == 0)
					{
						record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，-20\r\n";
						record_TB.SelectionStart = record_TB.Text.Length;
						record_TB.ScrollToCaret();
						point_LB.Text = (int.Parse(point_LB.Text) - 20).ToString();
					}
					else
					{
						record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，+20\r\n";
						record_TB.SelectionStart = record_TB.Text.Length;
						record_TB.ScrollToCaret();
						point_LB.Text = (int.Parse(point_LB.Text) + 20).ToString();
					}
				}
			}
			else if (c.rank == "K")
			{
				record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "，99!\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
				point_LB.Text = "99";
			}
			else
			{
				record_TB.Text += all_player[index].name + "打出了" + PrintCard(c) + "\r\n";
				record_TB.SelectionStart = record_TB.Text.Length;
				record_TB.ScrollToCaret();
				point_LB.Text = (int.Parse(point_LB.Text) + c.ToInt()).ToString();
			}
		}

		private void MoveOn()
		{
			index += way;
			if (index == -1) index = 3;
			if (index == 4) index = 0;
		}

		private void WinOrLose()
		{
			bool win = true;
			foreach (Label LB in name)
			{
				if (LB.BackColor != Color.Red)
				{
					win = false;
					break;
				}
			}
			if (win)
			{
				timer1.Enabled = false;
				MessageBox.Show("你贏了");
				Close();
			}

			bool lose = true;
			foreach (Button BT in CardButton)
			{
				if (BT.BackColor != Color.Red)
				{
					lose = false;
					break;
				}
			}
			if (lose)
			{
				timer1.Enabled = false;
				MessageBox.Show("你輸了");
				Close();
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (timer1.Interval != 1500) timer1.Interval = 1500;
			if (index != 0)
			{
				if (!all_player[index].pass)
				{
					foreach (Label LB in say)
					{
						LB.Visible = false;
					}
					CptPlay();
					MoveOn();
				}
				else
				{
					while (all_player[index].pass)
					{
						MoveOn();
					}
					if (index != 0)
					{
						foreach (Label LB in say)
						{
							LB.Visible = false;
						}
						CptPlay();
						MoveOn();
					}
					else timer1.Interval = 100;
				}
				ButtonUpdate();
			}
			else
			{
				ButtonUpdate();
				ButtonLuck(false);
				WinOrLose();
			}
		}
	}
}
