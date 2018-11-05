using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Diagnostics;


namespace MilutinTrisic
{
    public partial class Form1 : Form

    {

        public const int MAX_DIRECTORY_DEPTH = 2; //Decides how many subfolders you want to be displayed, in order to avoid poor performace

        public Form1()
        {
            InitializeComponent();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
          

            treeView1.Nodes.Clear();
            foreach (DriveInfo d in allDrives)
            {


                try
                {

                    ListDirectory(treeView1, d.ToString());//Adding all of the drives to the treeView

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    MessageBox.Show(ex.Message);
                }

            }





        }

        
        void ListDirectory(TreeView treeView, string path)
        {

            
            var rootDirectoryInfo = new DirectoryInfo(path);

            treeView.Nodes.Add(CreateTreeViewNodeFromDirectoryInfo(rootDirectoryInfo));
            


        }

        TreeNode CreateTreeViewNodeFromDirectoryInfo(DirectoryInfo directoryInfo, int depth = 0)
        {
            try
            {
                
                var directoryNode = new TreeNode(directoryInfo.Name);
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    if (depth < MAX_DIRECTORY_DEPTH)
                    {
                        directoryNode.Nodes.Add(CreateTreeViewNodeFromDirectoryInfo(directory, depth + 1));//Adding child nodes, but only as much as we allow it 
                    }                                                                                      //using MAX_DIRECTORY_DEPTH
                   
                }
                foreach (var file in directoryInfo.GetFiles())
                {

                    if (!comboBox1.Items.Contains(file.Extension))//Fills the comboBox with all of the file extensions
                    {                                             //found within the TreeView
                        comboBox1.Items.Add(file.Extension);      
                    }

                        directoryNode.Nodes.Add(new TreeNode(file.Name));
                    
                                            
                }

                return directoryNode;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error processing path; Error: " + e.Message, "\nDetails: " + e.ToString());
                return new TreeNode("UnaccessableNode");//Makes inaccessible directories appear as "UnaccessableNode" within the treeView
            }

        }

       

      



        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                string getInfo = treeView1.SelectedNode.FullPath;



                if (treeView1.SelectedNode.Parent == null)//Checks if the node is a drive
                {
                    DriveInfo di = new DriveInfo(getInfo);
                    string name = di.Name;
                    string freeSpace = di.TotalFreeSpace.ToString();
                    string totalSize = di.TotalSize.ToString();
                    this.textBox1.Text = name;                    
                    this.textBox3.Text = freeSpace;
                    this.textBox4.Text = totalSize;
                }
                else
                {
                    
                        

                    FileAttributes attr = File.GetAttributes(getInfo);

                        if (attr.HasFlag(FileAttributes.Directory))//If not a drive, checks if the selected node is a directory
                        {

                            DirectoryInfo dir = new DirectoryInfo(getInfo);
                            string folderName = dir.Name;
                            int numOfFiles = dir.GetDirectories().Length;
                            string folderAttributes = dir.Attributes.ToString();
                            this.textBox5.Text = folderName;
                            this.textBox6.Text = numOfFiles.ToString();
                            this.textBox7.Text = folderAttributes;
                        }

                        else//If neither a drive, nor a directory, we can safely assume it is a file.
                        {
                            FileInfo fi = new FileInfo(getInfo);
                            string fileName = fi.Name;
                            string fileSize = fi.Length.ToString();
                            string fileAttributes = fi.Attributes.ToString();
                            this.textBox8.Text = fileName;
                            this.textBox9.Text = fileSize;
                            this.textBox10.Text = fileAttributes;

                        }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error; Message: " + ex.Message + "; \nDetails: " + ex.ToString());
                MessageBox.Show("Error: " + ex.Message);
            }


        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)//using ShellExecute, allows us to open file or folder on double click
        {                                                                       

            string getInfo = treeView1.SelectedNode.FullPath;
            Process process = new Process();

            try
            {

                
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = getInfo;
                process.Start();


            }
                catch (Exception ex)
            {
                Console.WriteLine("Error; Message: " + ex.Message + "; \nDetails: " + ex.ToString());
                MessageBox.Show("Error: " + ex.Message);
            }
        }

      
    }
}
