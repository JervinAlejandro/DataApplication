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
        string option = null;
        int temp1;

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
            else if(validName() == false)
            {
                var result = MessageBox.Show("Name already exist", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                addNewInformation = new Information();
                addNewInformation.setName(textBoxName.Text);
                addNewInformation.setCategory(comboBoxCategory.Text);
                addNewInformation.setDefinition(textBoxDefinition.Text);
                addNewInformation.setStructure(option);
                wiki.Add(addNewInformation);
                toolStripStatusLabel1.Text = "Add Success";
                clearTextBox();
            }
            sortList();
            display();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            try
            {
                int currentSelect = listView1.SelectedIndices[0];
                if (checkEmpty() == true)
                {
                    var result = MessageBox.Show("Ensure that every attributes are filled", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (!wiki[currentSelect].getName().Equals(textBoxName.Text) && validName() == false)
                {
                    var result = MessageBox.Show("Name already exist", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    showTextBox("set", currentSelect);
                    toolStripStatusLabel1.Text = "Edit Success";
                    clearTextBox();
                    display();
                }
            }
            catch(System.ArgumentOutOfRangeException ex)
            {
                var result = MessageBox.Show("No data is selected", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int currentSelect = listView1.SelectedIndices[0];
                var result = MessageBox.Show("Are you sure you want to delete \""
                    + wiki[currentSelect].getName() + "\"?", "Question", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
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
            catch(System.ArgumentOutOfRangeException ex)
            {
                var result = MessageBox.Show("No data is selected", "ERROR", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            Information temp = new Information();
            temp.setName(textBoxSearch.Text);
            int test = wiki.BinarySearch(temp);
            if (!string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
                if (test != -1)
                {
                    toolStripStatusLabel1.Text = "Data Found";
                    listView1.Focus();
                    listView1.Items[test].Selected = true;
                    showTextBox("get", test);
                }
                else
                {
                    var result = MessageBox.Show("Data not found", "Information",
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                var result = MessageBox.Show("TextBox is empty", "ERROR",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            
            textBoxSearch.Clear();
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
                clearTextBox();
                display();
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
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxDefinition.Text) ||
                string.IsNullOrWhiteSpace(comboBoxCategory.Text) ||
                
                (!radioButtonLinear.Checked && !radioButtonNonLinear.Checked))
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

        private void showTextBox(string text, int index)
        {
            if(text == "set")
            {
                wiki[index].setName(textBoxName.Text);
                wiki[index].setCategory(comboBoxCategory.Text);
                wiki[index].setDefinition(textBoxDefinition.Text);
                wiki[index].setStructure(option);
            }

            if(text == "get")
            {
                textBoxName.Text = wiki[index].getName();
                comboBoxCategory.Text = wiki[index].getCategory();
                textBoxDefinition.Text = wiki[index].getDefinition();
                if (wiki[index].getStructure() == "Linear")
                {
                    radioButtonLinear.Checked = true;
                }
                else
                {
                    radioButtonNonLinear.Checked = true;
                }
            }
            
        }
        private void clearTextBox()
        {
            textBoxName.Text = string.Empty;
            textBoxDefinition.Text = string.Empty;
            comboBoxCategory.Text = category[0];
            radioButtonLinear.Checked = false;
            radioButtonNonLinear.Checked = false;
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            int currentRecord = listView1.SelectedIndices[0];
            showTextBox("get", currentRecord);
        }

        private string getRadioBox()
        {
            if (radioButtonLinear.Checked)
            {
                option = radioButtonLinear.Text;
                return option;
            }
            else
            {
                option = radioButtonNonLinear.Text;
                return option;
            }
        }

        private int getRadioIndex()
        {
            if (option == "Linear")
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        private void radioButtonLinear_CheckedChanged_1(object sender, EventArgs e)
        {
            getRadioBox();
            getRadioIndex();
        }

        private bool validName()
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
                    foreach(Information info in wiki)
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
                    foreach(Information info in wiki)
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

        private void sortList()
        {
            string temp;
            string temp1;
            string temp2;
            string temp3;
            for (int x = 0; x < wiki.Count; x++)
            {
                for (int y = x + 1; y < wiki.Count; y++)
                {
                    if (wiki[x].getName().CompareTo(wiki[y].getName()) > 0)
                    {
                        temp = wiki[x].getName();
                        temp1 = wiki[x].getCategory();
                        temp2 = wiki[x].getStructure();
                        temp3 = wiki[x].getDefinition();

                        wiki[x].setName(wiki[y].getName());
                        wiki[x].setCategory(wiki[y].getCategory());
                        wiki[x].setStructure(wiki[y].getStructure());
                        wiki[x].setDefinition(wiki[y].getDefinition());

                        wiki[y].setName(temp);
                        wiki[y].setCategory(temp1);
                        wiki[y].setStructure(temp2);
                        wiki[y].setDefinition(temp3);


                    }
                }
            }
        }
        #endregion


    }
}
