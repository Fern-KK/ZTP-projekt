using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ZTP;

    public static class GlobalGroups
    {
        public static Group AllGroup = new Group("Wszystko");
        public static Group AllTasksGroup = new Group("Zadania");
        public static Group AllNotesGroup = new Group("Notatki");
        
        public static void Initialize()
        {
            // Dodaj domyślne kategorie
            Categories.Add("szkoła");
            Categories.Add("dom");
            
            // Opcjonalnie: dodaj też tagi
            Tags.Add("pilne");
            Tags.Add("ważne");
            Tags.Add("codzienne");
        }
    }