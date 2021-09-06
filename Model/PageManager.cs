﻿using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace CrytonCore.Model
{
    public class PageManager : NotificationClass, IVisibility, IFileDragDropTarget
    {

        async void IFileDragDropTarget.OnFileDropAsync(string[] filePaths)
        {
            _ = await LoadFile(filePaths);
        }

        protected virtual async Task<bool> LoadFile(IEnumerable<string> fileNames)
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
