using System.Windows;
using System.Windows.Input;
using Diffusion.Toolkit.Classes;
using Diffusion.Toolkit.Configuration;
using Diffusion.Toolkit.MdStyles;
using Diffusion.Toolkit.Models;

namespace Diffusion.Toolkit
{
    public class TipsModel : BaseNotify
    {
        public string Markdown
        {
            get;
            set => SetField(ref field, value);
        }

        public Style Style
        {
            get;
            set => SetField(ref field, value);
        }

        public ICommand Escape
        {
            get;
            set => SetField(ref field, value);
        }
    }


    /// <summary>
    /// Interaction logic for Tips.xaml
    /// </summary>
    public partial class TipsWindow : Window
    {

        public TipsWindow()
        {
            InitializeComponent();

            // Load the localized "Getting Started" content: Chinese for zh-CN
            // (or other non-English) cultures, English otherwise.
            var culture = Settings.Instance?.Culture ?? "zh-CN";
            var tipsResource = culture.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase)
                ? "Diffusion.Toolkit.Tips.zh-CN.md"
                : "Diffusion.Toolkit.Tips.md";

            var tips = new TipsModel
            {
                Markdown = ResourceHelper.GetString(tipsResource),
                Style = CustomStyles.BetterGithub,
                Escape = new RelayCommand<object>(o => Close())
            };

            //Markdown engine = new Markdown();
            //engine.DocumentStyle = CustomStyles.BetterGithub;
            //FlowDocument document = engine.Transform(markdown);
            //RichTextBox.Document = document;
            DataContext = tips;
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "https://github.com/RupertAvery/DiffusionToolkit/blob/master/Diffusion.Toolkit/Tips.md");
        }
    }
}
