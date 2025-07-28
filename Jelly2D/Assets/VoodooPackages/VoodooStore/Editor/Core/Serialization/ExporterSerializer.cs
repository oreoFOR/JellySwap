using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{
    public static class ExporterSerializer
    {
        private const string FileName = "exporterInfo.json";
        
        private static readonly string FilePath = Path.Combine(PathHelper.DirectoryPath, FileName);
        
        public static void Read()
        {
            ExporterData data;

            if (File.Exists(FilePath) == false)
            {
                data = new ExporterData();
                DataToStore(data);
                return;
            }

            string text;
            using (StreamReader reader = File.OpenText(FilePath))
            {
                text = reader.ReadToEnd();
                reader.Close();
            }

            data = JsonUtility.FromJson<ExporterData>(text);

            if (data == null)
            {
                data = new ExporterData();
            }

            DataToStore(data);
        }

        private static void DataToStore(ExporterData data)
        {
            Exporter.data = data;
        }

        public static void Write()
        {
            ExporterData data = ExporterToData();

            string text = JsonUtility.ToJson(data, true);

            if (Directory.Exists(PathHelper.DirectoryPath) == false)
            {
                Directory.CreateDirectory(PathHelper.DirectoryPath);
            }

            File.WriteAllText(FilePath, text);
        }

        private static ExporterData ExporterToData()
        {
            string[] tempCategories        = new string[Exporter.data.categories.Length];
            string[] tempSubCategories     = new string[Exporter.data.subCategories.Length];
            string[] tempAuthors           = new string[Exporter.data.authors.Length];
            
            Array.Copy(Exporter.data.categories,        tempCategories,        Exporter.data.categories.Length);
            Array.Copy(Exporter.data.subCategories,     tempSubCategories,     Exporter.data.subCategories.Length);
            Array.Copy(Exporter.data.authors,           tempAuthors,           Exporter.data.authors.Length);
            
            return new ExporterData
            {
                dependencyPackages             = new List<DependencyPackage>(Exporter.data.dependencyPackages),
                unselectableDependencyPackages = new List<string>(Exporter.data.unselectableDependencyPackages),
                elementsToExport               = new List<string>(Exporter.data.elementsToExport),
                isNewPackage                   = Exporter.data.isNewPackage,
                onlinePackage                  = Exporter.data.onlinePackage,
                package                        = Exporter.data.package,
                newSubCategory                 = Exporter.data.newSubCategory,
                categorySelected               = Exporter.data.categorySelected,
                subCategorySelected            = Exporter.data.subCategorySelected,
                newSubCategoryText             = Exporter.data.newSubCategoryText,
                newAuthor                      = Exporter.data.newAuthor,
                selectedAuthor                 = Exporter.data.selectedAuthor,
                commitMessage                  = Exporter.data.commitMessage,
                categories                     = tempCategories,
                subCategories                  = tempSubCategories,
                authors                        = tempAuthors
            };
        }
    }
}