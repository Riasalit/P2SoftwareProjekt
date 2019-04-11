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
        List<LabelledDCNode> shipList = new List<LabelledDCNode>();
        List<List<LabelledDCNode>> tileList = new List<List<LabelledDCNode>>();
        Domain battleship;
        public AI(string name) : base(name)
        {
            battleship = new Domain();
            InitBayesianNetwork();
        }
        public override void YourTurn()
        {
            double probability;
            Dictionary<Point, double> probabilities = new Dictionary<Point, double>();
            for (int i = 0; i < 100; i++)
            {
                probability = 0;
                foreach (LabelledDCNode tile in tileList[i] )
                {
                    //Adds all the true value to a probability dictionary 
                    //to a probability mapping from a point (tile)
                    probability += tile.GetBelief(1);
                }
                Point tilePoint = new Point(i / 10, i % 10);
                probabilities.Add(tilePoint, probability);
            }
            ShootOpponent(FindShootingPoint(probabilities));
            //"You sunk {ship}!" "You hit a ship" "You missed"
            //Insert evidence
            foreach (LabelledDCNode tile in tileList[99])
            {
                tile.SelectState(1);
            }
        }
        private Point FindShootingPoint(Dictionary<Point, double> probabilities)
        {
            double maxValue = 0;
            maxValue = probabilities.Values.Max();

            Point[] points = probabilities.Where(p => p.Value == maxValue)
                                          .Select(p => p.Key).ToArray();
            return points[new Random().Next(0, points.Length)];
            
        }
        public Domain InitBayesianNetwork()
        {

            string[] shipNames = new string[5] {"Destroyer", "Submarine",
                                                "Cruiser", "battleship", "Carrier"};
            int[] shipLengths = new int[5] { 2, 3, 3, 4, 5 };
            //Initializes ships
            for (int i = 0; i < 5; i++)
            {
                shipList.Add(new LabelledDCNode(battleship));
                shipList[i].SetName(shipNames[i]);
                //Set states and tables for all ships
                shipList[i] = SetAllStatesForShips(shipList[i], shipLengths[i]);
            }

            //Initializes constraints (4+3+2+1=10 constraints) with the intent 
            //to prevent ships from overlapping 

            MakeStatesForDivorcing(shipList, false, "Overlap");

       
            //Initializes tiles
            for (int i = 0; i < 10; i++)
            {
                char letter = (char)(i + 65);
                for (int j = 0; j < 10; j++)
                {
                    tileList.Add(MakeStatesForDivorcing(shipList, true, $"{letter}{j}S"));
                }
            }
            battleship.Compile();

            Console.WriteLine("is domain alive?: " + battleship.IsAlive());

            //GetNodeByName("A0S0_1").

            battleship.SaveAsKB("hugintest.hkb");
            return battleship;

        }
        private LabelledDCNode SetAllStatesForShips(LabelledDCNode ship, int length)
        {
            //10 is the length of the board, 2 is the dimensions of the board
            //Length 2 ship: 180 dataitems
            //Length 3 ship: 160 dataitems
            //Length 4 ship: 140 dataitems
            //Length 5 ship: 120 dataitems
            double minimumLength = 10 - length + 1;
            double numberOfStates = minimumLength * 2 * 10;
            ulong count = 0;
            ship.SetNumberOfStates((ulong)numberOfStates);
            for (int orientation = 0; orientation < 2; orientation++)
            {
                for (int i = 0; i < (orientation == 0 ? 10 : minimumLength); i++)
                {
                    for (int j = 0; j < (orientation == 1 ? 10 : minimumLength); j++)
                    {
                        ship.SetStateLabel((count), (orientation == 1 ? "H" : "V") 
                                                                     + $"_{i}{j}");
                        ship.GetTable().SetDataItem((count), (1 / numberOfStates));
                        count++;
                    }
                }
            }
            return ship;

        }
        private BooleanDCNode SetAllStatesForConstraints(BooleanDCNode overlap, LabelledDCNode firstShip, LabelledDCNode secondShip)
        {
            //Example: 10+1-180/20 = 2
            int firstLength = 10 + 1 - firstShip.GetTable().GetData().Length / 20;
            int secondLength = 10 + 1 - secondShip.GetTable().GetData().Length / 20;
            List<Point> firstPoints = new List<Point>();
            List<Point> secondPoints = new List<Point>();
            ulong count = ulong.MaxValue; 
            string firstName, secondName;
            //Iterates through entire tables on constraint's parents
            for (int i = 0; i < (int)firstShip.GetNumberOfStates(); i++)
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
                    //Checks if any ship coordinates overlap
                    if (CheckForOverlap(firstPoints, secondPoints))
                    {
                        count++;
                        overlap.GetTable().SetDataItem(count, 0);
                        count++;
                        overlap.GetTable().SetDataItem(count, 1);
                        
                    }
                    else
                    {
                        count++;
                        overlap.GetTable().SetDataItem(count, 1);
                        count++;
                        overlap.GetTable().SetDataItem(count, 0);
                    }
                }
            }
            return overlap;
        }
        private List<Point> ReturnCoordinates(int length, string name)
        {
            List<Point> pointList = new List<Point>();
            char orientation = name[0];
            int xCoord = name[2] - '0';
            int yCoord = name[3] - '0';
            for (int i = 0; i < length; i++)
            {
                if (orientation == 'H')
                {
                    pointList.Add(new Point(xCoord+i, yCoord));
                }
                else if (orientation == 'V')
                {
                    pointList.Add(new Point(xCoord, yCoord+i));
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
        private List<LabelledDCNode> MakeStatesForDivorcing(List<LabelledDCNode> shipList, bool isTile, string name)
        {
            List<LabelledDCNode> tempTileList = new List<LabelledDCNode>();
            Node node;
            for (int i = 0; i < shipList.Count; i++)
            {
                for (int j = i + 1; j < shipList.Count; j++)
                {
                    if (isTile)
                    {
                        node = new LabelledDCNode(battleship);
                        ((LabelledDCNode)node).SetNumberOfStates(2);
                        ((LabelledDCNode)node).SetStateLabel(0, "False");
                        ((LabelledDCNode)node).SetStateLabel(1, "True");
                        node.AddParent(shipList[i]);
                        node.AddParent(shipList[j]);
                        node = SetAllStatesForTiles(((LabelledDCNode)node), shipList[i], shipList[j], name);
                    }
                    else
                    {
                        node = new BooleanDCNode(battleship);
                        ((BooleanDCNode)node).SetNumberOfStates(2);
                        ((BooleanDCNode)node).SetStateLabel(0, "False");
                        ((BooleanDCNode)node).SetStateLabel(1, "True");
                        node.AddParent(shipList[i]);
                        node.AddParent(shipList[j]);
                        node = SetAllStatesForConstraints(((BooleanDCNode)node), shipList[i], shipList[j]);
                        ((BooleanDCNode)node).SelectState(0);
                    }
                    node.SetName($"{name}{i}_{j}");
                    tempTileList.Add((LabelledDCNode)node);
                }
            }
            return tempTileList;
        }
        private LabelledDCNode SetAllStatesForTiles(LabelledDCNode tile, LabelledDCNode firstShip, LabelledDCNode secondShip, string name)
        {
            List<Point> firstPoints = new List<Point>();
            List<Point> secondPoints = new List<Point>();
            List<Point> concatList = new List<Point>();
            int xCoord = name[0] - 'A';
            int yCoord = name[1] - '0';
            string firstName, secondName;
            Point tilePlace = new Point(xCoord, yCoord);
            int firstLength = 10 + 1 - firstShip.GetTable().GetData().Length / 20;
            int secondLength = 10 + 1 - secondShip.GetTable().GetData().Length / 20;
            ulong count = ulong.MaxValue;
            for (int i = 0; i < (int)firstShip.GetNumberOfStates(); i++)
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
                    concatList = secondPoints.Concat(firstPoints).ToList();
                    secondPoints.Concat(firstPoints);
                    if (secondPoints.Contains(tilePlace) || firstPoints.Contains(tilePlace)) 
                    {
                        count++;
                        tile.GetTable().SetDataItem(count, 0);
                        count++;
                        tile.GetTable().SetDataItem(count, 1);
                    }
                    else
                    {
                        count++;
                        tile.GetTable().SetDataItem(count, 1);
                        count++;
                        tile.GetTable().SetDataItem(count, 0);
                    }
                }
            }
            return tile;
        }
        private LabelledDCNode SetAllStatesForTile(LabelledDCNode tile, List<LabelledDCNode> shipList)
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
                                    //Console.WriteLine($"{tile.GetName()}:{tile.GetTable().GetDataItem(count)}");
                                    count++;
                                    tile.GetTable().SetDataItem(count, 1);
                                    //Console.WriteLine($"{tile.GetName()}:{tile.GetTable().GetDataItem(count)}");
                                }
                                else
                                {
                                    count++;
                                    tile.GetTable().SetDataItem(count, 1);
                                    //Console.WriteLine($"{tile.GetName()} : {tile.GetTable().GetDataItem(count)}");
                                    count++;
                                    tile.GetTable().SetDataItem(count, 0);
                                    //Console.WriteLine($"{tile.GetName()} : {tile.GetTable().GetDataItem(count)}");
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
