using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Launcher.Forms
{
    public partial class GameListForm : JoinGame
    {
        public string SelectedGame
        {
            get
            {
                return GameList.SelectedItem.ToString();
            }
        }

        public GameListForm(string caption, string okText)
        {
            InitializeComponent();
            m_OkButton.Text = okText;
            this.Text = caption;
        }

        public bool SetGameList(IEnumerable<string> list)
        {
            if (list.Count() == 0)
                return false;

            GameList.Items.Clear();

            foreach (var game in list)
                GameList.Items.Add(game);

            GameList.SelectedIndex = 0;

            return true;
        }
    }
}
