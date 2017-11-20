using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;


namespace DraftTable
{
    public partial class DraftTableForm : Form
    {
        public string activeFolder = "00";
        public TimeSpan activeFolderTime = new TimeSpan(0);

        public string GetActiveFolderFullPath()
        {
            return FileUtils.GetActiveFolderFullPath(activeFolder);
        }

        public bool BackupMode { get { return checkBoxBackup.Checked; } set { checkBoxBackup.Checked = value; } }
        public bool DiffMode { get { return checkBoxDiff.Checked; } set { checkBoxDiff.Checked = value; } }


        static public DateTime lastTimerUpdate = DateTime.Now;
        static public Timer workTimer;

        public DraftTableForm()
        {
            InitializeComponent();

            this.Icon = global::DraftTable.Properties.Resources.favicon;

            this.FormClosing += DraftTable_FormClosing;

            workTimer = new Timer();
            workTimer.Interval = 1500;
            workTimer.Tick += WorkTimer_Tick;
            workTimer.Enabled = true;

            LoadSettings();
        }

        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            var Now = DateTime.Now;

            var currentTime = Now - lastTimerUpdate;

            if (!DraftTablePlugIn.Instance.idleWatcher.IsIdle(30000.0))
            {
                activeFolderTime += currentTime;
                this.Text = "DraftTable " + activeFolderTime.ToString("hh\\:mm\\:ss");
            }
            else
            {
                var idleTime = Now - DraftTablePlugIn.Instance.idleWatcher.lastMove;
                this.Text = "DraftTable " + activeFolderTime.ToString("hh\\:mm\\:ss") + " (Idle:" + idleTime.ToString("hh\\:mm\\:ss") + ")";
            }

            lastTimerUpdate = Now;
        }

        public bool LoadSettings()
        {
            var settings = DraftTablePlugIn.Instance.CommandSettings("DraftTable");

            if (settings == null)
                return false;

            var point = settings.GetPoint("Location", Location);
            var size = settings.GetSize("Size", Size);

            Location = point;
            Size = size;

            var backup = settings.GetBool("Backup", this.checkBoxBackup.Checked);
            var diff = settings.GetBool("Diff", this.checkBoxDiff.Checked);

            checkBoxBackup.Checked = backup;
            checkBoxDiff.Checked = diff;

            return true;
        }

        public bool SaveSettings()
        {
            SaveFolderSettings();

            var settings = DraftTablePlugIn.Instance.CommandSettings("DraftTable");

            if (settings == null)
                return false;

            settings.SetPoint("Location", Location);
            settings.SetSize("Size", Size);
            settings.SetBool("Backup", this.checkBoxBackup.Checked);
            settings.SetBool("Diff", this.checkBoxDiff.Checked);

            return true;
        }

        private void DraftTable_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        public void SaveFolderSettings()
        {
            if (!String.IsNullOrEmpty(activeFolder))
            {
                var folder = FileUtils.GetActiveFolderFullPath(activeFolder);
                File.WriteAllText(Path.Combine(folder, "time.dttime"), activeFolderTime.ToString());
            }
        }

