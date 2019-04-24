using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using HAPI;
using System.Diagnostics;

namespace BattleshipWeb
{
    public class AI : Player
    {
        private Domain battleship;
        private List<LabelledDCNode> shipList = new List<LabelledDCNode>();
        private List<List<LabelledDCNode>> tileList = new List<List<LabelledDCNode>>();
        private List<Point> previousHits = new List<Point>();
        private List<int> Indexes = new List<int>();

        public AI(string name) : base(name)
        {
            battleship = new Domain();
            InitBayesianNetwork();
        }
        // Executes all necessary methods on the player's turn
        public override void YourTurn()
        {
            Point shootingPoint;
            string shootingResult;
            Dictionary<Point, double> probabilities = new Dictionary<Point, double>();

            CalculateProbabilities(probabilities);
            // Finds point with highest probability to contain a ship
            shootingPoint = FindShootingPoint(probabilities);
            pointsShot.Add(shootingPoint);
            // Shoots at the board
            shootingResult = ShootOpponent(shootingPoint);
            /***************
            * Slet!!!!!!   *
            ****************/           
            battleship.SaveAsKB("TestStuffswithevidence.hkb");
            // Inserts evidence to the bayesian network
            SetEvidence(shootingResult, shootingPoint);
            battleship.Propagate(Domain.Equilibrium.H_EQUILIBRIUM_SUM,
                                 Domain.EvidenceMode.H_EVIDENCE_MODE_NORMAL);
        }
        // Calculates the probablility for each tile and adds the result to the dictionary
        private void CalculateProbabilities(Dictionary<Point, double> probabilities)
        {
            double probability;

            probabilities.Clear();
            for (int i = 0; i < Settings.boardSize; i++)
            {
                probability = tileList[i][tileList[i].Count - 1].GetBelief(1);
                Point tilePoint = new Point(i / Settings.boardWidth, i % Settings.boardWidth);
                probabilities.Add(tilePoint, probability);
            }
        }
        // Finds the points with the greatest probablities and returns one of these randomly
        private Point FindShootingPoint(Dictionary<Point, double> probabilities)
        {
            Dictionary<Point, double> temp = probabilities;
            double maxValue = 0;
            // Removes points already shot from the dictionary
            foreach (Point point in pointsShot)
            {
                temp.Remove(point);
            }
            maxValue = temp.Values.Max();
            Point[] shootingPoints = temp.Where(p => p.Value == maxValue).Select(p => p.Key).ToArray();

            return shootingPoints[new Random().Next(0, shootingPoints.Length)];
        }
        private void SetEvidence(string shootingResult, Point shootingPoint)
        {
            long shipStateIndex;
            int index = shootingPoint.X * Settings.boardWidth + shootingPoint.Y;
            int divorcedTiles = tileList[index].Count - 1;

            if (shootingResult == "You hit a ship")
            {
                // Only sets evidence for the last tile node 
                tileList[index][divorcedTiles].SelectState(1);
                previousHits.Add(shootingPoint);
            }
            else if (shootingResult == "You missed")
            {
                tileList[index][divorcedTiles].SelectState(0);
            }
            else
            {
                string shipName = shootingResult.Split(' ')[2];
                int totalShipLengths = 0;

                previousHits.Add(shootingPoint);
                previousHits.OrderBy(p => p.X).ThenBy(p => p.Y).Reverse();
                tileList[index][divorcedTiles].SelectState(1);
                for (int i = 0; i < Settings.shipCount; i++)
                {
                    if (Settings.shipNames[i] == shipName)
                    {
                        totalShipLengths = 0;
                        // Indexes keeps track of where in the list of ship names the sunken ship is
                        Indexes.Add(i);
                        // Sorts so evidence is set for the largest ships first
                        Indexes.Sort();

                        foreach (int shipIndex in Indexes)
                        {
                            totalShipLengths += Settings.shipLengths[shipIndex];
                        }
                        /*****************************************************
                        * REEEET!!!!!!                                       *
                        * Sunken ships bliver ikke fjernet fra previouseHits *                       
                        ******************************************************/

                        // Inserts evidence only if there is one possible position
                        if (totalShipLengths == previousHits.Count)
                        {
                            // Ændre navn pls
                            string noget;

                            foreach (int shipIndex in Indexes)
                            {
                                noget = FindShipPos(Settings.shipLengths[shipIndex], shipList[shipIndex]);
                                shipStateIndex = shipList[shipIndex].GetStateIndex(noget);
                                shipList[shipIndex].SelectState((ulong)shipStateIndex);
                            }
                            Indexes.Clear();
                        }
                    }
                }
            }
        }
        private string FindShipPos(int length, LabelledDCNode ship)
        {
            List<List<Point>> allPossiblePositions = new List<List<Point>>();
            List<double> beliefs = new List<double>();
            List<string> labels = new List<string>();
            double bestBelief = 0;
            string label = "";
            ulong beliefIndex;

            for (int i = 0; i < previousHits.Count; i++)
            {
                if (FindShipPosHelpMethod(length, 0, 1, previousHits[i]))
                {
                    allPossiblePositions.Add(CreateShipPositionsList(length, 'V', previousHits[i]));
                }
                else if (FindShipPosHelpMethod(length, 1, 0, previousHits[i]))
                {
                    allPossiblePositions.Add(CreateShipPositionsList(length, 'H', previousHits[i]));
                }
            }
            foreach (List<Point> list in allPossiblePositions)
            {
                if (list[0].Y - list[1].Y == 0)
                {
                    label = $"H_{list[0].X}{list[0].Y}";
                }
                else
                {
                    label = $"V_{list[0].X}{list[0].Y}";
                }
                beliefIndex = (ulong)ship.GetStateIndex(label);
                beliefs.Add(ship.GetBelief(beliefIndex));
                labels.Add(label);
            }
            for (int i = 0; i < beliefs.Count; i++)
            {
                if (beliefs[i] > bestBelief)
                {
                    label = labels[i];
                }
            }
            if (label != "")
            {
                return label;
            }
            throw new ArgumentException("Something went wrong while trying to find sunken ship start coordinate.");
        }
        private List<Point> CreateShipPositionsList(int length, char direction, Point start)
        {
            List<Point> returnList = new List<Point>();
            if(direction == 'H')
            {
                for (int i = 0; i < length; i++)
                {
                    returnList.Add(new Point(start.X + i, start.Y));
                }
            }
            else if (direction == 'V')
            {
                for (int i = 0; i < length; i++)
                {
                    returnList.Add(new Point(start.X, start.Y + i));
                }
            }
            else
            {
                throw new ArgumentException("invalid direction");
            }
            return returnList;
        }
        private bool FindShipPosHelpMethod(int length, int xDir, int yDir, Point coord)
        {
            int testLength = length - 1;
            Point newCoord = new Point(coord.X + xDir, coord.Y + yDir);

            if (testLength == 0)
            {
                return true;
            }
            if (!previousHits.Contains(newCoord))
            {
                return false;
            }

            return FindShipPosHelpMethod(testLength, xDir, yDir, newCoord);
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
            Console.WriteLine();
            battleship.Compress();
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
            int templistIndex = 0;
            LabelledDCNode tile;
            int constraintNumber = 0;
            tile = new LabelledDCNode(battleship);
            tile.SetNumberOfStates(2);
            tile.SetStateLabel(0, "False");
            tile.SetStateLabel(1, "True");
            tile.AddParent(shipList[0]);
            tile.AddParent(shipList[1]);
            SetAllStatesForTiles(tile, shipList[0], shipList[1], name);
            tile.SetName($"{name}{constraintNumber++}");
            tempTileList.Add(tile);
            for (int i = 2; i < shipList.Count; i++)
            {
                tile = new LabelledDCNode(battleship);
                tile.SetNumberOfStates(2);
                tile.SetStateLabel(0, "False");
                tile.SetStateLabel(1, "True");
                tile.AddParent(shipList[i]);
                tile.AddParent(tempTileList[templistIndex]);
                SetStatesForTilesWithOnlyOneShip(tile, shipList[i], tempTileList[templistIndex++], name);
                tile.SetName($"{name}{constraintNumber++}");
                tempTileList.Add(tile);
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
        //Basically copy paste function from "MakeStatesForTiles()" 
        //it compares to previous nodes instead of a second ship
        private void SetStatesForTilesWithOnlyOneShip(LabelledDCNode tile, LabelledDCNode ship, LabelledDCNode node, string name)
        {
            int shipLength = Settings.boardWidth + 1 - ship.GetTable().GetData().Length
                                                              / (2 * Settings.boardWidth);
            int xCoord = name[0] - 'A';
            int yCoord = name[1] - '0';
            Point tilePlace = new Point(xCoord, yCoord);
            List<Point> shipPoints = new List<Point>();
            bool labelIsTrue;
            string shipName;
            ulong count = 0;
            for (ulong i = 0; i < 2; i++)
            {
                labelIsTrue = node.GetStateLabel(i) == "True";
                for (ulong j = 0; j < ship.GetNumberOfStates(); j++)
                {
                    shipName = ship.GetStateLabel(j);
                    shipPoints = ReturnCoordinates(shipLength, shipName);
                    if (labelIsTrue || shipPoints.Contains(tilePlace))
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
        private void SetAllStatesForTiles(LabelledDCNode tile, LabelledDCNode secondShip,
                                          LabelledDCNode firstShip, string name)
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
        public void DeleteDomain()
        {
            battleship.Delete();
        }
    }
}
