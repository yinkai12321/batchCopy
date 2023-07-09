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
using System.Threading;
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
            this.WindowState = FormWindowState.Maximized;
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
            // 如果应用程序设置中保存有上次选择的文件夹路径，则将其设置为初始文件夹路径
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastFolderPath))
            {
                dialog.SelectedPath = Properties.Settings.Default.LastFolderPath;
            }

            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 保存所选文件夹的路径到应用程序设置中
                Properties.Settings.Default.LastFolderPath = dialog.SelectedPath;
                Properties.Settings.Default.Save();

                string foldPath = dialog.SelectedPath;

                txtCutSelectFolder.Text = foldPath;

                imageListView1.Items.Clear();

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
            var filenameList = imageListView1.Items;
            foreach (var item in filenameList)
            {
                var img = Image.FromFile(item.FileName);
                Image croppedImage = null;
                if (numTopPx.Value > 0)
                {
                    // 裁剪像素
                    var cutpx = Convert.ToInt32(numTopPx.Value);
                    croppedImage = MovePxImg(img, cutpx, "上");
                }
                else if (numLeftPx.Value > 0)
                {
                    // 裁剪像素
                    var cutpx = Convert.ToInt32(numLeftPx.Value);
                    croppedImage = MovePxImg(img, cutpx, "左");
                }
                else if (numDownPx.Value > 0)
                {
                    // 裁剪像素
                    var cutpx = Convert.ToInt32(numDownPx.Value);
                    croppedImage = MovePxImg(img, cutpx, "下");
                }
                else if (numRightPx.Value > 0)
                {
                    // 裁剪像素
                    var cutpx = Convert.ToInt32(numRightPx.Value);
                    croppedImage = MovePxImg(img, cutpx, "右");
                }
                else if (numFullPx.Value > 0)
                {
                    // 裁剪像素
                    var cutpx = Convert.ToInt32(numFullPx.Value);
                    croppedImage = MovePxImg(img, cutpx, "四周");
                }

                if (croppedImage == null)
                {
                    MessageBox.Show("请输入裁剪像素！");
                    return;
                }

                img.Dispose();

                croppedImage.Save(item.FileName);
            }

            MessageBox.Show("裁剪成功。");
        }


        /// <summary>
        /// 裁剪(备份文件)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            var filenameList = imageListView1.Items;
            foreach (var item in filenameList)
            {
                var img = Image.FromFile(item.FileName);
                Image croppedImage = null;
                if (numTopPx.Value > 0)
                {
                    // 裁剪像素
                    var cutpx = Convert.ToInt32(numTopPx.Value);
                    croppedImage = MovePxImg(img, cutpx,"上");
                }
                else if (numLeftPx.Value > 0)
                {
                    // 裁剪像素
                    var cutpx = Convert.ToInt32(numLeftPx.Value);
                    croppedImage = MovePxImg(img, cutpx, "左");
                }
                else if (numDownPx.Value > 0)
                {
                    // 裁剪像素
                    var cutpx = Convert.ToInt32(numDownPx.Value);
                    croppedImage = MovePxImg(img, cutpx, "下");
                }
                else if (numRightPx.Value > 0)
                {
                    // 裁剪像素
                    var cutpx = Convert.ToInt32(numRightPx.Value);
                    croppedImage = MovePxImg(img, cutpx, "右");
                }
                else if (numFullPx.Value > 0)
                {
                    // 裁剪像素
                    var cutpx = Convert.ToInt32(numFullPx.Value);
                    croppedImage = MovePxImg(img, cutpx, "四周");
                }

                if (croppedImage == null)
                {
                    MessageBox.Show("请输入裁剪像素！");
                    return;
                }

                var bakPath = Path.Combine(item.FilePath, "bak");
                if (!Directory.Exists(bakPath))
                {
                    Directory.CreateDirectory(bakPath);
                }

                var path = Path.Combine(bakPath, item.FileName.Split('\\').Last());
                croppedImage.Save(path);
            }
            MessageBox.Show("裁剪成功。图片保存至BAK文件夹");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Image MovePxImg(Image img,int cutpx, string type)
        {
            var zoomImg = ZoomImage(img, cutpx);

            Rectangle cropRectangle = new Rectangle();
            if (type == "上")
            {
                cropRectangle = new Rectangle(cutpx/2, cutpx, zoomImg.Width - cutpx, zoomImg.Height - cutpx);
            }
            else if (type == "左")
            {
                cropRectangle = new Rectangle(cutpx, cutpx/2, zoomImg.Width - cutpx, zoomImg.Height - cutpx);

            }
            else if (type == "下")
            {
                cropRectangle = new Rectangle(cutpx/2, 0, zoomImg.Width - cutpx, zoomImg.Height - cutpx);
            }
            else if (type == "右")
            {
                cropRectangle = new Rectangle(0, cutpx/2, zoomImg.Width - cutpx, zoomImg.Height - cutpx);
            }
            else if (type == "四周")
            {
                cropRectangle = new Rectangle(cutpx/2, cutpx/2, zoomImg.Width - cutpx, zoomImg.Height - cutpx);
            }

            Image croppedImage = CropImage(zoomImg, cropRectangle);
            return croppedImage;
        }


        /// <summary>
        /// 放大图片
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="zoomSize"></param>
        /// <returns></returns>
        public Image ZoomImage(Image sourceImage, int zoomSize)
        {
            int width = sourceImage.Width + zoomSize;
            int height = sourceImage.Height + zoomSize;
            Bitmap zoomedBitmap = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(zoomedBitmap))
            {
                // 设置绘图质量，以便更好地缩放图像
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(sourceImage, new Rectangle(zoomSize/2, zoomSize/2, sourceImage.Width, sourceImage.Height));

                // 在 Bitmap 上绘制缩放后的图像，并拉伸以适应 Bitmap 的大小
                graphics.DrawImage(sourceImage, new Rectangle(0, 0, sourceImage.Width + zoomSize, sourceImage.Height + zoomSize),
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), GraphicsUnit.Pixel);
            }

            return zoomedBitmap;
        }

        /// <summary>
        /// 裁剪图片
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="cropRectangle"></param>
        /// <returns></returns>
        public Image CropImage(Image sourceImage, Rectangle cropRectangle)
        {
            Bitmap croppedBitmap = new Bitmap(cropRectangle.Width, cropRectangle.Height);

            using (Graphics graphics = Graphics.FromImage(croppedBitmap))
            {
                graphics.DrawImage(sourceImage, new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height), cropRectangle, GraphicsUnit.Pixel);
            }

            return croppedBitmap;
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
