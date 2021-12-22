using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhishingTraining.Web.Helpers
{
    public static class SurveyHelper
    {
        public static Dictionary<int, string> Locations { get; } = new Dictionary<int, string>()
        {
            {1, "zu Hause"},
            {2, "zu Hause bei Freunden"},
            {3, "in der Arbeit (bezahlt/ehrenamtlich)"},
            {4, "in der Schule"},
            {5, "in einem Restaurant oder einer Bar"},
            {6, "in einem Geschäft"},
            {7, "im Fitnessstudio/ Sportanlage"},
            {8, "in einem Krankenhaus oder beim Arzt"},
            {9, "in einem Fahrzeug (Auto, Bus, etc.)"},
            {10, "in einem öffentlichen Gebäude"},
            {11, "in einem Park oder Garten"},
            {12, "draußen/auf der Straße" }
        };

        public static Dictionary<int, string> Activites { get; } = new Dictionary<int, string>()
        {
            { 1, "Ich bin körperlich leicht aktiv (z.B. zu Fuß gehen)" },
            { 2, "Ich bin körperlich aktiv (Sport)" },
            { 3, "Im Gespräch (persönlich)" },
            { 4, "Im Gespräch ((Video)Telefonie)" },
            { 5, "Schreiben per App/SMS/…" },
            { 6, "Essen" },
            { 7, "Arbeiten (bezahlt oder ehrenamtlich)" },
            { 8, "Lernen oder Hausaufgaben" },
            { 9, "Hausarbeit/Haushalt" },
            { 10, "Einkauf" },
            { 11, "Lesen" },
            { 12, "Fernsehen" },
            { 13, "Internet surfen" },
            { 14, "Ausruhen/nichts-tun" },
            { 15, "Etwas anderes" }
        };

        public static Dictionary<int, string> Company { get; } = new Dictionary<int, string>()
        {
            { 1, "Alleine" },
            { 2, "mit der Familie" },
            { 3, "mit meinem Partner" },
            { 4, "mit Freunden" },
            { 5, "mit Mitschülern" },
            { 6, "mit Fremden" },
            { 7, "Andere" }
        };
    }
}