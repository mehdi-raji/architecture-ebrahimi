using Entities;
using System.ComponentModel.DataAnnotations;

namespace Architecture.Models
{
	public class UserDto
	{
		[Required]
		[StringLength(100)]
		public string UserName { get; set; }

		[Required]
		[StringLength(500)]
		public string Password { get; set; }

		[Required]
		[StringLength(100)]
		public string FullName { get; set; }

		public int Age { get; set; }

		public GenderType Gender { get; set; }

	}
}
