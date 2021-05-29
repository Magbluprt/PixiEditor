﻿using PixiEditor.Parser;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace PixiEditor.SDK.FileParsers
{
    internal class FileParserList
    {
        public List<string> SupportedImageExtensions { get; set; } = new();

        public List<string> SupportedDocumentExtensions { get; set; } = new();

        public List<string> SupportedExtensions { get; set; } = new();

        public ListDictionary<string, ImageParserInfo> ImageParsers { get; set; } = new();

        public ListDictionary<string, DocumentParserInfo> DocumentParsers { get; set; } = new();

        public void AddDocumentParser(DocumentParserInfo info)
        {
            foreach (string ext in info.SupportedFileExtensions)
            {
                DocumentParsers.Add(ext, info);

                if (!SupportedDocumentExtensions.Contains(ext))
                {
                    SupportedDocumentExtensions.Add(ext);
                }

                if (!SupportedExtensions.Contains(ext))
                {
                    SupportedExtensions.Add(ext);
                }
            }
        }

        public void AddImageParser(ImageParserInfo info)
        {
            foreach (string ext in info.SupportedFileExtensions)
            {
                ImageParsers.Add(ext, info);

                if (!SupportedImageExtensions.Contains(ext))
                {
                    SupportedImageExtensions.Add(ext);
                }

                if (!SupportedExtensions.Contains(ext))
                {
                    SupportedExtensions.Add(ext);
                }
            }
        }

        public ImageParser CreateImageParser(string extensions, Stream stream) => 
            Create<ImageParserInfo, ImageParser, WriteableBitmap>(ImageParsers, extensions, stream);

        public DocumentParser CreateDocumentParser(string extension, Stream stream) =>
            Create<DocumentParserInfo, DocumentParser, SerializableDocument>(DocumentParsers, extension, stream);

        private static TParser Create<TParserInfo, TParser, T>(
            ListDictionary<string, TParserInfo> dict,
            string extension,
            Stream stream)

            where TParserInfo : FileParserInfo<TParser, T>
            where TParser : FileParser<T>
        {
            if (!dict.ContainsKey(extension))
            {
                return null;
            }

            var parserInfos = dict[extension];

            foreach (var fileParserInfo in parserInfos)
            {
                if (fileParserInfo.Enabled)
                {
                    return fileParserInfo.Create(stream);
                }
            }

            return null;
        }
    }
}