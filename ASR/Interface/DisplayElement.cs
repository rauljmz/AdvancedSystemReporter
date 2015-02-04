using System;
using System.Collections.Generic;
using System.Linq;

namespace ASR.Interface
{
    public class DisplayElement
    {
        private class Column
        {
            public Column(string name, string value)
            {
                Name = name;
                Value = value;
            }
            public Column(string name, string value, string sortingvalue) : this(name,value)
            {
                SortingValue = sortingvalue;
            }
            public string Name { get; private set; }
            public string Value {get; private set;}
            string _sortingValue;
            public string SortingValue
            {
                get { return _sortingValue ?? Value; }
                private set { _sortingValue = value; }
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                var column = obj as Column;
                if (column == null) return base.Equals(obj);
                return column.Name.Equals(this.Name);
            }
        }
        private HashSet<Column> columns;                

        /// <summary>
        /// Object returned by the scanner.
        /// </summary>
        public object Element { get; private set; }

        public string Header { get; set; }

        public string Value { get; set; }
        
        public string Icon { get; set; }

        public string ExtraInfo { get; set; }

        internal DisplayElement(object element)
        {
            Element = element;
            columns = new HashSet<Column>();
            Icon = "";
            ExtraInfo = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Valid column names for this element</returns>
        public IEnumerable<string> GetColumnNames()
        {
            return columns.Select(c => c.Name);
        }
        
        /// <summary>
        /// Add a new column value to this element
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddColumn(string name, string value, string sortingvalue = null)
        {
            if (!HasColumn(name))
            {
                columns.Add(new Column(name, value,sortingvalue));           
            }
        }

        /// <summary>
        /// Checks if a column name has also been added. You cannot repeat column names
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasColumn(string name)
        {
            return columns.Contains(new Column(name, null));
        }

        /// <summary>
        /// Queries the value of a particular column
        /// </summary>
        /// <param name="name">name of the colmn</param>
        /// <returns>null if the column is not defined.</returns>
        public string GetColumnValue(string name)
        {
            if (!HasColumn(name)) return null;
            return columns.First(c=> c.Name == name).Value;
        }

        public string GetColumnSortingValue(string name)
        {
            if (!HasColumn(name)) return null;
            return columns.First(c => c.Name == name).SortingValue;
        }
    }
}
