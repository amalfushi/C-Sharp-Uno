using System;
using System.Collections.Generic;
using System.Threading;

namespace Uno
{
    public class Game
    {
        public Deck deck;
        public Deck discard { get; set; } //discard pile
        public Card discarded { get; set; } //discarded card (can be the top card of the pile returned to the pile)
        public Player player;
        public Player[] players;
        public Card topCard;
        public bool playing;
        public bool reverse; //determines which direction the turn order is going
        public Player currentPlayer;
        public bool playedACard;
        public int startingTotal;

        public Game()
        {
            this.deck = new Deck();
            this.discard = new Deck(true);
            this.player = null;
        }

        public void Intro(){
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("\n\n\n\n\n\n\n\n");
            System.Console.WriteLine(@"
                   UUUUUUUU     UUUUUUUUNNNNNNNN        NNNNNNNN     OOOOOOOOO     
                   U::::::U     U::::::UN:::::::N       N::::::N   OO:::::::::OO   
                   U::::::U     U::::::UN::::::::N      N::::::N OO:::::::::::::OO 
                   UU:::::U     U:::::UUN:::::::::N     N::::::NO:::::::OOO:::::::O
                    U:::::U     U:::::U N::::::::::N    N::::::NO::::::O   O::::::O
                    U:::::D     D:::::U N:::::::::::N   N::::::NO:::::O     O:::::O
                    U:::::D     D:::::U N:::::::N::::N  N::::::NO:::::O     O:::::O
                    U:::::D     D:::::U N::::::N N::::N N::::::NO:::::O     O:::::O
                    U:::::D     D:::::U N::::::N  N::::N:::::::NO:::::O     O:::::O
                    U:::::D     D:::::U N::::::N   N:::::::::::NO:::::O     O:::::O
                    U:::::D     D:::::U N::::::N    N::::::::::NO:::::O     O:::::O
                    U::::::U   U::::::U N::::::N     N:::::::::NO::::::O   O::::::O
                    U:::::::UUU:::::::U N::::::N      N::::::::NO:::::::OOO:::::::O
                    UU:::::::::::::UU  N::::::N       N:::::::N OO:::::::::::::OO 
                      UU:::::::::UU    N::::::N        N::::::N   OO:::::::::OO   
                        UUUUUUUUU      NNNNNNNN         NNNNNNN     OOOOOOOOO     
            ");
            System.Console.WriteLine("\n\n\n\n\n\n\n\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("\nEnter your name:");
            System.Console.Out.Flush();

            string input = System.Console.ReadLine();
            
            while (input.Length < 1)
            {
                System.Console.WriteLine("\nName is too short.");
                input = System.Console.ReadLine();
            }
            Console.ResetColor();
            
            this.player = new Player(input, this);
        }

        public void Run()
        {
            topCard = deck.Deal();
            System.Console.WriteLine("\n-------------------------------------");
            while (playing)
            {
                if (deck.cards.Count < 1)
                {
                    System.Console.WriteLine("\nThe deck is impty. Shuffling Discard pile!\n\n-------------------------------------");
                    Reshuffle();
                }

                if (currentPlayer == player)
                {
                    playerTurn();
                    if (!playing)
                    {
                        if (PlayAgain()) Reset();
                        else return;
                    }
                    if (playedACard == true) ResolveCard();
                    Thread.Sleep(1000);
                    if (reverse) currentPlayer = currentPlayer.prev;
                    else currentPlayer = currentPlayer.next;
                }
                else
                {
                    opponentTurn();
                }
                System.Console.WriteLine("\n-------------------------------------");
            }
        }

        public bool CheckMove(Card playedCard)
        {
            if (playedCard.color == topCard.color || playedCard.faceVal == topCard.faceVal || playedCard.color == "Wild" || topCard.color == "Wild") return true;
            return false;
        }

        public void playerTurn()
        {
            System.Console.Write($"\nThe top card is a ");
            topCard.PrintCard();
            System.Console.WriteLine(".");

            System.Console.WriteLine("\nYour hand:");
            System.Console.WriteLine(player);
            if (CheckLegalMoves(player))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine("\nWhich card would you like to play?");
                Console.Out.Flush();

                int input;
                while (!(Int32.TryParse(Console.ReadLine(), out input)) || input > player.hand.Count - 1 || input < 0 || !CheckMove(player.hand[input]))
                {
                    System.Console.WriteLine("\nTry again.");
                }
                Console.ResetColor();

                discarded = topCard;
                topCard = player.Play(input);
                playedACard = true;
                discard.cards.Add(discarded);

                if (player.hand.Count == 0)//check for player win
                {
                    System.Console.WriteLine("\nYou win!");
                    playing = false;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine("\nYou have no legal moves! Press enter to draw a card.");
                Console.ResetColor();
                System.Console.ReadLine();

                player.Draw(deck);
                playedACard = false;
                return;
            }
        }

        public void opponentTurn()
        {
            System.Console.WriteLine($"\n{currentPlayer.name} takes their turn.");

            startingTotal = currentPlayer.hand.Count;
            discarded = topCard;
            topCard = currentPlayer.Play(topCard, deck);

            if (startingTotal <= currentPlayer.hand.Count) playedACard = false;
            else
            {
                playedACard = true;
                discard.cards.Add(discarded);
            }

            if (currentPlayer.hand.Count == 0)//check for opponent win
            {
                System.Console.WriteLine("\n" + currentPlayer.name + " wins!");
                if (!PlayAgain()) return;
                else Reset();
            }

            if (playedACard == true) ResolveCard();

            Thread.Sleep(2000);

            if (reverse) currentPlayer = currentPlayer.prev;
            else currentPlayer = currentPlayer.next;
        }

        public bool CheckLegalMoves(Player p)
        {
            foreach (Card c in p.hand)
            {
                if (CheckMove(c)) return true;
            }
            return false;
        }

        public void ResolveCard()
        {
            if (topCard.faceVal == "Reverse")
            {
                if (reverse == true) reverse = false;
                else reverse = true;
            }

            if (topCard.faceVal == "Skip")
            {
                if (reverse) currentPlayer = currentPlayer.prev;
                else currentPlayer = currentPlayer.next;
            }

            if (topCard.faceVal == "Draw +2")
            {
                if (reverse == true) DrawCard(2, currentPlayer.prev);
                else DrawCard(2, currentPlayer.next);
            }

            if (topCard.faceVal == "Card Draw +4")
            {
                if (reverse == true) DrawCard(4, currentPlayer.prev);
                else DrawCard(2, currentPlayer.next);
            }
        }

        public void DrawCard(int amount, Player pl)
        {
            if (pl == player)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine($"\nPress enter to draw {amount} cards.");
                System.Console.ReadLine();
                Console.ResetColor();
            }
            System.Console.WriteLine($"\n{pl.name} draws {amount} cards.");
            pl.Draw(deck, amount);
        }

        public bool PlayAgain()
        {
            List<string> yes = new List<string> { "y", "Y", "Yes", "yes", "YES" };
            List<string> no = new List<string> { "n", "N", "no", "No", "NO" };

            System.Console.WriteLine("\nWould you like to play again?");
            System.Console.Out.Flush();
            string input = System.Console.ReadLine();

            //while the player input is not from one of the lists above, continue to ask
            while (!yes.Contains(input) && !no.Contains(input))
            {
                System.Console.WriteLine("\nPlease enter Yes or No.");
                input = System.Console.ReadLine();
            }
            if (yes.Contains(input)) return true;
            return false;
        }

        public void Reset()
        {
            this.reverse = false;
            this.playedACard = false;
            int numberOpponents = 0;

            Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("\nHow many opponents would you like to play against? (1 to 9)");
            Console.Out.Flush();

            //while the player input is not a number or not a valid number, continue to ask;
            while (!(Int32.TryParse(Console.ReadLine(), out numberOpponents)) || numberOpponents < 1 || numberOpponents > 9)
            {
                System.Console.WriteLine("\nPlease enter a valid number of opponents.");
            }
            Console.ResetColor();

            this.players = new Player[numberOpponents + 1];
            this.players[0] = this.player;
            this.deck = new Deck();
            this.discard = new Deck(true);
            this.currentPlayer = player;

            //create opponents
            for (int i = 1; i <= numberOpponents; i++)
            {
                this.players[i] = new Player("Player " + (i + 1), this);
            }

            for (int i = 0; i < players.Length; i++)
            {
                if (i == 0)
                {
                    players[i].prev = players[players.Length - 1];
                    players[i].next = players[1];
                }
                else if (i == players.Length - 1)
                {
                    if (players.Length == 2) players[i].prev = players[0];
                    else players[i].prev = players[players.Length - 2];
                    players[i].next = players[0];
                }
                else
                {
                    players[i].prev = players[i - 1];
                    players[i].next = players[i + 1];
                }
            }

            startingTotal = 0;
            playing = true;
            currentPlayer = player;
            player.hand = new List<Card>();

            //for each player, dreaw a hand of cards
            foreach (Player p in players)
            {
                for (int i = 0; i < 7; i++)
                {
                    p.Draw(deck);
                }
            }
        }

        public void Reshuffle()
        {
            discard.Shuffle();
            deck.cards = discard.cards;
            discard = new Deck(true);
        }
    }
}