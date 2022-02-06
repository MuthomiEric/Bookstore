using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime YearOfPublication { get; set; }
        public virtual Guid AuthorId { get; set; }
        public virtual Author Author { get; set; }
    }
}
