using GistManager.GistService.Model;
using GistManager.Mvvm;
using GistManager.Mvvm.Commands.Async.AsyncCommand;
using System;
using System.Threading.Tasks;

namespace GistManager.ViewModels
{
    public class GistHistoryEntryViewModel : BindableBase
    {
        public GistHistoryEntryModel HistoryEntry { get; }
        private readonly IViewModelWithHistory owner;
        public GistHistoryEntryViewModel(GistHistoryEntryModel historyEntry, IViewModelWithHistory owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            this.HistoryEntry = historyEntry ?? throw new ArgumentNullException(nameof(historyEntry));
        }

        #region bound properties
        private bool isCheckedOut;
        public bool IsCheckedOut
        {
            get => isCheckedOut;
            set
            {
                SetProperty(ref isCheckedOut, value);
                if (value)
                {
                    owner.OnHistoryCheckoutAsync(this);
                }
            }
        }
        public string Url => HistoryEntry.Url;
        public DateTime Committed => HistoryEntry.CommittedAt.DateTime;
        public string Version => HistoryEntry.Version;
        #endregion        
    }
}
