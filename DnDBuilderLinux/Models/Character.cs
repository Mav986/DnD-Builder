using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DnDBuilderLinux.Models
{
    public class Character
    {
        private string _name;
        private string _gender;
        private string _biography;
        private string _race;
        private string _class;

        [Required]
        [JsonProperty("name")]
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "" : value;
        }

        [Required]
        [Range(0, 500, ErrorMessage = "{0} must be between {1} and {2}")]
        [JsonProperty("age")]
        public long Age { get; set; }

        [Required]
        [JsonProperty("gender")]
        public string Gender
        {
            get => string.IsNullOrWhiteSpace(_gender) ? "" : _gender; 
            set => _gender = value;
        }

        [MaxLength(500, ErrorMessage = "Biography must not exceed 500 characters")]
        [JsonProperty("bio")]
        public string Biography
        {
            get => string.IsNullOrWhiteSpace(_biography) ? "" : _biography;
            set => _biography = value;
        }

        [Required]
        [Range(1, 20, ErrorMessage = "Level must be between 1 and 20")]
        [JsonProperty("level")]
        public long Level { get; set; }

        [Required]
        [JsonProperty("race")]
        public string Race
        {
            get => _race;
            set => _race = string.IsNullOrWhiteSpace(value) ? "" : value; // Race will be a preset string from a dropdown box
        }

        [Required]
        [JsonProperty("class")]
        public string Class
        {
            get => _class;
            set => _class = string.IsNullOrWhiteSpace(value) ? "" : value; // Class will be a preset string from a dropdown box
        }

        [Required]
        [JsonProperty("con")]
        public long Con { get; set; }

        [Required]
        [JsonProperty("dex")]
        public long Dex { get; set; }

        [Required]
        [JsonProperty("str")]
        public long Str { get; set; }

        [Required]
        [JsonProperty("cha")]
        public long Cha { get; set; }

        [Required]
        [JsonProperty("intel")]
        public long Intel { get; set; }

        [Required]
        [JsonProperty("wis")]
        public long Wis { get; set; }
    }
}