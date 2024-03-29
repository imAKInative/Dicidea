﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using Prism.Services.Dialogs;

namespace Dicidea.ViewModels
{
    /// <summary>
    ///     ViewModel für den ErrorDialog.
    /// </summary>
    public class ErrorDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand<string> _okDialogCommand;
        public DelegateCommand<string> OkDialogCommand =>
            _okDialogCommand ?? (_okDialogCommand = new DelegateCommand<string>(OkDialog));

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _title = "Title";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// Setzt das Result des klickens auf den Button
        /// </summary>
        /// <param name="parameter"></param>
        protected virtual void OkDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
            {
                result = ButtonResult.OK;
            }
            else if (parameter?.ToLower() == "false")
            {
                result = ButtonResult.Cancel;
            }
            Debug.WriteLine("Buttonresult: " + result);
            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        /// <summary>
        /// Setzt beim Öffnen des Dialogs den Titel und die Nachricht
        /// </summary>
        /// <param name="parameters">Liste mit den Paramtern für den Titel und die Nachricht</param>
        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
            Title = parameters.GetValue<string>("title");
        }
    }
}
