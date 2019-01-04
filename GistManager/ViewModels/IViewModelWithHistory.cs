using GistManager.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.ViewModels
{
    public interface IViewModelWithHistory
    {
        ObservableRangeCollection<GistHistoryEntryViewModel> History { get; }
        Task OnHistoryCheckoutAsync(GistHistoryEntryViewModel historyItem);
    }
}
