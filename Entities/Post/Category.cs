﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Common;

namespace Entities
{
    public class Category : BaseEntity
	{
		[Required]
		[StringLength(50)]
		public string Name { get; set; }
		public int? ParentCategoryId { get; set; }

		[ForeignKey(nameof(ParentCategoryId))]
		public Category ParentCategory { get; set; }
		public ICollection<Category> ChildCategories { get; set; }
		public ICollection<Post> Posts { get; set; }
	}
}
