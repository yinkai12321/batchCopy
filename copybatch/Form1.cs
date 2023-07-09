using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Manina.Windows.Forms.ImageListView;

namespace copybatch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ////选择文件
            //OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Multiselect = true;//该值确定是否可以选择多个文件
            //dialog.Title = "请选择文件夹";
            //dialog.Filter = "所有文件(*.*)|*.*";
            //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{

            //    foreach (var file in dialog.FileNames)
            //    {
            //    }

            //    txtSourceFile.Text = string.Join(",", dialog.FileNames);

            //}

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;

                txtSourceFile.Text = foldPath;
               
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;

                txtTargetFolder.Text = foldPath;

                DirectoryInfo theFolder = new DirectoryInfo(foldPath);
                DirectoryInfo[] dirInfo = theFolder.GetDirectories();//获取所在目录的文件夹

                txtFolderList.Text = "";
                foreach (var item in dirInfo)
                {
                    txtFolderList.Text += item.FullName + "\r\n";
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            imageListView1.AllowDuplicateFileNames = true;
            //imageListView1.SetRenderer(new ImageListViewRenderers.DefaultRenderer());
            imageListView1.SortColumn = 0;
            imageListView1.SortOrder = Manina.Windows.Forms.SortOrder.AscendingNatural;
            // Find and add built-in renderers
            Assembly assembly = Assembly.GetAssembly(typeof(ImageListView));
            string cacheDir = Path.Combine(
                Path.GetDirectoryName(new Uri(assembly.GetName().CodeBase).LocalPath),
                "Cache"
                );
            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);
            imageListView1.PersistentCacheDirectory = cacheDir;
            imageListView1.Columns.Add(ColumnType.Name);
            imageListView1.Columns.Add(ColumnType.Dimensions);
            imageListView1.Columns.Add(ColumnType.FileSize);
            imageListView1.Columns.Add(ColumnType.FolderName);
            imageListView1.Columns.Add(ColumnType.DateModified);
            imageListView1.Columns.Add(ColumnType.FileType);
            var col = new ImageListView.ImageListViewColumnHeader(ColumnType.Custom, "random", "Random");
            col.Comparer = new RandomColumnComparer();
            imageListView1.Columns.Add(col);

        }
        public class RandomColumnComparer : IComparer<ImageListViewItem>
        {
            public int Compare(ImageListViewItem x, ImageListViewItem y)
            {
                return int.Parse(x.SubItems["random"].Text).CompareTo(int.Parse(y.SubItems["random"].Text));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtSourceFile.Text))
                {
                    MessageBox.Show("请选择源文件");
                    return;
                }

                if (string.IsNullOrEmpty(txtFolderList.Text))
                {
                    MessageBox.Show("目的文件夹为空");
                    return;
                }

                var sourceFiles = txtSourceFile.Text.Split(',');
                var targetFiles = txtFolderList.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                foreach (var source in sourceFiles)
                {
                    foreach (var target in targetFiles)
                    {
                        if (string.IsNullOrEmpty(target))
                        {
                            continue;
                        }
                        //File.Copy(source, Path.Combine(target, source.Split('\\').Last()));

                        CopyFolder(source,target);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("失败，"+ex.Message);
                return;
            }

            MessageBox.Show("批量复制成功");

        }

        /// <summary>
        /// 复制文件夹及文件
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public void CopyFolder(string sourceFolder, string destFolder)
        {
           
            //如果目标路径不存在,则创建目标路径
            if (!System.IO.Directory.Exists(destFolder))
            {
                System.IO.Directory.CreateDirectory(destFolder);
            }
            //得到原文件根目录下的所有文件
            string[] files = System.IO.Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileName(file);

                var sourceFoldername = sourceFolder.Split('\\').Last(); ;

                var targetFold = Path.Combine(destFolder, sourceFoldername);

                if (!System.IO.Directory.Exists(targetFold))
                {
                    System.IO.Directory.CreateDirectory(targetFold);
                }

                string dest = System.IO.Path.Combine(targetFold, name);
                System.IO.File.Copy(file, dest);//复制文件
            }
            //得到原文件根目录下的所有文件夹
            string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = System.IO.Path.GetFileName(folder);
                string dest = System.IO.Path.Combine(destFolder, name);
                CopyFolder(folder, dest);//构建目标路径,递归复制文件
            }
           

        }

        /// <summary>
        /// 文件名改为上级文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtSourceFile.Text))
                {
                    MessageBox.Show("请选择源文件");
                    return;
                }

                var sourceFiles = txtSourceFile.Text.Split(',');

                foreach (var source in sourceFiles)
                {
                    //得到原文件根目录下的所有文件夹
                    string[] folders = System.IO.Directory.GetDirectories(source);
                    foreach (var folder in folders)
                    {
                        //得到原文件根目录下的所有文件
                        string[] files = System.IO.Directory.GetFiles(folder);

                        if (files.Length > 0)
                        {
                            var file = new FileInfo(files[0]);
                            File.Move(files[0], folder + file.Extension);
                        }

                    }
                 

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("失败，"+ex.Message);
                return;
            }

            MessageBox.Show("批量修改成功");
        }


        /// <summary>
        /// 删除文件名前面的数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSourceFile.Text))
            {
                MessageBox.Show("请选择源文件");
                return;
            }

            var sourceFiles = txtSourceFile.Text.Split(',');

            foreach (var source in sourceFiles)
            {
                // 遍历文件及子文件夹
                listDirectory(source);

            }

            MessageBox.Show("批量修改成功");
        }

        private void listDirectory(string path)
        {
            DirectoryInfo theFolder = new DirectoryInfo(@path);

            //遍历文件
            foreach (FileInfo NextFile in theFolder.GetFiles())
            {
                try
                {
                    var fileName = NextFile.Name.Split('-')[1];
                    var newPath = Path.Combine(NextFile.DirectoryName, fileName);
                    File.Move(NextFile.FullName, newPath);
                }
                catch (Exception)
                {

                }
            }

            //遍历文件夹
            foreach (DirectoryInfo NextFolder in theFolder.GetDirectories())
            {
                listDirectory(NextFolder.FullName);
            }
        }


        /// <summary>
        /// 图片裁剪选择文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;

                txtCutSelectFolder.Text = foldPath;

                GetSelectImg(foldPath);
            }
        }


        private void GetSelectImg(string path)
        {
            DirectoryInfo theFolder = new DirectoryInfo(path);
            Random rnd = new Random();

            var selectFolder = txtSetSelectFolder.Text.Split(';');
            if (selectFolder.Where(t=>t == theFolder.Name).Count() > 0)
            {
                //遍历文件
                foreach (FileInfo p in theFolder.GetFiles())
                {
                    try
                    {
                        if (p.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.EndsWith(".cur", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.EndsWith(".emf", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.EndsWith(".wmf", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase) ||
                            p.Name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                        {
                            ImageListViewItem item = new ImageListViewItem(p.FullName);
                            item.SubItems.Add("random", rnd.Next(0, 999).ToString("000"));
                            imageListView1.Items.Add(item);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
         

            //遍历文件夹
            foreach (DirectoryInfo NextFolder in theFolder.GetDirectories())
            {
                GetSelectImg(NextFolder.FullName);
            }
        }


        /// <summary>
        /// 裁剪
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 裁剪(备份文件)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        ///  更改图片列表显示模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbn = sender as RadioButton;

            //第一种
            if (rbn.Checked)
            {
                string tag = rbn.Text.ToString();
                switch (tag)
                {
                    case "缩略图": imageListView1.View = Manina.Windows.Forms.View.Thumbnails;  break;
                    case "画廊": imageListView1.View = Manina.Windows.Forms.View.Gallery; break;
                    case "窗格": imageListView1.View = Manina.Windows.Forms.View.Pane; break;
                    case "详细": imageListView1.View = Manina.Windows.Forms.View.Details; break;
                    default: break;
                }
            }
        }
    }
}
