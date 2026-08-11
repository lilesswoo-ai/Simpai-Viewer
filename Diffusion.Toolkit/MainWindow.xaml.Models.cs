using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Diffusion.Toolkit.Models;
using Diffusion.Toolkit.Services;

namespace Diffusion.Toolkit
{
    public partial class MainWindow
    {
        public void LoadImageModels()
        {
            var existingModels = _model.ImageModels == null ? Enumerable.Empty<ModelViewModel>() : _model.ImageModels.ToList();

            var imageModels = _dataStore.GetImageModels();

            _model.ImageModels = imageModels.Select(m => new ModelViewModel()
            {
                IsTicked = existingModels.FirstOrDefault(d => d.Name == m.Name || d.Hash == m.Hash)?.IsTicked ?? false,
                Name = m.Name ?? ResolveModelName(m.Hash),
                Hash = m.Hash,
                ImageCount = m.ImageCount
            }).Where(m => !string.IsNullOrEmpty(m.Name) && !string.IsNullOrEmpty(m.Hash)).OrderBy(x => x.Name).ToList();

            foreach (var model in _model.ImageModels)
            {
                model.PropertyChanged += ImageModelOnPropertyChanged;
            }

            _model.ImageModelNames = imageModels.Where(m => !string.IsNullOrEmpty(m.Name)).Select(m => m.Name).OrderBy(x => x);

        }

        private void ImageModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ModelViewModel.IsTicked))
            {
                var selectedModels = _model.ImageModels.Where(d => d.IsTicked).ToList();
                _model.SelectedModelsCount = selectedModels.Count;
                _model.HasSelectedModels = selectedModels.Any();
                _search.SearchImages();
            }
        }

        private string ResolveModelName(string hash)
        {
            var matches = _allModels.Where(m =>
                !string.IsNullOrEmpty(hash) &&
                (String.Equals(m.Hash, hash, StringComparison.CurrentCultureIgnoreCase)
                 ||
                 (m.SHA256 != null && string.Equals(m.SHA256.Substring(0, hash.Length), hash, StringComparison.CurrentCultureIgnoreCase))
                )).ToList();

            if (matches.Any())
            {
                return matches[0].Filename;
            }
            else
            {
                return hash;
            }
        }

    }
}
