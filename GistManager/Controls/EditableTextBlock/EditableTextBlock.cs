using GistManager.Utils;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GistManager.Controls.EditableTextBlock
{
    // https://www.codeproject.com/articles/72544/editable-text-block-in-wpf
    public class EditableTextBlock : TextBlock
    {
        public EditableTextBlock()
        {
            if (SystemConfiguraiton.DarkModeSelected())
            {
                this.Background = new SolidColorBrush(Color.FromArgb(00, 32, 32, 32));
                this.Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            }
        }

        private EditableTextBlockAdorner adorner;

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(EditableTextBlock), new UIPropertyMetadata(0));
        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(EditableTextBlock), new UIPropertyMetadata(false, IsInEditModeUpdate));
        public bool IsInEditMode
        {
            get => (bool)GetValue(IsInEditModeProperty);
            set => SetValue(IsInEditModeProperty, value);
        }
        private static void IsInEditModeUpdate(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is EditableTextBlock textBlock))
                return;

            var layer = AdornerLayer.GetAdornerLayer(textBlock);
            if (layer == null)
                return;

            //If the IsInEditMode set to true means the user has enabled the edit mode then
            //add the adorner to the adorner layer of the TextBlock.
            if (textBlock.IsInEditMode)
            {
                if (null == textBlock.adorner)
                {
                    textBlock.adorner = new EditableTextBlockAdorner(textBlock);

                    //Events wired to exit edit mode when the user presses Enter key or leaves the control.
                    textBlock.adorner.TextBoxKeyUp += textBlock.TextBoxKeyUp;
                    textBlock.adorner.TextBoxLostFocus += textBlock.TextBoxLostFocus;
                }
                textBlock.adorner.FocusAndSelectAll();
                layer.Add(textBlock.adorner);
            }
            else
            {
                //Remove the adorner from the adorner layer.
                var adorners = layer.GetAdorners(textBlock);
                var textBlockAdorner = adorners.OfType<EditableTextBlockAdorner>().Single();
                layer.Remove(textBlockAdorner);
                if (textBlockAdorner.SaveMode == SaveMode.SaveEnabled)
                {
                    var expression = textBlock.GetBindingExpression(TextProperty);
                    expression?.UpdateTarget();
                }
            }
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e) => IsInEditMode = false;

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var layer = AdornerLayer.GetAdornerLayer(this);
                var adorners = layer.GetAdorners(this);
                var textBlockAdorner = adorners.OfType<EditableTextBlockAdorner>().Single();
                if (textBlockAdorner.SaveMode == SaveMode.DataError)
                {
                    return;
                }
                IsInEditMode = false;
            }
            if (e.Key == Key.Escape)
            {
                IsInEditMode = false;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                var dave = Helpers.FindParentOfType<GistManagerWindowControl>(this);

            }


            if (e.ClickCount == 2)
            {
                IsInEditMode = true;
            }
            base.OnMouseDown(e);
        }
    }
}