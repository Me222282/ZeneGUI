using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public static class XmlTypeParser
    {
        public static Cursor CursorParser(string str)
        {
            return str switch
            {
                "Arrow" => Cursor.Arrow,
                "CrossHair" => Cursor.CrossHair,
                "Default" => Cursor.Default,
                "Hand" => Cursor.Hand,
                "IBeam" => Cursor.IBeam,
                "NotAllowed" => Cursor.NotAllowed,
                "ResizeAll" => Cursor.ResizeAll,
                "ResizeBottomLeft" => Cursor.ResizeBottomLeft,
                "ResizeBottomRight" => Cursor.ResizeBottomRight,
                "ResizeHorizontal" => Cursor.ResizeHorizontal,
                "ResizeTopLeft" => Cursor.ResizeTopLeft,
                "ResizeTopRight" => Cursor.ResizeTopRight,
                "ResizeVertical" => Cursor.ResizeVertical,
                _ => throw new Exception("Invalid Cursor syntax")
            };
        }
        public static Colour ColourParser(string str)
        {
            return str switch
            {
                "MediumVioletRed" => Colour.MediumVioletRed,
                "DeepPink" => Colour.DeepPink,
                "PaleVioletRed" => Colour.PaleVioletRed,
                "HotPink" => Colour.HotPink,
                "LightPink" => Colour.LightPink,
                "Pink" => Colour.Pink,

                "DarkRed" => Colour.DarkRed,
                "Red" => Colour.Red,
                "Firebrick" => Colour.Firebrick,
                "Crimson" => Colour.Crimson,
                "IndianRed" => Colour.IndianRed,
                "LightCoral" => Colour.LightCoral,
                "Salmon" => Colour.Salmon,
                "DarkSalmon" => Colour.DarkSalmon,
                "LightSalmon" => Colour.LightSalmon,

                "OrangeRed" => Colour.OrangeRed,
                "Tomato" => Colour.Tomato,
                "DarkOrange" => Colour.DarkOrange,
                "Coral" => Colour.Coral,
                "Orange" => Colour.Orange,

                "DarkKhaki" => Colour.DarkKhaki,
                "Gold" => Colour.Gold,
                "Khaki" => Colour.Khaki,
                "PeachPuff" => Colour.PeachPuff,
                "Yellow" => Colour.Yellow,
                "PaleGoldenrod" => Colour.PaleGoldenrod,
                "Moccasin" => Colour.Moccasin,
                "PapayaWhip" => Colour.PapayaWhip,
                "LightGoldenrodYellow" => Colour.LightGoldenrodYellow,
                "LemonChiffon" => Colour.LemonChiffon,
                "LightYellow" => Colour.LightYellow,

                "Maroon" => Colour.Maroon,
                "Brown" => Colour.Brown,
                "SaddleBrown" => Colour.SaddleBrown,
                "Sienna" => Colour.Sienna,
                "Chocolate" => Colour.Chocolate,
                "DarkGoldenrod" => Colour.DarkGoldenrod,
                "Peru" => Colour.Peru,
                "RosyBrown" => Colour.RosyBrown,
                "Goldenrod" => Colour.Goldenrod,
                "SandyBrown" => Colour.SandyBrown,
                "Tan" => Colour.Tan,
                "Burlywood" => Colour.Burlywood,
                "Wheat" => Colour.Wheat,
                "NavajoWhite" => Colour.NavajoWhite,
                "Bisque" => Colour.Bisque,
                "BlanchedAlmond" => Colour.BlanchedAlmond,
                "Cornsilk" => Colour.Cornsilk,

                "Indigo" => Colour.Indigo,
                "Purple" => Colour.Purple,
                "DarkMagenta" => Colour.DarkMagenta,
                "DarkViolet" => Colour.DarkViolet,
                "DarkSlateBlue" => Colour.DarkSlateBlue,
                "BlueViolet" => Colour.BlueViolet,
                "DarkOrchid" => Colour.DarkOrchid,
                "Fuchsia" => Colour.Fuchsia,
                "Magenta" => Colour.Magenta,
                "SlateBlue" => Colour.SlateBlue,
                "MediumSlateBlue" => Colour.MediumSlateBlue,
                "MediumOrchid" => Colour.MediumOrchid,
                "MediumPurple" => Colour.MediumPurple,
                "Orchid" => Colour.Orchid,
                "Violet" => Colour.Violet,
                "Plum" => Colour.Plum,
                "Thistle" => Colour.Thistle,
                "Lavender" => Colour.Lavender,

                "MidnightBlue" => Colour.MidnightBlue,
                "Navy" => Colour.Navy,
                "DarkBlue" => Colour.DarkBlue,
                "MediumBlue" => Colour.MediumBlue,
                "Blue" => Colour.Blue,
                "RoyalBlue" => Colour.RoyalBlue,
                "SteelBlue" => Colour.SteelBlue,
                "DodgerBlue" => Colour.DodgerBlue,
                "DeepSkyBlue" => Colour.DeepSkyBlue,
                "CornflowerBlue" => Colour.CornflowerBlue,
                "SkyBlue" => Colour.SkyBlue,
                "LightSkyBlue" => Colour.LightSkyBlue,
                "LightSteelBlue" => Colour.LightSteelBlue,
                "LightBlue" => Colour.LightBlue,
                "PowderBlue" => Colour.PowderBlue,

                "Teal" => Colour.Teal,
                "DarkCyan" => Colour.DarkCyan,
                "LightSeaGreen" => Colour.LightSeaGreen,
                "CadetBlue" => Colour.CadetBlue,
                "DarkTurquoise" => Colour.DarkTurquoise,
                "MediumTurquoise" => Colour.MediumTurquoise,
                "Turquoise" => Colour.Turquoise,
                "Aqua" => Colour.Aqua,
                "Cyan" => Colour.Cyan,
                "Aquamarine" => Colour.Aquamarine,
                "PaleTurquoise" => Colour.PaleTurquoise,
                "LightCyan" => Colour.LightCyan,

                "DarkGreen" => Colour.DarkGreen,
                "Green" => Colour.Green,
                "DarkOliveGreen" => Colour.DarkOliveGreen,
                "ForestGreen" => Colour.ForestGreen,
                "SeaGreen" => Colour.SeaGreen,
                "Olive" => Colour.Olive,
                "OliveDrab" => Colour.OliveDrab,
                "MediumSeaGreen" => Colour.MediumSeaGreen,
                "LimeGreen" => Colour.LimeGreen,
                "Lime" => Colour.Lime,
                "SpringGreen" => Colour.SpringGreen,
                "MediumSpringGreen" => Colour.MediumSpringGreen,
                "DarkSeaGreen" => Colour.DarkSeaGreen,
                "MediumAquamarine" => Colour.MediumAquamarine,
                "YellowGreen" => Colour.YellowGreen,
                "LawnGreen" => Colour.LawnGreen,
                "Chartreuse" => Colour.Chartreuse,
                "LightGreen" => Colour.LightGreen,
                "GreenYellow" => Colour.GreenYellow,
                "PaleGreen" => Colour.PaleGreen,

                "MistyRose" => Colour.MistyRose,
                "AntiqueWhite" => Colour.AntiqueWhite,
                "Linen" => Colour.Linen,
                "Beige" => Colour.Beige,
                "WhiteSmoke" => Colour.WhiteSmoke,
                "LavenderBlush" => Colour.LavenderBlush,
                "OldLace" => Colour.OldLace,
                "AliceBlue" => Colour.AliceBlue,
                "Seashell" => Colour.Seashell,
                "GhostWhite" => Colour.GhostWhite,
                "Honeydew" => Colour.Honeydew,
                "FloralWhite" => Colour.FloralWhite,
                "Azure" => Colour.Azure,
                "MintCream" => Colour.MintCream,
                "Snow" => Colour.Snow,
                "Ivory" => Colour.Ivory,
                "White" => Colour.White,

                "Black" => Colour.Black,
                "DarkSlateGrey" => Colour.DarkSlateGrey,
                "DimGrey" => Colour.DimGrey,
                "SlateGrey" => Colour.SlateGrey,
                "DarkGrey" => Colour.DarkGrey,
                "LightSlateGrey" => Colour.LightSlateGrey,
                "Grey" => Colour.Grey,
                "Silver" => Colour.Silver,
                "LightGrey" => Colour.LightGrey,
                "Gainsboro" => Colour.Gainsboro,
                _ => throw new Exception("Invalid Colour syntax")
            };
        }
        public static ColourF ColourParserF(string str)
        {
            return str switch
            {
                "MediumVioletRed" => ColourF.MediumVioletRed,
                "DeepPink" => ColourF.DeepPink,
                "PaleVioletRed" => ColourF.PaleVioletRed,
                "HotPink" => ColourF.HotPink,
                "LightPink" => ColourF.LightPink,
                "Pink" => ColourF.Pink,

                "DarkRed" => ColourF.DarkRed,
                "Red" => ColourF.Red,
                "Firebrick" => ColourF.Firebrick,
                "Crimson" => ColourF.Crimson,
                "IndianRed" => ColourF.IndianRed,
                "LightCoral" => ColourF.LightCoral,
                "Salmon" => ColourF.Salmon,
                "DarkSalmon" => ColourF.DarkSalmon,
                "LightSalmon" => ColourF.LightSalmon,

                "OrangeRed" => ColourF.OrangeRed,
                "Tomato" => ColourF.Tomato,
                "DarkOrange" => ColourF.DarkOrange,
                "Coral" => ColourF.Coral,
                "Orange" => ColourF.Orange,

                "DarkKhaki" => ColourF.DarkKhaki,
                "Gold" => ColourF.Gold,
                "Khaki" => ColourF.Khaki,
                "PeachPuff" => ColourF.PeachPuff,
                "Yellow" => ColourF.Yellow,
                "PaleGoldenrod" => ColourF.PaleGoldenrod,
                "Moccasin" => ColourF.Moccasin,
                "PapayaWhip" => ColourF.PapayaWhip,
                "LightGoldenrodYellow" => ColourF.LightGoldenrodYellow,
                "LemonChiffon" => ColourF.LemonChiffon,
                "LightYellow" => ColourF.LightYellow,

                "Maroon" => ColourF.Maroon,
                "Brown" => ColourF.Brown,
                "SaddleBrown" => ColourF.SaddleBrown,
                "Sienna" => ColourF.Sienna,
                "Chocolate" => ColourF.Chocolate,
                "DarkGoldenrod" => ColourF.DarkGoldenrod,
                "Peru" => ColourF.Peru,
                "RosyBrown" => ColourF.RosyBrown,
                "Goldenrod" => ColourF.Goldenrod,
                "SandyBrown" => ColourF.SandyBrown,
                "Tan" => ColourF.Tan,
                "Burlywood" => ColourF.Burlywood,
                "Wheat" => ColourF.Wheat,
                "NavajoWhite" => ColourF.NavajoWhite,
                "Bisque" => ColourF.Bisque,
                "BlanchedAlmond" => ColourF.BlanchedAlmond,
                "Cornsilk" => ColourF.Cornsilk,

                "Indigo" => ColourF.Indigo,
                "Purple" => ColourF.Purple,
                "DarkMagenta" => ColourF.DarkMagenta,
                "DarkViolet" => ColourF.DarkViolet,
                "DarkSlateBlue" => ColourF.DarkSlateBlue,
                "BlueViolet" => ColourF.BlueViolet,
                "DarkOrchid" => ColourF.DarkOrchid,
                "Fuchsia" => ColourF.Fuchsia,
                "Magenta" => ColourF.Magenta,
                "SlateBlue" => ColourF.SlateBlue,
                "MediumSlateBlue" => ColourF.MediumSlateBlue,
                "MediumOrchid" => ColourF.MediumOrchid,
                "MediumPurple" => ColourF.MediumPurple,
                "Orchid" => ColourF.Orchid,
                "Violet" => ColourF.Violet,
                "Plum" => ColourF.Plum,
                "Thistle" => ColourF.Thistle,
                "Lavender" => ColourF.Lavender,

                "MidnightBlue" => ColourF.MidnightBlue,
                "Navy" => ColourF.Navy,
                "DarkBlue" => ColourF.DarkBlue,
                "MediumBlue" => ColourF.MediumBlue,
                "Blue" => ColourF.Blue,
                "RoyalBlue" => ColourF.RoyalBlue,
                "SteelBlue" => ColourF.SteelBlue,
                "DodgerBlue" => ColourF.DodgerBlue,
                "DeepSkyBlue" => ColourF.DeepSkyBlue,
                "CornflowerBlue" => ColourF.CornflowerBlue,
                "SkyBlue" => ColourF.SkyBlue,
                "LightSkyBlue" => ColourF.LightSkyBlue,
                "LightSteelBlue" => ColourF.LightSteelBlue,
                "LightBlue" => ColourF.LightBlue,
                "PowderBlue" => ColourF.PowderBlue,

                "Teal" => ColourF.Teal,
                "DarkCyan" => ColourF.DarkCyan,
                "LightSeaGreen" => ColourF.LightSeaGreen,
                "CadetBlue" => ColourF.CadetBlue,
                "DarkTurquoise" => ColourF.DarkTurquoise,
                "MediumTurquoise" => ColourF.MediumTurquoise,
                "Turquoise" => ColourF.Turquoise,
                "Aqua" => ColourF.Aqua,
                "Cyan" => ColourF.Cyan,
                "Aquamarine" => ColourF.Aquamarine,
                "PaleTurquoise" => ColourF.PaleTurquoise,
                "LightCyan" => ColourF.LightCyan,

                "DarkGreen" => ColourF.DarkGreen,
                "Green" => ColourF.Green,
                "DarkOliveGreen" => ColourF.DarkOliveGreen,
                "ForestGreen" => ColourF.ForestGreen,
                "SeaGreen" => ColourF.SeaGreen,
                "Olive" => ColourF.Olive,
                "OliveDrab" => ColourF.OliveDrab,
                "MediumSeaGreen" => ColourF.MediumSeaGreen,
                "LimeGreen" => ColourF.LimeGreen,
                "Lime" => ColourF.Lime,
                "SpringGreen" => ColourF.SpringGreen,
                "MediumSpringGreen" => ColourF.MediumSpringGreen,
                "DarkSeaGreen" => ColourF.DarkSeaGreen,
                "MediumAquamarine" => ColourF.MediumAquamarine,
                "YellowGreen" => ColourF.YellowGreen,
                "LawnGreen" => ColourF.LawnGreen,
                "Chartreuse" => ColourF.Chartreuse,
                "LightGreen" => ColourF.LightGreen,
                "GreenYellow" => ColourF.GreenYellow,
                "PaleGreen" => ColourF.PaleGreen,

                "MistyRose" => ColourF.MistyRose,
                "AntiqueWhite" => ColourF.AntiqueWhite,
                "Linen" => ColourF.Linen,
                "Beige" => ColourF.Beige,
                "WhiteSmoke" => ColourF.WhiteSmoke,
                "LavenderBlush" => ColourF.LavenderBlush,
                "OldLace" => ColourF.OldLace,
                "AliceBlue" => ColourF.AliceBlue,
                "Seashell" => ColourF.Seashell,
                "GhostWhite" => ColourF.GhostWhite,
                "Honeydew" => ColourF.Honeydew,
                "FloralWhite" => ColourF.FloralWhite,
                "Azure" => ColourF.Azure,
                "MintCream" => ColourF.MintCream,
                "Snow" => ColourF.Snow,
                "Ivory" => ColourF.Ivory,
                "White" => ColourF.White,

                "Black" => ColourF.Black,
                "DarkSlateGrey" => ColourF.DarkSlateGrey,
                "DimGrey" => ColourF.DimGrey,
                "SlateGrey" => ColourF.SlateGrey,
                "DarkGrey" => ColourF.DarkGrey,
                "LightSlateGrey" => ColourF.LightSlateGrey,
                "Grey" => ColourF.Grey,
                "Silver" => ColourF.Silver,
                "LightGrey" => ColourF.LightGrey,
                "Gainsboro" => ColourF.Gainsboro,
                _ => throw new Exception("Invalid ColourF syntax")
            };
        }

        public static Colour3 ColourParser3(string str)
        {
            return str switch
            {
                "MediumVioletRed" => Colour3.MediumVioletRed,
                "DeepPink" => Colour3.DeepPink,
                "PaleVioletRed" => Colour3.PaleVioletRed,
                "HotPink" => Colour3.HotPink,
                "LightPink" => Colour3.LightPink,
                "Pink" => Colour3.Pink,

                "DarkRed" => Colour3.DarkRed,
                "Red" => Colour3.Red,
                "Firebrick" => Colour3.Firebrick,
                "Crimson" => Colour3.Crimson,
                "IndianRed" => Colour3.IndianRed,
                "LightCoral" => Colour3.LightCoral,
                "Salmon" => Colour3.Salmon,
                "DarkSalmon" => Colour3.DarkSalmon,
                "LightSalmon" => Colour3.LightSalmon,

                "OrangeRed" => Colour3.OrangeRed,
                "Tomato" => Colour3.Tomato,
                "DarkOrange" => Colour3.DarkOrange,
                "Coral" => Colour3.Coral,
                "Orange" => Colour3.Orange,

                "DarkKhaki" => Colour3.DarkKhaki,
                "Gold" => Colour3.Gold,
                "Khaki" => Colour3.Khaki,
                "PeachPuff" => Colour3.PeachPuff,
                "Yellow" => Colour3.Yellow,
                "PaleGoldenrod" => Colour3.PaleGoldenrod,
                "Moccasin" => Colour3.Moccasin,
                "PapayaWhip" => Colour3.PapayaWhip,
                "LightGoldenrodYellow" => Colour3.LightGoldenrodYellow,
                "LemonChiffon" => Colour3.LemonChiffon,
                "LightYellow" => Colour3.LightYellow,

                "Maroon" => Colour3.Maroon,
                "Brown" => Colour3.Brown,
                "SaddleBrown" => Colour3.SaddleBrown,
                "Sienna" => Colour3.Sienna,
                "Chocolate" => Colour3.Chocolate,
                "DarkGoldenrod" => Colour3.DarkGoldenrod,
                "Peru" => Colour3.Peru,
                "RosyBrown" => Colour3.RosyBrown,
                "Goldenrod" => Colour3.Goldenrod,
                "SandyBrown" => Colour3.SandyBrown,
                "Tan" => Colour3.Tan,
                "Burlywood" => Colour3.Burlywood,
                "Wheat" => Colour3.Wheat,
                "NavajoWhite" => Colour3.NavajoWhite,
                "Bisque" => Colour3.Bisque,
                "BlanchedAlmond" => Colour3.BlanchedAlmond,
                "Cornsilk" => Colour3.Cornsilk,

                "Indigo" => Colour3.Indigo,
                "Purple" => Colour3.Purple,
                "DarkMagenta" => Colour3.DarkMagenta,
                "DarkViolet" => Colour3.DarkViolet,
                "DarkSlateBlue" => Colour3.DarkSlateBlue,
                "BlueViolet" => Colour3.BlueViolet,
                "DarkOrchid" => Colour3.DarkOrchid,
                "Fuchsia" => Colour3.Fuchsia,
                "Magenta" => Colour3.Magenta,
                "SlateBlue" => Colour3.SlateBlue,
                "MediumSlateBlue" => Colour3.MediumSlateBlue,
                "MediumOrchid" => Colour3.MediumOrchid,
                "MediumPurple" => Colour3.MediumPurple,
                "Orchid" => Colour3.Orchid,
                "Violet" => Colour3.Violet,
                "Plum" => Colour3.Plum,
                "Thistle" => Colour3.Thistle,
                "Lavender" => Colour3.Lavender,

                "MidnightBlue" => Colour3.MidnightBlue,
                "Navy" => Colour3.Navy,
                "DarkBlue" => Colour3.DarkBlue,
                "MediumBlue" => Colour3.MediumBlue,
                "Blue" => Colour3.Blue,
                "RoyalBlue" => Colour3.RoyalBlue,
                "SteelBlue" => Colour3.SteelBlue,
                "DodgerBlue" => Colour3.DodgerBlue,
                "DeepSkyBlue" => Colour3.DeepSkyBlue,
                "CornflowerBlue" => Colour3.CornflowerBlue,
                "SkyBlue" => Colour3.SkyBlue,
                "LightSkyBlue" => Colour3.LightSkyBlue,
                "LightSteelBlue" => Colour3.LightSteelBlue,
                "LightBlue" => Colour3.LightBlue,
                "PowderBlue" => Colour3.PowderBlue,

                "Teal" => Colour3.Teal,
                "DarkCyan" => Colour3.DarkCyan,
                "LightSeaGreen" => Colour3.LightSeaGreen,
                "CadetBlue" => Colour3.CadetBlue,
                "DarkTurquoise" => Colour3.DarkTurquoise,
                "MediumTurquoise" => Colour3.MediumTurquoise,
                "Turquoise" => Colour3.Turquoise,
                "Aqua" => Colour3.Aqua,
                "Cyan" => Colour3.Cyan,
                "Aquamarine" => Colour3.Aquamarine,
                "PaleTurquoise" => Colour3.PaleTurquoise,
                "LightCyan" => Colour3.LightCyan,

                "DarkGreen" => Colour3.DarkGreen,
                "Green" => Colour3.Green,
                "DarkOliveGreen" => Colour3.DarkOliveGreen,
                "ForestGreen" => Colour3.ForestGreen,
                "SeaGreen" => Colour3.SeaGreen,
                "Olive" => Colour3.Olive,
                "OliveDrab" => Colour3.OliveDrab,
                "MediumSeaGreen" => Colour3.MediumSeaGreen,
                "LimeGreen" => Colour3.LimeGreen,
                "Lime" => Colour3.Lime,
                "SpringGreen" => Colour3.SpringGreen,
                "MediumSpringGreen" => Colour3.MediumSpringGreen,
                "DarkSeaGreen" => Colour3.DarkSeaGreen,
                "MediumAquamarine" => Colour3.MediumAquamarine,
                "YellowGreen" => Colour3.YellowGreen,
                "LawnGreen" => Colour3.LawnGreen,
                "Chartreuse" => Colour3.Chartreuse,
                "LightGreen" => Colour3.LightGreen,
                "GreenYellow" => Colour3.GreenYellow,
                "PaleGreen" => Colour3.PaleGreen,

                "MistyRose" => Colour3.MistyRose,
                "AntiqueWhite" => Colour3.AntiqueWhite,
                "Linen" => Colour3.Linen,
                "Beige" => Colour3.Beige,
                "WhiteSmoke" => Colour3.WhiteSmoke,
                "LavenderBlush" => Colour3.LavenderBlush,
                "OldLace" => Colour3.OldLace,
                "AliceBlue" => Colour3.AliceBlue,
                "Seashell" => Colour3.Seashell,
                "GhostWhite" => Colour3.GhostWhite,
                "Honeydew" => Colour3.Honeydew,
                "FloralWhite" => Colour3.FloralWhite,
                "Azure" => Colour3.Azure,
                "MintCream" => Colour3.MintCream,
                "Snow" => Colour3.Snow,
                "Ivory" => Colour3.Ivory,
                "White" => Colour3.White,

                "Black" => Colour3.Black,
                "DarkSlateGrey" => Colour3.DarkSlateGrey,
                "DimGrey" => Colour3.DimGrey,
                "SlateGrey" => Colour3.SlateGrey,
                "DarkGrey" => Colour3.DarkGrey,
                "LightSlateGrey" => Colour3.LightSlateGrey,
                "Grey" => Colour3.Grey,
                "Silver" => Colour3.Silver,
                "LightGrey" => Colour3.LightGrey,
                "Gainsboro" => Colour3.Gainsboro,
                _ => throw new Exception("Invalid Colour3 syntax")
            };
        }
        public static ColourF3 ColourParserF3(string str)
        {
            return str switch
            {
                "MediumVioletRed" => ColourF3.MediumVioletRed,
                "DeepPink" => ColourF3.DeepPink,
                "PaleVioletRed" => ColourF3.PaleVioletRed,
                "HotPink" => ColourF3.HotPink,
                "LightPink" => ColourF3.LightPink,
                "Pink" => ColourF3.Pink,

                "DarkRed" => ColourF3.DarkRed,
                "Red" => ColourF3.Red,
                "Firebrick" => ColourF3.Firebrick,
                "Crimson" => ColourF3.Crimson,
                "IndianRed" => ColourF3.IndianRed,
                "LightCoral" => ColourF3.LightCoral,
                "Salmon" => ColourF3.Salmon,
                "DarkSalmon" => ColourF3.DarkSalmon,
                "LightSalmon" => ColourF3.LightSalmon,

                "OrangeRed" => ColourF3.OrangeRed,
                "Tomato" => ColourF3.Tomato,
                "DarkOrange" => ColourF3.DarkOrange,
                "Coral" => ColourF3.Coral,
                "Orange" => ColourF3.Orange,

                "DarkKhaki" => ColourF3.DarkKhaki,
                "Gold" => ColourF3.Gold,
                "Khaki" => ColourF3.Khaki,
                "PeachPuff" => ColourF3.PeachPuff,
                "Yellow" => ColourF3.Yellow,
                "PaleGoldenrod" => ColourF3.PaleGoldenrod,
                "Moccasin" => ColourF3.Moccasin,
                "PapayaWhip" => ColourF3.PapayaWhip,
                "LightGoldenrodYellow" => ColourF3.LightGoldenrodYellow,
                "LemonChiffon" => ColourF3.LemonChiffon,
                "LightYellow" => ColourF3.LightYellow,

                "Maroon" => ColourF3.Maroon,
                "Brown" => ColourF3.Brown,
                "SaddleBrown" => ColourF3.SaddleBrown,
                "Sienna" => ColourF3.Sienna,
                "Chocolate" => ColourF3.Chocolate,
                "DarkGoldenrod" => ColourF3.DarkGoldenrod,
                "Peru" => ColourF3.Peru,
                "RosyBrown" => ColourF3.RosyBrown,
                "Goldenrod" => ColourF3.Goldenrod,
                "SandyBrown" => ColourF3.SandyBrown,
                "Tan" => ColourF3.Tan,
                "Burlywood" => ColourF3.Burlywood,
                "Wheat" => ColourF3.Wheat,
                "NavajoWhite" => ColourF3.NavajoWhite,
                "Bisque" => ColourF3.Bisque,
                "BlanchedAlmond" => ColourF3.BlanchedAlmond,
                "Cornsilk" => ColourF3.Cornsilk,

                "Indigo" => ColourF3.Indigo,
                "Purple" => ColourF3.Purple,
                "DarkMagenta" => ColourF3.DarkMagenta,
                "DarkViolet" => ColourF3.DarkViolet,
                "DarkSlateBlue" => ColourF3.DarkSlateBlue,
                "BlueViolet" => ColourF3.BlueViolet,
                "DarkOrchid" => ColourF3.DarkOrchid,
                "Fuchsia" => ColourF3.Fuchsia,
                "Magenta" => ColourF3.Magenta,
                "SlateBlue" => ColourF3.SlateBlue,
                "MediumSlateBlue" => ColourF3.MediumSlateBlue,
                "MediumOrchid" => ColourF3.MediumOrchid,
                "MediumPurple" => ColourF3.MediumPurple,
                "Orchid" => ColourF3.Orchid,
                "Violet" => ColourF3.Violet,
                "Plum" => ColourF3.Plum,
                "Thistle" => ColourF3.Thistle,
                "Lavender" => ColourF3.Lavender,

                "MidnightBlue" => ColourF3.MidnightBlue,
                "Navy" => ColourF3.Navy,
                "DarkBlue" => ColourF3.DarkBlue,
                "MediumBlue" => ColourF3.MediumBlue,
                "Blue" => ColourF3.Blue,
                "RoyalBlue" => ColourF3.RoyalBlue,
                "SteelBlue" => ColourF3.SteelBlue,
                "DodgerBlue" => ColourF3.DodgerBlue,
                "DeepSkyBlue" => ColourF3.DeepSkyBlue,
                "CornflowerBlue" => ColourF3.CornflowerBlue,
                "SkyBlue" => ColourF3.SkyBlue,
                "LightSkyBlue" => ColourF3.LightSkyBlue,
                "LightSteelBlue" => ColourF3.LightSteelBlue,
                "LightBlue" => ColourF3.LightBlue,
                "PowderBlue" => ColourF3.PowderBlue,

                "Teal" => ColourF3.Teal,
                "DarkCyan" => ColourF3.DarkCyan,
                "LightSeaGreen" => ColourF3.LightSeaGreen,
                "CadetBlue" => ColourF3.CadetBlue,
                "DarkTurquoise" => ColourF3.DarkTurquoise,
                "MediumTurquoise" => ColourF3.MediumTurquoise,
                "Turquoise" => ColourF3.Turquoise,
                "Aqua" => ColourF3.Aqua,
                "Cyan" => ColourF3.Cyan,
                "Aquamarine" => ColourF3.Aquamarine,
                "PaleTurquoise" => ColourF3.PaleTurquoise,
                "LightCyan" => ColourF3.LightCyan,

                "DarkGreen" => ColourF3.DarkGreen,
                "Green" => ColourF3.Green,
                "DarkOliveGreen" => ColourF3.DarkOliveGreen,
                "ForestGreen" => ColourF3.ForestGreen,
                "SeaGreen" => ColourF3.SeaGreen,
                "Olive" => ColourF3.Olive,
                "OliveDrab" => ColourF3.OliveDrab,
                "MediumSeaGreen" => ColourF3.MediumSeaGreen,
                "LimeGreen" => ColourF3.LimeGreen,
                "Lime" => ColourF3.Lime,
                "SpringGreen" => ColourF3.SpringGreen,
                "MediumSpringGreen" => ColourF3.MediumSpringGreen,
                "DarkSeaGreen" => ColourF3.DarkSeaGreen,
                "MediumAquamarine" => ColourF3.MediumAquamarine,
                "YellowGreen" => ColourF3.YellowGreen,
                "LawnGreen" => ColourF3.LawnGreen,
                "Chartreuse" => ColourF3.Chartreuse,
                "LightGreen" => ColourF3.LightGreen,
                "GreenYellow" => ColourF3.GreenYellow,
                "PaleGreen" => ColourF3.PaleGreen,

                "MistyRose" => ColourF3.MistyRose,
                "AntiqueWhite" => ColourF3.AntiqueWhite,
                "Linen" => ColourF3.Linen,
                "Beige" => ColourF3.Beige,
                "WhiteSmoke" => ColourF3.WhiteSmoke,
                "LavenderBlush" => ColourF3.LavenderBlush,
                "OldLace" => ColourF3.OldLace,
                "AliceBlue" => ColourF3.AliceBlue,
                "Seashell" => ColourF3.Seashell,
                "GhostWhite" => ColourF3.GhostWhite,
                "Honeydew" => ColourF3.Honeydew,
                "FloralWhite" => ColourF3.FloralWhite,
                "Azure" => ColourF3.Azure,
                "MintCream" => ColourF3.MintCream,
                "Snow" => ColourF3.Snow,
                "Ivory" => ColourF3.Ivory,
                "White" => ColourF3.White,

                "Black" => ColourF3.Black,
                "DarkSlateGrey" => ColourF3.DarkSlateGrey,
                "DimGrey" => ColourF3.DimGrey,
                "SlateGrey" => ColourF3.SlateGrey,
                "DarkGrey" => ColourF3.DarkGrey,
                "LightSlateGrey" => ColourF3.LightSlateGrey,
                "Grey" => ColourF3.Grey,
                "Silver" => ColourF3.Silver,
                "LightGrey" => ColourF3.LightGrey,
                "Gainsboro" => ColourF3.Gainsboro,
                _ => throw new Exception("Invalid ColourF3 syntax")
            };
        }

        public static Vector2 Vector2Parser(string str)
        {
            str = str.Trim();

            if (str[0] != '{' || str[^1] != '}')
            {
                throw new Exception("Invlaid Vector2 syntax.");
            }

            str = str[1..^1];

            double x = double.Parse(str.Remove(str.IndexOf(',')));
            double y = double.Parse(str.Remove(0, str.IndexOf(',') + 1));

            return new Vector2(x, y);
        }
        public static Vector2I Vector2IParser(string str)
        {
            str = str.Trim();

            if (str[0] != '{' || str[^1] != '}')
            {
                throw new Exception("Invlaid Vector2I syntax.");
            }

            str = str[1..^1];

            int x = int.Parse(str.Remove(str.IndexOf(',')));
            int y = int.Parse(str.Remove(0, str.IndexOf(',') + 1));

            return new Vector2I(x, y);
        }
        public static Vector3 Vector3Parser(string str)
        {
            str = str.Trim();

            if (str[0] != '{' || str[^1] != '}')
            {
                throw new Exception("Invlaid Vector3 syntax.");
            }

            str = str[1..^1];

            double x = double.Parse(str.Remove(str.IndexOf(',')));
            str = str.Remove(0, str.IndexOf(',') + 1);
            double y = double.Parse(str.Remove(str.IndexOf(',')));
            double z = double.Parse(str.Remove(0, str.IndexOf(',') + 1));

            return new Vector3(x, y, z);
        }
        public static Vector3I Vector3IParser(string str)
        {
            str = str.Trim();

            if (str[0] != '{' || str[^1] != '}')
            {
                throw new Exception("Invlaid Vector3I syntax.");
            }

            str = str[1..^1];

            int x = int.Parse(str.Remove(str.IndexOf(',')));
            str = str.Remove(0, str.IndexOf(',') + 1);
            int y = int.Parse(str.Remove(str.IndexOf(',')));
            int z = int.Parse(str.Remove(0, str.IndexOf(',') + 1));

            return new Vector3I(x, y, z);
        }
        public static Vector4 Vector4Parser(string str)
        {
            str = str.Trim();

            if (str[0] != '{' || str[^1] != '}')
            {
                throw new Exception("Invlaid Vector4 syntax.");
            }

            str = str[1..^1];

            double x = double.Parse(str.Remove(str.IndexOf(',')));
            str = str.Remove(0, str.IndexOf(',') + 1);
            double y = double.Parse(str.Remove(str.IndexOf(',')));
            str = str.Remove(0, str.IndexOf(',') + 1);
            double z = double.Parse(str.Remove(str.IndexOf(',')));
            double w = double.Parse(str.Remove(0, str.IndexOf(',') + 1));

            return new Vector4(x, y, z, w);
        }
        public static Vector4I Vector4IParser(string str)
        {
            str = str.Trim();

            if (str[0] != '{' || str[^1] != '}')
            {
                throw new Exception("Invlaid Vector4I syntax.");
            }

            str = str[1..^1];

            int x = int.Parse(str.Remove(str.IndexOf(',')));
            str = str.Remove(0, str.IndexOf(',') + 1);
            int y = int.Parse(str.Remove(str.IndexOf(',')));
            str = str.Remove(0, str.IndexOf(',') + 1);
            int z = int.Parse(str.Remove(str.IndexOf(',')));
            int w = int.Parse(str.Remove(0, str.IndexOf(',') + 1));

            return new Vector4I(x, y, z, w);
        }
    }
}
