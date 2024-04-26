using System;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Ferdin_TB_Hub.UserControls
{
    public sealed partial class TermsandConditions : UserControl
    {

        public TermsandConditions()
        {
            InitializeComponent();
            LoadTermsAndConditionsTextFile();
        }

        private async void LoadTermsAndConditionsTextFile()
        {
            try
            {
                StorageFile file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("TermsAndConditions.txt");

                string fileContent = await FileIO.ReadTextAsync(file);

                DisplayTextInRichTextBlock(fileContent);
            }
            catch (Exception ex)
            {
                DisplayTextInRichTextBlock("Error loading file: " + ex.Message);
            }
        }


        private void DisplayTextInRichTextBlock(string text)
        {
            TermsAndConditions.Blocks.Clear();

            // Create a paragraph and set its text
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run { Text = text });

            // Add the paragraph to the RichTextBlock
            TermsAndConditions.Blocks.Add(paragraph);
        }


    }
}
