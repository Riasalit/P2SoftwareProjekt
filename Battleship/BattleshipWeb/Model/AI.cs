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
        private List<LabelledDCNode> shipList = new List<LabelledDCNode>();
        private List<List<LabelledDCNode>> tileList = new List<List<LabelledDCNode>>();
        private List<Point> previousHits = new List<Point>();
        //             index in plural
        private List<int> indices = new List<int>();
        private Domain battleship;
        public AI(string name) : base(name)
        {
            battleship = new Domain();
            InitBayesianNetwork();
        }
        public override void YourTurn()
        {
            Point shootingPoint;
            string shootingResult;
            Dictionary<Point, double> probabilities = new Dictionary<Point, double>();
            battleship.Propagate(Domain.Equilibrium.H_EQUILIBRIUM_SUM,
                                 Domain.EvidenceMode.H_EVIDENCE_MODE_NORMAL);
            //Gets all the probabilities for a ship to be on the tiles
            CalculateProbabilities(probabilities);
            //Finds point with highest probability to contain a ship
            shootingPoint = FindShootingPoint(probabilities);
            points.Add(shootingPoint);
            //Shoots at the board
            shootingResult = ShootOpponent(shootingPoint);
            //Inserts evidence to the bayesian network
            battleship.SaveAsKB("TestStuffs.hkb");
            SetEvidence(shootingResult, shootingPoint);
            
        }
        private void CalculateProbabilities(Dictionary<Point, double> probabilities)
        {
            double probability;
            probabilities.Clear();
            for (int i = 0; i < Settings.boardSize; i++)
            {
                probability = 0;
                foreach (LabelledDCNode tile in tileList[i])
                {
                    //Adds all the true value to a probability dictionary 
                    //to a probability mapping from a point (tile)
                    probability += tile.GetBelief(1);
                }
                Point tilePoint = new Point(i / Settings.boardWidth, i % Settings.boardWidth);
                probabilities.Add(tilePoint, probability);
            }
        }
        private void SetEvidence(string shootingResult, Point shootingPoint)
        {
            //"You sunk {ship}" "You hit a ship" "You missed"
            long shipStateIndex;
            int index = shootingPoint.X * Settings.boardWidth + shootingPoint.Y;
            if (shootingResult == "You hit a ship")
            {
                foreach (LabelledDCNode tile in tileList[index])
                {
                    tile.SelectState(1);
                }
                previousHits.Add(shootingPoint);
            }
            else if (shootingResult == "You missed")
            {
                foreach (LabelledDCNode tile in tileList[index])
                {
                    tile.SelectState(0);
                }
            }
            else
            {
                previousHits.Add(shootingPoint);
                foreach (LabelledDCNode tile in tileList[index])
                {
                    tile.SelectState(1);
                }
                string shipName = shootingResult.Split(' ')[2];
                for (int i = 0; i < Settings.shipCount; i++)
                {
                    if (Settings.shipNames[i] == shipName)
                    {
                        indices.Add(i);
                        indices.Sort();
                        int count = 0;
                        foreach(int shipIndex in indices)
                        {
                            count += Settings.shipLengths[shipIndex];
                        }
                        if(count == previousHits.Count)
                        {
                            foreach(int shipIndex in indices)
                            {
                                shipStateIndex = shipList[shipIndex].GetStateIndex(FindShipPos(Settings.shipLengths[i]));
                                shipList[shipIndex].SelectState((ulong)shipStateIndex);
                            }
                            indices.Clear();
                        }
                    }
                }
            }
        }
        private string FindShipPos(int length)
        {
            string returnString;
            for (int i = 0; i < previousHits.Count; i++)
            {
                if (FindShipPosHelpMethod(length, 0, 1, previousHits[i]))
                {
                    returnString = $"V_{previousHits[i].X}{previousHits[i].Y}";
                    RemoveCoordFromHitList(0, 1, previousHits[i], length);
                    return returnString;
                }
                if (FindShipPosHelpMethod(length, 1, 0, previousHits[i]))
                {
                    returnString = $"H_{previousHits[i].X}{previousHits[i].Y}";
                    RemoveCoordFromHitList(1, 0, previousHits[i], length);
                    return returnString;
                }
            }
            throw new ArgumentException("Something went wrong while trying to find sunken ship start coordinate.");
        }

        private bool FindShipPosHelpMethod(int length, int xDir, int yDir, Point coord) 
        {
            int testLength = length - 1;
            Point newCoord = new Point(coord.X + xDir, coord.Y + yDir);
            if (testLength == 0)
            {
                return true;
            }
            else if (!previousHits.Contains(newCoord))
            {
                return false;
            }
            else
            {
                return FindShipPosHelpMethod(testLength, xDir, yDir, newCoord);
            }
        }

        private void RemoveCoordFromHitList
            (int xDir, int yDir, Point coord, int length)
        {
            for (int i = 0; i < length; i++)
            {
                previousHits.Remove(new Point(coord.X + xDir * i, coord.Y + yDir * i));
            }
        }

        private Point FindShootingPoint(Dictionary<Point, double> probabilities)
        {
            Dictionary<Point, double> privateProbabilities = probabilities;
            double maxValue = 0;
            foreach (Point point in points)
            {
                privateProbabilities.Remove(point);
            }
            maxValue = privateProbabilities.Values.Max();
            Point[] shootingPoints = privateProbabilities.Where(p => p.Value == maxValue).Select(p => p.Key).ToArray();
            return shootingPoints[new Random().Next(0, shootingPoints.Length)];
        }
        public Domain InitBayesianNetwork()
        {
            //Initializes ships
            for (int i = 0; i < Settings.shipCount; i++)
            {
                shipList.Add(new LabelledDCNode(battleship));
                shipList[i].SetName(Settings.shipNames[i]);
                //Set states and tables for all ships
                shipList[i] = SetAllStatesForShips(shipList[i], Settings.shipLengths[i]);
            }

            //Initializes constraints (4+3+2+1=10 constraints) with the intent 
            //to prevent ships from overlapping 
            MakeStatesForOverlap(shipList);

            //Initializes tiles
            for (int i = 0; i < Settings.boardWidth; i++)
            {
                char letter = (char)(i + 'A');
                for (int j = 0; j < Settings.boardWidth; j++)
                {
                    tileList.Add(MakeStatesForTiles(shipList, $"{letter}{j}S"));
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
            double minimumLength = Settings.boardWidth - length + 1;
            double numberOfStates = minimumLength * 2 * Settings.boardWidth;
            ulong count = 0;
            ship.SetNumberOfStates((ulong)numberOfStates);
            for (int orientation = 0; orientation < 2; orientation++)
            {
                for (int i = 0; i < (orientation == 0 ? Settings.boardWidth : minimumLength); i++)
                {
                    for (int j = 0; j < (orientation == 1 ? Settings.boardWidth : minimumLength); j++)
                    {
                        ship.SetStateLabel(count, (orientation == 1 ? "H" : "V") + $"_{i}{j}");
                        ship.GetTable().SetDataItem(count, 1 / numberOfStates);
                        count++;
                    }
                }
            }
            return ship;
        }
        private void SetStatesForOverlaps(BooleanDCNode overlap, LabelledDCNode secondShip, LabelledDCNode firstShip)
        {
            //Example: 10+1-180/20 = 2
            int firstLength = Settings.boardWidth + 1 - firstShip.GetTable().GetData().Length
                                                   / (2 * Settings.boardWidth);
            int secondLength = Settings.boardWidth + 1 - secondShip.GetTable().GetData().Length
                                                   / (2 * Settings.boardWidth);
            List<Point> firstPoints = new List<Point>();
            List<Point> secondPoints = new List<Point>();
            ulong count = 0;
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
                        overlap.GetTable().SetDataItem(count++, 0);
                        overlap.GetTable().SetDataItem(count++, 1);
                    }
                    else
                    {
                        overlap.GetTable().SetDataItem(count++, 1);
                        overlap.GetTable().SetDataItem(count++, 0);
                    }
                }
            }
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
                    pointList.Add(new Point(xCoord + i, yCoord));
                }
                else if (orientation == 'V')
                {
                    pointList.Add(new Point(xCoord, yCoord + i));
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
        private List<LabelledDCNode> MakeStatesForTiles(List<LabelledDCNode> shipList, string name)
        {
            List<LabelledDCNode> tempTileList = new List<LabelledDCNode>();
            LabelledDCNode tile;
            //køre ikke ved 1 skib
            for (int i = 0; i < shipList.Count; i++)
            {
                for (int j = i + 1; j < shipList.Count; j++)
                {
                    tile = new LabelledDCNode(battleship);
                    tile.SetNumberOfStates(2);
                    tile.SetStateLabel(0, "False");
                    tile.SetStateLabel(1, "True");
                    tile.AddParent(shipList[i]);
                    tile.AddParent(shipList[j]);
                    SetAllStatesForTiles(tile, shipList[i], shipList[j], name);
                    tile.SetName($"{name}{i}_{j}");
                    tempTileList.Add(tile);
                }
            }
            return tempTileList;
        }
        private void MakeStatesForOverlap(List<LabelledDCNode> shipList)
        {
            BooleanDCNode overlap;
            for (int i = 0; i < shipList.Count; i++)
            {
                for (int j = i + 1; j < shipList.Count; j++)
                {
                    overlap = new BooleanDCNode(battleship);
                    overlap.SetNumberOfStates(2);
                    overlap.SetStateLabel(0, "False");
                    overlap.SetStateLabel(1, "True");
                    overlap.AddParent(shipList[i]);
                    overlap.AddParent(shipList[j]);
                    SetStatesForOverlaps(overlap, shipList[i], shipList[j]);
                    overlap.SelectState(0);
                    overlap.SetName($"Overlap{i}_{j}");
                }
            }
        }
        private void SetAllStatesForTiles(LabelledDCNode tile, LabelledDCNode secondShip, 
                                          LabelledDCNode  firstShip, string name)
        {
            List<Point> firstPoints = new List<Point>();
            List<Point> secondPoints = new List<Point>();
            int xCoord = name[0] - 'A';
            int yCoord = name[1] - '0';
            string firstName, secondName;
            Point tilePlace = new Point(xCoord, yCoord);
            int firstLength = Settings.boardWidth + 1 - firstShip.GetTable().GetData().Length
                                                  / (2 * Settings.boardWidth);
            int secondLength = Settings.boardWidth + 1 - secondShip.GetTable().GetData().Length
                                                   / (2 * Settings.boardWidth);
            ulong count = 0;
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
                    if (secondPoints.Contains(tilePlace) || firstPoints.Contains(tilePlace))
                    {
                        tile.GetTable().SetDataItem(count++, 0);
                        tile.GetTable().SetDataItem(count++, 1);
                    }
                    else
                    {
                        tile.GetTable().SetDataItem(count++, 1);
                        tile.GetTable().SetDataItem(count++, 0);
                    }
                }
            }
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
            for (int i = 0; i < Settings.shipCount; i++)
            {
                bool correctlyPlaced = false;
                while (!correctlyPlaced)
                {
                    Point point = new Point();
                    point.X = new Random().Next(0, Settings.boardWidth);
                    point.Y = new Random().Next(0, Settings.boardWidth);
                    orientation = new Random().Next(0, 2);
                    correctlyPlaced = board.PlaceShips(new Ship(Settings.shipNames[i], Settings.shipLengths[i], point,
                                                       orientation == 0 ? 'H' : 'V'));
                }
            }
        }
    }
}
