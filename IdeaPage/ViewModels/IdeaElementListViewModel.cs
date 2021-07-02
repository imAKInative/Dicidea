﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    public class IdeaElementListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<IdeaElementViewModel> _ideaElements;
        private IdeaCategoryViewModel _selectedIdeaCategory;
        private readonly IIdeaDataService _ideaDataService;
        private readonly IDialogService _dialogService;

        public IdeaElementListViewModel(IdeaCategoryViewModel ideaCategory, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaDataService = ideaDataService;
            _selectedIdeaCategory = ideaCategory;
            LoadIdeaElements();
        }

        public ObservableCollection<IdeaElementViewModel> IdeaElements
        {
            get => _ideaElements;
            private set => SetProperty(ref _ideaElements, value);
        }

        public async Task DeleteIdeaElementAsync(IdeaElementViewModel ideaElement)
        {
            IdeaElements.Remove(ideaElement);
            _selectedIdeaCategory.IdeaCategory.IdeaElements.Remove(ideaElement.IdeaElement);
            if (_ideaDataService != null)
            {
                await _ideaDataService.DeleteIdeaElementAsync(_selectedIdeaCategory.IdeaCategory, ideaElement.IdeaElement);
            }
        }

        private void LoadIdeaElements()
        {
            IdeaElements = new ObservableCollection<IdeaElementViewModel>();
            List<IdeaElement> ideaElements = _selectedIdeaCategory.IdeaCategory.IdeaElements;
            //Debug.WriteLine(categories != null);
            ideaElements?.ToList().ForEach(e => IdeaElements.Add(new IdeaElementViewModel(e, _selectedIdeaCategory, _ideaDataService, _dialogService)));
        }

    }
}
