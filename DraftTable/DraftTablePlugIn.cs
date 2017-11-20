using Rhino.PlugIns;
using System.Net;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Rhino.UI;

namespace DraftTable
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class DraftTablePlugIn : Rhino.PlugIns.PlugIn
    {
        public IdleWatcher idleWatcher;

        public class IdleWatcher : Rhino.UI.MouseCallback
        {
            public System.DateTime lastMove = System.DateTime.Now;

            protected override void OnMouseDown(Rhino.UI.MouseCallbackEventArgs e)
            {
                lastMove = System.DateTime.Now;
                base.OnMouseDown(e);
            }

            protected override void OnMouseMove(MouseCallbackEventArgs e)
            {
                lastMove = System.DateTime.Now;
                base.OnMouseMove(e);
            }

            public bool IsIdle(double checkTimeMill)
            {
                var totalTime = DateTime.Now - lastMove;
                return totalTime.TotalMilliseconds > checkTimeMill;
            }
        }

        public DraftTablePlugIn()
        {
            Instance = this;

            idleWatcher = new IdleWatcher();
            idleWatcher.Enabled = true;
        }

        ///<summary>Gets the only instance of the DraftTablePlugIn plug-in.</summary>
        public static DraftTablePlugIn Instance
        {
            get; private set;
        }


        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {

            return base.OnLoad(ref errorMessage);
        }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and mantain plug-in wide options in a document.
    }
}