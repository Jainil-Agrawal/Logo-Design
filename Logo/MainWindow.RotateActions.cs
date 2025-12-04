using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Logo
{
    public partial class MainWindow : Window
    {
        private void StartRotate_Click(object sender, RoutedEventArgs e)
        {
            StartRotation();
            UpdateSelectionMode();
        }

        private void StopRotate_Click(object sender, RoutedEventArgs e)
        {
            StopRotation();
            UpdateSelectionMode();
        }

        private void StartRotation()
        {
            if (selectImage == null)
            {
                MessageBox.Show("Select an image first.");
                return;
            }

            if (rotateTimer == null)
            {
                if (int.TryParse(speed.Text, out int x))
                {
                    rotateSpeed = x;
                }
                rotateTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(rotateSpeed)
                };
                rotateTimer.Tick += RotateTimer_Tick;
            }

            rotateTimer.Start();
            if (selectImage.RenderTransform is RotateTransform rt)
            {
                angle.Text = rt.Angle.ToString();
            }
        }

        private void StopRotation()
        {
            if (rotateTimer != null)
            {
                if (selectImage != null && rotateImages.Count == 0)
                {
                    rotateTimer.Stop();
                    return;
                }
                rotateTimer.Stop();

                var lastSelected = LogoList.SelectedItems.Cast<LogoItem>().LastOrDefault();

                LogoList.SelectedItems.Clear();

                if (lastSelected != null && LogoList.SelectionMode != SelectionMode.Single)
                {
                    LogoList.SelectedItems.Add(lastSelected);
                }
                else { LogoList.SelectedItem = lastSelected; }

                rotateImages = [];
            }

            if (selectImage != null && selectImage.RenderTransform is RotateTransform rt)
            {
                angle.Text = rt.Angle.ToString();
            }
        }

        private void RotateTimer_Tick(object? sender, EventArgs e)
        {
            if (selectImage == null) return;
            if (rotateImages.Count == 0)
            {
                if (selectImage.RenderTransform is RotateTransform oldRotate)
                    currentAngle = Convert.ToSingle(oldRotate.Angle);
                else
                    currentAngle = 0;

                currentAngle += 5;
                if (currentAngle > 360)
                    currentAngle = 0;
                angle.Text = currentAngle.ToString();
                RotateTransform rotate = new (currentAngle);
                selectImage.RenderTransform = rotate;
                selectImage.RenderTransformOrigin = new Point(0.5, 0.5);
                angle.Text = currentAngle.ToString();
                if (LogoList.SelectedItems.Count != 0)
                {
                    int index = LogoList.SelectedItems
                           .Cast<LogoItem>()
                           .ToList()
                           .FindIndex(x => x.Id.ToString() == selectImage.Tag.ToString());
                    var selectedItem = LogoList.SelectedItems[index] as LogoItem;
                    selectedItem.Angle = currentAngle;
                }
            }
            else
            {
                for (int i = 0; i < rotateImages.Count; i++)
                {
                    int index = LogoList.SelectedItems
                       .Cast<LogoItem>()
                       .ToList()
                       .FindIndex(x => x.Id.ToString() == rotateImages[i].Tag.ToString());
                    var selectedItem = LogoList.SelectedItems[index] as LogoItem;


                    selectedItem.Angle += 5;

                    if (selectedItem.Angle >= 360)
                        selectedItem.Angle = 0;

                    selectedItem.Angle = selectedItem.Angle;

                    rotateImages[i].RenderTransform = new RotateTransform(selectedItem.Angle);
                    rotateImages[i].RenderTransformOrigin = new Point(0.5, 0.5);
                    angle.Text = selectedItem.Angle.ToString();
                }
            }
        }

        private void SpeedUp_Click(object sender, RoutedEventArgs e)
        {
            if (rotateTimer == null) return;
            if (int.TryParse(speed.Text, out int x))
            {
                rotateSpeed = x;
            }
            rotateSpeed -= 20;
            if (rotateSpeed < 10) rotateSpeed = 0;
            speed.Text = rotateSpeed.ToString();
            rotateTimer.Interval = TimeSpan.FromMilliseconds(rotateSpeed);
        }

        private void SpeedDown_Click(object sender, RoutedEventArgs e)
        {
            if (rotateTimer == null) return;
            if (int.TryParse(speed.Text, out int x))
            {
                rotateSpeed = x;
            }
            rotateSpeed += 20;
            speed.Text = rotateSpeed.ToString();
            rotateTimer.Interval = TimeSpan.FromMilliseconds(rotateSpeed);
        }
    }
}
