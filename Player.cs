using System;
using System.Collections.Generic;

namespace Uno
{
    public class Player
    {
        public Game game { get; set; }
        public string name { get; set; }
        public List<Card> hand { get; set; }
        public Player prev { get; set; }
        public Player next { get; set; }

        public Player(string name, Game game)
        {
            this.name = name;
            this.hand = new List<Card>();
            this.prev = null;
            this.next = null;
            this.game = game;
        }

        public Card Draw(Deck deck)
        {
            hand.Add(deck.Deal());
            return hand[hand.Count - 1];
        }

        public void Draw(Deck deck, int cards)
        {
            for (int i = 0; i < cards; i++)
            {
                if (deck.cards.Count == 0) game.Reshuffle();
                hand.Add(deck.Deal());
            }
        }

        public Card Play(int val)
        {
            Card temp = hand[val];
            hand.RemoveAt(val);
            return temp;
        }

        public Card Play(Card topCard, Deck deck)
        {
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].color == topCard.color || hand[i].faceVal == topCard.faceVal || topCard.color == "Wild")
                {
                    System.Console.Write($"\n{name} plays a ");
                    hand[i].PrintCard();
                    System.Console.WriteLine($", and has {hand.Count - 1} cards remaining.");
                    return Play(i);
                }
            }

            Draw(deck);

            System.Console.WriteLine("\n" + name + " draws a card, and now has " + hand.Count + " cards.");
            return topCard;
        }


        public override string ToString()
        {
            for (int i = 0; i < hand.Count; i++)
            {
                System.Console.Write("     " + i + " - ");
                hand[i].PrintCard();
                System.Console.WriteLine();
            }

            return $"\nYou have {hand.Count} cards.";
        }
    }
}
