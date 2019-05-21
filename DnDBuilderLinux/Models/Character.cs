using System;
using System.ComponentModel.DataAnnotations;

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
        private long _hp;

        private long _con;
        private long _dex;
        private long _str;
        private long _cha;
        private long _intel;
        private long _wis;

        [Required]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public long Age
        {
            get => _age;
            set
            {
                if (value >= 0 && value <= 500) _age = value;
                else throw new ArgumentException("Age must be between 0 and 500");
            }
        }

        public string Gender
        {
            get => _gender; 
            set => _gender = value;
        }

        public string Biography
        {
            get => _biography;
            set
            {
                if (value.Length <= 500) _biography = value;
                else throw new ArgumentException("Biography must not be more than 500 characters");
            }
        }

        public long Level
        {
            get => _level;
            set
            {
                if (value >= 1 && value <= 20) _level = value;
                else throw new ArgumentException("Level must be between 1 and 20");
            }
        }

        public string Race
        {
            get => _race;
            set => _race = value; // Race will be a preset string from a dropdown box
        }

        public string Class
        {
            get => _class;
            set => _class = value; // Class will be a preset string from a dropdown box
        }

        public bool Caster
        {
            get;
            set;
        }

        public long Hitpoints
        {
            get => _hp;
            set => _hp = value;
        }

        public long Con
        {
            get => _con;
            set => _con = value;
        }

        public long Dex
        {
            get => _dex;
            set => _dex = value;
        }

        public long Str
        {
            get => _str;
            set => _str = value;
        }

        public long Cha
        {
            get => _cha;
            set => _cha = value;
        }

        public long Intel
        {
            get => _intel;
            set => _intel = value;
        }

        public long Wis
        {
            get => _wis;
            set => _wis = value;
        }

        public override string ToString()
        {
            return Name + "\n" + Age + "\n" + Gender + "\n"  + Biography + "\n" + Level +
                   "\n" + Race + "\n" + Class + "\n" + Caster + "\n" + Hitpoints + "\n" +
                   + Con + "\n" + Dex + "\n" + Str + "\n" + Cha + "\n" + Intel + "\n" + Wis;
        }
    }
}