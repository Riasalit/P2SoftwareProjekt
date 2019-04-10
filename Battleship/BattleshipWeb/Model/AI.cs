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
        public Domain InitBayesianNetwork()
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
            Domain d = new Domain();
            BooleanDCNode BLAH = new BooleanDCNode(battleShip);
            BLAH.SetName("hehehehe");
            BLAH.SetNumberOfStates(2);
            BLAH.GetTable().SetDataItem(1, 1);
            //Initializes tiles
            for (int i = 0; i < 10; i++)
            {
                char letter = (char)(i + 65);
                for (int j = 0; j < 10; j++)
                {
                    LabelledDCNode tile = new LabelledDCNode(battleShip);
                    tile.SetName(letter + $"{(j + 1)}");
                    tile.SetLabel($"{i}{j }");
                    tile.SetNumberOfStates(2);
                    tile.SetStateLabel(0, "False");
                    tile.SetStateLabel(1, "True");
                    for (int k = 0; k < 5; k++)
                    {
                        Console.WriteLine($"{k} : {tile.GetTable().GetSize()}");
                        tile.AddParent(shipList[k]);
                    }
                    tile = SetAllStatesForTiles(tile, shipList);
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
            ulong k = 0;
            node.SetNumberOfStates((ulong)numberOfStates);
            for (int orientation = 0; orientation < 2; orientation++)
            {
                for (int i = 0; i < (orientation == 0 ? 10 : minimumLength); i++)
                {
                    for (int j = 0; j < (orientation == 1 ? 10 : minimumLength); j++)
                    {
                        //FEJL!!! jeg har tjekket - jakob. Istedet for (i+j) skal der være en int, eks. k der incrementes efter hvert label-set. 
                        //Viser imorgen! :-)
                        node.SetStateLabel((ulong)(k), (orientation == 1 ? "H" : "V") 
                                                                     + $"_{i}{j}");
                        node.GetTable().SetDataItem((ulong)(k), 1 / numberOfStates);
                        k++;
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
            ulong count = ulong.MaxValue; 
            string firstName, secondName;
            //Iterates through entire tables on constraint's parents
            //istedet for firstShip.GetTable().GetData().Length kan vi skrive firstShip.GetNumberOfStates()
            for (int i = 0; i < firstShip.GetTable().GetData().Length; i++)
            {
                firstName = firstShip.GetStateLabel((ulong)i);
                //Gives coordinates for first ship
                firstPoints = ReturnCoordinates(firstLength, firstName);
                for (int j = 0; j < (int)secondShip.GetNumberOfStates(); j++)
                {
                    //secondShip.GetTable().GetData().Length
                    secondName = secondShip.GetStateLabel((ulong)j);
                    //Gives coordinates for second ship
                    secondPoints = ReturnCoordinates(secondLength, secondName);
                    //Checks if ships overlap
                    if (CheckForOverlap(firstPoints, secondPoints))
                    {
                        count++;
                        constraint.GetTable().SetDataItem(count, 0);
                        count++;
                        constraint.GetTable().SetDataItem(count, 1);
                        
                    }
                    else
                    {
                        count++;
                        constraint.GetTable().SetDataItem(count, 1);
                        count++;
                        constraint.GetTable().SetDataItem(count, 0);
                    }
                    //Console.WriteLine($"name:{constraint.GetName()}");
                    //Console.WriteLine($"state ship1: {firstShip.GetStateLabel((ulong)(i))}, lenght: {firstLength}");
                    //Console.WriteLine($"state ship2: {secondShip.GetStateLabel((ulong)j)}, lenght: {secondLength}");
                    //Console.WriteLine($"false: {constraint.GetTable().GetDataItem((ulong)(k-1))}");
                    //Console.WriteLine($"true: {constraint.GetTable().GetDataItem((ulong)(k))}");
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
            List<List<Point>> pointList = new List<List<Point>>();
            int[] lengthArray = new int[5];
            for (int i = 0; i < 5; i++)
            {
                lengthArray[i] = 10 + 1 - shipList[i].GetTable().GetData().Length / 20;
            }
            ulong count = ulong.MaxValue;
            for (int i = 0; i < (int)shipList[0].GetNumberOfStates(); i++)
            {
                pointList.Add(ReturnCoordinates(lengthArray[0], shipList[0].GetStateLabel((ulong)i)));
                for (int j = 0; j < (int)shipList[1].GetNumberOfStates(); j++)
                {
                    pointList.Add(ReturnCoordinates(lengthArray[1], shipList[1].GetStateLabel((ulong)j)));
                    for (int k = 0; k < (int)shipList[2].GetNumberOfStates(); k++)
                    {
                        pointList.Add(ReturnCoordinates(lengthArray[2], shipList[2].GetStateLabel((ulong)k)));
                        for (int l = 0; l < (int)shipList[3].GetNumberOfStates(); l++)
                        {
                            pointList.Add(ReturnCoordinates(lengthArray[3], shipList[3].GetStateLabel((ulong)k)));
                            for (int m = 0; m < (int)shipList[4].GetNumberOfStates(); m++)
                            {
                                pointList.Add(ReturnCoordinates(lengthArray[4], shipList[4].GetStateLabel((ulong)m)));
                                if (IsAnyShipOnTile(tile.GetLabel(), pointList))
                                {
                                    count++;
                                    tile.GetTable().SetDataItem(count, 0);
                                    Console.WriteLine($"{tile.GetName()}:{tile.GetTable().GetDataItem(count)}");
                                    count++;
                                    tile.GetTable().SetDataItem(count, 1);
                                    Console.WriteLine($"{tile.GetName()}:{tile.GetTable().GetDataItem(count)}");
                                }
                                else
                                {
                                    count++;
                                    tile.GetTable().SetDataItem(count, 1);
                                    Console.WriteLine($"{tile.GetName()} : {tile.GetTable().GetDataItem(count)}");
                                    count++;
                                    tile.GetTable().SetDataItem(count, 0);
                                    Console.WriteLine($"{tile.GetName()} : {tile.GetTable().GetDataItem(count)}");
                                }
                            }
                        }
                    }
                }
            }
            return tile;
        }
        private bool IsAnyShipOnTile(string name, List<List<Point>> shipList)
        {
            Point getCord = new Point(name[0], name[1]);
            foreach (List<Point> list in shipList)
            {
                if (list.Contains(getCord))
                {
                    return true;
                }
            }
            return false;
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
