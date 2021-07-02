﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    public class IdeaValueListViewModel : BindableBase
    {
        private ObservableCollection<IdeaValueViewModel> _ideaValues;
        private IdeaElementViewModel _selectedIdeaElement;
        private readonly IIdeaDataService _ideaDataService;
        private readonly IDialogService _dialogService;

        public IdeaValueListViewModel(IdeaElementViewModel ideaElement, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaDataService = ideaDataService;
            _selectedIdeaElement = ideaElement;
            LoadElements();
        }

        public ObservableCollection<IdeaValueViewModel> IdeaValues
        {
            get => _ideaValues;
            private set => SetProperty(ref _ideaValues, value);
        }

        public async Task DeleteIdeaValueAsync(IdeaValueViewModel ideaValue)
        {
            IdeaValues.Remove(ideaValue);
            _selectedIdeaElement.IdeaElement.IdeaValues.Remove(ideaValue.IdeaValue);
            if (_ideaDataService != null)
            {
                await _ideaDataService.DeleteIdeaValueAsync(_selectedIdeaElement.IdeaElement, ideaValue.IdeaValue);
            }
        }

        private void LoadElements()
        {
            IdeaValues = new ObservableCollection<IdeaValueViewModel>();
            List<IdeaValue> ideaValues = _selectedIdeaElement.IdeaElement.IdeaValues;
            //Debug.WriteLine(categories != null);
            if (ideaValues != null) ideaValues.ToList().ForEach(v => IdeaValues.Add(new IdeaValueViewModel(v, _selectedIdeaElement, _dialogService, _ideaDataService)));
        }
    }
}
