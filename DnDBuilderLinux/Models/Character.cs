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
        private bool _caster;
        private int _hitpoints;
        private int _abilityScore;

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
                else throw new ArgumentException("Biography must be less than 500 characters");
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
            get => _caster;
            set => _caster = value;
        }

        public int Hitpoints
        {
            get => _hitpoints;
            set => _hitpoints = value;
        }

        public int AbilityScore
        {
            get => _abilityScore;
            set => _abilityScore = value;
        }
    }
}