        public bool LoadFolder(string folder = "")
        {
            this.ThumbViewer.folderMode = false;
            this.ThumbViewer.fileMode = true;
            this.ThumbViewer.parentForm = this;
            this.ThumbViewer.ClearImages();

            if (String.IsNullOrWhiteSpace(folder))
            {
                folder = FileUtils.GetActiveFolderFullPath(activeFolder);
            }

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (Directory.Exists(folder))
            {
                var files = Directory.GetFiles(folder);

                foreach (var file in files)
                {
                    if ("dtFolderThumb" == Path.GetFileNameWithoutExtension(file))
                        continue;

                    this.ThumbViewer.AddFileLoad(file);
                }
            }

            activeFolderTime = new TimeSpan(0);

            try
            {
                var timeFile = Path.Combine(folder, "time.dttime");
                if (File.Exists(timeFile))
                {
                    var txt = File.ReadAllText(timeFile);
                    TimeSpan.TryParse(txt, out activeFolderTime);
                }
            }
            catch (Exception e)
            {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

            return true;
        }

        public bool LoadDraftTableFoldersThumbnails()
        {
            FolderThumbnail.SuspendLayout();

            this.FolderThumbnail.parentForm = this;
            this.FolderThumbnail.fileMode = false;
            this.FolderThumbnail.folderMode = true;

            string folder = FileUtils.GetDraftTableFolder();

            var directories = Directory.GetDirectories(folder);

            FolderThumbnail.ClearImages();

            foreach (var d in directories)
                FolderThumbnail.AddFolder(d);

            FolderThumbnail.UpdateFolderBackgroundColor();

            FolderThumbnail.ResumeLayout();

            return true;
        }


        private void toolStripSaveFile_Click(object sender, EventArgs e)
        {
            string file = "file";

            string path = FileUtils.GetActiveFolderFullPath(activeFolder);

            System.IO.Directory.CreateDirectory(path);

            string filename = FileUtils.GetNextFileName(Path.Combine(path, file + ".3dm"));

            FileUtils.SaveFile(filename);

            this.ThumbViewer.AddFile(filename, false, "");

        }

        private void toolStripSaveOutGroups_Click(object sender, EventArgs e)
        {

        }

        private void toolStripExportFile_Click(object sender, EventArgs e)
        {
            string file = "part";

            string path = FileUtils.GetActiveFolderFullPath(activeFolder);

            System.IO.Directory.CreateDirectory(path);

            string filename = FileUtils.GetNextFileName(Path.Combine(path, file + ".3dm"));

            FileUtils.SaveFile(filename, true);

            this.ThumbViewer.AddFile(filename, true, "");
        }

        private void toolStripNewFolder(object sender, EventArgs e)
        {
            string folder = FileUtils.GetDraftTableFolder();

            var directories = Directory.GetDirectories(folder);


            for (int i = 0; i < 100; i++)
            {
                string newFolderName = Path.Combine(folder, string.Format("{0:00}", i));

                if (!directories.Contains(newFolderName))
                {
                    System.IO.Directory.CreateDirectory(newFolderName);
                    this.LoadDraftTableFoldersThumbnails();
                    return;
                }
            }

        }

        private void toolStripButtonZip_Click(object sender, EventArgs e)
        {
            string folder = GetActiveFolderFullPath();
            string zip = folder + ".zip";

            if (!ModifierKeys.HasFlag(Keys.Control))
            {
                Rhino.UI.SaveFileDialog fd = new Rhino.UI.SaveFileDialog();
                fd.Title = "Zip File to export to";
                fd.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
                fd.DefaultExt = "zip";
                if (!fd.ShowSaveDialog())
                    return;

                zip = fd.FileName;
            }

            ZipFile.CreateFromDirectory(folder, zip);
        }

        private void checkBoxBackup_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void toolStripFromClipboard_Click(object sender, EventArgs e)
        {

            var data = Clipboard.GetDataObject();

            MemoryStream rhino5Data = data.GetData("Rhino 5.0 3DM Clip global mem") as MemoryStream;

            if (rhino5Data != null)
            {
                try
                {
                    System.IO.MemoryStream ms = rhino5Data;

                    string file = "file";

                    string path = FileUtils.GetActiveFolderFullPath(activeFolder);

                    System.IO.Directory.CreateDirectory(path);

                    string filename = FileUtils.GetNextFileName(Path.Combine(path, file + ".3dm"));

                    using (FileStream saveFile = new FileStream(filename, FileMode.Create, System.IO.FileAccess.Write))
                    {
                        byte[] bytes = new byte[ms.Length];
                        ms.Read(bytes, 0, (int)ms.Length);
                        saveFile.Write(bytes, 0, bytes.Length);
                        ms.Close();
                    }

                    this.ThumbViewer.AddFile(filename, false, "");

                }
                catch (Exception ex)
                {
                    Rhino.RhinoApp.WriteLine(ex.ToString());
                }

                return;
            }

            if (System.Windows.Forms.Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();


                int pythonCount = 0;
                int vbCount = 0;

                int totalCount = 0;

                using (System.IO.StringReader reader = new System.IO.StringReader(text))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        totalCount++;

                        if (totalCount < 1000)
                        {

                            if (FileUtils.IsPythonImport(line))
                                pythonCount++;

                            if (FileUtils.IsVBImport(line))
                                vbCount++;
                        }

                    }
                }

                string file = "text.txt";

                if (pythonCount > vbCount)
                    file = "script.py";

                if (vbCount > pythonCount)
                    file = "script.rvb";

                string path = FileUtils.GetActiveFolderFullPath(activeFolder);

                System.IO.Directory.CreateDirectory(path);

                string filename = FileUtils.GetNextFileName(Path.Combine(path, file));

                File.WriteAllText(filename, text);

                this.ThumbViewer.AddFile(filename, false, "");

                return;
            }

