using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System;

namespace GistManager.Controls.EditableTextBlock
{
    // https://www.codeproject.com/articles/72544/editable-text-block-in-wpf
    public class EditableTextBlockAdorner : Adorner
    {
        private readonly VisualCollection collection;
        private readonly TextBox textBox;
        private readonly TextBlock textBlock;

        internal SaveMode SaveMode { get; private set; }

        public event RoutedEventHandler TextBoxLostFocus
        {
            add => textBox.LostFocus += value;
            remove => textBox.LostFocus -= value;
        }
        public event KeyEventHandler TextBoxKeyUp
        {
            add => textBox.KeyUp += value;
            remove => textBox.KeyUp -= value;
        }
        public EditableTextBlockAdorner(EditableTextBlock adornedElement) : base(adornedElement)
        {
            textBlock = adornedElement;
            textBox = new TextBox { AcceptsReturn = true, MaxLength = adornedElement.MaxLength };
            textBox.SetBinding(TextBox.TextProperty, new Binding(nameof(TextBox.Text)) { Source = adornedElement, UpdateSourceTrigger = UpdateSourceTrigger.Explicit });
            textBox.KeyUp += textBox_KeyUp;
            textBox.LostFocus += textBox_LostFocus;
            collection = new VisualCollection(this) { textBox };
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e) =>  textBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();        

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    SaveMode = SaveMode.DataError;
                    InvalidateVisual();
                    return;
                }

                textBox.Text = textBox.Text.Replace(Environment.NewLine, string.Empty);
                var expression = textBox.GetBindingExpression(TextBox.TextProperty);
                expression?.UpdateSource();
                SaveMode = SaveMode.SaveEnabled;
            }
            if (e.Key == Key.Escape)
            {
                SaveMode = SaveMode.SaveCancelled;
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }
        }
        private Rect GetAdornerBox() => new Rect(0, 0, textBlock.DesiredSize.Width + 20, textBlock.DesiredSize.Height);


        protected override Visual GetVisualChild(int index) => collection[index];
        protected override int VisualChildrenCount => collection.Count;
        protected override Size ArrangeOverride(Size finalSize)
        {
            textBox.Arrange(GetAdornerBox());
            textBox.Focus();
            return finalSize;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.SaveMode == SaveMode.DataError)
            {
                drawingContext.DrawRectangle(null, new Pen { Brush = Brushes.Red, Thickness = 2.5, }, GetAdornerBox());
            }
            base.OnRender(drawingContext);
        }

        internal void FocusAndSelectAll()
        {
            textBox.Focus();
            textBox.SelectAll();
        }

    }
}
