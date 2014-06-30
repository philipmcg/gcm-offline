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
    public partial class LoginBox : Form
    {
        public void SetTitle(string title)
        {
            this.Text = title;
        }

        public string Username { get { return m_tbUsername.Text; } set { m_tbUsername.Text = value; } }
        public TextBox UsernameBox { get { return m_tbUsername; } set { m_tbUsername = value; } }
        public string Password { get { return m_tbPassword.Text; } set { m_tbPassword.Text = value; } }
        public TextBox PasswordBox { get { return m_tbPassword; } set { m_tbPassword = value; } }


        Action<LoginBox> ShowFunction;

        private IVariableBin var;
        private string usernameKey;
        private string passwordKey;

        private void Reset(string title, Action<LoginBox> showFunction, string cancelText, string okText)
        {
            SetTitle(title);
            ShowFunction = showFunction;
            m_cancelButton.Text = cancelText;
            m_acceptButton.Text = okText;
            UsernameBox.PasswordChar = default(char);
            PasswordBox.PasswordChar = '\u25CF';
            if (var.Str.ContainsKey(usernameKey) && GcmLauncher.Var.Str.ContainsKey(passwordKey))
            {
                UsernameBox.Text = GcmLauncher.Var.Str[usernameKey];
                PasswordBox.Text = GcmLauncher.Var.Str[passwordKey];
            }
            this.label3.Text = GcmLauncher.Var.Str["auth_error", ""];
            this.label3.Visible = true;
        }

        public LoginBox(IVariableBin variableBin, string usernameKey, string passwordKey)
        {
            this.var = variableBin;
            this.usernameKey = usernameKey;
            this.passwordKey = passwordKey;

            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            m_acceptButton.Focus();

            if(ShowFunction != null)
                ShowFunction(this);
        }
        public DialogResult ShowDialog(string title, Action<LoginBox> showFunction)
        {
            Reset(title, showFunction, "Cancel","OK");
            var result = ShowDialog();
            return result;
        }
        public DialogResult ShowDialog(string title, Action<LoginBox> showFunction, string cancelText, string okText)
        {
            Reset(title, showFunction, cancelText, okText);
            var result = ShowDialog();
            return result;
        }
    }
}
