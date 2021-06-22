﻿using System.Diagnostics;
using DicePage.ViewModels;
using Dicidea.Core.Constants;
using Dicidea.Core.Services;
using OverviewPage.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace OverviewPage
{
    public class OverviewModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;

        public OverviewModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager = regionManager;

            DiceDataService = new DiceDataServiceJson();
            RollEmSpaceDataService = new RollEmSpaceDataServiceJson(DiceDataService);

            DiceListViewModel = new DiceListViewModel(DiceDataService);

            _parameters = new NavigationParameters();
            _parameters.Add("DiceListViewModel", DiceListViewModel);

            Debug.WriteLine("DiceListViewModel Parameter in ShellViewModel isn't null: " + (_parameters["DiceListViewModel"] != null));
            
        }
        public IDiceDataService DiceDataService { get; }

        public DiceListViewModel DiceListViewModel { get; }
        public IRollEmSpaceDataService RollEmSpaceDataService { get; }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(Views.Overview), _parameters);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.Overview>();
            containerRegistry.RegisterForNavigation<Views.MainNavigation>();
        }
    }
}