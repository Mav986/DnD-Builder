using System;

namespace DnDBuilderLinux.Models
{
    public class Character
    {
        private string _name;
        private int _age;
        private string _gender;
        private string _biography;
        private int _level;
        private string _race;
        private string _class;
        private int _hp;

        private int _con;
        private int _dex;
        private int _str;
        private int _cha;
        private int _intel;
        private int _wis;

        public string Name
        {
            get => _name;
            set
            {
                if (value.Length > 0) _name = value;
                else throw new ArgumentException("Name cannot be empty");
            }
        }

        public int Age
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

        public int Level
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
        }

        public int Hitpoints
        {
            get => _hp;
            set => _hp = value;
        }

        public int Con
        {
            get => _con;
            set => _con = value;
        }

        public int Dex
        {
            get => _dex;
            set => _dex = value;
        }

        public int Str
        {
            get => _str;
            set => _str = value;
        }

        public int Cha
        {
            get => _cha;
            set => _cha = value;
        }

        public int Intel
        {
            get => _intel;
            set => _intel = value;
        }

        public int Wis
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