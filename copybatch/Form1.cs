using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

    }
}
