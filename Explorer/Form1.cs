using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    public partial class Form1 : Form
    {
        ImageList imageList = new ImageList();
        ImageList imageListLarge = new ImageList();
        bool mode = true;
        public Form1()
        {
            InitializeComponent();
        }

        private StringBuilder AppendPath(TreeNode node, StringBuilder acc)
        {
            var parent = node.Parent;

            if (parent is null)
            {
                return acc.Length > 0 ? acc : new StringBuilder(node.Text);
            }

            string text = parent.Text[parent.Text.Length - 1] == '\\' ? parent.Text : parent.Text + '\\';
            acc.Insert(0, text);

            return AppendPath(parent, acc);
        }

        private string ConstructPath(TreeNode node)
        {
            StringBuilder sb = new StringBuilder(node.Text);
            var path = AppendPath(node, sb);
            return path.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var drives = Directory.GetLogicalDrives();
            treeView1.Nodes.Clear();
            imageListLarge.ImageSize = new Size(32, 32);
            treeView1.ImageList = imageList;
            listView1.LargeImageList = imageListLarge;
            imageList.Images.Add(icoresx.hdd);
            TreeNode node = null;
            foreach (string drivePath in drives)
            {
                node = treeView1.Nodes.Add(drivePath, drivePath, "hdd");
                node.Nodes.Add("empty");
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var node = e.Node;
            string path = ConstructPath(node);

            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            treeView1.SelectedNode = node;
            node.Nodes.Clear();
            imageList.Images.Clear();
            listView1.Items.Clear();

            foreach (var dir in directories)
            {
                imageList.Images.Add(icoresx.folder);
                imageListLarge.Images.Add(icoresx.floppy);
                var info = new DirectoryInfo(dir);
                listView1.Items.Add(new ListViewItem(info.Name, "folder"));

                var dirNode = node.Nodes.Add(info.Name, info.Name, "folder");
                dirNode.Nodes.Add("empty");
            }

            foreach (var file in files)
            {
                imageList.Images.Add(file, Icon.ExtractAssociatedIcon(file));
                imageListLarge.Images.Add(file, Icon.ExtractAssociatedIcon(file));
                var info = new FileInfo(file);
                listView1.Items.Add(new ListViewItem(info.Name, file));

                node.Nodes.Add(info.Name, info.Name, file);
            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.Item.Selected)
                return;

            var path = ConstructPath(treeView1.SelectedNode) + $"\\{e.Item.Text}";
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);
            string infoStr = string.Empty;

            infoStr += $"Путь к файлу: {info.FileName}\n";
            infoStr += $"Производитель: {info.CompanyName}\n";
            infoStr += $"Комментарии: {info.Comments}\n";
            infoStr += $"Номер версии файла: {info.FileVersion}\n";
            infoStr += $"Номер сборки файла: {info.FileBuildPart}\n";

            MessageBox.Show(infoStr, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (mode)
            {
                mode = false;
                listView1.BackColor = Color.Gray;
                treeView1.BackColor = Color.Black;
                this.BackColor = Color.Black;
                treeView1.ForeColor = Color.White;
                listView1.ForeColor = Color.White;
            }
            else
            {
                mode = true;
                listView1.BackColor = Color.White;
                treeView1.BackColor = Color.White;
                this.BackColor = Color.White;
                treeView1.ForeColor = Color.Black;
                listView1.ForeColor = Color.Black;
            }
        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.LargeImageList.ImageSize = new Size(64, 64);
        }

        private void standartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.LargeImageList.ImageSize = new Size(32, 32);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Form1_Load(0, null);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.Expand();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.Collapse();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
                this.notifyIcon1.Visible = true;
                this.notifyIcon1.ShowBalloonTip(3);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
