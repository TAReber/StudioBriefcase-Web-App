using System.ComponentModel.DataAnnotations;

namespace StudioBriefcase.Models
{
    public class UserModel
    {
        [Key]
        public uint Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string profile_url { get; set; } = string.Empty;
        public string avatar_url { get; set; } = string.Empty;
        public uint role { get; set; } = 2;
    }
}
