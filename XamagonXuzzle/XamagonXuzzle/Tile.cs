using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace XamagonXuzzle
{
	public class Tile
	{
        //const string UrlPrefix = "http://xamarin.github.io/xamarin-forms-book-samples/XamagonXuzzle/";

        const string UrlPrefijo = "https://raw.githubusercontent.com/jotalexvalencia/XamarinUnad/master/XamUnad/XamUnad/img/"; 

        // Url prefijo con las nuevas imagenes 

        public Tile(int row, int col)
        {
            Row = row;
            Col = col;

            TileView = new ContentView
            {
                Padding = new Thickness(1),

                // Get the bitmap for each tile 
                Content = new Image
                {
                    //Source = ImageSource.FromUri(new Uri(UrlPrefix + "Bitmap" + row + col + ".png"))

                    Source = ImageSource.FromUri(new Uri(UrlPrefijo + "fila-" + row + "-col-"+ col + ".png"))

                    //fila-3-col-2
                }
            };

            // Add TileView to dictionary for obtaining Tile from TileView
            Dictionary.Add(TileView, this);
        }

        public static Dictionary<View, Tile> Dictionary { get; } = new Dictionary<View, Tile>();

        public int Row { set; get; }

        public int Col { set; get; }

        public View TileView { private set; get; }
    }
}