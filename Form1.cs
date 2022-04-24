using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataApplication
{
    //[Serializable()]
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<Information> wiki = new List<Information>();
        string[] category = { "Array", "List", "Tree", "Graphs", "Abstract", "Hash" };
        Information addNewInformation;

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxCategory.Items.AddRange(category);
            clearTextBox();
            if(File.Exists("default.bin") == true)
            {
                open("default.bin");
            }
            display();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            save("default.bin");
        }

        #region Buttons
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (checkEmpty() == true)
            {
                var result = MessageBox.Show("Ensure that every attributes are filled", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(ValidName() == false)
            {
                var result = MessageBox.Show("Name already exist", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                addNewInformation = new Information();
                addNewInformation.setName(textBoxName.Text);
                addNewInformation.setCategory(comboBoxCategory.Text);
                addNewInformation.setDefinition(textBoxDefinition.Text);

                if (radioButtonLinear.Checked)
                {
                    addNewInformation.setStructure(radioButtonLinear.Text);
                }
                else
                {
                    addNewInformation.setStructure(radioButtonNonLinear.Text);
                }

                wiki.Add(addNewInformation);
                toolStripStatusLabel1.Text = "Add Success";
                clearTextBox();
            }
            wiki.Sort();
            display();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int currentSelect = listView1.SelectedIndices[0];
            var result = MessageBox.Show("Are you sure you want to delete \"" 
                + wiki[currentSelect].getName() + "\"?", "Question", MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                wiki.RemoveAt(currentSelect);
                toolStripStatusLabel1.Text = "Delete Success";
                clearTextBox();
                display();
            }
            else
            {
                toolStripStatusLabel1.Text = "Delete Cancelled";
                clearTextBox();
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveBin = new SaveFileDialog();
            saveBin.Filter = "Bin|*.bin";
            saveBin.Title = "Save WikiDatabase";
            saveBin.InitialDirectory = Application.StartupPath;
            saveBin.DefaultExt = "bin";
            saveBin.ShowDialog();
            string fileName = saveBin.FileName;

            if (string.IsNullOrWhiteSpace(saveBin.FileName))
            {
                save("default.bin");
                MessageBox.Show("Save Cancelled");
            }
            else
            {
                save(fileName);
                MessageBox.Show("Save Success");
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openBin = new OpenFileDialog();
            openBin.Filter = "Bin|*.bin";
            openBin.Title = "Open Bin File";
            openBin.InitialDirectory = Application.StartupPath;

            if (openBin.ShowDialog() == DialogResult.OK)
            {
                open(openBin.FileName);
                MessageBox.Show("Open Success");
                display();
                clearTextBox();
            }
            else
            {
                MessageBox.Show("Open Cancelled");
            }
        }
        #endregion

        #region functions
        private bool checkEmpty()
        {
            if(string.IsNullOrWhiteSpace(textBoxName.Text) || 
                string.IsNullOrWhiteSpace(textBoxDefinition.Text) ||
                string.IsNullOrWhiteSpace(comboBoxCategory.Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void display()
        {
            listView1.Items.Clear();
            foreach(var info in wiki)
            {
                ListViewItem lvi = new ListViewItem(info.getName());
                lvi.SubItems.Add(info.getCategory());
                listView1.Items.Add(lvi);
            }
        }
        private void clearTextBox()
        {
            textBoxName.Text = string.Empty;
            textBoxDefinition.Text = string.Empty;
            comboBoxCategory.Text = category[0];
            radioButtonLinear.Checked = true;
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            int currentRecord = listView1.SelectedIndices[0];
            textBoxName.Text = wiki[currentRecord].getName();
            comboBoxCategory.Text = wiki[currentRecord].getCategory();
            textBoxDefinition.Text = wiki[currentRecord].getDefinition();

            if(getRadioIndex() == 0)
            {
                radioButtonLinear.Checked = true;
            }
            else
            {
                radioButtonNonLinear.Checked = true;
            }
        }

        private string getRadioBox()
        {
            if (radioButtonLinear.Checked)
            {
                return radioButtonLinear.Text;
            }
            else
            {
                return radioButtonNonLinear.Text;
            }
        }

        private int getRadioIndex()
        {
            if (radioButtonLinear.Checked)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        private bool ValidName()
        {
            foreach(var information in wiki)
            {
                if(textBoxName.Text == information.getName())
                {
                    return false;
                }
            }
            return true;
        }

        private void save(string saveName)
        {
            try
            {
                using (Stream stream = File.Open(saveName, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    foreach(var info in wiki)
                    {
                        bin.Serialize(stream, info);
                    }
                }
            }
            catch (IOException ex)
            {
                var result = MessageBox.Show("Cannot Save File", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void open(string openName)
        {
            try
            {
                using (Stream stream = File.Open(openName, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    foreach(var info in wiki)
                    {
                        wiki = (List<Information>)bin.Deserialize(stream);
                    }
                }
            }
            catch (IOException ex)
            {
                var result = MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion


    }
}
