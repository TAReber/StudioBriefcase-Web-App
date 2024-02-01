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
        public uint userclass { get; set; } = 1;
        public uint userprivilege { get; set; } = 1;
    }

    public class UserRoleUpdateModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;
        [Required]
        public string userclass { get; set; } = string.Empty;
    }

    public class DonationModel
    {
        public string Name { get; set; } = string.Empty;
        public int amount { get; set; }

        public DonationModel(string name, int amount)
        {
            Name = name;
            this.amount = amount;
        }   
    }
}
