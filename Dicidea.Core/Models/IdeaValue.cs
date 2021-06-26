﻿using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Dicidea.Core.Models
{
    public class IdeaValue : NotifyDataErrorInfo<IdeaValue>
    {
        private string _name;
        public IdeaValue(string name)
        {
            Id = Guid.NewGuid().ToString("N");
            Rules.Add(new DelegateRule<IdeaValue>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
            Name = name;
        }

        public IdeaValue()
        {
            Rules.Add(new DelegateRule<IdeaValue>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
        }

        [JsonProperty(PropertyName = "IdeaValueId", Required = Required.Always)]
        public string Id
        {
            get;
            set;
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string PictureUri
        {
            get;
            set;
        }
    }
}
