using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Xml.Linq;


namespace HITUI
{
    public partial class DummySolutionExplorer : ToolWindow
    {
        public event EventHandler<UpdateTestArgs> UpdateTestSet = null;        
        private XElement m_listElements;


        public DummySolutionExplorer()
        {
            InitializeComponent();
        }

        public void OnUpdateSet(object sender, UpdateSolutionExplorerArgs args)
        {
            try
            {
                treeView1.Nodes.Clear();
                m_listElements = args.ListElements;
                TreeNode setNode = treeView1.Nodes.Add(args.SetName, Path.GetFileNameWithoutExtension(args.SetName), 0);
                IEnumerable<XElement> listEnum = m_listElements.Elements();
                foreach (XElement e in listEnum)
                {
                    string filename = (string)e;
                    setNode.Nodes.Add(Path.GetFileNameWithoutExtension(filename),
                        Path.GetFileNameWithoutExtension(filename), 8);
                }
                treeView1.ExpandAll();
            }
            catch { }

        }

        protected override void OnRightToLeftLayoutChanged(EventArgs e)
        {
            
        }

        public static void RemoveNode(TreeNode node)
        {
            try
            {
                TreeNode parent = node.Parent;
                if (parent != null)
                {
                    int index = parent.Nodes.IndexOf(node);
                    if (index >= 0)
                    {
                        parent.Nodes.RemoveAt(index);
                        // bw : add this line to restore the originally selected node as selected
                        node.TreeView.SelectedNode = node.PrevNode;
                    }
                }
            }
            catch { }
        }

        public static int MoveUp(TreeNode node)
        {
            int index = -1;
            TreeNode parent = node.Parent;
            if (parent != null)
            {
                index = parent.Nodes.IndexOf(node);
                if (index > 0)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index - 1, node);

                    // bw : add this line to restore the originally selected node as selected
                    node.TreeView.SelectedNode = node;
                }
            }
            return index;
        }

        public static int MoveDown(TreeNode node)
        {
            int index = -1;
            TreeNode parent = node.Parent;
            if (parent != null)
            {
                index = parent.Nodes.IndexOf(node);
                if (index < parent.Nodes.Count - 1)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index + 1, node);

                    // bw : add this line to restore the originally selected node as selected
                    node.TreeView.SelectedNode = node;
                }
                else
                {
                    index = -1;
                }
            }

            return index;
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            try
            {
                int index = MoveUp(treeView1.SelectedNode);
                if (index != -1)
                {
                    UpdateTestSet(this, new UpdateTestArgs(UpdateTestAction.TestUp,index));
                }
                
            }
            catch { }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            try
            {
                int index = MoveDown(treeView1.SelectedNode);
                if (index != -1)
                {
                    UpdateTestSet(this, new UpdateTestArgs(UpdateTestAction.TestDown, index));
                }
                
            }
            catch { }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateTestSet(this, new UpdateTestArgs(UpdateTestAction.TestAdd));
              
            }
            catch { }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                RemoveNode(treeView1.SelectedNode);
                UpdateTestSet(this, new UpdateTestArgs(UpdateTestAction.TestDelete));
            }
            catch { }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                e.Node.SelectedImageIndex = 0;
            }
               
        }

    }
}