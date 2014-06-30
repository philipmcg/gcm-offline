using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace Utilities.Windows
{
    public static class WindowsExtensions
    {
        /// <summary>
        /// Invokes the action if necessary, otherwise calls it directly.
        /// </summary>
        public static void InvokeIfRequired(this Form me, Action action)
        {
            if (me.InvokeRequired)
            {
                me.Invoke(action);
            }
            else
            {
                action();
            }
        }


        /// <summary>
        /// Sets the window size and position to that specified in the variable bin.
        /// </summary>
        public static void ApplySavedFormLayout(this Form me, IVariableBin var, string prefix)
        {
            if (WindowParameterIsValid(var, prefix + "width"))
                me.Width = var.Int[prefix + "width"];

            if (WindowParameterIsValid(var, prefix + "height"))
                me.Height = var.Int[prefix + "height"];

            if (WindowParameterIsValid(var, prefix + "left"))
                me.Left = var.Int[prefix + "left"];

            if (WindowParameterIsValid(var, prefix + "top"))
                me.Top = var.Int[prefix + "top"];
        }

        static bool WindowParameterIsValid(IVariableBin var, string key)
        {
            return var.Int.ContainsKey(key) && var.Int[key] >= 0;
        }

        /// <summary>
        /// Saves the window size and position to values in the variable bin.
        /// </summary>
        public static void SaveFormLayout(this Form me, IVariableBin var, string prefix)
        {
            var.Int[prefix + "width"] = me.Width;
            var.Int[prefix + "height"] = me.Height;
            var.Int[prefix + "left"] = me.Left;
            var.Int[prefix + "top"] = me.Top;
        }
    }
}
