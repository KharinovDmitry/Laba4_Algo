using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace Core.ExternalSort
{
    internal class Renderer
    {
        private Image Image { get; }
        private int _cellWidth;
        private int _cellHeight;

        private List<List<RectangleF>> rectangles;

        Font drawFont = new Font("Arial", 12);

        public Renderer(Image image)
        {
            rectangles = new();
            Image = image;

        }

        private void InitRectangles(int n)
        {
            _cellWidth = (int)(Image.Width / n) - 1;
            _cellHeight = _cellWidth;

            for (int i = 0; i < 3; i++)
                rectangles.Add(new());
            for (int i = 0; i < n; i++)
            {
                rectangles[0].Add(new RectangleF(i * _cellWidth, 0 * _cellHeight, 1 * _cellWidth, _cellHeight));
                rectangles[1].Add(new RectangleF(i * _cellWidth, 2 * _cellHeight, 1 * _cellWidth, _cellHeight));
                rectangles[2].Add(new RectangleF(i * _cellWidth, 4 * _cellHeight, 1 * _cellWidth, _cellHeight));
            }
        }

        private void DrawLines(int n, Graphics gfx)
        {

            using (var pen = new System.Drawing.Pen(System.Drawing.Color.Black))
            {


                for (int i = 0; i < 3; i++)
                {
                    gfx.DrawRectangles(pen, rectangles[i].ToArray());
                }
            }

        }
        private BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        /*private void Render()
        {
            using (var bmp = new Bitmap((int)Image.Width, (int)Image.Height))
            using (var gfx = Graphics.FromImage(bmp))
            using (var pen = new System.Drawing.Pen(System.Drawing.Color.White))
            {
                // draw one thousand random white lines on a dark blue background
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(System.Drawing.Color.Navy);
                for (int i = 0; i < 1000; i++)
                {
                    var pt1 = new System.Drawing.Point(rand.Next(bmp.Width), rand.Next(bmp.Height));
                    var pt2 = new System.Drawing.Point(rand.Next(bmp.Width), rand.Next(bmp.Height));
                    gfx.DrawLine(pen, pt1, pt2);
                }

                Image.Source = BmpImageFromBmp(bmp);
            }
        }
*/

        public void Render(List<ExAction> actions)
        {
            using (var bmp = new Bitmap((int)Image.Width, (int)Image.Height))
            using (var gfx = Graphics.FromImage(bmp))
            {


                InitRectangles(10);
                DrawLines(10, gfx);
                foreach (var action in actions)
                {
                    DrawStep(action, gfx);
                    Image.Source = BmpImageFromBmp(bmp);
                }
                Image.Source = BmpImageFromBmp(bmp);
            }
        }

        // TODO передавай список файлов в конструктор
        private int Index(string filename)
        {
            switch (filename)
            {
                case "data.txt": return 0;

                case "a.txt": return 1;
                case "b.txt": return 2;
                default:
                    return -1;
            }
        }


        // ToFile/FromFile - индекс файла(строка)
        // rectangles[ToFile] - получил бы нужную срочку
        // rectangles[ToFile][ToIndex] - получил бы нужный рект



        private void DrawStep(ExAction action, Graphics gfx)
        {
            if (action.FromIndex >= 10 || action.ToIndex >= 10)
                return;
            switch (action.Action)
            {
                case Action.Compare:
                    {
                        var rect1 = rectangles[Index(action.FromFile)][action.FromIndex];
                        var rect2 = rectangles[Index(action.ToFile)][action.ToIndex];
                        using (var pen = new System.Drawing.Pen(System.Drawing.Color.Orange))
                        {
                            gfx.FillRectangle(pen.Brush, rect1);
                            gfx.FillRectangle(pen.Brush, rect2);

                            pen.Color = System.Drawing.Color.Black;
                            gfx.DrawString(action.ElementA.ToString()[..Math.Min(2, action.ElementA.ToString().Length)], drawFont, pen.Brush, rect1);
                            gfx.DrawString(action.ElementB.ToString()[..Math.Min(2, action.ElementB.ToString().Length)], drawFont, pen.Brush, rect2);

                            Thread.Sleep(200);
                        }
                        break;
                    }
            }
        }
    }
}
