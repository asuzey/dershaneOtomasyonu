using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dershaneOtomasyonu.Helpers
{
    public static class GlobalFontHelper
    {
        private static PrivateFontCollection _fontCollection = new();
        private static Dictionary<string, FontFamily> _fontFamilies = new();
        private static bool _isLoaded = false;

        public static void LoadFonts()
        {
            if (_isLoaded) return;

            string fontFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Fonts");

            // Font dosyalarının tam yollarını al
            string[] fontFiles = Directory.GetFiles(fontFolder, "*.otf");

            foreach (var fontFile in fontFiles)
            {
                _fontCollection.AddFontFile(fontFile);
            }

            foreach (var family in _fontCollection.Families)
            {
                if (!_fontFamilies.ContainsKey(family.Name))
                    _fontFamilies.Add(family.Name, family);
            }

            _isLoaded = true;
        }

        public static Font GetFont(string familyName, float size, FontStyle style = FontStyle.Regular)
        {
            LoadFonts();

            if (_fontFamilies.ContainsKey(familyName))
            {
                return new Font(_fontFamilies[familyName], size, style);
            }

            // Eğer bulunamazsa varsayılan fonta düş
            return new Font("Century Gothic", size, style);
        }

        // Kolaylık için SourceSansPro kısayolu
        public static Font SourceSans(float size, FontStyle style = FontStyle.Regular)
        {
            return GetFont("Source Sans Pro", size, style);
        }

        public static void ApplySourceSansToAllControls(Control parent)
        {
            LoadFonts();

            foreach (Control ctrl in parent.Controls)
            {
                // Mevcut yazı tipi bilgilerini al (boyut, kalınlık, italik vs.)
                var oldFont = ctrl.Font;
                ctrl.Font = new Font(GetFont("Source Sans Pro", oldFont.Size, oldFont.Style).FontFamily, oldFont.Size, oldFont.Style);

                // Eğer kontrolün içinde başka kontroller varsa (örneğin panel içinde label), onlara da uygula
                if (ctrl.HasChildren)
                {
                    ApplySourceSansToAllControls(ctrl);
                }
            }

            // En dış Form’a da uygula
            var formFont = parent.Font;
            parent.Font = new Font(GetFont("Source Sans Pro", formFont.Size, formFont.Style).FontFamily, formFont.Size, formFont.Style);
        }

    }
}
