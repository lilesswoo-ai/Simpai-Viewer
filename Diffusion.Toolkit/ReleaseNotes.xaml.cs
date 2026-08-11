using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Diffusion.Common;
using Diffusion.Toolkit.Classes;
using Diffusion.Toolkit.Models;
using Diffusion.Toolkit.Services;

namespace Diffusion.Toolkit
{
    public class ReleaseNotesModel : BaseNotify
    {

        private List<string> _files;
        private int _currentFile;
        private List<FolderChange> _folderChanges = new List<FolderChange>();

        public ICommand Escape { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand NextCommand { get; set; }

        public bool CanPrevious
        {
            get;
            set => SetField(ref field, value);
        }

        public bool CanNext
        {
            get;
            set => SetField(ref field, value);
        }

        public string Markdown
        {
            get;
            set => SetField(ref field, value);
        }

        public Style Style { get; set; }
        public Action Reset { get; set; }

        public ReleaseNotesModel()
        {
            // Load localized release notes: Chinese content for zh-CN (or other
            // non-English) cultures, English content otherwise. The English files
            // are kept untouched and remain available for en-US users.
            var culture = Configuration.Settings.Instance?.Culture ?? "zh-CN";
            var isChinese = culture.StartsWith("zh", StringComparison.OrdinalIgnoreCase);

            var releaseNotes = ResourceHelper.GetResources("Diffusion.Toolkit.Release_Notes");

            _files = releaseNotes
                .Where(SemanticVersion.IsSemanticVersion)
                .Where(f => isChinese ? f.Contains("zh-CN") : !f.Contains("zh-CN"))
                .OrderBy(SemanticVersion.Parse)
                .ToList();

            if (_files.Count == 0)
            {
                // Fall back to whatever is available so the window is never empty.
                _files = releaseNotes.Where(SemanticVersion.IsSemanticVersion).OrderBy(SemanticVersion.Parse).ToList();
            }

            _currentFile = _files.Count - 1;

            void UpdateButtons()
            {
                CanNext = _currentFile < _files.Count - 1;
                CanPrevious = _currentFile > 0;
            }

            PreviousCommand = new RelayCommand<object>((o) =>
            {
                if (_currentFile > 0)
                {
                    _currentFile--;
                    UpdateButtons();
                    LoadFile(_files[_currentFile]);
                    Reset?.Invoke();
                }
            });

            NextCommand = new RelayCommand<object>((o) =>
            {
                if (_currentFile < _files.Count - 1)
                {
                    _currentFile++;
                    UpdateButtons();
                    LoadFile(_files[_currentFile]);
                    Reset?.Invoke();
                }
            });

            Style = MdStyles.CustomStyles.BetterGithub;

            LoadFile(_files[_currentFile]);
            UpdateButtons();
        }

        private void LoadFile(string path)
        {
            Markdown = ResourceHelper.GetString(path);
        }
    }

    public partial class ReleaseNotesWindow : BorderlessWindow
    {
        private readonly ReleaseNotesModel _model = new ReleaseNotesModel();

        public ReleaseNotesWindow()
        {
            InitializeComponent();
            DataContext = _model;

            _model.Reset = Reset;
        }

        private void Reset()
        {
            var scrollViewer = GetScrollViewer(ReleaseNotesViewer) as ScrollViewer;
            scrollViewer.ScrollToTop();
        }



        public static DependencyObject? GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            {
                return o;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }

    }
}
