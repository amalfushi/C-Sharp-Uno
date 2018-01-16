using System;
using System.Collections.Generic;

namespace Uno
{
    public class Card
    {
        public string color { get; set; }
        public ConsoleColor consoleColor { get; set; }
        public string faceVal { get; set; }
        public int value { get; set; }

        public Card(string color, int value)
        {
            this.color = color;
            this.value = value;

            switch (value)
            {
                case 10:
                    faceVal = "Skip";
                    break;
                case 11:
                    faceVal = "Reverse";
                    break;
                case 12:
                    faceVal = "Draw +2";
                    break;
                case 13:
                    faceVal = "Card";
                    break;
                case 14:
                    faceVal = "Card Draw +4";
                    break;
                default:
                    faceVal = value.ToString();
                    break;
            }

            switch (color)
            {
                case "Red":
                    consoleColor = ConsoleColor.Red;
                    break;
                case "Blue":
                    consoleColor = ConsoleColor.Blue;
                    break;
                case "Green":
                    consoleColor = ConsoleColor.Green;
                    break;
                case "Yellow":
                    consoleColor = ConsoleColor.Yellow;
                    break;
                default:
                    break;
            }
        }

        public void PrintCard()
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            System.Console.Write(" ");
            if (color == "Wild")
            {
                string cardString = ToString();
                List<ConsoleColor> colors = new List<ConsoleColor> { ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Blue };

                for (int c = 0; c < cardString.Length; c++)
                {
                    Console.ForegroundColor = colors[c];
                    colors.Add(colors[c]);
                    System.Console.Write(cardString[c]);

                }
            }
            else
            {
                Console.ForegroundColor = consoleColor;
                System.Console.Write(ToString());
            }
            System.Console.Write(" ");
            Console.ResetColor();
        }

        public override string ToString()
        {
            return $"{color} {faceVal}";
        }
    }
}
