using System.ComponentModel.DataAnnotations;

namespace Yooocan.Models.Blog
{
    public class CreatePostModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [MaxLength(160)]
        [Display(Name = "Description for Google (up to 160 characters)")]
        public string Description { get; set; }
        public string Content { get; set; }
        public string HeaderImageUrl { get; set; }
    }
}