            if (Clipboard.ContainsImage())
            {
                var image = Clipboard.GetImage();


                string file = "picture";

                string path = FileUtils.GetActiveFolderFullPath(activeFolder);

                System.IO.Directory.CreateDirectory(path);

                string filename = FileUtils.GetNextFileName(Path.Combine(path, file + ".png"));

                image.Save(filename);

                this.ThumbViewer.AddFile(filename, false, "");

                return;
            }


            if (Clipboard.ContainsFileDropList())
            {
                var droplist = Clipboard.GetFileDropList();

                this.ThumbViewer.AddFilesToDraftTable(Clipboard.GetFileDropList().Cast<string>());
            }


            /*
            foreach (var f in data.GetFormats())
            {
                Rhino.RhinoApp.WriteLine(f);
                var d = data.GetData(f);

                if (d != null)
                    Rhino.RhinoApp.WriteLine(d.ToString());

            }
            */

        }

        private void DraftTableForm_Load(object sender, EventArgs e)
        {

        }

        private void toolStripRenderAll_Click(object sender, EventArgs e)
        {
            // Todo save out scene to temp file
            string folder = FileUtils.GetActiveFolderFullPath(activeFolder);

            string backupPath = Path.Combine(folder, "Backup");

            System.IO.Directory.CreateDirectory(backupPath);

            string tempFileName = FileUtils.GetNextFileName(Path.Combine(folder, "temp" + ".3dm"));

            FileUtils.SaveFile(tempFileName);

            var files = Directory.GetFiles(folder);

            foreach (var file in files)
            {
                if (Path.GetExtension(file).ToUpper().CompareTo(".3DM") != 0)
                    continue;

                if (Path.GetFileNameWithoutExtension(file).StartsWith("part"))
                    continue;

                Guid currentRenderGuid = Rhino.Render.Utilities.DefaultRenderPlugInId;

                var renderPlugin = Rhino.PlugIns.PlugIn.Find(currentRenderGuid);

                if (renderPlugin != null)
                {
                    if (renderPlugin.Name.Contains("V-Ray"))
                    {
                        Rhino.RhinoApp.RunScript(
                            @"!-_RunScript ( 
Option Explicit 
Dim VRay
Set VRay = Rhino.GetPluginObject(""{E8CFE179-B60C-411A-8416-62A893334519}"")
VRay.SetBatchRenderOn True
)", true);
                    }

                }

                // Render current file
                FileUtils.OpenFile(file);

                Rhino.RhinoApp.RunScript("!_-Render", true);

                string renderFileName = file + ".render.png";

                FileUtils.WriteBackupFile(renderFileName, true);

                Rhino.RhinoApp.RunScript("_-SaveRenderWindowAs \"" + renderFileName + "\"", true);
                Rhino.RhinoApp.RunScript("_-CloseRenderWindow", true);
            }

            FileUtils.OpenFile(tempFileName);

            try
            {
                File.Delete(tempFileName);
            }
            catch (Exception) { }
            

            LoadFolder();
        }
    }
}
