using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

// PR = Program Requirement

// Jervin Alejandro
// Date: 28 April 2022
// Version 1.2
// Data Application
// The application enables users to create, delete, edit, and search databases.
// Additionally, the program displays each database in the list view the user adds.
// Finally, it can save data to a bin file and open a bin file containing all saved databases.

namespace DataApplication
{
    // PR: 6.16 All code is required to be adequately commented.
    // Map the programming criteria and features to your code/methods by adding comments above the method signatures.
    // Ensure your code is compliant with the CITEMS coding standards.
    public partial class DataApplication : Form
    {
        public DataApplication()
        {
            InitializeComponent();
        }
        // PR: 6.2 Create a global List<T> of type Information called Wiki.
        // PR: 6.4 Create and initialise a global string array with the six categories as
        // indicated in the Data Structure Matrix. 
        List<Information> wiki = new List<Information>();
        string[] category = { "Array", "List", "Tree", "Graphs", "Abstract", "Hash" };

        #region FormLoad
        // PR: 6.4 Create a custom method to populate the ComboBox when the Form Load method is called.
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxCategory.Items.AddRange(category);
            clearTextBox();
            if(File.Exists("default.bin") == true)
            {
                open("default.bin");
                var result = MessageBox.Show("Default.bin successfully loaded", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            display();
        }
        // PR: 6.15 The Wiki application will save data when the form closes. 
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            save("default.bin");
            var result = MessageBox.Show("Data Automatically saved to Default.bin", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
        #region Buttons
        // PR: 6.3 Create a button method to ADD a new item to the list
        // Use a TextBox for the Name input, ComboBox for the Category,
        // Radio group for the Structure and Multiline TextBox for the Definition.
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (checkEmpty() == true)
            {
                var result = MessageBox.Show("Ensure that every attributes are filled", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "-";
            }
            else if(validName(textBoxName.Text) == false)
            {
                var result = MessageBox.Show("Name already exist", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "-";
            }
            else
            {
                Information addNewInformation = new Information();
                addNewInformation.setName(textBoxName.Text);
                addNewInformation.setCategory(comboBoxCategory.Text);
                addNewInformation.setDefinition(textBoxDefinition.Text);
                addNewInformation.setStructure(getRadioBox());
                wiki.Add(addNewInformation);
                toolStripStatusLabel1.Text = "Add Success";
                clearTextBox();
            }
            display();
        }
        // 6.8 Create a button method that will save the edited record of the currently selected item in the ListView.
        // All the changes in the input controls will be written back to the list.
        // Display an updated version of the sorted list at the end of this process.
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            try
            {
                int currentSelect = listView1.SelectedIndices[0];
                if (checkEmpty() == true)
                {
                    var result = MessageBox.Show("Ensure that every attributes are filled", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    toolStripStatusLabel1.Text = "-";
                }
                else if (!wiki[currentSelect].getName().Equals(textBoxName.Text) && validName(textBoxName.Text) == false)
                {
                    var result = MessageBox.Show("Name already exist", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    toolStripStatusLabel1.Text = "-";
                }
                else
                {
                    showTextBox("set", currentSelect);
                    toolStripStatusLabel1.Text = "Edit Success";
                    clearTextBox();
                    display();
                }
            }
            catch(System.ArgumentOutOfRangeException)
            {
                var result = MessageBox.Show("No data is selected", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "-";
            }
        }
        //6.7 Create a button method that will delete the currently selected record in the ListView.
        //Ensure the user has the option to backout of this action by using a dialog box.
        //Display an updated version of the sorted list at the end of this process.
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
            catch(System.ArgumentOutOfRangeException)
            {
                var result = MessageBox.Show("No data is selected", "ERROR", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "-";
            }

        }
        // PR: 6.10 Create a button method that will use the builtin binary search to find a Data Structure name.
        // If the record is found the associated details will populate the appropriate input controls and highlight
        // the name in the ListView. At the end of the search process the search input TextBox must be cleared.
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            Information temp = new Information();
            temp.setName(textBoxSearch.Text);
            int test = wiki.BinarySearch(temp);
            if (!string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
                if (test > -1)
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
                    clearTextBox();
                    toolStripStatusLabel1.Text = "-";
                }
            }
            else
            {
                var result = MessageBox.Show("TextBox is empty", "ERROR",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "-";
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
                clearTextBox();
                display();
                MessageBox.Show("Open Success");
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
        // 6.9 Create a single custom method that will sort and then display the Name and Category from the
        // wiki information in the list.
        private void display()
        {
            sortList();
            listView1.Items.Clear();
            foreach(var info in wiki)
            {
                ListViewItem lvi = new ListViewItem(info.getName());
                lvi.SubItems.Add(info.getCategory());
                listView1.Items.Add(lvi);
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
        private void showTextBox(string text, int index)
        {
            if(text == "set")
            {
                wiki[index].setName(textBoxName.Text);
                wiki[index].setCategory(comboBoxCategory.Text);
                wiki[index].setDefinition(textBoxDefinition.Text);
                wiki[index].setStructure(getRadioBox());
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
        // PR: 6.12 Create a custom method that will clear and reset the TextBboxes, ComboBox and Radio button
        private void clearTextBox()
        {
            textBoxName.Text = string.Empty;
            textBoxDefinition.Text = string.Empty;
            comboBoxCategory.Text = category[0];
            radioButtonLinear.Checked = false;
            radioButtonNonLinear.Checked = false;
        }
        // PR: 6.11 Create a ListView event so a user can select a Data Structure Name from the list of Names
        // and the associated information will be displayed in the related text boxes combo box and radio button.
        private void listView1_Click(object sender, EventArgs e)
        {
            int currentRecord = listView1.SelectedIndices[0];
            showTextBox("get", currentRecord);
        }
        // PR: 6.13 Create a double click event on the Name TextBox to clear the TextBboxes, ComboBox and Radio button.
        private void textBoxName_DoubleClick(object sender, EventArgs e)
        {
            clearTextBox();
        }
        // 6.6 Create two methods to highlight and return the values from the Radio button GroupBox.
        // The first method must return a string value from the selected radio button (Linear or Non-Linear).
        // The second method must send an integer index which will highlight an appropriate radio button.
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
            if (getRadioBox() == "Linear")
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
        // PR: 6.5 Create a custom ValidName method which will take a parameter string
        // value from the Textbox Name and returns a Boolean after checking for duplicates.
        // Use the built in List<T> method “Exists” to answer this requirement.
        private bool validName(string textBox)
        {
            if(wiki.Exists(x => x.getName() == textBox))
            {
                return false;
            }
            return true;
        }
        // PR:6.14 Create two buttons for the manual open and save option; this must use a dialog box to select a file
        // or rename a saved file. All Wiki data is stored/retrieved using a binary file format.
        private void save(string saveName)
        {
            try
            {
                using (Stream stream = File.Open(saveName, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, wiki);
                }
            }
            catch (IOException)
            {
                var result = MessageBox.Show("Cannot Save File", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "-";
            }
        }
        private void open(string openName)
        {
            try
            {
                using (Stream stream = File.Open(openName, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    while (stream.Position < stream.Length)
                    {
                        wiki = (List<Information>)bin.Deserialize(stream);
                    }
                }
            }
            catch (System.InvalidCastException ex)
            {
                var result = MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "-";
            }
        }
        #endregion


    }
}
