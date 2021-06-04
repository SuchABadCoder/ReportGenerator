using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Word;

namespace GitHubApiApp.Models
{
    public static class WordApi
    {
        private static void FindAndReplace(Application wordApp, object ToFindText, object replaceWithText)
        {
            if (replaceWithText.ToString().Length > 100)
            {
                List<string> parts = FilesManager.Split(replaceWithText.ToString());

                wordApp.Selection.Find.Execute(ref ToFindText,
                    true, true, false, false, false, true,
                    1, false, $"{parts[0]} %$#", 2, false,
                    false, false, false);
                for (int i = 1; i < parts.Count; i++)
                {
                    wordApp.Selection.Find.Execute("%$#",
                    true, true, false, false, false, true,
                    1, false, $"{parts[i]} %$#", 2, false,
                    false, false, false);
                }
                wordApp.Selection.Find.Execute("%$#",
                    true, true, false, false, false, true,
                    1, false, "", 2, false,
                    false, false, false);
            }
            else
                wordApp.Selection.Find.Execute(ref ToFindText,
                    true, true, false, false, false, true,
                    1, false, ref replaceWithText, 2, false,
                    false, false, false);
        }

        private static void InsertPhoto(Application wordApp, Document document, List<FileData> photo)
        {
            if (photo.Count != 0)
            {
                Find fnd = wordApp.ActiveWindow.Selection.Find;
                fnd.ClearFormatting();
                fnd.Replacement.ClearFormatting();
                fnd.Forward = true;
                fnd.Wrap = WdFindWrap.wdFindContinue;
                var keyword = "<image>";
                var sel = wordApp.Selection;
                sel.Find.Text = string.Format("[{0}]", keyword);
                wordApp.Selection.Find.Execute(keyword);
                Range range = wordApp.Selection.Range;
                if (range.Text.Contains(keyword))
                {
                    Range temprange = document.Range(range.End - 7, range.End);
                    temprange.Select();
                    sel.Find.Execute(Replace: WdReplace.wdReplaceOne);
                    sel.Range.Select();
                    sel.InlineShapes.AddPicture(FileName: photo[0].url, LinkToFile: false, SaveWithDocument: true);
                }
                for (int i = 1; i < photo.Count; i++)
                {
                    sel.InsertAfter("\n\n");
                    sel.InlineShapes.AddPicture(FileName: photo[i].url, LinkToFile: false, SaveWithDocument: true);
                }
            }
            else
                FindAndReplace(wordApp, "<image>", "");
        }

        private static void InsertFiles(Application wordApp, Document document, List<FileData> files)
        {
            if (files.Count != 0)
            {
                FindAndReplace(wordApp, "<listing>", files[0].name);
                var sel = wordApp.Selection;
                sel.Find.Text = string.Format("[{0}]", files[0].name);
                wordApp.Selection.Find.Execute(files[0].name);
                Range range = wordApp.Selection.Range;
                if (range.Text.Contains(files[0].name))
                {
                    Range temprange = document.Range(range.End - 8, range.End);
                    temprange.Select();
                    sel.Find.Execute(Replace: WdReplace.wdReplaceOne);
                    sel.Range.Select();
                    sel.InsertAfter("\n");
                    sel.InsertAfter(files[0].contents);
                }
                for (int i = 1; i < files.Count; i++)
                {
                    sel.InsertAfter("\n\n");
                    sel.InsertAfter(files[i].name+":");
                    sel.InsertAfter("\n");
                    sel.InsertAfter(files[i].contents);
                }
            }
        }

        public static void CreateWordDocument(object filename, object SaveAs, List<FileData> photo, List<FileData> files, Dictionary<string, string> data)
        {
            Application wordApp = new Application();
            object missing = Missing.Value;
            Document document = null;

            if (File.Exists((string)filename))
            {
                wordApp.Visible = false;
                document = wordApp.Documents.Open(ref filename, ref missing, false, ref missing,
                    ref missing, ref missing, ref missing,
                     ref missing, ref missing, ref missing,
                      ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing);
                document.Activate();

                InsertFiles(wordApp, document, files);
                InsertPhoto(wordApp, document, photo);
                FindAndReplace(wordApp, "<year>", DateTime.Now.Year.ToString());
                foreach (var item in data)
                    FindAndReplace(wordApp, item.Key, item.Value);
            }

            document.SaveAs2(ref SaveAs);
            document.Close();
            wordApp.Quit();
        }
    }
}
