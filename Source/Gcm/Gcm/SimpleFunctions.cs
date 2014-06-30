using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GcmShared;
using System.IO;

using System.Windows.Forms;

using Utilities;
using Utilities.Windows;

namespace Launcher.GCM
{
    public class SimpleFunctions
    {

        /// <summary>
        /// Launches url in the client's internet browser
        /// </summary>
        public bool LaunchFullURL(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
                return true;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Launcher.Forms.TextForm textForm = new Forms.TextForm();
                textForm.Text = "Unable to locate browser";
                textForm.Label.Text = "GCM was unable to find your internet browser.\n\nYou can copy and paste this URL manually:";
                textForm.TextBox.Text = url;
                textForm.Width = (int)(textForm.Width * 1.15);
                textForm.Height = (int)(textForm.Height * 1.2);
                textForm.ActionOnShown = f => {
                    f.TextBox.Focus();
                    f.TextBox.SelectAll();
                    f.TextBox.Copy();
                };
                textForm.ShowDialog();
                return false;
            }
        }

        public void SaveForm(Form form, string name)
        {
            form.SaveFormLayout(Gcm.Var, "opt_window_" + name + "_");
        }

        public void LoadForm(Form form, string name)
        {
            form.ApplySavedFormLayout(Gcm.Var, "opt_window_" + name + "_");
        }

        public void SaveCharacterName(int factionID, Name name, string state = null)
        {
            if (state != null)
                Gcm.Var.Str["opt_fac_{0}_state".With(factionID)] = state;

            Gcm.Var.Str["opt_fac_{0}_first_name".With(factionID)] = name.First;
            Gcm.Var.Str["opt_fac_{0}_middle_name".With(factionID)] = name.Middle;
            Gcm.Var.Str["opt_fac_{0}_last_name".With(factionID)] = name.Last;
            GcmLauncher.Data.SaveVariables();
        }

        public Name LoadCharacterName(int factionID)
        {
            string f = "";
            string m = "";
            string l = "";

            if (Gcm.Var.Str.ContainsKey("opt_fac_{0}_first_name".With(factionID)))
                f = Gcm.Var.Str["opt_fac_{0}_first_name".With(factionID)];
            if (Gcm.Var.Str.ContainsKey("opt_fac_{0}_middle_name".With(factionID)))
                m = Gcm.Var.Str["opt_fac_{0}_middle_name".With(factionID)];
            if (Gcm.Var.Str.ContainsKey("opt_fac_{0}_last_name".With(factionID)))
                l = Gcm.Var.Str["opt_fac_{0}_last_name".With(factionID)];

            return new Name(f,m,l);
        }

        public string LoadState(int factionID)
        {
            if (Gcm.Var.Str.ContainsKey("opt_fac_{0}_state".With(factionID)))
                return Gcm.Var.Str["opt_fac_{0}_state".With(factionID)];
            else
                return null;
        }

        public void Restart()
        {
            GcmLauncher.Data.SaveVariables();
            Utilities.Windows.Processes.RestartThisApplication();
        }
    }
}
