using System;
using System.Linq;
using Diffusion.Toolkit.Services;

namespace Diffusion.Toolkit.Pages
{
    public partial class Search
    {
        /// <summary>
        /// The index of the cursor when the navigation started
        /// </summary>
        private int _startIndex = -1;

        public void StartNavigateCursor()
        {
            if (isPaging) return;
            if (_startIndex == -1 && _model.SelectedImageEntry != null)
            {
                _startIndex = _model.Images.IndexOf(_model.SelectedImageEntry);
            }
        }

        public void EndNavigateCursor()
        {
            if (isPaging) return;
            _startIndex = -1;
        }


        public void Advance()
        {
            StartNavigateCursor();
            NavigateCursorNext();
        }

        private bool isPaging = false;

        public void NavigateCursorNext()
        {
            if (isPaging) return;

            if (_model.Images == null) return;

            int currentIndex = 0;

            var lastIndex = _model.Images.Count - 1;

            var empty = _model.Images.FirstOrDefault(d => d.IsEmpty);

            if (empty != null)
            {
                lastIndex = _model.Images.IndexOf(empty) - 1;
            }

            if (_model.SelectedImageEntry != null)
            {
                currentIndex = _model.Images.IndexOf(_model.SelectedImageEntry);
            }

            if (currentIndex < lastIndex)
            {
                ThumbnailListView.ShowItem(currentIndex + 1);
                _model.SelectedImageEntry = _model.Images[currentIndex + 1];
                ThumbnailListView.ThumbnailListView.SelectedItem = _model.SelectedImageEntry;

                if (!isPaging) PrefetchAdjacent();
            }
            else
            {
                if (_startIndex == lastIndex)
                {
                    isPaging = true;

                    var paged = ThumbnailListView.GoNextPage(() =>
                    {
                        _model.SelectedImageEntry = _model.Images[0];
                        ThumbnailListView.ThumbnailListView.SelectedItem = _model.SelectedImageEntry;
                        NavigationCompleted?.Invoke(this, new EventArgs());

                        _startIndex = 0;
                        isPaging = false;
                    });

                    if (!paged)
                    {
                        isPaging = false;
                    }

                }
            }

        }

        public void NavigateCursorPrevious()
        {
            if (isPaging) return;
            if (_model.Images == null) return;
            int currentIndex = 0;
            if (_model.SelectedImageEntry != null)
            {
                currentIndex = _model.Images.IndexOf(_model.SelectedImageEntry);
            }

            if (currentIndex > 0)
            {
                ThumbnailListView.ShowItem(currentIndex - 1);
                _model.SelectedImageEntry = _model.Images[currentIndex - 1];
                ThumbnailListView.ThumbnailListView.SelectedItem = _model.SelectedImageEntry;

                if (!isPaging) PrefetchAdjacent();
            }
            else
            {
                if (_startIndex == 0)
                {
                    isPaging = true;
                    var paged = ThumbnailListView.GoPrevPage(() =>
                    {
                        var empty = _model.Images.FirstOrDefault(d => d.IsEmpty);
                        var lastIndex = _model.Images.Count - 1;
                        if (empty != null)
                        {
                            lastIndex = _model.Images.IndexOf(empty) - 1;
                        }

                        _startIndex = lastIndex;

                        _model.SelectedImageEntry = _model.Images[lastIndex];
                        ThumbnailListView.ThumbnailListView.SelectedItem = _model.SelectedImageEntry;
                        NavigationCompleted?.Invoke(this, new EventArgs());

                        isPaging = false;

                    }, true);

                    if (!paged)
                    {
                        isPaging = false;
                    }

                }
            }

        }

        /// <summary>
        /// Number of images on each side of the cursor to prefetch.
        /// </summary>
        private const int PrefetchDistance = 3;

        /// <summary>
        /// Queues prefetch thumbnails for the images immediately before and
        /// after the currently selected image (alternating right/left, one
        /// image per side per distance). Called only on cursor navigation
        /// (wheel / arrow keys / auto-advance); page flips are handled by the
        /// paging mechanism and intentionally skipped.
        /// </summary>
        private void PrefetchAdjacent()
        {
            var images = _model.Images;
            var selected = _model.SelectedImageEntry;
            if (images == null || selected == null) return;

            var index = images.IndexOf(selected);
            if (index < 0) return;

            var count = images.Count;
            var service = ServiceLocator.ThumbnailService;

            for (var distance = 1; distance <= PrefetchDistance; distance++)
            {
                var right = index + distance;
                if (right < count && !images[right].IsEmpty)
                {
                    service.QueuePrefetch(images[right]);
                }

                var left = index - distance;
                if (left >= 0 && !images[left].IsEmpty)
                {
                    service.QueuePrefetch(images[left]);
                }
            }
        }

    }
}
