using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Threading;

namespace Utilities.Windows
{

    public class ProgramStarter
    {
        public bool WindowsFormsApplication { get; set; }
        public bool MessageOnUnauthorizedAccessException { get; set; }

        public ProgramStarter()
        {
            WindowsFormsApplication = false;
            MessageOnUnauthorizedAccessException = true;
        }

        public void Prepare(bool isProtected)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(isProtected)
                SetupThreadException();
        }

        public void StartWindowDelegate(Action action)
        {
            WindowsFormsApplication = true;

#if DEBUG
            UnProtectedRunWindowDelegate(action);
#else
            ProtectedRunWindowDelegate(action);
#endif
        }

        public void StartWindow<TForm>(Action beforeWindow = null, Action afterWindow = null) where TForm : Form, new()
        {
            WindowsFormsApplication = true;


#if DEBUG
            UnProtectedRunWindow<TForm>(beforeWindow, afterWindow);
#else
            ProtectedRunWindow<TForm>(beforeWindow, afterWindow);
#endif
        }

        public void StartConsole(Action callback)
        {
            WindowsFormsApplication = false;
#if DEBUG
            UnProtectedRunConsole(callback);
#else
            ProtectedRunConsole(callback);
#endif
        }

        void SetupThreadException()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
#if DEBUG
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
#else
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
#endif

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void ProtectedRunWindow<T>(Action beforeWindow = null, Action afterWindow = null) where T : Form, new()
        {
            try
            {
                if (beforeWindow != null)
                    beforeWindow();

                T form = new T();
                if (afterWindow != null)
                    form.Load += new EventHandler((o, e) => afterWindow());
                OnStarted();
                Application.Run(form);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        void UnProtectedRunWindow<T>(Action beforeWindow = null, Action afterWindow = null) where T : Form, new()
        {
            if (beforeWindow != null)
                beforeWindow();

            T form = new T();
            if (afterWindow != null)
                form.Load += new EventHandler((o, e) => afterWindow());
            OnStarted();
            Application.Run(form);
        }

        void ProtectedRunWindowDelegate(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        void UnProtectedRunWindowDelegate(Action action)
        {
            action();
        }

        void ProtectedRunConsole(Action callback)
        {
            try
            {
                callback();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        void UnProtectedRunConsole(Action callback)
        {
            callback();
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        protected virtual void HandleException(Exception e)
        {
            if (MessageOnUnauthorizedAccessException)
            {
                if (e is UnauthorizedAccessException)
                {
                    DisplayAdminRightsMessage();
                    return;
                }
            }

            SendErrorReport(e);
        }

        protected virtual void OnStarted() {

        }

        public static void SendErrorReport(Exception e)
        {
          System.IO.File.WriteAllText("gcm.crash.log", e.ToString());
        }

        protected void DisplayAdminRightsMessage()
        {
            string message = "This program requires administrator rights to run.";
            if (this.WindowsFormsApplication)
                MessageBox.Show(message);
            else
                Console.WriteLine(message);

            Application.Exit();
        }
    }
}
