﻿using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    /// <summary>
    /// Model Klasse eines Würfels
    /// </summary>
    public sealed class Dice : NotifyDataErrorInfo<Dice>
    {
        private string _name;
        private string _description;
        private DateTime _lastUsed;
        private int _amount;
        private bool _active;

        public Dice(bool newDice)
        {
            Categories = new List<Category>
            {
                new Category(newDice)
            };
            Rules.Add(new DelegateRule<Dice>(nameof(Name), "The dice has to have a name.", d => !string.IsNullOrWhiteSpace(d?.Name)));
            Id = Guid.NewGuid().ToString("N");
            Name = "";
            Description = "";
            Amount = 1;
            Active = true;
            LastUsed = DateTime.Now;
        }
        public Dice()
        {
            Rules.Add(new DelegateRule<Dice>(nameof(Name), "The dice has to have a name.", d => !string.IsNullOrWhiteSpace(d?.Name)));
        }

        [JsonProperty(PropertyName = "DiceId", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Name
        {
            get => _name; 
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description; 
            set => SetProperty(ref _description, value);
        }

        public DateTime LastUsed
        {
            get => _lastUsed;
            set => SetProperty(ref _lastUsed, value);
        }
        

        public List<Category> Categories { get; set; }
        
        public int Amount { 
            get => _amount; 
            set => SetProperty(ref _amount, value);
        }

        public bool Active
        {
            get => _active; 
            set => SetProperty(ref _active, value);
        }
    }
}
