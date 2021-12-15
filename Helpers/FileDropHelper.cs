using System.Windows;

namespace CrytonCore.Helpers
{
    /// <summary>
    /// IFileDragDropTarget Interface
    /// </summary>
    public interface IFileDragDropTarget
    {
        public void OnFileDropAsync(string[] filePaths);
    }

    /// <summary>
    /// FileDragDropHelper
    /// </summary>
    public class FileDragDropHelper
    {
        public static bool GetIsFileDragDropEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFileDragDropEnabledProperty);
        }

        public static void SetIsFileDragDropEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFileDragDropEnabledProperty, value);
        }

        public static bool GetFileDragDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(FileDragDropTargetProperty);
        }

        public static void SetFileDragDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(FileDragDropTargetProperty, value);
        }

        public static readonly DependencyProperty IsFileDragDropEnabledProperty =
                DependencyProperty.RegisterAttached("IsFileDragDropEnabled", typeof(bool), typeof(FileDragDropHelper), new PropertyMetadata(OnFileDragDropEnabled));

        public static readonly DependencyProperty FileDragDropTargetProperty =
                DependencyProperty.RegisterAttached("FileDragDropTarget", typeof(object), typeof(FileDragDropHelper), null);

        private static void OnFileDragDropEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            var control = d as UIElement;
            if (control != null) control.Drop += OnDrop;
        }

        private static void OnDrop(object _sender, DragEventArgs _dragEventArgs)
        {
            if (!(_sender is DependencyObject d)) return;
            object target = d.GetValue(FileDragDropTargetProperty);
            var fileTarget = target as IFileDragDropTarget;
            if (fileTarget != null)
            {
                if (_dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    fileTarget.OnFileDropAsync((string[])_dragEventArgs.Data.GetData(DataFormats.FileDrop));
                }
            }
            else
            {
                return;
            }
        }
    }
}
