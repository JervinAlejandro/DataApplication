using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataApplication
{
    [Serializable()]
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
        public void setName (String name)
        {
            this.name = name;
        }
        public string getCategory()
        {
            return category;
        }
        public void setCategory(String category)
        {
            this.category = category;
        }
        public string getStructure()
        {
            return structure;
        }
        public void setStructure(String structure)
        {
            this.structure = structure;
        }
        public string getDefinition()
        {
            return definition;
        }
        public void setDefinition(String definition)
        {
            this.definition = definition;
        }

        public int CompareTo(Information other)
        {
            return this.name.CompareTo(other.name);
        }
        public int Compare(Information info, String textbox)
        {
            if (info.getName() == textbox)
            {
                return 0;
            }
            if (info == null) 
            { 
                return -1; 
            }
            
            if (textbox == null)
            {
                return 1;
            }

            return String.Compare(info.getName(), textbox);
        }
    }
}
