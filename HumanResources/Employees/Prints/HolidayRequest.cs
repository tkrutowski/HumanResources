using Konfiguracja;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HumanResources.Employees.Prints
{
    public class HolidayRequest
    {
        //ścieżka do pliku temp
        static string temp = Path.GetTempPath();
        public static void Print()
        {

            //zmienna do wysylania i pobierania plików
            WebClient request = new WebClient();
            //missing oject to use with various word commands
            object missing = System.Reflection.Missing.Value;

            //dane logowania
            request.Credentials = new NetworkCredential(Polaczenia.ftpLogin, Polaczenia.ftpHaslo);
            //pobieranie pliku
            request.DownloadFile(new Uri("ftp://finanse.focik.net/Pliki/Wzory/wniosek_urlopowy.doc"), temp + "\\wniosek_urlopowy.doc");

            //the template file you will be using, you need to locate the template we   previously made
            object fileToOpen = (object)temp + "\\wniosek_urlopowy.doc";


            //Create new instance of word and create a new document
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document doc = null;

            //Properties for the new word document, so everything happens in the background. If this isn’t set all the word documents will be visible
            object readOnly = false;
            object isVisible = false;
            //Settings the application to invisible, so the user doesn't notice that anything is going on
            wordApp.Visible = false;

            //Open and activate the chosen template
            doc = wordApp.Documents.Open(ref fileToOpen, ref missing,
                  ref readOnly, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref isVisible, ref missing, ref missing,
                  ref missing, ref missing);

            doc.Activate();

            //Making word visible to be able to show the print preview.
            wordApp.Visible = true;
            doc.PrintPreview();
        }
    }
}
