using System;


namespace DataApplication
{
    [Serializable()]
    // PR: 6.1 Create a separate class file to hold the four data items of the Data Structure
    class Information : IComparable<Information>
    {
        private string name;
        private string category;
        private string structure;
        private string definition;

        public string getName()
        {
            return name;
        }
        public void setName(String newName)
        {
            name = newName;
        }
        public string getCategory()
        {
            return category;
        }
        public void setCategory(String newCategory)
        {
            category = newCategory;
        }
        public string getStructure()
        {
            return structure;
        }
        public void setStructure(String newStructure)
        {
            structure = newStructure;
        }
        public string getDefinition()
        {
            return definition;
        }
        public void setDefinition(String newDefinition)
        {
            definition = newDefinition;
        }

        public int CompareTo(Information other)
        {
            return this.name.CompareTo(other.name);
        }
    }
}
