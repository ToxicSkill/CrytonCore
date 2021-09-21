using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CrytonCore.Model
{
    public class PageManager : NotificationClass, IVisibility, IFileDragDropTarget
    {

        async void IFileDragDropTarget.OnFileDropAsync(string[] filePaths)
        {
            var fileInfos = filePaths.Select(f => new FileInfo(f));
            _ = await LoadFileViaDragDrop(fileInfos.ToList());
        }

        protected virtual async Task<bool> LoadFileViaDragDrop(IEnumerable<FileInfo> fileNames)
        {
            return await Task.Run(() => { return false; });
        }

        protected virtual async Task<bool> LoadFile(IEnumerable<FileInfo> fileNames)
        {
            return await Task.Run(() => { return false; });
        }

        public void ChangeVisibility(bool visibile)
        {
            VisibilityDefaultAsShowed = visibile ? Visibility.Hidden : Visibility.Visible;
            VisibilityDefaultAsHidden = visibile ? Visibility.Visible : Visibility.Hidden;
        }

        private Visibility _visibilityShowed = Visibility.Visible;
        private Visibility _visibilityHidden = Visibility.Hidden;

        public Visibility VisibilityDefaultAsShowed
        {
            get => _visibilityShowed;
            set
            {
                _visibilityShowed = value;
                OnPropertyChanged(nameof(VisibilityDefaultAsShowed));
            }
        }
        public Visibility VisibilityDefaultAsHidden
        {
            get => _visibilityHidden;
            set
            {
                _visibilityHidden = value;
                OnPropertyChanged(nameof(VisibilityDefaultAsHidden));
            }
        }
    }
}
