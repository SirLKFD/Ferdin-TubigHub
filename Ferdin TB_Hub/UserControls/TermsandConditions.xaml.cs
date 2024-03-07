using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Ferdin_TB_Hub.UserControls
{
    public sealed partial class TermsandConditions : UserControl
    {

        public TermsandConditions()
        {
            this.InitializeComponent();
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
