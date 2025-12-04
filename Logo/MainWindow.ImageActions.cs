using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Logo
{
    public partial class MainWindow : Window
    {
        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selectedImage = sender as Image;
            if (selectedImage == null) return;

            clickPosition = e.GetPosition(DesignCanvas);

            double left = Canvas.GetLeft(selectedImage);
            double top = Canvas.GetTop(selectedImage);

            offsetX = clickPosition.X - left;
            offsetY = clickPosition.Y - top;

            selectedImage.CaptureMouse();
        }

        private void Logo_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedImage == null || !selectedImage.IsMouseCaptured)
                return;
            if (selectedImage == selectImage)
            {
                Point mousePos = e.GetPosition(DesignCanvas);

                Canvas.SetLeft(selectedImage, mousePos.X - offsetX);
                Canvas.SetTop(selectedImage, mousePos.Y - offsetY);
                posX.Text = Canvas.GetLeft(selectedImage).ToString();
                posY.Text = Canvas.GetTop(selectedImage).ToString();
            }
        }

        private void Logo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedImage != null)
            {
                selectedImage.ReleaseMouseCapture();
                selectedImage = null;
            }
        }

        private void Overlap_Click(object sender, RoutedEventArgs e)
        {
            if (selectImage == null)
            {
                MessageBox.Show("Select an image first.");
                return;
            }
            string? tag = (sender as Button)?.Tag?.ToString()?.ToLower();
            switch (tag)
            {
                case "front":
                    Panel.SetZIndex(selectImage, 1000);
                    break;
                case "back":
                    Panel.SetZIndex(selectImage, -10);
                    break;
            }

        }

        private void HideShow_Click(object sender, RoutedEventArgs e)
        {
            if (selectImage == null)
            {
                MessageBox.Show("Select an image first.");
                return;
            }
            string? tag = (sender as Button)?.Tag?.ToString()?.ToLower();
           
            selectImage.Visibility = tag switch
            {
                "hide" => Visibility.Hidden,
                "show" => Visibility.Visible,
                _=> Visibility.Visible
            };
        }

        private void Color_Click(object sender, RoutedEventArgs e)
        {
            if (selectImage == null)
            {
                MessageBox.Show("Select an image first.");
                return;
            }

            var selectedItems = LogoList.SelectedItems
                                        .Cast<LogoItem>()
                                        .FirstOrDefault(x => x.Id.ToString() == selectImage.Tag.ToString());
            if (selectedItems == null) return;

            var logoItem = Images.FirstOrDefault(x => x.Id == selectedItems.Id);
            if (logoItem == null) return;

            string? tag = (sender as Button)?.Tag?.ToString()?.ToLower();
            string? newPath = "";
            newPath = tag switch
            {
                "red" => logoItem.RedImagePath,
                "blue" => logoItem.BlueImagePath,
                "green" => logoItem.GreenImagePath,
                "reset" => logoItem.DefaultImagePath,
                _ => logoItem.DefaultImagePath,
            };
            ;

            if (!string.IsNullOrEmpty(newPath) && File.Exists(newPath))
            {
                selectImage.Source = new BitmapImage(new Uri(newPath));

                logoItem.SelectedColor = newPath;
            }
        }

        private void Image_Size(object sender, RoutedEventArgs e)
        {
            string? tag = (sender as Button)?.Tag?.ToString()?.ToLower();
            if (selectImage == null) return;

            if (tag == "+")
            {
                selectImage.Width += 2;
                selectImage.Height += 2;
            }
            else
            {
                selectImage.Width -= 2;
                selectImage.Height -= 2;
            }
            img.Text = selectImage.Height.ToString();
        }
    }
}
