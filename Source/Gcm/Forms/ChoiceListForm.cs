using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utilities;

namespace Launcher.Forms
{
    public partial class ChoiceListForm : JoinGame
    {
        public string SelectedGame
        {
            get
            {
                return GameList.SelectedItem.ToString();
            }
        }

        public ChoiceListForm(string caption, string okText)
        {
            InitializeComponent();
            m_OkButton.Text = okText;
            this.Text = caption;
        }

        public bool SetGameList(PairList<int,string> list)
        {
            if (list.Count() == 0)
                return false;

            GameList.Items.Clear();

            foreach (var game in list.Select(p => p.Value + " " + p.Key))
                GameList.Items.Add(game);

            GameList.SelectedIndex = 0;
            
            return true;
        }
    }
}
