using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace DraftTable
{
    class FileUtils
    {

        static internal List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };
        static internal List<string> CadExtensions = new List<string> { ".3DM" };

        public static string GetNextFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string pathName = Path.GetDirectoryName(fileName);
            string fileNameOnly = Path.Combine(pathName, Path.GetFileNameWithoutExtension(fileName));
            int i = 0;
            // If the file exists, keep trying until it doesn't

            do
            {
                i += 1;
                fileName = string.Format("{0}{1:000}{2}", fileNameOnly, i, extension);
            } while (File.Exists(fileName));

            return fileName;
        }

        public static byte[] GetBigEndianBytes(UInt32 val, bool isLittleEndian)
        {
            UInt32 bigEndian = val;
            if (isLittleEndian)
            {
                bigEndian = (val & 0x000000FFU) << 24 | (val & 0x0000FF00U) << 8 |
                     (val & 0x00FF0000U) >> 8 | (val & 0xFF000000U) >> 24;
            }
            return BitConverter.GetBytes(bigEndian);
        }

        public static byte[] GetBigEndianBytes(UInt32 val)
        {
            return GetBigEndianBytes(val, false);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static string Hash(byte[] temp)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(temp);
                return ByteArrayToString(hash);
            }
        }

        public static string RemoveWhiteSpace(string input)
        {
            var s = new StringBuilder(input.Length); // (input.Length);
            using (var reader = new StringReader(input))
            {
                int i = 0;
                char c;
                for (; i < input.Length; i++)
                {
                    c = (char)reader.Read();
                    if (!char.IsWhiteSpace(c))
                    {
                        s.Append(c);
                    }
                }
            }

            return s.ToString();
        }

        public static byte[] StringToByteArray(String hex)
        {
            hex = RemoveWhiteSpace(hex);
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                string pair = hex.Substring(i, 2);
                bytes[i / 2] = Convert.ToByte(pair, 16);
            }
            return bytes;
        }

        public static string ConvertToText(byte[] temp)
        {
            string hex = BitConverter.ToString(temp).Replace("-", string.Empty);
            return hex;
        }

        public static byte[] ConvertFromText(string txt)
        {
            return StringToByteArray(txt);
        }

        public static FileInfo MakeUnique(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string fileExt = Path.GetExtension(path);

            for (int i = 1; ; ++i)
            {
                if (!File.Exists(path))
                    return new FileInfo(path);

                path = Path.Combine(dir, fileName + " " + i + fileExt);
            }
        }

        public static bool SaveFile(string filename, bool export = false)
        {
            int selectedObjectCount = 0;

            var selectedObjects = Rhino.RhinoDoc.ActiveDoc.Objects.GetSelectedObjects(true, false);

            foreach (var o in selectedObjects)
                selectedObjectCount++;


            Rhino.FileIO.FileWriteOptions options = new Rhino.FileIO.FileWriteOptions();
            options.SuppressDialogBoxes = true;

            if (export && selectedObjectCount > 0)
                options.WriteSelectedObjectsOnly = true;

            return Rhino.RhinoDoc.ActiveDoc.WriteFile(filename, options);
        }

        public static void OpenFile(string tempFileName)
        {
            Rhino.FileIO.FileReadOptions readOptions = new Rhino.FileIO.FileReadOptions();

            readOptions.ImportMode = false;
            readOptions.OpenMode = true;
            Rhino.RhinoDoc.ReadFile(tempFileName, readOptions);
        }

        public static StringBuilder[] Split(StringBuilder input, char separator)
        {
            List<StringBuilder> results = new List<StringBuilder>();

            StringBuilder current = new StringBuilder();
            for (int i = 0; i < input.Length; ++i)
            {
                if (input[i] == separator)
                {
                    results.Add(current);
                    current = new StringBuilder();
                }
                else
                    current.Append(input[i]);
            }

            if (current.Length > 0)
                results.Add(current);

            return results.ToArray();
        }

        public static string WriteBackupFile(string filename, bool diffBackup)
        {
            if (!File.Exists(filename))
                return "";

            string backupPath = Path.Combine(Path.GetDirectoryName(filename), "Backup");

            Byte[] bytes = File.ReadAllBytes(filename);

            string threedmSHA = Hash(bytes);

            StringBuilder b64Str = new StringBuilder(ConvertToText(bytes));
            
            StringBuilder [] splitFiles = new StringBuilder[1];
            splitFiles[0] = b64Str;

            if (diffBackup && b64Str.Length < 100000) {
                b64Str = SplitByTCode(b64Str);
                splitFiles = Split(b64Str, System.Environment.NewLine[System.Environment.NewLine.Length - 1]);
            }

            string draftTableFile = "";

            draftTableFile += "#ext:" + Path.GetExtension(filename).Replace(",", "?") + "," + System.Environment.NewLine;
            draftTableFile += "#filename:" + Path.GetFileName(filename).Replace(",", "?") + "," + System.Environment.NewLine;
            draftTableFile += "#fullpath:" + Path.GetFullPath(filename).Replace(",", "?") + "," + System.Environment.NewLine;
            draftTableFile += "#timeUTC:" + DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "," + System.Environment.NewLine;
            draftTableFile += "#time:" + DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "," + System.Environment.NewLine;
            draftTableFile += "#user:" + System.Environment.UserName + "," + System.Environment.NewLine;
            draftTableFile += "#3dmHash:" + threedmSHA + "," + System.Environment.NewLine;

            for (int i = 0; i < splitFiles.Length; i++)
            {
                System.IO.Directory.CreateDirectory(backupPath);

                var split = splitFiles[i];

                string sha = Hash(Encoding.UTF8.GetBytes(split.ToString()));

                if (diffBackup)
                    split = SplitByTableRec(split);

                draftTableFile += sha + "," + System.Environment.NewLine;

                string split64 = Path.Combine(backupPath, sha + ".hex");

                File.WriteAllText(split64, split.ToString());
            }

            draftTableFile += "#EOF";

            var newdtfile = MakeUnique(Path.Combine(backupPath, threedmSHA + ".dt"));

            File.WriteAllText(newdtfile.FullName, draftTableFile);

            return newdtfile.FullName;
        }

        static public bool BackupMatchesFilename(string backupFile, string filename3dm)
        {
            string fn = Path.GetFileName(filename3dm);

            string searchString = "#filename:" + Path.GetFileName(fn).Replace(",", "?") + ",";

            foreach (string line in File.ReadLines(backupFile))
                if (searchString == line)
                    return true; // and stop reading lines

            return false;
        }

        static public DateTime GetBackupFileDate(string backupFile)
        {
            string searchString = "#time:";

            foreach (string line in File.ReadLines(backupFile))
                if (line.StartsWith(searchString))
                {
                    string dateString = line.Substring(searchString.Length).Replace(",", "");

                    return Convert.ToDateTime(dateString);
                }

            return DateTime.Now;
        }

        static public bool RevertFromBackup(string backupFile, string destinationFile, bool backup, bool diffBackup)
        {
            if (backup)
                WriteBackupFile(destinationFile, diffBackup);

            return ReadDTBackupfile(backupFile, destinationFile);
        }

        public static string GetActiveFolderFullPath(string folder)
        {
            if (String.IsNullOrWhiteSpace(folder))
                folder = "00";

            string path = GetDraftTableFolder();
            path = Path.Combine(path, folder);
            return path;
        }

        public static string GetDraftTableFolder()
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }

            path = Path.Combine(path, "DraftTable");
            path = Path.Combine(path, "Default");
            return path;
        }

        public static StringBuilder SplitByTableRec(StringBuilder splitInput)
        {
            StringBuilder split = new StringBuilder(splitInput.ToString());
            
            uint TCODE_TABLEREC = 0x20000000;
            //uint TCODE_TABLE = 0x10000000;
            //uint TCODE_CRC = 0x8000;
            //uint TCODE_OPENNURBS_OBJECT = 0x00020000;
            //uint TCODE_GEOMETRY = 0x00100000;
            //uint TCODE_INTERFACE = 0x02000000;
            uint TCODE_SHORT = 0x80000000;

            for (uint j = 1; j < 0x180; j++)
            {
                uint SEARCH = TCODE_TABLEREC | j;

                string searchString = ConvertToText(GetBigEndianBytes(SEARCH));

                split = split.Replace(searchString, System.Environment.NewLine + searchString);
            }

            for (uint j = 1; j < 0x180; j++)
            {
                uint SEARCH = TCODE_TABLEREC | j | TCODE_SHORT;

                string searchString = ConvertToText(GetBigEndianBytes(SEARCH));

                split = split.Replace(searchString, System.Environment.NewLine + searchString);
            }

            return split;
        }

        public static StringBuilder SplitByTCode(StringBuilder b64StrInput)
        {
            StringBuilder b64Str = new StringBuilder(b64StrInput.ToString());
            uint TCODE_TABLEREC = 0x20000000;
            uint TCODE_TABLE = 0x10000000;
            uint TCODE_CRC = 0x8000;
            uint TCODE_OPENNURBS_OBJECT = 0x00020000;
            uint TCODE_GEOMETRY = 0x00100000;
            uint TCODE_INTERFACE = 0x02000000;
            uint TCODE_SHORT = 0x80000000;
            for (uint i = 1; i < 0x40; i++)
            {

                uint SEARCH = TCODE_TABLE | i;

                string searchString = ConvertToText(GetBigEndianBytes(SEARCH));

                b64Str = b64Str.Replace(searchString, System.Environment.NewLine + searchString);
            }

            for (uint j = 1; j < 0x180; j++)
            {
                uint SEARCH = TCODE_TABLEREC | TCODE_CRC | j;

                string searchString = ConvertToText(GetBigEndianBytes(SEARCH));

                b64Str = b64Str.Replace(searchString, System.Environment.NewLine + searchString);
            }

            for (uint j = 1; j < 0x20; j++)
            {
                uint SEARCH = TCODE_GEOMETRY | j;

                string searchString = ConvertToText(GetBigEndianBytes(SEARCH));

                b64Str = b64Str.Replace(searchString, System.Environment.NewLine + searchString);
            }

            for (uint j = 1; j < 0x20; j++)
            {
                uint SEARCH = TCODE_OPENNURBS_OBJECT | j;

                string searchString = ConvertToText(GetBigEndianBytes(SEARCH));

                b64Str = b64Str.Replace(searchString, System.Environment.NewLine + searchString);
            }

            for (uint j = 1; j < 0x20; j++)
            {
                uint SEARCH = TCODE_INTERFACE | j;

                string searchString = ConvertToText(GetBigEndianBytes(SEARCH));

                b64Str = b64Str.Replace(searchString, System.Environment.NewLine + searchString);
            }

            for (uint j = 1; j < 0x20; j++)
            {
                uint SEARCH = TCODE_INTERFACE | j | TCODE_SHORT;

                string searchString = ConvertToText(GetBigEndianBytes(SEARCH));

                b64Str = b64Str.Replace(searchString, System.Environment.NewLine + searchString);
            }

            return b64Str;
        }

        public static bool ReadDTBackupfile(string dtFile, string output3dm)
        {
            string convertBack = "";

            string draftTableFile = File.ReadAllText(dtFile);

            string[] dtSplit = draftTableFile.Split(',');

            string splitPath = Path.GetDirectoryName(dtFile);

            for (int i = 0; i < dtSplit.Length; i++)
            {
                string temp = RemoveWhiteSpace(dtSplit[i]);

                if (temp.Length < 1)
                    continue;

                if (!Char.IsLetterOrDigit(temp[0]))
                    continue;

                if (temp.Length < 40)
                    continue;

                string dtHexFile = temp.Substring(0, 40);
                convertBack += File.ReadAllText(Path.Combine(splitPath, dtHexFile + ".hex"));
            }

            Byte[] bytesReturn = ConvertFromText(convertBack);

            File.WriteAllBytes(output3dm, bytesReturn);

            return true;
        }

        public static void DrawStringToBmp(string f, Bitmap bmp)
        {
            using (Graphics flagGraphics = Graphics.FromImage(bmp))
            {
                flagGraphics.FillRectangle(Brushes.Gray, 0, 0, 128, 128);
                flagGraphics.DrawImage(global::DraftTable.Properties.Resources.file_bitmap, 0, 128 - global::DraftTable.Properties.Resources.file_bitmap.Height/2);

                Font drawFont = new Font("Arial", 16);
                SolidBrush drawBrush = new SolidBrush(Color.OrangeRed);

                var stringFormat = StringFormat.GenericDefault;
                stringFormat.Alignment = StringAlignment.Center;

                Rectangle drawRect = new Rectangle(0, 0, 128, 128);

                flagGraphics.DrawString(Path.GetFileNameWithoutExtension(f), drawFont, drawBrush, drawRect, stringFormat);
            }
        }

        public static bool IsPythonImport(string line)
        {
            string re1 = "(from)";  // Word 1
            string re2 = "(\\s+)";  // White Space 1
            string re3 = "((?:[a-z][a-z\\.\\d\\-]+)\\.(?:[a-z][a-z\\-]+))(?![\\w\\.])"; // Fully Qualified Domain Name 1
            string re4 = "(\\s+)";  // White Space 2
            string re5 = "(import)";    // Word 2
            string re6 = ".*?"; // Non-greedy match on filler
            //string re7 = "((?:[a-z][a-z\\.\\d\\-]+)\\.(?:[a-z][a-z\\-]+))(?![\\w\\.])"; // Fully Qualified Domain Name 2

            Regex rImport = new Regex(re5 + re2 + re6, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Regex rFromStar = new Regex(re1 + re2 + re3 + re4 + re5 + re6, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            Match mImport = rImport.Match(line);
            Match mFromStar = rFromStar.Match(line);

            return mImport.Success | mFromStar.Success;
        }

        public static bool IsVBImport(string line)
        {
            string re1 = "(End)";  // Word 1
            string re2 = "(\\s+)";  // White Space 1
            string re3 = "(Sub)"; //Sub
            string re4 = "(Function)";  // Function
            string re6 = ".*?"; // Non-greedy match on filler

            Regex rSub = new Regex(re1 + re2 + re3 + re6, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Regex rFunction = new Regex(re1 + re2 + re4 + re6, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            Match mSub = rSub.Match(line);
            Match mFunction = rFunction.Match(line);

            return mSub.Success | mFunction.Success;
        }
    }
}
