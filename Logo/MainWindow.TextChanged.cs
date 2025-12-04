using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Logo
{
    public partial class MainWindow : Window
    {
        private void Angle_Changed(object sender, EventArgs e)
        {
            if (selectImage == null)
            {
                MessageBox.Show("Select an image first.");
                return;
            }

            if (!double.TryParse(angle.Text, out double targetAngle))
            {
                targetAngle = (targetAngle % 360 + 360) % 360;
            }

            RotateTransform rt;

            if (selectImage.RenderTransform is RotateTransform existingRT)
            {
                rt = existingRT;
            }
            else
            {
                rt = new RotateTransform();
                selectImage.RenderTransform = rt;
                selectImage.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            rt.Angle = targetAngle;
        }

        private void Speed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectImage == null)
            {
                MessageBox.Show("Select an image first.");
                return;
            }
            if (double.TryParse(speed.Text, out double x))
            {
                if (rotateTimer == null) return;

                rotateTimer.Interval = TimeSpan.FromMilliseconds(x);
            }
        }

        private void PosX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectImage == null)
            {
                MessageBox.Show("Select an image first.");
                return;
            }
            if (double.TryParse(posX.Text, out double x))
            {
                Canvas.SetLeft(selectImage, x);
            }
        }

        private void PosY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectImage == null)
            {
                MessageBox.Show("Select an image first.");
                return;
            }
            if (double.TryParse(posY.Text, out double y))
            {
                Canvas.SetTop(selectImage, y);
            }
        }

        private void ImgSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectImage == null)
                return;

            if (double.TryParse(img.Text, out double x))
            {
                selectImage.Width = x;
                selectImage.Height = x;
            }
        }

        private void TxtBoxHeight_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (selectContainer == null)
            {
                MessageBox.Show("Select a Container first.");
                return;
            }
            if (double.TryParse(txt.Text, out double y))
            {
                selectContainer.Height = y;
            }
        }

        private void TxtBoxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (selectContainer == null)
            {
                MessageBox.Show("Select a Container first.");
                return;
            }
            if (double.TryParse(txtW.Text, out double y))
            {
                selectContainer.Width = y;
            }
        }
    }
}
