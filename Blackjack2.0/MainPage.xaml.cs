using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Permissions;

namespace Blackjack2._0
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            foreach (string s in strategies)
            {
                StrategyList1.Items.Add(s);
                StrategyList1.SelectedIndex = 0;
            }
        }

        //declare variables used throughout the program
        List<string> strategies = new List<string>()
            {
                "Stay on all 17",
                "Hit on soft 17",
                "Standard Play",
                "Count Cards #1"
            };
        int startupPlayers = 1;
        int startupDecks = 3;

        //deck variables
        int TheCount = 0;
        List<Card> deck = new List<Card>();
        int[] values = new int[52] { 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10,
                                     11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 
                                     11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 
                                     11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };

        string[] names = new string[52] {"AS", "2S", "3S", "4S", "5S", "6S", "7S", "8S", "9S", "0S", "JS", "QS", "KS",
                                         "AC", "2C", "3C", "4C", "5C", "6C", "7C", "8C", "9C", "0C", "JC", "QC", "KC",
                                         "AD", "2D", "3D", "4D", "5D", "6D", "7D", "8D", "9D", "0D", "JD", "QD", "KD",
                                         "AH", "2H", "3H", "4H", "5H", "6H", "7H", "8H", "9H", "0H", "JH", "QH", "KH"};

        //"Hi-Lo" count method
        int[] CountMethod1 = new int[52] {-1, 1, 1, 1, 1, 1, 0, 0, 0, -1, -1, -1, -1,
                                          -1, 1, 1, 1, 1, 1, 0, 0, 0, -1, -1, -1, -1,
                                          -1, 1, 1, 1, 1, 1, 0, 0, 0, -1, -1, -1, -1,
                                          -1, 1, 1, 1, 1, 1, 0, 0, 0, -1, -1, -1, -1 };

        //player variables
        List<Player> players = new List<Player>();
        List<Player> splits = new List<Player>();
        int TurnPosition = 0;
        bool splitting = false;

        //StandardPlay array
        #region
        //                                         Dealer showing:
        //                                      {0,A,2,3,4,5,6,7,8,9}
        int[,] StandardPlay = new int[35, 10] { {1,1,1,1,1,1,1,1,1,1},  //Total of 5
                                                {1,1,1,1,1,1,1,1,1,1},  //Total of 6
                                                {1,1,1,1,1,1,1,1,1,1},  //Total of 7
                                                {1,1,1,1,1,1,1,1,1,1},  //Total of 8
                                                {1,1,1,3,3,3,3,1,1,1},  //Total of 9
                                                {1,1,3,3,3,3,3,3,3,3},  //Total of 10
                                                {3,1,3,3,3,3,3,3,3,3},  //Total of 11
                                                {1,1,1,1,2,2,2,1,1,1},  //Total of 12
                                                {1,1,2,2,2,2,2,1,1,1},  //Total of 13
                                                {1,1,2,2,2,2,2,1,1,1},  //Total of 14
                                                {1,1,2,2,2,2,2,1,1,1},  //Total of 15
                                                {1,1,2,2,2,2,2,1,1,1},  //Total of 16
                                                {2,2,2,2,2,2,2,2,2,2},  //Total of 17
                                                {2,2,2,2,2,2,2,2,2,2},  //Total of 18
                                                {2,2,2,2,2,2,2,2,2,2},  //Total of 19
                                                {2,2,2,2,2,2,2,2,2,2},  //Total of 20
                                                {2,2,2,2,2,2,2,2,2,2},  //Total of 21
                                                {1,1,1,1,1,3,3,1,1,1},  //Total of 13 with an Ace
                                                {1,1,1,1,1,3,3,1,1,1},  //Total of 14 with an Ace  
                                                {1,1,1,1,3,3,3,1,1,1},  //Total of 15 with an Ace
                                                {1,1,1,1,3,3,3,1,1,1},  //Total of 16 with an Ace
                                                {1,1,1,3,3,3,3,1,1,1},  //Total of 17 with an Ace
                                                {1,1,2,3,3,3,3,2,2,1},  //Total of 18 with an Ace
                                                {2,2,2,2,2,2,2,2,2,2},  //Total of 19 with an Ace
                                                {2,2,2,2,2,2,2,2,2,2},  //Total of 20 with an Ace
                                                {2,2,2,2,2,2,2,2,2,2},  //Total of 21 with an Ace
                                                {1,1,4,4,4,4,4,4,1,1},  //Double 2's
                                                {1,1,4,4,4,4,4,4,1,1},  //Double 3's
                                                {1,1,1,1,1,4,4,1,1,1},  //Double 4's
                                                {1,1,3,3,3,3,3,3,3,3},  //Double 5's
                                                {1,1,4,4,4,4,4,1,1,1},  //Double 6's
                                                {1,1,4,4,4,4,4,4,1,1},  //Double 7's
                                                {4,4,4,4,4,4,4,4,4,4},  //Double 8's
                                                {2,2,4,4,4,4,4,2,4,4},  //Double 9's
                                                {4,4,4,4,4,4,4,4,4,4}   //Double Aces
                                                };
        #endregion 

        //Storage area for data storage from runs 
        IsolatedStorageFile RunDataStorage = IsolatedStorageFile.GetUserStoreForApplication();
        int handCount = 0;
     
        //CUSTOM CLASSES START HERE
        public class Card
        {
            public int ScoreValue { get; set; }
            public string name { get; set; }
            public int CountValue { get; set; }

            public Card()
            {
                ScoreValue = 0;
                name = "";
                CountValue = 0;
            }

            public Card(int s, string n, int c)
            {
                ScoreValue = s;
                name = n;
                CountValue = c;
            }
        }

        public class Player
        {
            public int score { get; set; }
            public double money { get; set; }
            public int aceCount { get; set; }
            public bool bust { get; set; }
            public List<Card> cards { get; set; }
            public Grid playArea { get; set; }
            public List<Rectangle> cardImages { get; set; }
            public TextBox betBox { get; set; }
            public TextBlock moneyBlock { get; set; }
            public TextBlock scoreBlock { get; set; }
            public bool active { get; set; }
            public int automation {get; set;}

            public Player()
            {
                score = 0;
                money = 0;
                aceCount = 0;
                bust = false;
                cards = new List<Card>();
                playArea = new Grid();
                cardImages = new List<Rectangle>();
                betBox = new TextBox();
                moneyBlock = new TextBlock();
                scoreBlock = new TextBlock();
                active = true;
                automation = 0;
            }

            public Player(double m)
            {
                score = 0;
                money = m;
                aceCount = 0;
                bust = false;
                cards = new List<Card>();
                playArea = new Grid();
                cardImages = new List<Rectangle>();
                betBox = new TextBox();
                moneyBlock = new TextBlock();
                scoreBlock = new TextBlock();
                active = true;
                automation = 0;
            }
        }

        //CUSTOM FUNCTIONS START HERE
        //This function adds complete decks equal to the number chosen and shuffles all of the cards
        private void Shuffle()
        {
            deck.Clear();
            TheCount = 0;
            TheCountBlock.Text = "0";
            for (int j = 0; j <= Convert.ToInt32(NumberOfDecks.Text); j++)
            {
                for (int i = 0; i < 52; i++)
                {
                    deck.Add(new Card(values[i], names[i], CountMethod1[i]));
                }
            }

            //Knuth shuffle, very important!
            var _random = new Random();
            for (int i = deck.Count(); i > 0; i--)
            {
                int n = _random.Next(i);
                Card tmp = deck[n];
                deck[n] = deck[i - 1];
                deck[i - 1] = tmp;
            }

            //For debugging, specify specific cards and positions here
            //deck[0] = new Card(8, "8S", 0);
            //deck[1] = new Card(11, "AD", -1);
            //deck[2] = new Card(8, "8D", 0);
            //deck[3] = new Card(2, "2D", 1);
            //deck[4] = new Card(10, "0H", -1);
            //deck[5] = new Card(8, "8S", 0);
            //deck[6] = new Card(2, "2S", 1);
            //deck[7] = new Card(2, "2D", 1);
            //deck[8] = new Card(10, "0D", -1);
            //deck[9] = new Card(2, "2D", 1);
            //deck[10] = new Card(2, "2H", 1);
            //deck[11] = new Card(11, "AS", -1);
        }

        //This function swaps the arguments of a list of players. Used for switching players between the players and splits lists.
        private void SwapPlayers(List<Player> a, List<Player> b, int i, int j)
        {
            Player c = a[i];
            a[i] = b[j];
            b[j] = c;

        }

        //Code to increase the number of players on the startup box
        private void PlayerIncrease(object sender, MouseButtonEventArgs e)
        {
            if (startupPlayers == 5) { }
            else
            {
                startupPlayers++;

                //increase box size
                StartupBox.Height += 110;
                StartupBoxRect.Height += 110;
                initialPlayerGrid.Height += 110;

                //add rectangle holder
                Rectangle playerRect = new Rectangle();
                playerRect.Height = 100;
                playerRect.Width = 280;
                playerRect.Fill = new SolidColorBrush(Color.FromArgb(255, 177, 249, 249));
                playerRect.RadiusX = 30;
                playerRect.RadiusY = 30;
                playerRect.Margin = new Thickness(0, (110 * (startupPlayers - 1)), 0, 0);
                playerRect.StrokeThickness = 1;
                playerRect.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                playerRect.Name = "PlayerRect" + startupPlayers.ToString();
                playerRect.VerticalAlignment = VerticalAlignment.Top;
                initialPlayerGrid.Children.Add(playerRect);

                //Add controls
                TextBlock playerName = new TextBlock();
                playerName.Text = "Player " + startupPlayers.ToString();
                playerName.Margin = new Thickness(0, 10 + (110 * (startupPlayers - 1)), 0, 0);
                playerName.Foreground = new SolidColorBrush(Colors.Black);
                playerName.Width = 50;
                playerName.Height = 16;
                playerName.TextAlignment = TextAlignment.Center;
                playerName.FontSize = 12;
                playerName.VerticalAlignment = VerticalAlignment.Top;
                playerName.Name = "PlayerName" + startupPlayers.ToString();
                initialPlayerGrid.Children.Add(playerName);

                TextBlock money = new TextBlock();
                money.Text = "Money";
                money.Margin = new Thickness(20, 44 + (110 * (startupPlayers - 1)), 210, 0);
                money.Foreground = new SolidColorBrush(Colors.Black);
                money.Width = 50;
                money.Height = 16;
                money.TextAlignment = TextAlignment.Center;
                money.FontSize = 11;
                money.VerticalAlignment = VerticalAlignment.Top;
                money.HorizontalAlignment = HorizontalAlignment.Left;
                money.Name = "Money" + startupPlayers.ToString();
                initialPlayerGrid.Children.Add(money);

                TextBox moneyBox = new TextBox();
                moneyBox.HorizontalAlignment = HorizontalAlignment.Left;
                moneyBox.VerticalAlignment = VerticalAlignment.Top;
                moneyBox.Height = 20;
                moneyBox.Width = 50;
                moneyBox.Margin = new Thickness(20, 60 + (110 * (startupPlayers - 1)), 0, 0);
                moneyBox.Text = "100";
                moneyBox.Name = "MoneyBox" + startupPlayers.ToString();
                initialPlayerGrid.Children.Add(moneyBox);

                RadioButton Human = new RadioButton()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 16,
                    GroupName = startupPlayers.ToString() + "A",
                    Margin = new Thickness(75, 44 + (110 * (startupPlayers - 1)), 145, 0),
                    Content = "Human",
                    Name = "Human" + startupPlayers.ToString()
                };
                RadioButton Computer = new RadioButton()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 16,
                    GroupName = startupPlayers.ToString() + "A",
                    Margin = new Thickness(75, 64 + (110 * (startupPlayers - 1)), 128, 0),
                    Content = "Computer",
                    Name = "Computer" + startupPlayers.ToString()
                };
                initialPlayerGrid.Children.Add(Human);
                initialPlayerGrid.Children.Add(Computer);

                TextBlock Strategy = new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Text = "Strategy",
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(157, 44 + (110 * (startupPlayers - 1)), 0, 0),
                    Width = 113,
                    TextAlignment = TextAlignment.Center,
                    Name = "Strategy" + startupPlayers.ToString()
                };
                initialPlayerGrid.Children.Add(Strategy);

                ComboBox StrategyList = new ComboBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(157, 60 + (110 * (startupPlayers - 1)), 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 113,
                    Name = "StrategyList" + startupPlayers.ToString(),
                };
                foreach (string s in strategies)
                {
                    StrategyList.Items.Add(s);
                }
                //Set a default choice for the automation list so it cannot be a blank selection
                StrategyList.SelectedIndex = 0;

                //add the combobox to the player's area
                initialPlayerGrid.Children.Add(StrategyList);

                //Increase number of players displayed
                NumberOfPlayers.Text = startupPlayers.ToString();
            }
        }

        //decrease the number of players on the startup box
        private void PlayerDecrease(object sender, MouseButtonEventArgs e)
        {
            if (startupPlayers == 1) { }
            else
            {
                //Remove Controls and shapes
                TextBlock playerName = FindName("PlayerName" + startupPlayers.ToString()) as TextBlock;
                initialPlayerGrid.Children.Remove(playerName);

                Rectangle playerRect = FindName("PlayerRect" + startupPlayers.ToString()) as Rectangle;
                initialPlayerGrid.Children.Remove(playerRect);

                TextBlock money = FindName("Money" + startupPlayers.ToString()) as TextBlock;
                initialPlayerGrid.Children.Remove(money);

                TextBox moneyBox = FindName("MoneyBox" + startupPlayers.ToString()) as TextBox;
                initialPlayerGrid.Children.Remove(moneyBox);

                RadioButton Human = FindName("Human" + startupPlayers.ToString()) as RadioButton;
                initialPlayerGrid.Children.Remove(Human);

                RadioButton Computer = FindName("Computer" + startupPlayers.ToString()) as RadioButton;
                initialPlayerGrid.Children.Remove(Computer);

                TextBlock Strategy = FindName("Strategy" + startupPlayers.ToString()) as TextBlock;
                initialPlayerGrid.Children.Remove(Strategy);

                ComboBox StrategyList = FindName("StrategyList" + startupPlayers.ToString()) as ComboBox;
                initialPlayerGrid.Children.Remove(StrategyList);

                //shrink boxes
                StartupBox.Height -= 110;
                StartupBoxRect.Height -= 110;
                initialPlayerGrid.Height -= 110;

                //decrease starting player count
                startupPlayers--;

                //decrease number of players displayed
                NumberOfPlayers.Text = startupPlayers.ToString();
            }
        }

        //Increase number of decks on startup box
        private void DecksUp(object sender, MouseButtonEventArgs e)
        {
            if (startupDecks == 8) { }
            else
            {
                startupDecks++;
                NumberOfDecks.Text = startupDecks.ToString();
            }
        }

        //Decrease number of decks on startup box
        private void DecksDown(object sender, MouseButtonEventArgs e)
        {
            if (startupDecks == 1) { }
            else
            {
                startupDecks--;
                NumberOfDecks.Text = startupDecks.ToString();
            }
        }

        //Show the count on mouseover
        private void ShowCount(object sender, MouseEventArgs e)
        {
            TheCountBlock.Opacity = 100;
            foreach (Player p in players)
            {
                p.scoreBlock.Visibility = Visibility.Visible;
            }
            foreach (Player s in splits)
            {
                s.scoreBlock.Visibility = Visibility.Visible;
            }
        }

        //Hide the count on mouse leave
        private void HideCount(object sender, MouseEventArgs e)
        {
            TheCountBlock.Opacity = 0;
            foreach (Player p in players)
            {
                p.scoreBlock.Visibility = Visibility.Collapsed;
            }
            foreach (Player s in splits)
            {
                s.scoreBlock.Visibility = Visibility.Collapsed;
            }
        }

        //GAME PLAY STARTS HERE
        //set player positions and add each player's controls (name, score, bet, money, and overall play area) and set first player to active
        private void InitialSetup(object sender, RoutedEventArgs e)
        {
            //Get a bigger storage space
            if (RunDataStorage.Quota < 10000000)
            {
                RunDataStorage.IncreaseQuotaTo(10000000);
            }

            //hide the box to start
            StartupBox.Visibility = Visibility.Collapsed;

            //shuffle the deck
            Shuffle();

            //if automating many hands, make sure all players are automated
            if (FullAuto.IsChecked == true)
            {
                for (int i = 1; i <= Convert.ToInt32(NumberOfPlayers.Text); i++)
                {
                    RadioButton rb = FindName("Computer" + i.ToString()) as RadioButton;
                    rb.IsChecked = true;
                }
            }

            //Add players, their money, and their play area positions
            for (int i = 0; i<Convert.ToInt32(NumberOfPlayers.Text); i++)
            {
                players.Add(new Player());
                TextBox MoneyBox = FindName("MoneyBox" + (i + 1).ToString()) as TextBox;
                players[i].money = Convert.ToDouble(MoneyBox.Text);
                players[i].playArea = new Grid()
                {
                    //size the play area. Includes 8 cards, money box, bet box, and player number
                    Height = 176,
                    Width = 192,
                    Margin = new Thickness((25+(1280/(Convert.ToInt32(NumberOfPlayers.Text)))*i),0,0,100),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //This brush is used to see the shape of player area when testing
                    //Background = new SolidColorBrush(Colors.Black)
                };
                GameTable.Children.Add(players[i].playArea);
                //Add controls in their area
                players[i].betBox = new TextBox()
                {
                    Width = 60,
                    Height = 20,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10,0,0,25),
                    Text = "0"
                };
                players[i].playArea.Children.Add(players[i].betBox);

                players[i].moneyBlock = new TextBlock()
                {
                    Width = 50,
                    Height = 20,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(80,0,0,25),
                    Text = players[i].money.ToString(),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 16
                };
                players[i].playArea.Children.Add(players[i].moneyBlock);

                TextBlock playerName = new TextBlock()
                {
                    Width = 192,
                    Height = 20,
                    Text = "Player " + (i+1).ToString(),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(10,0,0,5),
                    Name = "Player " + (i+1).ToString()
                };
                players[i].playArea.Children.Add(playerName);

                players[i].scoreBlock = new TextBlock()
                {
                    Width = 192,
                    Height = 20,
                    Margin = new Thickness(10,0,10,0),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontSize = 16,
                    Text = players[i].score.ToString(),
                    Foreground = new SolidColorBrush(Colors.Black),
                    Visibility = Visibility.Collapsed,
                };
                players[i].playArea.Children.Add(players[i].scoreBlock);

                //set player automation choice
                RadioButton humanChoice = FindName("Human" + (i + 1).ToString()) as RadioButton;
                RadioButton compChoice = FindName("Computer" + (i + 1).ToString()) as RadioButton;
                if (humanChoice.IsChecked == true || humanChoice.IsChecked == false && compChoice.IsChecked == false)
                {
                    //Human controlled, will never run Automate function on this player.
                    //This is also default, if no radio button is checked
                    players[i].automation = 0;
                }
                else if (compChoice.IsChecked == true)
                {
                    //Computer controlled, switch statement of list names
                    ComboBox stratList = FindName("StrategyList" + (i+1).ToString()) as ComboBox;
                    switch (stratList.SelectionBoxItem as string)
                    {
                        case "Stay on all 17":
                            players[i].automation = 1;
                            players[i].betBox.Text = "10";
                            break;
                        case "Hit on soft 17":
                            players[i].automation = 2;
                            players[i].betBox.Text = "10";
                            break;
                        case "Standard Play":
                            players[i].automation = 3;
                            players[i].betBox.Text = "10";
                            break;
                        case "Count Cards #1":
                            players[i].automation = 4;
                            players[i].betBox.Text = "5";
                            break;
                    }
                }

            }

            //Add dealer and dealer name and dealer score block
            players.Add(new Player());
            players[Convert.ToInt32(NumberOfPlayers.Text)].playArea = new Grid()
            {
                Height = 176,
                Width = 192,
                Margin = new Thickness(564, 0, 0, 300),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                //This brush is just to see the dealer's play area for testing
                //Background = new SolidColorBrush(Colors.Black)
            };
            GameTable.Children.Add(players[Convert.ToInt32(NumberOfPlayers.Text)].playArea);

            TextBlock dealerName = new TextBlock()
            {
                Width = 192,
                Height = 20,
                Text = "Dealer",
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10, 0, 0, 5),
                Name = "Player " + players.Count.ToString()
            };
            players[players.Count-1].playArea.Children.Add(dealerName);

            players[players.Count-1].scoreBlock = new TextBlock()
            {
                Width = 192,
                Height = 20,
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 16,
                Text = players[players.Count-1].score.ToString(),
                Foreground = new SolidColorBrush(Colors.Black),
                Visibility = Visibility.Collapsed,
                Opacity = 0
            };
            players[players.Count-1].playArea.Children.Add(players[players.Count-1].scoreBlock);

            //Make the dealer stand on soft 17's, hardcoded for now
            players[players.Count - 1].automation = 1;

            //Make the "Deal" button visible
            DealButton.Visibility = Visibility.Visible;

            //Disable split and double buttons to start
            DoubleButton.IsEnabled = false;
            SplitButton.IsEnabled = false;

            //Setup initial values in storage file for run data
            IsolatedStorageFileStream RunDataFile = RunDataStorage.CreateFile("RunData.txt");
            RunDataFile.Close();
            using (StreamWriter RunDataWrite = new StreamWriter(RunDataStorage.OpenFile("RunData.txt",FileMode.Open,FileAccess.Write)))
            {
                //Record number of players and decks
                RunDataWrite.WriteLine(NumberOfPlayers.Text + "," + NumberOfDecks.Text);

                //write automation method and initial money
                foreach (Player p in players)
                {
                    RunDataWrite.WriteLine(p.automation + "," + p.money.ToString());
                }

                //Write a separator between initial data and run data
                RunDataWrite.WriteLine("Run Data Below:");
            }

            //simulate many hands
            if (FullAuto.IsChecked == true)
            {
                for (int i = 1; i <= 500; i++)
                {
                    handCount = i;
                    while (DealButton.Visibility == Visibility.Visible)
                    {
                            Deal(null, null);
                    }
                    NewHand(null, null);
                }
            }
        }

        //Deal a card function. Only for dealing first two cards to each player; replaced with "hit" once that happens
        private void Deal(object sender, RoutedEventArgs e)
        {
            //Lock bets and subtract money from each player's pool, only if this hasn't already been done
            if (players[0].betBox.IsReadOnly == false)
            {
                for (int i = 0; i < startupPlayers; i++)
                {
                    players[i].betBox.IsReadOnly = true;
                    if (Convert.ToInt32(players[i].betBox.Text) > players[i].money && players[i].money != 0)
                    {
                        //if you bet more than you have, you lose all of your money and become inactive. Important for automation.
                        players[i].betBox.Text = "0";
                        players[i].money = 0;
                        players[i].moneyBlock.Text = "0";
                        players[i].active = false;
                    }
                    else if (Convert.ToInt32(players[i].betBox.Text) <= players[i].money && Convert.ToInt32(players[i].betBox.Text) != 0)
                    {
                        players[i].money -= Convert.ToDouble(players[i].betBox.Text);
                        players[i].moneyBlock.Text = players[i].money.ToString();
                        players[i].active = true;
                    }
                    else
                    {
                        //if bet is zero or player has no money, deactivate the player
                        players[i].active = false;
                    }
                }
                TurnPosition = 1;
            }

            //Only deal to active players
            while (players[TurnPosition - 1].active == false)
            {
                TurnPosition++;
            }

            //give a card to the active player
            players[TurnPosition - 1].cards.Add(deck[0]);

            //Update the player's score, aceCount, image, and scoreBlock
            players[TurnPosition - 1].score += deck[0].ScoreValue;
            if (deck[0].name == "AS" || deck[0].name == "AD" || deck[0].name == "AH" || deck[0].name == "AC")
            {
                players[TurnPosition - 1].aceCount++;
            }
            players[TurnPosition - 1].scoreBlock.Text = players[TurnPosition - 1].score.ToString();
            if (TurnPosition == Convert.ToInt32(NumberOfPlayers.Text) + 1 && players[TurnPosition - 1].cards.Count == 1)
            {
                players[TurnPosition - 1].cardImages.Add(new Rectangle()
                {
                    Width = 72,
                    Height = 96,
                    Margin = new Thickness(15 * players[TurnPosition - 1].cards.Count, 30, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri("Cards/Back.png", UriKind.RelativeOrAbsolute))
                    }
                }
                );
            }
            else
            {
                players[TurnPosition - 1].cardImages.Add(new Rectangle()
                {
                    Width = 72,
                    Height = 96,
                    Margin = new Thickness(15 * players[TurnPosition - 1].cards.Count, 30, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri("Cards/" + deck[0].name + ".png", UriKind.RelativeOrAbsolute))
                    }
                }
                );

                //Increase the card count
                TheCount += deck[0].CountValue;
                TheCountBlock.Text = TheCount.ToString();
            }
            players[TurnPosition - 1].playArea.Children.Add(players[TurnPosition - 1].cardImages[players[TurnPosition - 1].cards.Count - 1]);

            //check for double ace scenario and adjust info accordingly
            if (players[TurnPosition - 1].score == 22)
            {
                players[TurnPosition - 1].score = 12;
                players[TurnPosition - 1].aceCount--;
                players[TurnPosition - 1].scoreBlock.Text = "12";
            }

            //Remove the card from the deck
            deck.Remove(deck[0]);
            
            //Move to next player to deal a card
            if (TurnPosition != players.Count)
            {
                TurnPosition++;
            }
            else
            {
                TurnPosition = 1;
            }

            //If all cards have been dealt, switch to "hit" button, show "stay", "double", and "split" buttons
            int activeCount = 0;
            int cardDealCount = 0;
            foreach (Player p in players)
            {
                if (p.active == true)
                {
                    cardDealCount += p.cards.Count;
                    activeCount++;
                }
            }
            if (TurnPosition == 1 && cardDealCount == activeCount * 2)
            {
                DealButton.Visibility = Visibility.Collapsed;
                HitButton.Visibility = Visibility.Visible;
                StayButton.Visibility = Visibility.Visible;
                DoubleButton.Visibility = Visibility.Visible;
                SplitButton.Visibility = Visibility.Visible;

                //Auto finish if the dealer has a blackjack
                if (players[Convert.ToInt32(NumberOfPlayers.Text)].score == 21)
                {
                    //flip the card
                    players[Convert.ToInt32(NumberOfPlayers.Text)].cardImages[0].Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri("Cards/" + players[Convert.ToInt32(NumberOfPlayers.Text)].cards[0].name + ".png", UriKind.RelativeOrAbsolute))
                    };
                    //update the count
                    TheCount += players[Convert.ToInt32(NumberOfPlayers.Text)].cards[0].CountValue;
                    TheCountBlock.Text = TheCount.ToString();
                    //finish the hand
                    FinalizeHand();
                }
                else
                {

                    //Enable buttons if applicable
                    DoubleButton.IsEnabled = true;
                    if (players[TurnPosition - 1].cards.Count == 2 &&
                        players[TurnPosition - 1].cards[0].name.Substring(0, 1) == players[TurnPosition - 1].cards[1].name.Substring(0, 1) &&
                        players[TurnPosition - 1].money >= Convert.ToInt32(players[TurnPosition - 1].betBox.Text))
                    {
                        SplitButton.IsEnabled = true;
                    }

                    //if the first player is a computer, automate
                    if (players[TurnPosition - 1].automation != 0 && players[TurnPosition-1].active == true)
                    {
                        Automate();
                    }

                    //If the first player has a blackjack or is inactive, skip to player 2
                    else if (players[TurnPosition - 1].score == 21 || players[TurnPosition - 1].active == false)
                    {
                        NextPlayer();
                    }

                    //try & catch is in here because, on rare occasions, the FinalizeHand function has already been called before dealing is finished
                    //(i.e. all players are inactive or all active players have blackjacks [see if statement directly above]
                    //This means "TurnName" returns null and cannot be set to bold, crashing the program. This code is skipped if that happens.
                    //TextBlock TurnName = FindName("Player " + TurnPosition.ToString()) as TextBlock;
                    //try
                    //{
                    //    TurnName.FontWeight = FontWeights.Bold;
                    //}
                    //catch { }
                }
            }
        }

        //Gives the active player a card, updates their information, checks for bust, accounts for Aces, 
        //disables split and double options, flips the delaer's face down card
        private void Hit(object sender, RoutedEventArgs e)
        {
            //give the player a card
            players[TurnPosition - 1].cards.Add(deck[0]);

            //update their information
            players[TurnPosition - 1].score += deck[0].ScoreValue;
            if (deck[0].name == "AS" || deck[0].name == "AD" || deck[0].name == "AH" || deck[0].name == "AC")
            {
                players[TurnPosition - 1].aceCount++;
            }
            players[TurnPosition - 1].scoreBlock.Text = players[TurnPosition - 1].score.ToString();
            players[TurnPosition - 1].cardImages.Add(new Rectangle()
            {
                Width = 72,
                Height = 96,
                Margin = new Thickness(15 * players[TurnPosition - 1].cards.Count, 30, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Fill = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri("Cards/" + deck[0].name + ".png", UriKind.RelativeOrAbsolute))
                }
            }
            );
            players[TurnPosition - 1].playArea.Children.Add(players[TurnPosition - 1].cardImages[players[TurnPosition - 1].cards.Count - 1]);

            //Increase the card count
            TheCount += deck[0].CountValue;
            TheCountBlock.Text = TheCount.ToString();

            //Remove the card from the deck
            deck.Remove(deck[0]);

            //disable double and split options
            DoubleButton.IsEnabled = false;
            SplitButton.IsEnabled = false;

            //check for bust, adjust for aces
            if (players[TurnPosition - 1].score > 21)
            {
                if (players[TurnPosition - 1].aceCount > 0)
                {
                    players[TurnPosition - 1].score -= 10;
                    players[TurnPosition - 1].aceCount--;
                    players[TurnPosition - 1].scoreBlock.Text = players[TurnPosition - 1].score.ToString();
                }
                else
                {
                    players[TurnPosition - 1].scoreBlock.Text = "Bust";
                    players[TurnPosition - 1].bust = true;

                    //if splitting, swap the players back
                    if (splitting == true)
                    {
                        SwapPlayers(players, splits, TurnPosition - 1, splits.Count - 1);
                        splitting = false;
                        Hit(null, null);
                    }
                    else
                    {
                        NextPlayer();
                    }
                }
            }
        }

        //move to the next player, re-enable double and split options
        private void Stay(object sender, RoutedEventArgs e)
        {
            //if currently splitting, switch back to main hand
            if (splitting == true)
            {
                SwapPlayers(players,splits,TurnPosition-1,splits.Count -1);
                splitting = false;
                Hit(null, null);
            }
            else
            {
                NextPlayer();
            }
        }

        //double the bet, add a card, move to the next player
        private void Double(object sender, RoutedEventArgs e)
        {
            int bustCheck = TurnPosition;

            //if they have enough money to double down, do it. Also checks the right player if currently splitting
            if (players[TurnPosition - 1].money >= Convert.ToInt32(players[TurnPosition - 1].betBox.Text) && splitting == false)
            {
                players[TurnPosition - 1].money -= Convert.ToInt32(players[TurnPosition - 1].betBox.Text);
                players[TurnPosition - 1].betBox.Text = (2 * Convert.ToInt32(players[TurnPosition - 1].betBox.Text)).ToString();
                players[TurnPosition - 1].moneyBlock.Text = players[TurnPosition - 1].money.ToString();
                Hit(null, null);
            }
            else if (splitting == true && splits[splits.Count - 1].money >= Convert.ToInt32(players[TurnPosition - 1].betBox.Text))
            {
                splits[splits.Count - 1].money -= Convert.ToInt32(players[TurnPosition - 1].betBox.Text);
                players[TurnPosition - 1].betBox.Text = (2 * Convert.ToInt32(players[TurnPosition - 1].betBox.Text)).ToString();
                splits[splits.Count - 1].moneyBlock.Text = splits[splits.Count -1].money.ToString();
                Hit(null, null);
            }
            else
            {
                Hit(null, null);
            }

            //If they didn't bust when they doubled down, move to the next player
            if (bustCheck == TurnPosition)
            {
                if (splitting == true)
                {
                    SwapPlayers(players, splits, TurnPosition - 1, splits.Count - 1);
                    splitting = false;
                    Hit(null, null);
                }
                else
                {
                    NextPlayer();
                }
            }
        }

        //add a player to splits list, play them normally but store be sure the winnings go to the original splitting player
        private void Split(object sender, RoutedEventArgs e)
        {
            splits.Add(new Player());

            //double aces special case
            if (players[TurnPosition - 1].aceCount == 1 && players[TurnPosition - 1].score == 12)
            {
                players[TurnPosition - 1].score = 11;
                splits[splits.Count - 1].score = 11;
                splits[splits.Count - 1].aceCount = 1;
            }
            else
            {
                splits[splits.Count - 1].score = players[TurnPosition - 1].score/2;
                players[TurnPosition - 1].score = splits[splits.Count - 1].score;
            }

            //set up split player
            splits[splits.Count - 1].cards.Add(players[TurnPosition - 1].cards[1]);
            splits[splits.Count-1].playArea = new Grid()
            {
                //size the play area. Includes 9 cards, money box, bet box, and player number
                Height = 176,
                Width = 192,
                Margin = new Thickness(100+162*(splits.Count-1), 0, 0, 486),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                //This brush is used to see the shape of player area when testing
                //Background = new SolidColorBrush(Colors.Black)
            };
            GameTable.Children.Add(splits[splits.Count-1].playArea);
            splits[splits.Count - 1].betBox = new TextBox()
            {
                Width = 60,
                Height = 20,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 0, 0, 25),
                Text = players[TurnPosition - 1].betBox.Text
            };
            splits[splits.Count - 1].playArea.Children.Add(splits[splits.Count - 1].betBox);
            TextBlock splitName = new TextBlock()
            {
                Width = 192,
                Height = 20,
                Text = "Split " + splits.Count.ToString(),
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10, 0, 0, 5),
                Name = "Split " + splits.Count.ToString()
            };
            splits[splits.Count - 1].scoreBlock = new TextBlock()
            {
                Width = 192,
                Height = 20,
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 16,
                Text = splits[splits.Count - 1].score.ToString(),
                Foreground = new SolidColorBrush(Colors.Black),
                Visibility = Visibility.Collapsed,
            };
            splits[splits.Count - 1].playArea.Children.Add(splits[splits.Count - 1].scoreBlock);
            splits[splits.Count - 1].playArea.Children.Add(splitName);
            splits[splits.Count - 1].cardImages.Add(players[TurnPosition - 1].cardImages[1]);
            splits[splits.Count - 1].cardImages[0].Margin = new Thickness(15 * splits[splits.Count - 1].cards.Count, 30, 0, 0);
            
            //store the player who this split belongs to
            splits[splits.Count - 1].money = TurnPosition;


            //Correct original player
            players[TurnPosition - 1].cards.Remove(players[TurnPosition - 1].cards[1]);
            players[TurnPosition - 1].money -= Convert.ToInt32(players[TurnPosition - 1].betBox.Text);
            players[TurnPosition - 1].moneyBlock.Text = players[TurnPosition - 1].money.ToString();
            players[TurnPosition - 1].playArea.Children.Remove(players[TurnPosition - 1].cardImages[1]);
            players[TurnPosition - 1].cardImages.Remove(players[TurnPosition - 1].cardImages[1]);
            players[TurnPosition - 1].scoreBlock.Text = players[TurnPosition - 1].score.ToString();

            //give second card to splitter
            splits[splits.Count - 1].cards.Add(deck[0]);
            splits[splits.Count - 1].score += deck[0].ScoreValue;
            if (deck[0].name == "AS" || deck[0].name == "AD" || deck[0].name == "AH" || deck[0].name == "AC")
            {
                splits[splits.Count - 1].aceCount++;
            }
            splits[splits.Count - 1].scoreBlock.Text = splits[splits.Count - 1].score.ToString();
            splits[splits.Count - 1].cardImages.Add(new Rectangle()
            {
                Width = 72,
                Height = 96,
                Margin = new Thickness(15 * splits[splits.Count - 1].cards.Count, 30, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Fill = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri("Cards/" + deck[0].name + ".png", UriKind.RelativeOrAbsolute))
                }
            }
            );
            splits[splits.Count - 1].playArea.Children.Add(splits[splits.Count - 1].cardImages[0]);
            splits[splits.Count - 1].playArea.Children.Add(splits[splits.Count - 1].cardImages[1]);

            //update the count
            TheCount += deck[0].CountValue;

            //remove card from the deck
            deck.Remove(deck[0]);

            //prevent splitting a split
            SplitButton.IsEnabled = false;

            //Swap players out and set splitting to true
            SwapPlayers(players,splits,TurnPosition-1,splits.Count-1);
            splitting = true;
        }

        //compare scores to dealer score, payout correctly, show wins and losses, disable other options and open new bets
        private void FinalizeHand()
        {
            //Compare to dealer score and payout each player
            for (int i = 0; i < Convert.ToInt32(NumberOfPlayers.Text); i++)
            {
                //Only do this for active players
                if (players[i].active)
                {
                    if (players[i].score < players[players.Count - 1].score && players[Convert.ToInt32(NumberOfPlayers.Text)].bust == false || players[i].bust == true)
                    {
                        players[i].scoreBlock.Text = "Lose";
                    }
                    else if (players[i].score == 21 && players[i].cards.Count == 2)
                    {
                        players[i].scoreBlock.Text = "Blackjack";
                        players[i].money += 2.5 * Convert.ToInt32(players[i].betBox.Text);
                        players[i].moneyBlock.Text = players[i].money.ToString();
                    }
                    else if (players[i].score > players[players.Count - 1].score || players[Convert.ToInt32(NumberOfPlayers.Text)].bust == true)
                    {
                            players[i].scoreBlock.Text = "Win";
                            players[i].money += 2 * Convert.ToInt32(players[i].betBox.Text);
                            players[i].moneyBlock.Text = players[i].money.ToString();
                    }
                    else
                    {
                        players[i].scoreBlock.Text = "Push";
                        players[i].money += Convert.ToInt32(players[i].betBox.Text);
                        players[i].moneyBlock.Text = players[i].money.ToString();
                    }
                }
            }

            //compare and pay splits
            foreach (Player p in splits)
            {
                if (p.score < players[players.Count - 1].score && players[players.Count - 1].bust == false || p.bust == true)
                {
                    p.scoreBlock.Text = "Lose";
                }
                else if (p.score > players[players.Count - 1].score || players[players.Count - 1].bust == true)
                {
                    if (p.score == 21 && p.cards.Count == 2)
                    {
                        players[Convert.ToInt32(p.money) - 1].money += 2.5 * Convert.ToInt32(p.betBox.Text);
                        players[Convert.ToInt32(p.money) - 1].moneyBlock.Text = players[Convert.ToInt32(p.money) - 1].money.ToString();
                        p.scoreBlock.Text = "Blackjack";
                    }
                    else
                    {
                        players[Convert.ToInt32(p.money) - 1].money += 2 * Convert.ToInt32(p.betBox.Text);
                        players[Convert.ToInt32(p.money) - 1].moneyBlock.Text = players[Convert.ToInt32(p.money) - 1].money.ToString();
                        p.scoreBlock.Text = "Win";
                    }
                }
                else
                {
                    players[Convert.ToInt32(p.money) - 1].money += Convert.ToInt32(p.betBox.Text);
                    players[Convert.ToInt32(p.money) - 1].moneyBlock.Text = players[Convert.ToInt32(p.money) - 1].money.ToString();
                    p.scoreBlock.Text = "Push";
                }
            }

            //Store data from this hand
            using (StreamWriter RunDataWrite = new StreamWriter(RunDataStorage.OpenFile("RunData.txt", FileMode.Append, FileAccess.Write)))
            {
                RunDataWrite.WriteLine();
                foreach (Player p in players)
                {
                    //a crude way to separate the dealer from the players
                    if (p.betBox.Width == 60)
                    {
                        foreach (Card c in p.cards)
                        {
                            RunDataWrite.Write(c.name + ":" + c.ScoreValue + ":" + c.CountValue + ":");
                        }
                        RunDataWrite.Write("," + p.score + ",");
                        RunDataWrite.Write(p.aceCount + ",");
                        RunDataWrite.Write(p.betBox.Text + ",");
                        RunDataWrite.Write(p.scoreBlock.Text + ",");
                        RunDataWrite.Write(p.money.ToString() + ",");
                    }
                    else
                    {
                        //what to record of the dealer's information
                        foreach (Card c in p.cards)
                        {
                            RunDataWrite.Write(c.name + ":" + c.ScoreValue + ":" + c.CountValue + ":");
                        }
                        RunDataWrite.Write("," + p.score + ",");
                        RunDataWrite.Write(p.aceCount + ",");

                        //Once per hand information
                        RunDataWrite.Write(handCount.ToString() + ",");
                        RunDataWrite.Write(deck.Count.ToString() + ",");
                        RunDataWrite.Write(TheCount.ToString() + ",");
                    }
                }
                foreach (Player s in splits)
                {
                    //Record split info
                    foreach (Card c in s.cards)
                    {
                        RunDataWrite.Write(c.name + ":" + c.ScoreValue + ":" + c.CountValue + ":");
                    }
                    RunDataWrite.Write("," + s.score + ",");
                    RunDataWrite.Write(s.aceCount + ",");
                    RunDataWrite.Write(s.betBox.Text + ",");
                    RunDataWrite.Write(s.scoreBlock.Text + ",");
                    RunDataWrite.Write(s.money.ToString() + ",");
                }
            }

            //erase bet boxes after data is saved
            for (int i = 0; i < Convert.ToInt32(NumberOfPlayers.Text); i++)
            {
                players[i].betBox.Text = "0";
            }

            //Disable buttons, switch "hit" for "new hand"
            HitButton.Visibility = Visibility.Collapsed;
            StayButton.Visibility = Visibility.Collapsed;
            DoubleButton.Visibility = Visibility.Collapsed;
            SplitButton.Visibility = Visibility.Collapsed;
            NewHandButton.Visibility = Visibility.Visible;

        }

        //Clear all cards, clear all player data related to a single hand, open up bet boxes, reshuffle if shoe is low on cards
        private void NewHand(object sender, RoutedEventArgs e)
        {
            //refill the shoe if low
            if (deck.Count < 40)
            {
                Shuffle();
            }

            //clear player data
            foreach (Player p in players)
            {
                p.aceCount = 0;
                p.bust = false;
                p.cards.Clear();
                p.score = 0;
                p.scoreBlock.Text = "0";
                foreach (Rectangle r in p.cardImages)
                {
                    p.playArea.Children.Remove(r);
                }
                p.cardImages.Clear();
            }

            //clear all data in splits, and it's playareas from the board
            foreach (Player p in splits)
            {
                GameTable.Children.Remove(p.playArea);
            }
            splits.Clear();

            //open up bet boxes and set based on automation
            for (int i = 0; i < startupPlayers; i++)
            {
                players[i].betBox.IsReadOnly = false;
                switch (players[i].automation)
                {
                    case 0:
                        players[i].betBox.Text = "0";
                        break;
                    case 1:
                        players[i].betBox.Text = "10";
                        break;
                    case 2:
                        players[i].betBox.Text = "10";
                        break;
                    case 3:
                        players[i].betBox.Text = "10";
                        break;
                    case 4:
                        double trueCount = 0;
                        trueCount = TheCount / (Convert.ToDouble(deck.Count) / 52);
                        if (deck.Count < 40 || trueCount < 0)
                        {
                            trueCount = 0;
                        }
                        trueCount = Math.Round(trueCount);
                        trueCount = trueCount / 3;
                        trueCount = Math.Floor(trueCount);
                        players[i].betBox.Text =((10 * trueCount) + 10).ToString();
                        break;
                }
            }

            //switch back to "deal" button
            NewHandButton.Visibility = Visibility.Collapsed;
            DealButton.Visibility = Visibility.Visible;

            //Reset turn position
            TurnPosition = 1;
        }

        //Function to considate all the options when switching to a new player. Called in Hit (when someone busts) and in Double Down and Stay.
        private void NextPlayer()
        {
            //switch to next player
            TextBlock oldName = FindName("Player " + TurnPosition.ToString()) as TextBlock;
            oldName.FontWeight = FontWeights.Medium;
            TurnPosition++;
            if (TurnPosition != Convert.ToInt32(NumberOfPlayers.Text) + 2)
            {
                TextBlock TurnName = FindName("Player " + TurnPosition.ToString()) as TextBlock;
                TurnName.FontWeight = FontWeights.Bold;

                //flip the dealer's card if it is the dealer's turn, and allow their score to be shown by the ShowCount function
                if (TurnPosition == Convert.ToInt32(NumberOfPlayers.Text) + 1)
                {
                    players[Convert.ToInt32(NumberOfPlayers.Text)].cardImages[0].Fill = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri("Cards/" + players[Convert.ToInt32(NumberOfPlayers.Text)].cards[0].name + ".png", UriKind.RelativeOrAbsolute))
                    };
                    //Disable Double and Split buttons
                    DoubleButton.IsEnabled = false;
                    SplitButton.IsEnabled = false;

                    //Add the flipped card to the count (since it wasn't added when it was facedown) and allow total dealer score to be shown by ShowCount
                    TheCount += players[TurnPosition - 1].cards[0].CountValue;
                    TheCountBlock.Text = TheCount.ToString();
                    players[players.Count - 1].scoreBlock.Opacity = 100;
                }

                //reactivate Double and Split buttons if applicable
                if (TurnPosition != Convert.ToInt32(NumberOfPlayers.Text) + 1)
                {
                    DoubleButton.IsEnabled = true;
                    if (players[TurnPosition - 1].cards.Count == 2 &&
                    players[TurnPosition - 1].cards[0].name.Substring(0, 1) == players[TurnPosition - 1].cards[1].name.Substring(0, 1) &&
                    players[TurnPosition - 1].money >= Convert.ToInt32(players[TurnPosition - 1].betBox.Text))
                    {
                        SplitButton.IsEnabled = true;
                    }
                }

                //skip the player if they have a blackjack OR are inactive
                if (players[TurnPosition - 1].score == 21 && players[TurnPosition - 1].cards.Count == 2 || players[TurnPosition-1].active == false)
                {
                    NextPlayer();
                }
                else if (players[TurnPosition - 1].automation != 0)
                {
                    //Code for automating the player. An automation value of 0 means the player is human-controlled.
                    Automate();
                }
            }
            else
            {
                //If the dealer stays or busts, finish the round
                FinalizeHand();
            }
        }

        //Code for automating the player.
        private void Automate()
        {
            int turnCheck = -1;

            switch (players[TurnPosition - 1].automation)
            {
                case 1:
                    //Stay on all 17's
                    turnCheck = TurnPosition;
                    while (turnCheck == TurnPosition)
                    {
                        if (players[TurnPosition - 1].score < 17)
                        {
                            Hit(null, null);
                        }
                        else
                        {
                            Stay(null, null);
                        }
                    }
                    break;
                case 2:
                    //Hit on soft 17
                    turnCheck = TurnPosition;
                    while (turnCheck == TurnPosition)
                    {
                        if (players[TurnPosition - 1].score < 17 || players[TurnPosition - 1].score == 17 && players[TurnPosition - 1].aceCount > 0)
                        {
                            Hit(null, null);
                        }
                        else
                        {
                            Stay(null, null);
                        }
                    }
                    break;
                case 3:
                case 4:
                    //Follow "Standard Play" - http://www.bettingcorp.com/casino-games-strategies/blackjack-101/basic-blackjack-strategy/

                    //x-variable for selecting player score, aces, and doubles
                    int x = 0;

                    //y-variable for slecting dealer's showing card
                    int y = 0;

                    //decision variable, to return the result of the logic
                    int choice = 0;

                    //Choose y immediately, as it won't change while looping
                    y = players[players.Count - 1].cards[1].ScoreValue;
                    if (y >= 10)
                    {
                        y -= 10;
                    }

                    turnCheck = TurnPosition;
                    while (turnCheck == TurnPosition)
                    {
                        if (splitting == true || players[TurnPosition -1].cards.Count != 2 || splits.Count > 0 && splits[splits.Count-1].money == turnCheck)
                        {
                            //Cannot split a split, cannot split if they have more than 2 cards, cannot split if they have already split this on this hand
                            x = players[TurnPosition - 1].score - 5;

                            //If they split 2's, then get another 2 on the splitting hand, x would become -1, resulting in an out of range exception...wow...
                            if (x < 0)
                            {
                                //basically, make the automation assume the hand is 5 for the sake of a decision, which will be to hit -- the same it would be for 4.
                                x = 0;
                            }

                            //If they have an active ace, but not ace doubles (since those are treated as simply a total of 12) or 10 doubles (treat it as a total score of 20)
                            if (players[TurnPosition - 1].aceCount != 0 && players[TurnPosition - 1].cards[0].name.Substring(0, 1) != "A")
                            {
                                x += 9;
                            }
                        }
                        else
                        {
                            //First choice of any player should come here
                            x = players[TurnPosition - 1].score - 5;

                            //If they have an active ace, but not ace doubles
                            if (players[TurnPosition - 1].aceCount != 0 && players[TurnPosition - 1].cards[0].name.Substring(0, 1) != "A")
                            {
                                x += 9;
                            }
                            //if they have doubles, but not ace (treat as a total score of 12) or 10 doubles (treat it as a total score of 20)
                            else if (players[TurnPosition - 1].cards[0].name.Substring(0, 1) == players[TurnPosition - 1].cards[1].name.Substring(0, 1)
                                     && players[TurnPosition - 1].score != 20 && players[TurnPosition - 1].cards[0].name.Substring(0, 1) != "A")
                            {
                                x = (players[TurnPosition - 1].score / 2) + 24;
                            }
                            //if they have ace doubles
                            else if (players[TurnPosition - 1].cards[0].name.Substring(0, 1) == "A" && players[TurnPosition - 1].cards[1].name.Substring(0, 1) == "A")
                            {
                                x = 34;
                            }
                        }

                        //make a choice using the logic in the array
                        choice = StandardPlay[x, y];

                        //don't allow them double down if they have more than 2 cards
                        if (players[TurnPosition - 1].cards.Count != 2 && choice == 3)
                        {
                            //hit instead
                            choice = 1;
                        }

                        //turn the choice into a command
                        switch (choice)
                        {
                            case 1:
                                Hit(null,null);
                                break;
                            case 2:
                                Stay(null, null);
                                break;
                            case 3:
                                Double(null, null);
                                break;
                            case 4:
                                Split(null, null);
                                break;
                        }

                    }

                    break;
            }
        }













    }
}