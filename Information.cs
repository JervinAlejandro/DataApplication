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

        public string DisplayInfos()
        {
            return getName() + getCategory() + getDefinition() + getStructure();
        }
        public int CompareTo(Information other)
        {
            return this.name.CompareTo(other.name);
        }
    }
}
