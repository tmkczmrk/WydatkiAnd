using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace WydatkiAnd.Models
{
    public class Category : IEntity
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; }
        [NotNull]
        public string Name { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Expense> Expenses { get; set; }
        
        public Category(string name)
        {
            Name = name;
        }

        public Category()
        {

        }

        public override string ToString()
        {
            return Name;
        }
    }
}