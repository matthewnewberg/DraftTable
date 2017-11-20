using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DraftTable
{
    public partial class RevertForm : Form
    {
        string file;

        DraftTableForm mainForm;


        public class RevertTableItem
        {
            public string filename;
            public string destFilename;
            public string backupFilename;
            public DateTime date;

            public RevertTableItem(string filename, string destFilename, string srcFilename)
            {

                this.filename = filename;
                this.destFilename = destFilename;
                this.backupFilename = srcFilename;
                this.date = FileUtils.GetBackupFileDate(backupFilename);
            }

            public override string ToString()
            {
                return "Date:" + date.ToString();
            }
        }

        List<RevertTableItem> table = new List<RevertTableItem>();

        public RevertForm(string file, DraftTableForm mainForm)
        {
            InitializeComponent();

            this.file = file;

            string filename = System.IO.Path.GetFileName(file);
            
            string path = Path.GetDirectoryName(file);

            string backupPath = Path.Combine(path, "Backup");

            var files = Directory.GetFiles(backupPath, "*.dt", SearchOption.TopDirectoryOnly);

            foreach (var f in files)
            {
                if (FileUtils.BackupMatchesFilename(f, filename))
                {
                    table.Add(new RevertTableItem(filename, file, f));
                }
            }

            table.Sort((x, y) => x.date.CompareTo(y.date));

            this.mainForm = mainForm;

            this.listBox1.DataSource = table;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var item = this.listBox1.SelectedItem as RevertTableItem;

            bool backup = false;
            bool diffBackup = false;

            if (mainForm != null)
            {
                backup = mainForm.BackupMode;
                diffBackup = mainForm.DiffMode;
            }

            FileUtils.RevertFromBackup(item.backupFilename, item.destFilename, backup, diffBackup);

            if (mainForm != null)
                mainForm.LoadFolder();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
