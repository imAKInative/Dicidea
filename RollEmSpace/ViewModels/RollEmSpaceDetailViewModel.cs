﻿using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DicePage.ViewModels;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using IdeaPage.ViewModels;
using Prism.Regions;
using Prism.Services.Dialogs;
using RollEmSpacePage.Views;

namespace RollEmSpacePage.ViewModels
{
    public class RollEmSpaceDetailViewModel : NotifyPropertyChanges, INavigationAware
    {
        private IdeaListViewModel _ideaListViewModel;
        private IdeaListViewModel _mainIdeaListViewModel;
        //private readonly IDialogCoordinator _dialogCoordinator;
        private ListCollectionView _groupedIdeasView;
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        private DiceViewModel _selectedDice;
        private DiceListViewModel _diceListViewModel;
        private readonly IDialogService _dialogService;
        private readonly Random _random = new Random();
        private bool _showSaved = false;
        private bool _isSaving = false;
        private bool _showRolling = false;

        public RollEmSpaceDetailViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            GoToDiceCommand = new DelegateCommand<object>(GoToDice);
            GoToRollEmSpaceOverviewCommand = new DelegateCommand<object>(GoToRollEmSpaceOverview);
            RollCommand = new DelegateCommand(RollExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            EditCommand = new DelegateCommand(EditExecute);
            SaveCommand = new DelegateCommand(SaveExecute);
        }
        public ICommand GoToDiceCommand { get; private set; }
        public ICommand GoToRollEmSpaceOverviewCommand { get; private set; }

        public bool ShowSaved
        {
            get => _showSaved;
            set => SetProperty(ref _showSaved, value);
        }
        public bool ShowRolling
        {
            get => _showRolling;
            set => SetProperty(ref _showRolling, value);
        }
        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        private void GoToDice(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(DiceDetail), _parameters);
        }
        private void GoToRollEmSpaceOverview(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(RollEmSpaceOverview), _parameters);
            _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
        }

        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand RollCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        private async void RollExecute()
        {
            ShowRolling = true;
            await Task.Delay(10);
            await rollIdeas();
            ShowRolling = false;
        }

        private async Task rollIdeas()
        {
            List<Idea> rolledIdeas = new List<Idea>();
            for (int j = 0; j < SelectedDice.Dice.Amount; j++)
            {
                Idea idea = new Idea($"{j + 1}. {SelectedDice.Dice.Name} idea", SelectedDice.Dice.Name, SelectedDice.Dice.Description);
                rolledIdeas.Add(idea);
                List<Category> Categories = SelectedDice.Dice.Categories;
                for (int i = 0; i < Categories.Count; i++)
                {
                    if (Categories[i].Active)
                    {
                        for (int l = 0; l < Categories[i].Amount; l++)
                        {
                            IdeaCategory ideaCategory = new IdeaCategory(Categories[i].Name);
                            idea.IdeaCategories.Add(ideaCategory);
                            List<Element> Elements = Categories[i].Elements;
                            for (int a = 0; a < Elements.Count; a++)
                            {
                                if (Elements[a].Active)
                                {

                                    for (int m = 0; m < Elements[a].Amount; m++)
                                    {
                                        IdeaElement ideaElement = new IdeaElement(Elements[a].Name);
                                        ideaCategory.IdeaElements.Add(ideaElement);
                                        List<Value> Values = Elements[a].Values;
                                        for (int k = 0; k < Elements[a].ValueAmount; k++)
                                        {
                                            bool valid = false;
                                            Value value = new Value(true);
                                            do
                                            {
                                                int index = _random.Next(Values.Count);
                                                value = Values[index];
                                                if (value.Active)
                                                {
                                                    valid = true;
                                                }
                                            } while (!valid);
                                            ideaElement.IdeaValues.Add(new IdeaValue(value.Name));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            _ideaListViewModel = new IdeaListViewModel(rolledIdeas, _dialogService);//, _ideaDataService
            CreateGroupedView();
            SelectedDice.Dice.LastUsed = DateTime.Now;
            await _diceListViewModel.SaveDiceAsync();
        }
        private void EditExecute()
        {
            //Task.Run(SelectedDice.EditCategoryAsync);
        }

        public IRegionManager RegionManager { get; private set; }
        public ListCollectionView GroupedIdeasView
        {
            get => _groupedIdeasView;
            set => SetProperty(ref _groupedIdeasView, value);
        }

        public bool IsEditEnabled => GroupedIdeasView.CurrentItem != null;

        public IdeaViewModel SelectedIdea
        {
            get
            {
                if (GroupedIdeasView != null) return GroupedIdeasView.CurrentItem as IdeaViewModel;
                return null;
            }
            set => GroupedIdeasView.MoveCurrentTo(value);
        }
        public DiceViewModel SelectedDice
        {
            get => _selectedDice;
            
            set => SetProperty(ref _selectedDice, value);
        }

        private async void DeleteExecute()
        {
            bool delete = false;
            var selectedIdea = SelectedIdea;
            if (selectedIdea == null) return;

            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete idea?" },
                    { "message", $"Do you really want to delete '{selectedIdea.Idea.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if(delete) await _ideaListViewModel.DeleteIdeaAsync(selectedIdea);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Dice.Name))
            {
                GroupedIdeasView.Refresh();
            }
        }

        private async void SaveExecute()
        {
            IsSaving = true;
            await Task.Delay(3000);
            await _mainIdeaListViewModel.AddIdeasAsync(_ideaListViewModel.Ideas);
            await _mainIdeaListViewModel.SaveIdeasAsync();
            IsSaving = false;
            ShowSaved = true;
            await Task.Delay(3000);
            ShowSaved = false;
        }

        private void CreateGroupedView()
        {
            ObservableCollection<IdeaViewModel> ideaViewModels = _ideaListViewModel.AllIdeas;
            foreach (var ideaViewModel in ideaViewModels)
            {
                ideaViewModel.Idea.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "Idea.Name";
            GroupedIdeasView = new ListCollectionView(ideaViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            GroupedIdeasView.GroupDescriptions.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName,
                Converter = new NameToInitialConverter()
            });
            GroupedIdeasView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdea));
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Debug.WriteLine("Navigated to Detail Dice");
            if (navigationContext != null)
            {
                _parameters = navigationContext.Parameters;
                //navigationContext.Parameters;
                if (navigationContext.Parameters["diceListViewModel"] != null)
                {
                    _diceListViewModel = navigationContext.Parameters.GetValue<DiceListViewModel>("diceListViewModel");
                    _ideaListViewModel = new IdeaListViewModel(new List<Idea>(), _dialogService);
                    _mainIdeaListViewModel = navigationContext.Parameters.GetValue<IdeaListViewModel>("ideaListViewModel");
                    CreateGroupedView();
                    if (navigationContext.Parameters["selectedDice"] != null)
                    {
                        Debug.WriteLine("Selected dice is not null");
                        Debug.WriteLine("Selected Dice: " + navigationContext.Parameters.GetValue<DiceViewModel>("selectedDice").Dice.Name);
                        SelectedDice = navigationContext.Parameters.GetValue<DiceViewModel>("selectedDice");
                    }
                }

                if (navigationContext.Parameters["regionManager"] != null)
                {
                    RegionManager = navigationContext.Parameters["regionManager"] as IRegionManager;
                }
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Debug.WriteLine("Not implemented, navigated from DiceDetail to some other side");
        }
    }
}
