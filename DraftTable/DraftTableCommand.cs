using System;
using Rhino;
using Rhino.Commands;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DraftTable
{
    [System.Runtime.InteropServices.Guid("f8637aa3-46b1-4e75-9cf3-02dc3e06be35"),
     Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)
    ]
    public class DraftTableCommand : Command
    {
        public DraftTableCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static DraftTableCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "DraftTable"; }
        }
        
        DraftTableForm draftTableForm = null;

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        private const uint SW_RESTORE = 0x09;

        public static void Restore(Form form)
        {
            if (form.WindowState == FormWindowState.Minimized)
            {
                ShowWindow(form.Handle, SW_RESTORE);
            }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (draftTableForm != null && !draftTableForm.IsDisposed)
            {
                if (!draftTableForm.Visible)
                    draftTableForm.Show();

                Restore(draftTableForm);
                
                return Result.Success;
            }

            draftTableForm = new DraftTableForm();
            
            draftTableForm.Show();

            draftTableForm.LoadFolder("");
            draftTableForm.LoadDraftTableFoldersThumbnails();
            
            return Result.Success;
        }
    }
}
