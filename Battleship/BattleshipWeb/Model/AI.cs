using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using HAPI;
using System.Diagnostics;
using System.IO;


namespace BattleshipWeb
{
    public class AI : Player
    {
        public AI(string name) : base(name)
        {

        }
        public override void YourTurn()
        {
            InitBayesianNetwork();
        }
        private Domain InitBayesianNetwork()
        {
            Domain battleShip = new Domain();
            List<LabelledDCNode> shipList = new List<LabelledDCNode>();
            string[] shipNames = new string[5] {"Destroyer", "Submarine",
                                                "Cruiser", "Battleship", "Carrier"};
            int[] shipLengths = new int[5] { 2, 3, 3, 4, 5 };
            //Initializes ships
            for (int i = 0; i < 5; i++)
            {
                shipList.Add(new LabelledDCNode(battleShip));
                shipList[i].SetName(shipNames[i]);
                //Set states and tables for all ships
                shipList[i] = SetAllStatesForShips(shipList[i], shipLengths[i]);
            }

            //((LabelledDCNode)shipList[1]);
            //Initializes constraints (4+3+2+1=10 constraints)
            for (int i = 0; i < 5; i++)
            {
                for (int j = i + 1; j < 5; j++)
                {
                    BooleanDCNode constraint = new BooleanDCNode(battleShip);
                    constraint.AddParent(shipList[i]);
                    constraint.AddParent(shipList[j]);
                    constraint.SetName($"Overlap{i}_{j}");
                    constraint.SetNumberOfStates(2);
                    constraint.SetStateLabel(0, "False");
                    constraint.SetStateLabel(1, "True");
                    constraint = SetAllStatesForConstraints(constraint, shipList[i], shipList[j]);
                }
            }

            //Initializes tiles
            for (int i = 0; i < 10; i++)
            {
                char letter = (char)(i + 65);
                for (int j = 0; j < 10; j++)
                {
                    LabelledDCNode tile = new LabelledDCNode(battleShip);
                    tile.SetName(letter + $"{(j + 1)}");
                    tile.SetLabel($"{i+1}{j + 1}");
                    for (int k = 0; k < 5; k++)
                    {
                        tile.AddParent(shipList[k]);
                    }
                    SetAllStatesForTiles(tile, shipList);
                }
            }
            return battleShip;

        }
        private LabelledDCNode SetAllStatesForShips(LabelledDCNode node, int length)
        {
            //10 is the length of the board, 2 is the dimensions of the board
            //2 skib 180
            //3 skib 160
            //4 skib 140
            //5 skib 120
            int minimumLength = 10 - length + 1;
            int numberOfStates = minimumLength * 2 * 10;
            node.SetNumberOfStates((ulong)numberOfStates);
            for (int orientation = 0; orientation < 2; orientation++)
            {
                for (int i = 0; i < (orientation == 0 ? 10 : minimumLength); i++)
                {
                    for (int j = 0; j < (orientation == 1 ? 10 : minimumLength); j++)
                    {
                        node.SetStateLabel((ulong)(i+j), (orientation == 1 ? "H" : "V") 
                                                                     + $"_{i}{j}");
                        node.GetTable().SetDataItem((ulong)(i + j), 1 / numberOfStates); 
                    }
                }
            }
            return node;

        }
        private BooleanDCNode SetAllStatesForConstraints(BooleanDCNode constraint, LabelledDCNode firstShip, LabelledDCNode secondShip)
        {
            //Example: 10+1-180/20 = 2
            int firstLength = 10 + 1 - firstShip.GetTable().GetData().Length / 20;
            int secondLength = 10 + 1 - secondShip.GetTable().GetData().Length / 20;
            List<Point> firstPoints = new List<Point>();
            List<Point> secondPoints = new List<Point>();
            string firstName, secondName;
            //Iterates through entire tables on constraint's parents
            for (int i = 0; i < firstShip.GetTable().GetData().Length; i++)
            {
                firstName = firstShip.GetStateLabel((ulong)i);
                //Gives coordinates for first ship
                firstPoints = ReturnCoordinates(firstLength, firstName);
                for (int j = 0; j < secondShip.GetTable().GetData().Length; j++)
                {
                    secondName = secondShip.GetStateLabel((ulong)j);
                    //Gives coordinates for second ship
                    secondPoints = ReturnCoordinates(secondLength, secondName);
                    //Checks if ships overlap
                    if (CheckForOverlap(firstPoints, secondPoints))
                    {
                        constraint.GetTable().SetDataItem((ulong)(j + i) * 2, 0);
                        constraint.GetTable().SetDataItem((ulong)(j + i) * 2 + 1, 1);
                    }
                    else
                    {
                        constraint.GetTable().SetDataItem((ulong)(j + i) * 2, 1);
                        constraint.GetTable().SetDataItem((ulong)(j + i) * 2 + 1, 0);
                    }

                }
            }
            return constraint;
        }
        private List<Point> ReturnCoordinates(int length, string name)
        {
            List<Point> pointList = new List<Point>();
            char orientation = name[0];
            int xCord = name[2];
            int yCord = name[3];
            for (int i = 0; i < length; i++)
            {
                if (orientation == 'H')
                {
                    pointList.Add(new Point(xCord+i, yCord));
                }
                else if (orientation == 'V')
                {
                    pointList.Add(new Point(xCord, yCord+i));
                }
                else
                {
                    Debug.WriteLine($"Something seriously wrong with the orientation - said: {orientation}");
                }
            }
            return pointList;
        }
        private bool CheckForOverlap(List<Point> firstPoints, List<Point> secondPoints)
        {
            foreach (Point point in firstPoints)
            {
                if (secondPoints.Contains(point))
                {
                    return true;
                }
            }
            return false;
        }
        private LabelledDCNode SetAllStatesForTiles(LabelledDCNode tile, List<LabelledDCNode> shipList)
        {

            return tile;
        }
        public override void SetShips()
        {
            int orientation;
            string[] shipNames = new string[5] {"Destroyer", "Submarine",
                                                "Cruiser", "Battleship", "Carrier"};
            int[] shipLengths = new int[5] { 2, 3, 3, 4, 5 };

            for (int i = 0; i < 5; i++)
            {
                bool correctlyPlaced = false;
                while (!correctlyPlaced)
                {
                    Point point = new Point();
                    point.X = new Random().Next(0, 10);
                    point.Y = new Random().Next(0, 10);

                    orientation = new Random().Next(0, 2);
                    correctlyPlaced = board.PlaceShips(new Ship(shipNames[i], shipLengths[i], point,
                                                       orientation == 0 ? 'H' : 'V'));
                }
            }
        }
    }

}
