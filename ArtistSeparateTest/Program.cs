using System;
using System.Collections.Generic;
using System.IO;

namespace ArtistSeparateTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string songname = "Nathan Dawe, Ella Henderson, ATB, A7S";

            var artists = new List<string>();
            artists.AddRange(songname.Split(new string[] { ", " }, StringSplitOptions.None));
            artists.ForEach(x => x = x.Trim().TrimStart().TrimEnd());

            foreach (var artist in artists)
            {
                Console.WriteLine(artist);
            }
        }
    }
}
