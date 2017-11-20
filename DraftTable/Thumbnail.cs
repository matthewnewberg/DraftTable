using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace DraftTable
{
    public partial class Thumbnail : UserControl
    {
        public string filename;
        public DraftTableForm mainForm;
        public List<string> backupFiles = new List<string>();

        bool import = false;

        public Thumbnail(Image image, String filename, bool import, string backupFile, DraftTableForm mainForm)
        {
            InitializeComponent();


            bool cadMode = FileUtils.CadExtensions.Contains(Path.GetExtension(filename).ToUpperInvariant());
            Replace.Visible = cadMode;

            this.pictureBox1.Image = image;
            this.pictureBox1.Paint += thumbnail_paint;

            this.filename = filename;

            string fileExt = Path.GetExtension(filename).Substring(1);

            if (fileExt == null)
                fileExt = "";

            if (Path.GetFileNameWithoutExtension(filename).StartsWith("file") && fileExt == ".3dm")
            {
                pictureBox1.BackColor = Color.ForestGreen;
                labelKey.Text = "Key";
            }
            else if (Path.GetFileNameWithoutExtension(filename).StartsWith("part") && fileExt == ".3dm")
            {
                pictureBox1.BackColor = Color.Purple;
                labelKey.Text = "Part";
            }
            else if (fileExt.CompareTo(".gh") == 0)
            {
                pictureBox1.BackColor = Color.GreenYellow;
                labelKey.Text = "GH";
            }
            else if (fileExt.CompareTo(".py") == 0)
            {
                pictureBox1.BackColor = Color.Purple;
                labelKey.Text = "PY";
            }
            else if (fileExt.CompareTo(".rvb") == 0)
            {
                pictureBox1.BackColor = Color.RosyBrown;
                labelKey.Text = "RVB";
            }
            else if (fileExt.CompareTo(".txt") == 0)
            {
                pictureBox1.BackColor = Color.Tomato;
                labelKey.Text = "TXT";
            }
            else if (fileExt.CompareTo(".3dm") == 0)
            {
                pictureBox1.BackColor = Color.Black;
                labelKey.Text = "3dm";
            }
            else
            {
                pictureBox1.BackColor = Color.Black;
                labelKey.Text = fileExt;
            }

            this.import = import;

            if (!string.IsNullOrWhiteSpace(backupFile))
            {
                backupFiles.Add(backupFile);
            }
            else
            {

            }

            this.mainForm = mainForm;

            string ext = Path.GetExtension(filename).ToUpper();

            bool txtFile = (ext.CompareTo(".RVB") == 0 || ext.CompareTo(".PY") == 0 || ext.CompareTo(".TXT") == 0);

            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add(Path.GetFileNameWithoutExtension(filename));
            cm.MenuItems.Add("Delete", new EventHandler(menu_deleteClicked));
            cm.MenuItems.Add("Set Folder Icon", new EventHandler(menu_FolderIcon));
            cm.MenuItems.Add("Revert", new EventHandler(menu_revertClicked));
            cm.MenuItems.Add("Rename", new EventHandler(menu_renameClicked));

            if (txtFile)
            {
                cm.MenuItems.Add("Open In Notepad", new EventHandler(menu_openInNotepad));
                //cm.MenuItems.Add("Edit", new EventHandler(menu_Edit));
            }

            cm.MenuItems.Add("Backup", new EventHandler(menu_backupClicked));

            if (cadMode)
            {
                if (!import)
                    cm.MenuItems.Add("Import", new EventHandler(menu_importClicked));
                else
                    cm.MenuItems.Add("Open", new EventHandler(menu_openClicked));
            }

            pictureBox1.ContextMenu = cm;
        }

        private void thumbnail_paint(object sender, PaintEventArgs e)
        {
            var pictureBox = sender as PictureBox;
            ControlPaint.DrawBorder(e.Graphics, pictureBox.ClientRectangle, pictureBox.BackColor, ButtonBorderStyle.Solid);

            if (pictureBox.BackColor != Color.Black)
            {
                Rectangle smallRect = new Rectangle(pictureBox.ClientRectangle.Location, pictureBox.ClientRectangle.Size);

                smallRect.Inflate(-2, -2);

                ControlPaint.DrawBorder(e.Graphics, smallRect, pictureBox.BackColor, ButtonBorderStyle.Solid);
            }
        }

        private void menu_deleteClicked(object sender, EventArgs e)
        {
            try
            {
                File.Delete(filename);

                mainForm.LoadFolder();
            }
            catch (Exception exception)
            {
                Rhino.UI.Dialogs.ShowMessage(exception.Message, "Exception");
            }
        }


        private void menu_revertClicked(object sender, EventArgs e)
        {
            RevertForm form = new RevertForm(filename, mainForm);
            form.ShowDialog();
        }

        private void menu_openInNotepad(object sender, EventArgs e)
        {
            string ext = Path.GetExtension(filename).ToUpper();

            if (ext.CompareTo(".RVB") == 0)
            {
                System.Diagnostics.Process.Start("notepad.exe", filename);
            }

            if (ext.CompareTo(".PY") == 0)
            {
                System.Diagnostics.Process.Start("notepad.exe", filename);
            }

            if (ext.CompareTo(".TXT") == 0)
            {
                System.Diagnostics.Process.Start(filename);
            }
        }

        private void menu_Edit(object sender, EventArgs e)
        {
            string ext = Path.GetExtension(filename).ToUpper();

            if (ext.CompareTo(".RVB") == 0)
            {
                string script = @"-_EditScript """ + filename + @"""";

                Rhino.RhinoApp.RunScript(script, true);

            }

            if (ext.CompareTo(".PY") == 0)
            {
                string script = @"-_EditPythonScript """ + filename + @"""";
                Rhino.RhinoApp.RunScript(script, true);
            }

            if (ext.CompareTo(".TXT") == 0)
            {
                System.Diagnostics.Process.Start(filename);
            }
        }

        private void menu_renameClicked(object sender, EventArgs e)
        {
            string file = Path.GetFileName(filename);
            string newFile = "";
            Rhino.UI.Dialogs.ShowEditBox("Rename", "Rename file", file, false, out newFile);

            if (String.IsNullOrEmpty(newFile))
                return;


            string path = Path.GetDirectoryName(filename);

            string newFileName = Path.Combine(path, newFile);

            if (File.Exists(newFileName))
            {
                Rhino.UI.Dialogs.ShowTextDialog("File already exists", "Error");
                return;
            }

            if (Path.GetExtension(newFileName) != Path.GetExtension(filename))
            {
                var result = Rhino.UI.Dialogs.ShowMessage("File Extension is different! \n Would you like to rename?", "Warning", Rhino.UI.ShowMessageButton.YesNo, Rhino.UI.ShowMessageIcon.Question);

                if (result != Rhino.UI.ShowMessageResult.Yes)
                    return;
            }

            File.Copy(filename, newFileName);

            if (File.Exists(newFileName))
                File.Delete(filename);

            filename = newFileName;

            UpdateThumbnail();
        }

        private void menu_backupClicked(object sender, EventArgs e)
        {
            FileUtils.WriteBackupFile(filename, mainForm.DiffMode);
        }

        private void menu_importClicked(object sender, EventArgs e)
        {
            Rhino.FileIO.FileReadOptions readOptions = new Rhino.FileIO.FileReadOptions();

            readOptions.ImportMode = true;

            Rhino.RhinoDoc.ReadFile(filename, readOptions);
        }

        private void menu_openClicked(object sender, EventArgs e)
        {
            Rhino.FileIO.FileReadOptions readOptions = new Rhino.FileIO.FileReadOptions();

            readOptions.ImportMode = false;
            readOptions.OpenMode = true;
            Rhino.RhinoDoc.ReadFile(filename, readOptions);
        }

        private void menu_FolderIcon(object sender, EventArgs e)
        {

            string ext = Path.GetExtension(filename).ToUpperInvariant();

            if (FileUtils.CadExtensions.Contains(ext))
            {
                using (Bitmap bmp = Rhino.FileIO.File3dm.ReadPreviewImage(filename))
                {

                    string folder = Path.GetDirectoryName(filename);

                    string folderThumbnail = Path.Combine(folder, "dtFolderThumb.png");

                    bmp.Save(folderThumbnail);
                    mainForm.LoadDraftTableFoldersThumbnails();
                }
            }


            if (ext.CompareTo(".GH") == 0)
            {
                Bitmap flag = new Bitmap(128, 128);
                using (Graphics flagGraphics = Graphics.FromImage(flag))
                {
                    flagGraphics.FillRectangle(Brushes.Gray, 0, 0, 128, 128);
                    flagGraphics.DrawImage(global::DraftTable.Properties.Resources.file_bitmap, 0, 128 - global::DraftTable.Properties.Resources.file_bitmap.Height / 2);

                    Font drawFont = new Font("Arial", 16);
                    SolidBrush drawBrush = new SolidBrush(Color.OrangeRed);

                    var stringFormat = StringFormat.GenericDefault;
                    stringFormat.Alignment = StringAlignment.Center;

                    Rectangle drawRect = new Rectangle(0, 0, 128, 128);

                    flagGraphics.DrawString(Path.GetFileNameWithoutExtension(filename), drawFont, drawBrush, drawRect, stringFormat);
                }

                string folder = Path.GetDirectoryName(filename);

                string folderThumbnail = Path.Combine(folder, "dtFolderThumb.png");

                using (var thumbNailImage = flag.GetThumbnailImage(512, 512, null, new IntPtr()))
                {
                    thumbNailImage.Save(folderThumbnail);
                }
                mainForm.LoadDraftTableFoldersThumbnails();
            }


            if (FileUtils.ImageExtensions.Contains(ext))
            {
                using (Image image = Image.FromFile(filename))
                {
                    string folder = Path.GetDirectoryName(filename);

                    string folderThumbnail = Path.Combine(folder, "dtFolderThumb.png");

                    using (var thumbNailImage = image.GetThumbnailImage(512, 512, null, new IntPtr()))
                    {
                        thumbNailImage.Save(folderThumbnail);
                    }
                    mainForm.LoadDraftTableFoldersThumbnails();
                }

            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(filename))
                return;


            string ext = Path.GetExtension(filename).ToUpperInvariant();

            if (FileUtils.ImageExtensions.Contains(ext))
            {

                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    string script = "!_-PictureFrame \"" + filename + "\"";
                    Rhino.RhinoApp.RunScript(script, true);
                }
                else
                {
                    using (Image image = Image.FromFile(filename))
                    {
                        var clientRect = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ClientRectangle;

                        var lineUpperLeft = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ClientToWorld(new Point(clientRect.X, clientRect.Y));
                        var lineLowerRight = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ClientToWorld(new Point(clientRect.X + clientRect.Width, clientRect.Y + clientRect.Height));
                        var lineLowerLeft = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ClientToWorld(new Point(clientRect.X, clientRect.Y + clientRect.Height));

                        var plane = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ConstructionPlane();

                        double lineParam = 0;
                        Rhino.Geometry.Intersect.Intersection.LinePlane(lineLowerLeft, plane, out lineParam);
                        plane.Origin = lineLowerLeft.PointAt(lineParam);

                        double distanceScreen = lineUpperLeft.DistanceTo(lineLowerRight.PointAt(0), false);
                        double distanceImage = new Rhino.Geometry.Point2d(image.Width, image.Height).DistanceTo(Rhino.Geometry.Point2d.Origin);

                        double scale = distanceScreen / distanceImage;

                        Rhino.RhinoDoc.ActiveDoc.Objects.AddPictureFrame(plane, filename, false, image.Width * scale, image.Height * scale, false, false);

                        Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
                    }
                }
            }

            if (ext.CompareTo(".GH") == 0)
            {
                string script = @"-grasshopper editor load document open """ + filename + @"""";

                Rhino.RhinoApp.RunScript(script, true);

            }

            if (ext.CompareTo(".RVB") == 0)
            {
                string file = File.ReadAllText(filename);

                if (file.IndexOf("Rhino.AddAlias") > 0)
                {
                    string script = @"-_LoadScript """ + filename + @"""";

                    Rhino.RhinoApp.RunScript(script, true);

                    script = @"_RunScript";
                    Rhino.RhinoApp.RunScript(script, true);
                }
                else
                {
                    string script = @"-_Runscript (" + file + ")";
                    Rhino.RhinoApp.RunScript(script, true);
                }
            }

            if (ext.CompareTo(".PY") == 0)
            {
                string script = @"-_RunPythonScript """ + filename + @"""";
                Rhino.RhinoApp.RunScript(script, true);
            }

            if (ext.CompareTo(".TXT") == 0)
            {
                System.Diagnostics.Process.Start(filename);
            }

            if (FileUtils.CadExtensions.Contains(ext))
            {
                if (this.import)
                {
                    Rhino.FileIO.FileReadOptions readOptions = new Rhino.FileIO.FileReadOptions();

                    readOptions.ImportMode = true;

                    Rhino.RhinoDoc.ReadFile(filename, readOptions);
                }
                else
                {
                    Rhino.FileIO.FileReadOptions readOptions = new Rhino.FileIO.FileReadOptions();

                    readOptions.ImportMode = false;
                    readOptions.OpenMode = true;
                    Rhino.RhinoDoc.ReadFile(filename, readOptions);
                }

            }
        }

        public bool UpdateThumbnail()
        {

            string ext = Path.GetExtension(filename).ToUpperInvariant();
            if (FileUtils.ImageExtensions.Contains(filename))
            {
                using (var image = Image.FromFile(filename))
                {
                    var thumbnailImage = image.GetThumbnailImage(pictureBox1.Image.Width, pictureBox1.Image.Height, null, new IntPtr());
                    this.pictureBox1.Image = thumbnailImage;
                }

                GC.GetTotalMemory(true);
            }

            if (ext.CompareTo(".GH") == 0)
            {
                using (Bitmap bmp = new Bitmap(128, 128))
                {
                    FileUtils.DrawStringToBmp(filename, bmp);

                    var thumbnailImage = bmp.GetThumbnailImage(pictureBox1.Image.Width, pictureBox1.Image.Height, null, new IntPtr());

                    this.pictureBox1.Image = thumbnailImage;
                    this.pictureBox1.PerformLayout();
                }
            }

            if (ext.CompareTo(".PY") == 0 || ext.CompareTo(".RVB") == 0 || ext.CompareTo(".TXT") == 0)
            {
                using (Bitmap bmp = new Bitmap(128, 128))
                {
                    FileUtils.DrawStringToBmp(filename, bmp);

                    var thumbnailImage = bmp.GetThumbnailImage(pictureBox1.Image.Width, pictureBox1.Image.Height, null, new IntPtr());

                    this.pictureBox1.Image = thumbnailImage;
                    this.pictureBox1.PerformLayout();
                }
            }

            if (FileUtils.CadExtensions.Contains(ext))
            {

                using (Bitmap bmp = Rhino.FileIO.File3dm.ReadPreviewImage(filename))
                {
                    var thumbnailImage = bmp.GetThumbnailImage(pictureBox1.Image.Width, pictureBox1.Image.Height, null, new IntPtr());

                    this.pictureBox1.Image = thumbnailImage;
                    this.pictureBox1.PerformLayout();
                }

                GC.GetTotalMemory(true);
            }

            return true;
        }

        private void Replace_Click(object sender, EventArgs e)
        {
            if (mainForm.BackupMode)
            {
                backupFiles.Add(FileUtils.WriteBackupFile(filename, mainForm.DiffMode));
            }

            FileUtils.SaveFile(this.filename, this.import);

            UpdateThumbnail();
        }
    }
}
