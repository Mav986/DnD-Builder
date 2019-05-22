using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DnDBuilderLinux.Models
{
    public class Character
    {
        private string _name;
        private long _age;
        private string _gender;
        private string _biography;
        private long _level;
        private string _race;
        private string _class;

        private long _con;
        private long _dex;
        private long _str;
        private long _cha;
        private long _intel;
        private long _wis;

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
        public long Age
        {
            get => _age;
            set => _age = value;
        }

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
        public long Level
        {
            get => _level;
            set => _level = value;
        }

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
        public long Con
        {
            get => _con;
            set => _con = value;
        }

        [Required]
        [JsonProperty("dex")]
        public long Dex
        {
            get => _dex;
            set => _dex = value;
        }

        [Required]
        [JsonProperty("str")]
        public long Str
        {
            get => _str;
            set => _str = value;
        }

        [Required]
        [JsonProperty("cha")]
        public long Cha
        {
            get => _cha;
            set => _cha = value;
        }

        [Required]
        [JsonProperty("intel")]
        public long Intel
        {
            get => _intel;
            set => _intel = value;
        }

        [Required]
        [JsonProperty("wis")]
        public long Wis
        {
            get => _wis;
            set => _wis = value;
        }
    }
}