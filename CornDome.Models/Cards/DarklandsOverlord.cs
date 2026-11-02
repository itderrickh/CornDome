using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CornDome.Models.Cards
{
    [Table("darklands_overlord")]
    public class DarklandsOverlord
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public enum DarklandsOverlordEnum
    {
        CaptainRatbeard,
        SkyklorTheOozemother,
        QuarksExperiment,
        TheCrushasaurus
    }

    public static class DarklandsOverlordEnumConverter
    {
        public static string ToString(DarklandsOverlordEnum cardType)
        {
            return cardType switch
            {
                DarklandsOverlordEnum.CaptainRatbeard => "Captain Ratbeard",
                DarklandsOverlordEnum.SkyklorTheOozemother => "Skyklor, the Oozemother",
                DarklandsOverlordEnum.QuarksExperiment => "Quark's Experiment",
                DarklandsOverlordEnum.TheCrushasaurus => "The Crushasaurus!!!",
                _ => "",
            };
        }
    }
}
