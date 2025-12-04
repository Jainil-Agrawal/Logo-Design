using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Logo
{
    public partial class MainWindow : Window
    {
        private void LogoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is LogoItem added)
                {
                    HandleAddSelection(added.ImageControl);
                }
            }

            if (e.RemovedItems.Count > 0)
            {
                if (e.RemovedItems[0] is LogoItem removed)
                {
                    HandleRemoveSelection(removed.ImageControl);
                }
            }
            UpdateSelectionMode();
            UpdateSelectedImageUI(selectImage);
        }

        private void HandleAddSelection(Image addedImage)
        {
            if (rotateTimer != null)
            {

                if (rotateImages.Count == 0)
                {
                    if (selectImage != null)
                    {
                        rotateImages.Add(selectImage);
                    }
                }

                var canvasImg = GetCanvasImage(addedImage);

                if (!rotateImages.Contains(canvasImg))
                    rotateImages.Add(canvasImg);

                selectImage = canvasImg;
            }
            else
            {
                var canvasImg = GetCanvasImage(addedImage);
                selectImage = canvasImg;
                rotateImages = [];

            }
            addedImage.MouseLeftButtonDown += Logo_MouseDown;
            addedImage.MouseMove += Logo_MouseMove;
            addedImage.MouseLeftButtonUp += Logo_MouseUp;
            speed.Text = rotateSpeed.ToString();
        }

        private void HandleRemoveSelection(Image removedImage)
        {
            if (rotateTimer != null && rotateImages.Count != 0)
            {
                var canvasImg = GetCanvasImage(removedImage);
                rotateImages.Remove(canvasImg);

                if (rotateImages.Count == 0)
                {
                    selectImage = null;
                    StopRotation();

                    return;
                }
                if (rotateImages.Count != 0)
                {
                    selectImage = rotateImages.Last();
                }
            }
        }

        private void UpdateSelectedImageUI(Image image)
        {
            if (image == null) return;
            posX.Text = Canvas.GetLeft(image).ToString();
            posY.Text = Canvas.GetTop(image).ToString();
            img.Text = image.Height.ToString();

            if (image.RenderTransform is RotateTransform rt)
                angle.Text = rt.Angle.ToString();
        }

        private void UpdateSelectionMode()
        {
            var last = LogoList.SelectedItem;

            if (rotateTimer == null || !rotateTimer.IsEnabled)
            {
                LogoList.SelectionMode = SelectionMode.Single;
            }
            else
            {
                LogoList.SelectionMode = SelectionMode.Multiple;
                if (last != null && !LogoList.SelectedItems.Contains(last))
                {
                    LogoList.SelectedItems.Add(last);
                }
            }
        }

        private void TextBoxList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = TextBoxList.SelectedItem;
            if (selected == null) return;

            int id = (int)selected.GetType().GetProperty("Id").GetValue(selected, null);

            Grid? container = DesignCanvas.Children
                .OfType<Grid>()
                .FirstOrDefault(t => (int)t.Tag == id);
            if (container == null)
                return;
            TextBox? tb = null;
            selectContainer = container;
            
            foreach (var child in container.Children)
            {
                if (child is TextBox)
                {
                    tb = child as TextBox;
                    break;
                }
            }
            tb.CaretIndex = tb.Text.Length;
            tb.Focus();
            txt.Text = selectContainer?.Height.ToString();
            txtW.Text = selectContainer?.Width.ToString();
        }
    }
}
