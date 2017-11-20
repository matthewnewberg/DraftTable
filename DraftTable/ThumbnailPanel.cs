using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace DraftTable
{
    public partial class ThumbnailPanel : FlowLayoutPanel
    {
        public bool folderMode = false;
        public bool fileMode = true;
        public DraftTableForm parentForm;

        public ThumbnailPanel()
        {
            InitializeComponent();

            this.AutoScroll = true;
            this.AllowDrop = true;
            this.DoubleBuffered = true;

            // List of Image
            ImageList = new List<Image>();
            FileList = new List<string>();

            // Set DragDrop Event
            this.DragDrop += new DragEventHandler(ThumbnailViewerControl_DragDrop);
            this.DragEnter += new DragEventHandler(ThumbnailViewerControl_DragEnter);

            folderMode = false;
            fileMode = true;
            parentForm = null;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        /// <summary> Image Extensions accepted by this control
        /// </summary>


        /// <summary> Added Image will be stored in this List
        /// </summary>
        public List<Image> ImageList { get; set; }
        public List<string> FileList { get; set; }

        public void AddFilesToDraftTable(IEnumerable<string> files)
        {
            this.Cursor = Cursors.WaitCursor;

            string folder = "00";

            if (parentForm != null)
                folder = parentForm.activeFolder;

            string destPath = FileUtils.GetActiveFolderFullPath(folder);

            System.IO.Directory.CreateDirectory(destPath);

            foreach (var f in files)
            {
                string srcfilename = System.IO.Path.GetFileName(f);
                string srcPath = System.IO.Path.GetDirectoryName(f);

                string destFilename = FileUtils.GetNextFileName(Path.Combine(destPath, srcfilename));

                if (srcPath != destPath)
                {
                    System.IO.File.Copy(f, destFilename);
                    AddFile(destFilename, true, "");
                }
                else
                {
                    AddFile(f, true, "");
                }

            }

            this.Cursor = Cursors.Default;
        }

        public void AddFolder(string folder)
        {
            string thumbnail = Path.Combine(folder, "dtFolderThumb.png");

            var split = folder.Split(Path.DirectorySeparatorChar);

            string name = split.Last();

            if (File.Exists(thumbnail))
            {
                using (var image = Image.FromFile(thumbnail))
                {
                    MakeFolder(image, folder);
                }
            }
            else
            {
                Bitmap flag = new Bitmap(128, 128);
                using (Graphics flagGraphics = Graphics.FromImage(flag))
                {
                    flagGraphics.FillRectangle(Brushes.Gray, 0, 0, 128, 128);
                    flagGraphics.DrawImage(global::DraftTable.Properties.Resources.folder_bitmap, 0, 128 - global::DraftTable.Properties.Resources.folder_bitmap.Height/2);

                    Font drawFont = new Font("Arial", 16);
                    SolidBrush drawBrush = new SolidBrush(Color.OrangeRed);

                    var stringFormat = StringFormat.GenericDefault;
                    stringFormat.Alignment = StringAlignment.Center;

                    Rectangle drawRect = new Rectangle(0, 0, 128, 128);

                    flagGraphics.DrawString(name, drawFont, drawBrush, drawRect, stringFormat);
                }

                MakeFolder(flag, folder);
            }

        }

        public void AddFileLoad(string f)
        {
            string filename = Path.GetFileName(f);

            if (filename.StartsWith("file"))
                AddFile(f, false, "");
            else
                AddFile(f, true, "");

        }

        public void AddFile(string f, bool import, string backup)
        {

            string ext = Path.GetExtension(f).ToUpperInvariant();

            if (FileUtils.ImageExtensions.Contains(ext))
            {
                using (var image = Image.FromFile(f))
                {
                    // Add binary data to List
                    ImageList.Add(image);
                    FileList.Add(f);

                    // Create a Thumnail of Image and add Thumbnail to Panel
                    MakeThumbnail(image, f, true, backup);
                }

                GC.GetTotalMemory(true);
            }

            if (ext.CompareTo(".GH") == 0)
            {
                using (Bitmap bmp = new Bitmap(128, 128))
                {
                    FileUtils.DrawStringToBmp(f, bmp);

                    MakeThumbnail(bmp, f, true, backup);
                }
            }

            if (ext.CompareTo(".PY") == 0 || ext.CompareTo(".RVB") == 0 || ext.CompareTo(".TXT") == 0)
            {
                using (Bitmap bmp = new Bitmap(128, 128))
                {
                    FileUtils.DrawStringToBmp(f, bmp);

                    MakeThumbnail(bmp, f, true, backup);
                }
            }

            if (FileUtils.CadExtensions.Contains(ext))
            {
                Bitmap bmp = Rhino.FileIO.File3dm.ReadPreviewImage(f);

                if (bmp == null)
                {
                    bmp = new Bitmap(128, 128);
                    FileUtils.DrawStringToBmp(f, bmp);
                }
            
                string folder = Path.GetDirectoryName(f);

                string folderThumbnail = Path.Combine(folder, "dtFolderThumb.png");

                if (!import)
                {
                    if (!File.Exists(folderThumbnail))
                    {
                        bmp.Save(folderThumbnail);
                        parentForm.LoadDraftTableFoldersThumbnails();
                    }
                }

                ImageList.Add(bmp);
                FileList.Add(f);

                MakeThumbnail(bmp, f, import, backup);

                bmp.Dispose();

                GC.GetTotalMemory(true);
            }
        }



        public void ClearImages()
        {
            this.Controls.Clear();
        }

        public void MakeThumbnail(Image image, string filepath, bool import, string backupFile)
        {
            if (image == null)
                return;


            var thumbnailImage = image.GetThumbnailImage(128 - 2, 128 - 2, null, new IntPtr());

            var thumbnail = new Thumbnail(thumbnailImage, filepath, import, backupFile, parentForm);

            thumbnail.MaximumSize = new Size(128, 128);
            thumbnail.MinimumSize = new Size(128, 128);
            thumbnail.Size = new Size(128, 128);
            thumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //thumbnail.SizeMode = PictureBoxSizeMode.Zoom;

            // Create a border when Mouse entered
            thumbnail.MouseEnter += new EventHandler(thumb_MouseEnter);

            // Clear border when Mouse leaved
            thumbnail.MouseLeave += new EventHandler(thumb_MouseLeave);

            // Add to Panel
            this.Controls.Add(thumbnail);
        }

        private void folderThumb_paint(object sender, PaintEventArgs e)
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

        public void MakeFolder(Image image, string filepath)
        {

            // Create a Picture Box for showing thumbnail image
            PictureBox thumb = new PictureBox();
            thumb.MaximumSize = new Size(128, 128);
            thumb.MinimumSize = new Size(128, 128);
            thumb.Size = new Size(128, 128);
            thumb.BorderStyle = System.Windows.Forms.BorderStyle.None;
            thumb.SizeMode = PictureBoxSizeMode.Zoom;
            thumb.Paint += folderThumb_paint;
            thumb.BackColor = Color.Black;

            // Create a border when Mouse entered
            thumb.MouseEnter += new EventHandler(thumb_MouseEnter);

            // Clear border when Mouse leaved
            thumb.MouseLeave += new EventHandler(thumb_MouseLeave);

            // Preview image when Mouse Double Clicked
            thumb.DoubleClick += new EventHandler(thumb_DoubleClick);

            // Create a Picture Box for showing thumbnail image
            var thumbnailImage = image.GetThumbnailImage(thumb.Width - 2, thumb.Height - 2, null, new IntPtr());

            thumb.Image = thumbnailImage;
            thumb.Tag = filepath;


            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Delete", new EventHandler(menu_deleteClicked));
            thumb.ContextMenu = cm;

            // Add to Panel
            this.Controls.Add(thumb);
        }

        private void menu_deleteClicked(object sender, EventArgs e)
        {
            try
            {

                var result = Rhino.UI.Dialogs.ShowMessage("Do you want to delete all files (and backup)", "Delete Folder", Rhino.UI.ShowMessageButton.YesNo, Rhino.UI.ShowMessageIcon.Question); 

                if (result == Rhino.UI.ShowMessageResult.Yes)
                {
                    var p = sender as MenuItem;

                    var thumb = (p.Parent as ContextMenu).SourceControl as PictureBox;

                    string str = thumb.Tag as String;

                    Directory.Delete(str, true);

                    parentForm.LoadDraftTableFoldersThumbnails();
                }
            }
            catch (Exception exception)
            {
                Rhino.UI.Dialogs.ShowMessage(exception.Message, "Exception");
            }
        }

        public void UpdateFolderBackgroundColor()
        {
            if (parentForm == null)
                return;

            string currentFolder = parentForm.GetActiveFolderFullPath();

            foreach (var c in Controls)
            {
                var p = c as PictureBox;

                string str = p.Tag as String;

                if (str.CompareTo(currentFolder) == 0)
                    p.BackColor = Color.Green;
                else
                    p.BackColor = Color.Black;
            }
        }

        void thumb_DoubleClick(object sender, EventArgs e)
        {
            string filepath = "";

            if (folderMode)
            {

                try
                {
                    var box = sender as PictureBox;

                    filepath = box.Tag as string;

                    if (parentForm != null)
                    {
                        // changing form, save settings first
                        parentForm.SaveFolderSettings();
                        parentForm.activeFolder = filepath.Split(Path.DirectorySeparatorChar).Last();
                        UpdateFolderBackgroundColor();
                        parentForm.LoadFolder(filepath);                        
                    }
                }
                catch (Exception)
                {


                }
            }
        }

        void thumb_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).Invalidate();
        }

        void thumb_MouseEnter(object sender, EventArgs e)
        {
            if (folderMode)
            {


                var rc = ((PictureBox)sender).ClientRectangle;
                rc.Inflate(-2, -2);
                ControlPaint.DrawBorder(((PictureBox)sender).CreateGraphics()
                    , ((PictureBox)sender).ClientRectangle, Color.Red, ButtonBorderStyle.Solid);
                ControlPaint.DrawBorder3D(((PictureBox)sender).CreateGraphics()
                    , rc, Border3DStyle.Bump);
            }
        }

        void ThumbnailViewerControl_DragEnter(object sender, DragEventArgs e)
        {

            if (fileMode)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
        }

        void ThumbnailViewerControl_DragDrop(object sender, DragEventArgs e)
        {
            if (fileMode)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    AddFilesToDraftTable(files);
                }
            }
        }
    }
}
