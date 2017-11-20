namespace DraftTable
{
    partial class DraftTableForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DraftTableForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.FolderThumbnail = new DraftTable.ThumbnailPanel();
            this.checkBoxDiff = new System.Windows.Forms.CheckBox();
            this.checkBoxBackup = new System.Windows.Forms.CheckBox();
            this.ThumbViewer = new DraftTable.ThumbnailPanel();
            this.toolStripSave = new System.Windows.Forms.ToolStrip();
            this.toolStripSaveFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripExportFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripAddFolder = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZip = new System.Windows.Forms.ToolStripButton();
            this.toolStripFromClipboard = new System.Windows.Forms.ToolStripButton();
            this.toolStripRenderAll = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStripSave.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.FolderThumbnail);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.checkBoxDiff);
            this.splitContainer1.Panel2.Controls.Add(this.checkBoxBackup);
            this.splitContainer1.Panel2.Controls.Add(this.ThumbViewer);
            this.splitContainer1.Panel2.Controls.Add(this.toolStripSave);
            this.splitContainer1.Size = new System.Drawing.Size(759, 490);
            this.splitContainer1.SplitterDistance = 110;
            this.splitContainer1.TabIndex = 1;
            // 
            // FolderThumbnail
            // 
            this.FolderThumbnail.AllowDrop = true;
            this.FolderThumbnail.AutoScroll = true;
            this.FolderThumbnail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FolderThumbnail.FileList = ((System.Collections.Generic.List<string>)(resources.GetObject("FolderThumbnail.FileList")));
            this.FolderThumbnail.ImageList = ((System.Collections.Generic.List<System.Drawing.Image>)(resources.GetObject("FolderThumbnail.ImageList")));
            this.FolderThumbnail.Location = new System.Drawing.Point(0, 0);
            this.FolderThumbnail.Name = "FolderThumbnail";
            this.FolderThumbnail.Size = new System.Drawing.Size(110, 490);
            this.FolderThumbnail.TabIndex = 0;
            // 
            // checkBoxDiff
            // 
            this.checkBoxDiff.AutoSize = true;
            this.checkBoxDiff.Location = new System.Drawing.Point(557, 3);
            this.checkBoxDiff.Name = "checkBoxDiff";
            this.checkBoxDiff.Size = new System.Drawing.Size(79, 17);
            this.checkBoxDiff.TabIndex = 2;
            this.checkBoxDiff.Text = "DiffBackup";
            this.checkBoxDiff.UseVisualStyleBackColor = true;
            // 
            // checkBoxBackup
            // 
            this.checkBoxBackup.AutoSize = true;
            this.checkBoxBackup.Location = new System.Drawing.Point(488, 3);
            this.checkBoxBackup.Name = "checkBoxBackup";
            this.checkBoxBackup.Size = new System.Drawing.Size(63, 17);
            this.checkBoxBackup.TabIndex = 1;
            this.checkBoxBackup.Text = "Backup";
            this.checkBoxBackup.UseVisualStyleBackColor = true;
            this.checkBoxBackup.CheckedChanged += new System.EventHandler(this.checkBoxBackup_CheckedChanged);
            // 
            // ThumbViewer
            // 
            this.ThumbViewer.AllowDrop = true;
            this.ThumbViewer.AutoScroll = true;
            this.ThumbViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ThumbViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ThumbViewer.FileList = ((System.Collections.Generic.List<string>)(resources.GetObject("ThumbViewer.FileList")));
            this.ThumbViewer.ImageList = ((System.Collections.Generic.List<System.Drawing.Image>)(resources.GetObject("ThumbViewer.ImageList")));
            this.ThumbViewer.Location = new System.Drawing.Point(0, 25);
            this.ThumbViewer.Name = "ThumbViewer";
            this.ThumbViewer.Size = new System.Drawing.Size(645, 465);
            this.ThumbViewer.TabIndex = 0;
            // 
            // toolStripSave
            // 
            this.toolStripSave.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSaveFile,
            this.toolStripExportFile,
            this.toolStripAddFolder,
            this.toolStripButtonZip,
            this.toolStripFromClipboard,
            this.toolStripRenderAll});
            this.toolStripSave.Location = new System.Drawing.Point(0, 0);
            this.toolStripSave.Name = "toolStripSave";
            this.toolStripSave.Size = new System.Drawing.Size(645, 25);
            this.toolStripSave.TabIndex = 0;
            this.toolStripSave.Text = "Save";
            // 
            // toolStripSaveFile
            // 
            this.toolStripSaveFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSaveFile.Image = global::DraftTable.Properties.Resources.icon_save_small;
            this.toolStripSaveFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSaveFile.Name = "toolStripSaveFile";
            this.toolStripSaveFile.Size = new System.Drawing.Size(23, 22);
            this.toolStripSaveFile.Text = "Save Key File";
            this.toolStripSaveFile.Click += new System.EventHandler(this.toolStripSaveFile_Click);
            // 
            // toolStripExportFile
            // 
            this.toolStripExportFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripExportFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripExportFile.Image")));
            this.toolStripExportFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripExportFile.Name = "toolStripExportFile";
            this.toolStripExportFile.Size = new System.Drawing.Size(23, 22);
            this.toolStripExportFile.Text = "Export Part";
            this.toolStripExportFile.Click += new System.EventHandler(this.toolStripExportFile_Click);
            // 
            // toolStripAddFolder
            // 
            this.toolStripAddFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripAddFolder.Image = global::DraftTable.Properties.Resources.icon_add_folder_small;
            this.toolStripAddFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAddFolder.Name = "toolStripAddFolder";
            this.toolStripAddFolder.Size = new System.Drawing.Size(23, 22);
            this.toolStripAddFolder.Text = "toolStripButtonAdd";
            this.toolStripAddFolder.ToolTipText = "Create Folder";
            this.toolStripAddFolder.Click += new System.EventHandler(this.toolStripNewFolder);
            // 
            // toolStripButtonZip
            // 
            this.toolStripButtonZip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZip.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZip.Image")));
            this.toolStripButtonZip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZip.Name = "toolStripButtonZip";
            this.toolStripButtonZip.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonZip.Text = "toolStripButtonZip";
            this.toolStripButtonZip.ToolTipText = "Zip Folder";
            this.toolStripButtonZip.Click += new System.EventHandler(this.toolStripButtonZip_Click);
            // 
            // toolStripFromClipboard
            // 
            this.toolStripFromClipboard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripFromClipboard.Image = global::DraftTable.Properties.Resources.icon_paste_small;
            this.toolStripFromClipboard.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripFromClipboard.Name = "toolStripFromClipboard";
            this.toolStripFromClipboard.Size = new System.Drawing.Size(23, 22);
            this.toolStripFromClipboard.Text = "From Clipboard";
            this.toolStripFromClipboard.Click += new System.EventHandler(this.toolStripFromClipboard_Click);
            // 
            // toolStripRenderAll
            // 
            this.toolStripRenderAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripRenderAll.Image = global::DraftTable.Properties.Resources.icon_image_small;
            this.toolStripRenderAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRenderAll.Name = "toolStripRenderAll";
            this.toolStripRenderAll.Size = new System.Drawing.Size(23, 22);
            this.toolStripRenderAll.ToolTipText = "Render All Key Files";
            this.toolStripRenderAll.Click += new System.EventHandler(this.toolStripRenderAll_Click);
            // 
            // DraftTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 490);
            this.Controls.Add(this.splitContainer1);
            this.Name = "DraftTableForm";
            this.Text = "DraftTable";
            this.Load += new System.EventHandler(this.DraftTableForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStripSave.ResumeLayout(false);
            this.toolStripSave.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ThumbnailPanel ThumbViewer;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ThumbnailPanel FolderThumbnail;
        private System.Windows.Forms.CheckBox checkBoxBackup;
        private System.Windows.Forms.ToolStrip toolStripSave;
        private System.Windows.Forms.ToolStripButton toolStripSaveFile;
        private System.Windows.Forms.ToolStripButton toolStripExportFile;
        private System.Windows.Forms.ToolStripButton toolStripAddFolder;
        private System.Windows.Forms.ToolStripButton toolStripButtonZip;
        private System.Windows.Forms.CheckBox checkBoxDiff;
        private System.Windows.Forms.ToolStripButton toolStripFromClipboard;
        private System.Windows.Forms.ToolStripButton toolStripRenderAll;
    }
}