using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace SpaceRangersHdSaveEditor
{
    internal static class EditorAssets
    {
        private static readonly Dictionary<string, Image> Images =
            new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private static Icon icon;

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr handle);

        internal static Icon AppIcon()
        {
            if (icon != null) return (Icon)icon.Clone();
            using (Bitmap bitmap = new Bitmap(32, 32))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.Clear(Color.Transparent);
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        PointF[] star = {
                            new PointF(16, 2), new PointF(19, 12), new PointF(30, 16),
                            new PointF(19, 20), new PointF(16, 30), new PointF(13, 20),
                            new PointF(2, 16), new PointF(13, 12)
                        };
                        path.AddPolygon(star);
                        using (LinearGradientBrush fill = new LinearGradientBrush(
                            new Rectangle(2, 2, 28, 28), Color.Gold, Color.DeepSkyBlue, 45F))
                            graphics.FillPath(fill, path);
                        using (Pen border = new Pen(Color.FromArgb(38, 54, 82), 2F))
                            graphics.DrawPath(border, path);
                    }
                    using (Brush center = new SolidBrush(Color.White))
                        graphics.FillEllipse(center, 13, 13, 6, 6);
                }
                IntPtr handle = bitmap.GetHicon();
                try
                {
                    using (Icon temporary = Icon.FromHandle(handle)) icon = (Icon)temporary.Clone();
                }
                finally
                {
                    DestroyIcon(handle);
                }
            }
            return (Icon)icon.Clone();
        }

        internal static Image Image(string name)
        {
            string key = name ?? string.Empty;
            Image existing;
            if (Images.TryGetValue(key, out existing)) return existing;
            Bitmap bitmap = new Bitmap(16, 16);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.Transparent);
                Color color = ColorFor(key);
                using (Brush fill = new SolidBrush(color))
                using (Pen border = new Pen(Color.FromArgb(50, 62, 78), 1.25F))
                {
                    if (key.IndexOf("folder", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        graphics.FillRectangle(fill, 1, 5, 14, 9);
                        graphics.FillRectangle(fill, 2, 3, 6, 4);
                        graphics.DrawRectangle(border, 1, 5, 13, 8);
                    }
                    else if (key.IndexOf("disk", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        graphics.FillRectangle(fill, 2, 1, 12, 14);
                        graphics.DrawRectangle(border, 2, 1, 11, 13);
                        graphics.FillRectangle(Brushes.White, 4, 9, 8, 4);
                    }
                    else if (key.IndexOf("refresh", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (Pen refreshPen = new Pen(color, 2.5F))
                            graphics.DrawArc(refreshPen, 2, 2, 11, 11, 35, 285);
                        graphics.FillPolygon(fill, new[] { new Point(12, 1), new Point(15, 5), new Point(10, 6) });
                    }
                    else if (key.IndexOf("world", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (Brush ocean = new SolidBrush(Color.FromArgb(62, 151, 219)))
                        using (Brush land = new SolidBrush(Color.FromArgb(91, 178, 92)))
                        {
                            graphics.FillEllipse(ocean, 1, 1, 14, 14);
                            graphics.FillPie(land, 2, 2, 10, 8, 165, 120);
                            graphics.FillPie(land, 6, 7, 8, 7, 5, 125);
                            graphics.DrawEllipse(border, 1, 1, 13, 13);
                            graphics.DrawArc(border, 5, 1, 6, 14, 90, 180);
                        }
                    }
                    else if (key.IndexOf("controller", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        key.IndexOf("player", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (Brush suit = new SolidBrush(Color.FromArgb(54, 125, 190)))
                        using (Brush visor = new SolidBrush(Color.FromArgb(255, 199, 80)))
                        using (GraphicsPath body = RoundedBodyPath())
                        {
                            graphics.FillEllipse(visor, 5, 1, 6, 6);
                            graphics.DrawEllipse(border, 5, 1, 5, 5);
                            graphics.FillPath(suit, body);
                            graphics.DrawLine(border, 4, 14, 12, 14);
                        }
                    }
                    else if (key.IndexOf("cross_reference", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        key.IndexOf("mods", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (Brush blue = new SolidBrush(Color.FromArgb(72, 137, 210)))
                        using (Brush green = new SolidBrush(Color.FromArgb(91, 178, 92)))
                        using (Brush orange = new SolidBrush(Color.FromArgb(242, 165, 70)))
                        {
                            graphics.FillRectangle(blue, 1, 2, 6, 6);
                            graphics.FillRectangle(green, 9, 2, 6, 6);
                            graphics.FillRectangle(orange, 5, 9, 6, 6);
                            graphics.DrawRectangle(border, 1, 2, 5, 5);
                            graphics.DrawRectangle(border, 9, 2, 5, 5);
                            graphics.DrawRectangle(border, 5, 9, 5, 5);
                            graphics.DrawLine(border, 7, 5, 9, 5);
                            graphics.DrawLine(border, 5, 8, 8, 11);
                            graphics.DrawLine(border, 11, 8, 8, 11);
                        }
                    }
                    else if (key.IndexOf("cup", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        key.IndexOf("achievement", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (Brush gold = new SolidBrush(Color.FromArgb(241, 179, 45)))
                        {
                            graphics.FillRectangle(gold, 4, 2, 8, 7);
                            graphics.FillRectangle(gold, 7, 8, 2, 4);
                            graphics.FillRectangle(gold, 4, 12, 8, 3);
                            graphics.DrawRectangle(border, 4, 2, 7, 6);
                            graphics.DrawArc(border, 1, 3, 6, 6, 90, 180);
                            graphics.DrawArc(border, 9, 3, 6, 6, 270, 180);
                        }
                    }
                    else if (key.IndexOf("gear", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (Pen gearPen = new Pen(Color.FromArgb(90, 107, 126), 2.2F))
                        {
                            graphics.DrawEllipse(gearPen, 4, 4, 8, 8);
                            for (int spoke = 0; spoke < 8; spoke++)
                            {
                                double angle = spoke * Math.PI / 4D;
                                Point inner = new Point(8 + (int)Math.Round(Math.Cos(angle) * 5D),
                                    8 + (int)Math.Round(Math.Sin(angle) * 5D));
                                Point outer = new Point(8 + (int)Math.Round(Math.Cos(angle) * 7D),
                                    8 + (int)Math.Round(Math.Sin(angle) * 7D));
                                graphics.DrawLine(gearPen, inner, outer);
                            }
                            graphics.FillEllipse(Brushes.White, 6, 6, 4, 4);
                        }
                    }
                    else if (key.IndexOf("arrow_left", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (Brush arrow = new SolidBrush(Color.FromArgb(74, 127, 180)))
                            graphics.FillPolygon(arrow, new[] { new Point(3, 8), new Point(11, 2), new Point(11, 14) });
                    }
                    else if (key.IndexOf("arrow_right", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (Brush arrow = new SolidBrush(Color.FromArgb(74, 127, 180)))
                            graphics.FillPolygon(arrow, new[] { new Point(13, 8), new Point(5, 2), new Point(5, 14) });
                    }
                    else
                    {
                        graphics.FillPolygon(fill, new[] { new Point(8, 1), new Point(15, 8),
                            new Point(8, 15), new Point(1, 8) });
                        graphics.DrawPolygon(border, new[] { new Point(8, 1), new Point(15, 8),
                            new Point(8, 15), new Point(1, 8) });
                    }
                }
            }
            Images[key] = bitmap;
            return bitmap;
        }

        private static GraphicsPath RoundedBodyPath()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(2, 7, 12, 8, 180, 180);
            path.AddLine(14, 11, 14, 15);
            path.AddLine(2, 15, 2, 11);
            path.CloseFigure();
            return path;
        }

        private static Color ColorFor(string value)
        {
            int hash = 17;
            foreach (char character in value) hash = unchecked(hash * 31 + character);
            int red = 70 + (hash & 0x7f);
            int green = 90 + ((hash >> 7) & 0x7f);
            int blue = 110 + ((hash >> 14) & 0x7f);
            return Color.FromArgb(Math.Min(220, red), Math.Min(220, green), Math.Min(220, blue));
        }
    }
}
