using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;
using XamagonXuzzle.Droid;

//excelente
namespace XamagonXuzzle
{
    public partial class MainPage : ContentPage
    {
        // Number of tiles horizontally and vertically,
        //  fixed based on available bitmaps
        static readonly int NUM = 4;

        // Array of tiles
        public Tile[,] tiles = new Tile[NUM, NUM];
        // Array to compare and determine the winner
        public Tile[,] pieza = new Tile[NUM, NUM];

        // Empty row and column
        int emptyRow = NUM - 1;
        int emptyCol = NUM - 1;
        int contador = 0;
        double tileSize;
        bool isBusy;

        public MainPage()
        {
            InitializeComponent();

            AnimationLoop();


            // Loop through the rows and columns.
            for (int row = 0; row < NUM; row++)
            {
                for (int col = 0; col < NUM; col++)
                {
                    // But skip the last one!
                    if (row == NUM - 1 && col == NUM - 1)
                        break;

                    // Create the tile
                    Tile tile = new Tile(row, col);



                    // Add tap recognition.
                    TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += OnTileTapped;
                    tile.TileView.GestureRecognizers.Add(tapGestureRecognizer);



                    // Add the tile to the array and the AbsoluteLayout.
                    tiles[row, col] = tile;
                    pieza[row, col] = tile;
                    absoluteLayout.Children.Add(tile.TileView);




                }
            }
        }



        void OnContainerSizeChanged(object sender, EventArgs args)
        {
            View container = (View)sender;
            double width = container.Width;
            double height = container.Height;

            if (width <= 0 || height <= 0)
                return;

            // Orient StackLayout based on portrait/landscape mode.
            stackLayout.Orientation = (width < height) ? StackOrientation.Vertical :
                                                         StackOrientation.Horizontal;

            // Calculate tile size and position based on ContentView size.
            tileSize = Math.Min(width, height) / NUM;
            absoluteLayout.WidthRequest = NUM * tileSize;
            absoluteLayout.HeightRequest = NUM * tileSize;

            foreach (View fileView in absoluteLayout.Children)
            {
                Tile tile = Tile.Dictionary[fileView];

                // Set tile bounds.
                AbsoluteLayout.SetLayoutBounds(fileView, new Rectangle(tile.Col * tileSize,
                                                                       tile.Row * tileSize,
                                                                       tileSize,
                                                                       tileSize));
            }
        }

        async void AnimationLoop()
        {
            uint duration = 5 * 60 * 1000;
            // 5 minutes while (true) 
            {
                await Task.WhenAll(
                    image.RotateTo(307 * 360, duration),
                    image.RotateXTo(251 * 360, duration),
                    image.RotateYTo(199 * 360, duration));
                image.Rotation = 0;
                image.RotationX = 0; image.RotationY = 0;
            }
        }

        async void OnTileTapped(object sender, EventArgs args)
        {


            if (isBusy)
                return;

            isBusy = true;

            View tileView = (View)sender;
            Tile tappedTile = Tile.Dictionary[tileView];

            await ShiftIntoEmpty(tappedTile.Row, tappedTile.Col);
            isBusy = false;


        }

        async Task ShiftIntoEmpty(int tappedRow, int tappedCol, uint length = 100)
        {
            // Shift columns.
            if (tappedRow == emptyRow && tappedCol != emptyCol)
            {
                int inc = Math.Sign(tappedCol - emptyCol);
                int begCol = emptyCol + inc;
                int endCol = tappedCol + inc;

                for (int col = begCol; col != endCol; col += inc)
                {
                    await AnimateTile(emptyRow, col, emptyRow, emptyCol, length);
                }
            }
            // Shift rows.
            else if (tappedCol == emptyCol && tappedRow != emptyRow)
            {
                int inc = Math.Sign(tappedRow - emptyRow);
                int begRow = emptyRow + inc;
                int endRow = tappedRow + inc;

                for (int row = begRow; row != endRow; row += inc)
                {
                    await AnimateTile(row, emptyCol, emptyRow, emptyCol, length);
                }
            }








        }

        async Task AnimateTile(int row, int col, int newRow, int newCol, uint length)
        {
            // The tile to be animated.
            Tile tile = tiles[row, col];
            View tileView = tile.TileView;

            // The destination rectangle.
            Rectangle rect = new Rectangle(emptyCol * tileSize,
                                           emptyRow * tileSize,
                                           tileSize,
                                           tileSize);

            // Animate it!
            await tileView.LayoutTo(rect, length);

            // Set layout bounds to same Rectangle.
            AbsoluteLayout.SetLayoutBounds(tileView, rect);

            // Set several variables and properties for new layout.
            tiles[newRow, newCol] = tile;
            tile.Row = newRow;
            tile.Col = newCol;
            tiles[row, col] = null;
            emptyRow = row;
            emptyCol = col;

            #region ganador

            if (emptyRow == 3 && emptyCol == 3)
            {
                for (int i = 0; i < NUM; i++)
                {
                    for (int j = 0; j < NUM; j++)
                    {
                        if (i == NUM - 1 && j == NUM - 1)
                            break;
                        if (tiles[i,j].TileView.Id==pieza[i,j].TileView.Id)
                        {
                            contador++;

                            if (contador == 15)
                            {
                                await DisplayAlert("Felicitaciones", "ganaste", "Ok");
                                break;

                            }
                        }
                        else
                        {
                            contador = 0;
                        }

                        
                        
                    }

                }

                
            }
            #endregion

        }

        async void OnRandomizeButtonClicked(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;
            Random rand = new Random();

            isBusy = true;

            // Simulate some fast crazy taps.
            for (int i = 0; i < 100; i++)
            {
                await ShiftIntoEmpty(rand.Next(NUM), emptyCol, 25);
                await ShiftIntoEmpty(emptyRow, rand.Next(NUM), 25);
            }
            button.IsEnabled = true;

            isBusy = false;
        }


    }
}