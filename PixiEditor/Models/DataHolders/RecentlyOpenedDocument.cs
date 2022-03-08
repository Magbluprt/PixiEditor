﻿using PixiEditor.Helpers;
using PixiEditor.Models.IO;
using PixiEditor.Models.Position;
using PixiEditor.Parser;
using PixiEditor.Parser.Skia;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PixiEditor.Models.DataHolders
{
    [DebuggerDisplay("{FilePath}")]
    public class RecentlyOpenedDocument : NotifyableObject
    {
        private bool corrupt;

        private string filePath;

        private WriteableBitmap previewBitmap;

        public string FilePath
        {
            get => filePath;
            set
            {
                SetProperty(ref filePath, value);
                RaisePropertyChanged(nameof(FileName));
                RaisePropertyChanged(nameof(FileExtension));
                PreviewBitmap = null;
            }
        }

        public bool Corrupt { get => corrupt; set => SetProperty(ref corrupt, value); }

        public string FileName => Path.GetFileNameWithoutExtension(filePath);

        public string FileExtension
        {
            get
            {
                if (Corrupt)
                {
                    return "? (Corrupt)";
                }
                string extension = Path.GetExtension(filePath).ToLower();
                return SupportedFilesHelper.IsExtensionSupported(extension) ? extension : $"? ({extension})";
            }
        }

        public WriteableBitmap PreviewBitmap
        {
            get
            {
                if (previewBitmap == null && !Corrupt)
                {
                    previewBitmap = LoadPreviewBitmap();
                }

                return previewBitmap;
            }
            private set => SetProperty(ref previewBitmap, value);
        }

        public RecentlyOpenedDocument(string path)
        {
            FilePath = path;
        }

        private WriteableBitmap LoadPreviewBitmap()
        {
            if (FileExtension == ".pixi")
            {
                SerializableDocument serializableDocument;

                try
                {
                    serializableDocument = PixiParser.Deserialize(filePath);
                }
                catch
                {
                    corrupt = true;
                    return null;
                }

                using Surface surface = Surface.Combine(serializableDocument.Width, serializableDocument.Height,
                          serializableDocument.Layers
                              .Where(x => x.Opacity > 0.8)
                              .Select(x => (x.ToSKImage(), new Coordinates(x.OffsetX, x.OffsetY))));

                return DownscaleToMaxSize(surface.ToWriteableBitmap());
            }
            else if (SupportedFilesHelper.IsExtensionSupported(FileExtension))
            {
                WriteableBitmap bitmap = null;

                try
                {
                    bitmap = Importer.ImportWriteableBitmap(FilePath);
                }
                catch
                {
                    corrupt = true;
                    return null;
                }

                if (bitmap == null) //prevent crash
                    return null;

                return DownscaleToMaxSize(bitmap);
            }

            return null;
        }

        private WriteableBitmap DownscaleToMaxSize(WriteableBitmap bitmap)
        {
            if (bitmap.PixelWidth > Constants.MaxPreviewWidth || bitmap.PixelHeight > Constants.MaxPreviewHeight)
            {
                double factor = Math.Min(Constants.MaxPreviewWidth / (double)bitmap.PixelWidth, Constants.MaxPreviewHeight / (double)bitmap.PixelHeight);
                return bitmap.Resize((int)(bitmap.PixelWidth * factor), (int)(bitmap.PixelHeight * factor), WriteableBitmapExtensions.Interpolation.Bilinear);
            }
            return bitmap;
        }
    }
}
